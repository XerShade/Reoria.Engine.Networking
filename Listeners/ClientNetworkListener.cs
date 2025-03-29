using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reoria.Engine.Networking.Listeners;

public class ClientNetworkListener : NetworkListener
{
    public readonly string IPAddress;
    public readonly int Port;

    protected NetPeer? ServerPeer { get; set; } = null;

    public ClientNetworkListener(ILogger<INetEventListener> logger, IConfiguration configuration) : base(logger, configuration)
    {
        this.IPAddress = this.Configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
        this.Port = Convert.ToInt32(this.Configuration["Netowrking:Port"] ?? this.GetDefaultPort());
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    protected virtual string GetDefaultPort()
        => "7234";

    public virtual void ConnectToServer()
    {
        if (this.ServerPeer is null)
        {
            this.ServerPeer = this.Manager.Connect(this.IPAddress, this.Port, this.ConnectionKey);

            _ = Task.Run(() =>
            {
                while (this.ServerPeer.ConnectionState is not (ConnectionState.Connected or ConnectionState.Disconnected))
                {
                    continue;
                }

                if (this.ServerPeer.ConnectionState != ConnectionState.Connected)
                {
                    this.Logger.LogError("Failed to connect to the server at {ServerIP}:{ServerPort}.", this.IPAddress, this.Port);
                    this.ServerPeer.Disconnect();
                    this.ServerPeer = null;
                }
            });
        }
    }

    public virtual bool IsConnectedToServer()
    {
        if (this.ServerPeer is null)
        { return false; }

        if (this.ServerPeer.ConnectionState != ConnectionState.Connected)
        { return false; }

        return true;
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        base.OnPeerConnected(peer);

        string[] messages = ["Hello world!", "Testing 123.", DateTime.Now.ToString()];
        NetDataWriter writer = new();
        Random random = new();
        writer.Put(messages[random.Next(0, messages.Length - 1)]);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        this.ServerPeer?.Disconnect();
        this.ServerPeer = null;

        base.OnPeerDisconnected(peer, disconnectInfo);
    }
}
