using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket : SecureSocketBase, ISecureClientSocket
{
    protected readonly ISecureSession Session;

    public virtual string IPAddress => this.Configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();

    public SecureClientSocket(ILogger<ISocketBase> logger, IConfiguration configuration, ISecureSocketBuffer buffer, ISecureSession session) 
        : base(logger, configuration, buffer)
    {
        this.Session = session;
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";
}
