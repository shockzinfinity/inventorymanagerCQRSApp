using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Infrastructure.Bus.MassTransitBus;
using InventoryManager.Infrastructure.Core.IoC;
using InventoryManager.Infrastructure.Core.ServiceBus;
using InventoryManager.Infrastructure.UnityContainer;

namespace InventoryManager.Web.Main
{
    public class Bootstrapper
    {
        public static void Init()
        {
            SetContainer();

            RegisterServiceBus();
        }

        private static void RegisterServiceBus()
        {
            var bus = new MassTransitServiceBus(x => new MassTransitWithAzureServiceBusConfigurator(ConfigurationManager.AppSettings.Get("azure-namespace"), "InventoryManager.ReadSide", ConfigurationManager.AppSettings.Get("azure-key"), x));
            IoC.RegisterInstance<IServiceBus>(bus);
        }

        private static void SetContainer()
        {
            IoC.SetContainer(new UnityIocContainer());
        }
    }
}
