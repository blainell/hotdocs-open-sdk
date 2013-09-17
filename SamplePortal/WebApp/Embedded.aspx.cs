/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk.Cloud;
using HotDocs.Sdk.Server.Contracts;
using SamplePortal;
using System;
using System.IO;

/// <summary>
/// This page is an example of using an "embedded" interview from HotDocs Cloud Services. 
/// </summary>
public partial class Embedded : System.Web.UI.Page
{
	protected string _packageID = "d1f7cade-cb74-4457-a9a0-27d94f5c2d5b";

	protected void Page_Load(object sender, EventArgs e)
	{
		Header1.SiteName = "HotDocs Embedded Example";
	}

	protected string GetSessionID()
	{
		InterviewFormat format = HotDocs.Sdk.Util.ReadConfigurationEnum<InterviewFormat>("InterviewFormat", InterviewFormat.Unspecified);
		var client = new RestClient(Settings.SubscriberID, Settings.SigningKey);
		HotDocs.Sdk.Template template = new HotDocs.Sdk.Template(new HotDocs.Sdk.PackagePathTemplateLocation(_packageID, Path.Combine(Settings.TemplatePath, _packageID +  ".pkg")));
		return client.CreateSession(template, null, null, null, format);
	}
}
