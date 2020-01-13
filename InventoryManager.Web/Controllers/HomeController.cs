using InventoryManager.Application.InventoryItems;
using InventoryManager.Infrastructure.Core.IoC;
using InventoryManager.Infrastructure.Core.ServiceBus;
using InventoryManager.Query.DataAccess;
using System;
using System.Web.Mvc;

namespace InventoryManager.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IServiceBus _bus;
		private readonly InventoryItemsDao _inventoryItemsDao;

		public HomeController()
		{
			_bus = IoC.Resolve<IServiceBus>();
			_inventoryItemsDao = IoC.Resolve<InventoryItemsDao>();
		}

		public ActionResult Index()
		{
			var inventoryItems = _inventoryItemsDao.GetAllInventoryItems();
			return View(inventoryItems);
		}

		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Add(string name)
		{
			_bus.Send(new CreateInventoryItem(Guid.NewGuid(), name));
			return RedirectToAction("Index");
		}
	}
}