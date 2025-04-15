using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets;

public class SecureServerSocket : ISecureSocket
{
    protected readonly ILogger<ISecureSocket> Logger;
    protected readonly int MaxConnections;
    protected readonly int Port;
    protected readonly ConcurrentDictionary<Guid, SslStream> Connections;
    protected readonly X509Certificate2 Certificate;
    protected TcpListener? Listener;

    public event Func<Guid, byte[], Task> OnMessageReceived = default!;
    public event Func<Guid, Task> OnClientDisconnected = default!;

    public SecureServerSocket(ILogger<ISecureSocket> logger, IConfiguration configuration)
    {
        string? assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Reoria.Server";

        this.Logger = logger;
        this.MaxConnections = Convert.ToInt32(configuration["Networking:MaxConnections"] ?? this.GetDefaultMaxConnections());
        this.Port = Convert.ToInt32(configuration["Networking:SecurePort"] ?? this.DefaultSecurePort());
        this.Connections = [];
        this.Certificate = this.LoadCertificateFromPem(
            configuration["Networking:CertificatePath"] ?? $"{assemblyName}.pem".ToLower(),
            configuration["Networking:CertificateKeyPath"] ?? $"{assemblyName}.key.pem".ToLower());

        this.Logger.LogInformation("Created secure socket with '{SocketType}'.", this.GetType().Name);
    }

    protected virtual string GetDefaultMaxConnections()
        => "128";

    protected virtual string DefaultSecurePort()
        => "7235";

    protected virtual X509Certificate2 LoadCertificateFromPem(string certPath, string keyPath)
    {
        this.Logger.LogInformation("Reading secure socket certificate from '{Path}'.", certPath);
        string certPem = File.ReadAllText(certPath);
        this.Logger.LogInformation("Reading secure socket certificate key from '{Path}'.", keyPath);
        string keyPem = File.ReadAllText(keyPath);

#pragma warning disable SYSLIB0057 // Type or member is obsolete
        using X509Certificate2 cert = X509Certificate2.CreateFromPem(certPem, keyPem);
        return new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
#pragma warning restore SYSLIB0057 // Type or member is obsolete
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        this.Listener = new TcpListener(IPAddress.Any, this.Port);
        this.Listener.Start();
        this.Logger.LogInformation("Started listening on port '{Port}' with '{ListenerType}'.", this.Port, this.GetType().Name);

        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient connection = await this.Listener.AcceptTcpClientAsync(cancellationToken);
            _ = this.HandleConnectionAsync(connection);
        }
    }

    public virtual async Task StopAsync()
    {
        await Task.Run(() =>
        {
            this.Listener?.Stop();

            foreach (KeyValuePair<Guid, SslStream> connection in this.Connections)
            {
                connection.Value.Close();
            }
            this.Connections.Clear();

            this.Logger.LogInformation("Stopped listening on port '{Port}' with '{ListenerType}'.", this.Port, this.GetType().Name);
        });
    }

    public virtual async Task SendAsync(Guid connectionId, byte[] data)
    {
        if (this.Connections.TryGetValue(connectionId, out SslStream? sslStream))
        {
            this.Logger.LogInformation("Sending data of length '{DataLength}' to '{ConnectionId}'.", data.Length, connectionId);
            await sslStream.WriteAsync(data);
        }
    }

    protected virtual async Task HandleConnectionAsync(TcpClient connection)
    {
        Guid connectionId = Guid.NewGuid();
        SslStream sslStream = new(connection.GetStream(), false);

        await sslStream.AuthenticateAsServerAsync(this.Certificate, false, false);
        this.Logger.LogInformation("Recieved new secure socket connection from '{ConnectionEndpoint}'.", connection.Client.RemoteEndPoint);

        if (this.Connections.TryAdd(connectionId, sslStream))
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (true)
                {
                    int bytesRead = await sslStream.ReadAsync(buffer);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    byte[] data = buffer.Take(bytesRead).ToArray();

                    if (this.OnMessageReceived != null)
                    {
                        await this.OnMessageReceived.Invoke(connectionId, data);
                    }
                }
            }
            catch { }

            if (this.Connections.TryRemove(connectionId, out _))
            {
                if (this.OnClientDisconnected != null)
                {
                    await this.OnClientDisconnected.Invoke(connectionId);
                }
                this.Logger.LogInformation("Closed secure socket connection from '{ConnectionEndpoint}'.", connection.Client.LocalEndPoint);
            }
        }
    }

    public Task ConnectAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public bool IsConnectedToServer()
        => false;
}
