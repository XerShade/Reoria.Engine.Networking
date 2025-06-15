using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors;

public class SecureSocketServiceInjector(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISecureSocketBuffer buffer) : SocketServiceInjector(logger, configuration), ISecureSocketServiceInjector
{
    public new ISecureSocketConfiguration Configuration { get; init; } = configuration;
    public ISecureSocketBuffer Buffer { get; init; } = buffer;
}
