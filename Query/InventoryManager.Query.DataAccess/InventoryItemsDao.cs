using InventoryManager.Query.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManager.Query.DataAccess
{
	public class InventoryItemsDao
	{
		private readonly Func<InventoryManagerDbContext> _contextFactory;

		public InventoryItemsDao(Func<InventoryManagerDbContext> contextFactory)
		{
			_contextFactory = contextFactory;
		}

		public IList<InventoryItem> GetAllInventoryItems()
		{
			using (var context = _contextFactory.Invoke())
			{
				return context
					.Query<InventoryItem>()
					.ToList();
			}
		}
	}
}