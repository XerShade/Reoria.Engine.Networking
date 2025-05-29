using Microsoft.Extensions.DependencyInjection;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Collections.Concurrent;

namespace Reoria.Engine.Networking.Managers;

public class SessionManager<TSession>(IServiceProvider serviceProvider) : ISessionManager<TSession> where TSession : ISession
{
    protected readonly IServiceProvider ServiceProvider = serviceProvider;
    protected readonly ConcurrentDictionary<Guid, TSession> Sessions = [];

    public TSession Create()
    {
        TSession session = this.ServiceProvider.GetRequiredService<TSession>();

        this.Sessions[session.Id] = session;

        return session;
    }

    public bool Close(Guid id)
    {
        bool result = this.Sessions.TryRemove(id, out TSession? session);

        _ = session?.Close();

        return result;
    }

    public bool Close(TSession session)
    {
        bool result = this.Sessions.TryRemove(session.Id, out _);

        _ = session?.Close();

        return result;
    }

    public bool Close(Guid id, out TSession? session)
    {
        bool result = this.Sessions.TryRemove(id, out session);

        _ = session?.Close();

        return result;
    }

    public TSession GetSession(Guid id)
        => this.Sessions.TryGetValue(id, out TSession? session) ? session : throw new NullReferenceException();

    public IEnumerable<TSession> GetActiveSessions()
        => [.. this.Sessions.Values.Where((s) => !s.IsExpired)];

    public IEnumerable<TSession> GetExpiredSessions()
        => [.. this.Sessions.Values.Where((s) => s.IsExpired)];

    public IEnumerable<TSession> GetAllSessions()
        => [.. this.Sessions.Values];

    public void CleanupExpired()
    {
        foreach (TSession session in this.GetExpiredSessions())
        {
            _ = this.Close(session);
        }
    }
}
