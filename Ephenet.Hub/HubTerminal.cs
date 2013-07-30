using Ephenet.Interface;
using GenSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet.HubImplementation
{
	/// <summary>
	/// A terminal that is part of a hub.
	/// 
	/// TODO: Should this be merged into Terminal?
	/// </summary>
	public class HubTerminal : Terminal
	{
		public Callback2<HubTerminal, TerminalBase>.Type HubPlugInEvent;
		public Callback2<HubTerminal, TerminalReadOnly>.Type HubPlugOutEvent;
		public Callback<HubTerminal>.Type HubReceventEvent;

		protected override void OnPlugIn(TerminalBase to)
		{
			if (HubPlugInEvent != null)
				HubPlugInEvent(this, to);
		}

		protected override void OnPlugOut(TerminalReadOnly from)
		{
			if (HubPlugOutEvent != null)
				HubPlugOutEvent(this, from);
		}

		protected override void OnReceive()
		{
			if (HubReceventEvent != null)
				HubReceventEvent(this);
		}

		public Datagram? ReadDatagram ()
		{
			Datagram? d = null;
			this.DatagramCriticalSection((Datagrams) => {
				if (Datagrams.Count > 0)
				{
					d = Datagrams.Dequeue();
				}
			});
			return d;
		}
	}
}
