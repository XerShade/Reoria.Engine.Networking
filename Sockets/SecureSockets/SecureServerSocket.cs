using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Managers.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureServerSocket(ISecureServerSocketServiceInjector serviceInjector)
    : SecureSocketBase(serviceInjector), ISecureServerSocket
{
    private X509Certificate2? certificate;
    private TcpListener? tcpListener;

    protected readonly ISecureSessionManager<ISecureSession> SessionManager = serviceInjector.SessionManager;
    protected readonly ICertificateProvider<X509Certificate2> CertificateProvider = serviceInjector.CertificateProvider;

    protected X509Certificate2 Certificate { get => this.certificate ?? throw new NullReferenceException(); set => this.certificate = value; }
    protected TcpListener TcpListener { get => this.tcpListener ?? throw new NullReferenceException(); set => this.tcpListener = value; }

    public event Func<Guid, Task> OnClientConnected = default!;
    public event Func<Guid, Task> OnClientDisconnected = default!;

    public virtual int MaxConnections => this.Configuration.MaxConnections;

    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        this.Certificate = this.CertificateProvider.LoadCertificateFromFile(this.Configuration.CertificatePath, this.Configuration.CertificateKeyPath);

        this.TcpListener = new TcpListener(IPAddress.Any, this.Port);
        this.TcpListener.Start();
        this.Logger.LogInformation("Started listening on port '{Port}' with '{ListenerType}'.", this.Port, this.GetType().Name);

        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient connection = await this.TcpListener.AcceptTcpClientAsync(cancellationToken);
            _ = this.HandleConnectionAsync(connection, cancellationToken);
        }
    }

    public virtual async Task StopAsync()
    {
        await Task.Run(() =>
        {
            this.tcpListener?.Stop();

            foreach (ISecureSession session in this.SessionManager.GetAllSessions())
            {
                _ = this.SessionManager.Close(session);
            }

            this.Logger.LogInformation("Stopped listening on port '{Port}' with '{ListenerType}'.", this.Port, this.GetType().Name);

            this.tcpListener?.Dispose();
            this.tcpListener = null;

            this.certificate?.Dispose();
            this.certificate = null;
        });
    }

    protected virtual async Task HandleConnectionAsync(TcpClient connection, CancellationToken cancellationToken)
    {
        ISecureSession session = this.SessionManager.Create(connection);

        await session.SslStream.AuthenticateAsServerAsync(this.Certificate, false, false);
        this.Logger.LogInformation("Recieved new secure socket connection from '{ConnectionEndpoint}'.", connection.Client.RemoteEndPoint);

        if (this.SessionManager.Count >= this.MaxConnections)
        {
            byte[] message = Encoding.ASCII.GetBytes("The server has reached the maximum amount of connections allowed. Please try again later.");

            await this.Buffer.SendAsync(session.SslStream, message, cancellationToken);

            this.Logger.LogInformation("Rejected new secure socket connection from '{ConnectionEndpoint}', reason: {Message}",
                connection.Client.RemoteEndPoint, "The server has reached the maximum amount of connections allowed.");

            _ = this.SessionManager.Close(session);
            connection.Close();

            return;
        }

        try
        {
            this.Logger.LogInformation("Opened new secure socket connection from '{ConnectionEndpoint}'.", connection.Client.RemoteEndPoint);
            await this.InvokeOnClientConnected(session.Id);
            _ = await this.Buffer.ReadStreamBuffer(session.SslStream, cancellationToken);
        }
        catch { }

        if (this.SessionManager.Close(session.Id, out _))
        {
            await this.InvokeOnClientDisconnected(session.Id);
            this.Logger.LogInformation("Closed secure socket connection from '{ConnectionEndpoint}'.", session.TcpClient.Client.RemoteEndPoint);
        }
    }

    protected virtual Task InvokeOnClientConnected(Guid guid)
        => this.OnClientConnected?.Invoke(guid) ?? Task.CompletedTask;

    protected virtual Task InvokeOnClientDisconnected(Guid guid)
        => this.OnClientDisconnected?.Invoke(guid) ?? Task.CompletedTask;
}
