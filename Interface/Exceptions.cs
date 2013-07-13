using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephenet.Interface.Exceptions
{
    public class TerminalAlreadyConnectedException : Exception
    {
        public TerminalAlreadyConnectedException(TerminalBase terminal)
            : base(String.Format("Terminal is already connected!"))
        { }
    }

	public class TerminalNotConnectedException : Exception
	{
		public TerminalNotConnectedException(TerminalBase terminal)
			: base(String.Format("Terminal is not connected!"))
		{ }
	}
}
