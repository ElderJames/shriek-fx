namespace Shriek.Events
{
    public interface IEventHandler<in TEvent> where TEvent : Event
    {
        void Handle(TEvent e);
    }
}