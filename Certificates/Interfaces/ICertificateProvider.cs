using System.Security.Cryptography.X509Certificates;

namespace Reoria.Engine.Networking.Certificates.Interfaces;

public interface ICertificateProvider<TCertficateType> where TCertficateType : class
{
    TCertficateType LoadCertificateFromFile(string certPath, string keyPath);
}
