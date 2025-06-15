using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Interfaces;

public interface ISecureServerSocketServices : ISecureSocketServices
{
    ISessionManager<ISecureSession> SessionManager { get; init; }
}
