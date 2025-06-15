using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Net.Sockets;

namespace Reoria.Engine.Networking.Managers.Interfaces;

public interface ISecureSessionManager<TSession> : ISessionManager<TSession> where TSession : ISecureSession
{
    TSession Create(TcpClient connection);
}
