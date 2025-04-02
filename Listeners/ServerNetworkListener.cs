using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Common.Security.Encryption.Interfaces;

namespace Reoria.Engine.Networking.Listeners;

public class ServerNetworkListener : NetworkListener
{
    public readonly int MaxConnections;
    public readonly int Port;

    public ServerNetworkListener(ILogger<INetEventListener> logger, IConfiguration configuration, IEncryptionService encryptionService) : base(logger, configuration, encryptionService)
    {
        this.MaxConnections = Convert.ToInt32(this.Configuration["Networking:MaxConnections"] ?? this.GetDefaultMaxConnections());
        this.Port = Convert.ToInt32(this.Configuration["Netowrking:Port"] ?? this.GetDefaultPort());
    }

    protected virtual string GetDefaultMaxConnections()
        => "128";

    protected virtual string GetDefaultPort()
        => "7234";

    public override void Start()
    {
        if(!this.Manager.IsRunning)
        {
            _ = this.Manager.Start(this.Port);
            this.Logger.LogInformation("Started listening on port '{Port}' with '{ListenerType}'.", this.Manager.LocalPort, this.GetType().Name);
        }
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        NetDataWriter writer = new();
        writer.Put("AES_KEYS");
        writer.Put(Convert.ToBase64String(this.EncryptionService.AesKey));
        writer.Put(Convert.ToBase64String(this.EncryptionService.AesIV));
        peer.Send(writer, DeliveryMethod.ReliableOrdered);

        this.Logger.LogWarning("Sent AES key and IV to peer '{peer}'! \n" +
            "    - Key: {key} \n" +
            "    - IV: {iv}", peer.Id, Convert.ToBase64String(this.EncryptionService.AesKey), Convert.ToBase64String(this.EncryptionService.AesIV));

        base.OnPeerConnected(peer);
    }
}
