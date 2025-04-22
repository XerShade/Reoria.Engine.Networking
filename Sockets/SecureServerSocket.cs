using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Sockets.Data;
using Reoria.Engine.Networking.Sockets.Interfaces;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Reoria.Engine.Networking.Sockets;

public class SecureServerSocket : SecureSocket
{
    protected readonly int MaxConnections;
    protected readonly int Port;
    protected readonly ConcurrentDictionary<Guid, SecureSocketConnection> Connections;
    protected readonly X509Certificate2 Certificate;
    protected TcpListener? Listener;

    public SecureServerSocket(ILogger<ISecureSocket> logger, IConfiguration configuration) : base(logger)
    {
        string? assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Reoria.Server";

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

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        this.Listener = new TcpListener(IPAddress.Any, this.Port);
        this.Listener.Start();
        this.Logger.LogInformation("Started listening on port '{Port}' with '{ListenerType}'.", this.Port, this.GetType().Name);

        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient connection = await this.Listener.AcceptTcpClientAsync(cancellationToken);
            _ = this.HandleConnectionAsync(connection, cancellationToken);
        }
    }

    public override async Task StopAsync()
    {
        await Task.Run(() =>
        {
            this.Listener?.Stop();

            foreach (SecureSocketConnection connection in this.Connections.Values)
            {
                connection.SslStream.Close();
                connection.TcpClient.Close();
            }
            this.Connections.Clear();

            this.Logger.LogInformation("Stopped listening on port '{Port}' with '{ListenerType}'.", this.Port, this.GetType().Name);
        });
    }

    public override async Task SendAsync(Guid connectionId, byte[] data)
    {
        try
        {
            if (this.Connections.TryGetValue(connectionId, out SecureSocketConnection connection))
            {
                await base.SendAsync(connection, data);
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Unable to send data of length '{DataLength}' to '{ConnectionId}', reason: {Message}", data.Length, connectionId, ex.Message);
        }
    }

    protected virtual async Task HandleConnectionAsync(TcpClient incomingConnection, CancellationToken cancellationToken)
    {
        SecureSocketConnection connection = new(incomingConnection, new(incomingConnection.GetStream(), false));

        await connection.SslStream.AuthenticateAsServerAsync(this.Certificate, false, false);
        this.Logger.LogInformation("Recieved new secure socket connection from '{ConnectionEndpoint}'.", incomingConnection.Client.RemoteEndPoint);

        if (this.Connections.Count >= this.MaxConnections)
        {
            byte[] message = Encoding.ASCII.GetBytes("The server has reached the maximum amount of connections allowed. Please try again later.");

            await this.SendAsync(connection, message, cancellationToken);

            this.Logger.LogInformation("Rejected new secure socket connection from '{ConnectionEndpoint}', reason: {Message}",
                incomingConnection.Client.RemoteEndPoint, "The server has reached the maximum amount of connections allowed.");

            connection.Close();

            return;
        }

        if (this.Connections.TryAdd(connection.Guid, connection))
        {
            try
            {
                this.Logger.LogInformation("Opened new secure socket connection from '{ConnectionEndpoint}'.", incomingConnection.Client.RemoteEndPoint);
                await this.ReadStreamBuffer(connection, cancellationToken);
            }
            catch { }

            if (this.Connections.TryRemove(connection.Guid, out _))
            {
                await this.InvokeOnClientDisconnected(connection.Guid);
                this.Logger.LogInformation("Closed secure socket connection from '{ConnectionEndpoint}'.", connection.TcpClient.Client.RemoteEndPoint);
            }
        }
        else
        {
            this.Logger.LogError("Failed to add new connection to dictionary, '{ConnectionId}' already exists.", connection.Guid);
            return;
        }
    }
}
