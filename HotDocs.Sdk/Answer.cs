/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace HotDocs.Sdk
{
    /*
	 * This started out as more of a transparent (i.e. public) class cluster,
	 * but I turned TypedAnswer, ValueNode and ValueList into nested classes
	 * as an experiment to see if this design pattern will work as well in C#/.NET
	 * as it works in other, more dynamic languages such as Objective-C.
	 */

    /// <summary>
    ///     The Answer class is an implementation of the "opaque class cluster" pattern.
    ///     Callers do not create instances of Answer directly, but use AnswerCollection.CreateAnswer().
    ///     The object that is returned is actually an instance of a private subclass of Answer,
    ///     but you only access it via public methods and properties of the Answer class.
    /// </summary>
    public abstract class Answer
    {
        /// <summary>
        ///     The ValueEnumeration delegate.
        /// </summary>
        /// <param name="state">state</param>
        /// <param name="indices">indices</param>
        public delegate void ValueEnumerationDelegate(object state, int[] indices);

        /// <summary>
        ///     The ValueMutator delegate.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="value">value</param>
        /// <returns>Type</returns>
        public delegate T ValueMutator<T>(T value);

        private bool _save; // whether this answer is savable/permanent or temporary

        /// <summary>
        ///     Protected constructor for Answer objects.
        ///     Answer objects need not be constructed directly by callers;
        ///     use AnswerCollection.CreateAnswer() instead.
        /// </summary>
        /// <param name="parent">The AnswerCollection the answer should be a part of.</param>
        /// <param name="name">The answer name.</param>
        protected Answer(AnswerCollection parent, string name)
        {
            AnswerCollection = parent;
            Name = name;
            _save = !string.IsNullOrEmpty(name) && Name[0] != '(';
            UserExtendible = true;
        }

        /// <summary>
        ///     A reference to the AnswerCollection this Answer belongs to.
        /// </summary>
        protected AnswerCollection AnswerCollection { get; }

        /// <summary>
        ///     The current repeat depth of this answer.  Non-repeated answers have a Depth of 0.
        /// </summary>
        protected int Depth { get; set; }

        /// <summary>
        ///     The answer name. Corresponds to a HotDocs variable name.
        /// </summary>
        [DebuggerHidden]
        public string Name { get; }

        /// <summary>
        ///     The type of value that is stored in this answer.
        /// </summary>
        [DebuggerHidden]
        public abstract ValueType Type { get; }

        /// <summary>
        ///     When HotDocs saves a collection of answers as an Answer File, this flag determines whether this specific answer
        ///     will be saved or not. If true (the default), the answer will be saved.  If false, this answer will be
        ///     ignored/dropped and will not be persisted with the rest of the answers in the AnswerCollection.
        /// </summary>
        public bool Save
        {
            get { return _save; }
            set
            {
                if (!_save && value) // LRS: I don't expect this to happen
                    throw new InvalidOperationException("Attempted to designate a non-savable answer as savable.");
                _save = value;
            }
        }

        /// <summary>
        ///     For answers containing repeated values, indicates whether end users should be allowed to add/delete/move
        ///     repeat iterations in the interview UI (default is true).
        /// </summary>
        public bool UserExtendible { get; internal set; }

        /// <summary>
        ///     Indicates whether this answer is repeated or not.
        /// </summary>
        public abstract bool IsRepeated { get; }

        /// <summary>
        ///     IndexedValues provides a simple way to enumerate (and potentially modify) the values associated with an answer.
        /// </summary>
        public abstract IEnumerable<IndexedValue> IndexedValues { get; }

        // Factory method:
        internal static Answer Create<T>(AnswerCollection parent, string name) where T : IValue
        {
            return new TypedAnswer<T>(parent, name);
        }

        /// <summary>
        ///     Indicates whether (or not) a value is available at the indicated repeat indices.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>True or False</returns>
        public abstract bool GetAnswered(params int[] rptIdx);

        /// <summary>
        ///     Indicates whether (or not) a value should be modifiable by the user in the interview UI.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>True or False</returns>
        public abstract bool GetUserModifiable(params int[] rptIdx);

        /// <summary>
        ///     Indicates the number of child values at the specified repeat index.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>The number of children</returns>
        public abstract int GetChildCount(params int[] rptIdx);

        /// <summary>
        ///     Indicates the number of sibling values for the specified repeat index.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>The number of siblings</returns>
        public abstract int GetSiblingCount(params int[] rptIdx);

        /// <summary>
        ///     This method returns a specific value node from an answer.
        /// </summary>
        /// <typeparam name="T">The type of value being requested.</typeparam>
        /// <param name="createIfNecessary">Indicates if the node will be created if necessary.</param>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>A <c>ValueNode</c> for the value found at the specified repeat index.</returns>
        protected abstract ValueNode<T> GetValueNode<T>(bool createIfNecessary, params int[] rptIdx) where T : IValue;

        /// <summary>
        ///     This method returns a specific value from an answer.
        /// </summary>
        /// <typeparam name="T">The type of value being requested.</typeparam>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>The value found at the specified repeat index.</returns>
        public T GetValue<T>(params int[] rptIdx) where T : IValue
        {
            var node = GetValueNode<T>(false, rptIdx);
            if (node != null)
                return node.Value;
            return default(T);
        }

        /// <summary>
        ///     Gets the specified value.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>A value.</returns>
        public abstract IValue GetValue(params int[] rptIdx);

        /// <summary>
        ///     Sets the value for an answer.
        /// </summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="value">The new value for the answer.</param>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        public void SetValue<T>(T value, params int[] rptIdx) where T : IValue
        {
            SetValue(value, false, rptIdx);
        }

        protected void SetValue<T>(T value, bool suppressChangeNotification, params int[] rptIdx) where T : IValue
        {
            // The first parameter to GetValueNode (below) determines whether it will create nodes as necessary
            // to get to the requested index.  We only bother to expand the tree if the value we're setting
            // is answered.
            var nodeCreatedForUnansweredValue = false;
            var node = GetValueNode<T>(value.IsAnswered, rptIdx);
            // if we're setting a value that's answered, GetValueNode should always be returning something!
            // (When we set things to unanswered, the tree won't be expanding to hold the value, so we may get null back.)

            if (node == null)
            {
                // we're setting an unanswered value into the answer collection.
                // In this case we still need to expand the tree to have a place representing that unanswered value.
                node = GetValueNode<T>(true, rptIdx);
                nodeCreatedForUnansweredValue = true;
            }

            Debug.Assert(node != null);
            if (node != null)
            {
                ValueChangeType changed;
                if (value.IsAnswered && !node.Value.IsAnswered)
                    changed = ValueChangeType.BecameAnswered;
                else if (!value.IsAnswered && node.Value.IsAnswered)
                    changed = ValueChangeType.BecameUnanswered;
                else if (value.IsAnswered && node.Value.IsAnswered && !value.Equals(node.Value))
                    changed = ValueChangeType.Changed;
                else
                    changed = ValueChangeType.None;

                node.Value = value;

                if (changed != ValueChangeType.None || nodeCreatedForUnansweredValue)
                {
                    // if we have un-answered a repeated variable; perform any value tree cleanup that is needed
                    if (rptIdx != null && rptIdx.Length > 0 && !value.IsAnswered)
                        Recalculate(rptIdx);

                    if (changed != ValueChangeType.None && AnswerCollection != null && !suppressChangeNotification)
                        AnswerCollection.OnAnswerChanged(this, rptIdx, changed);
                }
            }
        }

        /// <summary>
        ///     Initializes the value for an answer.
        /// </summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="value">The new value for the answer.</param>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        public void InitValue<T>(T value, params int[] rptIdx) where T : IValue
        {
            SetValue(value, true, rptIdx);
        }

        /// <summary>
        ///     Clears the answer.
        /// </summary>
        public virtual void Clear()
        {
            EnumerateValues(this, ClearAnswerCallback);
        }

        protected static void ClearAnswerCallback(object state, int[] indices)
        {
            var answer = (Answer) state;
            answer.ClearValue(indices);
        }

        public void ClearValue(params int[] rptIdx)
        {
            if (GetAnswered(rptIdx))
            {
                switch (Type)
                {
                    case ValueType.Text:
                        SetValue(TextValue.Unanswered, rptIdx);
                        break;
                    case ValueType.Number:
                        SetValue(NumberValue.Unanswered, rptIdx);
                        break;
                    case ValueType.Date:
                        SetValue(DateValue.Unanswered, rptIdx);
                        break;
                    case ValueType.TrueFalse:
                        SetValue(TrueFalseValue.Unanswered, rptIdx);
                        break;
                    case ValueType.MultipleChoice:
                        SetValue(MultipleChoiceValue.Unanswered, rptIdx);
                        break;
                }
            }
        }

        /// <summary>
        ///     Inserts an answer at the specified indexes.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        public abstract void InsertIteration(int[] rptIdx);

        /// <summary>
        ///     Deletes an answer at the specified indexes.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        public abstract void DeleteIteration(int[] rptIdx);

        /// <summary>
        ///     Moves an answer value from one repeat index to another.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <param name="newPosition">newPosition</param>
        public abstract void MoveIteration(int[] rptIdx, int newPosition);

        /// <summary>
        ///     Recalculates an answer.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        protected abstract void Recalculate(int[] rptIdx);

        /// <summary>
        ///     Writes an answer to XML.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="writeDontSave">
        ///     If writeDontSave is non-zero, answers with the Save == false property are saved to the
        ///     writer.
        /// </param>
        public abstract void WriteXml(XmlWriter writer, bool writeDontSave);

        /// <summary>
        ///     ApplyValueMutator uses the Visitor design pattern to modify all the values associated with this answer
        ///     by applying the ValueMutator delegate, in turn, to each value.  This can be useful if you want to
        ///     apply the same modification to all values in an answer, such as marking all of them userModifiable=false.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="mutator">mutator</param>
        public abstract void ApplyValueMutator<T>(ValueMutator<T> mutator) where T : IValue;

        /// <summary>
        ///     EnumerateValues uses the Visitor design pattern to enumerate all the values associated with this answer.
        ///     For each value that is part of the answer, the supplied state object is passed to the supplied callback
        ///     method, along with the repeat indices for that value.  The callback must be a delegate of type
        ///     ValueEnumerationDelegate.
        /// </summary>
        /// <param name="state">
        ///     An object to keep track of whatever state you will need during the enumeration.
        ///     For simple enumerations, passing a reference to the Answer object in question
        ///     (so you can use the repeat indices to look up values) may be adequate.
        /// </param>
        /// <param name="callback">A delegate of type ValueEnumerationDelegate.</param>
        public abstract void EnumerateValues(object state, ValueEnumerationDelegate callback);
    }
}