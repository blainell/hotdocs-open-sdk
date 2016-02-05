/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace HotDocs.Sdk
{
	/// <summary>
	/// The MultipleChoiceValue struct is used to represent multiple choice values in HotDocs. Since it's a struct, MultipleChoiceValue is a value type
	/// with value semantics. Instances of MultipleChoiceValue directly contain a list of selected option strings, and therefore (unlike reference types) are immutable.
	/// </summary>
	public struct MultipleChoiceValue : IValue, IComparable
	{
		private readonly string[] _value;
		private readonly bool _protect;
		private const string s_separator = "|";

		/// <summary>
		/// Static (shared) instance of an unanswered MultipleChoiceValue.
		/// </summary>
		public readonly static MultipleChoiceValue Unanswered;
		/// <summary>
		/// Static (shared) instance of an unanswered MultipleChoiceValue that would be protected from unintentional modification in the interview UI.
		/// </summary>
		public readonly static MultipleChoiceValue UnansweredLocked;

		/// <summary>
		/// A string which represents the "None of the Above" option in a Multiple Choice variable.
		/// </summary>
		public const string NoneOfTheAbove = "None of the Above";

		/// <summary>
		/// Static constructor required so static fields are always initialized
		/// </summary>
		static MultipleChoiceValue()
		{
			MultipleChoiceValue.Unanswered = new MultipleChoiceValue();
			MultipleChoiceValue.UnansweredLocked = new MultipleChoiceValue((string)null, false);
		}

		/// <summary>
		/// MultipleChoiceValue constructor
		/// </summary>
		/// <param name="value">value</param>
		public MultipleChoiceValue(string value)
		{
			if (value == null)
				_value = null;
			else
				_value = value.Split(s_separator[0]); // passing in an empty string creates a string list with one empty entry
			_protect = false;
		}

		/// <summary>
		/// MultipleChoiceValue constructor
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
		public MultipleChoiceValue(string value, bool userModifiable)
		{
			if (value == null)
				_value = null;
			else
				_value = value.Split(s_separator[0]); // passing in an empty string creates a string list with one empty entry
			_protect = !userModifiable;
		}

		/// <summary>
		/// MultipleChoiceValue constructor
		/// </summary>
		/// <param name="value">value</param>
		public MultipleChoiceValue(string[] value)
		{
			if ((value == null) || (value.Length == 0))
				_value = null;
			else
				_value = value;
			_protect = false;
		}

		/// <summary>
		/// MultipleChoiceValue constructor
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
		public MultipleChoiceValue(string[] value, bool userModifiable)
		{
			if ((value == null) || (value.Length == 0))
				_value = null;
			else
				_value = value;
			_protect = !userModifiable;
		}

		/// <summary>
		/// Equals description
		/// </summary>
		/// <param name="obj">obj</param>
		/// <returns>True or False</returns>
		public override bool Equals(object obj)
		{
			if (obj is TextValue)
				return Equals((TextValue)obj);
			if (obj is MultipleChoiceValue)
				return Equals((MultipleChoiceValue)obj);
			return false;
		}

		/// <summary>
		/// Equals summary
		/// </summary>
		/// <param name="operand">operand</param>
		/// <returns>True or False</returns>
		private bool Equals(TextValue operand)
		{
			if (!IsAnswered || !operand.IsAnswered)
				throw new InvalidOperationException();

			for (int i = 0; i < _value.Length; i++)
			{
				if (String.Compare(_value[i], operand.Value, StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			}
			return false;
		}

		// This code matches desktop HotDocs which operates as follows:
		//	1. It assumes that there are no duplicate strings in the same MultipleChoiceValue.
		//	2. MCV1 is equal to MCV2 if every string in MCV1 is contained exactly once in MCV2
		//		and every string in MCV2 is contained exactly once in MCV1.
		//	3. All string comparisons ignore case, that is, are case insensitive.
		private bool Equals(MultipleChoiceValue operand)
		{
			if (!IsAnswered || !operand.IsAnswered)
				throw new InvalidOperationException();

			if (_value.Length != operand._value.Length)
				return false;

			for (int i = 0; i < _value.Length; i++)
			{
				int j = 0;
				while ((j < _value.Length) && (String.Compare(_value[i], operand._value[j],
					StringComparison.OrdinalIgnoreCase) != 0)) j++;
				if (j >= _value.Length)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Gets a hash code for the value.
		/// </summary>
		/// <returns>A hash code for the value.</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public static implicit operator MultipleChoiceValue(UnansweredValue v)
		{
			return Unanswered;
		}

		/// <summary>
		/// MultipleChoiceValue summary
		/// </summary>
		/// <param name="textValue">textValue</param>
		/// <returns>operator</returns>
		public static implicit operator MultipleChoiceValue(TextValue textValue)
		{
			return textValue.IsAnswered ? new MultipleChoiceValue(textValue.Value) : Unanswered;
		}

		/// <summary>
		/// Implicit operator for converting string[] to MultipleChoiceValue.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>operator</returns>
		static public implicit operator MultipleChoiceValue(string[] value)
		{
			return new MultipleChoiceValue(value);
		}

		/// <summary>
		/// Implicit operator for converting List/<string/> to MultipleChoiceValue.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>operator</returns>
		static public implicit operator MultipleChoiceValue(List<string> value)
		{
			return new MultipleChoiceValue(value.ToArray());
		}

		public static TextValue operator +(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TextValue(leftOperand.Value + rightOperand.Value) : TextValue.Unanswered;
		}

		public static TextValue operator +(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TextValue(leftOperand.Value + rightOperand.Value) : TextValue.Unanswered;
		}

		public static TextValue operator +(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TextValue(leftOperand.Value + rightOperand.Value) : TextValue.Unanswered;
		}

		public static TrueFalseValue operator ==(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator ==(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator ==(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(rightOperand.Equals(leftOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator !=(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(!leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator !=(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(!leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator !=(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(!rightOperand.Equals(leftOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) < 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) < 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) < 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) > 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) > 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) > 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <=(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) <= 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <=(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) <= 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <=(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) <= 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >=(MultipleChoiceValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) >= 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >=(MultipleChoiceValue leftOperand, TextValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) >= 0) :
				TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >=(TextValue leftOperand, MultipleChoiceValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(String.Compare(leftOperand.Value, rightOperand.Value, StringComparison.OrdinalIgnoreCase) >= 0) :
				TrueFalseValue.Unanswered;
		}

		/// <summary>
		/// Indicates the value type.
		/// </summary>
		public ValueType Type
		{
			get { return ValueType.MultipleChoice; }
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
			get { return String.Join(s_separator, _value); }
		}

		/// <summary>
		/// An array of choices selected from the Multiple Choice variable.
		/// </summary>
		public string[] Choices
		{
			get { return _value; }
		}

		public string ToString(string[] format)
		{
			if (!IsAnswered)
				throw new InvalidOperationException();

			// Input convention for format[]
			// format[0] = group format text or empty text if no group format.
			// format[1] - format[n] = merge text values.
			StringBuilder strBuilder = new StringBuilder();
			for (int i = 0; i < Choices.Length; i++)
			{
				if (i > 0)
					strBuilder.Append(Environment.NewLine);

				if ((format != null) && ((i + 1) < format.Length) && !String.IsNullOrEmpty(format[i + 1]))
					strBuilder.Append(format[i + 1]);
				else
					strBuilder.Append(Choices[i]);
			}

			return strBuilder.ToString();
		}

		/// <summary>
		/// Writes the XML representation of the answer.
		/// </summary>
		/// <param name="writer">The XmlWriter to which to write the answer value.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("MCValue");

			if (_protect)
				writer.WriteAttributeString("userModifiable", System.Xml.XmlConvert.ToString(!_protect));

			if (IsAnswered)
			{
				for (int i = 0; i < _value.Length; i++)
					writer.WriteElementString("SelValue", TextValue.XMLEscape(_value[i]));
			}
			else
				writer.WriteAttributeString("unans", System.Xml.XmlConvert.ToString(true));

			writer.WriteEndElement();
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
			return Convert.ToBoolean(_value.Length);
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
			return Convert.ToByte(_value.Length);
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
			return Convert.ToChar(Value, provider);
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
			return Convert.ToDateTime(Value, provider);
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
			return Convert.ToDecimal(_value.Length);
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
			return Convert.ToDouble(_value.Length);
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
			return Convert.ToInt16(_value.Length);
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
			return _value.Length;
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
			return Convert.ToInt64(_value.Length);
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
			return Convert.ToSByte(_value.Length);
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
			return Convert.ToSingle(_value.Length);
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
			return Value;
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
				case "HotDocs.Sdk.TextValue": return IsAnswered ? new TextValue(ToString(provider)) : TextValue.Unanswered;
				case "HotDocs.Sdk.NumberValue": return IsAnswered ? new NumberValue(ToDouble(provider)) : NumberValue.Unanswered;
				case "HotDocs.Sdk.DateValue": return IsAnswered ? new DateValue(ToDateTime(provider)) : DateValue.Unanswered;
				case "HotDocs.Sdk.TrueFalseValue": return IsAnswered ? new TrueFalseValue(ToBoolean(provider)) : TrueFalseValue.Unanswered;
				case "HotDocs.Sdk.MultipleChoiceValue": return IsAnswered ? new MultipleChoiceValue(ToString(provider)) : MultipleChoiceValue.Unanswered;
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
			return Convert.ToUInt16(_value.Length);
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
			return Convert.ToUInt32(_value.Length);
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
			return Convert.ToUInt64(_value.Length);
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (!(obj is MultipleChoiceValue))
				return -1;

			MultipleChoiceValue multipleChoiceValue = (MultipleChoiceValue)obj;
			if (!IsAnswered && !multipleChoiceValue.IsAnswered)
				return 0;
			if (!IsAnswered)
				return 1;
			if (!multipleChoiceValue.IsAnswered)
				return -1;

			if (Equals(multipleChoiceValue.Value))
				return 0;

			return String.Compare(Value, multipleChoiceValue.Value, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

	}
}
