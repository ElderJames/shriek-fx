namespace Shriek.ServiceProxy.Tcp.Protocol
{
    public class Message
    {
        public readonly int Id;

        public readonly MessageType MessageType;

        public readonly string Contract;

        public readonly string Operation;

        public readonly byte[][] Parameters;

        public readonly bool HasParameters;

        public Message(MessageType msgType, int id, object[] parameters)
        {
            this.MessageType = msgType;
            this.Id = id;
            if (parameters != null)
            {
                var length = parameters.Length;
                if (length > 0)
                {
                    this.HasParameters = true;
                    this.Parameters = new byte[length][];
                    for (int i = 0; i < length; i++)
                    {
                        var p = parameters[i];
                        if (p is byte[])
                            this.Parameters[i] = (byte[])p;
                        else
                            this.Parameters[i] = Global.Serializer.Serialize(p);
                    }
                }
            }
            this.Contract = string.Empty;
            this.Operation = string.Empty;
        }

        public Message(MessageType msgType, int id, object parameter)
        {
            this.MessageType = msgType;
            this.Id = id;
            this.HasParameters = true;
            if (parameter is byte[])
            {
                this.Parameters = new byte[1][] { (byte[])parameter };
            }
            else
            {
                this.Parameters = new byte[1][] { Global.Serializer.Serialize(parameter) };
            }
            this.Contract = string.Empty;
            this.Operation = string.Empty;
        }

        public Message(MessageType msgType, int id, string contract, string operation, object[] parameters)
            : this(msgType, id, parameters)
        {
            this.Contract = contract;
            this.Operation = operation;
        }

        public Message(MessageType msgType, int id, string contract, string operation, object parameter)
            : this(msgType, id, parameter)
        {
            this.Contract = contract;
            this.Operation = operation;
        }
    }
}