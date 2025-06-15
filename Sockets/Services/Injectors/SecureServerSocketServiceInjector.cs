using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors;

public class SecureServerSocketServiceInjector(ILogger<ISocketBase> logger, ISecureSocketConfiguration configuration, ISecureSocketBuffer buffer, 
    ISecureSessionManager<ISecureSession> sessionManager, ICertificateProvider<X509Certificate2> certificateProvider) 
    : SecureSocketServiceInjector(logger, configuration, buffer), ISecureServerSocketServiceInjector
{
    public ISecureSessionManager<ISecureSession> SessionManager { get; init; } = sessionManager;
    public ICertificateProvider<X509Certificate2> CertificateProvider { get; init; } = certificateProvider;
}
