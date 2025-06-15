using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets.Services.Interfaces;

public interface ISecureSocketServices : ISocketServices
{
    ISecureSocketBuffer Buffer { get; init; }
}
