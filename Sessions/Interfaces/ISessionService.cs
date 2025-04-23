using System.Net;

namespace Reoria.Engine.Networking.Sessions.Interfaces;

public interface ISessionService
{
    void Bind(Guid connectionGuid, IPEndPoint remoteEndPoint);
    Guid GetConnectionGuid(IPEndPoint remoteEndPoint);
    Guid GetRemoteEndPoint(IPEndPoint remoteEndPoint);
    string CreateUdpSessionKey(Guid connectionGuid);
    bool ValidateSessionKey(string key, out Guid connectionGuid);
}
