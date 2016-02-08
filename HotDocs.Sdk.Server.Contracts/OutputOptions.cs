/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     Abstract base class of all OutputOptions.
    ///     Provides facilities for serializing options in a very compact string representation for optimal transmission.
    /// </summary>
    [KnownType(typeof (BasicOutputOptions))]
    [KnownType(typeof (PdfOutputOptions))]
    [DataContract]
    public abstract class OutputOptions
    {
        // TODO: Provide facilities for serializing options in a very compact string representation for optimal transmission.
    }
}