using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket : SecureSocketBase, ISecureClientSocket
{
    protected readonly ISecureSession Session;

    public virtual string IPAddress => this.Configuration.IPAddress;

    public SecureClientSocket(ISecureSocketConfiguration configuration, ISecureSocketServices socketServices, ISecureSession session) 
        : base(configuration, socketServices)
    {
        this.Session = session;
    }
}
