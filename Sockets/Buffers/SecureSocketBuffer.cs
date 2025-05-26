using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Buffers.Interfaces;
using System.Buffers.Binary;
using System.Net.Security;

namespace Reoria.Engine.Networking.Sockets.Buffers;

public class SecureSocketBuffer(ILogger<ISecureSocketBuffer> logger) : ISecureSocketBuffer
{
    protected readonly ILogger<ISecureSocketBuffer> Logger = logger;

    public virtual int GetMinimumBufferLength()
        => 0;

    public virtual int GetMaximumBufferLength()
        => 1024 * 1024;

    public virtual async Task SendAsync(SslStream sslStream, byte[] data, CancellationToken cancellationToken = default)
    {
        if (sslStream is not null)
        {
            byte[] lengthPrefix = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(lengthPrefix, data.Length);

            this.Logger.LogDebug("Sending data of length '{DataLength}' to '{TargetHostName}'.", data.Length, sslStream.TargetHostName);
            await sslStream.WriteAsync(lengthPrefix, cancellationToken);
            await sslStream.WriteAsync(data, cancellationToken);
            await sslStream.FlushAsync(cancellationToken);
        }
    }

    public virtual async Task SendAsync(SslStream sslStream, NetDataWriter writer, CancellationToken cancellationToken = default)
        => await this.SendAsync(sslStream, writer.Data, cancellationToken);

    public virtual async Task<NetDataReader> ReadStreamBuffer(SslStream sslStream, CancellationToken cancellationToken = default)
    {
        byte[] lengthBuffer = new byte[4];

        int lengthValue = await sslStream.ReadAsync(lengthBuffer, cancellationToken);
        if (lengthValue <= 0)
        {
            this.Logger.LogWarning("Unable to SslStream buffer from '{TargetHostName}', reason: No length prefix was send on the buffer.", sslStream.TargetHostName);
            return new NetDataReader();
        }

        while (lengthValue < 4)
        {
            int more = await sslStream.ReadAsync(lengthBuffer.AsMemory(lengthValue, 4 - lengthValue), cancellationToken);
            if (more == 0)
            {
                break;
            }
            lengthValue += more;
        }

        int length = BinaryPrimitives.ReadInt32BigEndian(lengthBuffer);
        if (length < this.GetMinimumBufferLength() || length > this.GetMaximumBufferLength())
        {
            this.Logger.LogWarning("Unable to SslStream buffer from '{TargetHostName}', reason: Invalid length prefix, '{Length}' was sent expected {MinLength} to {MaxLength}.", 
                sslStream.TargetHostName, length, this.GetMinimumBufferLength(), this.GetMaximumBufferLength());
            return new NetDataReader();
        }

        byte[] payload = new byte[length];
        int bytesRead = 0;

        while (bytesRead < length)
        {
            int chunk = await sslStream.ReadAsync(payload.AsMemory(bytesRead, length - bytesRead), cancellationToken);
            if (chunk <= 0)
            {
                break;
            }
            bytesRead += chunk;
        }

        if (bytesRead == length)
        {
            this.Logger.LogDebug("Read data of length '{DataLength}' to '{TargetHostName}'.", payload.Length, sslStream.TargetHostName);
            return new NetDataReader(payload);
        }

        this.Logger.LogWarning("Unable to SslStream buffer from '{TargetHostName}', reason: The payload size of '{PayloadSize}' did not match the expected size of '{Length}'.",
            sslStream.TargetHostName, bytesRead, length);
        return new NetDataReader();
    }
}
