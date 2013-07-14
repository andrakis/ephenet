using Ephenet.HubImplementation;
using Ephenet.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet
{
	public class Hub : HubBase
	{
		public Hub(int ports) :
			base(ports)
		{
			Log(DebugLevel.Minimal, "with {0} ports initialized", ports);
		}

		protected override void OnReceive(TerminalBase sender, Datagram datagram)
		{
			Log(DebugLevel.LogData, "Received {0} bytes from {1}. Data:\n{2}",
				datagram.Payload.Length, datagram.Sender, datagram);
			if (isRehop(datagram))
			{
				Log(DebugLevel.LogDehop, "Datagram attempted to rehop across this hub, dropping (dehop)");
				return;
			}

			foreach (TerminalBase terminal in terminals.Values.Where(t => t.Address != sender.Address))
			{
				if (terminal.IsConnected)
				{
					Log(DebugLevel.LogSends, "submitting to {0}", terminal.Address);
					terminal.Send(new Datagram(terminal, datagram));
				}
			}
		}

		/// <summary>
		/// Check if this is a 'rehop' - does one of our terminals exist
		/// already in the 'hops' list?
		/// </summary>
		/// <param name="datagram"></param>
		/// <returns></returns>
		private bool isRehop(Datagram datagram)
		{
			bool result = datagram.Hops.Reverse().Any(hop => terminals.ContainsKey(hop));
			return result;
		}
	}
}
