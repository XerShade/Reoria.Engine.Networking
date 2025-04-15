using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets;

public class SecureClientSocket : ISecureSocket
{
    protected readonly ILogger<ISecureSocket> Logger;
    protected readonly string IPAddress;
    protected readonly int Port;
    protected Guid ServerGuid;
    protected SslStream? SslStream;
    protected TcpClient? Client;

    public event Func<Guid, byte[], Task> OnMessageReceived = default!;
    public event Func<Guid, Task> OnClientDisconnected = default!;

    public SecureClientSocket(ILogger<ISecureSocket> logger, IConfiguration configuration)
    {
        this.Logger = logger;
        this.IPAddress = configuration["Networking:IPAddress"] ?? this.GetDefaultIPAddress();
        this.Port = Convert.ToInt32(configuration["Networking:SecurePort"] ?? this.GetDefaultSecurePort());
        this.ServerGuid = Guid.Empty;

        this.Logger.LogInformation("Created secure socket with '{SocketType}'.", this.GetType().Name);
    }

    protected virtual string GetDefaultIPAddress()
        => "127.0.0.1";

    protected virtual string GetDefaultSecurePort()
        => "7235";

    public Task StartAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task StopAsync()
        => Task.CompletedTask;

    public async Task SendAsync(Guid connectionId, byte[] data)
    {
        if (this.SslStream != null)
        {
            this.Logger.LogInformation("Sending data of length '{DataLength}' to '{ConnectionId}'.", data.Length, this.ServerGuid);
            await this.SslStream.WriteAsync(data);
        }
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        this.Client = new();
        await this.Client.ConnectAsync(this.IPAddress, this.Port, cancellationToken);
        this.Logger.LogInformation("Attempting secure connection to '{IPAddress}:{Port}'.", this.IPAddress, this.Port);

        this.SslStream = new(this.Client.GetStream(), false, this.VerifySslCertificate);
        await this.SslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
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
        this.ServerGuid = Guid.NewGuid();
        byte[] buffer = new byte[4096];
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if(this.SslStream != null)
                {
                    int bytesRead = await this.SslStream.ReadAsync(buffer, cancellationToken);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    byte[] data = buffer.Take(bytesRead).ToArray();

                    if (this.OnMessageReceived != null)
                    {
                        await this.OnMessageReceived.Invoke(this.ServerGuid, data);
                    }
                }                
            }
        }
        catch { }

        if (this.OnClientDisconnected != null)
        {
            await this.OnClientDisconnected.Invoke(this.ServerGuid);
        }
        this.Logger.LogInformation("Closed secure socket connection to the server.");
    }

    public virtual bool IsConnectedToServer()
    {
        if (this.Client != null)
        {
            //return this.Client.Connected;
        }

        return false;
    }
}
