/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

////////////////////////////////////////////////////////////////////////////////////////////////////
// Class: UploadPageHandler
////////////////////////////////////////////////////////////////////////////////////////////////////
internal static class UploadPageHandler
{
	public static int GetMasterFileList(HttpRequest request, out string[] fileInfoList)
	{
		List<string> masterFilenames = new List<string>();
		string[] tary = request.Form["templates"].Split(new char[] { '|' });
		foreach (string tpl in tary)
			masterFilenames.Add(tpl);

		fileInfoList = masterFilenames.ToArray();
		return masterFilenames.Count;
	}
	public static int GetUploadCollisions(HttpRequest request, string destFolder, out string[] collisions)
	{
		List<string> collisionList = new List<string>();
		HttpFileCollection files = request.Files;
		foreach (string fileId in files)
		{
			HttpPostedFile file = files[fileId];
			if (file.ContentLength > 0 && File.Exists(Path.Combine(destFolder, Path.GetFileName(file.FileName))))
				collisionList.Add(file.FileName);
		}

		collisions = collisionList.ToArray();
		return collisionList.Count;
	}
	public static void UploadFiles(HttpRequest request, string destFolder)
	{
		foreach (string fileId in request.Files)
		{
			HttpPostedFile file = request.Files[fileId];
			int nFileLen = file.ContentLength; // size of uploaded file
			string filename = Path.GetFileName(file.FileName);
			if (nFileLen > 0) // only bother with non-empty files
			{
				byte[] fileData = new byte[nFileLen]; // allocate a buffer for the incoming file
				file.InputStream.Read(fileData, 0, nFileLen); // read uploaded file into the buffer

				//Write the file to disk.
				FileStream filestream = new FileStream(Path.Combine(destFolder, filename), FileMode.Create, FileAccess.Write);
				filestream.Write(fileData, 0, fileData.Length);
				filestream.Close();
			}
		}
	}
}
