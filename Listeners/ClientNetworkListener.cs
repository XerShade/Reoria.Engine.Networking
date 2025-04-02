using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Common.Security.Encryption.Interfaces;

namespace Reoria.Engine.Networking.Listeners;

public class ClientNetworkListener : NetworkListener
{
    public readonly string IPAddress;
    public readonly int Port;

    protected NetPeer? ServerPeer { get; set; } = null;

    public ClientNetworkListener(ILogger<INetEventListener> logger, IConfiguration configuration, IEncryptionService encryptionService) : base(logger, configuration, encryptionService)
    {
        this.IPAddress = this.Configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
        this.Port = Convert.ToInt32(this.Configuration["Netowrking:Port"] ?? this.GetDefaultPort());
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    protected virtual string GetDefaultPort()
        => "7234";

    public virtual void ConnectToServer() 
        => this.ServerPeer ??= this.Manager.Connect(this.IPAddress, this.Port, this.ConnectionKey);

    public virtual bool IsConnectedToServer()
    {
        if (this.ServerPeer is null)
        { return false; }

        if (this.ServerPeer.ConnectionState != ConnectionState.Connected)
        { return false; }

        return true;
    }

    protected override void SendSecretMessage()
    {
        string[] messages = ["Hello world!", "Testing 123.", DateTime.Now.ToString()];
        NetDataWriter encryptedWriter = new();
        Random random = new();

        string message = messages[random.Next(0, messages.Length)];
        this.Logger.LogInformation("Encrypting and sending message: '{message}'", message);
        encryptedWriter.Put(message);
        byte[] encryptedData = this.EncryptionService.Encrypt(encryptedWriter.Data);

        NetDataWriter writer = new();
        writer.Put("MESSAGE");
        writer.Put(encryptedData);

        this.ServerPeer?.Send(writer, DeliveryMethod.ReliableOrdered);

        base.SendSecretMessage();
    }


    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        this.ServerPeer?.Disconnect();
        this.ServerPeer = null;

        base.OnPeerDisconnected(peer, disconnectInfo);
    }
}
