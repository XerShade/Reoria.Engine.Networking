using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Certificates;

public class DefaultSystemChainValidator(IConfiguration configuration, ILogger<DevelopmentSystemChainValidator> logger)
    : ICertificateChainValidator<X509Certificate2, X509Chain>
{
    protected readonly IConfiguration Configuration = configuration;
    protected readonly ILogger<DevelopmentSystemChainValidator> Logger = logger;

    public bool Validate(X509Certificate2 certificate, X509Chain? providedChain = null)
    {
        if (Convert.ToBoolean(this.Configuration["Networking:AllowUntrustedConnections"] ?? "false"))
        {
            this.Logger.LogError("The secure socket will be running on an untrusted connection, please be extremely careful about what information is sent over this connection.");
            return true;
        }

        X509Chain chain = providedChain ?? new X509Chain();

        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

        return chain.Build(certificate);
    }
}
