/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{

	/// <summary>
	/// <c>PdfPermissions</c> 
	/// </summary>
	[DataContract, Flags]
	public enum PdfPermissions
	{
		/// <summary>
		/// No permissions permitted
		/// </summary>
		[EnumMember]
		None = 0x0,

		/// <summary>
		/// Pdf Print permission
		/// </summary>
		[EnumMember]
		Print = 0x4,

		/// <summary>
		/// Pdf Copy permission
		/// </summary>
		[EnumMember]
		Copy = 0x10,

		/// <summary>
		/// Pdf Modify permission
		/// </summary>
		[EnumMember]
		Modify = 0x28,

		/// <summary>
		/// All Pdf permissions -- Print, Copy, and Modify
		/// </summary>
		[EnumMember]
		All = Print | Copy | Modify
	}

	/// <summary>
	/// Abstract base class of all OutputOptions.
	/// Provides facilities for serializing options in a very compact string representation for optimal transmission.
	/// </summary>
	[KnownType(typeof(BasicOutputOptions))]
	[KnownType(typeof(PdfOutputOptions))]
	[DataContract]
	public abstract class OutputOptions
	{
		// TODO: Provide facilities for serializing options in a very compact string representation for optimal transmission.
	}

	/// <summary>
	/// encapsulates all OutputOptions classes that incorporate support for document-level metadata
	/// </summary>
	[KnownType(typeof(PdfOutputOptions))]
	[DataContract]
	public abstract class BasicOutputOptions : OutputOptions
	{
		internal BasicOutputOptions()
		{
			_customValues = new Dictionary<string, string>();
		}

		// if these properties are null, metadata is copied (if possible) from the source.

		/// <summary>
		/// Author of the assembled document.
		/// </summary>
		[DataMember]
		public string Author { get; set; } // defaults to null

		/// <summary>
		/// Comments pertaining to the assembled document.
		/// </summary>
		[DataMember]
		public string Comments { get; set; } // defaults to null

		/// <summary>
		/// Company of the assembled document.
		/// </summary>
		[DataMember]
		public string Company { get; set; } // defaults to null

		/// <summary>
		/// Keywords of the assembled document.
		/// </summary>
		[DataMember]
		public string Keywords { get; set; } // defaults to null

		/// <summary>
		/// Subject of the assembled document.
		/// </summary>
		[DataMember]
		public string Subject { get; set; } // defaults to null

		/// <summary>
		/// Title of the assembled document.
		/// </summary>
		[DataMember]
		public string Title { get; set; } // defaults to null

		[DataMember]
		private readonly Dictionary<string, string> _customValues;


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

	/// <summary>
	/// Output options for PDF documents.
	/// </summary>
	[DataContract]
	public class PdfOutputOptions : BasicOutputOptions
	{
		/// <summary>
		/// <c>EmbedFonts</c> flag for the the assembled form.
		/// </summary>
		[DataMember]
		public bool EmbedFonts { get; set; } // defauls to Default

		/// <summary>
		/// <c>PdfA</c> boolean flag for the the assembled form.
		/// </summary>
		[DataMember]
		public bool PdfA { get; set; } // defaults to false

		/// <summary>
		/// <c>TaggedPdf</c> flag for the the assembled form.
		/// </summary>
		[DataMember]
		public bool TaggedPdf { get; set; } // defaults to false

		/// <summary>
		/// KeepFillablePdf flag for the the assembled form.
		/// </summary>
		[DataMember]
		public bool KeepFillablePdf { get; set; } // defaults to false

		/// <summary>
		/// <c>TruncateFields</c> flag for the the assembled form.
		/// </summary>
		[DataMember]
		public bool TruncateFields { get; set; } // defaults to ?

		/// <summary>
		/// <c>PdfPermissions</c> enumeration (print, modify, copy) for the the assembled form.
		/// </summary>
		[DataMember]
		public PdfPermissions Permissions { get; set; } // defaults to ?

		/// <summary>
		/// <c>OwnerPassword</c> is an owner password for the the assembled form.
		/// </summary>
		[DataMember]
		public string OwnerPassword { get; set; } // defaults to null

		/// <summary>
		/// <c>UserPassword</c> is a user password for the the assembled form.
		/// </summary>
		[DataMember]
		public string UserPassword { get; set; } // defaults to null
	}

	/// <summary>
	/// Output options for HTML/MHTML documents
	/// </summary>
	[DataContract]
	public class HtmlOutputOptions : BasicOutputOptions
	{
		/// <summary>
		/// <c>Encoding</c> specifies the encoding (such as UTF8 or UTF16) for the assembled document. 
		/// If left empty the server default will be used.
		/// </summary>
		[DataMember]
		public string Encoding { get; set; } // defaults to null (leave it up to the server default)
	}

	/// <summary>
	/// Output options for plain text documents
	/// </summary>
	[DataContract]
	public class TextOutputOptions : OutputOptions
	{
		/// <summary>
		/// <c>Encoding</c> specifies the encoding (such as UTF8 or UTF16) for the assembled document. 
		/// If left empty the server default will be used.
		/// </summary>
		[DataMember]
		public string Encoding { get; set; } // defaults to null (leave it up to the server default)
	}

}
