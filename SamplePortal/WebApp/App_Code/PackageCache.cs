/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Complete XML comments.

using System.IO;

namespace SamplePortal
{
	/// <summary>
	/// Summary description for PackageCache
	/// </summary>
	public class PackageCache
	{
		public static string GetLocalPackagePath(string packageId)
		{
			return Path.Combine(Settings.TemplatePath, packageId + ".pkg");
		}

		public static bool PackageExists(string packageId)
		{
			string packagePath = GetLocalPackagePath(packageId);
			return File.Exists(Path.Combine(Settings.TemplatePath, packagePath));
		}
	}
}
