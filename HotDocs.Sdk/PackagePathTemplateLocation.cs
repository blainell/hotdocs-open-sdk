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
	public class PackagePathTemplateLocation : PackageTemplateLocation, IEquatable<PackagePathTemplateLocation>
	{
		public PackagePathTemplateLocation(string packageID, string packagePath) : base(packageID)
		{
			if (!File.Exists(packagePath))
				throw new Exception("The package does not exist.");

			PackagePath = packagePath;
		}

		public override TemplateLocation Duplicate()
		{
			PackagePathTemplateLocation location = new PackagePathTemplateLocation(PackageID, PackagePath);
			location._templateDir = _templateDir;
			return location;
		}

		public override bool Equals(object obj)
		{
			return (obj != null) && (obj is PackagePathTemplateLocation) && Equals((PackagePathTemplateLocation)obj);
		}

		public override int GetHashCode()
		{
			const int prime = 397;
			int result = PackagePath.ToLower().GetHashCode(); // package path must be case-insensitive
			result = (result * prime) ^ PackageID.GetHashCode(); // combine the hashes
			return result;
		}

		#region IEquatable<PackagePathTemplateLocation> Members

		public bool Equals(PackagePathTemplateLocation other)
		{
			if (other == null)
				return false;
			return string.Equals(PackageID, other.PackageID, StringComparison.Ordinal)
				&& string.Equals(_templateDir, other._templateDir, StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		//TODO: Handle readonly files.
		//TODO: Don't extract the files to disk if they aren't already.
		public override Stream GetFile(string fileName)
		{
			string filePath = Path.Combine(GetTemplateDirectory(), fileName);
			return new FileStream(filePath, FileMode.Open);
		}
		public override string GetTemplateDirectory()
		{
			if (_templateDir != null && Directory.Exists(_templateDir))
				return _templateDir;
			if (!File.Exists(PackagePath))
				throw new Exception("The package does not exist.");

			string folder = Path.Combine(Path.GetDirectoryName(PackagePath), Path.GetFileNameWithoutExtension(PackagePath) + ".dir");

			//If the folder does not exist, create the folder and fill the cache.
			//TODO: What if the folder content is out of date?
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
				TemplatePackage.ExtractAllFiles(PackagePath, "", folder);
			}

			_templateDir = folder;
			return _templateDir;
		}

		public override Stream GetPackageStream()
		{
			return new FileStream(PackagePath, FileMode.Open, FileAccess.Read);
		}

		public override TemplatePackageManifest GetPackageManifest()
		{
			HotDocs.Sdk.TemplatePackage pkg = new HotDocs.Sdk.TemplatePackage();
			pkg.Open(PackagePath);
			return pkg.Manifest;
		}

		protected override string SerializeContent()
		{
			return PackageID + "|" + PackagePath + "|" + GetTemplateDirectory();
		}

		protected override void DeserializeContent(string content)
		{
			string[] tokens = content.Split(new char[] {'|'});
			if (tokens.Length != 3)
				throw new Exception("Invalid template location.");

			PackageID = tokens[0];
			PackagePath = tokens[1];
			_templateDir = tokens[2];
		}

		public string PackagePath { get; protected set; }
		private string _templateDir = null;
	}
}
