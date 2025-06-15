using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services;

public class SocketServices(ILogger<ISocketBase> logger, ISocketConfiguration configuration) : ISocketServices
{
    public ILogger<ISocketBase> Logger { get; init; } = logger;
    public ISocketConfiguration Configuration { get; init; } = configuration;
}
