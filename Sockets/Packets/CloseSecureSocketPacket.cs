using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Packets.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Packets;

public class CloseSecureSocketPacket(ILogger<CloseSecureSocketPacket> logger, ISocketCancellationRequest cancellationRequest) : IPacket
{
    protected readonly ILogger<CloseSecureSocketPacket> Logger = logger;
    protected readonly ISocketCancellationRequest CancellationRequest = cancellationRequest;

    public string PacketIdentifier => "Sockets.CloseSecureSocketPacket";

    public void Deserialize(NetDataReader reader)
    {
        string message = reader.GetString();

        _ = this.CancellationRequest.DisconnectAsync();

        this.Logger.LogInformation("Secure socket was closed by remote host, reason: {message}", message);
    }

    public void Serialize(NetDataWriter writer) 
        => writer.Put("Connection has been closed by the server.");
}
