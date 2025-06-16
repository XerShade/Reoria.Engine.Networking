using Reoria.Engine.Common;
using Reoria.Engine.Networking.Sessions.Interfaces;

namespace Reoria.Engine.Networking.Sessions;

public class Session : Disposable, ISession
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Created { get; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public TimeSpan Expiry { get; set; } = TimeSpan.FromHours(1);
    public bool IsExpired => DateTime.UtcNow > this.LastActive + this.Expiry;

    public virtual ISession Close() => this;
}
