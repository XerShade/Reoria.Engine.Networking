using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.NetListeners;
public class NetEventListener : INetEventListener
{
    public const string DEFAULT_CONNECTIONKEY = "Reoria";
    public const string DEFAULT_IPADDRESS = "127.0.0.1";
    public const int DEFAULT_PORT = 5555;

    protected readonly NetManager netManager;
    protected readonly ILogger<NetEventListener> logger;
    protected readonly IConfiguration configuration;

    public readonly string ConnectionKey;
    public readonly string Address;
    public readonly int Port;

    public NetEventListener(ILogger<NetEventListener> logger, IConfigurationRoot configuration)
    {
        this.netManager = new NetManager(this) { 
            AutoRecycle = true,
        };
        this.logger = logger;
        this.configuration = configuration;

        this.ConnectionKey = this.configuration["Networking:ConnectionKey"] ?? DEFAULT_CONNECTIONKEY;
        this.Address = this.configuration["Networking:Address"] ?? DEFAULT_IPADDRESS;
        try
        {
            this.Port = Convert.ToInt32(this.configuration["Networking:Port"]);
        }
        catch
        {
            this.Port = DEFAULT_PORT;
        }
    }

    public virtual void PollEvents() => this.netManager.PollEvents();

    public virtual void OnConnectionRequest(ConnectionRequest request) { }
    public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
    public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
    public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) { }
    public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
    public virtual void OnPeerConnected(NetPeer peer) { }
    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) { }
}
