using Reoria.Engine.Networking.Interfaces;

namespace Reoria.Engine.Networking.Packets.Interface;

public interface IPacketSender
{
    void Send<TPacket>(TPacket packet, INetworkContext context) where TPacket : IPacket;
}
