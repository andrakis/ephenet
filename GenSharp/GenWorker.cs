using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GenSharp
{
	/// <summary>
	/// Provides a generic worker service.
	/// Requires a class that implements a matching IGenWorker.
	/// </summary>
	/// <typeparam name="DataType">Type of data that is sent to the worker.</typeparam>
	/// <typeparam name="StateType">
	///		Type of state the worker can receive and update.
	///		This state is created in the worker implementation's Init function,
	///		and is passed to the worker implementation each time it receives a
	///		piece of data. That function can optionally return a new state.
	///	</typeparam>
	public class GenWorker<DataType, StateType> : Gen, IDisposable
	{
		/// <summary>
		/// The type of the worker class.
		/// </summary>
		protected readonly Type WorkerType;

		/// <summary>
		/// The list of worker controllers.
		/// </summary>
		protected List<WorkerProcessController> workers = new List<WorkerProcessController>();

		/// <summary>
		/// The data queue.
		/// ConcurrentQueues are guaranteed to be thread-safe.
		/// </summary>
		protected ConcurrentQueue<DataType> queue = new ConcurrentQueue<DataType>();

		/// <summary>
		/// Create a new parent GenWorker class.
		/// Can manage any of the worker classes.
		/// </summary>
		/// <param name="workerType">Use typeof(Class)</param>
		/// <param name="workers">Number of workers to create.</param>
		/// <param name="args">Any arguments to pass to the Init method.</param>
		public GenWorker(Type workerType, int workers, params object[] args)
		{
			WorkerType = workerType;

			for (int i = 0; i < workers; i++)
				AddWorker(args);
		}

		~GenWorker()
		{
			Dispose();
		}

		/// <summary>
		/// Adds a new worker process.
		/// </summary>
		public void AddWorker(object[] args)
		{
			WorkerProcessController worker = new WorkerProcessController(this, WorkerType, args);

			Thread t = new Thread(worker.Loop);

			workers.Add(worker);
			t.Start();
		}

		/// <summary>
		/// Add the given data items to the queue.
		/// </summary>
		/// <param name="data"></param>
		public void AddData(params DataType[] data)
		{
			foreach (DataType d in data)
				queue.Enqueue(d);
		}

		/// <summary>
		/// Attempt to get data from the queue.
		/// Provide the appropriate callbacks.
		/// </summary>
		/// <param name="hasData"></param>
		/// <param name="noData"></param>
		public void GetData(Callback<DataType>.Type hasData, Callback.Type noData)
		{
			DataType data;
			if (queue.TryDequeue(out data))
			{
				hasData(data);
			}
			else
			{
				noData();
			}
		}

		/// <summary>
		/// A process that controls one worker. It is responsible for asking the
		/// parent class (GenWorker) for more data.
		/// </summary>
		protected class WorkerProcessController
		{
			/// <summary>
			/// The parent GenWorker object.
			/// </summary>
			protected readonly GenWorker<DataType, StateType> Parent;

			/// <summary>
			/// The instance of the worker class.
			/// </summary>
			protected readonly IGenWorker<DataType, StateType> Worker;

			/// <summary>
			/// The worker state.
			/// </summary>
			protected StateType State;

			/// <summary>
			/// Whether the thread should stop.
			/// Used by the parent process to signal that the thread should stop.
			/// </summary>
			public bool ShouldStop = false;

			/// <summary>
			/// Create a new instance of the worker process controller.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="ActorType"></param>
			/// <param name="args"></param>
			public WorkerProcessController(GenWorker<DataType, StateType> parent, Type ActorType, object[] args)
			{
				Parent = parent;
				Worker = (IGenWorker<DataType, StateType>)Activator.CreateInstance(ActorType);
				Tuple<GenValue, StateType> result = Worker.Init(args);

				State = result.Item2;

				System.Diagnostics.Debug.Assert(result.Item1 == GenValue.OK);
			}

			/// <summary>
			/// Callback used when the parent has some data for the worker to
			/// process.
			/// </summary>
			/// <param name="data">The data read from the parent queue.</param>
			protected void HasData(DataType data)
			{
				GenWorkerResult result = Worker.Process(data, State);
				if (result.SetNewState)
					State = result.NewState;
				Debug.Assert(result.Value == GenValue.OK);
			}

			/// <summary>
			/// Callback used when the parent has no data.
			/// </summary>
			protected void HasNoData()
			{
				Thread.Sleep(10);
			}

			/// <summary>
			/// The thread's loop. Queries the parent for data as fast as it
			/// can, and calls the appropriate callback (see above.)
			/// </summary>
			public void Loop()
			{
				while (ShouldStop == false)
				{
					Parent.GetData(HasData, HasNoData);
				}

				Console.WriteLine("Worker shutting down");
			}
		}

		/// <summary>
		/// A result value from a Gen Worker implementation.
		/// </summary>
		public struct GenWorkerResult
		{
			public readonly StateType NewState;
			public readonly bool SetNewState;
			public readonly GenValue Value;

			/// <summary>
			/// Create a simple return value.
			/// </summary>
			/// <param name="value"></param>
			public GenWorkerResult(GenValue value)
			{
				Value = value;
				SetNewState = false;
				NewState = default(StateType);
			}
			
			/// <summary>
			/// Create a return value and set the new state of the worker.
			/// </summary>
			/// <param name="value"></param>
			/// <param name="newState"></param>
			public GenWorkerResult(GenValue value, StateType newState)
			{
				Value = value;
				SetNewState = true;
				NewState = newState;
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Notify worker processes they should stop.
		/// </summary>
		public void Dispose()
		{
			Console.WriteLine("GenWorker parent implementation shutting down, sending sig");
			workers.ForEach((WorkerProcessController wpc) =>
			{
				wpc.ShouldStop = true;
			});
		}

		#endregion
	}

	/// <summary>
	/// Provides the interface that all GenWorker actors must implement.
	/// </summary>
	/// <typeparam name="T">Type of data the worker works with.</typeparam>
	public interface IGenWorker<DataType, StateType>
	{
		/// <summary>
		/// Initializes the gen worker, and returns its state.
		/// </summary>
		/// <param name="args"></param>
		/// <returns>
		/// Tuple<GenValue.OK, InitialState>
		/// </returns>
		Tuple<GenValue, StateType> Init(object[] args);

		/// <summary>
		/// Process the given data.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		GenWorker<DataType, StateType>.GenWorkerResult Process(DataType data, StateType state);
	}
}
