/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

<%@ WebHandler Language="C#" Class="GetInterviewFile" %>

using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using HotDocs.Sdk.Server;

public class GetInterviewFile : IHttpHandler, IRequiresSessionState
{

	/// <summary>
	/// Returns a file required by a HotDocs interview running in the browser.
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		WorkSession session = SamplePortal.Factory.GetWorkSession(context.Session);
		HttpRequest req = context.Request;
		HttpResponse resp = context.Response;

		// Cache the file for 60 minutes to improve performance.
		resp.Cache.SetCacheability(HttpCacheability.Public);
		resp.Cache.SetExpires(DateTime.Now.AddMinutes(60));
		resp.Cache.SetMaxAge(new TimeSpan(0, 60, 0));
		resp.AddHeader("Last-Modified", DateTime.Now.ToLongDateString());

		// Read and validate the required parameters from the query string.
		string templateLocator = req.QueryString["loc"];
		if (string.IsNullOrEmpty(templateLocator))
			throw new ArgumentNullException("templateLocator");

		string fileName = req.QueryString["template"]; // file name of template or image
		if (string.IsNullOrEmpty(fileName))
			throw new ArgumentNullException("fileName");

		string fileType = req.QueryString["type"];
		if (string.IsNullOrEmpty(fileType))
			throw new ArgumentNullException("fileType");
		else
			fileType = fileType.ToLower();

		// Set the MIME type for the requested file.
		string outputFileName = fileType == "img" ? fileName : fileName + "." + fileType;
		resp.ContentType = HotDocs.Sdk.Util.GetMimeType(outputFileName, fileType == "img");

		// Add an appropriate header if we are returning a Silverlight DLL.
		if (fileType == "dll")
			resp.AppendHeader("Content-Disposition", "attachment; filename=" + outputFileName);
		
		// Get the file and copy it to the output stream.
		using (Stream stream = session.Service.GetInterviewFile(HotDocs.Sdk.Template.Locate(templateLocator), fileName, fileType))
		{
			stream.CopyTo(resp.OutputStream);
		}
		resp.End();
	}

	/// <summary>
	/// Indicates whether another request can reuse this IHttpHandler instance.
	/// </summary>
	public bool IsReusable
	{
		// This file handler has no state, so the handler can be reused as needed.
		get { return true; }
	}
}