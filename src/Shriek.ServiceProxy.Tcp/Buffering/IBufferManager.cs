namespace Shriek.ServiceProxy.Tcp.Buffering
{
    public interface IBufferManager
    {
        byte[] GetFitBuffer(int size);

        void AddBuffer(byte[] buffer);
    }
}