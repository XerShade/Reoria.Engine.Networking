using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket : SecureSocketBase, ISecureClientSocket
{
    protected readonly ISecureSession Session;

    public virtual string IPAddress => this.Configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();

    public SecureClientSocket(ISecureSocketServices socketServices, ISecureSession session) 
        : base(socketServices)
    {
        this.Session = session;
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";
}
