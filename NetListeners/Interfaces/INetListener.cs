using LiteNetLib;

namespace Reoria.Engine.Networking.NetListeners.Interfaces;
public interface INetListener : INetEventListener
{
    bool IsServer { get; }

    int GetLocalPort();
    void OnConfigureNetManager(NetManager netManager);
    void PollEvents();
    void Start();
    void Stop();
}