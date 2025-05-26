using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Reflection;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SocketBase(ILogger<ISocketBase> logger, IConfiguration configuration) : ISocketBase
{
    protected readonly ILogger<ISocketBase> Logger = logger;
    protected readonly IConfiguration Configuration = configuration;
    protected readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Reoria";

    public virtual int Port => Convert.ToInt32(this.Configuration["Networking:Port"] ?? this.GetDefaultPort());

    protected virtual string GetDefaultPort()
        => "7234";
}
