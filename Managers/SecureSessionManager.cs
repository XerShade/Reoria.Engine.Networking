using Microsoft.Extensions.DependencyInjection;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Managers;

public class SecureSessionManager<TSession>(IServiceProvider serviceProvider) : SessionManager<TSession>(serviceProvider), ISecureSessionManager<TSession> where TSession : ISecureSession
{
    protected new readonly ConcurrentDictionary<Guid, TSession> Sessions = [];

    public TSession Create(TcpClient connection)
    {
        TSession session = (TSession)this.ServiceProvider.GetRequiredService<TSession>()
            .AssignTcpClient(connection)
            .AssignSslStream(new(connection.GetStream(), false));

        this.Sessions[session.Id] = session;

        return session;
    }
}
