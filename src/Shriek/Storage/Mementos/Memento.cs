using System;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Storage.Mementos
{
    public class Memento<TKey> : IMemento<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        public int Id { get; set; }

        public TKey AggregateId { get; set; }

        public int Version { get; set; }

        public string Data { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public interface IMemento
    {
    }

    public interface IMemento<TKey> : IMemento where TKey : IEquatable<TKey>
    {
        TKey AggregateId { get; set; }
    }
}