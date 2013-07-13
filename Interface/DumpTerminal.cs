using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet.Interface
{
	/// <summary>
	/// A simple type of terminal - one that simply prints whatever it
	/// receives, and can send too.
	/// </summary>
	public class DumpTerminal : Terminal
	{
		public DumpTerminal()
		{
			this.PlugInEvent = to =>
			{
				Console.WriteLine("{0}: Plugged into {1}", this.Address, to.Address);
			};

			this.PlugOutEvent = was => {
				Console.WriteLine("{0}: Lost connection to {1}", this.Address, was.Address);
			};
		}

		protected override void OnReceive()
		{
			Datagram[] grams = {};
			this.DatagramCriticalSection((Datagrams) =>
			{
				grams = Datagrams.ToArray();
				Datagrams.Clear();
			});

			foreach (Datagram gram in grams)
			{
				Console.WriteLine("{0}: Received {1} bytes from {2}. Data:\n{3}",
					this.Address, gram.Payload.Length, gram.Sender, gram);
			}
		}
	}
}
