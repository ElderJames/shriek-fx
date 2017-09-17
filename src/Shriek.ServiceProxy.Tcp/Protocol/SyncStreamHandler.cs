namespace Shriek.ServiceProxy.Tcp.Protocol
{
    //abstract class SyncStreamHandler : StreamHandler
    //{
    //    public override bool CanRead
    //    {
    //        get { return this.Socket.Connected; }
    //    }

    //    readonly Socket Socket;

    //    public SyncStreamHandler(TcpClient client)
    //        : base(client)
    //    {
    //        this.Socket = client.Client;
    //    }

    //    protected override Task<int> ActualRead(byte[] buffer, int offset, int length)
    //    {
    //        var read = this.Socket.Receive(buffer, offset, length, SocketFlags.None);
    //        return Task.FromResult(read);
    //    }

    //    protected override Task ActualWrite(byte[] buffer, int offset, int length)
    //    {
    //        this.Socket.Send(buffer, offset, length, SocketFlags.None);
    //        return Task.CompletedTask;
    //    }
    //}
}