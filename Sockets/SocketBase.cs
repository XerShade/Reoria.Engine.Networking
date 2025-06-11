using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Reflection;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SocketBase(ISocketServices socketServices) : ISocketBase
{
    protected readonly ILogger<ISocketBase> Logger = socketServices.Logger;
    protected readonly IConfiguration Configuration = socketServices.Configuration;
    protected readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Reoria";

    public virtual int Port => Convert.ToInt32(this.Configuration["Networking:Port"] ?? this.GetDefaultPort());

    protected virtual string GetDefaultPort()
        => "7234";
}
