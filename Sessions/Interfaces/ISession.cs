namespace Reoria.Engine.Networking.Sessions.Interfaces;

public interface ISession : IDisposable, IAsyncDisposable
{
    Guid Id { get; }
    DateTime Created { get; }
    DateTime LastActive { get; set; }
    TimeSpan Expiry { get; set; }
    bool IsExpired { get; }

    ISession Close();
}
