namespace Reoria.Engine.Networking.Sockets.Interfaces;

public interface ISecureSocket
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync();
    Task SendAsync(Guid connectionId, byte[] data);
    Task ConnectAsync(CancellationToken cancellationToken = default);
    bool IsConnectedToServer();

    event Func<Guid, byte[], Task> OnMessageReceived;
    event Func<Guid, Task> OnClientDisconnected;
}
