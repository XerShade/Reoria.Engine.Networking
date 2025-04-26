using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Certificates.Interfaces;

public interface ICertificateProvider
{
    X509Certificate GenerateCertificate(string certPath, string keyPath);
    X509Certificate LoadCertificateFromFile(string certPath, string keyPath);
}
