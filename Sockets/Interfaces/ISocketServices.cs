using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reoria.Engine.Networking.Sockets.Interfaces;

public interface ISocketServices
{
    ILogger<ISocketBase> Logger { get; init; }
    IConfiguration Configuration { get; init; }
}
