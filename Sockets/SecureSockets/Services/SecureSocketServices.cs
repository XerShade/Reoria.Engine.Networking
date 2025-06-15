using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Services.Interfaces;
using Reoria.Engine.Networking.Sockets.Services;

namespace Reoria.Engine.Networking.Sockets.SecureSockets.Services;

public class SecureSocketServices(ILogger<ISocketBase> logger, IConfiguration configuration, ISecureSocketBuffer buffer) : SocketServices(logger, configuration), ISecureSocketServices
{
    public ISecureSocketBuffer Buffer { get; init; } = buffer;
}
