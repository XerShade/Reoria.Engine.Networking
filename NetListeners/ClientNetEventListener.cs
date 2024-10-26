using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.NetListeners;

public class ClientNetEventListener : NetEventListener
{
    private NetPeer? serverPeer = null;

    public ClientNetEventListener(ILogger<NetEventListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {

    }

    public virtual void Start()
    {
        _ = this.netManager.Start();
        this.serverPeer = this.netManager.Connect(this.Address, this.Port, this.ConnectionKey);
    }

    public virtual void Stop() => this.netManager.Stop();

    public override void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        this.logger.LogInformation("A network error has occured: {socketError}", socketError);

        base.OnNetworkError(endPoint, socketError);
    }

    public override void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        string command = reader.GetString().ToUpper();
        this.logger.LogInformation("Received packet {command} from the server.", command);

        base.OnNetworkReceive(peer, reader, channelNumber, deliveryMethod);
    }
}
