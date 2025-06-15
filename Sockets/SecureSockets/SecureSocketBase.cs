using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public abstract class SecureSocketBase : SocketBase, ISecureSocketBase
{
    protected new readonly ISecureSocketConfiguration Configuration;
    protected readonly ISecureSocketBuffer Buffer;

    public SecureSocketBase(ISecureSocketConfiguration configuration, ISecureSocketServices socketServices) : base(configuration, socketServices)
    {
        this.Configuration = configuration;
        this.Buffer = socketServices.Buffer;

        this.Logger.LogInformation("Created secure socket with '{SocketType}' for assembly '{AssemblyName}'.", this.GetType().Name, this.AssemblyName);
    }
}
