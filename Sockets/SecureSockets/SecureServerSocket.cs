using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureServerSocket(ISecureServerSocketServiceInjector serviceInjector) 
    : SecureSocketBase(serviceInjector), ISecureServerSocket
{
    protected readonly ISessionManager<ISecureSession> SessionManager = serviceInjector.SessionManager;

    public virtual int MaxConnections => this.Configuration.MaxConnections;
}
