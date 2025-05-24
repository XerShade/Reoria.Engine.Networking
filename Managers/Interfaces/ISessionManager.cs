using Reoria.Engine.Networking.Sessions;

namespace Reoria.Engine.Networking.Managers.Interfaces;

public interface ISessionManager
{
    void CheckSessionExpiry();
    void CloseSession(SocketSession session);
    SocketSession OpenSession();
}
