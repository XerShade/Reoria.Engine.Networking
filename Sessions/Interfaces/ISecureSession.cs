using System.Net.Security;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Sessions.Interfaces;

public interface ISecureSession : ISession
{
    TcpClient TcpClient { get; }
    SslStream SslStream { get; }
    bool IsTcpClientNull { get; }
    bool IsSslStreamNull { get; }

    ISecureSession AssignSslStream(SslStream sslStream);
    ISecureSession AssignTcpClient(TcpClient tcpClient);
}
