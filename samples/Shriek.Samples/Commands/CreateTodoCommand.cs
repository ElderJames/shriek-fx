using Shriek.Commands;
using System;

namespace Shriek.Samples.Commands
{
    public class CreateTodoCommand : Command<Guid>
    {
        public CreateTodoCommand(Guid id) : base(id)
        {
        }

        public string Name { get; set; }

        public string Desception { get; set; }
        public DateTime FinishTime { get; set; }
    }
}