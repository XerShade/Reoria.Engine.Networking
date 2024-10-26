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

    public bool IsServer { get; protected set; } = false;

    public NetEventListener(ILogger<NetEventListener> logger, IConfigurationRoot configuration)
    {
        this.netManager = new NetManager(this) { 
            AutoRecycle = true,
        };
        this.OnConfigureNetManager(this.netManager);

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

    public virtual void PollEvents()
    {
        if (this.netManager is null)
        { return; }

        if (this.netManager.IsRunning)
        {
            this.netManager.PollEvents();
        }
    }
    public virtual void Start() { }
    public virtual void Stop() { }
    public virtual void OnConfigureNetManager(NetManager netManager) { }

    public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        if (this.IsServer)
        {
            this.logger.LogInformation("An error has occured on connection from {endPoint}: {socketError}", endPoint, socketError);
        }
        else
        {
            this.logger.LogInformation("A network error has occured: {socketError}", socketError);
        }
    }
    public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        string command = reader.GetString().ToUpper();
        this.logger.LogInformation("Received packet {command} from {ipaddress}.", command, peer.Address.ToString());
    }
    public virtual void OnConnectionRequest(ConnectionRequest request) => this.logger.LogInformation("Received connection request from {Address}...", request.RemoteEndPoint.ToString());
    public virtual void OnPeerConnected(NetPeer peer) => this.logger.LogInformation("Received new connection from {Address}.", peer.Address);
    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) => this.logger.LogInformation("Lost connection from {Address}, reason: {Reason}", peer.Address, disconnectInfo.Reason.ToString());
    public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
    public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
}
