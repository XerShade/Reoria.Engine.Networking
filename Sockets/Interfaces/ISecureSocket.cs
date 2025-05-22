using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Packets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Interfaces;

public interface ISecureSocket
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync();
    Task SendAsync(Guid connectionId, byte[] data);
    Task ConnectAsync(CancellationToken cancellationToken = default);
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    bool IsConnectedToServer();
    Task SendAsync<TPacket>(Guid connectionId) where TPacket : IPacket;
    Task SendAsync(Guid connectionId, Type packetType);
    Task AttachNetworkManager(INetworkManager networkManager);
    Task ForceDisconnectAsync(CancellationToken cancellationToken = default);

    event Func<Guid, byte[], Task> OnMessageReceived;
    event Func<Guid, Task> OnClientDisconnected;
    event Func<Guid, Task> OnClientConnected;
}
