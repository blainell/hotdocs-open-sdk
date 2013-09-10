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
		/// An answer for an unknown variable type.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// An answer for a Text variable.
		/// </summary>
		Text,
		/// <summary>
		/// An answer for a Number variable.
		/// </summary>
		Number,
		/// <summary>
		/// An answer for a Date variable.
		/// </summary>
		Date,
		/// <summary>
		/// An answer for a True/False variable or grouped child dialog.
		/// </summary>
		TrueFalse,
		/// <summary>
		/// An answer for a Multiple Choice variable.
		/// </summary>
		MultipleChoice,
		/// <summary>
		/// An answer for an other variable.
		/// </summary>
		Other };

	/// <summary>
	/// An interface for a HotDocs value.
	/// </summary>
	public interface IValue : IConvertible
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
}
