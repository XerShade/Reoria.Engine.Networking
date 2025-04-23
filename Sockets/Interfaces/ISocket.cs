using LiteNetLib.Utils;
using System.Net;

namespace Reoria.Engine.Networking.Sockets.Interfaces;

public interface ISocket
{
    void Send(NetDataWriter writer, IPEndPoint remoteEndPoint);
}
