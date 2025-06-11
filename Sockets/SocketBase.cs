using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SocketBase(ISocketConfiguration configuration, ISocketServices socketServices) : ISocketBase
{
    protected readonly ILogger<ISocketBase> Logger = socketServices.Logger;
    protected readonly ISocketConfiguration Configuration = configuration;

    public virtual string AssemblyName => this.Configuration.AssemblyName;
    public virtual int Port => this.Configuration.Port;
}
