using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shriek;
using Shriek.Messages;

namespace Shriek.EventBus.RabbitMQ
{
    public static class RabbitMQEventBusExtensions
    {
        public static IShriekBuilder AddRabbitMqEventBus(this IShriekBuilder builder, Action<ConnectionFactory> optionAction)
        {
            var factory = new ConnectionFactory();

            optionAction(factory);

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            builder.Services.AddScoped(x => channel);

            //声明一个队列
            channel.QueueDeclare("eventQueue", false, false, false, null);

            builder.Services.AddSingleton<IMessagePublisher, RabbitMQMessageProducer>();

            return builder;
        }
    }
}