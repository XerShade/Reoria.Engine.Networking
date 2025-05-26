using LiteNetLib.Utils;
using System.Net.Security;

namespace Reoria.Engine.Networking.Sockets.Buffers.Interfaces;

public interface ISecureSocketBuffer
{
    int GetMaximumBufferLength();
    int GetMinimumBufferLength();
    Task<NetDataReader> ReadStreamBuffer(SslStream sslStream, CancellationToken cancellationToken = default);
    Task SendAsync(SslStream sslStream, byte[] data, CancellationToken cancellationToken = default);
    Task SendAsync(SslStream sslStream, NetDataWriter writer, CancellationToken cancellationToken = default);
}
