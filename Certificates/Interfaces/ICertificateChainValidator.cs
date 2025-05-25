namespace Reoria.Engine.Networking.Certificates.Interfaces;

public interface ICertificateChainValidator<TCertficateType, TChainType> where TCertficateType : class where TChainType : class
{
    bool Validate(TCertficateType certificate, TChainType? providedChain = null);
}
