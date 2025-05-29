using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Net.Security;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Sessions;

public class SecureSession : Session, ISecureSession
{
    protected TcpClient? tcpClient;
    protected SslStream? sslStream;

    public TcpClient TcpClient { get => tcpClient ?? throw new NullReferenceException(); protected set => tcpClient = value; }
    public SslStream SslStream { get => sslStream ?? throw new NullReferenceException(); protected set => sslStream = value; }

    public virtual ISession AssignTcpClient(TcpClient tcpClient)
    {
        this.TcpClient = tcpClient;
        return this;
    }

    public virtual ISession AssignSslStream(SslStream sslStream)
    {
        this.SslStream = sslStream;
        return this;
    }

    public override ISession Close()
    {
        this.sslStream?.Close();
        this.sslStream = null;

        this.tcpClient?.Close();
        this.tcpClient = null;

        return this;
    }
}
