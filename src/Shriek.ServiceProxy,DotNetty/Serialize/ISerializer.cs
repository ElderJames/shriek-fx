namespace Shriek.ServiceProxy.DotNetty.Serialize
{
	public interface ISerializer
	{
		/// <summary>
		/// serialize a object to byte array ,for netty client send message
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		byte[] Serialize(object target);

		/// <summary>
		/// deserialize byte array received by servier side
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		object Deserialize(byte[] buffer);
	}
}