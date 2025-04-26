using Microsoft.Extensions.Logging;
using Reoria.Engine.Networking.Certificates.Interfaces;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Reoria.Engine.Networking.Certificates;

public class X509CertificateProvider(ILogger<ICertificateProvider> logger) : ICertificateProvider
{
    protected readonly ILogger<ICertificateProvider> Logger = logger;

    public X509Certificate GenerateCertificate(string certPath, string keyPath)
    {
        X500DistinguishedNameBuilder distinguishedNameBuilder = new();
        distinguishedNameBuilder.AddCommonName("localhost");

        X500DistinguishedName distinguishedName = distinguishedNameBuilder.Build();

        using RSA rsa = RSA.Create(4096);

        CertificateRequest request = new(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(10));

        string certPem = this.ExportCertificateToPem(certificate);
        File.WriteAllText(certPath, certPem);

        string keyPem = this.ExportPrivateKeyToPem(rsa);
        File.WriteAllText(keyPath, keyPem);

#pragma warning disable SYSLIB0057 // Type or member is obsolete
        using X509Certificate2 cert = X509Certificate2.CreateFromPem(certPem, keyPem);
        return new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
#pragma warning restore SYSLIB0057 // Type or member is obsolete
    }

    protected string ExportCertificateToPem(X509Certificate2 cert)
    {
        StringBuilder builder = new StringBuilder()
            .AppendLine("-----BEGIN CERTIFICATE-----")
            .AppendLine(Convert.ToBase64String(cert.RawData, Base64FormattingOptions.InsertLineBreaks))
            .AppendLine("-----END CERTIFICATE-----");
        this.Logger.LogDebug("Generated new ssl certificate public key: \n {PublicKey}", builder.ToString());
        return builder.ToString();
    }

    protected string ExportPrivateKeyToPem(RSA rsa)
    {
        byte[] keyBytes = rsa.ExportPkcs8PrivateKey();
        StringBuilder builder = new StringBuilder()
            .AppendLine("-----BEGIN PRIVATE KEY-----")
            .AppendLine(Convert.ToBase64String(keyBytes, Base64FormattingOptions.InsertLineBreaks))
            .AppendLine("-----END PRIVATE KEY-----");
        this.Logger.LogDebug("Generated new ssl certificate private key: \n {PrivateKey}", builder.ToString());
        return builder.ToString();
    }

    public X509Certificate LoadCertificateFromFile(string certPath, string keyPath)
    {
        try
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
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "An error has occured while trying to load the ssl certificate. Attempting to generate a new one.");
        }

        return this.GenerateCertificate(certPath, keyPath);
    }
}
