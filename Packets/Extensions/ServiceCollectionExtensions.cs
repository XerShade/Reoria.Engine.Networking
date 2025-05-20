using Microsoft.Extensions.DependencyInjection;
using Reoria.Engine.Networking.Packets.Interfaces;
using Reoria.Engine.Networking.Sockets.Packets;
using System.Runtime.CompilerServices;

namespace Reoria.Engine.Networking.Packets.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonPacket<TPacketType>(this IServiceCollection services) where TPacketType : class, IPacket
        => services.AddSingleton<IPacket, TPacketType>().AddSingleton<TPacketType>();

    public static IServiceCollection AddTransientPacket<TPacketType>(this IServiceCollection services) where TPacketType : class, IPacket
        => services.AddTransient<IPacket, TPacketType>().AddTransient<TPacketType>();

    public static IServiceCollection AddScopedPacket<TPacketType>(this IServiceCollection services) where TPacketType : class, IPacket
        => services.AddScoped<IPacket, TPacketType>().AddScoped<TPacketType>();

    public static IServiceCollection AddSingletonPacket(this IServiceCollection services, Type implementationType)
        => services.AddSingleton(typeof(IPacket), implementationType).AddSingleton(implementationType);

    public static IServiceCollection AddTransientPacket(this IServiceCollection services, Type implementationType)
        => services.AddTransient(typeof(IPacket), implementationType).AddTransient(implementationType);

    public static IServiceCollection AddScopedPacket(this IServiceCollection services, Type implementationType)
        => services.AddScoped(typeof(IPacket), implementationType).AddScoped(implementationType);

    public static IServiceCollection AddNetworkingPackets(this IServiceCollection services)
    {
        _ = services.AddTransientPacket<CloseSecureSocketPacket>();
        _ = services.AddTransientPacket<CloseUDPSocketPacket>();
        _ = services.AddTransientPacket<OpenUDPSocketPacket>();

        return services;
    }
}
