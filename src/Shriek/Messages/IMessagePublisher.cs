using System;

namespace Shriek.Messages
{
    public interface IMessagePublisher : IDisposable
    {
        void Send<TMessage>(TMessage message) where TMessage : Message;
    }
}