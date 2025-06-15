using Microsoft.Extensions.Configuration;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using System.Reflection;

namespace Reoria.Engine.Networking.Sockets.Configuration;

public class SocketConfiguration(IConfiguration configuration) : ISocketConfiguration
{
    protected readonly IConfiguration Configuration = configuration;

    public virtual string AssemblyName => Assembly.GetExecutingAssembly().GetName().Name ?? this.GetDefaultAssemblyName();
    public virtual string IPAddress => this.Configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
    public virtual int Port => Convert.ToInt32(this.Configuration["Networking:Port"] ?? this.GetDefaultPort());
    public virtual int MaxConnections => Convert.ToInt32(this.Configuration["Networking:MaxConnections"] ?? this.GetDefaultMaxConnections());

    protected virtual string GetDefaultAssemblyName()
        => "Reoria";

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    protected virtual string GetDefaultPort()
        => "7234";

    protected virtual string GetDefaultMaxConnections()
        => "128";
}
