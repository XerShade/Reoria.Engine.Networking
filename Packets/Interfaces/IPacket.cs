using LiteNetLib.Utils;

namespace Reoria.Engine.Networking.Packets.Interfaces;

public interface IPacket
{
    string PacketIdentifier { get; }
    void Deserialize(NetDataReader reader);
    void Serialize(NetDataWriter writer);
}
