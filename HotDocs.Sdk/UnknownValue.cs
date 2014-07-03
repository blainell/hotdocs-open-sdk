/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk
{
	/// <summary>
	/// The UnknownValue struct is used to represent unrecognized values in HotDocs. This would include values that have somehow found their
	/// way into a HotDocs answer file, but which this version of HotDocs does not know how to parse or understand.
	/// </summary>
	internal struct UnknownValue : IValue, IComparable
	{
		private string _outerXml; // XML fragment for unknown value (typically either a DBValue, ClauseLibValue, or DocTextValue)

		public UnknownValue(string outerXml)
		{
			_outerXml = outerXml;
		}

		#region IValue Members

		public ValueType Type
		{
			get { return ValueType.Other; }
		}

		public bool IsAnswered
		{
			get { return true; }
		}

		public bool UserModifiable
		{
			get { return false; }
		}

		/// <summary>
		/// Writes the XML representation of the answer.
		/// </summary>
		/// <param name="writer">The XmlWriter to which to write the answer value.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteWhitespace("\n\t\t"); // To go the extra mile, figure out how many tab characters to use
			writer.WriteRaw(_outerXml);
			writer.WriteWhitespace("\n\t\t"); // To go the extra mile, figure out how many tab characters to use
		}

		#endregion

		#region IConvertible Members

		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		public byte ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		public char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a DateTime.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A DateTime representation of the answer.</returns>
		public DateTime ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a decimal.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A decimal representation of the answer.</returns>
		public decimal ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a double.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A double representation of the answer.</returns>
		public double ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a 16-bit (short) integer.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A 16-bit (short) integer representation of the answer.</returns>
		public short ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		public int ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a 64-bit integer.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A 64-bit (long) integer representation of the answer.</returns>
		public long ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a float.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A float representation of the answer.</returns>
		public float ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A string representation of the answer.</returns>
		public string ToString(IFormatProvider provider)
		{
			return _outerXml;
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType.GUID == GetType().GUID)
				return this;
			throw new InvalidCastException();
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a uint.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A uint representation of the answer.</returns>
		public uint ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		/// <summary>
		/// Converts the value to a ulong.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A ulong representation of the answer.</returns>
		public ulong ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (!(obj is UnknownValue))
				return -1;

			UnknownValue unkValue = (UnknownValue)obj;
			return String.Compare(_outerXml, unkValue._outerXml, StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}
