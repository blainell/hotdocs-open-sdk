using System;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This enumeration lists the options available when requesting an interview. 
    /// </summary>
    [DataContract, Flags]
    public enum InterviewOptions : int
    {
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        None = 0x0,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        OmitImages = 0x1,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        NoSave = 0x4,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        NoPreview = 0x8,
        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ExcludeStateFromOutput = 0x10,
    }
}