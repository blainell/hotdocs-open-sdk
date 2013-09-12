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

	/// <summary>
	/// <c>PackagePathTemplateLocation</c> is a <c>PackageTemplateLocation</c> that expects a package
	/// to exist on disk. Furthermore, the package content is extracted to a subfolder of the package folder.
	/// The subfolder's name consists of the package ID followed by a ".dir" extension. To extract package
	/// content elsewhere, derive a different PackageTemplateLocation class.
	/// </summary>
	public class PackagePathTemplateLocation : PackageTemplateLocation, IEquatable<PackagePathTemplateLocation>

	{
		/// <summary>
		/// Construct a template location for a specific package in the file system.
		/// </summary>
		/// <param name="packageID">The ID of the package.</param>
		/// <param name="packagePath">The full path of the package file.</param>
		public PackagePathTemplateLocation(string packageID, string packagePath) : base(packageID)
		{
			if (!File.Exists(packagePath))
				throw new Exception("The package does not exist.");

			_templateDir = Path.Combine(Path.GetDirectoryName(packagePath), Path.GetFileNameWithoutExtension(packagePath) + ".dir");
			PackagePath = packagePath;
		}

		/// <summary>
		/// Returns a new duplicate instance of this object. Overrides <see cref="TemplateLocation.Duplicate"/>.
		/// </summary>
		/// <returns></returns>
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

		//TODO: Don't extract the files to disk if they aren't already.
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public override Stream GetFile(string fileName)
		{
			string filePath = Path.Combine(GetTemplateDirectory(), fileName);
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}
		/// <summary>
		/// Returns the folder that the package content was extracted to. Overrides <see cref="TemplateLocation.GetTemplateDirectory"/>.
		/// </summary>
		/// <returns></returns>
		public override string GetTemplateDirectory()
		{
			return _templateDir;
		}
		/// <summary>
		/// Returns a read-only stream for the package file.
		/// </summary>
		/// <returns></returns>
		public override Stream GetPackageStream()
		{
			return new FileStream(PackagePath, FileMode.Open, FileAccess.Read);
		}
		/// <summary>
		/// Returns a <see cref="TemplatePackageManifest"/> for the package.
		/// </summary>
		/// <returns></returns>
		public override TemplatePackageManifest GetPackageManifest()
		{
			HotDocs.Sdk.TemplatePackage pkg = new HotDocs.Sdk.TemplatePackage();
			pkg.Open(PackagePath);
			return pkg.Manifest;
		}
		/// <summary>
		/// Overrides <see cref="TemplateLocation.SerializeContent"/>.
		/// </summary>
		/// <returns></returns>
		protected override string SerializeContent()
		{
			return PackageID + "|" + PackagePath + "|" + GetTemplateDirectory();
		}
		/// <summary>
		/// Overrides <see cref="TemplateLocation.DeserializeContent"/>.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		protected override void DeserializeContent(string content)
		{
			string[] tokens = content.Split(new char[] {'|'});
			if (tokens.Length != 3)
				throw new Exception("Invalid template location.");

			PackageID = tokens[0];
			PackagePath = tokens[1];
			_templateDir = tokens[2];
		}
		/// <summary>
		/// Extract the create the package content folder and extract the package content to it.
		/// </summary>
		/// <param name="force">If true, the directory is overwritten. Otherwise, the directory
		/// is created and files extracted only if the directory does not exist. If force is true
		/// all previously existing directory content is destroyed.</param>
		public virtual void ExtractPackageFiles(bool force=false)
		{
			if (force)
				CleanPackageFiles();
			if (!Directory.Exists(_templateDir))
			{
				Directory.CreateDirectory(_templateDir);
				TemplatePackage.ExtractAllFiles(PackagePath, "", _templateDir);
			}
		}
		/// <summary>
		/// Destroy the package template folder and all of its contents.
		/// </summary>
		public virtual void CleanPackageFiles()
		{
			if (Directory.Exists(_templateDir))
				Directory.Delete(_templateDir, true);
		}

		/// <summary>
		/// 
		/// </summary>
		public string PackagePath { get; protected set; }
		private string _templateDir = null;
	}
}
