using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Application.InventoryItems;
using InventoryManager.Domain;
using InventoryManager.Infrastructure.Azure;
using InventoryManager.Infrastructure.Bus.MassTransitBus;
using InventoryManager.Infrastructure.Core;
using InventoryManager.Infrastructure.Core.EventSourcing;
using InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents;
using InventoryManager.Infrastructure.Core.EventSourcing.Store;
using InventoryManager.Infrastructure.Core.IoC;
using InventoryManager.Infrastructure.Core.ServiceBus;
using InventoryManager.Infrastructure.UnityContainer;
using InventoryManager.Query.ReadModel;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using InventoryItem = InventoryManager.Domain.InventoryItem;

namespace InventoryManager.Worker.Main
{
    public class Bootstrapper
    {
        public static void Init()
        {
            SetContainer();

            RegisterServiceBus();

            RegisterRepositories();
        }

        private static void RegisterRepositories()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            RegisterRepository<InventoryItem>(storageAccount, "InventoryItems");
        }

        private static void RegisterRepository<T>(CloudStorageAccount storageAccount, string eventStoreTableName) where T: class, IEventSourced  
        {
            // TODO: See if we can work with registerType (non-singletons) for all of the following
            var eventStore = new EventStore<T>(storageAccount, eventStoreTableName);
            IoC.RegisterInstance<IEventStore<T>>(eventStore);
            IoC.RegisterInstance<IPendingEventsQueue<T>>(eventStore);
            IoC.RegisterAsSingleton<IEventStoreBusPublisher<T>, EventStoreBusPublisher<T>>();
            IoC.RegisterAsSingleton<IEventSourcedRepository<T>, AzureEventSourcedRepository<T>>();
            IoC.RegisterAsSingleton<IProcessor, EventPublishProcessor<T>>(name: typeof(T).Name + "_EventPublisherProcessor");
        }

        private static void RegisterServiceBus()
        {
            var bus = new MassTransitServiceBus(
                x => new MassTransitWithAzureServiceBusConfigurator(ConfigurationManager.AppSettings.Get("azure-namespace"), 
                                                                    "InventoryManager.WriteSide", 
                                                                    ConfigurationManager.AppSettings.Get("azure-key"), x)
                                                          .WithHandler<CreateInventoryItem, InventoryItemAppService>()
                                                          .WithHandler<InventoryItemCreated, InventoryViewModelGenerator>());
            ;
            IoC.RegisterInstance<IServiceBus>(bus);
        }

        private static void SetContainer()
        {
            IoC.SetContainer(new UnityIocContainer());
        }
    }
}
