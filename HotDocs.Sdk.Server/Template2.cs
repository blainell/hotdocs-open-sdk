/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HotDocs.Sdk.Server
{
#if false
	[Serializable]
	public class Template
	{
		//Constructors
		public Template(string fileName, TemplateLocation location, string switches="")
		{
			//TODO: Check parameters here. Make sure location is not null.

			FileName = fileName;
			Location = location;
			Switches = switches;
		}

		public Template(PackageTemplateLocation location, string switches="")
		{
			HotDocs.Sdk.TemplateInfo ti = location.GetPackageManifest().MainTemplate;
			FileName = ti.FileName;
			Location = location;
			Switches = switches;
		}

		public string CreateLocator()
		{
			string locator = FileName + "|" + Switches + "|" + Location.CreateLocator();
			return Util.EncryptString(locator);
		}

		public static Template Locate(string locator)
		{
			string[] tokens = locator.Split('|');
			if (tokens.Length != 3)
				throw new Exception("Invalid template locator.");

			return new Template(tokens[0], TemplateLocation.Locate(tokens[2]), tokens[1]);
		}

		//Public methods.
		/// <summary>
		/// Returns the template title as defined in the template's manifest.
		/// </summary>
		/// <returns></returns>
		public string GetTitle()
		{
			if (_title == null)
			{
				try
				{
					TemplateManifest manifest = GetManifest(ManifestParseFlags.ParseTemplateInfo);
					_title = manifest.Title;
				}
				catch (Exception)
				{
					_title = "";
				}
			}
			return _title;
		}
		/// <summary>
		/// Get the template manifest for this template. Can optionally parse an entire template manifest spanning tree.
		/// </summary>
		/// <param name="parseFlags"></param>
		/// <returns></returns>
		public TemplateManifest GetManifest(ManifestParseFlags parseFlags)
		{
			return TemplateManifest.ParseManifest(GetFullPath(), parseFlags);
		}

		//Public properties.
		/// <summary>
		/// The file name (including extension) of the template. No path information is included.
		/// </summary>
		public string FileName { get; private set; }
		/// <summary>
		/// Defines the location of the template.
		/// </summary>
		public TemplateLocation Location { get; private set; }
		/// <summary>
		/// Command line switches that may be applicable when assembling the template, as provided to the ASSEMBLE instruction.
		/// </summary>
		public string Switches { get; set; }
		/// <summary>
		/// A key identifying the template. When using a template management scheme where the template file itself is temporary
		/// (such as with a DMS) set this key to help HotDocs Server to keep track of which server files are for which template.
		/// </summary>
		public string Key { get; set; }
		/// <summary>
		/// If the host app wants to know, this property does what's necessary to
		/// tell you the type of template you're dealing with.
		/// </summary>
		public TemplateType TemplateType
		{
			get
			{
				switch (Path.GetExtension(FileName).ToLowerInvariant())
				{
					case ".cmp": return TemplateType.InterviewOnly;
					case ".docx": return TemplateType.WordDOCX;
					case ".rtf": return TemplateType.WordRTF;
					case ".hpt": return TemplateType.HotDocsPDF;
					case ".hft": return TemplateType.HotDocsHFT;
					case ".wpt": return TemplateType.WordPerfect;
					case ".ttx": return TemplateType.PlainText;
					default: return TemplateType.Unknown;
				}
			}
		}
		/// <summary>
		/// Parses command-line switches to inform the host app whether or not
		/// an interview should be displayed for this template.
		/// </summary>
		public bool HasInterview
		{
			get
			{
				string switches = String.IsNullOrEmpty(Switches) ? String.Empty : Switches.ToLower();
				return !switches.Contains("/nw") && !switches.Contains("/naw") && !switches.Contains("/ni");
			}
		}
		/// <summary>
		/// Based on TemplateType, tells the host app whether this type of template
		/// generates a document or not (although regardless, ALL template types
		/// need to be assembled in order to participate in assembly queues)
		/// </summary>
		public bool GeneratesDocument
		{
			get
			{
				TemplateType type = TemplateType;
				return
					type == TemplateType.WordDOCX ||
					type == TemplateType.WordRTF ||
					type == TemplateType.WordPerfect ||
					type == TemplateType.HotDocsHFT ||
					type == TemplateType.HotDocsPDF ||
					type == TemplateType.PlainText;
			}
		}
		/// <summary>
		/// Based on the template file extension, get the document type native to the template type.
		/// </summary>
		public DocumentType NativeDocumentType
		{
			get
			{
				string ext = Path.GetExtension(FileName);
				if (string.Compare(ext, ".docx", true) == 0)
					return DocumentType.WordDOCX;
				if (string.Compare(ext, ".rtf", true) == 0)
					return DocumentType.WordRTF;
				if (string.Compare(ext, ".hpt", true) == 0)
					return DocumentType.PDF;
				if (string.Compare(ext, ".hft", true) == 0)
					return DocumentType.HFD;
				if (string.Compare(ext, ".wpt", true) == 0)
					return DocumentType.WordPerfect;
				if (string.Compare(ext, ".ttx", true) == 0)
					return DocumentType.PlainText;
				return DocumentType.Unknown;
			}
		}

		/// <summary>
		/// Get a file associated with this template.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public Stream GetFile(string fileName)
		{
			if (Location == null)
				throw new Exception("No location has been specified.");
			string filePath = Path.Combine(Location.GetTemplateDirectory(), fileName);
			return new FileStream(filePath, FileMode.Open);
		}

		#region Private implementation

		//Private methods
		public string GetFullPath()
		{
			if (Location == null)
				throw new Exception("No location has been specified.");
			return Path.Combine(Location.GetTemplateDirectory(), FileName);
		}

		//Private fields
		private string _title = null;//A cached title when non-null.

		#endregion
	}
#endif
}
