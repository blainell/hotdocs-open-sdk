/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HotDocs.Sdk
{

		/// <summary>
		/// ValueNode summary
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		public class ValueNode<T> : IEquatable<ValueNode<T>> where T : IValue
		{
		    /// <summary>
			/// ValueNode constructor
			/// </summary>
			public ValueNode()
			{
				Value = default(T);
				Children = null;
			}

			/// <summary>
			/// ValueNode constructor
			/// </summary>
			/// <param name="value">value</param>
			public ValueNode(T value)
			{
				Value = value;
				Children = null;
			}

			/// <summary>
			/// Value
			/// </summary>
			[DebuggerHidden]
			public T Value { get; set; }

		    /// <summary>
			/// Indicates the value type.
			/// </summary>
			public ValueType Type
			{
				get { return Value.Type; }
			}

			/// <summary>
			/// Indicates if the value is answered.
			/// </summary>
			public bool IsAnswered
			{
				get { return Value.IsAnswered; }
			}

			/// <summary>
			/// Indicates whether the users should be allowed to modify this value in the interview UI.
			/// </summary>
			public bool IsUserModifiable
			{
				get { return Value.UserModifiable; }
			}

			/// <summary>
			/// Indicates if the value has any children.
			/// </summary>
			public bool HasChildren
			{
				get { return (Children != null && Children.SetCount > 0); }
			}

			/// <summary>
			/// Children of the value.
			/// </summary>
			[DebuggerHidden]
			public ValueNodeList<T> Children { get; set; }

		    /// <summary>
			/// Expand description
			/// </summary>
			/// <param name="levels">levels</param>
			/// <param name="expandUnanswered">expandUnanswered</param>
			public void Expand(int levels, bool expandUnanswered)
			{
				Debug.Assert(levels > 0);
				if (levels <= 0) return; // nothing to do -- unexpected

				if (!expandUnanswered && !IsAnswered && !HasChildren)
					return; // don't waste memory by pushing down unanswered values to the next level unnecessarily

				if (Children == null)
				{
					// we may be expanding an Answered node or an Unanswered node here
					if (IsAnswered)
					{
						Children = new ValueNodeList<T>(1);
						// push current value down to first child
						Children.PrepareForIndex(0);
						(Children[0] as ValueNode<T>).Value = Value;
						Value = default(T); // this node becomes unanswered
					}
					else // not answered
					{
						Debug.Assert(expandUnanswered);
						Children = new ValueNodeList<T>();
					}
					--levels;
				}
				else
				{
					// children already exist
					Debug.Assert(!IsAnswered);
				}

				if (levels == 0) // we're done
					return; // drop out of recursion

				// recurse into child nodes
				foreach (ValueNode<T> child in Children)
					child.Expand(levels, expandUnanswered);
			}

			/// <summary>
			/// Converts a value to a string.
			/// </summary>
			/// <returns>A string representation of the value.</returns>
			public override string ToString()
			{
				return Value.ToString();
			}

			/// <summary>
			/// Equals description
			/// </summary>
			/// <param name="other">other</param>
			/// <returns>True or False</returns>
			public bool Equals(ValueNode<T> other)
			{
				return Value.Equals(other.Value);
			}

			/// <summary>
			/// Writes the XML representation of the answer at the specified depth.
			/// </summary>
			/// <param name="writer">The XmlWriter to which to write the answer value.</param>
			/// <param name="atDepth">The depth of the answer value.</param>
			public void WriteXml(System.Xml.XmlWriter writer, int atDepth)
			{
				if (atDepth == 0)
					Value.WriteXml(writer);
				else if (Children != null)
					Children.WriteXml(writer, --atDepth);
				else // no children but not yet at answer's full repeat depth, so
					writer.WriteElementString("RptValue", null); // write a empty repeat value node (placeholder)
			}
		}
}
