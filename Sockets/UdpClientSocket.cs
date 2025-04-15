using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public class UdpClientSocket : UdpSocket
{
    protected readonly string IPAddress;
    protected NetPeer? ServerPeer;

    public UdpClientSocket(ILogger<ISocket> logger, IConfiguration configuration) : base(logger, configuration)
    {
        this.IPAddress = configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    public override bool ConnectToServer()
    {
        this.ServerPeer ??= this.Manager.Connect(this.IPAddress, this.Port, string.Empty);

        return this.ServerPeer is not null;
    }

    public override bool IsConnectedToServer()
    {
        if (this.ServerPeer is null)
        { return false; }

        if (this.ServerPeer.ConnectionState != ConnectionState.Connected)
        { return false; }

        return true;
    }

    protected override void OnStop(NetManager netManager)
    {
        this.ServerPeer?.Disconnect();
        this.ServerPeer = null;

        base.OnStop(netManager);
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
