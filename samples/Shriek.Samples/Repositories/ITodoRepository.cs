using Shriek.Samples.Aggregates;

namespace Shriek.Samples.Repositories
{
    public interface ITodoRepository
    {
        bool Create(TodoAggregateRoot root);

        bool Change(TodoAggregateRoot root);

        bool Delete(TodoAggregateRoot root);

        int Save();
    }
}