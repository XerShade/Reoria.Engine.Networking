using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Interfaces;

public interface ISocketServices
{
    ILogger<ISocketBase> Logger { get; init; }
    IConfiguration Configuration { get; init; }
}
