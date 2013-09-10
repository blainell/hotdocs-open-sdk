<%@ WebHandler Language="C#" Class="GetImage" %>

/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk;
using System;
using System.IO;
using System.Web;

public class GetImage : IHttpHandler
{
	public void ProcessRequest(HttpContext context)
	{
		var req = context.Request;
		var resp = context.Response;

		// Get the name of the image from the query string.
		string imageFileName = req.QueryString["img"];
		if (imageFileName == null)
			throw new ArgumentNullException();
		if (imageFileName == String.Empty)
			throw new ArgumentException();

		// Get the location of the template associated with the image from the query string.
		string templateLocator = req.QueryString["loc"];
		if (templateLocator == null)
			throw new ArgumentNullException();
		if (templateLocator == String.Empty)
			throw new ArgumentException();

		// Locate the template (using the "loc" value from the query string) and stream the file to the output stream.
		Template template = Template.Locate(templateLocator);
		using (Stream stream = template.Location.GetFile(imageFileName))
		{
			resp.ContentType = Util.GetMimeType(imageFileName, true); // Only check for supported image mime types.
			stream.CopyTo(resp.OutputStream);
			resp.Flush();
		}
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