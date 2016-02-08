/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

namespace HotDocs.Sdk
{
    /*
	 * This started out as more of a transparent (i.e. public) class cluster,
	 * but I turned TypedAnswer, ValueNode and ValueList into nested classes
	 * as an experiment to see if this design pattern will work as well in C#/.NET
	 * as it works in other, more dynamic languages such as Objective-C.
	 */

    public class Answer
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
    }
}