/**
 * Ephenet Hub
 * 
 * Provides a virtual hub for an arbitary number of ports.
 * Simply replicates data to all given ports that are in use.
 *
 */

using Ephenet.Interface;
using PublicDomain.CSharpVitamins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet.HubImplementation
{
	/// <summary>
	/// Provides the base implementation for an Ephenet hub.
	/// </summary>
    public abstract class HubBase
    {
		/// <summary>
		/// The address of the hub itself.
		/// </summary>
		public readonly ShortGuid Address = ShortGuid.NewGuid();

		protected readonly Dictionary<ShortGuid, TerminalBase> terminals = new Dictionary<ShortGuid, TerminalBase>();

		public readonly int Ports;

		public static int DefaultPorts = 4;

		public enum DebugLevel
		{
			/// <summary>
			/// No debugging.
			/// </summary>
			None = 0,
			/// <summary>
			/// Minimal debugging.
			/// </summary>
			Minimal  = 0x0000001,
			/// <summary>
			/// Log plug and unplug events.
			/// </summary>
			LogPlugs = 0x0000010,
			/// <summary>
			/// Log data received.
			/// </summary>
			LogData  = 0x0000100,
			/// <summary>
			/// Log when a rehop is detected and dropped.
			/// </summary>
			LogDehop = 0x0001000,
			/// <summary>
			/// Log whenever a packet is sent?
			/// </summary>
			LogSends = 0x0010000,
			/// <summary>
			/// Log everything.
			/// </summary>
			All      = 0x1111111,
		};

#if RELEASE
		public int DebugVerbosity = (int)DebugLevel.None;
#elif DEBUG
		public int DebugVerbosity = (int)DebugLevel.LogPlugs | (int)DebugLevel.LogDehop | (int)DebugLevel.LogSends;
#endif

		/// <summary>
		/// Log the given message if DebugVerbosity includes the given level.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void Log(DebugLevel level, string format, params object[] args)
		{
			if ((DebugVerbosity & (int)level) != 0)
			{
				Console.WriteLine("Hub-{0} - {1}", Address, String.Format(format, args));
			}
		}

		public HubBase()
			: this(DefaultPorts)
		{
		}

		public HubBase(int ports)
		{
			Ports = ports;

			// Add our terminals
			for (int i = 0; i < ports; i++)
			{
				HubTerminal terminal = new HubTerminal();
				tieTerminalEvents(terminal);
				
				terminals[terminal.Address] = terminal;
			}
		}

		/// <summary>
		/// Tie the terminal events to handlers in this class.
		/// </summary>
		/// <param name="terminal"></param>
		protected virtual void tieTerminalEvents(HubTerminal terminal)
		{
			terminal.HubPlugInEvent = OnPlugIn;
			terminal.HubPlugOutEvent = OnPlugOut;
			terminal.HubReceventEvent = OnReceive;
		}

		protected virtual void OnPlugIn(HubTerminal localTerminal, TerminalBase remote)
		{
			Log(DebugLevel.LogPlugs, "{0} received plug into {1}", localTerminal.Address, remote.Address);
		}

		protected virtual void OnPlugOut(HubTerminal localTerminal, TerminalReadOnly remote)
		{
			Log(DebugLevel.LogPlugs, "{0} lost plug (was {1})", localTerminal.Address, remote.Address);
		}

		protected virtual void OnReceive(HubTerminal localTerminal)
		{
			Datagram? datagram = localTerminal.ReadDatagram();
			if (datagram.HasValue)
				OnReceive(localTerminal, datagram.Value);
		}

		protected abstract void OnReceive(TerminalBase sender, Datagram datagram);

		public virtual bool PlugInToFree(TerminalBase remote)
		{
			TerminalBase free = GetFreeTerminal();
			if (free == null)
				return false;

			free.PlugIn(remote);

			return true;
		}

		public virtual TerminalBase GetFreeTerminal()
		{
			TerminalBase terminal;
			terminal = terminals.FirstOrDefault(kv => kv.Value.IsConnected == false).Value;

			return terminal;
		}
	}
}
