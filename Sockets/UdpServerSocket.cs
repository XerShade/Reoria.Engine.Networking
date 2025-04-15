using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public class UdpServerSocket : UdpSocket
{
    protected readonly int MaxConnections;

    public UdpServerSocket(ILogger<ISocket> logger, IConfiguration configuration) : base(logger, configuration)
    {
        this.MaxConnections = Convert.ToInt32(configuration["Networking:MaxConnections"] ?? this.GetDefaultMaxConnections());
    }

    protected virtual string GetDefaultMaxConnections()
        => "128";

    protected override void OnStart(NetManager netManager) 
        => _ = netManager.Start(this.Port);
}
