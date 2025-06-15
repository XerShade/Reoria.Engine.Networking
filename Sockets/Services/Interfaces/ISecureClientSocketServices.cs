using Reoria.Engine.Networking.Sessions.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Services.Interfaces;

public interface ISecureClientSocketServices : ISecureSocketServices
{
    ISecureSession Session { get; init; }
}
