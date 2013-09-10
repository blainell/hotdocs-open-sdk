/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments.

using System;
using System.IO;

namespace SamplePortal
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Class: Factory
	//
	//	Description: This is the factory class for producing adapter objects.
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class Factory
	{
		public static HotDocs.Sdk.Server.WorkSession GetWorkSession(System.Web.SessionState.HttpSessionState session)
		{
			return (HotDocs.Sdk.Server.WorkSession)session["HdSession"];
		}

		public static HotDocs.Sdk.Server.WorkSession CreateWorkSession(System.Web.SessionState.HttpSessionState session, string packageID)
		{
			HotDocs.Sdk.Template template = OpenTemplate(packageID);
			HotDocs.Sdk.Server.IServices service = GetServices();
			HotDocs.Sdk.Server.WorkSession workSession = new HotDocs.Sdk.Server.WorkSession(service, template);
			session["HdSession"] = workSession;
			return workSession;
		}

		//TODO: This needs to be called but is not called.
		public static void RetireWorkSession(System.Web.SessionState.HttpSessionState session)
		{
			HotDocs.Sdk.Server.WorkSession workSession = (HotDocs.Sdk.Server.WorkSession)session["HdSession"];
			if (workSession != null)
			{
				session["HdSession"] = null;
			}
		}

		static public HotDocs.Sdk.Server.HdProtocol HdsRoute
		{
			get
			{
				string s = System.Configuration.ConfigurationManager.AppSettings["HdsRoute"];
				if (s.Equals("WS", StringComparison.OrdinalIgnoreCase))
					return HotDocs.Sdk.Server.HdProtocol.WebService;
				if (s.Equals("CLOUD", StringComparison.OrdinalIgnoreCase))
					return HotDocs.Sdk.Server.HdProtocol.Cloud;
				if (s.Equals("LOCAL", StringComparison.OrdinalIgnoreCase))
					return HotDocs.Sdk.Server.HdProtocol.Local;

				//Default
				return HotDocs.Sdk.Server.HdProtocol.Local;
			}
		}

		static public HotDocs.Sdk.Server.IServices GetServices()
		{
			switch (HdsRoute)
			{
				case HotDocs.Sdk.Server.HdProtocol.Cloud:
					return new HotDocs.Sdk.Server.Cloud.Services(Settings.SubscriberID, Settings.SigningKey);
				case HotDocs.Sdk.Server.HdProtocol.Local:
					return new HotDocs.Sdk.Server.Local.Services(Settings.TempPath);
				case HotDocs.Sdk.Server.HdProtocol.WebService:
					return new HotDocs.Sdk.Server.WebService.Services(Settings.WebServiceEndPoint, Settings.TemplatePath);
			}
			throw new Exception("Unsupported protocol.");
		}

		private static HotDocs.Sdk.Template OpenTemplate(string packageId)
		{
			//Note that in this sample portal, the package ID is used to construct the package file name, but this does not need to be the case.
			string packagePath = PackageCache.GetLocalPackagePath(packageId);
			if (!File.Exists(packagePath))
				throw new Exception("The template does not exist.");
			HotDocs.Sdk.PackagePathTemplateLocation location = new HotDocs.Sdk.PackagePathTemplateLocation(packageId, packagePath);

			return new HotDocs.Sdk.Template(location);
		}
	}
}
