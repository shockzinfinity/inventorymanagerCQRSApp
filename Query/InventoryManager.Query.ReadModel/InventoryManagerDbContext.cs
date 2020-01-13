using System;
using System.Data.Entity;
using System.Linq;

namespace InventoryManager.Query.ReadModel
{
	public class InventoryManagerDbContext : DbContext
	{
		public DbSet<InventoryItem> InventoryItems { get; set; }

		public T Find<T>(Guid id) where T : class
		{
			return Set<T>().Find(id);
		}

		public IQueryable<T> Query<T>() where T : class
		{
			return Set<T>();
		}

		public void Save<T>(T entity) where T : class
		{
			var entry = Entry(entity);

			if (entry.State == EntityState.Detached)
				Set<T>().Add(entity);

			SaveChanges();
		}
	}
}