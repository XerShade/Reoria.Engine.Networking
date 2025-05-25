using Reoria.Engine.Networking.Certificates.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Certificates;

public class X509CertificateProvider(ICertificateGenerator<X509Certificate> certificateGenerator) : ICertificateProvider<X509Certificate>
{
    protected readonly ICertificateGenerator<X509Certificate> CertificateGenerator = certificateGenerator ?? throw new ArgumentNullException(nameof(certificateGenerator));

    public X509Certificate LoadCertificateFromFile(string certPath, string keyPath)
    {
        if (!File.Exists(certPath) || !File.Exists(keyPath))
        {
            return this.CertificateGenerator.GenerateCertificate(certPath, keyPath);
        }

        string certPem = File.ReadAllText(certPath);

        string keyPem = File.ReadAllText(keyPath);

#pragma warning disable SYSLIB0057 // Type or member is obsolete
        using X509Certificate2 cert = X509Certificate2.CreateFromPem(certPem, keyPem);
        return new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
#pragma warning restore SYSLIB0057 // Type or member is obsolete
    }
}
