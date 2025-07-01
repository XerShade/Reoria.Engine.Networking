using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Container.Registrars;
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
    public void RegisterServices(ContainerBuilder builder, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        // General services.
        _ = builder.RegisterType<Session>().As<ISession>().InstancePerDependency();
        _ = builder.RegisterType<SecureSession>().As<ISecureSession>().InstancePerDependency();
        _ = builder.RegisterType<SecureSocketBuffer>().As<ISecureSocketBuffer>().InstancePerDependency();
        _ = builder.RegisterType<SocketConfiguration>().As<ISocketConfiguration>().InstancePerDependency();
        _ = builder.RegisterType<SecureSocketConfiguration>().As<ISecureSocketConfiguration>().InstancePerDependency();
        _ = builder.RegisterType<SocketServiceInjector>().As<ISocketServiceInjector>().InstancePerDependency();
        _ = builder.RegisterType<SecureSocketServiceInjector>().As<ISecureSocketServiceInjector>().InstancePerDependency();

        // Client side services.
        _ = this.AddCertficateChainValidator(builder, configuration, loggerFactory);
        _ = builder.RegisterType<SecureClientSocket>().As<ISecureClientSocket>().SingleInstance();
        _ = builder.RegisterType<SecureClientSocketServiceInjector>().As<ISecureClientSocketServiceInjector>().InstancePerDependency();

        // Server side services.
        _ = builder.RegisterType<X509Certificate2Generator>().As<ICertificateGenerator<X509Certificate2>>().InstancePerDependency();
        _ = builder.RegisterType<X509Certificate2Provider>().As<ICertificateProvider<X509Certificate2>>().InstancePerDependency();
        _ = builder.RegisterType<SessionManager<ISession>>().As<ISessionManager<ISession>>().SingleInstance();
        _ = builder.RegisterType<SecureSessionManager<ISecureSession>>().As<ISecureSessionManager<ISecureSession>>().SingleInstance();
        _ = builder.RegisterType<SecureServerSocket>().As<ISecureServerSocket>().SingleInstance();
        _ = builder.RegisterType<SecureServerSocketServiceInjector>().As<ISecureServerSocketServiceInjector>().InstancePerDependency();
    }

    protected virtual ContainerBuilder AddCertficateChainValidator(ContainerBuilder builder, IConfiguration configuration, ILoggerFactory loggerFactory)
    {
#if !DEBUG
        builder.RegisterType<DefaultSystemChainValidator>()
               .As<ICertificateChainValidator<X509Certificate2, X509Chain>>()
               .InstancePerDependency();
#else
        builder.RegisterType<DevelopmentSystemChainValidator>()
               .As<ICertificateChainValidator<X509Certificate2, X509Chain>>()
               .InstancePerDependency();
#endif
        return builder;
    }
}
