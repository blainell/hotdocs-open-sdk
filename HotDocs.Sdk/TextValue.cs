/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace HotDocs.Sdk
{
	/// <summary>
	/// A HotDocs Text value.
	/// </summary>
	public struct TextValue : IValue
	{
		private string _value;
		private bool _protect;
		private static Regex s_xmlEscaper = null;
		private static Regex s_xmlUnescaper = null;

		/// <summary>
		/// Static (shared) instance of an unanswered TextValue.
		/// </summary>
		public readonly static TextValue Unanswered;

		/// <summary>
		/// Static (shared) instance of an unanswered TextValue that should be protected from unintentional modification in the interview UI.
		/// </summary>
		public readonly static TextValue UnansweredLocked;

		/// <summary>
		/// Static constructor required so static fields are always initialized
		/// </summary>
		static TextValue()
		{
			TextValue.Unanswered = new TextValue();
			TextValue.UnansweredLocked = new TextValue(null, false);
			TextValue.s_xmlEscaper = null;
		}

		/// <summary>
		/// TextValue constructor
		/// </summary>
		/// <param name="value">value</param>
		public TextValue(string value)
		{
			_value = NormalizeLineBreaks(value);
			_protect = false;
		}

		/// <summary>
		/// TextValue constructor
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
		public TextValue(string value, bool userModifiable)
		{
			_value = NormalizeLineBreaks(value);
			_protect = !userModifiable;
		}

		/// <summary>
		/// Equals description
		/// </summary>
		/// <param name="obj">obj</param>
		/// <returns>True or False</returns>
		public override bool Equals(object obj)
		{
			return (obj is TextValue) ? Equals((TextValue)obj) : false;
		}

		private bool Equals(TextValue operand)
		{
			if (!IsAnswered || !operand.IsAnswered)
				throw new InvalidOperationException();

			return String.Compare(Value, operand.Value, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Gets a hash code for the value.
		/// </summary>
		/// <returns>A hash code for the value.</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		/// <summary>
		/// User-defined implicit conversion from string to TextValue
		/// </summary>
		/// <param name="s">s</param>
		/// <returns>operator</returns>
		public static implicit operator TextValue(string s)
		{
			return new TextValue(s);
		}

		/// <summary>
		/// User-defined implicit conversion from MultipleChoiceValue to TextValue
		/// </summary>
		/// <param name="multipleChoiceValue">multipleChoiceValue</param>
		/// <returns>operator</returns>
		public static implicit operator TextValue(MultipleChoiceValue multipleChoiceValue)
		{
			return multipleChoiceValue.IsAnswered ? new TextValue(multipleChoiceValue.Value) : Unanswered;
		}

		/// <summary>
		/// Indicates the value type.
		/// </summary>
		public ValueType Type
		{
			get { return ValueType.Text; }
		}

		/// <summary>
		/// Indicates if the value is answered.
		/// </summary>
		public bool IsAnswered
		{
			get { return _value != null; }
		}

		/// <summary>
		/// Indicates whether the value should be modifiable by an end user in the interview UI (default is true).
		/// </summary>
		public bool UserModifiable
		{
			get { return !_protect; }
		}

		/// <summary>
		/// Indicates the value.
		/// </summary>
		public string Value
		{
			get { return _value; }
		}

		/// <summary>
		/// Writes the XML representation of the answer.
		/// </summary>
		/// <param name="writer">The XmlWriter to which to write the answer value.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("TextValue");

			if (_protect)
				writer.WriteAttributeString("userModifiable", System.Xml.XmlConvert.ToString(!_protect));

			if (IsAnswered)
				writer.WriteString(_value);
			else
				writer.WriteAttributeString("unans", System.Xml.XmlConvert.ToString(true));

			writer.WriteEndElement();
		}

		internal static string XMLEscape(string text)
		{
			// escape control characters and backslashes in text, for use when outputting value to XML answer file
			// (and ONLY when outputting to attributes, apparently, not element text!)
			if (s_xmlEscaper == null)
				s_xmlEscaper = new Regex(@"[\x00-\x08\x0B-\x0C\x0E-\x1F\\]", RegexOptions.CultureInvariant);
			return s_xmlEscaper.Replace(text,
				delegate(Match m)
				{
					return String.Format("\\{0:x2}", Convert.ToInt32(m.Value[0]));
				});
		}

		internal static string XMLUnescape(string text)
		{
			if (s_xmlUnescaper == null)
				s_xmlUnescaper = new Regex(@"\\[0-9a-fA-F]{2}", RegexOptions.CultureInvariant);
			return s_xmlUnescaper.Replace(text,
				delegate(Match m)
				{
					return ((char)Convert.ToInt32(m.Value.Substring(1), 16)).ToString();
				});
		}

		internal static string NormalizeLineBreaks(string text)
		{
			// always normalize line breaks at the time any text answer is saved into the answer set
			if (!String.IsNullOrEmpty(text) && (text.Contains("\r") || text.Contains("\n")))
			{
				// replace all CRs & LFs with unique tokens
				text = text.Replace('\r', '\u2637');
				text = text.Replace('\n', '\u2630');
				// turn any existing CRLF combinations back to normal
				text = text.Replace("\u2637\u2630", "\r\n");
				// now replace any remaining tokens (formerly bare CRs or LFs) with CRLF combinations
				text = text.Replace("\u2637", "\r\n");
				text = text.Replace("\u2630", "\r\n");
			}
			return text;
		}

#if DEBUG
		// for ease in the debugger...
		// for some reason just having a ToString() seems to work better/a little more smoothly
		// than defining a DebuggerDisplay attribute!
		/// <summary>
		/// Convert to string. 
		/// </summary>
		/// <returns>A string representation of the value.</returns>
		public override string ToString()
		{
			if (IsAnswered)
				return String.Format("\"{0}\"", Value);
			else
				return "Unanswered";
		}
#endif

		#region IConvertible Members

		/// <summary>
		/// Gets the type of value.
		/// </summary>
		/// <returns>The TypeCode for the value.</returns>
		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		/// <summary>
		/// Converts the value to a boolean
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A boolean representation of the answer.</returns>
		public bool ToBoolean(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToBoolean(_value, provider);
		}

		/// <summary>
		/// Converts the value to a byte.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A byte representation of the answer.</returns>
		public byte ToByte(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToByte(_value, provider);
		}

		/// <summary>
		/// Converts the value to a char.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A char representation of the answer.</returns>
		public char ToChar(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToChar(_value, provider);
		}

		/// <summary>
		/// Converts the value to a DateTime.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A DateTime representation of the answer.</returns>
		public DateTime ToDateTime(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToDateTime(_value, provider);
		}

		/// <summary>
		/// Converts the value to a decimal.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A decimal representation of the answer.</returns>
		public decimal ToDecimal(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToDecimal(_value, provider);
		}

		/// <summary>
		/// Converts the value to a double.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A double representation of the answer.</returns>
		public double ToDouble(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToDouble(_value, provider);
		}

		/// <summary>
		/// Converts the value to a 16-bit (short) integer.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A 16-bit (short) integer representation of the answer.</returns>
		public short ToInt16(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToInt16(_value, provider);
		}

		/// <summary>
		/// Converts the value to a 32-bit integer.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A 32-bit (int) integer representation of the answer.</returns>
		public int ToInt32(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToInt32(_value, provider);
		}

		/// <summary>
		/// Converts the value to a 64-bit integer.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A 64-bit (long) integer representation of the answer.</returns>
		public long ToInt64(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToInt64(_value, provider);
		}

		/// <summary>
		/// Converts the value to an sbyte.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>An sbyte representation of the answer.</returns>
		public sbyte ToSByte(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToSByte(_value, provider);
		}

		/// <summary>
		/// Converts the value to a float.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A float representation of the answer.</returns>
		public float ToSingle(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToSingle(_value, provider);
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A string representation of the answer.</returns>
		public string ToString(IFormatProvider provider)
		{
			if (!IsAnswered)
				return null;
			return _value;
		}

		/// <summary>
		/// Converts the value to the specified type.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/Type/param[@name='conversionType']"/>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A representation of the answer in the specified type.</returns>
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType.GUID == GetType().GUID)
				return this;
			switch (conversionType.FullName)
			{
				case "HotDocs.TextValue": return IsAnswered ? new TextValue(ToString(provider)) : TextValue.Unanswered;
				case "HotDocs.NumberValue": return IsAnswered ? new NumberValue(ToDouble(provider)) : NumberValue.Unanswered;
				case "HotDocs.DateValue": return IsAnswered ? new DateValue(ToDateTime(provider)) : DateValue.Unanswered;
				case "HotDocs.TrueFalseValue": return IsAnswered ? new TrueFalseValue(ToBoolean(provider)) : TrueFalseValue.Unanswered;
				case "HotDocs.MultipleChoiceValue": return IsAnswered ? new MultipleChoiceValue(ToString(provider)) : MultipleChoiceValue.Unanswered;
			}
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ChangeType(_value, conversionType, provider);
		}

		/// <summary>
		/// Converts the value to ushort.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A ushort representation of the answer.</returns>
		public ushort ToUInt16(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToUInt16(_value, provider);
		}

		/// <summary>
		/// Converts the value to a uint.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A uint representation of the answer.</returns>
		public uint ToUInt32(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToUInt32(_value, provider);
		}

		/// <summary>
		/// Converts the value to a ulong.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A ulong representation of the answer.</returns>
		public ulong ToUInt64(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToUInt64(_value, provider);
		}

		#endregion
	}
}
