using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public class SocketCancellationRequest : ISocketCancellationRequest
{
    protected readonly Lock @lock = new();
    public bool IsRequested { get; protected set; }

    public virtual Task DisconnectAsync(CancellationToken cancellationToken = default!)
    {
        lock(this.@lock)
        {
            this.IsRequested = true;
        }

        return Task.CompletedTask;
    }

    public virtual Task ProcessAsync(CancellationToken cancellationToken = default!)
    {
        lock (this.@lock)
        {
            this.IsRequested = false;
        }

        return Task.CompletedTask;
    }
}
