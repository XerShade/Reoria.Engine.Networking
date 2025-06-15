using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors;

public class SecureClientSocketServiceInjector(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISecureSocketBuffer buffer, ISecureSession secureSession)
    : SecureSocketServiceInjector(logger, configuration, buffer), ISecureClientSocketServiceInjector
{
    public ISecureSession Session { get; init; } = secureSession;
}
