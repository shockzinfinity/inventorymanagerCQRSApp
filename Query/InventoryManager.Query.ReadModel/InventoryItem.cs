using System;

namespace InventoryManager.Query.ReadModel
{
	public class InventoryItem
	{
		protected InventoryItem()
		{
		}

		public InventoryItem(Guid id, string name)
		{
			Id = id;
			Name = name;
		}

		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}