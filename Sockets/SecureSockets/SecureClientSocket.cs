using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket(ISecureSocketConfiguration configuration, ISecureClientSocketServices socketServices) : SecureSocketBase(configuration, socketServices), ISecureClientSocket
{
    protected readonly ISecureSession Session = socketServices.Session;

    public virtual string IPAddress => this.Configuration.IPAddress;
}
