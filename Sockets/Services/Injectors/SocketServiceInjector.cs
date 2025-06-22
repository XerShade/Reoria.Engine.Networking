using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;
using Reoria.Engine.Signals.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors;

public class SocketServiceInjector(ILogger<ISocketBase> logger, ISocketConfiguration configuration, ISignalBus signalBus) : ISocketServiceInjector
{
    public ILogger<ISocketBase> Logger { get; init; } = logger;
    public ISocketConfiguration Configuration { get; init; } = configuration;
    public ISignalBus SignalBus { get; init; } = signalBus;
}
