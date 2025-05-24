using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions;

namespace Reoria.Engine.Networking.Managers;

public class SessionManager(ILogger<ISessionManager> logger) : ISessionManager
{
    protected readonly ILogger<ISessionManager> Logger = logger;
    protected readonly List<SocketSession> Sessions = [];
    protected readonly Lock @lock = new();

    public virtual SocketSession OpenSession()
    {
        SocketSession session = new();

        this.Sessions.Add(session);

        return session;
    }

    public virtual void CloseSession(SocketSession session)
    {
        _ = this.Sessions.Remove(session);
        session.Close();
    }

    public virtual void CheckSessionExpiry()
    {
        foreach (SocketSession session in this.Sessions.ToArray())
        {
            if (session.IsExpired)
            {
                this.CloseSession(session);
            }
        }
    }
}
