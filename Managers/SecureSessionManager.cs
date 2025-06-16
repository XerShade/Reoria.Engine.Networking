using Microsoft.Extensions.DependencyInjection;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Managers;

public class SecureSessionManager<TSession>(IServiceProvider serviceProvider) : SessionManager<TSession>(serviceProvider), ISecureSessionManager<TSession> where TSession : ISecureSession
{
    public TSession Create(TcpClient connection)
    {
        TSession session = (TSession)this.ServiceProvider.GetRequiredService<TSession>()
            .AssignTcpClient(connection)
            .AssignSslStream(new(connection.GetStream(), false));

        _ = this.Sessions.TryAdd(session.Id, session);

        return session;
    }
}
