using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services;

public class SecureClientSocketServices(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISecureSocketBuffer buffer, ISecureSession secureSession)
    : SecureSocketServices(logger, configuration, buffer), ISecureClientSocketServices
{
    public ISecureSession Session { get; init; } = secureSession;
}
