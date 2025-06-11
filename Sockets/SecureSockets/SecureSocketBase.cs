using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public abstract class SecureSocketBase : SocketBase, ISecureSocketBase
{
    protected readonly ISecureSocketBuffer Buffer;

    public override int Port => Convert.ToInt32(this.Configuration["Networking:SecurePort"] ?? this.GetDefaultPort());

    public SecureSocketBase(ISecureSocketServices socketServices) : base(socketServices)
    {
        this.Buffer = socketServices.Buffer;

        this.Logger.LogInformation("Created secure socket with '{SocketType}' for assembly '{AssemblyName}'.", this.GetType().Name, this.AssemblyName);
    }

    protected override string GetDefaultPort() 
        => "7235";
}
