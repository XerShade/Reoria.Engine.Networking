using LiteNetLib.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Data;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets;

public class SecureClientSocket : SecureSocket
{
    protected readonly string IPAddress;
    protected readonly int Port;
    protected SecureSocketConnection ServerConnection;

    public SecureClientSocket(ILogger<ISecureSocket> logger, IConfiguration configuration) : base(logger)
    {
        this.IPAddress = configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
        this.Port = Convert.ToInt32(configuration["Networking:SecurePort"] ?? this.GetDefaultSecurePort());
        this.ServerConnection = new();

        this.Logger.LogInformation("Created secure socket with '{SocketType}'.", this.GetType().Name);
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    protected virtual string GetDefaultSecurePort()
        => "7235";

    public override async Task SendAsync(Guid connectionId, byte[] data)
    {
        if (this.ServerConnection.SslStream != null)
        {
            await this.SendAsync(this.ServerConnection, data);
        }
    }

    public override async Task SendAsync<TPacket>(Guid connectionId)
    {
        NetDataWriter writer = this.NetworkManager.PacketRegistry.HandleOutgoingData<TPacket>();

        await this.SendAsync(connectionId, writer.Data);
    }

    public override async Task SendAsync(Guid connectionId, Type packetType)
    {
        NetDataWriter writer = this.NetworkManager.PacketRegistry.HandleOutgoingData(packetType);

        await this.SendAsync(connectionId, writer.Data);
    }

    public override async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        this.ServerConnection.TcpClient ??= new();
        await this.ServerConnection.TcpClient.ConnectAsync(this.IPAddress, this.Port, cancellationToken);
        this.Logger.LogInformation("Attempting secure connection to '{IPAddress}:{Port}'.", this.IPAddress, this.Port);

        this.ServerConnection.SslStream ??= new(this.ServerConnection.TcpClient.GetStream(), false, this.VerifySslCertificate);
        await this.ServerConnection.SslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
        {
            TargetHost = this.IPAddress,
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck
        }, cancellationToken);
        this.Logger.LogInformation("Established secure connection to '{IPAddress}:{Port}'.", this.IPAddress, this.Port);

        _ = Task.Run(() => this.ReceiveLoop(cancellationToken), cancellationToken);
    }

    public override Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        this.RequestedDisconnect = true;

        return base.DisconnectAsync(cancellationToken);
    }

    public override Task ForceDisconnectAsync(CancellationToken cancellationToken = default)
    {
        this.ServerConnection.Close();
        this.ServerConnection = new();

        return base.ForceDisconnectAsync(cancellationToken);
    }

    protected virtual bool VerifySslCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        => true;

    protected virtual async Task ReceiveLoop(CancellationToken cancellationToken)
    {
        try
        {
            this.Logger.LogInformation("Opened secure socket connection to the server.");
            await this.ReadStreamBuffer(this.ServerConnection, cancellationToken);
        }
        catch { }

        await this.InvokeOnClientDisconnected(this.ServerConnection.Guid);
        this.ServerConnection.Close();
        this.ServerConnection = new();
        this.Logger.LogInformation("Closed secure socket connection to the server.");
    }

    public override bool IsConnectedToServer()
    {
        if (this.ServerConnection.TcpClient != null)
        {
            return this.ServerConnection.TcpClient.Connected;
        }

        return false;
    }
}
