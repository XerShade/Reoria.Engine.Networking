using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SocketBase(ISocketServices socketServices) : ISocketBase
{
    protected readonly ILogger<ISocketBase> Logger = socketServices.Logger;
    protected readonly ISocketConfiguration Configuration = socketServices.Configuration;

    public virtual string AssemblyName => this.Configuration.AssemblyName;
    public virtual int Port => this.Configuration.Port;
}
