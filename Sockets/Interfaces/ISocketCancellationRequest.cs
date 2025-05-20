
namespace Reoria.Engine.Networking.Sockets.Interfaces;

public interface ISocketCancellationRequest
{
    bool IsRequested { get; }

    Task DisconnectAsync(CancellationToken cancellationToken = default!);
    Task ProcessAsync(CancellationToken cancellationToken = default!);
}
