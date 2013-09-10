/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML documentation.

/// <summary>
/// Summary description for UploadItem
/// </summary>
namespace SamplePortal.Data
{
	public class UploadItem
	{
		public string Title
		{
			get;
			set;
		}
		public string SessionID
		{
			get;
			set;
		}
		public string MainTemplateFileName
		{
			get;
			set;
		}
		public string PackageID
		{
			get;
			set;
		}
		public string Description
		{
			get;
			set;
		}
		public string FullFilePath
		{
			get;
			set;
		}

		/// <summary>
		/// ItemType can be an HD11+ template, a file, or a URL. HotDocs 11 (and later) sets this field, and HotDocs 10 does not set this field.
		/// The current Asp.Net web app could distinguish between HotDocs 11 templates and HotDocs 10 templates by examining this field to be 
		/// non-empty for HotDocs 11 and empty for HotDocs 10. This Sample Portal application does not save this value, and it is included
		/// here as sample code.
		/// </summary>
		public string ItemType
		{
			get;
			set;
		}

		/// <summary>
		/// LibraryPath is an optional field that is intended to give this portal an indication of where the current template was stored
		/// in the user's HotDocs library on the desktop. This Sample Portal application does not save this value, and it is included
		/// here as sample code.
		/// </summary>
		public string LibraryPath
		{
			get;
			set;
		}

		/// <summary>
		/// CommandLineSwitches is an optional field that contains any command line parameters that were used for the current template
		/// with the desktop HotDocs software. This Sample Portal application does not save this value, and it is included here as sample
		/// code.
		/// </summary>
		public string CommandLineSwitches
		{
			get;
			set;
		}

		/// <summary>
		/// ExpirationDate is an optional field that gives publishers the option of specifying a date when the current template will expire.
		///  This Sample Portal application does not save this value, and it is included here as sample code.
		/// </summary>
		public string ExpirationDate
		{
			get;
			set;
		}

		/// <summary>
		/// ExpirationWarningDays is an optional field used in conjunction with ExpirationDate that allows the user to be warned that the
		/// current template will expire the given number of days before the expireation date. This Sample Portal application does not 
		/// save this value, and it is included here as sample code.
		/// </summary>
		public string ExpirationWarningDays
		{
			get;
			set;
		}

		/// <summary>
		/// ExpirationExtensionDays is an optional field used in conjunction with ExpirationDate that allows the user to continue using the
		/// current template after the given expiration date has passed. This Sample Portal application does not save this value, and 
		/// it is included here as sample code.
		/// </summary>
		public string ExpirationExtensionDays
		{
			get;
			set;
		}
	}
}
