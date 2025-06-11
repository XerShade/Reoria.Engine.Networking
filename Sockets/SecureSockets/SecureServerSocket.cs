using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureServerSocket : SecureSocketBase, ISecureServerSocket
{
    protected readonly ISessionManager<ISecureSession> SessionManager;

    public virtual int MaxConnections => this.Configuration.MaxConnections;

    public SecureServerSocket(ISecureSocketConfiguration configuration, ISecureSocketServices socketServices, ISessionManager<ISecureSession> sessionManager) 
        : base(configuration, socketServices)
    {
        this.SessionManager = sessionManager;
    }
}
