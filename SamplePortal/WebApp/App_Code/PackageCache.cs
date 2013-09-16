/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.IO;

namespace SamplePortal
{
	/// <summary>
	/// <c>PackageCache</c> manages package files on disk.
	/// </summary>
	public class PackageCache
	{
		/// <summary>
		/// Returns a package path based on a package ID.
		/// </summary>
		/// <param name="packageId"></param>
		/// <returns></returns>
		public static string GetLocalPackagePath(string packageId)
		{
			return Path.Combine(Settings.TemplatePath, packageId + ".pkg");
		}

		/// <summary>
		/// Returns non-zero if the package file for the given package ID exists.
		/// </summary>
		/// <param name="packageId">The package ID of the package to check for.</param>
		/// <returns></returns>
		public static bool PackageExists(string packageId)
		{
			string packagePath = GetLocalPackagePath(packageId);
			return File.Exists(Path.Combine(Settings.TemplatePath, packagePath));
		}
	}
}
