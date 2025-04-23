using LiteNetLib.Utils;

namespace Reoria.Engine.Networking.Packets.Interface;

public interface IPacket
{
    void Serialize(NetDataWriter writer);
    void Deserialize(NetDataReader reader);
}
