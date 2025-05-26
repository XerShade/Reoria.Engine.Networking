using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public abstract class SecureSocketBase : SocketBase, ISecureSocketBase
{
    protected readonly ISecureSocketBuffer Buffer;

    public override int Port => Convert.ToInt32(this.Configuration["Networking:SecurePort"] ?? this.GetDefaultPort());

    public SecureSocketBase(ILogger<ISocketBase> logger, IConfiguration configuration, ISecureSocketBuffer buffer) : base(logger, configuration)
    {
        this.Buffer = buffer;

        this.Logger.LogInformation("Created secure socket with '{SocketType}' for assembly '{AssemblyName}'.", this.GetType().Name, this.AssemblyName);
    }

    protected override string GetDefaultPort() 
        => "7235";
}
