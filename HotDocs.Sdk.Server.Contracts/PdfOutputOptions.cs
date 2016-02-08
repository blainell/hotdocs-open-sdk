using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    ///     Output options for PDF documents.
    /// </summary>
    [DataContract]
    public class PdfOutputOptions : BasicOutputOptions
    {
        /// <summary>
        ///     <c>EmbedFonts</c> flag for the the assembled form.
        /// </summary>
        [DataMember]
        public bool EmbedFonts { get; set; } // defauls to Default

        /// <summary>
        ///     <c>PdfA</c> boolean flag for the the assembled form.
        /// </summary>
        [DataMember]
        public bool PdfA { get; set; } // defaults to false

        /// <summary>
        ///     <c>TaggedPdf</c> flag for the the assembled form.
        /// </summary>
        [DataMember]
        public bool TaggedPdf { get; set; } // defaults to false

        /// <summary>
        ///     KeepFillablePdf flag for the the assembled form.
        /// </summary>
        [DataMember]
        public bool KeepFillablePdf { get; set; } // defaults to false

        /// <summary>
        ///     <c>TruncateFields</c> flag for the the assembled form.
        /// </summary>
        [DataMember]
        public bool TruncateFields { get; set; } // defaults to ?

        /// <summary>
        ///     <c>PdfPermissions</c> enumeration (print, modify, copy) for the the assembled form.
        /// </summary>
        [DataMember]
        public PdfPermissions Permissions { get; set; } // defaults to ?

        /// <summary>
        ///     <c>OwnerPassword</c> is an owner password for the the assembled form.
        /// </summary>
        [DataMember]
        public string OwnerPassword { get; set; } // defaults to null

        /// <summary>
        ///     <c>UserPassword</c> is a user password for the the assembled form.
        /// </summary>
        [DataMember]
        public string UserPassword { get; set; } // defaults to null
    }
}