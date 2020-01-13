using InventoryManager.Domain;
using InventoryManager.Infrastructure.Core.ServiceBus;
using InventoryManager.Query.ReadModel;
using System;
using System.Diagnostics;
using InventoryItem = InventoryManager.Query.ReadModel.InventoryItem;

namespace InventoryManager.Application.InventoryItems
{
	public class InventoryViewModelGenerator : IHandle<InventoryItemCreated>
	{
		private readonly Func<InventoryManagerDbContext> _contextFactory;

		public InventoryViewModelGenerator(Func<InventoryManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
		}

		public void Handle(InventoryItemCreated eventMessage)
		{
			Trace.TraceInformation("InventoryViewModelGenerator Processing the message InventoryItemCreated - {0} - {1}", eventMessage.InventoryItemId, eventMessage.Name);

			using (var context = _contextFactory.Invoke())
			{
				var item = new InventoryItem(eventMessage.InventoryItemId, eventMessage.Name);
				context.Save(item);
			}
		}
	}
}