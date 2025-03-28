using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Reoria.Engine.Networking.Listeners;

public class ClientNetworkListener : NetworkListener
{
    public readonly string IPAddress;
    public readonly int Port;
    public readonly int ConnectionTimeOut;

    protected NetPeer? ServerPeer { get; set; } = null;

    public ClientNetworkListener(ILogger<INetEventListener> logger, IConfiguration configuration) : base(logger, configuration)
    {
        this.IPAddress = this.Configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
        this.Port = Convert.ToInt32(this.Configuration["Netowrking:Port"] ?? this.GetDefaultPort());
        this.ConnectionTimeOut = Convert.ToInt32(this.Configuration["Netowrking:Port"] ?? this.GetDefaultConnectionTimeOut());
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    protected virtual string GetDefaultPort()
        => "7234";

    protected virtual string GetDefaultConnectionTimeOut()
        => "30";

    public virtual void ConnectToServer()
    {
        if (this.ServerPeer is null)
        {
            this.ServerPeer = this.Manager.Connect(this.IPAddress, this.Port, this.ConnectionKey);

            _ = Task.Run(() =>
            {
                Stopwatch timeout = Stopwatch.StartNew();

                while (this.ServerPeer.ConnectionState != ConnectionState.Connected)
                {
                    if (timeout.Elapsed.Seconds >= this.ConnectionTimeOut)
                    {
                        break;
                    }
                }

                timeout.Stop();

                if (this.ServerPeer.ConnectionState != ConnectionState.Connected)
                {
                    this.Logger.LogError("Unable to connect to the server at {ServerIP}:{ServerPort} after {Timeout} seconds.", this.IPAddress, this.Port, timeout.Elapsed.Seconds);
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
}
