/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Text;

namespace HotDocs.Sdk
{
	/************************************************************************/
	/*  Utility functions for dealing with repeat indices                   */
	/************************************************************************/
	internal static class RepeatIndices
	{
		public readonly static int[] Empty;

		static RepeatIndices()
		{
			RepeatIndices.Empty = new int[0];
		}

		internal static bool IsNullOrEmpty(int[] indices)
		{
			return indices == null || indices.Length == 0;
		}

		internal static int[] Push(int[] indices)
		{
			return PushIndex(indices, 0);
		}

		internal static int[] PushIndex(int[] indices, int newTopIndex)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");

			int[] result = new int[indices.Length + 1];
			indices.CopyTo(result, 0);
			result[indices.Length] = newTopIndex;
			return result;
		}

		internal static int[] Pop(int[] indices)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new ArgumentException("Cannot pop empty repeat stack", "indices");

			int[] result = new int[indices.Length - 1];
			Array.Copy(indices, result, result.Length);
			return result;
		}

		internal static int GetTopIndex(int[] indices)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new ArgumentException("Cannot peek empty repeat stack", "indices");
			return indices[indices.Length - 1];
		}

		internal static int[] SetTopIndex(int[] indices, int newTopIndex)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new ArgumentException("Cannot set empty repeat stack", "indices");

			int[] result = new int[indices.Length];
			indices.CopyTo(result, 0);
			result[result.Length - 1] = newTopIndex;
			return result;
		}

		internal static int[] SetBaseIndices(int[] indices, int[] newBaseIndices)
		{
			if (newBaseIndices == null || newBaseIndices.Length == 0)
				return indices;

			if (indices == null)
				indices = Empty;

			if (newBaseIndices.Length > indices.Length)
				throw new ArgumentException("Invalid base indices.", "newBaseIndices");

			int[] result = new int[indices.Length];
			for (int i = 0; i < newBaseIndices.Length; i++)
				result[i] = newBaseIndices[i];
			for (int i = newBaseIndices.Length; i < indices.Length; i++)
				result[i] = indices[i];
			return result;
		}

		internal static void Increment(int[] indices)
		{
			Offset(indices, 1);
		}

		internal static void Decrement(int[] indices)
		{
			Offset(indices, -1);
		}

		private static void Offset(int[] indices, int offset)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new ArgumentException("Cannot modify empty repeat stack", "indices");

			indices[indices.Length - 1] += offset;
		}

		internal static bool IsFirst(int[] indices)
		{
			for (int i = 0; i < indices.Length; i++)
				if (indices[i] > 0)
					return false;
			return true;
		}

		/************************************************************************/
		/* Returns true if two sets of indices refer to the same value in       */
		/* a value tree.  Basically compares the array indices while            */
		/* ignoring any trailing zeros or negative numbers, since these do      */
		/* not really affect which value would be retrieved from an answer.     */
		/************************************************************************/
		internal static bool AreEquivalent(int[] indices1, int[] indices2)
		{
			// handle empty indices
			if (indices1 == null) indices1 = Empty;
			if (indices2 == null) indices2 = Empty;
			if (indices1.Length == 0 && indices2.Length == 0)
				return true;

			int[] longer = (indices2.Length > indices1.Length) ? indices2 : indices1;
			int[] shorter = (indices2.Length > indices1.Length) ? indices1 : indices2;
			int i = 0;

			// compare indices shared between the two sets, ignoring the difference between 0 and -1
			for (; i < shorter.Length; i++)
				if (shorter[i] != longer[i] && (shorter[i] > 0 || longer[i] > 0))
					return false;

			// if one set of indices is longer than the other, make sure the remaining indices are <= 0
			for (; i < longer.Length; i++)
				if (longer[i] > 0)
					return false;

			return true;
		}

		/************************************************************************/
		/* Returns true if two sets of indices are either exactly equal, or     */
		/* else if one is an ancestor of the other in the value tree.           */
		/* This is used for testing whether a given repeat index matches (but   */
		/* may be at a lower repeat level than) another repeat index.           */
		/************************************************************************/
		internal static bool AreRelated(int[] indices1, int[] indices2)
		{
			// handle empty indices
			if (indices1 == null) indices1 = Empty;
			if (indices2 == null) indices2 = Empty;
			if (indices1.Length == 0 && indices2.Length == 0)
				return true;

			int sharedLen = Math.Min(indices1.Length, indices2.Length);
			for (int i = 0; i < sharedLen; i++)
				if (indices1[i] != indices2[i])
					return false;
			return true;
		}
	}
}
