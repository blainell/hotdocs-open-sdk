using System.Collections.Generic;
using System.Xml;

namespace HotDocs.Sdk
{
    public interface IAnswer
    {
        /// <summary>
        ///     The answer name. Corresponds to a HotDocs variable name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The type of value that is stored in this answer.
        /// </summary>
        ValueType Type { get; }

        /// <summary>
        ///     When HotDocs saves a collection of answers as an Answer File, this flag determines whether this specific answer
        ///     will be saved or not. If true (the default), the answer will be saved.  If false, this answer will be
        ///     ignored/dropped and will not be persisted with the rest of the answers in the AnswerCollection.
        /// </summary>
        bool Save { get; set; }

        /// <summary>
        ///     For answers containing repeated values, indicates whether end users should be allowed to add/delete/move
        ///     repeat iterations in the interview UI (default is true).
        /// </summary>
        bool UserExtendible { get; set; }

        /// <summary>
        ///     Indicates whether this answer is repeated or not.
        /// </summary>
        bool IsRepeated { get; }

        /// <summary>
        ///     IndexedValues provides a simple way to enumerate (and potentially modify) the values associated with an answer.
        /// </summary>
        IEnumerable<IndexedValue> IndexedValues { get; }

        /// <summary>
        ///     Indicates whether (or not) a value is available at the indicated repeat indices.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>True or False</returns>
        bool GetAnswered(params int[] rptIdx);

        /// <summary>
        ///     Indicates whether (or not) a value should be modifiable by the user in the interview UI.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>True or False</returns>
        bool GetUserModifiable(params int[] rptIdx);

        /// <summary>
        ///     Indicates the number of child values at the specified repeat index.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>The number of children</returns>
        int GetChildCount(params int[] rptIdx);

        /// <summary>
        ///     Indicates the number of sibling values for the specified repeat index.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>The number of siblings</returns>
        int GetSiblingCount(params int[] rptIdx);

        /// <summary>
        ///     This method returns a specific value from an answer.
        /// </summary>
        /// <typeparam name="T">The type of value being requested.</typeparam>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>The value found at the specified repeat index.</returns>
        T GetValue<T>(params int[] rptIdx) where T : IValue;

        /// <summary>
        ///     Gets the specified value.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <returns>A value.</returns>
        IValue GetValue(params int[] rptIdx);

        /// <summary>
        ///     Sets the value for an answer.
        /// </summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="value">The new value for the answer.</param>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        void SetValue<T>(T value, params int[] rptIdx) where T : IValue;

        /// <summary>
        ///     Initializes the value for an answer.
        /// </summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="value">The new value for the answer.</param>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        void InitValue<T>(T value, params int[] rptIdx) where T : IValue;

        /// <summary>
        ///     Clears the answer.
        /// </summary>
        void Clear();

        void ClearValue(params int[] rptIdx);

        /// <summary>
        ///     Inserts an answer at the specified indexes.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        void InsertIteration(int[] rptIdx);

        /// <summary>
        ///     Deletes an answer at the specified indexes.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        void DeleteIteration(int[] rptIdx);

        /// <summary>
        ///     Moves an answer value from one repeat index to another.
        /// </summary>
        /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']" />
        /// <param name="newPosition">newPosition</param>
        void MoveIteration(int[] rptIdx, int newPosition);

        /// <summary>
        ///     Writes an answer to XML.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        /// <param name="writeDontSave">
        ///     If writeDontSave is non-zero, answers with the Save == false property are saved to the
        ///     writer.
        /// </param>
        void WriteXml(XmlWriter writer, bool writeDontSave);

        /// <summary>
        ///     ApplyValueMutator uses the Visitor design pattern to modify all the values associated with this answer
        ///     by applying the ValueMutator delegate, in turn, to each value.  This can be useful if you want to
        ///     apply the same modification to all values in an answer, such as marking all of them userModifiable=false.
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="mutator">mutator</param>
        void ApplyValueMutator<T>(Answer.ValueMutator<T> mutator) where T : IValue;

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
        void EnumerateValues(object state, Answer.ValueEnumerationDelegate callback);
    }
}