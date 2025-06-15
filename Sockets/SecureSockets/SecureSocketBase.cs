using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public abstract class SecureSocketBase : SocketBase, ISecureSocketBase
{
    protected new readonly ISecureSocketConfiguration Configuration;
    protected readonly ISecureSocketBuffer Buffer;

    public SecureSocketBase(ISecureSocketServiceInjector serviceInjector) : base(serviceInjector)
    {
        this.Configuration = serviceInjector.Configuration;
        this.Buffer = serviceInjector.Buffer;

        this.Logger.LogInformation("Created secure socket with '{SocketType}' for assembly '{AssemblyName}'.", this.GetType().Name, this.AssemblyName);
    }
}
