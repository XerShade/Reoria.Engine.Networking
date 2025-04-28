
namespace Reoria.Engine.Networking.Packets.Interfaces;

public interface IPacketFactory
{
    IPacket CreatePacket(Type packetType);
    TPacket CreatePacket<TPacket>() where TPacket : IPacket;
}
