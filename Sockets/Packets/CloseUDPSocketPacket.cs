using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Packets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Packets;

public class CloseUDPSocketPacket(ILogger<CloseUDPSocketPacket> logger) : IPacket
{
    protected readonly ILogger<CloseUDPSocketPacket> Logger = logger;

    public string PacketIdentifier => "Sockets.CloseUDPSocketPacket";

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {
        
    }
}
