using LiteNetLib.Utils;
using Reoria.Engine.Networking.Packets.Interface;

namespace Reoria.Engine.Networking.Packets;

public class SessionPacket : IPacket
{
    public Guid PlayerGuid = Guid.Empty;
    public string UdpSessionKey = string.Empty;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(this.PlayerGuid.ToString());
        writer.Put(this.UdpSessionKey);
    }

    public void Deserialize(NetDataReader reader)
    {
        this.PlayerGuid = Guid.Parse(reader.GetString());
        this.UdpSessionKey = reader.GetString();
    }
}
