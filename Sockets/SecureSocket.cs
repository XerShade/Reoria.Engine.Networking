using Reoria.Engine.Networking.Sockets.Data;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SecureSocket : ISecureSocket
{
    public event Func<Guid, byte[], Task> OnMessageReceived = default!;
    public event Func<Guid, Task> OnClientDisconnected = default!;

    protected virtual Task InvokeOnClientDisconnected(Guid guid)
        => this.OnClientDisconnected?.Invoke(guid) ?? Task.CompletedTask;

    public virtual Task StartAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public virtual Task StopAsync()
        => Task.CompletedTask;

    public virtual Task SendAsync(Guid connectionId, byte[] data)
        => Task.CompletedTask;

    public virtual Task ConnectAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public virtual bool IsConnectedToServer()
        => false;

    protected virtual async Task ReadStreamBuffer(SecureSocketConnection connection, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            int bytesRead = await connection.SslStream.ReadAsync(connection.Buffer, cancellationToken);

            if (bytesRead <= 0)
            {
                break;
            }

            byte[] data = connection.Buffer.Take(bytesRead).ToArray();

            await (this.OnMessageReceived?.Invoke(connection.Guid, data) ?? Task.CompletedTask);
        }
    }
}
