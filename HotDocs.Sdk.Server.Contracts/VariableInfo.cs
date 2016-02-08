using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This class provides details about a variable. In the XML, the name of this class is "Var".
    /// </summary>
    [DataContract(Name = "Var")]
    public class VariableInfo
    {
        /// <summary>
        /// This is the name of the variable.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// This is the type of the variable.
        /// </summary>
        [DataMember(Name = "Type")]
        public string Type { get; set; }
    }
}