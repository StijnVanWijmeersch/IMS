namespace IMS.SharedKernel;

public class Entity
{
    public ulong Id { get; set; }

    protected readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(ulong id)
    {
        Id = id;
    }

    protected Entity() { }

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();

        return copy;
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
