using InventoryManager.Infrastructure.Core.ServiceBus;
using MassTransit;
using MassTransit.BusConfigurators;
using MassTransit.Log4NetIntegration.Logging;
using System;
using IServiceBus = InventoryManager.Infrastructure.Core.ServiceBus.IServiceBus;

namespace InventoryManager.Infrastructure.Bus.MassTransitBus
{
	public class MassTransitServiceBus : IServiceBus
	{
		private readonly MassTransit.IServiceBus _massTransitBus;

		public MassTransitServiceBus(Action<ServiceBusConfigurator> configurator)
		{
			Log4NetLogger.Use();
			_massTransitBus = ServiceBusFactory.New(sbc => configurator(sbc));
		}

		public void Publish(IEvent eventMessage)
		{
			_massTransitBus.Publish(eventMessage, eventMessage.GetType(), x => { x.SetDeliveryMode(MassTransit.DeliveryMode.Persistent); });
		}

		public void Send(ICommand commandMessage)
		{
			_massTransitBus.Publish(commandMessage, commandMessage.GetType(), x => { x.SetDeliveryMode(MassTransit.DeliveryMode.Persistent); });
		}

		public void Dispose()
		{
			_massTransitBus.Dispose();
		}
	}
}