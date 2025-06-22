

namespace Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

public interface ISecureServerSocket : ISecureSocketBase
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync();
}
