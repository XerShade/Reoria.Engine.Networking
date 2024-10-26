using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reoria.Engine.Networking.NetListeners;

public class ClientNetListener : NetListener
{
    private NetPeer? serverPeer = null;

    public ClientNetListener(ILogger<NetListener> logger, IConfigurationRoot configuration) : base(logger, configuration)
    {
    }

    public override void Start()
    {
        _ = this.netManager.Start();
        this.serverPeer = this.netManager.Connect(this.Address, this.Port, this.ConnectionKey);
    }

    public override void Stop() => this.netManager.Stop();

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        base.OnConnectionRequest(request);
        request.RejectForce();
    }
}
