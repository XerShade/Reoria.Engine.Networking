using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket(ISecureClientSocketServiceInjector serviceInjector) : SecureSocketBase(serviceInjector), ISecureClientSocket
{
    protected readonly ISecureSession Session = serviceInjector.Session;

    public virtual string IPAddress => this.Configuration.IPAddress;
}
