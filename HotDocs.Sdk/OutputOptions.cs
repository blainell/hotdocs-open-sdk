/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;

namespace HotDocs.Sdk
{
	[Flags]
	public enum PdfPermissions
	{
		None = 0x0,
		Print = 0x4,
		Copy = 0x10,
		Modify = 0x28,
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
		public string Author { get; set; } // defaults to null
		public string Comments { get; set; } // defaults to null
		public string Company { get; set; } // defaults to null
		public string Keywords { get; set; } // defaults to null
		public string Subject { get; set; } // defaults to null
		public string Title { get; set; } // defaults to null

		private Dictionary<string, string> _customValues;

		public void SetValue(string name, string value)
		{
			_customValues[name] = value;
		}

		public string GetValue(string name)
		{
			return _customValues[name];
		}

		public bool HasValue(string name)
		{
			return _customValues.ContainsKey(name);
		}
	}

	/// <summary>
	/// Output options for PDF documents.
	/// </summary>
	public class PdfOutputOptions : BasicOutputOptions
	{
		public PdfOutputOptions() : base() { }

		public Tristate EmbedFonts { get; set; } // defauls to Default
		public bool PdfA { get; set; } // defaults to false
		public bool TaggedPdf { get; set; } // defaults to false
		public bool KeepFillablePdf { get; set; } // defaults to false
		public bool TruncateFields { get; set; } // defaults to ?
		public PdfPermissions Permissions { get; set; } // defaults to ?
		public string OwnerPassword { get; set; } // defaults to null
		public string UserPassword { get; set; } // defaults to null
	}

	/// <summary>
	/// Output options for HTML/MHTML documents
	/// </summary>
	public class HtmlOutputOptions : BasicOutputOptions
	{
		public HtmlOutputOptions() : base() { }

		public string Encoding { get; set; } // defaults to null (leave it up to the server default)
	}

	/// <summary>
	/// Output options for plain text documents
	/// </summary>
	public class TextOutputOptions : OutputOptions
	{
		public TextOutputOptions() { }

		public string Encoding { get; set; } // defaults to null (leave it up to the server default)
	}

}
