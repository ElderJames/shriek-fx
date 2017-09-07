using System;
using System.ComponentModel.DataAnnotations;

namespace Shriek.Samples.Models
{
    public class Todo
    {
        [Required]
        public Guid AggregateId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Desception { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FinishTime { get; set; }
    }
}