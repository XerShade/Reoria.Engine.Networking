using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Packets.Interfaces;

namespace Reoria.Engine.Networking.Packets;

public class PacketFactory(ILogger<IPacketFactory> logger, IServiceProvider serviceProvider) : IPacketFactory
{
    protected readonly ILogger<IPacketFactory> Logger = logger;
    protected readonly IServiceProvider ServiceProvider = serviceProvider;

    public virtual IPacket CreatePacket(Type packetType)
    {
        string errorMessagePrefix = $"An error occured while trying to create packet of type '{packetType.Name}':";

        if (!packetType.IsAssignableTo(typeof(IPacket)))
        {
            throw new InvalidCastException($"{errorMessagePrefix} Unable to cast {packetType.Name} as IPacket.");
        }

        return this.ServiceProvider.GetRequiredService(packetType) as IPacket ??
            throw new NullReferenceException($"{errorMessagePrefix} Service provider returned a null reference.");
    }

    public virtual TPacket CreatePacket<TPacket>() where TPacket : IPacket
        => this.ServiceProvider.GetRequiredService<TPacket>();
}
