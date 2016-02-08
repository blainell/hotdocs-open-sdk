/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     The NumberValue struct is used to represent numeric values in HotDocs. Since it's a struct, NumberValue is a value
    ///     type
    ///     with value semantics. Instances of NumberValue directly contain a floating point value, and therefore (unlike
    ///     reference types) are immutable.
    /// </summary>
    /// <remarks>
    ///     Also included as part of NumberValue is code that "fudges" numeric values to eliminate the rounding errors
    ///     typical of floating-point arithmetic on digital computers. This is possible because HotDocs supports only up to
    ///     seven decimal
    ///     places of precision, and the relevant "fudging" takes place five decimal places beyond that necessary level of
    ///     precision.
    /// </remarks>
    public struct NumberValue : IValue, IComparable
    {
        private readonly double? _value;
        private readonly bool _protect;

        /// <summary>
        ///     Static (shared) instance of an unanswered NumberValue.
        /// </summary>
        public static readonly NumberValue Unanswered;

        /// <summary>
        ///     Static (shared) instance of an unanswered NumberValue that would be protected from unintentional modification in
        ///     the interview UI.
        /// </summary>
        public static readonly NumberValue UnansweredLocked;

        /// <summary>
        ///     Static constructor required so static fields are always initialized
        /// </summary>
        static NumberValue()
        {
            Unanswered = new NumberValue();
            UnansweredLocked = new NumberValue(null, false);
        }

        /// <summary>
        ///     NumberValue constructor
        /// </summary>
        /// <param name="value">value</param>
        public NumberValue(double? value)
        {
            _value = value;
            _protect = false;
        }

        /// <summary>
        ///     NumberValue constructor
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
        public NumberValue(double? value, bool userModifiable)
        {
            _value = value;
            _protect = !userModifiable;
        }

        /// <summary>
        ///     NumberValue constructor
        /// </summary>
        /// <param name="value">value</param>
        public NumberValue(int? value)
        {
            if (value.HasValue)
                _value = value.Value;
            else
                _value = null;
            _protect = false;
        }

        /// <summary>
        ///     NumberValue constructor
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="userModifiable">Whether this value should be modifiable by end users in the interview UI.</param>
        public NumberValue(int? value, bool userModifiable)
        {
            if (value.HasValue)
                _value = value.Value;
            else
                _value = null;
            _protect = !userModifiable;
        }

        /// <summary>
        ///     Equals description
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns>True or False</returns>
        public override bool Equals(object obj)
        {
            return obj is NumberValue ? Equals((NumberValue) obj) : false;
        }

        private bool Equals(NumberValue operand)
        {
            if (!IsAnswered || !operand.IsAnswered)
                throw new InvalidOperationException();

            return FudgeDouble(Value, 7, true).Equals(FudgeDouble(operand.Value, 7, true));
        }

        /// <summary>
        ///     Gets a hash code for the value.
        /// </summary>
        /// <returns>A hash code for the value.</returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static implicit operator NumberValue(UnansweredValue v)
        {
            return Unanswered;
        }

        /// <summary>
        ///     NumberValue summary
        /// </summary>
        /// <param name="d">d</param>
        /// <returns>operator</returns>
        public static implicit operator NumberValue(double d)
        {
            return new NumberValue(d);
        }

        /// <summary>
        ///     NumberValue summary
        /// </summary>
        /// <param name="i">i</param>
        /// <returns>operator</returns>
        public static implicit operator NumberValue(int i)
        {
            return new NumberValue((double) i);
        }

        public static implicit operator NumberValue(TextValue textValue)
        {
            if (!textValue.IsAnswered)
                return Unanswered;

            double num;
            return double.TryParse(textValue.Value, out num) ? new NumberValue(num) : Unanswered;
        }

        public static implicit operator NumberValue(MultipleChoiceValue multipleChoiceValue)
        {
            if (!multipleChoiceValue.IsAnswered)
                return Unanswered;

            double num;
            return double.TryParse(multipleChoiceValue.Value, out num) ? new NumberValue(num) : Unanswered;
        }

        // Unary negation
        public static NumberValue operator -(NumberValue operand)
        {
            return operand.IsAnswered
                ? new NumberValue(-operand.Value)
                : Unanswered;
        }

        public static NumberValue operator +(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new NumberValue(leftOperand.Value + rightOperand.Value)
                : Unanswered;
        }

        public static NumberValue operator -(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new NumberValue(leftOperand.Value - rightOperand.Value)
                : Unanswered;
        }

        public static NumberValue operator *(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new NumberValue(leftOperand.Value*rightOperand.Value)
                : Unanswered;
        }

        public static NumberValue operator /(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new NumberValue(leftOperand.Value/rightOperand.Value)
                : Unanswered;
        }

        public static TrueFalseValue operator ==(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new TrueFalseValue(leftOperand.Equals(rightOperand))
                : TrueFalseValue.Unanswered;
        }

        public static TrueFalseValue operator !=(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new TrueFalseValue(!leftOperand.Equals(rightOperand))
                : TrueFalseValue.Unanswered;
        }

        public static TrueFalseValue operator <(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new TrueFalseValue(FudgeDouble(leftOperand.Value, 7, true) < FudgeDouble(rightOperand.Value, 7, true))
                : TrueFalseValue.Unanswered;
        }

        public static TrueFalseValue operator >(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new TrueFalseValue(FudgeDouble(leftOperand.Value, 7, true) > FudgeDouble(rightOperand.Value, 7, true))
                : TrueFalseValue.Unanswered;
        }

        public static TrueFalseValue operator <=(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new TrueFalseValue(FudgeDouble(leftOperand.Value, 7, true) <= FudgeDouble(rightOperand.Value, 7, true))
                : TrueFalseValue.Unanswered;
        }

        public static TrueFalseValue operator >=(NumberValue leftOperand, NumberValue rightOperand)
        {
            return leftOperand.IsAnswered && rightOperand.IsAnswered
                ? new TrueFalseValue(FudgeDouble(leftOperand.Value, 7, true) >= FudgeDouble(rightOperand.Value, 7, true))
                : TrueFalseValue.Unanswered;
        }

        /// <summary>
        ///     Indicates the value type.
        /// </summary>
        public ValueType Type
        {
            get { return ValueType.Number; }
        }

        /// <summary>
        ///     Indicates if the value is answered.
        /// </summary>
        public bool IsAnswered
        {
            get { return _value.HasValue; }
        }

        /// <summary>
        ///     Indicates whether the value should be modifiable by an end user in the interview UI (default is true).
        /// </summary>
        public bool UserModifiable
        {
            get { return !_protect; }
        }

        /// <summary>
        ///     Indicates the value.
        /// </summary>
        public double Value
        {
            get { return _value.Value; }
        }

        /// <summary>
        ///     Writes the XML representation of the answer.
        /// </summary>
        /// <param name="writer">The XmlWriter to which to write the answer value.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("NumValue");

            if (_protect)
                writer.WriteAttributeString("userModifiable", XmlConvert.ToString(!_protect));

            if (IsAnswered)
                writer.WriteString(FudgeDouble(Value, 7, false).ToString(CultureInfo.InvariantCulture));
            else
                writer.WriteAttributeString("unans", XmlConvert.ToString(true));

            writer.WriteEndElement();
        }

        private static readonly double[] s_floatFudgeFactors =
        {
            1.0e-05, //	1.0e-02,
            1.0e-06, //	1.0e-03,
            1.0e-07, //	1.0e-04,
            1.0e-08, //	1.0e-05,
            1.0e-09, //	1.0e-06,
            1.0e-10, //	1.0e-07,
            1.0e-11, //	1.0e-08,
            1.0e-12, //	1.0e-09,
            1.0e-13 //	1.0e-10
        };

        private static readonly int[] s_powTenTable =
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000
        };

        // This is perhaps goofy but we need to replicate exactly what we do in desktop HotDocs.
        public static double FudgeDouble(double number, int decimalPlaces, bool round)
        {
            Debug.Assert((decimalPlaces >= 0) && (decimalPlaces <= 7));

            var fractionalPart = 0.0;
            if (number < 0.0)
                fractionalPart = number - Math.Ceiling(number);
            else
                fractionalPart = number - Math.Floor(number);

            if (fractionalPart != 0.0)
            {
                // The double has a fractional part.
                if (number < 0.0)
                    number -= s_floatFudgeFactors[decimalPlaces];
                else
                    number += s_floatFudgeFactors[decimalPlaces];
                var multiplier = s_powTenTable[decimalPlaces];
                number *= multiplier;
                if (round)
                {
                    if (number < 0.0)
                        number -= 0.5;
                    else
                        number += 0.5;
                }
                number = number < 0.0 ? Math.Ceiling(number) : Math.Floor(number);
                number = number/multiplier;
            }
            return number;
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
        ///     Gets the type of value.
        /// </summary>
        /// <returns>The TypeCode for the value.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        /// <summary>
        ///     Converts the value to a boolean
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A boolean representation of the answer.</returns>
        public bool ToBoolean(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToBoolean(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a byte.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A byte representation of the answer.</returns>
        public byte ToByte(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToByte(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a char.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A char representation of the answer.</returns>
        public char ToChar(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToChar(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a DateTime.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A DateTime representation of the answer.</returns>
        public DateTime ToDateTime(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToDateTime(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a decimal.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A decimal representation of the answer.</returns>
        public decimal ToDecimal(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToDecimal(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a double.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A double representation of the answer.</returns>
        public double ToDouble(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return _value.Value;
        }

        /// <summary>
        ///     Converts the value to a 16-bit (short) integer.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A 16-bit (short) integer representation of the answer.</returns>
        public short ToInt16(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToInt16(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a 32-bit integer.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A 32-bit (int) integer representation of the answer.</returns>
        public int ToInt32(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToInt32(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a 64-bit integer.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A 64-bit (long) integer representation of the answer.</returns>
        public long ToInt64(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToInt64(_value.Value);
        }

        /// <summary>
        ///     Converts the value to an sbyte.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>An sbyte representation of the answer.</returns>
        public sbyte ToSByte(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToSByte(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a float.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A float representation of the answer.</returns>
        public float ToSingle(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToSingle(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a string.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A string representation of the answer.</returns>
        public string ToString(IFormatProvider provider)
        {
            if (!IsAnswered)
                return null;
            return Convert.ToString(_value.Value, provider);
        }

        /// <summary>
        ///     Converts the value to the specified type.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/Type/param[@name='conversionType']" />
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A representation of the answer in the specified type.</returns>
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType.GUID == GetType().GUID)
                return this;
            switch (conversionType.FullName)
            {
                case "HotDocs.Sdk.TextValue":
                    return IsAnswered ? new TextValue(ToString(provider)) : TextValue.Unanswered;
                case "HotDocs.Sdk.NumberValue":
                    return IsAnswered ? new NumberValue(ToDouble(provider)) : Unanswered;
                case "HotDocs.Sdk.DateValue":
                    return IsAnswered ? new DateValue(ToDateTime(provider)) : DateValue.Unanswered;
                case "HotDocs.Sdk.TrueFalseValue":
                    return IsAnswered ? new TrueFalseValue(ToBoolean(provider)) : TrueFalseValue.Unanswered;
                case "HotDocs.Sdk.MultipleChoiceValue":
                    return IsAnswered ? new MultipleChoiceValue(ToString(provider)) : MultipleChoiceValue.Unanswered;
            }
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ChangeType(_value, conversionType, provider);
        }

        /// <summary>
        ///     Converts the value to ushort.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A ushort representation of the answer.</returns>
        public ushort ToUInt16(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToUInt16(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a uint.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
        /// <returns>A uint representation of the answer.</returns>
        public uint ToUInt32(IFormatProvider provider)
        {
            if (!IsAnswered)
                throw new InvalidCastException();
            return Convert.ToUInt32(_value.Value);
        }

        /// <summary>
        ///     Converts the value to a ulong.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/IFormatProvider/param[@name='provider']" />
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
            if (!(obj is NumberValue))
                return -1;

            var numberValue = (NumberValue) obj;
            if (!IsAnswered && !numberValue.IsAnswered)
                return 0;
            if (!IsAnswered)
                return 1;
            if (!numberValue.IsAnswered)
                return -1;

            return FudgeDouble(Value, 7, true).CompareTo(FudgeDouble(numberValue.Value, 7, true));
        }

        #endregion
    }
}