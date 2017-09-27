using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shriek.Messages;

namespace Shriek.EventBus.RabbitMQ
{
    public class RabbitMQMessageProducer : IMessagePublisher
    {
        private IModel channel;
        private IServiceProvider container;

        public RabbitMQMessageProducer(IModel channel, IServiceProvider container)
        {
            this.channel = channel;

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body);
                var msg = JsonConvert.DeserializeObject<Message>(json);
                var message = JsonConvert.DeserializeObject(json, Type.GetType(msg.MessageType));

                var subscribers = container.GetServices(typeof(IMessageSubscriber<>).MakeGenericType(message.GetType()));

                foreach (var sub in subscribers)
                {
                    ((dynamic)sub).Execute((dynamic)message);
                }

                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
                //
            };

            //启动消费者 设置为手动应答消息
            channel.BasicConsume("eventQueue", false, consumer);
        }

        public void Dispose()
        {
            channel.Dispose();
        }

        public void Send<TMessage>(TMessage message) where TMessage : Message
        {
            var msg = JsonConvert.SerializeObject(message);
            var sendBytes = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish("", "eventQueue", null, sendBytes);
        }
    }
}