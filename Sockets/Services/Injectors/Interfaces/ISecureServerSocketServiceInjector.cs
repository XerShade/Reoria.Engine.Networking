using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;

public interface ISecureServerSocketServiceInjector : ISecureSocketServiceInjector
{
    ISecureSessionManager<ISecureSession> SessionManager { get; init; }
    ICertificateProvider<X509Certificate2> CertificateProvider { get; init; }
}
