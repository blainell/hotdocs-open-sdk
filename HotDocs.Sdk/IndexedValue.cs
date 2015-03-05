/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace HotDocs.Sdk
{
	/// <summary>
	/// An IndexedValue represents a single value within a HotDocs Answer object containing multiple values.
	/// (A so-called "Repeated Answer".)  IndexedValues are used to facilitate easy iteration of the values
	/// within a repeated answer -- see the Answer.IndexedValues property.
	/// </summary>
	public struct IndexedValue
	{
		private readonly Answer _answer;
		private readonly int[] _repeatIndices;

		internal IndexedValue(Answer answer, int[] repeatIndices)
		{
			_answer = answer;
			_repeatIndices = repeatIndices;
		}

		/// <summary>
		/// The data type of this value.
		/// </summary>
		public ValueType Type { get { return _answer.Type; } }

		/// <summary>
		/// A type-specific getter to retrieve the actual value encapsulated by this Value object.
		/// Where performance is a concern and the data type of the value is known, this accessor
		/// should be used in preference to the Value property.
		/// </summary>
		/// <typeparam name="T">The type of value encapsulated by this Value:  either TextValue, NumberValue,
		/// DateValue, TrueFalseValue or MultipleChoiceValue.</typeparam>
		/// <returns>An instance of the given type, containing the value encapsulated by this Value object.</returns>
		public T GetValue<T>() where T : IValue
		{
			return _answer.GetValue<T>(_repeatIndices);
		}

		/// <summary>
		/// A convenience accessor to set or retrieve the actual value stored in this Value object.
		/// It must be either a TextValue, NumberValue, DateValue, TrueFalseValue or MultipleChoiceValue.
		/// </summary>
		public IValue Value
		{
			get { return _answer.GetValue(_repeatIndices); }

			set
			{
				Debug.Assert(value.Type == _answer.Type);
				if (value.Type == _answer.Type)
				{
					switch (value.Type)
					{
						case ValueType.Text:
							_answer.SetValue<TextValue>((TextValue)value, _repeatIndices);
							break;
						case ValueType.Number:
							_answer.SetValue<NumberValue>((NumberValue)value, _repeatIndices);
							break;
						case ValueType.Date:
							_answer.SetValue<DateValue>((DateValue)value, _repeatIndices);
							break;
						case ValueType.TrueFalse:
							_answer.SetValue<TrueFalseValue>((TrueFalseValue)value, _repeatIndices);
							break;
						case ValueType.MultipleChoice:
							_answer.SetValue<MultipleChoiceValue>((MultipleChoiceValue)value, _repeatIndices);
							break;
					}
				}
			}
		}

		/// <summary>
		/// The repeat indexes that identify this Value within the Answer object of which it is a part.
		/// </summary>
		public int[] RepeatIndices
		{
			get { return _repeatIndices; }
		}

		/// <summary>
		/// The Answer object of which this Value is a part.
		/// </summary>
		public Answer Answer
		{
			get { return _answer; }
		}
	}
}
