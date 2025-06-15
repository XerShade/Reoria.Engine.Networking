using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services;

public class SocketServices(ILogger<ISocketBase> logger, IConfiguration configuration) : ISocketServices
{
    public ILogger<ISocketBase> Logger { get; init; } = logger;
    public IConfiguration Configuration { get; init; } = configuration;
}
