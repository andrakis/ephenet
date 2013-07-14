using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBureau;
using PublicDomain.CSharpVitamins;

namespace Ephenet.Interface
{
	/// <summary>
	/// A piece of data being sent or received.
	/// </summary>
	public struct Datagram
	{
		/// <summary>
		/// Unique id of sender.
		/// </summary>
		public readonly ShortGuid Sender;

		/// <summary>
		/// Unique id of destination.
		/// </summary>
		public readonly ShortGuid Destination;

		/// <summary>
		/// The sequence number of this packet - ideally, each time a network
		/// device resends this packet (forwarding for example) it should increment
		/// this number, and add a corresponding entry to the Hops list.
		/// </summary>
		public readonly int Sequence;

		/// <summary>
		/// A list of all the devices this packet has "hopped" through.
		/// </summary>
		public readonly ShortGuid[] Hops;

		/// <summary>
		/// The payload data.
		/// </summary>
		public readonly char[] Payload;

		/// <summary>
		/// Create a new datagram packet, with the given sender, destination,
		/// and payload.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="destination"></param>
		/// <param name="payload"></param>
		public Datagram(TerminalBase sender, TerminalBase destination, char[] payload)
		{
			Sender = sender.Address;
			Destination = destination.Address;
			Sequence = 0;
			Hops = new ShortGuid[] { };
			Payload = payload;
		}

		public Datagram(TerminalBase resender, Datagram datagram)
		{
			Sender = datagram.Sender;
			Destination = datagram.Destination;
			Sequence = datagram.Sequence + 1;
			List<ShortGuid> hops_new = new List<ShortGuid>(datagram.Hops);
			hops_new.Add(resender.Address);
			Hops = hops_new.ToArray();
			Payload = datagram.Payload;
		}

		public override string ToString()
		{
			StringBuilder hops = new StringBuilder();

			foreach (Guid hop in this.Hops)
			{
				hops.AppendLine("		" + hop.ToString());
			}
			
			return String.Format(
"{{\n" +
"	Sender: {0},\n" +
"	Destination: {1},\n" +
"	Sequence: {2},\n" +
"	Hops: {{\n" +
"	{3}\n" +
"	}},\n" +
"	Payload: {4}\n" +
"}}",
				this.Sender,
				this.Destination,
				this.Sequence,
				hops,
				new String(this.Payload)
			);

		}
	}
}
