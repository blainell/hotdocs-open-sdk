/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using HotDocs.Sdk;
using ValueType = HotDocs.Sdk.ValueType;

class TypedAnswer<T> : Answer where T : IValue
{
    private ValueNode<T> _value;

    public TypedAnswer(AnswerCollection set, string name)
        : base(set, name)
    {
        _value = new ValueNode<T>();
    }

    public override IValue GetValue(params int[] rptIdx)
    {
        return GetValue<T>(rptIdx);
    }

    public override ValueType Type
    {
        get
        {
            return _value.Type;
        }
    }

    public override bool IsRepeated
    {
        get
        {
            return (_value.HasChildren || Depth > 0);
        }
    }

    [Flags]
    private enum IndexTrimOptions { Default = 0, TrimTrailingZeros = 1, TruncateLastIndex = 2 }
    // the following are mutually exclusive, therefore this is NOT a [Flags] enumeration:
    private enum GetNodeOptions { Default = 0, CreateNodesAsNecessary = 1, NoBubbleAndFlow = 2 }

    public override bool GetAnswered(params int[] rptIdx)
    {
        // Get the requested node.  If it exists, return its answered status.
        // We trim trailing zeros from the indices because they don't affect the "answeredness" of a specific set of indices
        // (because individual values bubble up and flow down zero indices of value trees)
        int[] indices = TrimIndices(IndexTrimOptions.TrimTrailingZeros, rptIdx);
        ValueNode<T> val = GetNode(GetNodeOptions.Default, indices);
        if (val != null)
            return val.IsAnswered;
        else
            return false;
    }

    public override bool GetUserModifiable(params int[] rptIdx)
    {
        // Get the requested node.  If it exists, return whether it should be user modifiable in the UI.
        int[] indices = TrimIndices(IndexTrimOptions.TrimTrailingZeros, rptIdx);
        ValueNode<T> val = GetNode(GetNodeOptions.Default, indices);
        if (val != null)
            return val.IsUserModifiable;
        else
            return true;
    }

    public override int GetChildCount(params int[] rptIdx)
    {
        // Get the number of answers that are CHILDREN of the requested node.
        // Don't trim any trailing zeros from the indices, because trailing zeros
        // affect the repeat count differently than they do when retrieving individual values.

        // Note that this means you should pass in the repeat index of a (parent) repeat node,
        // not that of an individual iteration, to get the correct count.

        int[] indices = TrimIndices(IndexTrimOptions.Default, rptIdx);
        return GetCount(indices);
    }

    public override int GetSiblingCount(params int[] rptIdx)
    {
        // Get the number of answers that are SIBLINGS of the requested node.
        // Don't trim any trailing zeros from the indices, because trailing zeros
        // affect the repeat count differently than they do when retrieving individual values.

        // This is the same as calling GetChildCount() from the parent index of rptIdx.

        int[] indices = TrimIndices(IndexTrimOptions.TruncateLastIndex, rptIdx);
        return GetCount(indices);
    }

    private int GetCount(int[] indices)
    {
        ValueNode<T> val = GetNode(GetNodeOptions.NoBubbleAndFlow, indices);
        if (val == null) // exact requested node does not exist
        {
            val = GetNode(GetNodeOptions.Default, indices); // request the nearest ancestor
            if (val != null && val.IsAnswered)
            {
                return 1;
            }
            // else
            return 0;
        }
        if (val.HasChildren)
            return val.Children.SetCount;
        else if (val.IsAnswered)
            return 1;
        else
            return 0;
    }

    protected override ValueNode<TExternal> GetValueNode<TExternal>(bool createIfNecessary, params int[] rptIdx)
    {
        if (typeof(TExternal) != typeof(T))
            throw new ArgumentException("Answer/Value type mismatch");

        // If we're looking up a value for "read" purposes, we trim trailing zeros from the indices.
        // But if we're looking up a value for "write" purposes, we leave trailing zeros alone
        // because they influence the formation of the value tree.
        int[] indices = TrimIndices(createIfNecessary ? IndexTrimOptions.Default : IndexTrimOptions.TrimTrailingZeros, rptIdx);
        return GetNode(createIfNecessary ? GetNodeOptions.CreateNodesAsNecessary : GetNodeOptions.Default, indices) as ValueNode<TExternal>;
    }

    private static int[] TrimIndices(IndexTrimOptions options, params int[] rptIdx)
    {
        int trimmedLastIdx;
        return TrimIndices(options, out trimmedLastIdx, rptIdx);
    }

    private static int[] TrimIndices(IndexTrimOptions options, out int trimmedLastIdx, params int[] rptIdx)
    {
        trimmedLastIdx = -1;

        if (rptIdx == null || rptIdx.Length == 0)
            return RepeatIndices.Empty;

        int newSize = rptIdx.Length;
        if ((options & IndexTrimOptions.TrimTrailingZeros) != 0)
        {
            while (newSize > 0 && rptIdx[newSize - 1] <= 0) // trim 0's and -1's from the end
                newSize--;
        }
        else
        {
            while (newSize > 0 && rptIdx[newSize - 1] < 0) // trim -1's from the end, leave zeros
                newSize--;
        }
        if ((options & IndexTrimOptions.TruncateLastIndex) != 0 && newSize > 0)
        {
            trimmedLastIdx = rptIdx[newSize - 1];
            newSize--;
        }

        int[] newIndices = new int[newSize];
        if (newSize > 0)
            Array.Copy(rptIdx, newIndices, newSize);
        return newIndices;
    }

    private ValueNode<T> GetNode(GetNodeOptions options, int[] rptIdx)
    {
        Debug.Assert(rptIdx != null);

        bool createIfNecessary = (options == GetNodeOptions.CreateNodesAsNecessary);
        bool valuesBubbleAndFlow = (options != GetNodeOptions.NoBubbleAndFlow);

        if (createIfNecessary && rptIdx.Length > Depth)
        {
            // make sure our value tree is the appropriate depth
            _value.Expand(rptIdx.Length - Depth, false);
            Depth = rptIdx.Length;
        }

        return GetNodeHelper(_value, 0, createIfNecessary, valuesBubbleAndFlow, rptIdx);
    }

    // private recursive helper function
    private ValueNode<T> GetNodeHelper(ValueNode<T> node, int atDepth, bool createIfNecessary, bool valuesBubbleAndFlow, int[] rptIdx)
    {
        // this can be invoked with the following flag combinations:
        //     createIfNecessary == true   &&   valuesBubbleAndFlow == true   // we will be setting an answer
        //     createIfNecessary == false  &&   valuesBubbleAndFlow == true   // we will be getting an answer
        //     createIfNecessary == false  &&   valuesBubbleAndFlow == false  // we are looking up a node to count or manipulate its children

        // if we're recursing deeper than the _depth of this answer, we should not be creating nodes as we go!
        // (In that case, ValueNode<T>.Expand should have been called first to push values down the tree as necessary.)
        Debug.Assert(atDepth <= Depth || !createIfNecessary);

        if (node == null) throw new ArgumentNullException("node");
        if (rptIdx == null) throw new ArgumentNullException("rptIdx");

        // see if we're done recursing through all the repeat levels
        if (rptIdx.Length == 0 && atDepth == Depth)
        {
            ValueNode<T> result = node;
            if (valuesBubbleAndFlow)
            {
                // if we've found a node but it still has children in the tree,
                // then the value from a 0-indexed descendant node will "bubble up".
                while (result.HasChildren)
                    result = result.Children[0];
            }
            return result;
        }

        int idx;
        int[] newIdx;
        if (rptIdx.Length > 0)
        {
            idx = rptIdx[0];
            // continue recursing down through the repeat levels
            newIdx = new int[rptIdx.Length - 1];
            if (rptIdx.Length > 1)
                Array.Copy(rptIdx, 1, newIdx, 0, newIdx.Length);
        }
        else // rptIdx.Length == 0 ... we've run out of repeat indices
        {
            Debug.Assert(atDepth < Depth);
            if (valuesBubbleAndFlow) // create new, zero indices so we can continue recursion
            {
                idx = 0;
                newIdx = new int[Depth - atDepth - 1]; // make sure value gets set down at the appropriate repeat level
            }
            else
            {
                Debug.Assert(!createIfNecessary); // we don't want to allow setting values at non-leaf nodes of the tree!
                return node;
            }
        }

        // there are still repeat indices through which to recurse...
        if (createIfNecessary) // expand current node as necessary
        {
            if (!node.HasChildren)
                node.Expand(1, true);
            if (idx >= node.Children.SetCount)
                node.Children.PrepareForIndex(idx);
        }
        else if (!node.HasChildren)
        {
            // requested node does not exist -- given indexes exceed depth of repeated values
            return (valuesBubbleAndFlow && idx == 0 && RepeatIndices.IsFirst(newIdx)) ? node : null;
        }
        else if (idx >= node.Children.SetCount)
        {
            // requested node does not exist -- given indexes exceed number of repeated values
            return null;
        }

        return GetNodeHelper(node.Children[idx], ++atDepth, createIfNecessary, valuesBubbleAndFlow, newIdx);
    }

    /// <summary>
    /// ApplyValueMutator summary
    /// </summary>
    /// <typeparam name="TExternal">Type</typeparam>
    /// <param name="mutator">mutator</param>
    public override void ApplyValueMutator<TExternal>(ValueMutator<TExternal> mutator)
    {
        if (typeof(TExternal) != typeof(T))
            throw new ArgumentException("Answer/Value type mismatch");

        TraverseAndApply<TExternal>(mutator, _value);
    }

    // private recursive helper function
    private static void TraverseAndApply<TExternal>(ValueMutator<TExternal> mutator, ValueNode<T> node) where TExternal : IValue
    {
        if (node.HasChildren)
        {
            foreach (ValueNode<T> child in node.Children)
                TraverseAndApply<TExternal>(mutator, child);
        }
        else
        {
            ValueNode<TExternal> nod = node as ValueNode<TExternal>;
            if (nod != null)
                nod.Value = mutator(nod.Value);
        }
    }

    public override void EnumerateValues(object state, ValueEnumerationDelegate callback)
    {
        ValueEnumerationHelper(callback, _value, RepeatIndices.Empty, state);
    }

    private void ValueEnumerationHelper(ValueEnumerationDelegate callback, ValueNode<T> node, int[] indices, object state)
    {
        if (node.HasChildren)
        {
            int[] childIndices;
            for (int i = 0; i < node.Children.Count; i++)
            {
                childIndices = new int[indices.Length + 1];
                Array.Copy(indices, childIndices, indices.Length);
                childIndices[indices.Length] = i;
                ValueEnumerationHelper(callback, node.Children[i], childIndices, state);
            }
        }
        else if (indices.Length == Depth)
        {
            // do callback and fall out of recursion
            callback(state, indices);
        }
    }

    /// <summary>
    /// IndexedValues provides a simple way to enumerate (and potentially modify) the values associated with an answer.
    /// </summary>
    public override IEnumerable<IndexedValue> IndexedValues
    {
        get { return GetIndexedValues(_value, RepeatIndices.Empty); }
    }

    private IEnumerable<IndexedValue> GetIndexedValues(ValueNode<T> node, int[] indices)
    {
        if (node.HasChildren)
        {
            int[] childIndices;
            for (int i = 0; i < node.Children.Count; i++)
            {
                childIndices = new int[indices.Length + 1];
                Array.Copy(indices, childIndices, indices.Length);
                childIndices[indices.Length] = i;
                foreach (var child in GetIndexedValues(node.Children[i], childIndices))
                    yield return child;
            }
        }
        else if (indices.Length == Depth)
        {
            // return an indexed value and fall out of recursion
            yield return new IndexedValue(this, indices);
        }
    }

    protected override void Recalculate(int[] rptIdx)
    {
        // we have un-answered a repeated variable; check to see if any value tree cleanup is needed
        RecalculateHelper(_value, rptIdx);
    }

    private void RecalculateHelper(ValueNode<T> node, int[] rptIdx)
    {
        // recurse down the value tree to the designated index, and then while falling out of the recursion,
        // make sure _maxSet is set properly in each ValueNodeList
        if (rptIdx.Length == 0)
            return;

        // pop the top repeat index off
        int topIndex = rptIdx[0];
        int[] newIdx = new int[rptIdx.Length - 1];
        if (rptIdx.Length > 1)
            Array.Copy(rptIdx, 1, newIdx, 0, newIdx.Length);

        // if node has enough children (which it should), recursively descend into the correct child
        if (node.HasChildren && node.Children.SetCount > topIndex)
            RecalculateHelper(node.Children[topIndex], newIdx);

        // as we fall out of the recursion, if the repeat index at this level is the last iteration
        // (and thus by unanswering something, it could need its repeat count adjusted),
        // call ResetCount to recalculate the SetCount
        if (node.HasChildren && node.Children.SetCount == topIndex + 1)
            node.Children.ResetCount();
    }

    /// <summary>
    /// Completely resets the answer.
    /// </summary>
    public override void Clear()
    {
        base.Clear(); // this recursively clears out all existing values, firing appropriate change notifications along the way

        // now (for good measure) throw away all the previous value structures and reset the answer completely
        _value = new ValueNode<T>();
        Depth = 0;
    }

    /// <summary>
    /// Gets the parent of the requested node,
    /// and inserts an unanswered child iteration at the requested index.
    /// </summary>
    /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']"/>
    public override void InsertIteration(int[] rptIdx)
    {
        // get the parent of the requested node,
        // and insert an unanswered child iteration at the requested index
        int iterationToInsert;
        int[] parIdx = TrimIndices(IndexTrimOptions.TruncateLastIndex, out iterationToInsert, rptIdx);
        if (iterationToInsert < 0) // there is no iteration before which to insert!
            return;

        ValueNode<T> parentNode = GetNode(GetNodeOptions.NoBubbleAndFlow, parIdx);
        if (parentNode == null || parentNode.Children == null || iterationToInsert >= parentNode.Children.SetCount)
            return;

        parentNode.Children.Insert(iterationToInsert, new ValueNode<T>());
        if (AnswerCollection != null)
            AnswerCollection.OnAnswerChanged(this, rptIdx, ValueChangeType.IndexShift);
    }

    /// <summary>
    /// Gets the parent of the requested node,
    /// and deletes the indicated child iteration.
    /// </summary>
    /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']"/>
    public override void DeleteIteration(int[] rptIdx)
    {
        // get the parent of the requested node,
        // and delete the indicated child iteration
        int iterationToDelete;
        int[] parIdx = TrimIndices(IndexTrimOptions.TruncateLastIndex, out iterationToDelete, rptIdx);
        if (iterationToDelete < 0) // there is no iteration to be deleted!
            return;

        ValueNode<T> parentNode = GetNode(GetNodeOptions.NoBubbleAndFlow, parIdx);
        if (parentNode == null || parentNode.Children == null || iterationToDelete >= parentNode.Children.SetCount)
            return;

        parentNode.Children.RemoveAt(iterationToDelete);

        if (AnswerCollection != null)
            AnswerCollection.OnAnswerChanged(this, rptIdx, ValueChangeType.IndexShift);
    }

    /// <summary>
    /// Gets the parent of the requested node, and removes the indicated child iteration from the parent, and re-inserts it at the indicated new position.
    /// </summary>
    /// <include file="../Shared/Help.xml" path="Help/intAry/param[@name='rptIdx']"/>
    /// <param name="newPosition">newPosition</param>
    public override void MoveIteration(int[] rptIdx, int newPosition)
    {
        if (newPosition < 0)
            throw new ArgumentException("Cannot move repeated data before the first repeat iteration.", "newPosition");
        // get the parent of the requested node,
        // removes the indicated child iteration from the parent,
        // and re-inserts it at the indicated new position
        int iterationToMove;
        int[] parIdx = TrimIndices(IndexTrimOptions.TruncateLastIndex, out iterationToMove, rptIdx);
        if (iterationToMove < 0) // there is no iteration to be moved!
            return;

        ValueNode<T> parentNode = GetNode(GetNodeOptions.NoBubbleAndFlow, parIdx);
        if (parentNode == null || parentNode.Children == null
            || (newPosition < iterationToMove && newPosition >= parentNode.Children.SetCount) // move up
            || (newPosition > iterationToMove && iterationToMove >= parentNode.Children.SetCount)) // move down
        {
            // nothing to do
            return;
        }

        int maxIteration = Math.Max(iterationToMove, newPosition);
        if (maxIteration >= parentNode.Children.SetCount)
        {
            //[LRS] Need to be able to do the following! (Because other answers in same iteration may be moving.)
            parentNode.Children.PrepareForIndex(maxIteration);
        }

        ValueNode<T> movedNode = parentNode.Children.RemoveAt(iterationToMove);
        parentNode.Children.Insert(newPosition, movedNode);
        if (AnswerCollection != null)
            AnswerCollection.OnAnswerChanged(this, rptIdx, ValueChangeType.IndexShift);
    }

    /// <summary>
    /// Writes the answer to an XML writer.
    /// </summary>
    /// <param name="writer">XML writer.</param>
    /// <param name="writeDontSave">Indicates if the answer should be written even if it is not supposed to be saved.</param>
    public override void WriteXml(System.Xml.XmlWriter writer, bool writeDontSave)
    {
        if (!writeDontSave && !Save)
            return;
        writer.WriteStartElement("Answer");
        writer.WriteAttributeString("name", TextValue.XMLEscape(Name));
        if (!Save)
            writer.WriteAttributeString("save", System.Xml.XmlConvert.ToString(Save));
        if (!UserExtendible)
            writer.WriteAttributeString("userExtendible", System.Xml.XmlConvert.ToString(UserExtendible));
        _value.WriteXml(writer, Depth);
        writer.WriteEndElement();
    }
}

