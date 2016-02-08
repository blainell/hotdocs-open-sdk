using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This class is used to return the results from assembling a document.
    /// </summary>
    [DataContract]
    public class AssemblyResult
    {
        /// <summary>
        /// An array of documents created as a result of the assembly. The first file is always the answers.
        /// </summary>
        [DataMember]
        public BinaryObject[] Documents;

        /// <summary>
        /// An array of assemblies that should be completed after this assembly is finished.
        /// </summary>
        [DataMember]
        public PendingAssembly[] PendingAssemblies;

        /// <summary>
        /// A string array of variables that were unanswered when the document was assembled.
        /// </summary>
        [DataMember]
        public string[] UnansweredVariables;
    }
}