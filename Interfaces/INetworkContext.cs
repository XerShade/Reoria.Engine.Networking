using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Net;

namespace Reoria.Engine.Networking.Interfaces;

public interface INetworkContext
{
    ISecureSocket SecureSocket { get; }
    IPEndPoint RemoteEndPoint { get; }
    Guid? LocalPlayerGuid { get; set; }
}
