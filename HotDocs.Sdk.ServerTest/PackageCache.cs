/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;

namespace HotDocs.Sdk.ServerTest
{
	/// <summary>
	/// Summary description for PackageCache
	/// </summary>
	public class PackageCache
	{
		public static string GetLocalPackagePath(string packageId)
		{
			return Path.Combine(TemplatePath, packageId + ".pkg");
		}

		public static bool PackageExists(string packageId)
		{
			string packagePath = GetLocalPackagePath(packageId);
			string templatePath = System.Configuration.ConfigurationManager.AppSettings["TemplatePath"];
			return File.Exists(Path.Combine(templatePath, packagePath));
		}

		private static string TemplatePath
		{
			get
			{
				return System.Configuration.ConfigurationManager.AppSettings["TemplatePath"];
			}
		}
	}
}
