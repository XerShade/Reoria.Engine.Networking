using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Packets.Interfaces;
using Reoria.Engine.Networking.Sockets.Data;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Buffers.Binary;

namespace Reoria.Engine.Networking.Sockets;

public abstract class SecureSocket(ILogger<ISecureSocket> logger) : ISecureSocket
{
    private INetworkManager? networkManager;

    protected readonly ILogger<ISecureSocket> Logger = logger;
    protected bool RequestedDisconnect = false;
    
    protected INetworkManager NetworkManager
    {
        get => this.networkManager ?? throw new NullReferenceException();
        private set => this.networkManager = value;
    }

    public event Func<Guid, byte[], Task> OnMessageReceived = default!;
    public event Func<Guid, Task> OnClientConnected = default!;
    public event Func<Guid, Task> OnClientDisconnected = default!;

    public virtual async Task AttachNetworkManager(INetworkManager networkManager)
        => await Task.Run(() => this.NetworkManager = networkManager);

    protected virtual Task InvokeOnClientConnected(Guid guid)
        => this.OnClientConnected?.Invoke(guid) ?? Task.CompletedTask;

    protected virtual Task InvokeOnClientDisconnected(Guid guid)
        => this.OnClientDisconnected?.Invoke(guid) ?? Task.CompletedTask;

    public virtual Task StartAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public virtual Task StopAsync()
        => Task.CompletedTask;

    public virtual Task SendAsync(Guid connectionId, byte[] data)
        => Task.CompletedTask;

    public virtual Task SendAsync<TPacket>(Guid connectionId) where TPacket : IPacket
        => Task.CompletedTask;

    public virtual Task SendAsync(Guid connectionId, Type packetType)
        => Task.CompletedTask;

    public virtual Task ConnectAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public virtual Task DisconnectAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public virtual Task ForceDisconnectAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public virtual bool IsConnectedToServer()
        => false;

    protected virtual async Task ReadStreamBuffer(SecureSocketConnection connection, CancellationToken cancellationToken)
    {
        byte[] lengthBuffer = new byte[4];

        while (!cancellationToken.IsCancellationRequested)
        {
            int lengthValue = await connection.SslStream.ReadAsync(lengthBuffer, cancellationToken);
            if (lengthValue <= 0)
            {
                break;
            }

            while (lengthValue < 4)
            {
                int more = await connection.SslStream.ReadAsync(lengthBuffer.AsMemory(lengthValue, 4 - lengthValue), cancellationToken);
                if (more == 0)
                {
                    break;
                }
                lengthValue += more;
            }

            int length = BinaryPrimitives.ReadInt32BigEndian(lengthBuffer);
            if (length is <= 0 or > (1024 * 1024))
            {
                break;
            }

            byte[] payload = new byte[length];
            int bytesRead = 0;

            while (bytesRead < length)
            {
                int chunk = await connection.SslStream.ReadAsync(payload.AsMemory(bytesRead, length - bytesRead), cancellationToken);
                if (chunk <= 0)
                {
                    break;
                }
                bytesRead += chunk;
            }

            if (bytesRead == length)
            {
                this.Logger.LogInformation("Read data of length '{DataLength}' from '{ConnectionId}'.", payload.Length, connection.Guid);
                await (this.OnMessageReceived?.Invoke(connection.Guid, payload) ?? Task.CompletedTask);

                this.NetworkManager.PacketRegistry.HandleIncomingData(payload);
            }

            if(this.RequestedDisconnect)
            {
                this.RequestedDisconnect = false;
                await this.ForceDisconnectAsync(cancellationToken);
            }
        }
    }

    protected virtual async Task SendAsync(SecureSocketConnection connection, byte[] data, CancellationToken cancellationToken = default)
    {
        if (connection.SslStream is not null)
        {
            byte[] lengthPrefix = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(lengthPrefix, data.Length);

            this.Logger.LogInformation("Sending data of length '{DataLength}' to '{ConnectionId}'.", data.Length, connection.Guid);
            await connection.SslStream.WriteAsync(lengthPrefix, cancellationToken);
            await connection.SslStream.WriteAsync(data, cancellationToken);
            await connection.SslStream.FlushAsync(cancellationToken);
        }
    }
}
