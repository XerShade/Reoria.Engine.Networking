using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Interfaces;

public interface ISecureSocketServices : ISocketServices
{
    ISecureSocketBuffer Buffer { get; init; }
}
