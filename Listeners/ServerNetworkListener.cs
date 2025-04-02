using LiteNetLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reoria.Engine.Networking.Listeners;

public class ServerNetworkListener : NetworkListener
{
    public readonly int MaxConnections;
    public readonly int Port;

    public ServerNetworkListener(ILogger<INetEventListener> logger, IConfiguration configuration) : base(logger, configuration)
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
}
