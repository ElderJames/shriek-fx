namespace Shriek.Messages
{
    public interface IMessageSubscriber<TMessage> where TMessage : Message
    {
        void Execute(TMessage e);
    }
}