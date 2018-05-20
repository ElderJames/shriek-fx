using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
            builder.AddRabbitMQ(optionAction);
            builder.Services.AddTransient<IEventBus, RabbitMqEventBus>();
        }

        public static void UseRabbitMqCommandBus(this ShriekOptionBuilder builder, Action<RabbitMqOptions> optionAction)
        {
            builder.AddRabbitMQ(optionAction);
            builder.Services.AddTransient<ICommandBus, RabbitMqCommandBus>();
        }

        public static void AddRabbitMQ(this ShriekOptionBuilder builder, Action<RabbitMqOptions> optionAction)
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

            //声明一个队列 (durable=true 持久化消息）
            channel.QueueDeclare(option.QueueName, true, false, false, null);

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
                var msgPackJson = Encoding.UTF8.GetString(args.Body);
                var msgPack = JsonConvert.DeserializeObject<MessagePack>(msgPackJson);
                var o = JObject.Parse(msgPack.Data);
                var messageType = Type.GetType(msgPack.MessageType);
                dynamic message = o.ToObject(messageType);

                try
                {
                    var subscribers = option.ServiceProvider.GetServices(typeof(IMessageSubscriber<>).MakeGenericType(messageType));

                    foreach (var sub in subscribers)
                    {
                        ((dynamic)sub).Execute((dynamic)message);
                    }

                    //确认该消息已被消费
                    channel.BasicAck(args.DeliveryTag, false);
                }
                catch
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