namespace Reoria.Engine.Networking.Certificates.Interfaces;

public interface ICertificateGenerator<TCertficateType> where TCertficateType : class
{
    TCertficateType GenerateCertificate(string certPath, string keyPath);
}