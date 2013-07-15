using Ephenet.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ephenet.Common
{
	/// <summary>
	/// Provides an interface similar to EMCA (JavaScript)'s setTimeout.
	/// 
	/// This class spawns and maintains an implementation thread
	/// (TimeoutImplementation) which will delay the exit of your application
	/// until the thread exits.
	/// </summary>
	public static class Timeout
	{
		internal struct Entry
		{
			public DateTime Scheduled;
			public Callback.Type Callback;

			public Entry(DateTime scheduled, Callback.Type callback)
			{
				this.Scheduled = scheduled;
				this.Callback = callback;
			}
		}

		private static Queue<Entry> timeouts = new Queue<Entry>();
		private static Thread thread = null;

		/// <summary>
		/// Sleep for how long when idle?
		/// </summary>
		public static TimeSpan IdleSleep = new TimeSpan(0, 0, 0, 1);

		/// <summary>
		/// Sleep for how long when busy?
		/// </summary>
		public static TimeSpan BusySleep = new TimeSpan(0, 0, 0, 0, 1);

		/// <summary>
		/// The implementation thread will only exit after this many idle
		/// sleeps (no timeouts were queued.)
		/// However, this delays the exit of your application by
		/// MaxIdle * IdleSleep, so you may want to tweak those variables
		/// during initialization.
		/// </summary>
		public static int MaxIdle = 5;

		/// <summary>
		/// Set a callback to be called after timeout.
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="callback"></param>
		public static void Set(TimeSpan timeout, Callback.Type callback)
		{
			Entry entry = new Entry(DateTime.Now.Add(timeout), callback);
			lock (typeof(Timeout))
			{
				List<Entry> list = new List<Entry>(Timeout.timeouts);
				list.Add(entry);
				Console.WriteLine("Timeouts size is now: {0}", list.Count);
				Timeout.timeouts = new Queue<Entry>(list.OrderBy(e => e.Scheduled));

				if (thread == null || thread.IsAlive == false)
				{
					TimeoutImplementation impl = new TimeoutImplementation();
					thread = new Thread(new ThreadStart(impl.EventLoop));
					thread.Start();
				}
			}
		}

		/// <summary>
		/// This invokation is deprecated. Please use Set(timeout, callback).
		/// </summary>
		/// <param name="callback"></param>
		/// <param name="timeout"></param>
		public static void Set(Callback.Type callback, TimeSpan timeout)
		{
			Set(timeout, callback);
		}

		internal static void TimeoutsCriticalSection(Callback<Queue<Timeout.Entry>, Queue<Timeout.Entry>>.Type callback)
		{
			lock (typeof(Timeout))
			{
				Console.WriteLine("Timeouts: enter critical section, size {0}", Timeout.timeouts.Count);
				Timeout.timeouts = callback(Timeout.timeouts);
				Console.WriteLine("Timeouts: enter critical section, size {0}", Timeout.timeouts.Count);
			}
		}

		/// <summary>
		/// Used to notify the class that the implementation thread is about
		/// to exit, and allows us to see if it should instead stay alive.
		/// </summary>
		internal static bool ImplementationCanExit
		{
			get
			{
				lock (typeof(Timeout))
				{
					return Timeout.timeouts.Count == 0;
				}
			}
		}
	}

	/// <summary>
	/// Implements the Timeout thread.
	/// </summary>
	internal class TimeoutImplementation
	{
		internal TimeoutImplementation()
		{
		}

		internal void EventLoop()
		{
			int idleCount = 0;

			// Loop while we haven't idled too long, but check with the Timeout
			// class to see if we really can exit before we actually do.
			while (idleCount < Timeout.MaxIdle || !Timeout.ImplementationCanExit)
			{
				TimeSpan sleepTime = Timeout.IdleSleep;

				Timeout.TimeoutsCriticalSection((Queue<Timeout.Entry> timeouts) =>
				{
					if (timeouts.Count == 0)
					{
						idleCount++;
						return timeouts;
					}

					if (timeouts.Peek().Scheduled < DateTime.Now)
					{
						try
						{
							timeouts.Dequeue().Callback();
						}
						catch (Exception ex)
						{
							Console.WriteLine("TimeoutImplementation.CriticalSection: exception in callback");
							Console.WriteLine("{0}", ex);
						}
						finally
						{
							sleepTime = Timeout.BusySleep;
						}
					}

					return timeouts;
				});

				Thread.Sleep(sleepTime);
			}
		}
	}
}
