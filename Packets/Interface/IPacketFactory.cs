using LiteNetLib.Utils;
using Reoria.Engine.Networking.Interfaces;

namespace Reoria.Engine.Networking.Packets.Interface;

public interface IPacketFactory
{
    void Register<TPacket>(ushort packetId) where TPacket : IPacket, new();
    void Unregister<TPacket>(ushort packetId) where TPacket : IPacket;

    IPacket CreatePacket(ushort packetId);
    ushort GetPacketId(Type packetType);
    ushort GetPacketId<TPacket>() where TPacket : IPacket;

    void HandlePacket(ushort packetId, NetDataReader reader, INetworkContext context);
}