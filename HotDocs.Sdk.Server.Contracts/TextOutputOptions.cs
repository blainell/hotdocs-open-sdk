using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     Output options for plain text documents
    /// </summary>
    [DataContract]
    public class TextOutputOptions : OutputOptions
    {
        /// <summary>
        ///     <c>Encoding</c> specifies the encoding (such as UTF8 or UTF16) for the assembled document.
        ///     If left empty the server default will be used.
        /// </summary>
        [DataMember]
        public string Encoding { get; set; } // defaults to null (leave it up to the server default)
    }
}