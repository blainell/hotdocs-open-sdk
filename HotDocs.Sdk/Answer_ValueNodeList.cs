/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HotDocs.Sdk
{
	public abstract partial class Answer
	{
		/// <summary>
		/// ValueNodeList description
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		protected class ValueNodeList<T> : ICollection<ValueNode<T>> where T : IValue
		{
			private List<ValueNode<T>> _list;
			private int _maxSet;

			#region Constructors

			/// <summary>
			/// ValueNodeList constructor
			/// </summary>
			public ValueNodeList()
			{
				_list = new List<ValueNode<T>>();
				_maxSet = -1;
			}

			/// <summary>
			/// ValueNodeList constructor
			/// </summary>
			/// <param name="capacity">capacity</param>
			public ValueNodeList(int capacity)
			{
				_list = new List<ValueNode<T>>(capacity);
				_maxSet = -1;
			}

			#endregion

			/// <summary>
			/// Returns the number of values in the list.
			/// </summary>
			public int SetCount
			{
				get { return _maxSet + 1; }
			}

			/// <summary>
			/// ValueNode summary
			/// </summary>
			/// <param name="index">index</param>
			/// <returns>The specified ValueNode.</returns>
			public ValueNode<T> this[int index]
			{
				get { return _list[index]; }
			}

			/// <summary>
			/// Insert summary
			/// </summary>
			/// <param name="index">index</param>
			/// <param name="item">item</param>
			public void Insert(int index, ValueNode<T> item)
			{
				if (item.IsAnswered || item.HasChildren || index <= _maxSet)
				{
					PrepareForIndex(index - 1);
					_list.Insert(index, item);
					_maxSet++;
				}
			}

			/// <summary>
			/// RemoveAt summary
			/// </summary>
			/// <param name="index">index</param>
			/// <returns>The ValueNode being removed.</returns>
			public ValueNode<T> RemoveAt(int index)
			{
				if (index > _maxSet)
					return new ValueNode<T>();

				ValueNode<T> result = _list[index];
				_list.RemoveAt(index);
				_maxSet--;

				if (index > _maxSet)
					ResetCount();

				return result;
			}

			/// <summary>
			/// PrepareForIndex summary
			/// </summary>
			/// <param name="index">index</param>
			public void PrepareForIndex(int index)
			{
				EnsureCapacity(index + 1);

				if (index > _maxSet)
					_maxSet = index;

				for (int i = _list.Count; i <= index; i++)
					_list.Add(new ValueNode<T>());
			}

			/// <summary>
			/// Resets the count.
			/// </summary>
			public void ResetCount()
			{
				while (_maxSet >= 0 && !_list[_maxSet].IsAnswered && !_list[_maxSet].HasChildren)
					_maxSet--;
			}

			/// <summary>
			/// EnsureCapacity summary.
			/// </summary>
			/// <param name="min">min</param>
			private void EnsureCapacity(int min)
			{
				if (_list.Capacity < min)
				{
					int num = (_list.Count == 0) ? 4 : (_list.Count * 2);
					if (num < min)
						num = min;
					_list.Capacity = num;
				}
			}

			/// <summary>
			/// Writes the XML representation of the answer at the specified depth.
			/// </summary>
			/// <param name="writer">The XmlWriter to which to write the answer value.</param>
			/// <param name="atDepth">The depth of the answer value.</param>
			public void WriteXml(System.Xml.XmlWriter writer, int atDepth)
			{
				writer.WriteStartElement("RptValue");
				for (int i = 0; i < SetCount; i++)
					_list[i].WriteXml(writer, atDepth);

				// Always include one final "unanswered" value at the end of the list.
				ValueNode<T> n = new ValueNode<T>();
				n.WriteXml(writer, atDepth);

				writer.WriteEndElement();
			}

			#region IEnumerable<ValueNode<T>> Members

			/// <summary>
			/// GetEnumerator summary
			/// </summary>
			/// <returns>An enumerator</returns>
			public IEnumerator<ValueNode<T>> GetEnumerator()
			{
				foreach (ValueNode<T> node in _list)
					yield return node;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion

			#region ICollection<ValueNode<T>> Members

			/// <summary>
			/// Adds an item to the collection.
			/// </summary>
			/// <param name="item">The item to add to the collection.</param>
			public void Add(ValueNode<T> item)
			{
				_list.Add(item);
			}

			/// <summary>
			/// Clears the collection.
			/// </summary>
			public void Clear()
			{
				_list.Clear();
			}

			/// <summary>
			/// Determines if the collection contains the specified item.
			/// </summary>
			/// <param name="item">The item to search for in the collection.</param>
			/// <returns>True or False depending on whether or not the item was found in the collection.</returns>
			public bool Contains(ValueNode<T> item)
			{
				return _list.Contains(item);
			}

			/// <summary>
			/// Not implemented.
			/// </summary>
			/// <param name="array">array</param>
			/// <param name="arrayIndex">arrayIndex</param>
			public void CopyTo(ValueNode<T>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			/// <summary>
			/// Returns the number of items in the collection.
			/// </summary>
			public int Count
			{
				get { return _list.Count; }
			}

			/// <summary>
			/// Indicates if the collection is read-only or not.
			/// </summary>
			public bool IsReadOnly
			{
				get { return false; }
			}

			/// <summary>
			/// Removes an item from the collection.
			/// </summary>
			/// <param name="item">The item to remove.</param>
			/// <returns>True or False depending on a successful removal of the item.</returns>
			public bool Remove(ValueNode<T> item)
			{
				return _list.Remove(item);
			}

			#endregion
		}
	}
}
