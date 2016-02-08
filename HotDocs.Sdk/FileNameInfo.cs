using System.Xml.Serialization;

namespace HotDocs.Sdk
{
    /// <summary>
    /// A helper class to store a file name
    /// </summary>
    public class FileNameInfo
    {
        /// <summary>
        /// Construct an instance.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public FileNameInfo(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public FileNameInfo()
        {
        }

        /// <summary>
        /// Set/get the FileName property.
        /// </summary>
        [XmlAttribute("fileName")]
        public string FileName { get; set; }
    }
}