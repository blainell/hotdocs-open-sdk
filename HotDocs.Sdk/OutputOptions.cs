/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;

namespace HotDocs.Sdk
{

	/// <summary>
	/// <c>PdfPermissions</c> 
	/// </summary>
	[Flags]
	public enum PdfPermissions
	{
		/// <summary>
		/// No permissions permitted
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Pdf Print permission
		/// </summary>
		Print = 0x4,

		/// <summary>
		/// Pdf Copy permission
		/// </summary>
		Copy = 0x10,

		/// <summary>
		/// Pdf Modify permission
		/// </summary>
		Modify = 0x28,

		/// <summary>
		/// All Pdf permissions -- Print, Copy, and Modify
		/// </summary>
		All = Print | Copy | Modify
	}

	/// <summary>
	/// Abstract base class of all OutputOptions.
	/// Provides facilities for serializing options in a very compact string representation for optimal transmission.
	/// </summary>
	public abstract class OutputOptions
	{
		// TODO: Provide facilities for serializing options in a very compact string representation for optimal transmission.
	}

	/// <summary>
	/// encapsulates all OutputOptions classes that incorporate support for document-level metadata
	/// </summary>
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
		public string Author { get; set; } // defaults to null

		/// <summary>
		/// Comments pertaining to the assembled document.
		/// </summary>
		public string Comments { get; set; } // defaults to null

		/// <summary>
		/// Company of the assembled document.
		/// </summary>
		public string Company { get; set; } // defaults to null

		/// <summary>
		/// Keywords of the assembled document.
		/// </summary>
		public string Keywords { get; set; } // defaults to null

		/// <summary>
		/// Subject of the assembled document.
		/// </summary>
		public string Subject { get; set; } // defaults to null

		/// <summary>
		/// Title of the assembled document.
		/// </summary>
		public string Title { get; set; } // defaults to null

		private Dictionary<string, string> _customValues;


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
	public class PdfOutputOptions : BasicOutputOptions
	{
		/// <summary>
		/// <c>EmbedFonts</c> flag for the the assembled form.
		/// </summary>
		public bool EmbedFonts { get; set; } // defauls to Default

		/// <summary>
		/// <c>PdfA</c> boolean flag for the the assembled form.
		/// </summary>
		public bool PdfA { get; set; } // defaults to false

		/// <summary>
		/// <c>TaggedPdf</c> flag for the the assembled form.
		/// </summary>
		public bool TaggedPdf { get; set; } // defaults to false

		/// <summary>
		/// KeepFillablePdf flag for the the assembled form.
		/// </summary>
		public bool KeepFillablePdf { get; set; } // defaults to false

		/// <summary>
		/// <c>TruncateFields</c> flag for the the assembled form.
		/// </summary>
		public bool TruncateFields { get; set; } // defaults to ?

		/// <summary>
		/// <c>PdfPermissions</c> enumeration (print, modify, copy) for the the assembled form.
		/// </summary>
		public PdfPermissions Permissions { get; set; } // defaults to ?

		/// <summary>
		/// <c>OwnerPassword</c> is an owner password for the the assembled form.
		/// </summary>
		public string OwnerPassword { get; set; } // defaults to null

		/// <summary>
		/// <c>UserPassword</c> is a user password for the the assembled form.
		/// </summary>
		public string UserPassword { get; set; } // defaults to null
	}

	/// <summary>
	/// Output options for HTML/MHTML documents
	/// </summary>
	public class HtmlOutputOptions : BasicOutputOptions
	{
		/// <summary>
		/// <c>Encoding</c> specifies the encoding (such as UTF8 or UTF16) for the assembled document. 
		/// If left empty the server default will be used.
		/// </summary>
		public string Encoding { get; set; } // defaults to null (leave it up to the server default)
	}

	/// <summary>
	/// Output options for plain text documents
	/// </summary>
	public class TextOutputOptions : OutputOptions
	{
		/// <summary>
		/// <c>Encoding</c> specifies the encoding (such as UTF8 or UTF16) for the assembled document. 
		/// If left empty the server default will be used.
		/// </summary>
		public string Encoding { get; set; } // defaults to null (leave it up to the server default)
	}

}
