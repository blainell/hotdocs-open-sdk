using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     The <c>InterviewFormat</c> enumeration is used when referring to the types of browser-based interviews supported by
    ///     HotDocs Server.
    /// </summary>
    [DataContract]
    public enum InterviewFormat
    {
        /// <summary>
        /// </summary>
        [EnumMember] JavaScript,

        /// <summary>
        /// </summary>
        [EnumMember] Silverlight,

        /// <summary>
        /// </summary>
        [EnumMember] Unspecified
    }
}