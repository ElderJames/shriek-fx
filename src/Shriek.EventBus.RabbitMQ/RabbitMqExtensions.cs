using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shriek.Commands;
using Shriek.Events;
using System;
using System.Text;

namespace Shriek.Messages.RabbitMQ
{
    public static class RabbitMqExtensions
    {
        public static void UseRabbitMqEventBus(this ShriekOptionBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            builder.AddRabbitMQSubscriber(optionAction);
            builder.Services.AddTransient<IEventBus, RabbitMqEventBus>();
        }

        public static void UseRabbitMqCommandBus(this ShriekOptionBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            builder.AddRabbitMQSubscriber(optionAction);
            builder.Services.AddTransient<ICommandBus, RabbitMqCommandBus>();
        }

        public static void AddRabbitMQSubscriber(this ShriekOptionBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            var option = new RabbitMqOptions();
            optionAction(option);

            var factory = new ConnectionFactory()
            {
                HostName = option.HostName,
                UserName = option.UserName,
                Password = option.Password
            };

            builder.Services.AddSingleton(r =>
            {
                option.ServiceProvider = r;
                return option;
            });

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            //声明一个队列 (durable=true 持久化消息）
            channel.QueueDeclare(option.QueueName, true, false, false, null);

            channel.BasicQos(0, 1, false);

            if (!string.IsNullOrEmpty(option.ExchangeName))
            {
                channel.ExchangeDeclare(option.ExchangeName, option.ExchangeType, true, false, null);

                //将队列绑定到交换机
                channel.QueueBind(option.QueueName, option.ExchangeName, option.RouteKey, null);
            }

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //启动消费者 设置为手动应答消息
            channel.BasicConsume(option.QueueName, false, consumer);

            //接收到消息事件
            consumer.Received += (sender, args) =>
            {
                var response = string.Empty;

                var body = args.Body;
                var props = args.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var json = Encoding.UTF8.GetString(body);
                    var o = JObject.Parse(json);
                    var messageType = Type.GetType(o[nameof(Message.MessageType)].Value<string>());
                    dynamic message = o.ToObject(messageType);

                    var subscribers = option.ServiceProvider.GetServices(typeof(IMessageSubscriber<>).MakeGenericType(messageType));

                    foreach (var sub in subscribers)
                    {
                        ((dynamic)sub).Execute(message);
                    }
                }
                catch (Exception ex)
                {
                    response = ex.Message;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);

                    channel.BasicPublish(option.ExchangeName, props.ReplyTo, replyProps, responseBytes);

                    //确认该消息已被消费
                    channel.BasicAck(args.DeliveryTag, false);
                }
            };
        }

        public static void AddRabbitMQPublisher(this ShriekOptionBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            var option = new RabbitMqOptions();
            optionAction(option);

            var factory = new ConnectionFactory()
            {
                HostName = option.HostName,
                UserName = option.UserName,
                Password = option.Password
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            var props = channel.CreateBasicProperties();

            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;

            option.Publisher = channel;

            builder.Services.AddScoped<IMessagePublisher>(r => new RabbitMqMessagePublisher(r, option));
        }
    }
}