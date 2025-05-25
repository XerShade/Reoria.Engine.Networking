using Reoria.Engine.Networking.Certificates.Interfaces;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Reoria.Engine.Networking.Certificates;

public class X509Certificate2Generator : ICertificateGenerator<X509Certificate2>
{
    public X509Certificate2 GenerateCertificate(string certPath, string keyPath)
    {
        X500DistinguishedNameBuilder distinguishedNameBuilder = new();
        distinguishedNameBuilder.AddCommonName("localhost");

        X500DistinguishedName distinguishedName = distinguishedNameBuilder.Build();

        using RSA rsa = RSA.Create(4096);

        CertificateRequest request = new(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        X509Certificate2 certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(10));

        string certPem = ExportCertificateToPem(certificate);
        File.WriteAllText(certPath, certPem);

        string keyPem = ExportPrivateKeyToPem(rsa);
        File.WriteAllText(keyPath, keyPem);

#pragma warning disable SYSLIB0057 // Type or member is obsolete
        using X509Certificate2 cert = X509Certificate2.CreateFromPem(certPem, keyPem);
        return new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
#pragma warning restore SYSLIB0057 // Type or member is obsolete
    }

    protected static string ExportCertificateToPem(X509Certificate2 cert)
    {
        StringBuilder builder = new StringBuilder()
            .AppendLine("-----BEGIN CERTIFICATE-----")
            .AppendLine(Convert.ToBase64String(cert.RawData, Base64FormattingOptions.InsertLineBreaks))
            .AppendLine("-----END CERTIFICATE-----");
        return builder.ToString();
    }

    protected static string ExportPrivateKeyToPem(RSA rsa)
    {
        byte[] keyBytes = rsa.ExportPkcs8PrivateKey();
        StringBuilder builder = new StringBuilder()
            .AppendLine("-----BEGIN PRIVATE KEY-----")
            .AppendLine(Convert.ToBase64String(keyBytes, Base64FormattingOptions.InsertLineBreaks))
            .AppendLine("-----END PRIVATE KEY-----");
        return builder.ToString();
    }
}
