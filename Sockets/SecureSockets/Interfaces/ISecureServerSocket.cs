

namespace Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

public interface ISecureServerSocket : ISecureSocketBase
{
    event Func<Guid, Task> OnClientConnected;
    event Func<Guid, Task> OnClientDisconnected;

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync();
}
