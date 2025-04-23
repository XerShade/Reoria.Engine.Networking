using Reoria.Engine.Networking.Interfaces;

namespace Reoria.Engine.Networking.Packets.Interface;

public interface IPacketHandler<TPacket> where TPacket : IPacket
{
    void Handle(TPacket packet, INetworkContext context);
}
