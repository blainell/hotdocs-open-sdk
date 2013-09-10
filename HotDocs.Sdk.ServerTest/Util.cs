/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace HotDocs.Sdk.ServerTest
{
	static class Util
	{
		private static string GetLocalPackagePath(string packageId)
		{
			string templatePath = ConfigurationManager.AppSettings["TemplatePath"];
			return Path.Combine(templatePath, packageId + ".pkg");
		}

		public static string TestFilesPath
		{
			get
			{
				return ConfigurationManager.AppSettings["TestFilesPath"];
			}
		}

		public static string GetFileContentAsString(string filePath)
		{
			string content = "";
			using (FileStream fs = File.OpenRead(filePath))
			{
				TextReader rdr = new StreamReader(fs);
				content = rdr.ReadToEnd();
			}
			return content;
		}

		public static HotDocs.Sdk.Template OpenTemplate(string packageId)
		{
			//Note that in this sample portal, the package ID is used to construct the package file name, but this does not need to be the case.
			string packagePath = PackageCache.GetLocalPackagePath(packageId);
			if (!File.Exists(packagePath))
				throw new Exception("The template does not exist.");
			HotDocs.Sdk.PackagePathTemplateLocation location = new HotDocs.Sdk.PackagePathTemplateLocation(packageId, packagePath);

			return new HotDocs.Sdk.Template(location);
		}

		public static HotDocs.Sdk.Server.IServices GetLocalServicesInterface()
		{
			string tempPath = ConfigurationManager.AppSettings["TempPath"];
			return new HotDocs.Sdk.Server.Local.Services(tempPath);
		}

		public static HotDocs.Sdk.Server.IServices GetWebServiceServicesInterface()
		{
			string endPointName = ConfigurationManager.AppSettings["WebServiceEndPoint"];
			string templatePath = ConfigurationManager.AppSettings["TemplatePath"];
			return new HotDocs.Sdk.Server.WebService.Services(endPointName, templatePath);
		}

		public static HotDocs.Sdk.Server.IServices GetCloudServicesInterface()
		{
			string cloudSigningKey = ConfigurationManager.AppSettings["SigningKey"];
			string cloudSubscriberID = ConfigurationManager.AppSettings["SubscriberID"];
			return new HotDocs.Sdk.Server.Cloud.Services(cloudSubscriberID, cloudSigningKey);
		}
	}
}
