namespace InventoryManager.Infrastructure.Core.EventSourcing
{   
    public interface IVersionedEvent : IEventFromSource
    {
        int Version { get; }
    }
}
