/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace HotDocs.Sdk
{
	// The whole purpose of this class is to allow us to serialize to an XML string
	// but still have it say UTF-8 at the top.
	internal sealed class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding { get { return Encoding.UTF8; } }
	}

	/*
	/// <summary>
	/// The type of dependency.
	/// NOTE: It needs to be kept in sync with the HotDocs desktop and server IDL files!
	/// </summary>
	public enum HDDependencyType
	{
		[XmlEnum("noDependency")]
		NoDependency = 0,
		[XmlEnum("baseCmpFile")]
		BaseCmpFileDependency = 1,
		[XmlEnum("pointedToCmpFile")]
		PointedToCmpFileDependency = 2,
		[XmlEnum("templateInsert")]
		TemplateInsertDependency = 3,
		[XmlEnum("clauseInsert")]
		ClauseInsertDependency = 4,
		[XmlEnum("clauseLibraryInsert")]
		ClauseLibraryInsertDependency = 5,
		[XmlEnum("imageInsert")]
		ImageInsertDependency = 6,
		[XmlEnum("interviewImage")]
		InterviewImageDependency = 7,
		[XmlEnum("variableTemplateInsert")]
		VariableTemplateInsertDependency = 8,
		[XmlEnum("variableImageInsert")]
		VariableImageInsertDependency = 9,
		[XmlEnum("missingVariable")]
		MissingVariableDependency = 10,
		[XmlEnum("missingFile")]
		MissingFileDependency = 11,
		[XmlEnum("assemble")]
		AssembleDependency = 12,
		[XmlEnum("publisherMapFile")]
		PublisherMapFileDependency = 13,
		[XmlEnum("userMapFile")]
		UserMapFileDependency = 14,
		[XmlEnum("additionalTemplate")]
		AdditionalTemplateDependency = 15
	}
	*/

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

	/// <summary>
	/// A helper class to store a set of file names
	/// </summary>
	internal class FileNameSet
	{
		private Dictionary<string, string> dict = new Dictionary<string, string>();

		/// <summary>
		/// Include a filename to the set
		/// </summary>
		/// <param name="fileName">the file name to include</param>
		public void Include(string fileName)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				string key = fileName.ToLower();
				if (!dict.ContainsKey(key))
				{
					dict[key] = fileName;
				}
			}
		}

		/// <summary>
		/// Get an array of all file names in the set
		/// </summary>
		/// <returns>an array of all file names in the set</returns>
		public string[] ToArray()
		{
			//return Enumerable.ToArray(dict.Values);
			return dict.Values.ToArray<string>();
		}
	}

	/*
	/// <summary>
	/// This class stores a dependency of a template with another file
	/// </summary>
	public class HDDependency : IEquatable<HDDependency>
	{
		/// <summary>
		/// The target name. It must be an internal name for the package. E.g. "insert.rtf"
		/// </summary>
		[XmlAttribute("fileName")]
		public string FileName { get; set; }
		/// <summary>
		/// The type of dependency.
		/// </summary>
		[XmlAttribute("type")]
		public DependencyType DependencyType { get; set; }

		/// <summary>
		/// Create a new instance with an empty target and no dependency.
		/// </summary>
		public HDDependency()
		{
			FileName = string.Empty;
			DependencyType = DependencyType.NoDependency;
		}

		/// <summary>
		/// Create a new instance.
		/// </summary>
		/// <param name="fileName">The file name</param>
		/// <param name="depType">The dependency type</param>
		public HDDependency(string fileName, DependencyType depType)
		{
			FileName = fileName;
			DependencyType = depType;
		}

		/// <summary>
		/// Make a copy.
		/// </summary>
		/// <returns></returns>
		public HDDependency Copy()
		{
			return new HDDependency(FileName, DependencyType);
		}

		/// <summary>
		/// Check if a dependency refers to a template
		/// </summary>
		/// <param name="dt">The type of dependency.</param>
		/// <returns>true if the dependency refers to a template.</returns>
		public static bool IsTemplateDependency(DependencyType dt)
		{
			return dt == DependencyType.TemplateInsert || dt == DependencyType.Assemble;
		}

		/// <summary>
		/// Equals() method for IEquatable interface.
		/// </summary>
		/// <param name="template">The HDDependency object to which this one is compared.</param>
		/// <returns>true if they're equal</returns>
		public bool Equals(HDDependency template)
		{
			return string.Equals(this.FileName, template.FileName, StringComparison.OrdinalIgnoreCase);
		}
	}
	*/


	/// <summary>
	/// The manifest information of a package. All this information will be serialized as XML and stored in the package as another file with the name as defined at TemplatePackage.ManifestName
	/// Given a variable 'package' of type TemplatePackage, the main template name is  package.Manifest.MainTemplate.FileName
	/// </summary>
	[XmlRoot("packageManifest")]
	public class TemplatePackageManifest
	{
		private static readonly string PackageNamespace = "http://www.hotdocs.com/schemas/package_manifest/2012";
		private static readonly string PackageNamespace10 = "http://www.hotdocs.com/schemas/package_manifest/2011";

		static XmlSerializer _xmlSerializer;
		static XmlSerializer _xmlSerializer10;
		//static XmlSerializerNamespaces _xmlSerializerNamespaces;

		static TemplatePackageManifest()
		{
			var v10Overrides = new XmlAttributeOverrides();

			var v10DependencyEnumMap = new Dictionary<string,string>()
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
				v10Overrides.Add(typeof(DependencyType), kv.Value, new XmlAttributes() { XmlEnum = new XmlEnumAttribute(kv.Key) });
			}

			v10Overrides.Add(typeof(Dependency), "FileName", new XmlAttributes() { XmlAttribute = new XmlAttributeAttribute("target") });
			v10Overrides.Add(typeof(TemplateInfo), "FileName", new XmlAttributes() { XmlAttribute = new XmlAttributeAttribute("name") });
			var attributes = new XmlAttributes() { XmlArray = new XmlArrayAttribute("serverFiles") };
			attributes.XmlArrayItems.Add(new XmlArrayItemAttribute("file"));
			v10Overrides.Add(typeof(TemplateInfo), "GeneratedHDFiles", attributes);

			_xmlSerializer = new XmlSerializer(typeof(TemplatePackageManifest), PackageNamespace);
			_xmlSerializer10 = new XmlSerializer(typeof(TemplatePackageManifest), v10Overrides, null, null, PackageNamespace10);

			//_xmlSerializerNamespaces = new XmlSerializerNamespaces();
			//_xmlSerializerNamespaces.Add(string.Empty, string.Empty);

			/*
			TemplatePackageManifest manifest = new TemplatePackageManifest();
			var main = manifest.MainTemplate = new TemplateInfo();
			main.FileName = "myfilename.rtf";
			main.EffectiveComponentFile = "mycomponentfile.cmp";
			string xml;
			using (var sw = new StringWriter())
			{
				_xmlSerializer10.Serialize(sw, manifest);
				xml = sw.ToString();
			}
			*/
		}

		private static void IncludeTemplateFiles(FileNameSet fs, TemplateInfo templateInfo, bool includeGeneratedFiles = true)
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
			if (includeGeneratedFiles && templateInfo.GeneratedHDFiles != null && templateInfo.GeneratedHDFiles.Length > 0)
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
			FileNameSet fs = new FileNameSet();
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
		/// Return all of the files referenced in the manifest, including the main template file and all
		/// its dependencies. This returns all except the manifest file itself.
		/// </summary>
		public string[] AllFiles
		{
			get
			{
				return GetFiles(true);
			}
		}

		/// <summary>
		/// Return all of the files referenced in the manifest, including the main template file and all
		/// its dependencies, except all generated files (.js, .dll, .hvc) and the manifest file itself.
		/// </summary>
		public string[] AllFilesExceptGenerated
		{
			get
			{
				return GetFiles(false);
			}
		}

		/// <summary>
		/// The SHA-1 hash of the files in the package.
		/// </summary>
		[XmlAttribute("signature")]
		public string Signature { get; set; }

		/// <summary>
		/// The version of HotDocs that created this package
		/// </summary>
		[XmlAttribute("hotdocsVersion")]
		public string HotDocsVersion { get; set; }

		/// <summary>
		/// The date and time the package was published in XSD UTC format.
		/// </summary>
		[XmlAttribute("publishDateTime")]
		public string PublishDateTime { get; set; }

		/// <summary>
		/// The date the package expires.
		/// </summary>
		[XmlAttribute("expirationDate")]
		public string ExpirationDate { get; set; }

		/// <summary>
		/// Returns true if there is an expiration date specified.
		/// </summary>
		[XmlIgnore]
		public bool ExpirationDateSpecified
		{
			get { return !string.IsNullOrEmpty(ExpirationDate); }
		}

		/// <summary>
		/// The number of warning days preceding the expiration date.
		/// </summary>
		[XmlAttribute("warningDays")]
		public int WarningDays { get; set; }

		/// <summary>
		/// Returns true if there is an warning date specified.
		/// </summary>
		[XmlIgnore]
		public bool WarningDaysSpecified
		{
			get { return WarningDays > 0; }
		}

		/// <summary>
		/// The number of warning days preceding the expiration date.
		/// </summary>
		[XmlAttribute("extensionDays")]
		public int ExtensionDays { get; set; }

		/// <summary>
		/// Returns true if there are extension days specified.
		/// </summary>
		[XmlIgnore]
		public bool ExtensionDaysSpecified
		{
			get { return ExtensionDays > 0; }
		}

		/// <summary>
		/// The information of the main template.
		/// </summary>
		[XmlElement("mainTemplate")]
		public TemplateInfo MainTemplate { get; set; }

		private TemplateInfo[] _otherTemplates;
		/// <summary>
		/// The information of other templates used by the main template using INSERT and ASSEMBLE instructions.
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
			set
			{
				_otherTemplates = value;
			}
		}
		/// <summary>
		/// Returns true if OtherTemplates should be serialized to XML. For internal use only. 
		/// </summary>
		/// <returns>true if the value should be serialized.</returns>
		public bool ShouldSerializeOtherTemplates()
		{
			return _otherTemplates != null && _otherTemplates.Length > 0;
		}

		private FileNameInfo[] _additionalHDFiles;

		/// <summary>
		/// A list of additional files needed by the template.
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
			set
			{
				_additionalHDFiles = value;
			}
		}
		/// <summary>
		/// Returns true if AdditionalHDFiles should be serialized to XML. For internal use only. 
		/// </summary>
		/// <returns>true if the value should be serialized.</returns>
		public bool ShouldSerializeAdditionalHDFiles()
		{
			return _additionalHDFiles != null && _additionalHDFiles.Length > 0;
		}

		/// <summary>
		/// A list of other files needed to use the main template. They are usually graphics or help files.
		/// Each name must be an internal name for the package. E.g. "icon.jpg"
		/// </summary>
		[XmlIgnore]
		public string[] AdditionalFiles
		{
			get
			{
				return AdditionalHDFiles.Select(hdf => hdf != null ? hdf.FileName : null).ToArray();
			}
			set
			{
				AdditionalHDFiles = value.Select(fn => new FileNameInfo(fn)).ToArray();
			}
		}

		/// <summary>
		/// Return the HotDocs version number that created this package.
		/// </summary>
		[XmlIgnore]
		public int Version
		{
			get
			{
				try
				{
					string[] segments = HotDocsVersion.Split('.');
					return int.Parse(segments[0]);
				}
				catch (Exception ex)
				{
					throw new Exception("Error reading the HotDocs version in the manifest.", ex);
				}
			}
		}

		/// <summary>
		/// Create a new instance.
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
		/// Create a deep copy.
		/// </summary>
		/// <returns>Returns a deep copy.</returns>
		public TemplatePackageManifest Copy()
		{
			TemplatePackageManifest m = new TemplatePackageManifest();
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
		/// Serialize this manifest to an XML string
		/// </summary>
		/// <returns>An XML representation of the manifest.</returns>
		public string ToXml()
		{
			var stringWriter = new Utf8StringWriter();
			_xmlSerializer.Serialize(stringWriter, this);
			return stringWriter.ToString();
		}

		/// <summary>
		/// Serialize this manifest to an XML string and write it to a stream using UTF-8 encoding (with a byte-order-mark).
		/// </summary>
		/// <param name="st">The stream to write to.</param>
		public void ToStream(Stream st)
		{
			StreamWriter sw = new StreamWriter(st, Encoding.UTF8);
			sw.Write(ToXml());
			sw.Flush();
		}

		/*
		private static string GetCurrentAssemblyName()
		{
			string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
			int i = assemblyName.IndexOf(',');
			//
			return i > 0 ? assemblyName.Substring(0, i) : assemblyName;
		}

		private static string ReplaceAssemblyName(string manifestXml, string newName)
		{
			const string Pat = "xmlns=\"clr-namespace:HotDocs.Packaging;assembly=";
			int i = manifestXml.IndexOf(Pat), j;
			//
			if (i>0)
			{
				i += Pat.Length;
				j = manifestXml.IndexOf('"', i);
				if (i<j)
				{
					manifestXml = manifestXml.Substring(0, i) + newName + manifestXml.Substring(j);
				}
			}
			return manifestXml;
		}
		*/

		/// <summary>
		/// Create a new manifest instance and initialize it by deserializing an XML string.
		/// </summary>
		/// <param name="manifestXml">An XML representation of a manifest.</param>
		/// <returns>A manifest object initialized by deserializing the XML representation.</returns>
		public static TemplatePackageManifest FromXml(string manifestXml)
		{
			// We need to get the version *before* we deserialize it, so we'll cheat and use regex
			Match match = Regex.Match(manifestXml, @"hotdocsVersion\s*=\s*""(\d\d)");
			if (match.Groups.Count < 2)
			{
				return null; // TODO: Throw an exception
			}

			StringReader sr = new StringReader(manifestXml);

			string majorVersion = match.Groups[1].Value;
			if (majorVersion == "10")
			{
				return (TemplatePackageManifest)_xmlSerializer10.Deserialize(sr);
			}

			return (TemplatePackageManifest)_xmlSerializer.Deserialize(sr);
		}

		/// <summary>
		/// Create a new manifest instance and initialize it by deserializing from a stream.
		/// </summary>
		/// <param name="st">The stream to read from.</param>
		/// <returns>A manifest object initialized by deserializing the XML representation stored in the stream.</returns>
		public static TemplatePackageManifest FromStream(Stream st)
		{
			StreamReader sr = new StreamReader(st, true);
			string manifestXml = sr.ReadToEnd();
			//
			return FromXml(manifestXml);
		}

	}

	/// <summary>
	/// This class stores information about a template.
	/// </summary>
	[XmlType]
	public class TemplateInfo : IEquatable<TemplateInfo>
	{
		/// <summary>
		/// The unique ID of the template.
		/// </summary>
		[XmlAttribute("templateId")]
		public string TemplateId { get; set; }
		/// <summary>
		/// Returns true if TemplateId should be serialized to XML. For internal use only. 
		/// </summary>
		/// <returns>true if the value should be serialized.</returns>
		[XmlIgnore]
		public bool TemplateIdSpecified
		{
			get { return !string.IsNullOrEmpty(TemplateId); }
		}

		/// <summary>
		/// The name identifier for the template.  It must be an internal name for the package. E.g. "template.rtf"
		/// </summary>
		[XmlAttribute("fileName")]
		public string FileName { get; set; }

		/// <summary>
		/// The title of the template as defined in the component file.
		/// </summary>
		[XmlElement("title")]
		public string Title { get; set; }
		/// <summary>
		/// Returns true if Title should be serialized to XML. For internal use only. 
		/// </summary>
		/// <returns>true if the value should be serialized.</returns>
		public bool ShouldSerializeTitle()
		{
			return !string.IsNullOrEmpty(Title);
		}

		/// <summary>
		/// The description of the template as defined in the component file.
		/// </summary>
		[XmlElement("description")]
		public string Description { get; set; }
		/// <summary>
		/// Returns true if Description should be serialized to XML. For internal use only. 
		/// </summary>
		/// <returns>true if the value should be serialized.</returns>
		public bool ShouldSerializeDescription()
		{
			return !string.IsNullOrEmpty(Description);
		}

		/// <summary>
		/// The name of the actual component file. It is the name of the primary component file if it isn't pointed to another one. Otherwise it is the pointed to component file name.
		/// It must be an internal name for the package. E.g. "template.cmp"
		/// </summary>
		[XmlAttribute("effectiveCmpFile")]
		public string EffectiveComponentFile { get; set; }

		/// <summary>
		/// The list of dependencies of this template.
		/// </summary>
		[XmlArray("dependencies")]
		[XmlArrayItem("dependency")]
		public Dependency[] Dependencies { get; set; }

		/// <summary>
		///  Js and dll files that were generated for this template.
		/// </summary>
		[XmlArray("generatedFiles")]
		[XmlArrayItem("file")]
		public FileNameInfo[] GeneratedHDFiles { get; set; }

		/// <summary>
		/// The list of server file names for this template. Each template has a Silverlight assembly DLL. The main template and each template referenced in an ASSEMBLY instruction has a JavaScript (.js) and a variable collection file (.hvc).
		/// </summary>
		[XmlIgnore]
		public string[] GeneratedFiles
		{
			get
			{
				return GeneratedHDFiles != null ? GeneratedHDFiles.Select(hdf => hdf != null ? hdf.FileName : null).ToArray() : null;
			}

			set
			{
				GeneratedHDFiles = value.Select(fn => fn != null ? new FileNameInfo(fn) : null).ToArray();
			}
		}

		/// <summary>
		/// Create a new instance.
		/// </summary>
		public TemplateInfo()
		{
			TemplateId = String.Empty;
			FileName = string.Empty;
			Title = string.Empty;
			Description = string.Empty;
			EffectiveComponentFile = string.Empty;
		}

		/// <summary>
		/// Create a deep copy.
		/// </summary>
		/// <returns>Returns a deep copy.</returns>
		public TemplateInfo Copy()
		{
			TemplateInfo ti = new TemplateInfo();
			//
			ti.TemplateId = TemplateId;
			ti.FileName = FileName;
			ti.Title = Title;
			ti.Description = Description;
			ti.EffectiveComponentFile = EffectiveComponentFile;
			if (Dependencies != null)
			{
				ti.Dependencies = new Dependency[Dependencies.Length];
				for (int i = 0; i < Dependencies.Length; i++)
					ti.Dependencies[i] = Dependencies[i].Copy();
			}
			else
			{
				ti.Dependencies = new Dependency[0];
			}
			if (GeneratedFiles != null)
			{
				ti.GeneratedFiles = new string[GeneratedFiles.Length];
				for (int i = 0; i < GeneratedFiles.Length; i++)
					ti.GeneratedFiles[i] = GeneratedFiles[i];
			}
			else
			{
				ti.GeneratedFiles = new string[0];
			}
			return ti;
		}

		/// <summary>
		/// Equals() method for IEquatable interface.
		/// </summary>
		/// <param name="template">The TemplateInfo object to which this one is compared.</param>
		/// <returns>true if they're equal</returns>
		public bool Equals(TemplateInfo template)
		{
			return string.Equals(this.FileName, template.FileName, StringComparison.OrdinalIgnoreCase);
		}
	}

}
