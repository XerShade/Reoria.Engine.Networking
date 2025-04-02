using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Common.Security.Encryption.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Reoria.Engine.Networking.Listeners;

public abstract class NetworkListener : INetEventListener
{
    protected readonly string ConnectionKey;
    protected readonly NetManager Manager;
    protected readonly ILogger<INetEventListener> Logger;
    protected readonly IConfiguration Configuration;
    protected readonly IEncryptionService EncryptionService;

    public NetworkListener(ILogger<INetEventListener> logger, IConfiguration configuration, IEncryptionService encryptionService)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.EncryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        this.ConnectionKey = this.Configuration["Networking:ConnectionKey"] ?? this.GetDefaultConnectionKey();
        this.Manager = new(this);

        this.Logger.LogInformation("Created '{ListenerType}' with connection key '{ConnectionKey}'.", this.GetType().Name, this.ConnectionKey);
    }

    protected virtual string GetDefaultConnectionKey() 
        => $"Reoria-{Assembly.GetExecutingAssembly().GetName().Version}";

    public virtual void Start()
    {
        if (!this.Manager.IsRunning)
        {
            _ = this.Manager.Start();
            this.Logger.LogInformation("Started listening on port '{Port}' with '{ListenerType}'.", this.Manager.LocalPort, this.GetType().Name);
        }
    }

    public virtual void Stop()
    {
        if (this.Manager.IsRunning)
        {
            int port = this.Manager.LocalPort;

            this.Manager.Stop(true);
            this.Logger.LogInformation("Stopped listening on port '{Port}' with '{ListenerType}'.", this.GetType().Name, port);
        }
    }

    public virtual void PollEvents()
    {
        if (this.Manager.IsRunning)
        {
            this.Manager.PollEvents();
        }
    }

    public virtual void OnConnectionRequest(ConnectionRequest request)
        => request.AcceptIfKey(this.ConnectionKey);

    public virtual void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        => this.Logger.LogError("A socket error from {PeerIP}:{PeerPort} has occured: {SocketError}", endPoint.Address, endPoint.Port, socketError);

    public virtual void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

    public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        string packetKey = reader.GetString();
        switch (packetKey)
        {
            case "AES_KEYS":
                byte[] aes_key = Convert.FromBase64String(reader.GetString());
                byte[] aes_iv = Convert.FromBase64String(reader.GetString());

                this.EncryptionService.ChangeKey(aes_key, aes_iv);
                this.Logger.LogWarning("AES Encryption Key and IV have been changed! \n" +
                    "    - Key: {key} \n" +
                    "    - IV: {iv}", Convert.ToBase64String(this.EncryptionService.AesKey), Convert.ToBase64String(this.EncryptionService.AesIV));
                this.SendSecretMessage();
                break;
            default:
                byte[] data = reader.GetRemainingBytes();

                if (data.Length > 0)
                {
                    NetDataReader decryptedReader = new(this.EncryptionService.Decrypt(data));

                    if (decryptedReader.AvailableBytes > 0)
                    {
                        string decryptedMessage = decryptedReader.GetString();

                        this.Logger.LogInformation("Received message '{Message}' via {DeliveryMethod} from peer '{PeerID}' at {PeerIP}:{PeerPort}.", decryptedMessage, deliveryMethod, peer.Id, peer.Address, peer.Port);
                    }
                }
                break;

        }
    }

    protected virtual void SendSecretMessage() { }

    public virtual void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        => this.Logger.LogWarning("Recieved message type {MessageType} from unkown peer at {PeerIP}:{PeerPort}.", messageType, remoteEndPoint.Address, remoteEndPoint.Port);
    public virtual void OnPeerConnected(NetPeer peer)
        => this.Logger.LogInformation("Network peer '{PeerID}' has connected from {PeerIP}:{PeerPort}.", peer.Id, peer.Address, peer.Port);
    
    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        => this.Logger.LogInformation("Network peer '{PeerID}' has disconnected from {PeerIP}:{PeerPort}.\n    - Reason: {reason}", peer.Id, peer.Address, peer.Port, disconnectInfo.Reason);
}
