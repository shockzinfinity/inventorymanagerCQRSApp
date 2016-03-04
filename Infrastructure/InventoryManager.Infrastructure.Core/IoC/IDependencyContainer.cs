namespace InventoryManager.Infrastructure.Core.IoC
{
    public interface IDependencyContainer
    {
        IDependencyRegistry Registrar { get; set; }
        IDependencyResolver Resolver { get; set; }
        object Container { get; }        
    }
}
