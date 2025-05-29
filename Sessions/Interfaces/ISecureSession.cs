using System.Net.Security;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Sessions.Interfaces;

public interface ISecureSession : ISession
{
    TcpClient TcpClient { get; }
    SslStream SslStream { get; }

    ISession AssignSslStream(SslStream sslStream);
    ISession AssignTcpClient(TcpClient tcpClient);
}
