using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenSharp.Sample
{
	partial class SampleWorker
	{
		/// <summary>
		/// Run the tests for the class.
		/// </summary>
		/// <returns></returns>
		public static bool Test()
		{
			Callback.Type t = () => {
				bool[] got = {false, false};
				string MsgA = "Hello there!";
				string MsgB = "Let's go shopping!";

				GenWorker<string, SampleState> workerParent = null;

				object _lock = new object();
				// Provide a callback on data received so we can test that we
				// received the data we were interested in.
				Callback<string>.Type onData = (message) =>
				{
					lock (_lock)
					{
						if (message == MsgA) got[0] = true;
						else if (message == MsgB) got[1] = true;

						// Have we received all data?
						if (got[0] && got[1])
						{
							Console.WriteLine("Got all data, shutting down worker manager");
							workerParent.Dispose();
						}
					}
				};

				workerParent = new GenWorker<string, SampleState>(typeof(SampleWorker), Environment.ProcessorCount, onData);

				workerParent.AddData("Hello there!", "Let's go shopping!");
			};
			t();

			Console.WriteLine("{0}End of sample, press enter to exit.{0}", Environment.NewLine);
			Console.ReadLine();
		
			return true;
		}
	}
}
