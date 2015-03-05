/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using SamplePortal;
using SamplePortal.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class Upload : System.Web.UI.Page
{
	private int TotalNumTemplates = -1;
	private int RemainingTemplates = -1;
	private int _gridSize = -1;

	private List<UploadItem> ConflictUploadItems
	{
		get
		{
			if (Session["ConflictUploadItems"] == null)
				this.ConflictUploadItems = new List<UploadItem>();
			return Session["ConflictUploadItems"] as List<UploadItem>;
		}
		set
		{
			Session["ConflictUploadItems"] = value;
		}
	}

	private List<UploadItem> SuccessfulUploadItems
	{
		get
		{
			if (Session["SuccessfulUploadItems"] == null)
				this.SuccessfulUploadItems = new List<UploadItem>();
			return Session["SuccessfulUploadItems"] as List<UploadItem>;
		}
		set
		{
			Session["SuccessfulUploadItems"] = value;
		}
	}

	private List<UploadItem> CanceledUploadItems
	{
		get
		{
			if (Session["CanceledUploadItems"] == null)
				this.CanceledUploadItems = new List<UploadItem>();
			return Session["CanceledUploadItems"] as List<UploadItem>;
		}
		set
		{
			Session["CanceledUploadItems"] = value;
		}
	}
	
	private int GridSize
	{
		get
		{
			if (_gridSize < 0)
				_gridSize = Settings.UploadPageSize;

			if (_gridSize == 0)
				_gridSize = RemainingTemplates;
			if (RemainingTemplates < _gridSize)
				_gridSize = RemainingTemplates;

			return _gridSize;
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (Request.QueryString.HasKeys())
		{
			string numTemplates = Request.QueryString["HD_NumberTemplates"];
			// Get the number of templates that remain to be uploaded.
			int.TryParse(numTemplates, out RemainingTemplates);

			// If this is the first time at the page, the number of remaining templates is the same as the total number of templates.
			if (string.IsNullOrEmpty(TotalTemplateCount.Value))
			{
				TotalTemplateCount.Value = RemainingTemplates.ToString();
				SuccessfulUploadItems.Clear();
				ConflictUploadItems.Clear();
				CanceledUploadItems.Clear();
			}
			TotalNumTemplates = int.Parse(TotalTemplateCount.Value);
		}

		UpdatePanelVisibility();

		if (RemainingTemplates > 0)
		{
			UploadedTemplatesGrid.Rows.Clear();


			for (int i = 0; i < GridSize; i++)
			{
				System.Web.UI.HtmlControls.HtmlTableRow tr = new System.Web.UI.HtmlControls.HtmlTableRow();
				System.Web.UI.HtmlControls.HtmlTableCell tc;

				// Template Title
				tc = new System.Web.UI.HtmlControls.HtmlTableCell();
				tc.Width = "30%";
				TextBox templateTitle = new TextBox();
				templateTitle.ID = string.Format("HD_Template_Title{0}", i.ToString());
				templateTitle.Rows = 3;
				templateTitle.TextMode = TextBoxMode.MultiLine;
				tc.Controls.Add(templateTitle);
				tr.Controls.Add(tc);

				// Template Description
				tc = new System.Web.UI.HtmlControls.HtmlTableCell();
				tc.Width = "70%";
				TextBox templateDescription = new TextBox();
				templateDescription.ID = string.Format("HD_Template_Description{0}", i.ToString());
				templateDescription.Rows = 3;
				templateDescription.TextMode = TextBoxMode.MultiLine;
				tc.Controls.Add(templateDescription);
				tr.Controls.Add(tc);

				// Hidden controls
				tc = new System.Web.UI.HtmlControls.HtmlTableCell();
				tc.Style.Add("display", "none");

				// The upload item type can be a template, a file, or a URL. This field is utilized by HotDocs 11 (and later). The
				// Upload plugin for HotDocs 10 does not set this field.
				HiddenField uploadItemType = new HiddenField();
				uploadItemType.ID = "HD_Template_UploadItemType" + i;
				tc.Controls.Add(uploadItemType);

				// Main Template File Name
				HiddenField templateMain = new HiddenField();
				templateMain.ID = "HD_Template_FileName" + i;
				tc.Controls.Add(templateMain);

				HiddenField libraryPath = new HiddenField();
				libraryPath.ID = "HD_Template_Library_Path" + i;
				tc.Controls.Add(libraryPath);

				HiddenField expirationDate = new HiddenField();
				expirationDate.ID = "HD_Template_Expiration_Date" + i;
				tc.Controls.Add(expirationDate);

				HiddenField expirationWarningDays = new HiddenField();
				expirationWarningDays.ID = "HD_Template_Warning_Days" + i;
				tc.Controls.Add(expirationWarningDays);

				HiddenField expirationExtensionDays = new HiddenField();
				expirationExtensionDays.ID = "HD_Template_Extension_Days" + i;
				tc.Controls.Add(expirationExtensionDays);

				HiddenField commandLineSwitches = new HiddenField();
				commandLineSwitches.ID = "HD_Template_CommandLineSwitches" + i;
				tc.Controls.Add(commandLineSwitches);

				// Template package file upload control
				FileUpload ulCtrl = new FileUpload();
				ulCtrl.ID = string.Format("HD_Upload{0}", i.ToString());
				tc.Controls.Add(ulCtrl);

				// Template manifest file upload control
				FileUpload ulPackageManifestXML = new FileUpload();
				ulPackageManifestXML.ID = string.Format("HD_Package_Manifest{0}", i.ToString());
				tc.Controls.Add(ulPackageManifestXML);

				tr.Controls.Add(tc);

				UploadedTemplatesGrid.Rows.Add(tr);
			}

			if (GridSize > 1)
				submitBtn.Text = string.Format("Upload Templates {0}-{1} of {2}", (TotalNumTemplates - RemainingTemplates) + 1, GridSize, TotalNumTemplates);
			else
				submitBtn.Text = string.Format("Upload Template {0} of {1}", (TotalNumTemplates - RemainingTemplates) + 1, TotalNumTemplates);
		}

		if (Request.Files.Count > 0)
		{
			int templateIndex = 0;
			int fileIndex = 0;
			while (fileIndex < Request.Files.Count)
			{
				HttpPostedFile postedFile = Request.Files[fileIndex];

				if (postedFile.ContentLength > 0 && !string.IsNullOrEmpty(postedFile.FileName))
				{
					FileInfo finfo = new FileInfo(postedFile.FileName);
					if (finfo.Extension.ToLower() != ".xml")
					{
						string ext = Path.GetExtension(postedFile.FileName).ToLower();
						string baseFileName = Path.GetFileNameWithoutExtension(postedFile.FileName);
						// note: there are three possibilities here:
						// 1) The fileName is a template package, formatted <guid>.pkg
						// 2) The fileName is a url, formatted <guid>.url
						// 3) The filename is a raw file, formatted with the a non-guid base file name and extension:
						// Note: Here in this sample portal we ignore 2) and 3) because we are only concerned
						// with uploading templates:
						if (!IsGuid(baseFileName) || ext == ".url")
						{
							return;
						}
						string packageID = baseFileName;
						string packagePath = PackageCache.GetLocalPackagePath(packageID);
						if (!string.IsNullOrEmpty(packageID))
						{
							FileStream fs = File.OpenWrite(packagePath);
							postedFile.InputStream.CopyTo(fs);
							fs.Close();

							UploadItem infoItem = new UploadItem
							{
								Title = Request.Form["HD_Template_Title" + templateIndex],
								Description = Request.Form["HD_Template_Description" + templateIndex],
								MainTemplateFileName = Request.Form["HD_Template_FileName" + templateIndex],

								/// LibraryPath is an optional field that is intended to give this portal an indication
								/// of where the current template was stored in the user's HotDocs library on the desktop. 
								/// This Sample Portal application does not save this value, and it is included
								/// here as sample code.
								LibraryPath = Request.Form["HD_Library_Path" + templateIndex],

								/// CommandLineSwitches is an optional field that contains any command line parameters
								/// that were used for the current template with the desktop HotDocs software. This 
								/// Sample Portal application does not save this value, and it is included here 
								/// as sample code because these command line parameters may be of use.
								CommandLineSwitches = Request.Form["HD_Template_CommandLineSwitches" + templateIndex],

								/// ItemType can be an HD11+ template, a file, or a URL. HotDocs 11 (and later) 
								/// sets this field, and HotDocs 10 does not set this field.  The current 
								/// Asp.Net web app could distinguish between HotDocs 11 templates and HotDocs 10 
								/// templates by examining this field to be non-empty for HotDocs 11 and empty 
								/// for HotDocs 10. This Sample Portal application does not save this value, 
								/// and it is included here as sample code.
								ItemType = Request.Form["HD_Template_UploadItemType" + templateIndex],

								/// ExpirationDate is an optional field that gives publishers the option of specifying
								/// a date when the current template will expire. This Sample Portal application 
								/// does not save this value, and it is included here as sample code. 
								ExpirationDate = Request.Form["HD_Template_Expiration_Date" + templateIndex],

								/// ExpirationWarningDays is an optional field used in conjunction with ExpirationDate 
								/// that allows the user to be warned that the current template will expire the given 
								/// number of days before the expireation date. This Sample Portal application 
								/// does not save this value, and it is included here as sample code.
								ExpirationWarningDays = Request.Form["HD_Template_Warning_Days" + templateIndex],

								/// ExpirationExtensionDays is an optional field used in conjunction with ExpirationDate 
								/// that allows the user to continue using the current template after the given 
								/// expiration date has passed. This Sample Portal application does not save this 
								/// value, and it is included here as sample code.
								ExpirationExtensionDays = Request.Form["HD_Template_Extension_Days" + templateIndex],
								PackageID = packageID,
								FullFilePath = packagePath
							};

							using (SamplePortal.Data.Templates templates = new SamplePortal.Data.Templates())
							{
								if (templates.TemplateExists(infoItem.MainTemplateFileName, infoItem.Title))
									ConflictUploadItems.Add(infoItem);
								else
								{
									templates.InsertNewTemplate(infoItem);
									SuccessfulUploadItems.Add(infoItem);
								}
							}
						}
						templateIndex++;
					}
					else
					{
						string filename = Path.Combine(Settings.TemplatePath, finfo.Name);
						FileStream fs = File.OpenWrite(filename);
						postedFile.InputStream.CopyTo(fs);
						fs.Close();
					}
					fileIndex++;
				}
				else
				{
					panelHDError.Visible = true;
					break;
				}
			}
			UpdatePanelVisibility();
		}
	}

	protected void btnCancel_Click(object sender, EventArgs e)
	{
		foreach (UploadItem i in ConflictUploadItems)
		{
			// Delete the new package from disk since the user has decided not to go through with overwriting the old package.
			PackageCache.DeleteTemplatePackage(i.PackageID);

			CanceledUploadItems.Add(i);	// Add the item to the list of canceled items.
		}

		ConflictUploadItems.Clear(); // Remove all conflict items.
		UpdatePanelVisibility();
	}

	protected void btnContinue_Click(object sender, EventArgs e)
	{
		using (SamplePortal.Data.Templates templates = new SamplePortal.Data.Templates())
		{
			foreach (UploadItem i in ConflictUploadItems)
			{
				templates.UpdateTemplate(i);	// Overwrite the existing entry with the new one and delete the old package.
				SuccessfulUploadItems.Add(i); // Add the item to the list of successfully uploaded packages.
			}
			ConflictUploadItems.Clear(); // Remove all conflict items.
		}

		UpdatePanelVisibility();
	}

	private static Regex isGuid =
	  new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);

	internal static bool IsGuid(string candidate)
	{
		bool isValid = false;

		if (candidate != null)
		{

			if (isGuid.IsMatch(candidate))
			{
				isValid = true;
			}
		}

		return isValid;
	}

	private void UpdatePanelVisibility()
	{
		panelOverwrite.Visible = false;
		panelHDForm.Visible = false;
		panelHDFinished.Visible = false;

		if (ConflictUploadItems.Count > 0)
		{
			panelOverwrite.Visible = true;

			lblOverwriteList.Text = "<ul>";
			foreach (UploadItem i in ConflictUploadItems)
			{
				lblOverwriteList.Text += "<li>" + i.Title + "</li>";
			}
			lblOverwriteList.Text += "</ul>";
		}
		else if (RemainingTemplates > 0)
		{
			panelHDForm.Visible = true;
		}
		else
		{
			panelHDFinished.Visible = true;

			if (SuccessfulUploadItems.Count <= 0 && CanceledUploadItems.Count <= 0)
			{
				lblStatus.Text = "<p>This page handles template packages uploaded from HotDocs Developer. To learn how to upload packages, see the note on the <a href='Templates.aspx'>Template Management</a> page.</p>";
				return;
			}

			lblStatus.Text = "<p>The template package upload is now complete.</p>";

			if (SuccessfulUploadItems.Count > 0)
			{
				lblStatus.Text += string.Format("<p>The following item{0} successfully uploaded to {1}:</p><ul>", SuccessfulUploadItems.Count == 1 ? " was" : "s were", Settings.SiteName);
				foreach (UploadItem i in SuccessfulUploadItems)
				{
					lblStatus.Text += "<li>" + i.Title + "</li>";
				}
				lblStatus.Text += "</ul>";
				SuccessfulUploadItems.Clear();
			}

			if (CanceledUploadItems.Count > 0)
			{
				lblStatus.Text += string.Format("<p>The upload of the following item{0} was canceled:</p><ul>", CanceledUploadItems.Count == 1 ? "" : "s");
				foreach (UploadItem i in CanceledUploadItems)
				{
					lblStatus.Text += "<li>" + i.Title + "</li>";
				}
				lblStatus.Text += "</ul>";
				CanceledUploadItems.Clear();
			}
		}
	}
}