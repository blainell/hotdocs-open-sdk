using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     encapsulates all OutputOptions classes that incorporate support for document-level metadata
    /// </summary>
    [KnownType(typeof (PdfOutputOptions))]
    [DataContract]
    public abstract class BasicOutputOptions : OutputOptions
    {
        [DataMember] private readonly Dictionary<string, string> _customValues;

        internal BasicOutputOptions()
        {
            _customValues = new Dictionary<string, string>();
        }

        // if these properties are null, metadata is copied (if possible) from the source.

        /// <summary>
        ///     Author of the assembled document.
        /// </summary>
        [DataMember]
        public string Author { get; set; } // defaults to null

        /// <summary>
        ///     Comments pertaining to the assembled document.
        /// </summary>
        [DataMember]
        public string Comments { get; set; } // defaults to null

        /// <summary>
        ///     Company of the assembled document.
        /// </summary>
        [DataMember]
        public string Company { get; set; } // defaults to null

        /// <summary>
        ///     Keywords of the assembled document.
        /// </summary>
        [DataMember]
        public string Keywords { get; set; } // defaults to null

        /// <summary>
        ///     Subject of the assembled document.
        /// </summary>
        [DataMember]
        public string Subject { get; set; } // defaults to null

        /// <summary>
        ///     Title of the assembled document.
        /// </summary>
        [DataMember]
        public string Title { get; set; } // defaults to null


        internal void SetValue(string name, string value)
        {
            _customValues[name] = value;
        }

        internal string GetValue(string name)
        {
            return _customValues[name];
        }

        internal bool HasValue(string name)
        {
            return _customValues.ContainsKey(name);
        }
    }
}