using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket(ISecureClientSocketServices socketServices) : SecureSocketBase(socketServices), ISecureClientSocket
{
    protected readonly ISecureSession Session = socketServices.Session;

    public virtual string IPAddress => this.Configuration.IPAddress;
}
