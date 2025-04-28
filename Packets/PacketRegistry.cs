using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Packets.Interfaces;
using System.Collections.Concurrent;

namespace Reoria.Engine.Networking.Packets;

public class PacketRegistry : IPacketRegistry
{
    protected readonly ILogger<IPacketRegistry> Logger;
    protected readonly IPacketFactory PacketFactory;
    protected readonly ConcurrentDictionary<string, Type> PacketTypes;

    public PacketRegistry(ILogger<IPacketRegistry> logger, IPacketFactory packetFactory, IEnumerable<IPacket> packets)
    {
        this.Logger = logger;
        this.PacketFactory = packetFactory;
        this.PacketTypes = [];

        this.Logger.LogInformation("Creating packet registry, found {PacketCount} packets.", packets.Count());
        foreach (IPacket packet in packets)
        {
            if (this.PacketTypes.TryAdd(packet.PacketIdentifier, packet.GetType()))
            {
                this.Logger.LogDebug("Adding packet '{PacketType}' with identifier '{PacketIdentifier}'.", packet.GetType().Name, packet.PacketIdentifier);
            }
        }
    }

    public virtual void HandleIncomingData(byte[] data)
    {
        try
        {
            NetDataReader reader = new(data);

            this.HandleIncomingData(reader);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error has occurred while handling incoming packet data.");
            throw;
        }
    }

    public virtual void HandleIncomingData(NetDataReader reader)
    {
        try
        {
            string packetIdentifier = reader.GetString();

            if (string.IsNullOrWhiteSpace(packetIdentifier))
            {
                throw new InvalidOperationException("Packet identifier can not be null or white space.");
            }

            if (this.PacketTypes.TryGetValue(packetIdentifier, out Type? packetType))
            {
                IPacket packet = this.PacketFactory.CreatePacket(packetType);

                packet.Deserialize(reader);
            }
            else
            {
                throw new InvalidOperationException($"Packet identifier '{packetIdentifier}' is not a valid packet identifier.");
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error has occurred while handling incoming packet data.");
            throw;
        }
    }

    public virtual NetDataWriter HandleOutgoingData<TPacket>() where TPacket : IPacket
    {
        try
        {
            NetDataWriter writer = new();
            TPacket packet = this.PacketFactory.CreatePacket<TPacket>();

            return this.HandleOutgoingData(packet, writer);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error has occurred while handling outgoing packet data.");
            throw;
        }
    }

    public virtual NetDataWriter HandleOutgoingData(Type packetType)
    {
        try
        {
            NetDataWriter writer = new();
            IPacket packet = this.PacketFactory.CreatePacket(packetType);

            return this.HandleOutgoingData(packet, writer);
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error has occurred while handling outgoing packet data.");
            throw;
        }
    }

    public virtual NetDataWriter HandleOutgoingData(IPacket packet, NetDataWriter writer)
    {
        try
        {
            writer.Put(packet.PacketIdentifier);

            packet.Serialize(writer);

            return writer;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error has occurred while handling outgoing packet data.");
            throw;
        }
    }
}
