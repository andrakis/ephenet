using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenSharp
{
	/// <summary>
	/// A simple callback type. It takes no parameters and is void type.
	/// </summary>
	public class Callback
	{
		public delegate void Type();
	}

	/// <summary>
	/// A callback that takes a single parameter of the given type.
	/// The function is void type.
	/// </summary>
	/// <typeparam name="ValueType">Parameter type ("in type".)</typeparam>
	public class Callback<ValueType>
	{
		public delegate void Type(ValueType value);
	}

	/// <summary>
	/// A callback that takes a single parameter of the given ValueType, and
	/// returns a value of the type ReturnType.
	/// </summary>
	/// <typeparam name="ValueType">Parameter type ("in type".)</typeparam>
	/// <typeparam name="ReturnType">Return type ("out type".)</typeparam>
	public class Callback<ValueType, ReturnType>
	{
		public delegate ReturnType Type(ValueType value);
	}

	/// <summary>
	/// A callback that takes two parameters of the given types, and does not
	/// return a value (void).
	/// </summary>
	/// <typeparam name="A1Type"></typeparam>
	/// <typeparam name="A2Type"></typeparam>
	public class Callback2<A1Type, A2Type>
	{
		public delegate void Type(A1Type a1, A2Type a2);
	}
}
