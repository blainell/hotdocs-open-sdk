/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

<%@ WebHandler Language="C#" Class="GetInterviewDef" %>

using System;
using System.Web;
using System.Web.SessionState;

using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Contracts;

public class GetInterviewDef : IHttpHandler, IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        HotDocs.Sdk.Server.WorkSession session = SamplePortal.Factory.GetWorkSession(context.Session);
        HttpResponse resp = context.Response;
        InterviewFormat format;
        string state = context.Request.QueryString["stateString"];
        string templateFile = context.Request.QueryString["template"];
        if (context.Request.QueryString["type"] == "js")
        {
            //curAsm.InterviewFormat = HotDocs.Sdk.Server.InterviewFormat.JavaScript;
            format = InterviewFormat.JavaScript;
            resp.ContentType = "application/javascript";
        }
        else // (context.Request.QueryString["type"] == "dll")
        {
            //curAsm.InterviewFormat = HotDocs.Sdk.Server.InterviewFormat.Silverlight;
            format = InterviewFormat.Silverlight;
            resp.ContentType = "application/octet-stream";
            resp.AppendHeader("Content-Disposition", "attachment; filename=" + templateFile + ".dll");
        }

        System.IO.Stream stream = session.Service.GetInterviewDefinition(state, templateFile, format);
        byte[] intvwDef = new byte[stream.Length];
        stream.Read(intvwDef, 0, (int)stream.Length);
		stream.Close();

        resp.BinaryWrite(intvwDef);
        resp.End();
    }

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}