using System;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This enumeration contains options for what server files are built.
    /// </summary>
    [DataContract, Flags]
    public enum HDSupportFilesBuildFlags
    {
        /// <summary>
        /// Build javascript files.
        /// </summary>
        [EnumMember]
        BuildJavaScriptFiles = 0x01,
        /// <summary>
        /// Build Silverlight files.
        /// </summary>
        [EnumMember]
        BuildSilverlightFiles = 0x02,
        /// <summary>
        /// Force the rebuilding of all files.
        /// </summary>
        [EnumMember]
        ForceRebuildAll = 0x04,
        /// <summary>
        /// Build files for templates included with an assemble instruction.
        /// </summary>
        [EnumMember]
        IncludeAssembleTemplates = 0x08,
    }
}