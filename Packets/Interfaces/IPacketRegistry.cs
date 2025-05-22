using LiteNetLib.Utils;
using Reoria.Engine.Networking.Managers.Interfaces;

namespace Reoria.Engine.Networking.Packets.Interfaces;

public interface IPacketRegistry
{
    Task AttachNetworkManager(INetworkManager networkManager);
    void HandleIncomingData(byte[] data);
    void HandleIncomingData(NetDataReader reader);
    NetDataWriter HandleOutgoingData(IPacket packet, NetDataWriter writer);
    NetDataWriter HandleOutgoingData(Type packetType);
    NetDataWriter HandleOutgoingData<TPacket>() where TPacket : IPacket;
}
