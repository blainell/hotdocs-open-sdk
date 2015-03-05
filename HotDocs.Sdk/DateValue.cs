/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace HotDocs.Sdk
{
	/// <summary>
	/// The DateValue struct is used to represent date values in HotDocs. Since it's a struct, DateValue is a value type
	/// with value semantics. Instances of DateValue directly contain the relevant date, and therefore (unlike reference types) are immutable.
	/// </summary>
	public struct DateValue : IValue, IComparable
	{
		private DateTime? _value;
		private bool _protect;

		/// <summary>
		/// Static (shared) instance of an unanswered DateValue.
		/// </summary>
		public readonly static DateValue Unanswered;
		/// <summary>
		/// Static (shared) instance of an unanswered DateValue that (if shown in the interview UI) should be protected from unintentional modification.
		/// </summary>
		public readonly static DateValue UnansweredLocked;

		/// <summary>
		/// Static constructor required so static fields are always initialized
		/// </summary>
		static DateValue()
		{
			DateValue.Unanswered = new DateValue();
			DateValue.UnansweredLocked = new DateValue(null, false);
		}

		/// <summary>
		/// Constructor that uses a DateTime value.
		/// </summary>
		/// <param name="value">The initial value.</param>
		public DateValue(DateTime? value)
		{
			_value = value;
			_protect = false;
		}

		/// <summary>
		/// Constructor that uses a DateTime value.
		/// </summary>
		/// <param name="value">The initial value.</param>
		/// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
		public DateValue(DateTime? value, bool userModifiable)
		{
			_value = value;
			_protect = !userModifiable;
		}

		/// <summary>
		/// Constructor that uses three parameters for year, month, and day.
		/// </summary>
		/// <param name="year">Year</param>
		/// <param name="month">Month</param>
		/// <param name="day">Day</param>
		public DateValue(int year, int month, int day)
		{
			_value = new DateTime(year, month, day);
			_protect = false;
		}

		/// <summary>
		/// Constructor that uses three parameters for year, month, and day.
		/// </summary>
		/// <param name="year">Year</param>
		/// <param name="month">Month</param>
		/// <param name="day">Day</param>
		/// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
		public DateValue(int year, int month, int day, bool userModifiable)
		{
			_value = new DateTime(year, month, day);
			_protect = !userModifiable;
		}

		/// <summary>
		/// Indicates if the DateValue is equal to another value.
		/// </summary>
		/// <param name="obj">The object to compare the DateValue to.</param>
		/// <returns>A value indicating if the two values are equal or not.</returns>
		public override bool Equals(object obj)
		{
			return (obj is DateValue) ? Equals((DateValue)obj) : false;
		}

		/// <summary>
		/// Indicates if the DateValue is equal to another value.
		/// </summary>
		/// <param name="operand">The value to compare the DateValue to.</param>
		/// <returns>A value indicating if the two values are equal or not.</returns>
		private bool Equals(DateValue operand)
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

		/// <summary>
		/// Allow instances of UnansweredValue to be implicitly converted to unanswered DateValues if necessary.
		/// </summary>
		/// <param name="v">An instance of UnansweredValue</param>
		/// <returns>An equivalent unanswered HotDocs.Sdk.DateValue</returns>
		public static implicit operator DateValue(UnansweredValue v)
		{
			return Unanswered;
		}

		/// <summary>
		/// Allow instances of built-in .NET DateTime struct to be implicitly converted to HotDocs DateValues.
		/// </summary>
		/// <param name="d">An instance of the .NET DateTime struct</param>
		/// <returns>An equivalent instance of HotDocs.Sdk.DateValue</returns>
		public static implicit operator DateValue(DateTime d)
		{
			return new DateValue(d);
		}

		/// <summary>
		/// Allow equality comparison of two DateValues using the == operator.
		/// </summary>
		/// <param name="leftOperand">The first DateValue to compare</param>
		/// <param name="rightOperand">The second DateValue to compare</param>
		/// <returns>A TrueFalseValue indicating whether HotDocs considers the DateValues equal.</returns>
		public static TrueFalseValue operator ==(DateValue leftOperand, DateValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator !=(DateValue leftOperand, DateValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(!leftOperand.Equals(rightOperand)) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <(DateValue leftOperand, DateValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Value < rightOperand.Value) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >(DateValue leftOperand, DateValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Value > rightOperand.Value) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator <=(DateValue leftOperand, DateValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Value <= rightOperand.Value) : TrueFalseValue.Unanswered;
		}

		public static TrueFalseValue operator >=(DateValue leftOperand, DateValue rightOperand)
		{
			return (leftOperand.IsAnswered && rightOperand.IsAnswered) ?
				new TrueFalseValue(leftOperand.Value >= rightOperand.Value) : TrueFalseValue.Unanswered;
		}

		public static DateValue operator +(DateValue leftOperand, TimeSpanValue rightOperand)
		{
			if (!leftOperand.IsAnswered || !rightOperand.IsAnswered)
				return DateValue.Unanswered;

			switch (rightOperand.PeriodType)
			{
				case Period.Days:
					return new DateValue(leftOperand.Value.AddDays(rightOperand.Value));
				case Period.Months:
					return new DateValue(leftOperand.Value.AddMonths(rightOperand.Value));
				case Period.Years:
					return new DateValue(leftOperand.Value.AddYears(rightOperand.Value));
			}
			return DateValue.Unanswered;
		}

		public static DateValue operator -(DateValue leftOperand, TimeSpanValue rightOperand)
		{
			if (!leftOperand.IsAnswered || !rightOperand.IsAnswered)
				return DateValue.Unanswered;

			switch (rightOperand.PeriodType)
			{
				case Period.Days:
					return new DateValue(leftOperand.Value.AddDays(-rightOperand.Value));
				case Period.Months:
					return new DateValue(leftOperand.Value.AddMonths(-rightOperand.Value));
				case Period.Years:
					return new DateValue(leftOperand.Value.AddYears(-rightOperand.Value));
			}
			return DateValue.Unanswered;
		}

		/// <summary>
		/// Indicates the value type.
		/// </summary>
		public ValueType Type
		{
			get { return ValueType.Date; }
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
		/// The value.
		/// </summary>
		public DateTime Value
		{
			get { return _value.Value; }
		}

		/// <summary>
		/// Writes the XML representation of the answer.
		/// </summary>
		/// <param name="writer">The XmlWriter to which to write the answer value.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("DateValue");

			if (_protect)
				writer.WriteAttributeString("userModifiable", System.Xml.XmlConvert.ToString(!_protect));

			if (IsAnswered)
				writer.WriteString(_value.Value.ToString("d/M/yyyy"));
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
				return String.Format("{0:M/d/yyyy}", Value);
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
		/// Converts the DateValue to a boolean
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']"/>
		/// <returns>A boolean representation of the answer.</returns>
		public bool ToBoolean(IFormatProvider provider)
		{
			if (!IsAnswered)
				throw new InvalidCastException();
			return Convert.ToBoolean(_value.Value);
		}

		/// <summary>
		/// Converts the DateValue to a byte.
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
		/// Converts the DateValue to a char.
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
			return _value.Value;
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
		/// Converts the DateValue to an sbyte.
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
		/// Converts the DateValue to the specified type.
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
		/// Converts the DateValue to ushort.
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
            if (!(obj is DateValue))
                return -1;

            DateValue dateValue = (DateValue)obj;
            if (!IsAnswered && !dateValue.IsAnswered)
                return 0;
            if (!IsAnswered)
                return 1;
            if (!dateValue.IsAnswered)
                return -1;

            return DateTime.Compare(Value, dateValue.Value);
        }

        #endregion

	}
}

