namespace InventoryManager.Infrastructure.Core.ServiceBus
{
    public interface IHandle<in T>
    {
        void Handle(T message);
    }
}
