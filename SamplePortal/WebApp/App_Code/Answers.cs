/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Consider removing OrigFilename from the schema (AnswerData.xsd).

using System;
using System.Data;
using System.IO;

namespace SamplePortal.Data
{
	/// <summary>
	/// Summary description for Answers.
	/// </summary>
	public class Answers : IDisposable
	{
		protected AnswerData ansData;

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

		public void FlushUpdates()
		{
			ansData.WriteXml(Path.Combine(Util.SafeDir(Settings.AnswerPath), "index.xml"));
		}

		#region IDisposable Members

		public void Dispose()
		{
			ansData.Dispose();
		}

		#endregion
		
		public DataView SelectFile(string ansfname)
		{
			DataView dv = new DataView(ansData.Answers);
			dv.RowFilter = "[Filename] = '" + ansfname + "'";
			return dv;
		}

		//TODO: Not used. Remove?
		public DataView SelectAll(string sortExpression)
		{
			return SelectAll(sortExpression, null);
		}

		public DataView SelectAll(string sortExpression, string searchFilter)
		{
			DataView dv = new DataView(ansData.Answers);
			if (sortExpression != null)
				dv.Sort = sortExpression;
			if (searchFilter != null && searchFilter.Length > 0)
				dv.RowFilter = "[Title] LIKE '%" + searchFilter + "%' OR [Description] LIKE '%" + searchFilter + "%'";
			return dv;
		}

		//TODO: Not used. Remove?
		public bool AnswerFileExists(string filename)
		{
			DataView dv = SelectFile(filename);
			return (dv.Count > 0);
		}

		//TODO: The original file name may not be used (other than being stored here). Consider removing it.
		public void InsertNewAnswerFile(string filename, string title, string desc, string origFilename)
		{
			DataRow newRow = ansData.Answers.NewRow();
			newRow["Filename"] = filename;
			if (title != null)
				newRow["Title"] = title;
			else
			{
				if (origFilename != null)
					newRow["Title"] = origFilename; //if no title is supplied, use the filename
				else
					newRow["Title"] = filename;
			}
			if (desc != null)
				newRow["Description"] = desc;
			if (origFilename != null)
				newRow["OrigFilename"] = origFilename;
			newRow["DateCreated"] = DateTime.Now;
			newRow["LastModified"] = DateTime.Now;
			ansData.Answers.Rows.Add(newRow);
			FlushUpdates();
		}

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

		public void DeleteAnswerFile(string filename)
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
