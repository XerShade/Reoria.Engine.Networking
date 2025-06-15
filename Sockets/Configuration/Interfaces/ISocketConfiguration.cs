namespace Reoria.Engine.Networking.Sockets.Configuration.Interfaces;

public interface ISocketConfiguration
{
    int Port { get; }
    string AssemblyName { get; }
    int MaxConnections { get; }
    string IPAddress { get; }
}
