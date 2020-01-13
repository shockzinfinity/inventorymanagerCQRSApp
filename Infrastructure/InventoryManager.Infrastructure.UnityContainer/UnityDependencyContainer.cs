using InventoryManager.Infrastructure.Core.IoC;
using Microsoft.Practices.Unity;

namespace InventoryManager.Infrastructure.UnityContainer
{
	public class UnityDependencyContainer : IDependencyContainer
	{
		public IDependencyRegistry Registrar { get; set; }
		public IDependencyResolver Resolver { get; set; }
		public object Container { get; private set; }

		public UnityDependencyContainer()
		{
			Container = new Microsoft.Practices.Unity.UnityContainer();
			Registrar = new UnityRegistry(Container as IUnityContainer);
			Resolver = new UnityDependencyResolver(Container as IUnityContainer);
		}
	}
}