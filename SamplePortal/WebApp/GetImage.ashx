<%@ WebHandler Language="C#" Class="GetImage" %>

/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Web;
using HotDocs.Sdk;

public class GetImage : IHttpHandler
{
	public void ProcessRequest (HttpContext context)
	{
		var req = context.Request;
		var resp = context.Response;

		//Extract the parameters.
		string imageFileName = req.QueryString["img"];
		if (imageFileName == null)
			throw new System.ArgumentNullException();
		if (imageFileName == String.Empty)
			throw new System.ArgumentException();
		string templateLocator = req.QueryString["loc"];
		if (templateLocator == null)
			throw new System.ArgumentNullException();
		if (templateLocator == String.Empty)
			throw new System.ArgumentException();

		Template template = Template.Locate(templateLocator);
		using (System.IO.Stream stream = template.Location.GetFile(imageFileName))
		{
			resp.ContentType = Util.GetMimeType(imageFileName, true);
			stream.CopyTo(resp.OutputStream);
			resp.Flush();
		}
	}
 
	public bool IsReusable
	{
		get { return true; }
	}
}