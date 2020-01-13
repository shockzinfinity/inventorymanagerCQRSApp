using InventoryManager.Infrastructure.Core.ServiceBus;
using System;

namespace InventoryManager.Application.InventoryItems
{
	public class CreateInventoryItem : ICommand
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public CreateInventoryItem(Guid id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}