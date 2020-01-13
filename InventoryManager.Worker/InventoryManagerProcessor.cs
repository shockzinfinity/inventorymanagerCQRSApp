using InventoryManager.Infrastructure.Core;
using InventoryManager.Infrastructure.Core.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InventoryManager.Worker
{
	internal class InventoryManagerProcessor : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly List<IProcessor> _processors;

		public InventoryManagerProcessor()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_processors = IoC.ResolveAll<IProcessor>().ToList();
		}

		public void Start()
		{
			_processors.ForEach(p => p.Start());
		}

		public void Stop()
		{
			_cancellationTokenSource.Cancel();
			_processors.ForEach(p => p.Stop());
		}

		public void Dispose()
		{
			_cancellationTokenSource.Dispose();
		}
	}
}