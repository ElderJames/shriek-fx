namespace Shriek.Messages
{
    public interface IMessageSubscriber<in TMessage> where TMessage : Message
    {
        void Execute(TMessage e);
    }
}