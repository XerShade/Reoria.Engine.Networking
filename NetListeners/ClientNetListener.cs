using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.NetListeners.Interfaces;

namespace Reoria.Engine.Networking.NetListeners;

public class ClientNetListener : NetListener, IClientNetListener
{
    private NetPeer? serverPeer = null;
    public ConnectionState ConnectionState => this.serverPeer?.ConnectionState ?? ConnectionState.Disconnected;

    public ClientNetListener(ILogger<NetListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {
    }

    public virtual async Task<bool> TryConnectToServer()
    {
        if (this.ConnectionState == ConnectionState.Outgoing)
        { return false; }

        if (this.ConnectionState == ConnectionState.Connected)
        { return true; }

        this.serverPeer = this.netManager.Connect(this.Address, this.Port, this.ConnectionKey);

        if (this.serverPeer != null)
        {
            while (this.ConnectionState == ConnectionState.Outgoing)
            {
                await Task.Delay(1);
            }

            if (this.ConnectionState == ConnectionState.Connected)
            {
                return true;
            }
        }

        this.serverPeer = null;
        return false;
    }

    public override void Start() => _ = this.netManager.Start();
    public override void Stop() => this.netManager.Stop();
    public override void OnConnectionRequest(ConnectionRequest request)
    {
        base.OnConnectionRequest(request);
        request.RejectForce();
    }
    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        if (this.ConnectionState != ConnectionState.Connected)
        {
            this.serverPeer = null;
        }
        base.OnPeerDisconnected(peer, disconnectInfo);
    }
}
