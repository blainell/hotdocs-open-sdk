/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace HotDocs.Sdk.ServerTest
{
	static class Util
	{
		//private static string GetLocalPackagePath(string packageId)
		//{
		//	Assembly _asm = Assembly.GetExecutingAssembly();

		//	string templatePath = Path.GetDirectoryName(_asm.Location);
		//	return Path.Combine(templatePath, packageId + ".pkg");
		//}

		public static StreamReader GetTestFile(string fileName)
		{
			Assembly _assembly = Assembly.GetExecutingAssembly();
			return new StreamReader(_assembly.GetManifestResourceStream("HotDocs.Sdk.ServerTest.TestFiles." + fileName));
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

		public static PackagePathTemplateLocation CreatePackagePathLocation(string packageID)
		{
			//Note that in this sample portal, the package ID is used to construct the package file name, but this does not need to be the case.

			Assembly _asm = Assembly.GetExecutingAssembly();
			string templateDirectory = Path.Combine(Path.GetDirectoryName(_asm.Location), "TestFiles");
			string packagePath = Path.Combine(templateDirectory, packageID + ".pkg");

			if (!File.Exists(packagePath))
				throw new Exception("The template does not exist.");
			return new HotDocs.Sdk.PackagePathTemplateLocation(packageID, packagePath);
		}

		public static HotDocs.Sdk.Template OpenTemplate(string packageID)
		{
			return new HotDocs.Sdk.Template(CreatePackagePathLocation(packageID));
		}

		public static HotDocs.Sdk.Server.IServices GetLocalServicesInterface()
		{
			string tempPath = ConfigurationManager.AppSettings["TempPath"];
			return new HotDocs.Sdk.Server.Local.Services(tempPath);
		}

		public static HotDocs.Sdk.Server.IServices GetWebServiceServicesInterface()
		{
			string endPointName = ConfigurationManager.AppSettings["WebServiceEndPoint"];

			Assembly _asm = Assembly.GetExecutingAssembly();
			string templatePath = Path.Combine(GetSamplePortalTemplateFolder(), "TestFiles");

			return new HotDocs.Sdk.Server.WebService.Services(endPointName, templatePath);
		}

		public static HotDocs.Sdk.Server.IServices GetCloudServicesInterface()
		{
			string cloudSigningKey = ConfigurationManager.AppSettings["SigningKey"];
			string cloudSubscriberID = ConfigurationManager.AppSettings["SubscriberID"];
			return new HotDocs.Sdk.Server.Cloud.Services(cloudSubscriberID, cloudSigningKey);
		}

		#region Private methods
		private static string GetSamplePortalTemplateFolder()
		{
			return GetRootedPath("Templates");
		}

		private static string GetRootedPath(string path)
		{
			if (!Path.IsPathRooted(path))
			{
				string siteRoot = System.Web.Hosting.HostingEnvironment.MapPath("~");
				string siteRootParent = Directory.GetParent(siteRoot).FullName;
				path = Path.Combine(siteRootParent, "Files", path);
			}
			return path;
		}

		#endregion // Private methods
	}
}
