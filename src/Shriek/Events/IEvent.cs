namespace Shriek.Events
{
    internal interface IEvent<out TKey>
    {
        TKey AggregateId { get; }
    }
}