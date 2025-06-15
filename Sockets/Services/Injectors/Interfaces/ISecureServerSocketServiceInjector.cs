using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

public interface ISecureServerSocketServiceInjector : ISecureSocketServiceInjector
{
    ISessionManager<ISecureSession> SessionManager { get; init; }
}
