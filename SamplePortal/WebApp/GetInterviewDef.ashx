/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

<%@ WebHandler Language="C#" Class="GetInterviewDef" %>

using System;
using System.Web;
using System.Web.SessionState;

using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Contracts;

public class GetInterviewDef : IHttpHandler, IRequiresSessionState
{

	public void ProcessRequest(HttpContext context)
	{
		WorkSession session = SamplePortal.Factory.GetWorkSession(context.Session);
		HttpResponse resp = context.Response;
		InterviewFormat format;
		string state = context.Request.QueryString["stateString"];
		string templateFile = context.Request.QueryString["template"];
		if (context.Request.QueryString["type"] == "js")
		{
			format = InterviewFormat.JavaScript;
			resp.ContentType = HotDocs.Sdk.Util.GetMimeType(".js");
		}
		else // (context.Request.QueryString["type"] == "dll")
		{
			format = InterviewFormat.Silverlight;
			resp.ContentType = HotDocs.Sdk.Util.GetMimeType(".dll");
			resp.AppendHeader("Content-Disposition", "attachment; filename=" + templateFile + ".dll");
		}

		using (System.IO.Stream stream = session.Service.GetInterviewDefinition(state, templateFile, format))
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