using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenSharp.Sample
{
	/// <summary>
	/// A simple worker that prints a message.
	/// </summary>
	partial class SampleWorker : IGenWorker<string, Thread>
	{
		#region IGenWorker<string,Thread> Members

		/// <summary>
		/// Initialize the sample worker.
		/// The state of this worker is the current thread, so that we don't
		/// need to keep calling Thread.CurrentThread.
		/// </summary>
		/// <param name="args">Ignored.</param>
		/// <returns></returns>
		public Tuple<GenValue, Thread> Init(object[] args)
		{
			return new Tuple<GenValue, Thread>(GenValue.OK, Thread.CurrentThread);
		}

		/// <summary>
		/// Process the input message. Doesn't alter state.
		/// </summary>
		/// <param name="data">Message to print.</param>
		/// <param name="state"></param>
		/// <returns></returns>
		public GenWorker<string, Thread>.GenWorkerResult Process(string data, Thread state)
		{
			Console.WriteLine("Worker {0} got: {1}", state.ManagedThreadId, data);
			return new GenWorker<string, Thread>.GenWorkerResult(GenValue.OK, state);
		}

		#endregion
	}
}
