using Newtonsoft.Json;

namespace Shriek.Messages
{
    public class MessagePack
    {
        public MessagePack(Message message)
        {
            this.MessageType = message.GetType().AssemblyQualifiedName;
            this.Data = JsonConvert.SerializeObject(message);
        }

        public string MessageType { get; set; }

        public string Data { get; set; }
    }
}