namespace Reoria.Engine.Networking.Sockets.Configuration.Interfaces;

public interface ISecureSocketConfiguration : ISocketConfiguration
{
    string CertificatePath { get; }
    string CertificateKeyPath { get; }
}
