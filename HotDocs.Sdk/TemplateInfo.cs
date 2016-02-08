using System;
using System.Linq;
using System.Xml.Serialization;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     This class stores information about a template.
    /// </summary>
    [XmlType]
    public class TemplateInfo : IEquatable<TemplateInfo>
    {
        /// <summary>
        ///     Create a new instance.
        /// </summary>
        public TemplateInfo()
        {
            TemplateId = string.Empty;
            FileName = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            EffectiveComponentFile = string.Empty;
        }

        /// <summary>
        ///     The unique ID of the template.
        /// </summary>
        [XmlAttribute("templateId")]
        public string TemplateId { get; set; }

        /// <summary>
        ///     Returns true if TemplateId should be serialized to XML. For internal use only.
        /// </summary>
        /// <returns>true if the value should be serialized.</returns>
        [XmlIgnore]
        public bool TemplateIdSpecified
        {
            get { return !string.IsNullOrEmpty(TemplateId); }
        }

        /// <summary>
        ///     The name identifier for the template.  It must be an internal name for the package. E.g. "template.rtf"
        /// </summary>
        [XmlAttribute("fileName")]
        public string FileName { get; set; }

        /// <summary>
        ///     The title of the template as defined in the component file.
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        ///     The description of the template as defined in the component file.
        /// </summary>
        [XmlElement("description")]
        public string Description { get; set; }

        /// <summary>
        ///     The name of the actual component file. It is the name of the primary component file if it isn't pointed to another
        ///     one. Otherwise it is the pointed to component file name.
        ///     It must be an internal name for the package. E.g. "template.cmp"
        /// </summary>
        [XmlAttribute("effectiveCmpFile")]
        public string EffectiveComponentFile { get; set; }

        /// <summary>
        ///     The list of dependencies of this template.
        /// </summary>
        [XmlArray("dependencies")]
        [XmlArrayItem("dependency")]
        public Dependency[] Dependencies { get; set; }

        /// <summary>
        ///     Js and dll files that were generated for this template.
        /// </summary>
        [XmlArray("generatedFiles")]
        [XmlArrayItem("file")]
        public FileNameInfo[] GeneratedHDFiles { get; set; }

        /// <summary>
        ///     The list of server file names for this template. Each template has a Silverlight assembly DLL. The main template
        ///     and each template referenced in an ASSEMBLY instruction has a JavaScript (.js) and a variable collection file
        ///     (.hvc).
        /// </summary>
        [XmlIgnore]
        public string[] GeneratedFiles
        {
            get
            {
                return GeneratedHDFiles != null
                    ? GeneratedHDFiles.Select(hdf => hdf != null ? hdf.FileName : null).ToArray()
                    : null;
            }

            set { GeneratedHDFiles = value.Select(fn => fn != null ? new FileNameInfo(fn) : null).ToArray(); }
        }

        /// <summary>
        ///     Equals() method for IEquatable interface.
        /// </summary>
        /// <param name="template">The TemplateInfo object to which this one is compared.</param>
        /// <returns>true if they're equal</returns>
        public bool Equals(TemplateInfo template)
        {
            return string.Equals(FileName, template.FileName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Returns true if Title should be serialized to XML. For internal use only.
        /// </summary>
        /// <returns>true if the value should be serialized.</returns>
        public bool ShouldSerializeTitle()
        {
            return !string.IsNullOrEmpty(Title);
        }

        /// <summary>
        ///     Returns true if Description should be serialized to XML. For internal use only.
        /// </summary>
        /// <returns>true if the value should be serialized.</returns>
        public bool ShouldSerializeDescription()
        {
            return !string.IsNullOrEmpty(Description);
        }

        /// <summary>
        ///     Create a deep copy.
        /// </summary>
        /// <returns>Returns a deep copy.</returns>
        public TemplateInfo Copy()
        {
            var ti = new TemplateInfo();
            //
            ti.TemplateId = TemplateId;
            ti.FileName = FileName;
            ti.Title = Title;
            ti.Description = Description;
            ti.EffectiveComponentFile = EffectiveComponentFile;
            if (Dependencies != null)
            {
                ti.Dependencies = new Dependency[Dependencies.Length];
                for (var i = 0; i < Dependencies.Length; i++)
                    ti.Dependencies[i] = Dependencies[i].Copy();
            }
            else
            {
                ti.Dependencies = new Dependency[0];
            }
            if (GeneratedFiles != null)
            {
                ti.GeneratedFiles = new string[GeneratedFiles.Length];
                for (var i = 0; i < GeneratedFiles.Length; i++)
                    ti.GeneratedFiles[i] = GeneratedFiles[i];
            }
            else
            {
                ti.GeneratedFiles = new string[0];
            }
            return ti;
        }
    }
}