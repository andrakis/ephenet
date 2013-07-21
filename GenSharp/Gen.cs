/**
 * GenSharp is a C# implementation of various generic behaviors such as those
 * found in Erlang.
 * In this case, we use interfaces rather than behaviors, but the same actor
 * model and state system is used.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenSharp
{
	/// <summary>
	/// The parent generic class.
	/// Provides a common interface that is shared between the other gen
	/// implementations.
	/// </summary>
	public abstract class Gen
	{
	}

	/// <summary>
	/// A value that can be returned.
	/// </summary>
	public enum GenValue
	{
		OK,
		ERROR
	}
}
