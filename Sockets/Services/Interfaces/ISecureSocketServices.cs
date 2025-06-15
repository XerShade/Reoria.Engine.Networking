using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Interfaces;

public interface ISecureSocketServices : ISocketServices
{
    new ISecureSocketConfiguration Configuration { get; }
    ISecureSocketBuffer Buffer { get; init; }
}
