using System;

namespace Shriek.Messages
{
    public class Message
    {
        public Message()
        {
            this.MessageType = this.GetType().AssemblyQualifiedName;
        }

        public string MessageType { get; set; }

        public DateTime Timestamp { get; set; }
    }
}