using System;

namespace InventoryManager.Infrastructure.Core.ServiceBus
{
	public interface IServiceBus : IDisposable
	{
		void Publish(IEvent eventMessage);

		void Send(ICommand commandMessage);
	}
}