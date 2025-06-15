using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

public interface ISecureClientSocketServiceInjector : ISecureSocketServiceInjector
{
    ISecureSession Session { get; init; }
    ICertificateChainValidator<X509Certificate, X509Chain> CertificateChainValidator { get; init; }
}
