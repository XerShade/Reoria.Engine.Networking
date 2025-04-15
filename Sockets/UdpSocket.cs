using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Sockets;

public abstract class UdpSocket : ISocket
{
    protected readonly ILogger<ISocket> Logger;
    protected readonly int Port;
    protected readonly NetManager Manager;

    protected UdpSocket(ILogger<ISocket> logger, IConfiguration configuration)
    {
        this.Logger = logger;
        this.Port = Convert.ToInt32(configuration["Networking:Port"] ?? this.GetDefaultPort());
        this.Manager = new(this);

        this.Logger.LogInformation("Created udp socket with '{SocketType}'.", this.GetType().Name);
    }

    protected virtual string GetDefaultPort()
        => "7234";

    public virtual void Start()
    {
        if (!this.Manager.IsRunning)
        {
            this.OnStart(this.Manager);
            this.Logger.LogInformation("Started listening on port '{Port}' with '{ListenerType}'.", this.Manager.LocalPort, this.GetType().Name);
        }
    }

    protected virtual void OnStart(NetManager netManager)
        => netManager.Start();

    public virtual void Stop()
    {
        if (this.Manager.IsRunning)
        {
            int port = this.Manager.LocalPort;

            this.OnStop(this.Manager);
            this.Logger.LogInformation("Stopped listening on port '{Port}' with '{ListenerType}'.", this.GetType().Name, port);
        }
    }

    protected virtual void OnStop(NetManager netManager)
        => netManager.Stop(true);

    public virtual void PollEvents()
    {
        if (this.Manager.IsRunning)
        {
            this.OnPollEvents(this.Manager);
        }
    }

    protected virtual void OnPollEvents(NetManager netManager)
        => netManager.PollEvents();

    public virtual bool ConnectToServer()
        => false;

    public virtual bool IsConnectedToServer()
        => false;

    public virtual void OnConnectionRequest(ConnectionRequest request)
        => request.Accept();

    public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        => this.Logger.LogError("A socket error from {PeerIP}:{PeerPort} has occured: {SocketError}", endPoint.Address, endPoint.Port, socketError);

    public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

    public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        => this.Logger.LogWarning("Recieved message '{Message}' via {DeliveryMethod} from peer '{PeerID}' at {PeerIP}:{PeerPort}.", reader.GetString(), deliveryMethod, peer.Id, peer.Address, peer.Port);

    public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        => this.Logger.LogWarning("Recieved message type {MessageType} from unkown peer at {PeerIP}:{PeerPort}.", messageType, remoteEndPoint.Address, remoteEndPoint.Port);

    public virtual void OnPeerConnected(NetPeer peer)
        => this.Logger.LogInformation("Network peer '{PeerID}' has connected from {PeerIP}:{PeerPort}.", peer.Id, peer.Address, peer.Port);

    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        => this.Logger.LogInformation("Network peer '{PeerID}' has disconnected from {PeerIP}:{PeerPort}.\n    - Reason: {reason}", peer.Id, peer.Address, peer.Port, disconnectInfo.Reason);
}
