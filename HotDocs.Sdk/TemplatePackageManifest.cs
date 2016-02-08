/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace HotDocs.Sdk
{
    // The whole purpose of this class is to allow us to serialize to an XML string
    // but still have it say UTF-8 at the top.

    /// <summary>
    ///     The manifest information of a package. All this information will be serialized as XML and stored in the package as
    ///     another file with the name as defined at TemplatePackage.ManifestName
    ///     Given a variable 'package' of type TemplatePackage, the main template name is
    ///     package.Manifest.MainTemplate.FileName
    /// </summary>
    [XmlRoot("packageManifest")]
    public class TemplatePackageManifest
    {
        private static readonly string PackageNamespace = "http://www.hotdocs.com/schemas/package_manifest/2012";
        private static readonly string PackageNamespace10 = "http://www.hotdocs.com/schemas/package_manifest/2011";

        private static XmlSerializer _xmlSerializer;
        private static XmlSerializer _xmlSerializer10;

        private FileNameInfo[] _additionalHDFiles;

        private TemplateInfo[] _otherTemplates;

        static TemplatePackageManifest()
        {
            var v10Overrides = new XmlAttributeOverrides();

            var v10DependencyEnumMap = new Dictionary<string, string>
            {
                {"NoDependency", "NoDependency"},
                {"PrimaryCmpFile", "BaseCmpFile"},
                {"SharedCmpFile", "PointedToCmpFile"},
                {"InsertedTemplate", "TemplateInsert"},
                {"InsertedClause", "ClauseInsert"},
                {"InsertedClauseLibrary", "ClauseLibraryInsert"},
                {"InsertedImage", "ImageInsert"},
                {"InterviewImage", "InterviewImage"},
                {"VariableInsertedTemplate", "VariableTemplateInsert"},
                {"VariableInsertedImage", "VariableImageInsert"},
                {"MissingVariable", "MissingVariable"},
                {"MissingFile", "MissingFile"},
                {"AssembledTemplate", "Assemble"},
                {"PublisherMapFile", "PublisherMapFile"},
                {"UserMapFile", "UserMapFile"}
            };

            foreach (var kv in v10DependencyEnumMap)
            {
                v10Overrides.Add(typeof (DependencyType), kv.Value,
                    new XmlAttributes {XmlEnum = new XmlEnumAttribute(kv.Key)});
            }

            v10Overrides.Add(typeof (Dependency), "FileName",
                new XmlAttributes {XmlAttribute = new XmlAttributeAttribute("target")});
            v10Overrides.Add(typeof (TemplateInfo), "FileName",
                new XmlAttributes {XmlAttribute = new XmlAttributeAttribute("name")});
            var attributes = new XmlAttributes {XmlArray = new XmlArrayAttribute("serverFiles")};
            attributes.XmlArrayItems.Add(new XmlArrayItemAttribute("file"));
            v10Overrides.Add(typeof (TemplateInfo), "GeneratedHDFiles", attributes);

            _xmlSerializer = new XmlSerializer(typeof (TemplatePackageManifest), PackageNamespace);
            _xmlSerializer10 = new XmlSerializer(typeof (TemplatePackageManifest), v10Overrides, null, null,
                PackageNamespace10);
        }

        /// <summary>
        ///     Create a new instance.
        /// </summary>
        public TemplatePackageManifest()
        {
            Signature = string.Empty;
            HotDocsVersion = string.Empty;
            PublishDateTime = string.Empty;
            ExpirationDate = string.Empty;
            WarningDays = 0;
            ExtensionDays = 0;
            MainTemplate = new TemplateInfo();
        }

        /// <summary>
        ///     Return all of the files referenced in the manifest, including the main template file and all
        ///     its dependencies. This returns all except the manifest file itself.
        /// </summary>
        public string[] AllFiles
        {
            get { return GetFiles(true); }
        }

        /// <summary>
        ///     Return all of the files referenced in the manifest, including the main template file and all
        ///     its dependencies, except all generated files (.js, .dll, .hvc) and the manifest file itself.
        /// </summary>
        public string[] AllFilesExceptGenerated
        {
            get { return GetFiles(false); }
        }

        /// <summary>
        ///     The SHA-1 hash of the files in the package.
        /// </summary>
        [XmlAttribute("signature")]
        public string Signature { get; set; }

        /// <summary>
        ///     The version of HotDocs that created this package
        /// </summary>
        [XmlAttribute("hotdocsVersion")]
        public string HotDocsVersion { get; set; }

        /// <summary>
        ///     The date and time the package was published in XSD UTC format.
        /// </summary>
        [XmlAttribute("publishDateTime")]
        public string PublishDateTime { get; set; }

        /// <summary>
        ///     The date the package expires.
        /// </summary>
        [XmlAttribute("expirationDate")]
        public string ExpirationDate { get; set; }

        /// <summary>
        ///     Returns true if there is an expiration date specified.
        /// </summary>
        [XmlIgnore]
        public bool ExpirationDateSpecified
        {
            get { return !string.IsNullOrEmpty(ExpirationDate); }
        }

        /// <summary>
        ///     The number of warning days preceding the expiration date.
        /// </summary>
        [XmlAttribute("warningDays")]
        public int WarningDays { get; set; }

        /// <summary>
        ///     Returns true if there is an warning date specified.
        /// </summary>
        [XmlIgnore]
        public bool WarningDaysSpecified
        {
            get { return WarningDays > 0; }
        }

        /// <summary>
        ///     The number of warning days preceding the expiration date.
        /// </summary>
        [XmlAttribute("extensionDays")]
        public int ExtensionDays { get; set; }

        /// <summary>
        ///     Returns true if there are extension days specified.
        /// </summary>
        [XmlIgnore]
        public bool ExtensionDaysSpecified
        {
            get { return ExtensionDays > 0; }
        }

        /// <summary>
        ///     The information of the main template.
        /// </summary>
        [XmlElement("mainTemplate")]
        public TemplateInfo MainTemplate { get; set; }

        /// <summary>
        ///     The information of other templates used by the main template using INSERT and ASSEMBLE instructions.
        /// </summary>
        [XmlArray("otherTemplates")]
        [XmlArrayItem("template")]
        public TemplateInfo[] OtherTemplates
        {
            get
            {
                if (_otherTemplates == null)
                {
                    _otherTemplates = new TemplateInfo[0];
                }
                return _otherTemplates;
            }
            set { _otherTemplates = value; }
        }

        /// <summary>
        ///     A list of additional files needed by the template.
        /// </summary>
        [XmlArray("additionalFiles")]
        [XmlArrayItem("file")]
        public FileNameInfo[] AdditionalHDFiles
        {
            get
            {
                if (_additionalHDFiles == null)
                {
                    _additionalHDFiles = new FileNameInfo[0];
                }
                return _additionalHDFiles;
            }
            set { _additionalHDFiles = value; }
        }

        /// <summary>
        ///     A list of other files needed to use the main template. They are usually graphics or help files.
        ///     Each name must be an internal name for the package. E.g. "icon.jpg"
        /// </summary>
        [XmlIgnore]
        public string[] AdditionalFiles
        {
            get { return AdditionalHDFiles.Select(hdf => hdf != null ? hdf.FileName : null).ToArray(); }
            set { AdditionalHDFiles = value.Select(fn => new FileNameInfo(fn)).ToArray(); }
        }

        /// <summary>
        ///     Return the HotDocs version number that created this package.
        /// </summary>
        [XmlIgnore]
        public int Version
        {
            get
            {
                try
                {
                    var segments = HotDocsVersion.Split('.');
                    return int.Parse(segments[0]);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error reading the HotDocs version in the manifest.", ex);
                }
            }
        }

        private static void IncludeTemplateFiles(FileNameSet fs, TemplateInfo templateInfo,
            bool includeGeneratedFiles = true)
        {
            fs.Include(templateInfo.FileName);
            if (templateInfo.Dependencies != null && templateInfo.Dependencies.Length > 0)
            {
                foreach (var dep in templateInfo.Dependencies)
                {
                    if (dep != null)
                    {
                        fs.Include(dep.FileName);
                    }
                }
            }
            fs.Include(templateInfo.EffectiveComponentFile);
            if (includeGeneratedFiles && templateInfo.GeneratedHDFiles != null &&
                templateInfo.GeneratedHDFiles.Length > 0)
            {
                foreach (var f in templateInfo.GeneratedHDFiles)
                {
                    if (f != null)
                    {
                        fs.Include(f.FileName);
                    }
                }
            }
        }

        private string[] GetFiles(bool includeGeneratedFiles = true)
        {
            var fs = new FileNameSet();
            IncludeTemplateFiles(fs, MainTemplate, includeGeneratedFiles);
            if (_additionalHDFiles != null && _additionalHDFiles.Length > 0)
            {
                foreach (var f in _additionalHDFiles)
                {
                    if (f != null)
                    {
                        fs.Include(f.FileName);
                    }
                }
            }
            if (_otherTemplates != null && _otherTemplates.Length > 0)
            {
                foreach (var ti in _otherTemplates)
                {
                    if (ti != null)
                    {
                        IncludeTemplateFiles(fs, ti, includeGeneratedFiles);
                    }
                }
            }
            return fs.ToArray();
        }

        /// <summary>
        ///     Returns true if OtherTemplates should be serialized to XML. For internal use only.
        /// </summary>
        /// <returns>true if the value should be serialized.</returns>
        public bool ShouldSerializeOtherTemplates()
        {
            return _otherTemplates != null && _otherTemplates.Length > 0;
        }

        /// <summary>
        ///     Returns true if AdditionalHDFiles should be serialized to XML. For internal use only.
        /// </summary>
        /// <returns>true if the value should be serialized.</returns>
        public bool ShouldSerializeAdditionalHDFiles()
        {
            return _additionalHDFiles != null && _additionalHDFiles.Length > 0;
        }

        /// <summary>
        ///     Create a deep copy.
        /// </summary>
        /// <returns>Returns a deep copy.</returns>
        public TemplatePackageManifest Copy()
        {
            var m = new TemplatePackageManifest();
            int i;
            //
            m.Signature = Signature;
            m.HotDocsVersion = HotDocsVersion;
            m.PublishDateTime = PublishDateTime;
            m.ExpirationDate = ExpirationDate;
            m.WarningDays = WarningDays;
            m.ExtensionDays = ExtensionDays;
            m.MainTemplate = MainTemplate != null ? MainTemplate.Copy() : new TemplateInfo();
            if (OtherTemplates != null)
            {
                m.OtherTemplates = new TemplateInfo[OtherTemplates.Length];
                for (i = 0; i < OtherTemplates.Length; i++)
                    m.OtherTemplates[i] = OtherTemplates[i].Copy();
            }
            else
            {
                m.OtherTemplates = new TemplateInfo[0];
            }
            if (AdditionalFiles != null)
            {
                m.AdditionalFiles = new string[AdditionalFiles.Length];
                for (i = 0; i < AdditionalFiles.Length; i++)
                    m.AdditionalFiles[i] = AdditionalFiles[i];
            }
            else
            {
                m.AdditionalFiles = new string[0];
            }
            return m;
        }

        /// <summary>
        ///     Serialize this manifest to an XML string
        /// </summary>
        /// <returns>An XML representation of the manifest.</returns>
        public string ToXml()
        {
            var stringWriter = new Utf8StringWriter();
            _xmlSerializer.Serialize(stringWriter, this);
            return stringWriter.ToString();
        }

        /// <summary>
        ///     Serialize this manifest to an XML string and write it to a stream using UTF-8 encoding (with a byte-order-mark).
        /// </summary>
        /// <param name="st">The stream to write to.</param>
        public void ToStream(Stream st)
        {
            var sw = new StreamWriter(st, Encoding.UTF8);
            sw.Write(ToXml());
            sw.Flush();
        }

        /// <summary>
        ///     Create a new manifest instance and initialize it by deserializing an XML string.
        /// </summary>
        /// <param name="manifestXml">An XML representation of a manifest.</param>
        /// <returns>A manifest object initialized by deserializing the XML representation.</returns>
        public static TemplatePackageManifest FromXml(string manifestXml)
        {
            // We need to get the version *before* we deserialize it, so we'll cheat and use regex
            var match = Regex.Match(manifestXml, @"hotdocsVersion\s*=\s*""(\d\d)");
            if (match.Groups.Count < 2)
            {
                throw new ArgumentException("TemplatePackageManifest.FromXml: invalid manifestXml parameter");
            }

            var sr = new StringReader(manifestXml);

            var majorVersion = match.Groups[1].Value;
            if (majorVersion == "10")
            {
                return (TemplatePackageManifest) _xmlSerializer10.Deserialize(sr);
            }

            return (TemplatePackageManifest) _xmlSerializer.Deserialize(sr);
        }

        /// <summary>
        ///     Create a new manifest instance and initialize it by deserializing from a stream.
        /// </summary>
        /// <param name="st">The stream to read from.</param>
        /// <returns>A manifest object initialized by deserializing the XML representation stored in the stream.</returns>
        public static TemplatePackageManifest FromStream(Stream st)
        {
            var sr = new StreamReader(st, true);
            var manifestXml = sr.ReadToEnd();
            //
            return FromXml(manifestXml);
        }
    }
}