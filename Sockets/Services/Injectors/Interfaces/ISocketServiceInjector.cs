using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

public interface ISocketServiceInjector
{
    ILogger<ISocketBase> Logger { get; init; }
    ISocketConfiguration Configuration { get; init; }
}
