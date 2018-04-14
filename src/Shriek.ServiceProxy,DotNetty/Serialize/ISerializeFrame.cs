using DotNetty.Transport.Channels;

namespace Shriek.ServiceProxy.DotNetty.Serialize
{
	public interface ISerializeFrame
	{
		void Select(ISerializer protocol, IChannelPipeline pipeline);
	}
}