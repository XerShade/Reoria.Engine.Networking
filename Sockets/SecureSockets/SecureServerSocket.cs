using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureServerSocket : SecureSocketBase, ISecureServerSocket
{
    public virtual int MaxConnections => Convert.ToInt32(this.Configuration["Networking:MaxConnections"] ?? this.GetDefaultMaxConnections());

    public SecureServerSocket(ILogger<ISocketBase> logger, IConfiguration configuration, ISecureSocketBuffer buffer) 
        : base(logger, configuration, buffer)
    {

    }

    protected virtual string GetDefaultMaxConnections()
        => "128";
}
