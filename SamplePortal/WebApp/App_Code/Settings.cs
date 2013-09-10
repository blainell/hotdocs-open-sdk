/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace SamplePortal
{
	/// <summary>
	/// Summary description for Settings.
	/// </summary>
	public static class Settings
	{
		#region Sample Portal Settings

		/// <summary>
		/// This is the name of the Web site, which appears at the top of each page and other places throughout
		/// the user interface.
		/// </summary>
		public static string SiteName
		{
			get
			{
				return GetSettingOrDefault("SiteName", "HotDocs Open SDK Sample Portal");
			}
		}
		#endregion

		#region HotDocs Server Settings

		/// <summary>
		/// This is the type of HotDocs engine that will service requests. It can be either a local instance of 
		/// HotDocs Server (LOCAL), the HotDocs Server Web Service (WS), or HotDocs Cloud Services (CLOUD).
		/// </summary>
		public static HotDocs.Sdk.Server.HdProtocol HdsRoute
		{
			get
			{
				switch (GetSettingOrDefault("HdsRoute", "LOCAL").ToUpper())
				{
					case "WS":
						return HotDocs.Sdk.Server.HdProtocol.WebService;
					case "CLOUD":
						return HotDocs.Sdk.Server.HdProtocol.Cloud;
					default: // LOCAL
						return HotDocs.Sdk.Server.HdProtocol.Local;
				}
			}
		}

		/// <summary>
		/// The source URL from which the HDServerFilesHandler will retrieve files when handling requests.
		/// </summary>
		public static string HDServerFilesUrl
		{
			get
			{
				// Use the Cloud Services URL as the default value since it is guaranteed to always be there.
				// Other possible defaults, such as http://localhost/HDServerFiles, would only work under very
				// limited circumstances.
				string defaultUrl = "http://files.hotdocs.ws/serverfiles";

				switch (HdsRoute)
				{
					case HotDocs.Sdk.Server.HdProtocol.WebService:
						return GetSettingOrDefault("HDServerFilesWS", defaultUrl);
					case HotDocs.Sdk.Server.HdProtocol.Cloud:
						return GetSettingOrDefault("HDServerFilesCloud", defaultUrl);
					default: //local
						return GetSettingOrDefault("HDServerFilesUrl", defaultUrl);
				}
			}
		}

		public static int MaxTitleLength
		{
			get
			{
				return GetSettingOrDefault("MaxTitleLength", 255);
			}
		}

		public static int MaxDescriptionLength
		{
			get
			{
				return GetSettingOrDefault("MaxDescriptionLength", 4000);
			}
		}

		public static string SigningKey
		{
			get
			{
				return GetSettingOrDefault("SigningKey", string.Empty);
			}
		}

		public static string SubscriberID
		{
			get
			{
				return GetSettingOrDefault("SubscriberID", string.Empty);
			}
		}

		public static string WebServiceEndPoint
		{
			get
			{
				return GetSettingOrDefault("WebServiceEndPoint", string.Empty);
			}
		}

		public static string JavaScriptUrl
		{
			get
			{
				return GetSettingOrDefault("InterviewRuntimeUrl", "HDServerFiles/js").TrimEnd('/');
			}
		}

		public static string StylesheetUrl
		{
			get
			{
				string url = GetSettingOrDefault("StylesheetUrl", "HDServerFiles/Stylesheets").TrimEnd('/');
				string theme = GetSettingOrDefault("InterviewTheme", "hdsuser");
				return url + "/" + theme + ".css";
			}
		}

		#endregion

		public static int UploadPageSize
		{
			get
			{
				return GetSettingOrDefault("UploadGridPageSize", 0);
			}
		}

		#region Silverlight / JavaScript interview format settings

		public static bool AllowInterviewFallback
		{
			get { return GetSettingOrDefault("InterviewFallback", true); }
		}

		#endregion

		public static string TemplatePath
		{
			get { return GetRootedPath(GetSettingOrDefault("TemplatePath", "Templates")); }
		}

		public static string AnswerPath
		{
			get { return GetRootedPath(GetSettingOrDefault("AnswerPath", "Answers")); }
		}

		public static string DocPath
		{
			get { return GetRootedPath(GetSettingOrDefault("DocumentPath", "Documents")); }
		}

		/// <summary>
		/// This is where Sample Portal caches the files requested through the HDServerFilesHandler.
		/// </summary>
		public static string CachePath
		{
			get { return GetRootedPath(GetSettingOrDefault("CachePath", "Cache")); }
		}

		public static bool AssembleHPTToPDF
		{
			get { return GetSettingOrDefault("AssembleHPTToPDF", true); }
		}

		public static string TempPath
		{
			get
			{
				string v = GetSettingOrDefault("TempPath", "Cache\\Tmp");
				//Look for potentially problematic paths. Is there anything else we should look for here?
				if (v == "\\" || v.Contains(".\\"))
					throw new Exception("Invalid temporary path.");

				if (v.Length == 0)
				{
					v = System.IO.Path.GetTempPath();
				}
				else
				{
					v = GetRootedPath(v);
				}

				return v;
			}
		}

		public static bool DebugApplication
		{
			get { return GetSettingOrDefault("DebugApplication", false); }
		}

		#region Database Settings
		public static string DefaultAnswerTableSortExpression
		{
			get { return GetSettingOrDefault("DefaultAnswerTableSortExpression", "LastModified DESC"); }
		}

		public static int AnswerGridPageSize
		{
			get
			{
				// The page size may not be less than 1 (default is 5).
				return Math.Max(GetSettingOrDefault("AnswerGridPageSize", 5), 1);
			}
		}

		public static string DefaultTemplateTableSortExpression
		{
			get
			{
				return GetSettingOrDefault("DefaultTemplateTableSortExpression", "Title");
			}
		}

		public static int TemplateGridPageSize
		{
			get
			{
				// The page size may not be less than 1 (default is 15).
				return Math.Max(GetSettingOrDefault("TemplateGridPageSize", 15), 1);
			}
		}
		#endregion

		#region Hard-coded Settings
		public static string TempRelPath
		{
			get { return "tmp" + System.IO.Path.DirectorySeparatorChar; }
		}

		public static int TempLen
		{
			get { return 4; }
		}
		#endregion

		#region Private methods
		private static string GetRootedPath(string path)
		{
			if (!Path.IsPathRooted(path))
			{
				string siteRoot = System.Web.Hosting.HostingEnvironment.MapPath("~");
				string siteRootParent = Directory.GetParent(siteRoot).FullName;
				path = Path.Combine(siteRootParent, "Files", path);
			}
			return path;
		}

		/// <summary>
		/// Reads a configuration setting and returns its value as a string (or the default value if it is not found)
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static string GetSettingOrDefault(string settingName, string defaultValue)
		{
			string val = ConfigurationManager.AppSettings[settingName];
			return string.IsNullOrWhiteSpace(val) ? defaultValue : val;
		}

		/// <summary>
		/// Reads a configuration setting and returns its value as a boolean (or the default value if it is not found)
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static bool GetSettingOrDefault(string settingName, bool defaultValue)
		{
			string val = GetSettingOrDefault(settingName, defaultValue.ToString());
			return val.Equals("True", StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Reads a configuration setting and returns its value as an integer (or the default value if it is not found)
		/// </summary>
		/// <param name="settingName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private static int GetSettingOrDefault(string settingName, int defaultValue)
		{
			int val;
			try
			{
				val = Convert.ToInt32(GetSettingOrDefault(settingName, defaultValue.ToString()));
			}
			catch (Exception)
			{
				val = defaultValue;
			}
			return val;
		}

		#endregion

	}
}
