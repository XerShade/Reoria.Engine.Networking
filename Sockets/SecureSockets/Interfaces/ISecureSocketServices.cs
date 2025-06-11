using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

public interface ISecureSocketServices : ISocketServices
{
    ISecureSocketBuffer Buffer { get; init; }
}
