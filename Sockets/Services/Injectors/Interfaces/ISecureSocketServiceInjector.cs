using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

public interface ISecureSocketServiceInjector : ISocketServiceInjector
{
    new ISecureSocketConfiguration Configuration { get; }
    ISecureSocketBuffer Buffer { get; init; }
}
