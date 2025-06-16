using Microsoft.Extensions.DependencyInjection;
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

namespace Reoria.Engine.Networking.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNetworkingServices(this IServiceCollection services)
    {
        _ = services.AddTransient<ISession, Session>();
        _ = services.AddTransient<ISecureSession, SecureSession>();
        _ = services.AddTransient<ISecureSocketBuffer, SecureSocketBuffer>();
        _ = services.AddTransient<ISocketConfiguration, SocketConfiguration>();
        _ = services.AddTransient<ISecureSocketConfiguration, SecureSocketConfiguration>();
        _ = services.AddTransient<ISocketServiceInjector, SocketServiceInjector>();
        _ = services.AddTransient<ISecureSocketServiceInjector, SecureSocketServiceInjector>();

        return services;
    }

    public static IServiceCollection AddClientNetworkingServices(this IServiceCollection services)
    {
        _ = services.AddNetworkingServices();
        _ = services.AddCertficateChainValidator();
        _ = services.AddSingleton<ISecureClientSocket, SecureClientSocket>();
        _ = services.AddTransient<ISecureClientSocketServiceInjector, SecureClientSocketServiceInjector>();

        return services;
    }

    public static IServiceCollection AddServerNetworkingServices(this IServiceCollection services)
    {
        _ = services.AddNetworkingServices();

        _ = services.AddTransient<ICertificateGenerator<X509Certificate2>, X509Certificate2Generator>();
        _ = services.AddTransient<ICertificateProvider<X509Certificate2>, X509Certificate2Provider>();
        _ = services.AddSingleton<ISessionManager<ISession>, SessionManager<ISession>>();
        _ = services.AddSingleton<ISecureSessionManager<ISecureSession>, SecureSessionManager<ISecureSession>>();
        _ = services.AddSingleton<ISecureServerSocket, SecureServerSocket>();
        _ = services.AddTransient<ISecureServerSocketServiceInjector, SecureServerSocketServiceInjector>();

        return services;
    }

    public static IServiceCollection AddCertficateChainValidator(this IServiceCollection services)
    {
#if !DEBUG
        _ = services.AddTransient<ICertificateChainValidator<X509Certificate2, X509Chain>, DefaultSystemChainValidator>();
#else
        _ = services.AddTransient<ICertificateChainValidator<X509Certificate2, X509Chain>, DevelopmentSystemChainValidator>();
#endif
        return services;
    }
}
