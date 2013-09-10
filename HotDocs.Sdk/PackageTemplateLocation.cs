/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.

using System.IO;

namespace HotDocs.Sdk
{
	public abstract class PackageTemplateLocation : TemplateLocation
	{
		public PackageTemplateLocation(string packageID)
		{
			PackageID = packageID;
		}

		public abstract Stream GetPackageStream();
		public abstract TemplatePackageManifest GetPackageManifest();

		public string PackageID { get; protected set; }
	}
}
