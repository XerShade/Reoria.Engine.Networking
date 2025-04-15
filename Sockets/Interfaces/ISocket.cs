using LiteNetLib;

namespace Reoria.Engine.Networking.Sockets.Interfaces;

public interface ISocket : INetEventListener
{
    bool ConnectToServer();
    bool IsConnectedToServer();
    void PollEvents();
    void Start();
    void Stop();
}
