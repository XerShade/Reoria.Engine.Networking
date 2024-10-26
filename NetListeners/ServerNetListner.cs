using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.NetListeners.Interfaces;

namespace Reoria.Engine.Networking.NetListeners;

public class ServerNetListener : NetListener, IServerNetListener
{
    public ServerNetListener(ILogger<NetListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {
        this.IsServer = true;
    }

    public override void Start() => this.netManager.Start(this.Port);
    public override void Stop() => this.netManager.Stop();

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        base.OnConnectionRequest(request);
        _ = request.AcceptIfKey(this.ConnectionKey);
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
