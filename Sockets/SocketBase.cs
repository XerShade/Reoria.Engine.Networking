using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SocketBase(ISocketServiceInjector serviceInjector) : ISocketBase
{
    protected readonly ILogger<ISocketBase> Logger = serviceInjector.Logger;
    protected readonly ISocketConfiguration Configuration = serviceInjector.Configuration;

    public virtual string AssemblyName => this.Configuration.AssemblyName;
    public virtual int Port => this.Configuration.Port;
}
