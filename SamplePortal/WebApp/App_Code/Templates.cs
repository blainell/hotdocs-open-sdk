/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Data;
using System.IO;
using System.Collections.Generic;

namespace SamplePortal.Data
{
	/// <summary>
	/// Summary description for Templates.
	/// </summary>
	public class Templates : IDisposable
	{
		protected TemplateData tplData;

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

		public void FlushUpdates()
		{
			tplData.WriteXml(Path.Combine(Util.SafeDir(Settings.TemplatePath), "index.xml"));
		}

		#region IDisposable Members

		public void Dispose()
		{
			tplData.Dispose();
		}

		#endregion

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

		public DataView SelectFile(string tplname)
		{
			DataView dv = new DataView(tplData.Templates);
			dv.RowFilter = "[Filename] = '" + EscapeStringForFilter(tplname, false) + "'";
			return dv;
		}

		public DataView SelectFileLike(string tplname)
		{
			DataView dv = new DataView(tplData.Templates);
			dv.RowFilter = "[Filename] LIKE '" + EscapeStringForFilter(tplname, true) + "*'";
			return dv;
		}

		public DataView SelectAll(string sortExpression)
		{
			return SelectAll(sortExpression, null);
		}

		public DataView SelectAll(string sortExpression, string searchFilter)
		{
			DataView dv = new DataView(tplData.Templates);
			if (sortExpression != null)
				dv.Sort = sortExpression;
			if (searchFilter != null && searchFilter.Length > 0)
				dv.RowFilter = "[Title] LIKE '%" + searchFilter + "%' OR [Description] LIKE '%" + searchFilter + "%'";
			return dv;
		}

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

		public bool HasMetadataValue(string name)
		{
			DataRowView drv = LoadMetaDataRow(name);
			return drv != null;
		}

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

		public string GetVersion()
		{
			return GetMetadataValue("Version");
		}

		public void SetVersion(string version)
		{
			SetMetadataValue("Version", version);
		}

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

	}

}
