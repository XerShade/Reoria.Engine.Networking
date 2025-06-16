using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using Reoria.Engine.Networking.Sessions.Interfaces;
using Reoria.Engine.Networking.Sockets.SecureSockets.Interfaces;
using Reoria.Engine.Networking.Sockets.Services.Injectors.Interfaces;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Sockets.SecureSockets;

public class SecureClientSocket(ISecureClientSocketServiceInjector serviceInjector) : SecureSocketBase(serviceInjector), ISecureClientSocket
{
    protected readonly ISecureSession Session = serviceInjector.Session;
    protected readonly ICertificateChainValidator<X509Certificate2, X509Chain> CertificateChainValidator = serviceInjector.CertificateChainValidator;

    public virtual string IPAddress => this.Configuration.IPAddress;

    public virtual async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        _ = this.Session.AssignTcpClient(new());
        await this.Session.TcpClient.ConnectAsync(this.IPAddress, this.Port, cancellationToken);
        this.Logger.LogInformation("Attempting secure connection to '{IPAddress}:{Port}'.", this.IPAddress, this.Port);

        _ = this.Session.AssignSslStream(new(this.Session.TcpClient.GetStream(), false, this.VerifySslCertificate));
        await this.Session.SslStream.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
        {
            TargetHost = this.IPAddress,
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
            CertificateRevocationCheckMode = X509RevocationMode.NoCheck
        }, cancellationToken);
        this.Logger.LogInformation("Established secure connection to '{IPAddress}:{Port}'.", this.IPAddress, this.Port);

        _ = Task.Run(() => this.ReceiveLoop(cancellationToken), cancellationToken);
    }

    public virtual Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _ = this.Session.Close();

        return Task.CompletedTask;
    }

    protected virtual bool VerifySslCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
    {
        if (certificate is not X509Certificate2 cert2)
        {
            throw new InvalidOperationException($"The certificate passed to the validation callback is of type {certificate?.GetType().FullName}, expected X509Certificate2.");
        }

        return this.CertificateChainValidator.Validate(cert2, chain ?? throw new NullReferenceException());
    }

    protected virtual async Task ReceiveLoop(CancellationToken cancellationToken)
    {
        try
        {
            this.Logger.LogInformation("Opened secure socket connection to the server.");
            _ = await this.Buffer.ReadStreamBuffer(this.Session.SslStream, cancellationToken);
        }
        catch { }

        _ = this.Session.Close();
        this.Logger.LogInformation("Closed secure socket connection to the server.");
    }

    public virtual bool IsConnectedToServer()
    {
        if (!this.Session.IsTcpClientNull)
        {
            return this.Session.TcpClient.Connected;
        }

        return false;
    }
}
