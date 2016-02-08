using System;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This enumeration contains options for how the document is assembled.
    /// </summary>
    [DataContract, Flags]
    public enum AssemblyOptions : int
    {
        /// <summary>
        /// Documents are assembled in the default view.
        /// </summary>
        [EnumMember]
        None = 0x0,
        /// <summary>
        /// Documents are assembled in "markup" view where unanswered variable names appear in square brackets.
        /// </summary>
        [EnumMember]
        MarkupView = 0x1
    }
}