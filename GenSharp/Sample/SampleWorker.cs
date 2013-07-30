using GenSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenSharp.Sample
{
	struct SampleState
	{
		public readonly Thread Thread;
		public readonly Callback<string>.Type Callback;

		public SampleState(Thread thread, Callback<string>.Type callback)
		{
			Thread = thread;
			Callback = callback;
		}
	}

	/// <summary>
	/// A simple worker that prints a message.
	/// </summary>
	partial class SampleWorker : IGenWorker<string, SampleState>
	{
		#region IGenWorker<string,Thread> Members

		/// <summary>
		/// Initialize the sample worker.
		/// The state of this worker is the current thread, so that we don't
		/// need to keep calling Thread.CurrentThread.
		/// </summary>
		/// <param name="args">An array of:
		///   * [0] - Callback(string).Type - Callback when data is received.
		/// </param>
		/// <returns></returns>
		public Tuple<GenValue, SampleState> Init(object[] args)
		{
			System.Diagnostics.Debug.Assert(args.Length == 1);

			Callback<string>.Type callback = (Callback<string>.Type)args[0];
			SampleState state = new SampleState(Thread.CurrentThread, callback);

			return new Tuple<GenValue, SampleState>(GenValue.OK, state);
		}

		/// <summary>
		/// Process the input message. Doesn't alter state.
		/// </summary>
		/// <param name="data">Message to print.</param>
		/// <param name="state"></param>
		/// <returns></returns>
		public GenWorker<string, SampleState>.GenWorkerResult Process(string data, SampleState state)
		{
			Console.WriteLine("Worker {0} got: {1}", state.Thread.ManagedThreadId, data);
			state.Callback(data);
			return new GenWorker<string, SampleState>.GenWorkerResult(GenValue.OK);
		}

		#endregion
	}
}
