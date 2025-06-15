using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Interfaces;

public interface ISocketServices
{
    ILogger<ISocketBase> Logger { get; init; }
    ISocketConfiguration Configuration { get; init; }
}
