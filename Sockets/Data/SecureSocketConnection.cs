using System.Net.Security;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Sockets.Data;

public struct SecureSocketConnection
{
    public Guid Guid { get; } = Guid.NewGuid();
    public byte[] Buffer { get; set; } = new byte[4096];
    public TcpClient TcpClient { get; set; } = default!;
    public SslStream SslStream { get; set; } = default!;

    public SecureSocketConnection()
    {
        this.Guid = Guid.NewGuid();
        this.Buffer = new byte[4096];
        this.TcpClient = default!;
        this.SslStream = default!;
    }

    public SecureSocketConnection(TcpClient tcpClient, SslStream sslStream)
    {
        this.Guid = Guid.NewGuid();
        this.Buffer = new byte[4096];
        this.TcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
        this.SslStream = sslStream ?? throw new ArgumentNullException(nameof(sslStream));
    }

    public SecureSocketConnection(Guid guid, TcpClient tcpClient, SslStream sslStream)
    {
        this.Guid = guid;
        this.Buffer = new byte[4096];
        this.TcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
        this.SslStream = sslStream ?? throw new ArgumentNullException(nameof(sslStream));
    }

    public SecureSocketConnection(Guid guid, byte[] buffer, TcpClient tcpClient, SslStream sslStream)
    {
        this.Guid = guid;
        this.Buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));
        this.TcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
        this.SslStream = sslStream ?? throw new ArgumentNullException(nameof(sslStream));
    }
}
