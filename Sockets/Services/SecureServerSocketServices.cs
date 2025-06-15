using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services;

public class SecureServerSocketServices(ILogger<ISocketBase> logger, IConfiguration configuration, ISecureSocketBuffer buffer, ISessionManager<ISecureSession> sessionManager) 
    : SecureSocketServices(logger, configuration, buffer), ISecureServerSocketServices
{
    public ISessionManager<ISecureSession> SessionManager { get; init; } = sessionManager;
}
