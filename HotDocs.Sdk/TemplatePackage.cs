/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging; // Needs reference to WindowsBase
using System.Security.Cryptography;
using System.Xml;

/*
A simple class to zip and extract files using the System.IO.Packaging.Package class.

Some helpful links:
http://blogs.msdn.com/b/opc/archive/2009/05/18/adventures-in-packaging-episode-1.aspx
http://stackoverflow.com/questions/1338754/how-to-write-my-own-zip-class-in-net
http://dotnetzip.codeplex.com/
http://www.icsharpcode.net/opensource/sharpziplib/
http://forums.asp.net/t/1468145.aspx/1
http://msdn.microsoft.com/en-us/library/system.io.packaging%28VS.85%29.aspx
http://office.microsoft.com/en-us/word-help/office-open-xml-i-exploring-the-office-open-xml-formats-RZ010243529.aspx?section=16
http://msdn.microsoft.com/en-us/library/ms569886(v=VS.85).aspx
http://galratner.com/blogs/net/archive/2011/01/24/using-system-io-packaging-to-zip-your-files.aspx
http://blogs.msdn.com/b/ericwhite/archive/2007/12/11/packages-and-parts.aspx
http://techmikael.blogspot.com/2010/11/creating-zip-files-with.html
http://madprops.org/blog/Zip-Your-Streams-with-System-IO-Packaging/
*/

namespace HotDocs.Sdk
{
	/// <summary>
	/// A class for HotDocs template packages. A template package is a zip file containing a template and all it's dependent files. Optionally, a package can also be encrypted.
	/// </summary>
	public class TemplatePackage
	{
		/// <summary>
		/// The internal name of the manifest file. It will be automatically created if the Manifest property has been set upon saving the package. When opening a package, this manifest will be used to initialize the Manifest property.
		/// </summary>
		public const string ManifestName = "manifest.xml";

		public bool CheckForGeneratedFiles { get; set; }

		private static readonly byte[] ZipSig = new byte[] { 0x50, 0x4b, 0x03, 0x04 }; // The first 4 bytes of a .zip file
		private static readonly byte[] HDSig  = new byte[] { 0x48, 0x44, 0xae, 0x1a }; // The first 4 bytes of an encrypted HotDocs package file

		private MemoryStream _stream;
		private Package _package;

	    /// <summary>
		/// Create a new (empty) package.
		/// </summary>
		public TemplatePackage()
		{
			CheckForGeneratedFiles = true;
		}

		private static void CopyStream(Stream to, Stream from)
		{
			int BufSize = 16*1024;
			byte[] buf = new byte[BufSize];
			int n;
			//
			for (; ; )
			{
				n = from.Read(buf, 0, BufSize);
				if (n <= 0)
					break;
				to.Write(buf, 0, n);
			}
		}

		private void CondCreateStream()
		{
			if (_stream == null)
			{
				_stream = new MemoryStream();
				_package = Package.Open(_stream, FileMode.Create, FileAccess.ReadWrite);
			}
		}

		/// <summary>
		/// Returns true if the package is new, i.e. Open hasn't been called.
		/// </summary>
		public bool IsNew
		{
			get
			{
				return string.IsNullOrEmpty(CurrentPath);
			}
		}

		private static bool EqualBytes(byte[] buf1, byte[] buf2, int len)
		{
			for (int i = 0; i < len; i++)
			{
				if (buf1[i] != buf2[i])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Create a new RSA public/private key pair serialized as XML.
		/// </summary>
		/// <returns>An XML string representing a new RSA public/private key pair defined in System.Security.Cryptography.RSAParameters.</returns>
		public static string CreateKey()
		{
			string key;
			//
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
			{
				key = rsa.ToXmlString(true);
			}
			return key;
		}

		/// <summary>
		/// Check if a string represents a valid RSA key serialized as XML.
		/// </summary>
		/// <param name="rsaParamsXml">RSA key serialized to XML. This can be a public/private key pair or a public key only.</param>
		/// <returns>True if the key is valid.</returns>
		public static bool IsValidKey(string rsaParamsXml)
		{
			bool res;
			//
			try
			{
				using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
				{
					rsa.FromXmlString(rsaParamsXml);
				}
				res = true;
			}
			catch (Exception)
			{
				res = false;
			}
			return res;
		}

		/// <summary>
		/// Get the public RSA key serialized as XML from a RSA public/private key pair serialized as XML.
		/// </summary>
		/// <param name="rsaParamsXml">RSA key serialized to XML. This can be a public/private key pair or a public key only.</param>
		/// <returns>An XML string representing the RSA public key part defined in System.Security.Cryptography.RSAParameters.</returns>
		public static string GetPublicKeyOf(string rsaParamsXml)
		{
			string key;
			//
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
			{
				rsa.FromXmlString(rsaParamsXml);
				key = rsa.ToXmlString(false);
			}
			return key;
		}

		/// <summary>
		/// Determine if the RSA key is only a public key.
		/// </summary>
		/// <param name="rsaParamsXml">RSA key serialized to XML. This can be a public/private key pair or a public key only.</param>
		/// <returns>True if the RSA key is only a public key, not a public/private key pair.</returns>
		public static bool IsPublicOnlyKey(string rsaParamsXml)
		{
			bool res;
			//
			using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
			{
				rsa.FromXmlString(rsaParamsXml);
				res = rsa.PublicOnly;
			}
			return res;
		}

		/// <summary>
		/// A helper function to convert an unencrypted HotDocs package stream to an encrypted HotDocs package stream.
		/// </summary>
		/// <param name="ms">The unencrypted HotDocs package file/stream.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML.</param>
		/// <returns>If rsaParamsXml is null or empty, the input stream ms is simply returned. Otherwise, the input stream ms is being copied and encrypted to the returned output stream. The output stream will contain a 4 byte signature at the beginning identifying it as a HotDocs package followed by encrypted AES keys and the encrypted data.</returns>
		public static MemoryStream Encrypt(MemoryStream ms, string rsaParamsXml)
		{
			byte[] encryptedAesKey;
			byte[] decryptedAesKey;
			byte[] encryptedAesIV;
			byte[] decryptedAesIV;
			MemoryStream s;
			//
			if (string.IsNullOrEmpty(rsaParamsXml))
				return ms;
			s = new MemoryStream();
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				aes.GenerateKey(); decryptedAesKey = aes.Key;
				aes.GenerateIV(); decryptedAesIV = aes.IV;
				using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
				{
					rsa.FromXmlString(rsaParamsXml);
					encryptedAesKey = rsa.Encrypt(decryptedAesKey, false);
					encryptedAesIV = rsa.Encrypt(decryptedAesIV, false);
				}
				s.Write(HDSig, 0, 4);
				s.WriteByte(0); // Version number
				s.WriteByte((byte)encryptedAesKey.Length);
				s.Write(encryptedAesKey, 0, encryptedAesKey.Length);
				s.WriteByte((byte)encryptedAesIV.Length);
				s.Write(encryptedAesIV, 0, encryptedAesIV.Length);
				MemoryStream temps = new MemoryStream();
				using (CryptoStream cs = new CryptoStream(temps, aes.CreateEncryptor(), CryptoStreamMode.Write))
				{
					ms.Position = 0;
					CopyStream(cs, ms);
					cs.FlushFinalBlock();
					cs.Flush();
					temps.Position = 0;
					CopyStream(s, temps);
				}
			}
			s.Position = 0;
			return s;
		}

		/// <summary>
		/// A helper function to convert an encrypted HotDocs package stream to an unencrypted HotDocs package stream.
		/// </summary>
		/// <param name="ms">The encrypted HotDocs package file/stream.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML.</param>
		/// <returns>If rsaParamsXml is null or empty or if the input stream ms is not an encrypted HotDocs package, the input stream ms is simply returned. Otherwise, the input stream ms is being copied and decrypted to the returned output stream.</returns>
		public static MemoryStream Decrypt(MemoryStream ms, string rsaParamsXml)
		{
			byte[] buf = new byte[4];
			byte[] encryptedAesKey;
			byte[] decryptedAesKey;
			byte[] encryptedAesIV;
			byte[] decryptedAesIV;
			int len;
			MemoryStream s;
			//
			if (ms.Length < 4)
				return ms;
			ms.Position = 0;
			ms.Read(buf, 0, 4);
			if (EqualBytes(buf, ZipSig, 4))
				return ms; // Unencrypted zip file
			if (!EqualBytes(buf, HDSig, 4))
				throw new Exception("Encrypted package has an unknown file signature");
			if (string.IsNullOrEmpty(rsaParamsXml))
				throw new Exception("No key provided for encrypted package");
			len = ms.ReadByte();
			if (len>0)
				throw new Exception("This package was encrypted with a later version of the program");

			len = ms.ReadByte();
			if (len<=0 || len>255)
				throw new Exception("Unknown key length detected");
			encryptedAesKey = new byte[len];
			ms.Read(encryptedAesKey, 0, len);

			len = ms.ReadByte();
			if (len<=0 || len>255)
				throw new Exception("Unknown iv length detected");
			encryptedAesIV = new byte[len];
			ms.Read(encryptedAesIV, 0, len);

			s = new MemoryStream();
			using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
			{
				using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
				{
					rsa.FromXmlString(rsaParamsXml);
					decryptedAesKey = rsa.Decrypt(encryptedAesKey, false);
					decryptedAesIV = rsa.Decrypt(encryptedAesIV, false);
				}
				aes.Key = decryptedAesKey;
				aes.IV = decryptedAesIV;
				MemoryStream temps = new MemoryStream();
				using (CryptoStream cs = new CryptoStream(temps, aes.CreateDecryptor(), CryptoStreamMode.Write))
				{
					CopyStream(cs, ms);
					cs.FlushFinalBlock();
					cs.Flush();
					temps.Position = 0;
					CopyStream(s, temps);
				}
			}
			return s;
		}

		private void LoadManifest()
		{
			Uri partUri = InternalNameToPartUri(ManifestName);
			PackagePart packagePart;
			//
			Manifest = null;
			if (_package.PartExists(partUri))
			{
				packagePart = _package.GetPart(partUri);
				using (Stream ps = packagePart.GetStream(FileMode.Open, FileAccess.Read))
				{
					Manifest = TemplatePackageManifest.FromStream(ps);
				}
			}
			else
			{
				throw new Exception("Package has no manifest");
			}
		}

		private void SaveManifest()
		{
			Uri partUri = InternalNameToPartUri(ManifestName);
			PackagePart packagePart;
			string errMsg;

			if (!IsValidManifest(out errMsg))
				throw new Exception("Invalid manifest: " + errMsg);
			if (_package.PartExists(partUri))
			{
				packagePart = _package.GetPart(partUri);
			}
			else
			{
				packagePart = _package.CreatePart(partUri, Util.GetMimeType(ManifestName), CurrentCompression);
			}
			using (Stream ps = packagePart.GetStream(FileMode.Create, FileAccess.ReadWrite))
			{
				Manifest.ToStream(ps);
			}
		}

		/// <summary>
		/// Open a package from a byte stream.
		/// </summary>
		/// <param name="stream">The byte stream of a package.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. If null, open an unencrypted package.</param>
		public void Open(Stream stream, string rsaParamsXml)
		{
			Package package;
			MemoryStream ms = new MemoryStream();
			//
			_stream = null;
			_package = null;
			CurrentPath = null;

			stream.Position = 0;
			CopyStream(ms, stream);
			ms = Decrypt(ms, rsaParamsXml);
			ms.Position = 0;

			package = Package.Open(ms, FileMode.Open, FileAccess.ReadWrite);
			if (package == null)
				throw new Exception("Failed to open package");

			_stream = ms;
			_package = package;

			LoadManifest();
		}

		/// <summary>
		/// Open a package from a file.
		/// </summary>
		/// <param name="path">The file path of the package.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. If null, open an unencrypted package.</param>
		public void Open(string path, string rsaParamsXml)
		{
			MemoryStream ms = new MemoryStream();
			Package package;
			//
			_stream = null;
			_package = null;
			CurrentPath = null;

			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				CopyStream(ms, fs);
			}

			ms = Decrypt(ms, rsaParamsXml);
			ms.Position = 0;

			package = Package.Open(ms, FileMode.Open, FileAccess.ReadWrite);
			if (package == null)
				throw new Exception("Failed to open package");

			_stream = ms;
			_package = package;
			CurrentPath = path;

			LoadManifest();
		}

		/// <summary>
		/// Open an unencrypted package
		/// </summary>
		/// <param name="path">The file path of the package.</param>
		public void Open(string path)
		{
			Open(path, null);
		}

		/// <summary>
		/// Save the package to a file. This function only works if Open has been called before and it uses the same file path again.
		/// </summary>
		/// <param name="rsaParamsXml">RSA key serialized to XML. If null, save an unencrypted package.</param>
		public void Save(string rsaParamsXml)
		{
			MemoryStream ms;
			//
			if (IsNew)
				throw new Exception("Cannot save a new package. Use SaveAs");
			SaveManifest();
			_package.Flush();
			_stream.Flush();

			ms = Encrypt(_stream, rsaParamsXml);

			using (FileStream fs = new FileStream(CurrentPath, FileMode.Create, FileAccess.ReadWrite))
			{
				ms.Position = 0;
				CopyStream(fs, ms);
			}
		}

		/// <summary>
		/// Save the package unencrypted to a file. This function only works if Open has been called before and it uses the same file path again.
		/// </summary>
		public void Save()
		{
			Save(null);
		}

		/// <summary>
		/// Save the package to a file.
		/// </summary>
		/// <param name="path">The file path of the package. Any existing file will be overwritten.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. If null, save an unencrypted package.</param>
		public void SaveAs(string path, string rsaParamsXml)
		{
			MemoryStream ms;
			bool needContentHack = IsNew;
			//
			CondCreateStream();
			SaveManifest();
			_package.Flush();
			if (needContentHack) // Flush() doesn't flush the [Content_Types].xml file! Arghh!
			{
				_package.Close();
				_stream.Position = 0;
				_package = Package.Open(_stream, FileMode.Open, FileAccess.ReadWrite);
				_package.Flush();
			}
			_stream.Flush();

			ms = Encrypt(_stream, rsaParamsXml);

			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
			{
				ms.Position = 0;
				CopyStream(fs, ms);
			}
			CurrentPath = path;
		}

		/// <summary>
		/// Save the package unencrypted to a file.
		/// </summary>
		/// <param name="path">The file path of the package. Any existing file will be overwritten.</param>
		public void SaveAs(string path)
		{
			SaveAs(path, null);
		}

		/// <summary>
		/// Returns the current path, i.e. the file path used in the Open function. If Open hasn't been called, return null.
		/// </summary>
		public string CurrentPath { get; private set; }

	    /// <summary>
		/// Set/get the current compression level. It will be used in the AddFile and Create functions. Default is 'Maximum'.
		/// </summary>
		public CompressionOption CurrentCompression { get; set; } = CompressionOption.Maximum;

	    /// <summary>
		/// Get or set the current manifest for the package
		/// </summary>
		public TemplatePackageManifest Manifest { get; set; }

	    /// <summary>
		/// Get or set the current manifest from an XML string
		/// </summary>
		public string ManifestXml
		{
			get
			{
				return Manifest == null ? null : Manifest.ToXml();
			}
			set
			{
				Manifest = string.IsNullOrEmpty(value) ? null : TemplatePackageManifest.FromXml(value);
			}
		}

		private bool CollectDependencies(TemplateInfo ti, Dictionary<string, Dependency> dict, out string errMsg)
		{
			string name, key;
			DependencyType depType;
			Dictionary<string, Dependency> dictD = new Dictionary<string, Dependency>();
			//
			if (string.IsNullOrEmpty(ti.FileName))
			{
				errMsg = "Empty template name";
				return false;
			}
			name = ti.FileName;
			key = name.ToLower();
			if (!dict.ContainsKey(key))
			{
				dict[key] = new Dependency(name, DependencyType.TemplateInsert);
			}
			if (ti.Dependencies==null)
			{
				errMsg = "Dependencies cannot be null";
				return false;
			}
			foreach (Dependency dep in ti.Dependencies)
			{
				if (string.IsNullOrEmpty(dep.FileName))
				{
					errMsg = "Dependency Target cannot be null or empty";
					return false;
				}
				name = dep.FileName;
				key = name.ToLower();
				depType = dep.DependencyType;
				if (depType == DependencyType.NoDependency)
				{
					errMsg = "Dependency Target '" + name + "' cannot be 'NoDependency'";
					return false;
				}
				if (!dict.ContainsKey(key))
				{
					dict[key] = new Dependency(name, depType);
				}
				else if (depType == DependencyType.Assemble)
				{
					dict[key].DependencyType = depType;
				}
				key += "|" + depType.ToString().ToLower();
				if (dictD.ContainsKey(key))
				{
					errMsg = "Dependency (Target=" + name + ", type=" + depType.ToString() + ") is defined more than once";
					return false;
				}
				else
				{
					dictD[key] = dep;
				}
			}
			errMsg = string.Empty;
			return true;
		}

		private bool IsValidTemplateInfo(TemplateInfo ti, Dictionary<string, Dependency> dict, out string errMsg)
		{
			string name, key, keyC, ext;
			DependencyType depType;
			Dictionary<string, string> dictS = new Dictionary<string, string>();
			bool hasJs = false, hasHvc = false;
			//
			name = ti.FileName;
			key = name.ToLower();
			depType = dict[key].DependencyType;
			if (string.IsNullOrEmpty(ti.EffectiveComponentFile))
			{
				errMsg = "EffectiveComponentFile cannot be null or empty";
				return false;
			}
			keyC = ti.EffectiveComponentFile.ToLower();
			if (!dict.ContainsKey(keyC))
			{
				errMsg = "EffectiveComponentFile doesn't occur in a dependency";
				return false;
			}
			if (!FileExists(ti.EffectiveComponentFile))
			{
				errMsg = "EffectiveComponentFile defines the file '" + ti.EffectiveComponentFile + "' which is not part of the package";
				return false;
			}
			if (ti.GeneratedFiles == null)
			{
				errMsg = "GeneratedFiles cannot be null";
				return false;
			}
			foreach (string s in ti.GeneratedFiles)
			{
				if (string.IsNullOrEmpty(s))
				{
					errMsg = "GeneratedFiles contains a null or empty string";
					return false;
				}
				name = s;
				key = name.ToLower();
				if (dictS.ContainsKey(key))
				{
					errMsg = "GeneratedFiles contains the file '" + name + "' more than once";
					return false;
				}
				dictS[key] = name;
				ext = Path.GetExtension(key).ToLower();
				if (ext == ".js" || ext == ".hvc")
				{
					hasJs = hasJs || ext==".js";
					hasHvc = hasHvc || ext == ".hvc";
					if (dict.ContainsKey(key))
					{
						errMsg = "GeneratedFiles contains the file '" + name + "' which is already referenced from another template";
						return false;
					}
					dict[key] = new Dependency(name, DependencyType.NoDependency);
					if (!FileExists(name))
					{
						errMsg = "GeneratedFiles contains the file '" + name + "' which is not part of the package";
						return false;
					}
				}
				else if (ext == ".dll" || ext == ".xml")
				{
					if (dict.ContainsKey(key))
					{
						errMsg = "GeneratedFiles contains the file '" + name + "' which is already referenced from another template";
						return false;
					}
					dict[key] = new Dependency(name, DependencyType.NoDependency);
					if (!FileExists(name))
					{
						errMsg = "GeneratedFiles contains the file '" + name + "' which is not part of the package";
						return false;
					}
				}
				else
				{
					errMsg = "GeneratedFiles contains the file '" + name + "' which has an unknown file extension";
					return false;
				}
			}

			if (CheckForGeneratedFiles)
			{
				if (!hasJs && depType == DependencyType.Assemble)
				{
					errMsg = "GeneratedFiles doesn't contain a .js file";
					return false;
				}
				if (!hasHvc && depType == DependencyType.Assemble)
				{
					errMsg = "GeneratedFiles doesn't contain a .hvc file";
					return false;
				}
			}

			errMsg = string.Empty;
			return true;
		}

		/// <summary>
		/// Check if the current manifest (property 'Manifest') is valid for the current files in the package.
		/// </summary>
		/// <param name="errMsg">If the function returns false, this string will contain an error message.</param>
		/// <returns>true if the current manifest is valid, false otherwise.</returns>
		public bool IsValidManifest(out string errMsg)
		{
			return IsValidManifest(Manifest, out errMsg);
		}

		/// <summary>
		/// Check if a given manifest is valid for the current files in the package.
		/// </summary>
		/// <param name="manifest">The manifest object to test.</param>
		/// <param name="errMsg">If the function returns false, this string will contain an error message.</param>
		/// <returns>true if the manifest is valid, false otherwise.</returns>
		public bool IsValidManifest(TemplatePackageManifest manifest, out string errMsg)
		{
			Dictionary<string, Dependency> dict = new Dictionary<string, Dependency>();
			Dictionary<string, string> dictT = new Dictionary<string, string>();
			string name, key, location;
			DependencyType depType;
			int i;
			//
			if (manifest == null)
			{
				errMsg = "No manifest defined";
				return false;
			}
			if (string.IsNullOrEmpty(manifest.HotDocsVersion))
			{
				errMsg = "Missing HotDocsVersion in manifest";
				return false;
			}
			if (manifest.Version >= 11 && string.IsNullOrEmpty(manifest.PublishDateTime))
			{
				errMsg = "Missing PublishDateTime in manifest";
				return false;
			}
			if (manifest.MainTemplate == null)
			{
				errMsg = "MainTemplate in manifest cannot be null";
				return false;
			}
			location = "MainTemplate";
			if (!CollectDependencies(manifest.MainTemplate, dict, out errMsg))
			{
				errMsg = "Error in " + location + " of manifest: " + errMsg;
				return false;
			}
			name = manifest.MainTemplate.FileName;
			key = name.ToLower();
			dict[key] = new Dependency(name, DependencyType.Assemble);
			dictT[key] = name;
			for (i = 0; i < manifest.OtherTemplates.Length; i++)
			{
				location = string.Format("OtherTemplates[{0}]", i + 1);
				if (!CollectDependencies(manifest.OtherTemplates[i], dict, out errMsg))
				{
					errMsg = "Error in " + location + " of manifest: " + errMsg;
					return false;
				}
				name = manifest.OtherTemplates[i].FileName;
				key = name.ToLower();
				if (dictT.ContainsKey(key))
				{
					errMsg = "Error in " + location + " of manifest: template'" + name + "' has been defined already";
					return false;
				}
				dictT[key] = name;
			}
			foreach (var d in dict)
			{
				key = d.Key;
				name = d.Value.FileName;
				depType = d.Value.DependencyType;
				if (Dependency.IsTemplateDependency(depType) && !dictT.ContainsKey(key))
				{
					errMsg = "Template '" + name + "' is not defined in the manifest but is listed as a dependency";
					return false;
				}
				if (!FileExists(name))
				{
					errMsg = "File '" + name + "' in manifest doesn't exist in the package";
					return false;
				}
			}
			location = "MainTemplate";
			if (!IsValidTemplateInfo(manifest.MainTemplate, dict, out errMsg))
			{
				errMsg = "Error in " + location + " of manifest: " + errMsg;
				return false;
			}
			for (i = 0; i < manifest.OtherTemplates.Length; i++)
			{
				location = string.Format("OtherTemplates[{0}]", i + 1);
				if (!IsValidTemplateInfo(manifest.OtherTemplates[i], dict, out errMsg))
				{
					errMsg = "Error in " + location + " of manifest: " + errMsg;
					return false;
				}
			}
			if (manifest.AdditionalFiles==null)
			{
				errMsg = "AdditionalFiles in manifest cannot be null";
				return false;
			}
			for (i = 0; i < manifest.AdditionalFiles.Length; i++)
			{
				location = string.Format("AdditionalFiles[{0}]", i + 1);
				name = manifest.AdditionalFiles[i];
				if (string.IsNullOrEmpty(name))
				{
					errMsg = location + " in manifest cannot be null or empty";
					return false;
				}
				key = name.ToLower();
				if (dict.ContainsKey(key))
				{
					errMsg = "Additional file '" + name + "' in manifest is already referenced from a template or occurs twice in additional files";
					return false;
				}
				dict[key] = new Dependency(name, DependencyType.NoDependency);
				if (!FileExists(name))
				{
					errMsg = "Additional file '" + name + "' in manifest doesn't exist in the package";
					return false;
				}
			}
			foreach (string s in GetFiles())
			{
				name = s;
				key = name.ToLower();
				if (!dict.ContainsKey(key))
				{
					errMsg = "File '" + name + "' in package is not referenced in manifest";
					return false;
				}
			}
			errMsg = string.Empty;
			return true;
		}

		private static bool IsDirSep(char ch)
		{
			return ch == '/' || ch == '\\';
		}

		private static Uri InternalNameToPartUri(string internalName)
		{
			Uri retUri = null;
			try
			{ 
				Uri paramUri = new Uri(internalName, UriKind.Relative);
				retUri = PackUriHelper.CreatePartUri(paramUri);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("TemplatePackage.InternalNameToPartUri: exception thrown: " + ex.Message);
				throw;
			}
			return retUri;
		}

		private static string PartUriToInternalName(Uri partUri)
		{
			string s = Uri.UnescapeDataString(partUri.ToString());
			//
			if (s.Length > 0 && IsDirSep(s[0]))
				s = s.Substring(1);
			return s.Replace('/', '\\');
		}

		/// <summary>
		/// Check if a file exists in the package.
		/// </summary>
		/// <param name="internalName">The name within the package. E.g. "template.rtf" or "images/icon.png".</param>
		/// <returns>True if the file exists in the package, false if not.</returns>
		public bool FileExists(string internalName)
		{
			Uri partUri;
			//
			CondCreateStream();
			partUri = InternalNameToPartUri(internalName);
			return _package.PartExists(partUri);
		}

		/// <summary>
		/// Add a file to the package.
		/// </summary>
		/// <param name="filePath">The full path of the file to be added. E.g. "D:\files\template.rtf".</param>
		/// <param name="internalName">The name within the package. E.g. "template.rtf" or "images/icon.png".</param>
		/// <returns>The internal name used. E.g. "template.rtf" or "images/icon.png". This name can be used to be shown to the user and can be used to extract a file again.</returns>
		public string AddFile(string filePath, string internalName)
		{
			Uri partUri;
			PackagePart packagePart = null;
			//
			CondCreateStream();
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					partUri = InternalNameToPartUri(internalName);
					packagePart = _package.CreatePart(partUri, Util.GetMimeType(filePath), CurrentCompression);
					using (Stream ps = packagePart.GetStream(FileMode.Create, FileAccess.ReadWrite))
					{
						CopyStream(ps, fs);
					}
				}
			}
			catch (IOException ex)
			{
				System.Diagnostics.Debug.WriteLine("TemplatePackage.AddFile Exception thrown: " + ex);
				throw;
			}
			if (packagePart != null)
				return PartUriToInternalName(packagePart.Uri);
			else
				return null;
		}

		/// <summary>
		/// Add a file to the package. This function uses the filepath of the path as the internal name.
		/// </summary>
		/// <param name="filePath">The full path of the file to be added. E.g. "D:\files\template.rtf".</param>
		/// <returns>The internal name used. E.g. "template.rtf". This name can be used to be shown to the user and can be used to extract a file again.</returns>
		public string AddFile(string filePath)
		{
			return AddFile(filePath, Path.GetFileName(filePath));
		}

		/// <summary>
		/// Get a list of all file names stored in the package. These are internal names. E.g. "template.rtf" or "images/icon.png".
		/// </summary>
		/// <returns>An array of (internal) file names.</returns>
		public string[] GetFiles()
		{
			return GetFiles(false);
		}

		/// <summary>
		/// Get a list of all file names stored in the package. These are internal names. E.g. "template.rtf" or "images/icon.png".
		/// </summary>
		/// <param name="includeManifest">Indicates whether the manifest should be included in the list.</param>
		/// <returns>An array of (internal) file names.</returns>
		public string[] GetFiles(bool includeManifest)
		{
			List<string> files = new List<string>();
			string filename;
			//
			if (_package != null)
			{
				foreach (var part in _package.GetParts())
				{
					filename = PartUriToInternalName(part.Uri);
					if (includeManifest || !string.Equals(filename, ManifestName, StringComparison.OrdinalIgnoreCase))
						files.Add(filename);
				}
			}
			return files.ToArray();
		}

		/// <summary>
		/// Extract an existing file.
		/// </summary>
		/// <param name="internalName">The internal name used. E.g. "template.rtf".</param>
		/// <param name="targetFolderPath">The folder where the file should be extracted to. This folder will be created if needed. Any existing file will be overwritten.</param>
		/// <returns>The full path of the extracted file.</returns>
		public string ExtractFile(string internalName, string targetFolderPath)
		{
			Uri partUri;
			PackagePart packagePart;
			string path, dir;
			//
			if (string.IsNullOrEmpty(internalName))
			{
				throw new Exception("Failed to extract file: No file name");
			}
			if (targetFolderPath==null || targetFolderPath.Length == 0)
			{
				throw new Exception("Failed to extract file: No folder name");
			}
			if (_stream == null)
			{
				throw new Exception("Failed to extract file. No stream defined");
			}
			if (IsDirSep(targetFolderPath[targetFolderPath.Length - 1]))
			{
				targetFolderPath = targetFolderPath.Substring(0, targetFolderPath.Length - 1);
			}
			partUri = InternalNameToPartUri(internalName);
			packagePart = _package.GetPart(partUri);
			if (packagePart == null)
				throw new Exception("Failed to extract file. Package part doesn't exist");

			path = targetFolderPath;
			if (!IsDirSep(internalName[0]))
				path += '\\';
			path += internalName;
			path = path.Replace('/', '\\');

			dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
			{
				using (Stream ps = packagePart.GetStream(FileMode.Open, FileAccess.Read))
				{
					CopyStream(fs, ps);
				}
			}
			return path;
		}

		/// <summary>
		/// Extract an existing file.
		/// </summary>
		/// <param name="internalName">The internal name used. E.g. "template.rtf".</param>        
		/// <returns>The stream of the extracted file.</returns>
		public Stream ExtractFile(string internalName)
		{
			Uri partUri;
			PackagePart packagePart;
			//
			if (string.IsNullOrEmpty(internalName))
			{
				throw new Exception("Failed to extract file: No file name");
			}            
			if (_stream == null)
			{
				throw new Exception("Failed to extract file. No stream defined");
			}
			
			partUri = InternalNameToPartUri(internalName);
			packagePart = _package.GetPart(partUri);
			if (packagePart == null)
				throw new Exception("Failed to extract file. Package part doesn't exist");
			
			MemoryStream ms = new MemoryStream();            
			using (Stream ps = packagePart.GetStream(FileMode.Open, FileAccess.Read))
				CopyStream(ms, ps);
			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		/// <summary>
		/// Remove an existing file.
		/// </summary>
		/// <param name="internalName">The internal name used. E.g. "template.rtf".</param>
		public void RemoveFile(string internalName)
		{
			Uri partUri;
			//
			if (string.IsNullOrEmpty(internalName))
			{
				throw new Exception("Failed to remove file: No file name");
			}
			if (_stream == null)
			{
				throw new Exception("Failed to remove file. No stream defined");
			}
			partUri = InternalNameToPartUri(internalName);
			_package.DeletePart(partUri);
		}

		/// <summary>
		/// Create a package in one step.
		/// </summary>
		/// <param name="packagePath">The full file path were the package should be written to. Any existing file will be overwritten.</param>
		/// <param name="manifest">The manifest of the package. Passing null creates a package without a manifest.</param>
		/// <param name="filePaths">A complete list of all file paths to be added to the package.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. It can be a public/private key pair or only a public key. If null, save an unencrypted package.</param>
		/// <param name="compression">The compression level.</param>
		public static void Create(string packagePath, TemplatePackageManifest manifest, IEnumerable<string> filePaths, string rsaParamsXml, CompressionOption compression, bool checkForGeneratedFiles)
		{
			TemplatePackage package = new TemplatePackage();
			package.CheckForGeneratedFiles = checkForGeneratedFiles;
			//
			package.CurrentCompression = compression;
			foreach (var s in filePaths)
			{
				package.AddFile(s);
			}
			package.Manifest = manifest;
			package.SaveAs(packagePath, rsaParamsXml);
		}

		public static void Create(string packagePath, TemplatePackageManifest manifest, IEnumerable<string> filePaths, string rsaParamsXml, CompressionOption compression)
		{
			Create(packagePath, manifest, filePaths, rsaParamsXml, compression, true);
		}

		/// <summary>
		/// Create a package in one step using the default compression option (=Maximum).
		/// </summary>
		/// <param name="packagePath">The full file path were the package should be written to. Any existing file will be overwritten.</param>
		/// <param name="manifest">The manifest of the package. Passing null creates a package without a manifest.</param>
		/// <param name="filePaths">A complete list of all file paths to be added to the package.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. It can be a public/private key pair or only a public key. If null, save an unencrypted package.</param>
		public static void Create(string packagePath, TemplatePackageManifest manifest, IEnumerable<string> filePaths, string rsaParamsXml, bool checkForGeneratedFiles)
		{
			Create(packagePath, manifest, filePaths, rsaParamsXml, CompressionOption.Maximum, checkForGeneratedFiles);
		}

		public static void Create(string packagePath, TemplatePackageManifest manifest, IEnumerable<string> filePaths, string rsaParamsXml)
		{
			Create(packagePath, manifest, filePaths, rsaParamsXml, true);
		}

		/// <summary>
		/// Create an unencrypted package in one step using the default compression option (=Maximum).
		/// </summary>
		/// <param name="packagePath">The full file path were the package should be written to. Any existing file will be overwritten.</param>
		/// <param name="manifest">The manifest of the package. Passing null creates a package without a manifest.</param>
		/// <param name="filePaths">A complete list of all file paths to be added to the package.</param>
		public static void Create(string packagePath, TemplatePackageManifest manifest, IEnumerable<string> filePaths, bool checkForGeneratedFiles)
		{
			Create(packagePath, manifest, filePaths, null, checkForGeneratedFiles);
		}

		public static void Create(string packagePath, TemplatePackageManifest manifest, IEnumerable<string> filePaths)
		{
			Create(packagePath, manifest, filePaths, true);
		}

		/// <summary>
		/// Extract a package in one step.
		/// </summary>
		/// <param name="packagePath">The full file path of the package.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. If null, extract an unencrypted package.</param>
		/// <param name="targetFolderPath">The folder where the file should be extracted to. This folder will be created if needed. Any existing file will be overwritten.</param>
		/// <param name="manifest">The manifest of the package. If null, the package had no manifest.</param>
		/// <param name="filePaths">A list containing the full file paths of all extracted files.</param>
		public static void Extract(string packagePath, string rsaParamsXml, string targetFolderPath,
								   out TemplatePackageManifest manifest, out IEnumerable<string> filePaths)
		{
			TemplatePackage package = new TemplatePackage();
			List<string> fileList = new List<string>();
			string path;
			//
			package.Open(packagePath, rsaParamsXml);
			foreach (var s in package.GetFiles())
			{
				path = package.ExtractFile(s, targetFolderPath);
				fileList.Add(path);
			}
			manifest = package.Manifest;
			filePaths = fileList;
		}

		/// <summary>
		/// Extract a package in one step, treating the manifest like any other file.
		/// </summary>
		/// <param name="packagePath">The full file path of the package.</param>
		/// <param name="rsaParamsXml">RSA key serialized to XML. If null, extract an unencrypted package.</param>
		/// <param name="targetFolderPath">The folder where the file should be extracted to. This folder will be created if needed. Any existing file will be overwritten.</param>
		public static void ExtractAllFiles(string packagePath, string rsaParamsXml, string targetFolderPath)
		{
			TemplatePackage package = new TemplatePackage();
			package.Open(packagePath, rsaParamsXml);
			foreach (var s in package.GetFiles(true))
			{
				package.ExtractFile(s, targetFolderPath);
			}
		}

		/// <summary>
		/// Extract an unencrypted package in one step.
		/// </summary>
		/// <param name="packagePath">The full file path of the package.</param>
		/// <param name="targetFolderPath">The folder where the file should be extracted to. This folder will be created if needed. Any existing file will be overwritten.</param>
		/// <param name="manifest">The manifest of the package. If null, the package had no manifest.</param>
		/// <param name="filePaths">A list containing the full file paths of all extracted files.</param>
		public static void Extract(string packagePath, string targetFolderPath,
								   out TemplatePackageManifest manifest, out IEnumerable<string> filePaths)
		{
			Extract(packagePath, null, targetFolderPath, out manifest, out filePaths);
		}
	}
}
