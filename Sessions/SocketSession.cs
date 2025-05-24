namespace Reoria.Engine.Networking.Sessions;

public struct SocketSession
{
    public Guid Guid { get; private set; }
    public TimeSpan Expiry { get; private set; }
    public DateTime LastUse { get; private set; }
    public bool IsValid { get; private set; }

    public SocketSession()
    {
        this.Guid = Guid.NewGuid();
        this.Expiry = TimeSpan.FromHours(1);
        this.LastUse = DateTime.Now;
        this.IsValid = true;
    }

    public readonly bool IsExpired 
        => !this.IsValid || DateTime.Now >= this.LastUse + this.Expiry;

    public void Update()
        => this.LastUse = DateTime.Now;

    public void Close()
        => this.IsValid = false;
}
