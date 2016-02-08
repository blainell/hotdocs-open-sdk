using System;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     <c>PdfPermissions</c>
    /// </summary>
    [DataContract, Flags]
    public enum PdfPermissions
    {
        /// <summary>
        ///     No permissions permitted
        /// </summary>
        [EnumMember] None = 0x0,

        /// <summary>
        ///     Pdf Print permission
        /// </summary>
        [EnumMember] Print = 0x4,

        /// <summary>
        ///     Pdf Copy permission
        /// </summary>
        [EnumMember] Copy = 0x10,

        /// <summary>
        ///     Pdf Modify permission
        /// </summary>
        [EnumMember] Modify = 0x28,

        /// <summary>
        ///     All Pdf permissions -- Print, Copy, and Modify
        /// </summary>
        [EnumMember] All = Print | Copy | Modify
    }
}