namespace Shriek.Events
{
    internal interface IEvent<TAggregateId>
    {
        TAggregateId AggregateId { get; }
    }
}