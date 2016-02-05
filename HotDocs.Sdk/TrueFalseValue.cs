/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Diagnostics;

namespace HotDocs.Sdk
{
	/// <summary>
	/// The TrueFalseValue struct is used to represent boolean (true/false) values in HotDocs. Since it's a struct, TrueFalseValue is a value type
	/// with value semantics. Instances of TrueFalseValue directly contain a boolean value, and therefore (unlike reference types) are immutable.
	/// </summary>
	public struct TrueFalseValue : IValue, IComparable
	{
		private readonly bool? _value;
		private readonly bool _protect;

		/// <summary>
		/// Static (shared) instance of an unanswered TrueFalseValue.
		/// </summary>
		public readonly static TrueFalseValue Unanswered;
		/// <summary>
		/// Static (shared) instance of an unanswered TrueFalseValue that would be protected from unintentional modification in the interview UI.
		/// </summary>
		public readonly static TrueFalseValue UnansweredLocked;
		
		/// <summary>
		/// Static (shared) instance of the True value.
		/// </summary>
		public readonly static TrueFalseValue True;
		
		/// <summary>
		/// Static (shared) instance of the False value.
		/// </summary>
		public readonly static TrueFalseValue False;

		/// <summary>
		/// Static constructor required so static fields are always initialized
		/// </summary>
		static TrueFalseValue()
		{
			TrueFalseValue.Unanswered = new TrueFalseValue();
			TrueFalseValue.UnansweredLocked = new TrueFalseValue(null, false);
			TrueFalseValue.True = new TrueFalseValue(true);
			TrueFalseValue.False = new TrueFalseValue(false);
		}

		/// <summary>
		/// TrueFalseValue constructor
		/// </summary>
		/// <param name="value">The initial value.</param>
		public TrueFalseValue(bool? value)
		{
			_value = value;
			_protect = false;
		}

		/// <summary>
		/// TrueFalseValue constructor
		/// </summary>
		/// <param name="value">The initial value.</param>
		/// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
		public TrueFalseValue(bool? value, bool userModifiable)
		{
			_value = value;
			_protect = !userModifiable;
		}

		/// <summary>
		/// Determines if two values are equal.
		/// </summary>
		/// <param name="obj">The value to compare with.</param>
		/// <returns>True or False</returns>
		public override bool Equals(object obj)
		{
			return (obj is TrueFalseValue) ? Equals((TrueFalseValue)obj) : false;
		}

		private bool Equals(TrueFalseValue operand)
		{
			if (!IsAnswered || !operand.IsAnswered)
				throw new InvalidOperationException();

			return Value.Equals(operand.Value);
		}

		/// <summary>
		/// Gets a hash code for the value.
		/// </summary>
		/// <returns>A hash code for the value.</returns>
		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public static implicit operator TrueFalseValue(UnansweredValue v)
		{
			return Unanswered;
		}

		/// <summary>
		/// TrueFalseValue operator
		/// </summary>
		/// <param name="b">b</param>
		/// <returns>TrueFalse value</returns>
		public static implicit operator TrueFalseValue(bool b)
		{
			return new TrueFalseValue(b);
		}

		public static implicit operator bool(TrueFalseValue v)
		{
			return v.IsAnswered ? v.Value : false;
		}

		public static TrueFalseValue operator ==(TrueFalseValue leftOperand, TrueFalseValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator !=(TrueFalseValue leftOperand, TrueFalseValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(!leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator !(TrueFalseValue operand)
		{
			return (operand.IsAnswered) ?
				new TrueFalseValue(!operand.Value) : TrueFalseValue.Unanswered;
		}

		/// <summary>
		/// Indicates the value type.
		/// </summary>
		public ValueType Type
		{
			get { return ValueType.TrueFalse; }
		}

		/// <summary>
		/// Indicates if the value is answered.
		/// </summary>
		public bool IsAnswered
		{
			get { return _value.HasValue; }
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
		public bool Value
		{
			get { return _value.Value; }
		}

		/// <summary>
		/// Writes the XML representation of the answer.
		/// </summary>
		/// <param name="writer">The XmlWriter to which to write the answer value.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("TFValue");

			if (_protect)
				writer.WriteAttributeString("userModifiable", System.Xml.XmlConvert.ToString(!_protect));

			if (IsAnswered)
				writer.WriteString(System.Xml.XmlConvert.ToString(_value.Value));
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
				return Value.ToString();
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
			return _value.Value;
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
			return Convert.ToByte(_value.Value);
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
			return Convert.ToChar(_value.Value);
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
			return Convert.ToDateTime(_value.Value);
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
			return Convert.ToDecimal(_value.Value);
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
			return Convert.ToDouble(_value.Value);
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
			return Convert.ToInt16(_value.Value);
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
			return Convert.ToInt32(_value.Value);
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
			return Convert.ToInt64(_value.Value);
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
			return Convert.ToSByte(_value.Value);
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
			return Convert.ToSingle(_value.Value);
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
			return Convert.ToString(_value.Value, provider);
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
			return Convert.ToUInt16(_value.Value);
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
			return Convert.ToUInt32(_value.Value);
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
			return Convert.ToUInt64(_value.Value);
		}

		#endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is TrueFalseValue))
                return -1;

            TrueFalseValue trueFalseValue = (TrueFalseValue)obj;
            if (!IsAnswered && !trueFalseValue.IsAnswered)
                return 0;
            if (!IsAnswered)
                return 1;
            if (!trueFalseValue.IsAnswered)
                return -1;

            if (Value == trueFalseValue.Value)
                return 0;
            return Value ? -1 : 1;
        }

        #endregion

	}
}
