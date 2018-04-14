using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shriek.ServiceProxy.DotNetty.Serialize
{
	public class DefaultSerializer : ISerializer
	{
		public byte[] Serialize(object target)
		{
			if (target == null)
				return null;
			var bf = new BinaryFormatter();
			var ms = new MemoryStream();
			bf.Serialize(ms, target);
			return ms.ToArray();
		}

		public object Deserialize(byte[] buffer)
		{
			var memStream = new MemoryStream();
			var binForm = new BinaryFormatter();
			memStream.Write(buffer, 0, buffer.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			object obj = binForm.Deserialize(memStream);
			return obj;
		}
	}
}