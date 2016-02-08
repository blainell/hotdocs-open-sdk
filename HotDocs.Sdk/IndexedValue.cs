/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.Diagnostics;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     An IndexedValue represents a single value within a HotDocs Answer object containing multiple values.
    ///     (A so-called "Repeated Answer".)  IndexedValues are used to facilitate easy iteration of the values
    ///     within a repeated answer -- see the Answer.IndexedValues property.
    /// </summary>
    public struct IndexedValue
    {
        internal IndexedValue(IAnswer answer, int[] repeatIndices)
        {
            Answer = answer;
            RepeatIndices = repeatIndices;
        }

        /// <summary>
        ///     The data type of this value.
        /// </summary>
        public ValueType Type
        {
            get { return Answer.Type; }
        }

        /// <summary>
        ///     A type-specific getter to retrieve the actual value encapsulated by this Value object.
        ///     Where performance is a concern and the data type of the value is known, this accessor
        ///     should be used in preference to the Value property.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of value encapsulated by this Value:  either TextValue, NumberValue,
        ///     DateValue, TrueFalseValue or MultipleChoiceValue.
        /// </typeparam>
        /// <returns>An instance of the given type, containing the value encapsulated by this Value object.</returns>
        public T GetValue<T>() where T : IValue
        {
            return Answer.GetValue<T>(RepeatIndices);
        }

        /// <summary>
        ///     A convenience accessor to set or retrieve the actual value stored in this Value object.
        ///     It must be either a TextValue, NumberValue, DateValue, TrueFalseValue or MultipleChoiceValue.
        /// </summary>
        public IValue Value
        {
            get { return Answer.GetValue(RepeatIndices); }

            set
            {
                Debug.Assert(value.Type == Answer.Type);
                if (value.Type == Answer.Type)
                {
                    switch (value.Type)
                    {
                        case ValueType.Text:
                            Answer.SetValue((TextValue) value, RepeatIndices);
                            break;
                        case ValueType.Number:
                            Answer.SetValue((NumberValue) value, RepeatIndices);
                            break;
                        case ValueType.Date:
                            Answer.SetValue((DateValue) value, RepeatIndices);
                            break;
                        case ValueType.TrueFalse:
                            Answer.SetValue((TrueFalseValue) value, RepeatIndices);
                            break;
                        case ValueType.MultipleChoice:
                            Answer.SetValue((MultipleChoiceValue) value, RepeatIndices);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     The repeat indexes that identify this Value within the Answer object of which it is a part.
        /// </summary>
        public int[] RepeatIndices { get; }

        /// <summary>
        ///     The Answer object of which this Value is a part.
        /// </summary>
        public IAnswer Answer { get; }
    }
}