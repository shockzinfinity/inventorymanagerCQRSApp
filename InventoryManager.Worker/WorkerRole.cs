using InventoryManager.Worker.Main;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryManager.Worker
{
	public class WorkerRole : RoleEntryPoint
	{
		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

		public override void Run()
		{
			Trace.TraceInformation("InventoryManager.Worker is running");

			try
			{
				this.RunAsync(this.cancellationTokenSource.Token).Wait();
			}
			finally
			{
				this.runCompleteEvent.Set();
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			bool result = base.OnStart();
			Bootstrapper.Init();
			Trace.TraceInformation("InventoryManager.Worker has been started");

			return result;
		}

		public override void OnStop()
		{
			Trace.TraceInformation("InventoryManager.Worker is stopping");

			this.cancellationTokenSource.Cancel();
			this.runCompleteEvent.WaitOne();

			base.OnStop();

			Trace.TraceInformation("InventoryManager.Worker has stopped");
		}

		private async Task RunAsync(CancellationToken cancellationToken)
		{
			//// TODO: Replace the following with your own logic.
			//while (!cancellationToken.IsCancellationRequested)
			//{
			//    Trace.TraceInformation("Working");
			//    await Task.Delay(1000);
			//}
			Trace.WriteLine("Starting the InventoryManager processor", "Information");
			using (var processor = new InventoryManagerProcessor())
			{
				processor.Start();

				while (!cancellationToken.IsCancellationRequested)
				{
					Thread.Sleep(10000);
				}

				processor.Stop();

				// cause the process to recycle
				return;
			}
		}
	}
}