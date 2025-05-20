using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Packets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Packets;

public class OpenUDPSocketPacket(ILogger<OpenUDPSocketPacket> logger) : IPacket
{
    protected readonly ILogger<OpenUDPSocketPacket> Logger = logger;

    public string PacketIdentifier => "Sockets.OpenUDPSocketPacket";

    public void Deserialize(NetDataReader reader)
    {

    }

    public void Serialize(NetDataWriter writer)
    {

    }
}
