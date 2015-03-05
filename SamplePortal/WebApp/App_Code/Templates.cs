/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */


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
			dv.RowFilter = "[Filename] = '" + EncodeString(tplname) + "'";
			return dv;
		}

		/// <summary>
		/// Returns a DataView for a specific template.
		/// </summary>
		/// <param name="tplname">The template name.</param>
		/// <returns></returns>
		public DataView SelectTitle(string title, string extension)
		{
			DataView dv = new DataView(tplData.Templates);
			dv.RowFilter = "[Title] = '" + EncodeString(title) + "' and [Filename] like '%" + extension + "'";
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
			searchFilter = EncodeString(searchFilter);
			if (!string.IsNullOrEmpty(searchFilter))
				dv.RowFilter = "[Title] LIKE '%" + searchFilter + "%' OR [Description] LIKE '%" + searchFilter + "%'";
			return dv;
		}

		/// <summary>
		/// Returns true if the filename exists in 'index.xml'
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		public bool TemplateExists(string filename, string title)
		{
			DataView dv = dv = SelectFile(filename);
			return (dv.Count > 0);
		}

		/// <summary>
		/// Inserts a new template using the parameter in 'index.xml'
		/// </summary>
		/// <param name="ui"></param>
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

		/// <summary>
		/// Updates 'index.xml' with the new title, description, and package ID from the "ui" parameter.
		/// </summary>
		/// <param name="ui"></param>
		public void UpdateTemplate(UploadItem ui)
		{
			UpdateTemplate(ui.MainTemplateFileName, ui.Title, ui.Description, ui.PackageID);
		}

		/// <summary>
		/// Updates 'index.xml' the new title, description, and packageID passed in.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="title"></param>
		/// <param name="desc"></param>
		/// <param name="packageID"></param>
		public void UpdateTemplate(string filename, string title, string desc, string packageID)
		{
			DataView dv = null;
			FileInfo finfo = new FileInfo(filename);
			switch (finfo.Extension.ToLower())
			{
				case ".url":
					dv = SelectTitle(title, finfo.Extension);
					break;
				default:
					dv = SelectFile(filename);
					break;
			}
			string curPkgID = dv[0]["PackageID"].ToString();

			if (packageID != null)
			{
				if (curPkgID != packageID)
				{
					// If the packageID is different than the old one, delete the old package from disk.
					PackageCache.DeleteTemplatePackage(curPkgID);
				}

				dv[0]["PackageID"] = packageID;
			}
			// pass null if you don't want something changed
			if (filename != null)
				dv[0]["Filename"] = filename;
			if (title != null) 
				dv[0]["Title"] = title;
			if (desc != null)
				dv[0]["Description"] = desc;
			FlushUpdates();
		}

		/// <summary>
		/// Deletes the "filename" from index.xml.
		/// </summary>
		/// <param name="filename"></param>
		public void DeleteTemplate(string filename)
		{
			DataView dv = SelectFile(filename);
			if (dv.Count > 0)
			{
				dv[0].Delete();
			}
			FlushUpdates();
		}

		/// <summary>
		/// Flushes updates.
		/// </summary>
		private void FlushUpdates()
		{
			tplData.WriteXml(Path.Combine(Util.SafeDir(Settings.TemplatePath), "index.xml"));
		}

		private string EncodeString(string s)
		{
			if (string.IsNullOrEmpty(s))
				return string.Empty;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				switch (c)
				{
					case '*':
					case '%':
					case '[':
					case ']':
						sb.Append("[").Append(c).Append("]");
						break;
					case '\'':
						sb.Append("''");
						break;
					case '<':
						sb.Append("&lt;");
						break;
					case '>':
						sb.Append("&gt;");
						break;
					default:
						sb.Append(c);
						break;
				}
			}

			return sb.ToString();
		}
	}
} 

