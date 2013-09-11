/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML documentation.

using System;
using System.IO;

using HotDocs.Sdk.Server.Contracts;

namespace SamplePortal
{
	public enum AnsSrcType
	{
		NewSrc,
		UploadedSrc,
		ServerSrc
	}

	public class Document
	{
		private string m_templateTitle = "";
		public string TemplateTitle
		{
			get
			{
				return m_templateTitle;
			}
			set
			{
				m_templateTitle = value;
			}
		}

		private string m_documentFilePath = "";
		public string DocumentFilePath
		{
			get
			{
				return m_documentFilePath;
			}
			set
			{
				m_documentFilePath = value;
			}
		}
		public Document(string documentFilePath, string templateTitle)
		{
			TemplateTitle = templateTitle;
			DocumentFilePath = documentFilePath;
		}
	}
	/// <summary>
	/// Global utility functions (all static methods).  No need to create an instance of Util.
	/// </summary>
	public static class Util
	{
		public static string ConvertNumToBase(ulong num, byte baseN)
		{
			// takes any positive integer (up to 64-bit) and returns a string representing that number int base n (max base == 37)
			const string alldigits = "0123456789abcdefghijklmnopqrstuvwxyz_";
			if (num == 0)
				return "";
			return ConvertNumToBase(num / baseN, baseN) + alldigits[unchecked((int)(num % baseN))];
		}

		public static string MakeTempFilename(string ext)
		{
			string tmp = ConvertNumToBase(unchecked((ulong)DateTime.Now.Ticks), 36);
			return (tmp.Length > 10 ? tmp.Substring(tmp.Length - 10) : tmp) + ext;
		}

		public static string EnsureValidFilename(string filename)
		{
			return System.Text.RegularExpressions.Regex.Replace(filename, "[/\\:<>|?*\"]", "_");
		}

		public static bool IsEmpty(string s)
		{
			return (s == null || s == String.Empty || s.Length == 0);
		}

		public static void SafeDeleteFolder(string folder)
		{
			if (folder != null && folder != "" && folder[0] != '\\')
				System.IO.Directory.Delete(folder, true);
		}
		//TODO: Move this to HotDocs.Sdk.Template?
		public static string GetDocExtension(string tplfname)
		{
			tplfname = Path.GetExtension(tplfname).ToLower();
			switch (tplfname)
			{
				case ".rtf": return ".rtf";
				case ".hpt": return ".pdf";
				case ".wpt": return ".wpd";
				case ".hft": return ".hfd";
				case ".cmp": return null;
			}
			return ".rtf"; // default
		}

		/// <summary>
		/// This method deletes a template package and its corresponding manifest file from the Templates folder.
		/// </summary>
		/// <param name="pkgFileName">The name of the template package to delete. The manifest will have the same name, but a different file name extension.</param>
		//TODO: Perhaps this should be moved to the PackageCache class.
		//TODO: The extracted files and folder should be removed as well.
		public static void DeleteTemplatePackage(string pkgFileName)
		{
			// Delete the package file.
			string packagePath = Path.Combine(Settings.TemplatePath, pkgFileName);
			if (File.Exists(packagePath))
				File.Delete(packagePath);

			// Delete the manifest file.
			string manifestPath = System.IO.Path.ChangeExtension(packagePath, ".xml");
			if (File.Exists(manifestPath))
				File.Delete(manifestPath);
		}

		public static void SaveAnswers(HotDocs.Sdk.AnswerCollection ansColl, string newTitle, string newDescription)
		{
			string answerPath = ansColl.FilePath;
			string answerFileName = Path.GetFileName(answerPath);

			using (SamplePortal.Data.Answers answers = new SamplePortal.Data.Answers())
			{
				if (!String.IsNullOrEmpty(ansColl.FilePath))//If this assembly began with an answer file on the server
				{
					System.Data.DataView ansData = answers.SelectFile(answerFileName);
					if (ansData.Count > 0 && ansData[0]["Title"].ToString() == newTitle)//If the title did not change
					{
						//Update the existing entry.
						ansColl.WriteFile(false);
						answers.UpdateAnswerFile(answerFileName, newTitle, newDescription);
					}
					else
					{
						//Create a new entry and answer set.
						string newfname = Util.MakeTempFilename(".anx");
						ansColl.WriteFile(Path.Combine(Settings.AnswerPath, newfname), false);

						answers.InsertNewAnswerFile(newfname, newTitle, newDescription, null);
					}
				}
				else//If this assembly began with a new or uploaded answer file
				{
					//Create a new answer file.
					string newfname = Util.MakeTempFilename(".anx");
					ansColl.WriteFile(Path.Combine(Util.SafeDir(Settings.AnswerPath), newfname), false);

					//Create a new entry.
					answers.InsertNewAnswerFile(newfname, newTitle, newDescription, null);
				}
			}
		}

		//TODO: Remove.
		public static string GetTemporaryAnsFilename(string ansFilename)
		{
			return ansFilename.Substring(Settings.TempLen);
		}

		public static void SweepTempDirectories()
		{
			//TODO: Are we sweeping the right folders?
			SweepDirectory(Settings.DocPath, 60);
			SweepDirectory(Path.Combine(Settings.AnswerPath, Settings.TempRelPath), 60);
		}

		//TODO: Move the comment to XML comments.
		public static void SweepDirectory(string dirName, int timeoutMinutes)
		{
			// this goes through every file in the named directory, and attempts to
			// delete all files that were created more than timeoutMinutes ago.
			// USE WITH CARE! THIS CAN DELETE ANY FILE THE USER HAS RIGHTS TO!
			DirectoryInfo dInfo = new DirectoryInfo(dirName);
			if (dInfo.Exists)
			{
				FileInfo[] fInfos = dInfo.GetFiles();
				foreach (FileInfo fInfo in fInfos)
				{
					if (fInfo.CreationTime.AddMinutes(timeoutMinutes) < DateTime.Now)
						fInfo.Delete();
				}
			}
		}

		//TODO: The implementation of this method could be cleaned up. It has the following problems:
		// The parameter variable is modified.
		// The text " DESC" is added and then removed for later comparison.
		public static string ToggleSortOrder(System.Web.UI.WebControls.DataGrid grid, string newSort, string currentSort)
		{
			if (newSort != null)
			{
				if (newSort == currentSort && !currentSort.EndsWith(" DESC"))
					currentSort += " DESC";
				else
					currentSort = newSort;
			}

			//this code block is to put a little triangle sort hint.
			string sortPrefix = currentSort;
			if (sortPrefix.EndsWith(" DESC"))
				sortPrefix = sortPrefix.Substring(0, sortPrefix.Length - 5);
			for (int i = 0; i < grid.Columns.Count; i++)
			{
				//strip the existing <span> tag if there is one
				int tagIndex = grid.Columns[i].HeaderText.IndexOf("<span");
				if (tagIndex > 0)
					grid.Columns[i].HeaderText = grid.Columns[i].HeaderText.Substring(0, tagIndex - 1);

				//add the character to the column that's being sorted
				if (grid.Columns[i].SortExpression == sortPrefix)
				{
					string letter = "&#9662;";
					if (currentSort.EndsWith("DESC"))
						letter = "&#9652;";
					grid.Columns[i].HeaderText += " <span>" + letter + "</span>";
				}
			}

			return currentSort;
		}

		public static void CustomizePager(System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			System.Web.UI.WebControls.TableCell pager = (System.Web.UI.WebControls.TableCell)e.Item.Controls[0];
			for (int i = 0; i < pager.Controls.Count; i++)
			{
				if (pager.Controls[i].ToString() == "System.Web.UI.WebControls.Label")
				{
					System.Web.UI.WebControls.Label lbl = (System.Web.UI.WebControls.Label)pager.Controls[i];
					lbl.Text = "Page " + lbl.Text;
				}
				else if (pager.Controls[i].ToString() == "System.Web.UI.LiteralControl")
				{
					System.Web.UI.LiteralControl lit = (System.Web.UI.LiteralControl)pager.Controls[i];
					lit.Text = " | ";
				}
				else
				{
					System.Web.UI.WebControls.LinkButton lnk = (System.Web.UI.WebControls.LinkButton)pager.Controls[i];
					lnk.Text = "Page " + lnk.Text;
				}
			}
		}

		public static void SetNewPageSize(System.Web.UI.WebControls.DataGrid dataGrid, int pageSize, int recordCount)
		{
			if (pageSize < 1)
				throw new ArgumentOutOfRangeException("The grid size must be greater than zero.");

			dataGrid.PageSize = pageSize;
			if (dataGrid.CurrentPageIndex > recordCount / pageSize)
				dataGrid.CurrentPageIndex = recordCount / pageSize;
		}

		public static bool CheckIfGenerateTemplateManifests(string tplPath)
		{
			string tplManifestPath = tplPath + ".manifest.xml";
			if (!File.Exists(tplManifestPath))
			{
				// If the upload is from a pre-HotDocs 11 version of HotDocs then no template manifests will have been
				// published and uploaded. In this case force the creation of an JavaScript interview definition file for
				// the template that will have the side effect of creating a template manifest for it also. Making sure 
				// there are templates manifest created for all templates assures that a variable colection file can be 
				// dynamically created by the server when an interview is requested for the template. 

				// We also no longer need to keep around in Sample Portal uploaded variable collection files (HVC) or 
				// JavaScript (JS) files because the server now generates these files on the server and caches them.
				File.Delete(Path.ChangeExtension(tplPath, ".hvc"));
				File.Delete(Path.ChangeExtension(tplPath, ".js"));

				//Generate the template manifest.
				return true;
			}
			return false;//Do not generate the template manifest.
		}

		//TODO: Not used. Remove?
		public static string GetBrowserName(System.Web.HttpRequest request)
		{
			//Determine what browser is being used
			string browserName = "unknown";
			if (request.Browser.IsBrowser("IE"))
				browserName = "IE";
			else if (request.Browser.IsBrowser("Safari"))
			{
				if (request.UserAgent.Contains("Chrome"))
					browserName = "chrome";
				else
					browserName = "safari";
			}
			else if (request.Browser.IsBrowser("Opera"))
				browserName = "opera";
			else if (request.Browser.Browser.Contains("Firefox"))
				browserName = "firefox";
			return browserName;
		}

		public static bool BrowserSupportsInlineImages(System.Web.HttpRequest req)
		{
			string inlineImages = req.Form["InlineImages"];
			string browser = req.Browser.Browser.ToUpper();
			string type = req.Browser.Type.ToUpper();

			bool supportsInlineImages;
			if (inlineImages != null)
				supportsInlineImages = (inlineImages.ToLowerInvariant() == "true");
			else
				supportsInlineImages = !browser.Contains("IE") || type == "IE9" || type == "IE10"; // IE9 supports up to 4GB in a data uri (http://msdn.microsoft.com/en-us/ie/ff468705.aspx#_DataURI)

			return supportsInlineImages;
		}

		/// <summary>
		/// Wrap a file path in this call in order to make sure that the directory exists.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		//TODO: Not used. Remove?
		public static string SafeFile(string filePath)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			return filePath;
		}

		/// <summary>
		/// Wrap a directory path in this call in order to make sure that the directory exists.
		/// </summary>
		/// <param name="dirPath"></param>
		/// <returns></returns>
		public static string SafeDir(string dirPath)
		{
			Directory.CreateDirectory(dirPath);
			return dirPath;
		}

		public static AssembledDocsCache GetAssembledDocsCache(System.Web.SessionState.HttpSessionState session)
		{
			AssembledDocsCache cache = (AssembledDocsCache)session["AssembledDocsCache"];
			if (cache == null)
			{
				cache = new AssembledDocsCache(Settings.TempPath);
				session["AssembledDocsCache"] = cache;
			}
			return cache;
		}

		public static string GetInterviewAnswers(System.Web.HttpRequest request)
		{
			string ansdata = String.Join(String.Empty, request.Form.GetValues("HDInfo"));
			return ansdata;
		}

		#region Debugging aids
		//TODO: Not used. Remove?
		public static void debugger()
		{
			if (System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Break();
			else
				System.Diagnostics.Debugger.Launch();
		}
		#endregion
	}
}
