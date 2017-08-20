using Shriek.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.ConfigCenter.Domain.Commands
{
    public class CreateConfigItemCommand : Command<Guid>
    {
        public CreateConfigItemCommand(Guid id, int version = 0) : base(id, version)
        {
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}