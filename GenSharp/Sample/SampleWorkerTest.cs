using Ephenet.Common;
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
				GenWorker<string, Thread> workerParent = new GenWorker<string, Thread>(typeof(SampleWorker), Environment.ProcessorCount);
				workerParent.AddData("Hello there!", "Let's go shopping!");
				Thread.Sleep(1500);
				workerParent.Dispose();
			};
			t();

			Console.WriteLine("End of sample.");
			Console.ReadLine();
		
			return true;
		}
	}
}
