using Microsoft.Extensions.Configuration;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;

namespace Reoria.Engine.Networking.Sockets.Configuration;

public class SecureSocketConfiguration(IConfiguration configuration) : SocketConfiguration(configuration), ISecureSocketConfiguration
{
    public override int Port => Convert.ToInt32(this.Configuration["Networking:SecurePort"] ?? this.GetDefaultPort());
    public virtual string CertificatePath => this.Configuration["Networking:CertificatePath"] ?? this.GetDefaultCertificatePath();
    public virtual string CertificateKeyPath => this.Configuration["Networking:CertificateKeyPath"] ?? this.GetDefaultCertificateKeyPath();

    protected virtual string GetDefaultCertificatePath() => $"{this.AssemblyName}.pem".ToLower();
    protected virtual string GetDefaultCertificateKeyPath() => $"{this.AssemblyName}.key.pem".ToLower();
}
