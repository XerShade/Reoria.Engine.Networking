using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services;

public class SecureSocketServices(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISecureSocketBuffer buffer) : SocketServices(logger, configuration), ISecureSocketServices
{
    public new ISecureSocketConfiguration Configuration { get; init; } = configuration;
    public ISecureSocketBuffer Buffer { get; init; } = buffer;
}
