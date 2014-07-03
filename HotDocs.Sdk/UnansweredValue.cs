/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Diagnostics;

namespace HotDocs.Sdk
{
	/// <summary>
	/// The UnansweredValue struct represents unanswered values in HotDocs. It is not typically needed, because usually an unanswered value
	/// in HotDocs is still associated with a value type (Text, Number, Date, etc.). However, in contexts where the specific value type may
	/// not be known, UnansweredValue can still be used to represent the value.
	/// </summary>
	public struct UnansweredValue
	{
		public override bool Equals(object obj)
		{
		    return obj is UnansweredValue;
		}

		public override int GetHashCode()
		{
		    return base.GetHashCode();
		}
	}
}
