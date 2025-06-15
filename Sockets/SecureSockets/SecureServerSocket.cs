using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureServerSocket(ISecureServerSocketServices socketServices) 
    : SecureSocketBase(socketServices), ISecureServerSocket
{
    protected readonly ISessionManager<ISecureSession> SessionManager = socketServices.SessionManager;

    public virtual int MaxConnections => this.Configuration.MaxConnections;
}
