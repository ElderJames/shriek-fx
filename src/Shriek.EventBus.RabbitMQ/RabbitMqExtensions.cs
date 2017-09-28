using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shriek.Commands;
using Shriek.Events;

namespace Shriek.Messages.RabbitMQ
{
    public static class RabbitMqExtensions
    {
        public static IShriekBuilder AddRabbitMqEventBus(this IShriekBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            var option = new EventBusRabbitMqOptions();
            optionAction(option);

            AddRabbitMq(builder, option);

            builder.Services.AddTransient<IEventBus, RabbitMqEventBus>();

            return builder;
        }

        public static IShriekBuilder AddRabbitMqCommandBus(this IShriekBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            var option = new CommandBusRabbitMqOptions();
            optionAction(option);

            AddRabbitMq(builder, option);

            builder.Services.AddTransient<ICommandBus, RabbitMqCommandBus>();

            return builder;
        }

        private static void AddRabbitMq(IShriekBuilder builder, RabbitMqOptions option)
        {
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

            //声明一个队列
            channel.QueueDeclare(option.QueueName, false, false, false, null);

            if (!string.IsNullOrEmpty(option.ExchangeName))
            {
                channel.ExchangeDeclare(option.ExchangeName, option.ExchangeType, false, false, null);

                //将队列绑定到交换机
                channel.QueueBind(option.QueueName, option.ExchangeName, option.RouteKey, null);
            }

            option.Channel = channel;

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (sender, args) =>
            {
                var json = Encoding.UTF8.GetString(args.Body);
                var o = JObject.Parse(json);
                var message = o.ToObject(Type.GetType(o[nameof(Message.MessageType)].Value<string>()));

                try
                {
                    option.MessagePublisher.Send((dynamic)message);

                    //确认该消息已被消费
                    channel.BasicAck(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            };

            //启动消费者 设置为手动应答消息
            channel.BasicConsume(option.QueueName, false, consumer);

            builder.Services.AddSingleton(option.GetType(), x => option);
        }
    }
}