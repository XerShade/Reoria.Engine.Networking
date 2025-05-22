using Reoria.Engine.Networking.Packets.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Managers.Interfaces;

public interface INetworkManager
{
    ISecureSocket SecureSocket { get; init; }
    IPacketRegistry PacketRegistry { get; init; }
}
