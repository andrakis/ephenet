using Ephenet.Interface.Exceptions;
using GenSharp;
using PublicDomain.CSharpVitamins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet.Interface
{
	/// <summary>
	/// Provides the Terminal "abstract interface", like an interface but
	/// defines non-public members.
	/// </summary>
	public abstract class TerminalBaseAbstractInterface
	{
		#region Events
		/// <summary>
		/// Called when the cable is plugged in.
		/// Use the public getter Remote to access the remote terminal
		/// information.
		/// </summary>
		protected abstract void OnPlugIn(TerminalBase to);

		/// <summary>
		/// Called when the cable is unplugged. Remote will no longer refer
		/// to a terminal, and the terminal object provided contains only
		/// read only access to all data.
		/// </summary>
		protected abstract void OnPlugOut(TerminalReadOnly terminal);

		/// <summary>
		/// Called when the terminal receives a datagram. The datagrams are
		/// maintained privately but accessible via the Datagrams queue.
		/// </summary>
		protected abstract void OnReceive();
		#endregion
	}

    /// <summary>
    /// Provides the base implementation for TerminalBase.
    /// </summary>
    public abstract class TerminalBase : TerminalBaseAbstractInterface
    {
        #region Private variables
        /// <summary>
        /// The remotely connected terminal.
        /// </summary>
        private TerminalBase remote = null;

		protected Queue<Datagram> datagrams = new Queue<Datagram>();
		protected object datagrams_lock = new object();
        #endregion

		#region Static methods
		private static Dictionary<ShortGuid, TerminalBase> terminalDB =
			new Dictionary<ShortGuid, TerminalBase>();
		private static object terminalDB_lock = new object();
		#endregion

		public TerminalBase()
		{
			Address = Guid.NewGuid();
			lock (TerminalBase.terminalDB_lock)
			{
				TerminalBase.terminalDB[this.Address] = this;
			}
		}

		#region Protected accessors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="callback"></param>
		protected void DatagramCriticalSection (Callback<Queue<Datagram>>.Type callback) {
			lock (this.datagrams_lock)
			{
				callback(this.datagrams);
			}
		}
		#endregion

		#region Public accessors
		public readonly ShortGuid Address;

		/// <summary>
		/// Create a clone of the terminal database to browse.
		/// </summary>
		public static Dictionary<ShortGuid, TerminalBase> Terminals
		{
			get { return new Dictionary<ShortGuid, TerminalBase>(TerminalBase.terminalDB); }
		}

        /// <summary>
        /// Information about the remote node.
        /// Note: accessing Remote.Remote will point to this node, unless
        /// something has gone horribly wrong.
        /// </summary>
        public TerminalBase Remote
        {
            get { return this.remote; }
        }

        /// <summary>
        /// Is the terminal currently connected?
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return (this.remote != null);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Plug in this terminal to the given terminal.
        /// </summary>
        /// <param name="to"></param>
        /// <exception cref="TerminalAlreadyConnectedException">
        ///     If either terminal already connected.
        /// </exception>
        public virtual void PlugIn(TerminalBase to)
        {
            if (this.IsConnected)
                throw new TerminalAlreadyConnectedException(this);
            if (to.IsConnected)
                throw new TerminalAlreadyConnectedException(to);

            this._handlePlugIn(to);
            to._handlePlugIn((TerminalBase)this);
        }

        protected virtual void _handlePlugIn(TerminalBase to)
        {
            this.remote = to;
			this.OnPlugIn(to);
        }

        /// <summary>
        /// Unplug the terminal from the remote.
        /// </summary>
        public virtual void PlugOut()
        {
            if (this.IsConnected)
            {
				TerminalReadOnly ro_this = new TerminalReadOnly(this);
                TerminalReadOnly ro_remote = new TerminalReadOnly(remote);
				remote._handlePlugOut(ro_this);
				this._handlePlugOut(ro_remote);
            }
        }

		protected virtual void _handlePlugOut(TerminalReadOnly from)
		{
			this.remote = null;
			this.OnPlugOut(from);
		}

		public virtual void Send(Datagram datagram)
		{
			if (!this.IsConnected)
				throw new TerminalNotConnectedException(this);

			this.remote._handleReceive(datagram);
		}

		protected virtual void _handleReceive(Datagram datagram)
		{
			lock (this.datagrams_lock)
			{
				this.datagrams.Enqueue(datagram);
				this.OnReceive();
			}
		}

        #endregion
    }

    /// <summary>
    /// A readonly implementation of a generic terminal.
    /// This allows the OnPlugOut method to view some final information about
    /// the terminal that was just disconnected.
    /// </summary>
    public class TerminalReadOnly : TerminalBase
    {
        public TerminalReadOnly(TerminalBase source)
        {
        }

        protected override void OnPlugIn(TerminalBase to) { }
        protected override void OnPlugOut(TerminalReadOnly terminal) { }
		protected override void OnReceive() { }

		protected override void _handleReceive(Datagram datagram) { }

        public override void PlugIn(TerminalBase to) { }
        public override void PlugOut() { }
    }

    /// <summary>
    /// An Ephenet "hardware" terminal.
    /// 
    /// This provides the hardware-like object you use to send and receive
    /// data across the wire.
    /// </summary>
    public class Terminal : TerminalBase
    {
		public Callback<TerminalBase>.Type PlugInEvent;
		public Callback<TerminalReadOnly>.Type PlugOutEvent;
		public Callback.Type ReceiveEvent;

		protected override void OnPlugIn(TerminalBase to)
		{
			if (PlugInEvent != null)
				PlugInEvent(to);
		}

		protected override void OnPlugOut(TerminalReadOnly from)
		{
			if (PlugOutEvent != null)
				PlugOutEvent(from);
		}

		protected override void OnReceive()
		{
			if (ReceiveEvent != null)
				ReceiveEvent();
		}
	}
}
