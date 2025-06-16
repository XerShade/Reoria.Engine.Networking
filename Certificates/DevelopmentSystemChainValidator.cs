using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Certificates;

public class DevelopmentSystemChainValidator(IConfiguration configuration, ILogger<DevelopmentSystemChainValidator> logger) 
    : ICertificateChainValidator<X509Certificate2, X509Chain>
{
    protected readonly IConfiguration Configuration = configuration;
    protected readonly ILogger<DevelopmentSystemChainValidator> Logger = logger;

    public bool Validate(X509Certificate2 certificate, X509Chain? providedChain = null)
    {
        string? assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Reoria.Server";
        string? certPath = this.Configuration["Networking:CertificatePath"] ?? $"{assemblyName}.pem".ToLower();

        if(!File.Exists(certPath))
        {
            this.Logger.LogError("Unable to validate ssl certficiate from the server, reason: The required public certificate is not located at '{CertificatePath}'.", certPath);
            if (Convert.ToBoolean(this.Configuration["Networking:AllowUntrustedConnections"] ?? "false"))
            {
                this.Logger.LogError("The secure socket will be running on an untrusted connection, please be extremely careful about what information is sent over this connection.");
                return true;
            }
            return false;
        }

#pragma warning disable SYSLIB0057 // Type or member is obsolete
        X509Certificate2 localCertificate = new(File.ReadAllBytes(certPath));
#pragma warning restore SYSLIB0057 // Type or member is obsolete

        X509Chain chain = providedChain ?? new X509Chain();
        _ = chain.ChainPolicy.ExtraStore.Add(localCertificate);
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

        return chain.Build(certificate) &&
               chain.ChainElements[^1].Certificate.Thumbprint == localCertificate.Thumbprint;
    }
}
