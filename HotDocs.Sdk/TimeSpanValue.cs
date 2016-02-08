/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     The TimeSpanValue struct is part of the infrastructure related to how Date values are stored and manipulated within
    ///     HotDocs.
    ///     A TimeSpanValue will never appear on its own in a HotDocs answer file, but they are used to adjust DateValues by a
    ///     relative amount.
    /// </summary>
    public struct TimeSpanValue
    {
        private readonly int? _value;

        /// <summary>
        ///     Static (shared) instance of an unanswered TimeSpanValue.
        /// </summary>
        public static readonly TimeSpanValue Unanswered;

        /// <summary>
        ///     Static constructor required so static fields are always initialized
        /// </summary>
        static TimeSpanValue()
        {
            Unanswered = new TimeSpanValue();
        }

        public TimeSpanValue(NumberValue value, Period period)
        {
            if (value.IsAnswered)
            {
                _value = (int) value.Value;
                PeriodType = period;
            }
            else
            {
                _value = null;
                PeriodType = Period.Unknown;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TimeSpanValue ? Equals((TimeSpanValue) obj) : false;
        }

        private bool Equals(TimeSpanValue operand)
        {
            if (!IsAnswered || !operand.IsAnswered)
                throw new InvalidOperationException();

            return (_value == operand._value) && (PeriodType == operand.PeriodType);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static UnansweredValue operator +(UnansweredValue leftOperand, TimeSpanValue rightOperand)
        {
            return new UnansweredValue();
        }

        public static UnansweredValue operator -(UnansweredValue leftOperand, TimeSpanValue rightOperand)
        {
            return new UnansweredValue();
        }

        public bool IsAnswered
        {
            get { return _value.HasValue; }
        }

        public int Value
        {
            get { return _value.Value; }
        }

        public Period PeriodType { get; }
    }
}