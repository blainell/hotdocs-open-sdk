using System;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
    /// <summary>
    /// This enumeration lists the types of files that can be returned by a call to AssembleDocument.
    /// </summary>
    [DataContract, Flags]
    public enum OutputFormat
    {
        /// <summary>
        /// No output
        /// </summary>
        [EnumMember]
        None = 0x0000,
        /// <summary>
        /// <para>XML Answer File</para>
        /// <para>Note: An XML answer file returned from a call to AssembleDocument does not
        /// necessarily contain the exact same set of answers as the combined set of answer
        /// files sent in the request. For example, if during the course of assembling a
        /// document, HotDocs encounters a SET instruction, the answer for that variable may
        /// change in the answer set. Likewise, if a template author designates a variable
        /// as one not to save in the answer file, it is included in the answer set returned
        /// from an interview, but it is then removed when the answer set is saved as XML
        /// prior to being returned from AssembleDocument.</para>
        /// </summary>
        [EnumMember]
        Answers = 0x0001,
        /// <summary>
        /// <para>
        /// The native output document format for the template:
        /// </para>
        /// <list type="bullet">
        /// <item>RTF (Word) templates produce RTF documents.</item>
        /// <item>WPT (WordPerfect) templates produce WPD documents.</item>
        /// <item>HPT (PDF-based) form templates produce PDF documents.</item>
        /// <item>HFT (Envoy-based) form templates produce HFD documents.</item>
        /// </list>
        /// </summary>
        [EnumMember]
        Native = 0x0002,
        /// <summary>
        /// A PDF document. (Only supported with RTF/DOCX and HPT templates.)
        /// </summary>
        [EnumMember]
        PDF = 0x0004,
        /// <summary>
        /// An HTML version of the assembled document. (Only supported with RTF/DOCX templates.)
        /// </summary>
        [EnumMember]
        HTML = 0x0008,
        /// <summary>
        /// A plain text version of the assembled document. (Only supported with RTF/DOCX templates.)
        /// </summary>
        [EnumMember]
        PlainText = 0x0010,
        /// <summary>
        /// An HTML version of the assembled document where all images are encoded and included in-line in Data URIs. (Only supported with RTF/DOCX templates.)
        /// </summary>
        [EnumMember]
        HTMLwDataURIs = 0x0020, // see http://en.wikipedia.org/wiki/Data_URI_scheme
        /// <summary>
        /// An MHTML version of the assembled document where all images are packaged along with the HTML. (Only supported with RTF/DOCX templates.)
        /// </summary>
        [EnumMember]
        MHTML = 0x0040, // see http://en.wikipedia.org/wiki/Mhtml
        /// <summary>
        /// A Microsoft Word RTF document.
        /// </summary>
        [EnumMember]
        RTF = 0x0080,
        /// <summary>
        /// A Microsoft Word Open XML document.
        /// </summary>
        [EnumMember]
        DOCX = 0x0100,
        /// <summary>
        /// A WordPerfect document.  (Not supported in HotDocs Core Services.)
        /// </summary>
        [EnumMember]
        WPD = 0x0200,
        /// <summary>
        /// A PDF-based HotDocs form document. (Requires HotDocs Filler to open.)
        /// </summary>
        [EnumMember]
        HPD = 0x0400,
        /// <summary>
        /// An Envoy-based HotDocs form document. (Requires HotDocs Filler to open.)
        /// </summary>
        [EnumMember]
        HFD = 0x0800,
        /// <summary>
        /// A .JPG image file.  (Placeholder for future implementation.)
        /// </summary>
        [EnumMember]
        JPEG = 0x1000,
        /// <summary>
        /// A Portable Network Graphics (.PNG) image file.  (Placeholder for future implementation.)
        /// </summary>
        [EnumMember]
        PNG = 0x2000
    }
}