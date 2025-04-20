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
    protected readonly ILogger<ISecureSocket> Logger;
    protected readonly string IPAddress;
    protected readonly int Port;
    protected SecureSocketConnection ServerConnection;

    public SecureClientSocket(ILogger<ISecureSocket> logger, IConfiguration configuration)
    {
        this.Logger = logger;
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
            this.Logger.LogInformation("Sending data of length '{DataLength}' to '{ConnectionId}'.", data.Length, this.ServerConnection.Guid);
            await this.ServerConnection.SslStream.WriteAsync(data);
        }
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
        this.Logger.LogInformation("Establishing secure connection to '{IPAddress}:{Port}'.", this.IPAddress, this.Port);

        _ = Task.Run(() => this.ReceiveLoop(cancellationToken), cancellationToken);
    }

    protected virtual bool VerifySslCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        => true;

    protected virtual async Task ReceiveLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if(this.ServerConnection.SslStream != null)
                {
                    int bytesRead = await this.ServerConnection.SslStream.ReadAsync(this.ServerConnection.Buffer, cancellationToken);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    byte[] data = this.ServerConnection.Buffer.Take(bytesRead).ToArray();

                    await this.InvokeOnMessageReceived(this.ServerConnection.Guid, data);
                }                
            }
        }
        catch { }

        await this.InvokeOnClientDisconnected(this.ServerConnection.Guid);
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
