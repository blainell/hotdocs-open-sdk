using System;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This class represents an assembly that should be run, usually as a result of assembling a related template.
    /// </summary>
    [DataContract]
    [Serializable]
    public class PendingAssembly
    {
        /// <summary>
        /// The name of the template.
        /// </summary>
        [DataMember(IsRequired = true, Order = 1)]
        public string TemplateName;

        /// <summary>
        /// Command line switches that should be used when assembling the template.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Order = 2)]
        public string Switches;
    }
}