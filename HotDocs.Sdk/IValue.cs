/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;

namespace HotDocs.Sdk
{
	/// <summary>
	/// An enumeration for the various types of HotDocs answer values.
	/// </summary>
	public enum ValueType {
		/// <summary>
		/// A value of some type that is not directly understood by the Open SDK.
        /// </summary>
		Unknown = 0,
        /// <summary>
        /// A value of some type that is not directly understood by the Open SDK.
        /// This is a synonym for Unknown, and is included only for backwards compatibility.
        /// </summary>
        Other = 0,
		/// <summary>
		/// A text value. HotDocs Text variables have answers of this type.
		/// </summary>
		Text = 1,
		/// <summary>
		/// A numeric value. HotDocs Number variables have answers of this type.
		/// </summary>
		Number,
		/// <summary>
		/// A date value. HotDocs Date variables have answers of this type.
		/// </summary>
		Date,
		/// <summary>
		/// A true/false (Boolean) value. HotDocs True/False variables and grouped child dialogs have answers of this type.
		/// </summary>
		TrueFalse,
		/// <summary>
		/// A multiple choice value, which is represented by an array of one or more text strings.
        /// HotDocs Multiple Choice variables have answers of this type.
		/// </summary>
		MultipleChoice };

	/// <summary>
	/// An interface for a HotDocs value.
	/// </summary>
	public interface IValue : IConvertible, IComparable
	{
		/// <summary>
		/// Indicates the value type.
		/// </summary>
		ValueType Type { get; }
	
		/// <summary>
		/// Indicates if the value is answered or not.
		/// </summary>
		bool IsAnswered { get; }

		/// <summary>
		/// Indicates whether the value should be modifiable by an end user in the interview UI (default is true).
		/// </summary>
		bool UserModifiable { get; }

		/// <summary>
		/// Writes the value to XML.
		/// </summary>
		/// <param name="writer">XmlWriter to which to write the value.</param>
		void WriteXml(System.Xml.XmlWriter writer);
	}

	public static class ValueConverter
	{
		public static TOut Convert<TIn,TOut>(TIn value)
			where TIn : IValue
			where TOut : IValue
		{
			if (value is TOut)
				return (TOut)(IValue)value; // boxing & unboxing facilitates relatively quick coersion/conversion from TIn to TOut

			throw new InvalidCastException(String.Format("Invalid cast from {0} to {1}.", typeof(TIn).Name, typeof(TOut).Name));
		}
	}
}
