/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments.
//TODO: Remove the Metadata table from TemplateData.xsd.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SamplePortal.Data
{
	/// <summary>
	/// The <c>Templates</c> class provides access to the list of main templates on the server. The templates
	/// are listed in an XML document, index.xml, with the schema defined in TemplateData.xsd.
	/// </summary>
	public class Templates : IDisposable
	{
		/// <summary>
		/// The template data set generated from TemplateData.xsd.
		/// </summary>
		protected TemplateData tplData;

		/// <summary>
		/// Construct a new <c>Template object</c>.
		/// </summary>
		public Templates()
		{
			tplData = new TemplateData();
			try
			{
				tplData.ReadXml(Path.Combine(Settings.TemplatePath, "index.xml"));
			}
			catch (System.IO.FileNotFoundException)
			{ }
			catch (System.IO.DirectoryNotFoundException)
			{ }
			tplData.CaseSensitive = false;
		}

		#region IDisposable Members

		public void Dispose()
		{
			tplData.Dispose();
		}

		#endregion

		/// <summary>
		/// Returns a DataView for a specific template.
		/// </summary>
		/// <param name="tplname">The template name.</param>
		/// <returns></returns>
		public DataView SelectFile(string tplname)
		{
			DataView dv = new DataView(tplData.Templates);
			dv.RowFilter = "[Filename] = '" + EscapeStringForFilter(tplname, false) + "'";
			return dv;
		}
		/// <summary>
		/// Returns a sorted DataView for a given filter.
		/// </summary>
		/// <param name="sortExpression">The sort expression. See DataView.Sort.</param>
		/// <param name="searchFilter">The filter criterea. See DataView.RowFilter</param>
		/// <returns></returns>
		public DataView SelectAll(string sortExpression)
		{
			return SelectAll(sortExpression, null);
		}

		/// <summary>
		/// Returns a sorted DataView for a given filter.
		/// </summary>
		/// <param name="sortExpression">The sort expression. See DataView.Sort.</param>
		/// <param name="searchFilter">The filter criterea. See DataView.RowFilter</param>
		/// <returns></returns>
		public DataView SelectAll(string sortExpression, string searchFilter)
		{
			DataView dv = new DataView(tplData.Templates);
			if (sortExpression != null)
				dv.Sort = sortExpression;
			if (searchFilter != null && searchFilter.Length > 0)
				dv.RowFilter = "[Title] LIKE '%" + searchFilter + "%' OR [Description] LIKE '%" + searchFilter + "%'";
			return dv;
		}

		//TODO: Remove. Metadata table is not used.
		private DataRowView LoadMetaDataRow(string name)
		{
			DataView dv = new DataView(tplData.Metadata);
			if (dv != null)
			{
				try
				{
					dv.RowFilter = "[Name] = '" + EscapeStringForFilter(name, false) + "'";
				}
				catch (Exception)
				{
					dv.Dispose();
					return null;
				}
			}
			return dv.Count > 0 ? dv[0] : null;
		}

		//TODO: Remove. Metadata table is not used.
		public bool HasMetadataValue(string name)
		{
			DataRowView drv = LoadMetaDataRow(name);
			return drv != null;
		}

		//TODO: Remove. Metadata table is not used.
		public string GetMetadataValue(string name)
		{
			DataRowView drv = LoadMetaDataRow(name);
			string val = "";
			if (drv != null)
			{
				try
				{
					val = (string)drv["Value"];
				}
				catch (Exception)
				{
					val = "";
				}
			}
			return val;
		}

		//TODO: Remove. Metadata table is not used.
		public void SetMetadataValue(string name, string value)
		{
			DataRowView drv = LoadMetaDataRow(name);
			if (drv != null)
			{
				drv["Value"] = value;
			}
			else
			{
				DataRow newRow = tplData.Metadata.NewRow();
				newRow["Name"] = name;
				newRow["Value"] = value;
				tplData.Metadata.Rows.Add(newRow);
			}
			FlushUpdates();
		}

		//TODO: Remove. Metadata table is not used.
		public string GetVersion()
		{
			return GetMetadataValue("Version");
		}

		//TODO: Remove. Metadata table is not used.
		public void SetVersion(string version)
		{
			SetMetadataValue("Version", version);
		}

		//TODO: Not used. Remove?
		public System.Collections.Generic.IEnumerable<string> GetTemplateFiles()
		{
			List<string> items = new List<string>();
			foreach (DataRow row in tplData.Templates.Rows)
				items.Add(row["Filename"].ToString());
			return items;
		}


		public bool TemplateExists(string filename)
		{
			DataView dv = SelectFile(filename);
			return (dv.Count > 0);
		}

		public void InsertNewTemplate(UploadItem ui)
		{
			DataRow newRow = tplData.Templates.NewRow();
			newRow["PackageID"] = ui.PackageID;
			if (!string.IsNullOrEmpty(ui.MainTemplateFileName))
				newRow["Filename"] = ui.MainTemplateFileName;
			if (!string.IsNullOrEmpty(ui.Title))
				newRow["Title"] = ui.Title;
			else
			{
				if (!string.IsNullOrEmpty(ui.MainTemplateFileName))
					newRow["Title"] = ui.MainTemplateFileName; //if no title is supplied, use the filename
			}
			if (ui.Description != null)
				newRow["Description"] = ui.Description;
			newRow["DateAdded"] = DateTime.Now;
			tplData.Templates.Rows.Add(newRow);
			FlushUpdates();
		}

		public void UpdateTemplate(UploadItem ui)
		{
			UpdateTemplate(ui.MainTemplateFileName, ui.Title, ui.Description, ui.PackageID);
		}

		public void UpdateTemplate(string filename, string title, string desc, string packageID)
		{
			DataView dv = SelectFile(filename);

			string curPkgID = dv[0]["PackageID"].ToString();

			if (packageID != null)
			{
				if (curPkgID != packageID)
				{
					// If the packageID is different than the old one, delete the old package from disk.
					Util.DeleteTemplatePackage(curPkgID);
				}

				dv[0]["PackageID"] = packageID;
			}
			if (title != null) // pass null if you don't want something changed
				dv[0]["Title"] = title;
			if (desc != null)
				dv[0]["Description"] = desc;
			FlushUpdates();
		}

		public void DeleteTemplate(string filename)
		{
			DataView dv = SelectFile(filename);
			if (dv.Count > 0)
			{
				dv[0].Delete();
			}
			FlushUpdates();
		}

		private void FlushUpdates()
		{
			tplData.WriteXml(Path.Combine(Util.SafeDir(Settings.TemplatePath), "index.xml"));
		}

		private string EscapeStringForFilter(string str, bool like)
		{
			string result = str.Replace("'", "''");
			if (like)
			{
				if (result.Contains("[") || result.Contains("]"))
					result = result.Replace('[', '\x1B').Replace(']', '\x1D').Replace("\x1B", "[[]").Replace("\x1D", "[]]");

				result = result.Replace("*", "[*]").Replace("%", "[%]");
			}
			return result;
		}
	}
}
