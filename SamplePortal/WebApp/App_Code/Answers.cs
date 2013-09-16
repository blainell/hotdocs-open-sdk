/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Data;
using System.IO;

namespace SamplePortal.Data
{
	/// <summary>
	/// The Answers class provides access to the list of answer files on the server. The answer files
	/// are listed in an XML document, index.xml, with the schema defined in AnswerData.xsd.
	/// </summary>
	public class Answers : IDisposable
	{
		protected AnswerData ansData;

		/// <summary>
		/// Construct a new Answers object.
		/// </summary>
		public Answers()
		{
			ansData = new AnswerData();
			try
			{
				ansData.ReadXml(Path.Combine(Settings.AnswerPath, "index.xml"));
			}
			catch (System.IO.FileNotFoundException)
			{ }
			catch (System.IO.DirectoryNotFoundException)
			{ }
			ansData.CaseSensitive = false;
		}

		#region IDisposable Members

		/// <summary></summary>
		public void Dispose()
		{
			ansData.Dispose();
		}

		#endregion
		
		/// <summary>
		/// Returns a DataView for a specific answer file.
		/// </summary>
		/// <param name="ansfname">The file name for the answer file.</param>
		/// <returns></returns>
		public DataView SelectFile(string ansfname)
		{
			DataView dv = new DataView(ansData.Answers);
			dv.RowFilter = "[Filename] = '" + ansfname + "'";
			return dv;
		}
		/// <summary>
		/// Returns a sorted DataView for a given filter.
		/// </summary>
		/// <param name="sortExpression">The sort expression. See DataView.Sort.</param>
		/// <param name="searchFilter">The filter criterea. See DataView.RowFilter</param>
		/// <returns></returns>
		public DataView SelectAll(string sortExpression, string searchFilter)
		{
			DataView dv = new DataView(ansData.Answers);
			if (sortExpression != null)
				dv.Sort = sortExpression;
			if (searchFilter != null && searchFilter.Length > 0)
				dv.RowFilter = "[Title] LIKE '%" + searchFilter + "%' OR [Description] LIKE '%" + searchFilter + "%'";
			return dv;
		}
		/// <summary>
		/// Add a new answer file to the list.
		/// </summary>
		/// <param name="filename">The file name for the new answer file.</param>
		/// <param name="title">The title for the new answer file.</param>
		/// <param name="desc">The description for the new answer file.</param>
		public void InsertNewAnswerFile(string filename, string title, string desc)
		{
			DataRow newRow = ansData.Answers.NewRow();
			newRow["Filename"] = filename;
			if (title != null)
				newRow["Title"] = title;
			if (desc != null)
				newRow["Description"] = desc;
			newRow["DateCreated"] = DateTime.Now;
			newRow["LastModified"] = DateTime.Now;
			ansData.Answers.Rows.Add(newRow);
			FlushUpdates();
		}
		/// <summary>
		/// Update the title and description for an existing answer file.
		/// </summary>
		/// <param name="filename">The file name for the existing answer file.</param>
		/// <param name="title">The new title.</param>
		/// <param name="desc">The new description.</param>
		public void UpdateAnswerFile(string filename, string title, string desc)
		{
			DataView dv = SelectFile(filename);
			if (title != null) // pass null if you don't want something changed
				dv[0]["Title"] = title;
			if (desc != null)
				dv[0]["Description"] = desc;
			dv[0]["LastModified"] = DateTime.Now;
			FlushUpdates();
		}
		/// <summary>
		/// Remove an answer file from the list.
		/// </summary>
		/// <param name="filename">The file name of the file to remove.</param>
		public void DeleteAnswerFile(string filename)
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
			ansData.WriteXml(Path.Combine(Util.SafeDir(Settings.AnswerPath), "index.xml"));
		}
	}
}
