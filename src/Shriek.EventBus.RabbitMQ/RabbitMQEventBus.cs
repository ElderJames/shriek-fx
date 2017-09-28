using Shriek.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shriek.Messages;

namespace Shriek.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private IServiceProvider container;
        private IModel channel;

        public RabbitMQEventBus(IServiceProvider container, IModel channel)
        {
            this.container = container;
            this.channel = channel;

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body);
                var o = JObject.Parse(json);
                var message = o.ToObject(Type.GetType(o["MessageType"].Value<string>()));

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

        public void Publish<T>(T @event) where T : Event
        {
            var msg = JsonConvert.SerializeObject(@event);
            var sendBytes = Encoding.UTF8.GetBytes(msg);

            channel.BasicPublish("", "eventQueue", null, sendBytes);
        }
    }
}