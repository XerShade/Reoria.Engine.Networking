using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;
using Reoria.Engine.Signals.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors;

public class SecureClientSocketServiceInjector(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISignalBus signalBus, ISecureSocketBuffer buffer,
    ISecureSession secureSession, ICertificateChainValidator<X509Certificate2, X509Chain> certificateChainValidator)
    : SecureSocketServiceInjector(logger, configuration, signalBus, buffer), ISecureClientSocketServiceInjector
{
    public ISecureSession Session { get; init; } = secureSession;
    public ICertificateChainValidator<X509Certificate2, X509Chain> CertificateChainValidator { get; init; } = certificateChainValidator;
}
