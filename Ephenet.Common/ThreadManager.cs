using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common
{
	/// <summary>
	/// A class that manages a set of threads.
	/// </summary>
	public class ThreadManager<T>
	{
		protected readonly Thread[] workers;
		public readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

		/// <summary>
		/// Round Robin index.
		/// </summary>
		protected int worker_round_robin = 0;

		public ThreadManager(ThreadStart workerStart, int numWorkers)
		{
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < numWorkers; i++)
			{
				Thread worker = new Thread(workerStart);
				threads.Add(worker);
			}

			workers = threads.ToArray();
		}

		public void Stop()
		{
			workers.ToList().ForEach((thread) => thread.Abort());
		}

		public void EnqueueData(T data)
		{
			queue.Enqueue(data);
		}
	}
}
