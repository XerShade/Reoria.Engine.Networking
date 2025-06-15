
namespace Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

public interface ISecureClientSocket : ISecureSocketBase
{
    string IPAddress { get; }

    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    bool IsConnectedToServer();
}
