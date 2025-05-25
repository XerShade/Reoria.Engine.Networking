using Reoria.Engine.Networking.Certificates.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Certificates;

public class DefaultSystemChainValidator : ICertificateChainValidator<X509Certificate2, X509Chain>
{
    public bool Validate(X509Certificate2 certificate, X509Chain? providedChain = null)
    {
        X509Chain chain = providedChain ?? new X509Chain();

        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

        return chain.Build(certificate);
    }
}
