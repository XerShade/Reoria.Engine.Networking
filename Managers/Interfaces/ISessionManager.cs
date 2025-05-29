using Reoria.Engine.Networking.Sessions.Interfaces;

namespace Reoria.Engine.Networking.Managers.Interfaces;

public interface ISessionManager<TSession> where TSession : ISession
{
    TSession Create();
    bool Close(Guid id);
    TSession GetSession(Guid id);
    IEnumerable<TSession> GetActiveSessions();
    IEnumerable<TSession> GetExpiredSessions();
    IEnumerable<TSession> GetAllSessions();
    void CleanupExpired();
    bool Close(Guid id, out TSession? session);
    bool Close(TSession session);
}
