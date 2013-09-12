/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.

using System;
using System.IO;

namespace HotDocs.Sdk
{
	public abstract class PackageTemplateLocation : TemplateLocation
	{
		public PackageTemplateLocation(string packageID)
		{
			if (packageID == null)
				throw new ArgumentNullException();

			if (packageID == String.Empty)
				throw new ArgumentException();

			PackageID = packageID;
		}

		public abstract Stream GetPackageStream();
		public abstract TemplatePackageManifest GetPackageManifest();

		public string PackageID { get; protected set; }
	}
}
