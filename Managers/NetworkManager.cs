using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Packets.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Managers;

public class NetworkManager : INetworkManager
{
    protected readonly ILogger<INetworkManager> Logger;
    public ISecureSocket SecureSocket { get; init; }
    public IPacketRegistry PacketRegistry { get; init; }

    public NetworkManager(ILogger<INetworkManager> logger, ISecureSocket secureSocket, IPacketRegistry packetRegistry)
    {
        this.Logger = logger;
        this.SecureSocket = secureSocket;
        this.PacketRegistry = packetRegistry;

        _ = this.SecureSocket.AttachNetworkManager(this);
        _ = this.PacketRegistry.AttachNetworkManager(this);
    }
}
