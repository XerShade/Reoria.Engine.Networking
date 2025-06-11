using Microsoft.Extensions.Configuration;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureSocketConfiguration(IConfiguration configuration) : SocketConfiguration(configuration), ISecureSocketConfiguration
{
    public override int Port => Convert.ToInt32(this.Configuration["Networking:SecurePort"] ?? this.GetDefaultPort());
}
