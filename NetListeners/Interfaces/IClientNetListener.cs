using LiteNetLib;

namespace Reoria.Engine.Networking.NetListeners.Interfaces;
public interface IClientNetListener : INetListener
{
    ConnectionState ConnectionState { get; }

    Task<bool> TryConnectToServer();
}