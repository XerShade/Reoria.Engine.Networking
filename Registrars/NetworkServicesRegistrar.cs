using Microsoft.Extensions.DependencyInjection;
using Reoria.Engine.Container.Registrars;
using Reoria.Engine.Container.Services.Interfaces;
using Reoria.Engine.Networking.Certificates;
using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Managers;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.Buffers;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using Reoria.Engine.Networking.Sockets.Configuration;
using Reoria.Engine.Networking.Sockets.Configuration.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Registrars;
public class NetworkServicesRegistrar : IServiceRegistrar
{
    public void RegisterServices(IServiceRegistryGuard registryGuard)
    {
        // General services.
        registryGuard.TryRegister<ISession, Session>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISecureSession, SecureSession>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISecureSocketBuffer, SecureSocketBuffer>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISocketConfiguration, SocketConfiguration>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISecureSocketConfiguration, SecureSocketConfiguration>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISocketServiceInjector, SocketServiceInjector>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISecureSocketServiceInjector, SecureSocketServiceInjector>(ServiceLifetime.Transient);

        // Client side services.
        this.AddCertficateChainValidator(registryGuard);
        registryGuard.TryRegister<ISecureClientSocket, SecureClientSocket>(ServiceLifetime.Singleton);
        registryGuard.TryRegister<ISecureClientSocketServiceInjector, SecureClientSocketServiceInjector>(ServiceLifetime.Transient);

        // Server side services.
        registryGuard.TryRegister<ICertificateGenerator<X509Certificate2>, X509Certificate2Generator>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ICertificateProvider<X509Certificate2>, X509Certificate2Provider>(ServiceLifetime.Transient);
        registryGuard.TryRegister<ISessionManager<ISession>, SessionManager<ISession>>(ServiceLifetime.Singleton);
        registryGuard.TryRegister<ISecureSessionManager<ISecureSession>, SecureSessionManager<ISecureSession>>(ServiceLifetime.Singleton);
        registryGuard.TryRegister<ISecureServerSocket, SecureServerSocket>(ServiceLifetime.Singleton);
        registryGuard.TryRegister<ISecureServerSocketServiceInjector, SecureServerSocketServiceInjector>(ServiceLifetime.Transient);
    }

    protected virtual void AddCertficateChainValidator(IServiceRegistryGuard registryGuard)
    {
#if !DEBUG
        ArgumentNullException.ThrowIfNull(registryGuard);
        registryGuard.TryRegister<ICertificateChainValidator<X509Certificate2, X509Chain>, DefaultSystemChainValidator>(ServiceLifetime.Transient);
#else
        ArgumentNullException.ThrowIfNull(registryGuard);
        registryGuard.TryRegister<ICertificateChainValidator<X509Certificate2, X509Chain>, DevelopmentSystemChainValidator>(ServiceLifetime.Transient);
#endif
    }
}
