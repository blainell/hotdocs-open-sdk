/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

[assembly: ContractNamespace("http://hotdocs.com/contracts/", ClrNamespace = "HotDocs.Sdk.Server.Contracts")]

namespace HotDocs.Sdk.Server.Contracts
{
	/// <summary>
	/// The <c>InterviewFormat</c> enumeration is used when referring to the types of browser-based interviews supported by HotDocs Server.
	/// </summary>
	[DataContract]
	public enum InterviewFormat
	{
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		JavaScript,
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		Silverlight,
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		Unspecified
	}

	/// <summary>
	/// This enumeration lists the options available when requesting an interview. 
	/// </summary>
	[DataContract, Flags]
	public enum InterviewOptions : int
	{
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		None = 0x0,
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		OmitImages = 0x1,
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		NoSave = 0x4,
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		NoPreview = 0x8,
		/// <summary>
		/// 
		/// </summary>
		[EnumMember]
		ExcludeStateFromOutput = 0x10,
	}

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

	/// <summary>
	/// This enumeration contains options for how the document is assembled.
	/// </summary>
	[DataContract, Flags]
	public enum AssemblyOptions : int
	{
		/// <summary>
		/// Documents are assembled in the default view.
		/// </summary>
		[EnumMember]
		None = 0x0,
		/// <summary>
		/// Documents are assembled in "markup" view where unanswered variable names appear in square brackets.
		/// </summary>
		[EnumMember]
		MarkupView = 0x1
	}

	/// <summary>
	/// This enumeration contains options for what server files are built.
	/// </summary>
	[DataContract, Flags]
	public enum HDSupportFilesBuildFlags
	{
		/// <summary>
		/// Build javascript files.
		/// </summary>
		[EnumMember]
		BuildJavaScriptFiles = 0x01,
		/// <summary>
		/// Build Silverlight files.
		/// </summary>
		[EnumMember]
		BuildSilverlightFiles = 0x02,
		/// <summary>
		/// Force the rebuilding of all files.
		/// </summary>
		[EnumMember]
		ForceRebuildAll = 0x04,
		/// <summary>
		/// Build files for templates included with an assemble instruction.
		/// </summary>
		[EnumMember]
		IncludeAssembleTemplates = 0x08,
	}

	/// <summary>
	/// This class is used for sending and receiving files, such as answers, templates, and documents.
	/// </summary>
	[DataContract]
	public class BinaryObject
	{
		/// <summary>
		/// The name of the file.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string FileName;
		/// <summary>
		/// The encoding used for the data.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string DataEncoding;
		/// <summary>
		/// The format of the file.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public OutputFormat Format;
		/// <summary>
		/// The bytes that make up the file.
		/// </summary>
		[DataMember(IsRequired = true)]
		public byte[] Data;
	}

	/// <summary>
	/// This class represents an assembly that should be run, usually as a result of assembling a related template.
	/// </summary>
	[DataContract]
	[Serializable]
	public class PendingAssembly
	{
		/// <summary>
		/// The name of the template.
		/// </summary>
		[DataMember(IsRequired = true, Order = 1)]
		public string TemplateName;

		/// <summary>
		/// Command line switches that should be used when assembling the template.
		/// </summary>
		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Switches;
	}

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

	/// <summary>
	/// This class provides details about a template.
	/// </summary>
	[DataContract]
	public class TemplateInfo
	{
		/// <summary>
		/// The ID of the template.
		/// </summary>
		[DataMember(Order = 1)]
		public string ID;

		/// <summary>
		/// The title of the template.
		/// </summary>
		[DataMember(Order = 2)]
		public string Title;

		/// <summary>
		/// The description of the template.
		/// </summary>
		[DataMember(Order = 3, EmitDefaultValue = false)]
		public string Description;

		//[DataMember(Order = 4, EmitDefaultValue = false)]
		//public TemplateInfo[] Dependencies;
	}

	/// <summary>
	/// This class provides details about a variable. In the XML, the name of this class is "Var".
	/// </summary>
	[DataContract(Name = "Var")]
	public class VariableInfo
	{
		/// <summary>
		/// This is the name of the variable.
		/// </summary>
		[DataMember(Name = "Name")]
		public string Name { get; set; }

		/// <summary>
		/// This is the type of the variable.
		/// </summary>
		[DataMember(Name = "Type")]
		public string Type { get; set; }
	}

	/// <summary>
	/// This class provides details about a dialog. In the XML, the name of this class is "Dlg".
	/// </summary>
	[DataContract(Name = "Dlg")]
	public class DialogInfo
	{
		/// <summary>
		/// The name of the dialog. 
		/// </summary>
		/// <value>The name of this property in the XML is "Name".</value>
		[DataMember(Name = "Name", Order = 1)]
		public string Name { get; set; }

		/// <summary>
		/// Indicates whether or not the dialog is repeated.
		/// </summary>
		/// <value> The name of this property in the XML is "Rpt".</value>
		[DataMember(Name = "Rpt", Order = 2, EmitDefaultValue = false)]
		public Boolean Repeat { get; set; }

		/// <summary>
		/// The name of the answer source associated with the dialog.
		/// </summary>
		/// <value>The name of this property in the XML is "Src".</value>
		[DataMember(Name = "Src", Order = 3, EmitDefaultValue = false)]
		public String AnswerSource { get; set; }

		/// <summary>
		/// A list of items contained in the dialog.
		/// </summary>
		[DataMember(Order = 4)]
		public List<DialogItemInfo> Items = new List<DialogItemInfo>();
	}

	/// <summary>
	/// This class provides information about items in a dialog.
	/// </summary>
	[DataContract(Name = "Item")]
	public class DialogItemInfo
	{
		/// <summary>
		/// The name of the dialog item.
		/// </summary>
		[DataMember(Name = "Name", Order = 1)]
		public String Name { get; set; }
		/// <summary>
		/// The name of the external data to which the dialog item is mapped. For example, if the dialog is mapped to a database, this would be the name of the column in the database associated with this variable.
		/// </summary>
		[DataMember(Name = "Map", Order = 2, EmitDefaultValue = false)]
		public String Mapping { get; set; }
	}

	/// <summary>
	/// This class provides information about components in a template's interview.
	/// </summary>
	[DataContract]
	public class ComponentInfo
	{
		/// <summary>
		/// The list of variables contained in the template's interview.
		/// </summary>
		[DataMember(Order = 1)]
		public List<VariableInfo> Variables = new List<VariableInfo>();

		/// <summary>
		/// The list of dialogs contained in the template's interview.
		/// </summary>
		[DataMember(Order = 2, EmitDefaultValue = false)]
		public List<DialogInfo> Dialogs = null;

		private Dictionary<string, bool> _varIndex = new Dictionary<string, bool>();

		/// <summary>
		/// This method indicates whether or not the variable is defined.
		/// </summary>
		/// <param name="variableName">The name of the variable to check.</param>
		/// <returns>True if the variable is defined, or false otherwise.</returns>
		public bool IsDefinedVariable(string variableName)
		{
			return _varIndex.ContainsKey(variableName);
		}

		/// <summary>
		/// This method adds a variable to the list of variables for the component file.
		/// </summary>
		/// <param name="item">The variable to add to the list.</param>
		public void AddVariable(VariableInfo item)
		{
			Variables.Add(item);
			_varIndex[item.Name] = true;
		}

		/// <summary>
		/// This method adds a dialog to the list of dialogs for the component file.
		/// </summary>
		/// <param name="item">The dialog to add to the list.</param>
		public void AddDialog(DialogInfo item)
		{
			if (Dialogs == null)
				Dialogs = new List<DialogInfo>(16); // default capacity
			Dialogs.Add(item);
		}
	}
}
