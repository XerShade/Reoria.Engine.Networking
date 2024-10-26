using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.NetListeners;

public class ServerNetEventListener(ILogger<NetEventListener> logger, IConfigurationRoot configuration) : NetEventListener(logger, configuration)
{
    public virtual void Start() => this.netManager.Start(this.Port);
    public virtual void Stop() => this.netManager.Stop();
    public virtual int GetLocalPort() => this.netManager.LocalPort;

    public override void OnPeerConnected(NetPeer peer)
    {
        this.logger.LogInformation("Received new client connection from {Address}.", peer.Address);

        base.OnPeerConnected(peer);
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        this.logger.LogInformation("Lost connection from {Address}", peer.Address);

        base.OnPeerDisconnected(peer, disconnectInfo);
    }

    public override void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        string command = reader.GetString().ToUpper();
        this.logger.LogInformation("Received packet {command} from {ipaddress}.", command, peer.Address.ToString());

        base.OnNetworkReceive(peer, reader, channelNumber, deliveryMethod);
    }

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        _ = request.AcceptIfKey(this.ConnectionKey);
        base.OnConnectionRequest(request);
    }

    public override void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        this.logger.LogInformation("An error has occured on connection from {endPoint}: {socketError}", endPoint, socketError);

        base.OnNetworkError(endPoint, socketError);
    }

    public virtual void BroadcastTo(NetPeer peer, NetDataWriter writer)
    {
        if (this.netManager.IsRunning)
        {
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }

    public virtual void BroadcastToAllBut(NetPeer excludePeer, NetDataWriter writer)
    {
        foreach (NetPeer peer in this.netManager.ConnectedPeerList)
        {
            if (peer.Id != excludePeer.Id)
            {
                this.BroadcastTo(peer, writer);
            }
        }
    }

    public virtual void BroadcastToAll(NetDataWriter writer)
    {
        foreach (NetPeer peer in this.netManager.ConnectedPeerList)
        {
            this.BroadcastTo(peer, writer);
        }
    }
}
