using Reoria.Engine.Networking.Sessions.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

public interface ISecureClientSocketServiceInjector : ISecureSocketServiceInjector
{
    ISecureSession Session { get; init; }
}
