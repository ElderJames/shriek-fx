namespace Shriek.ServiceProxy.Tcp.Buffering
{
    public class DummyBufferManager : IBufferManager
    {
        public void AddBuffer(byte[] buffer)
        {
            //do nothing
        }

        public byte[] GetFitBuffer(int size)
        {
            return new byte[size];
        }
    }
}