using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors;

public class SecureServerSocketServiceInjector(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISecureSocketBuffer buffer, ISessionManager<ISecureSession> sessionManager) 
    : SecureSocketServiceInjector(logger, configuration, buffer), ISecureServerSocketServiceInjector
{
    public ISessionManager<ISecureSession> SessionManager { get; init; } = sessionManager;
}
