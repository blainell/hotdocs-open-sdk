/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HotDocs.Sdk
{
	public partial class Util
	{
		internal static string EncryptString(string textToEncrypt)
		{
			byte[] key = GetPersistentEncryptionKey();
			byte[] initializationVector = GetInitializationVector();
			byte[] encryptedBuf = EncryptStringToBytes_Aes(textToEncrypt, key, initializationVector);
			return Convert.ToBase64String(encryptedBuf);
		}

		internal static string DecryptString(string textToDecrypt)
		{
			byte[] key = GetPersistentEncryptionKey();
			byte[] initializationVector = GetInitializationVector();
			byte[] encryptedBuffer = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
			return DecryptStringFromBytes_Aes(encryptedBuffer, key, initializationVector);
		}

		internal static TEnum ReadConfigurationEnum<TEnum>(string settingName, TEnum defaultValue) where TEnum : struct
		{
			string sTemp = ConfigurationManager.AppSettings[settingName];
			if (String.IsNullOrWhiteSpace(sTemp))
				return defaultValue;
			TEnum result;
			if (Enum.TryParse<TEnum>(sTemp, true, out result))
				return result;
			// else
			throw new ApplicationException("Invalid configuration setting " + settingName);
		}

		internal static Tristate ReadConfigurationTristate(string settingName, Tristate defaultValue)
		{
			return ReadConfigurationEnum<Tristate>(settingName, defaultValue);
		}

		internal static bool ReadConfigurationBoolean(string settingName, bool defaultValue)
		{
			string sTemp = ConfigurationManager.AppSettings[settingName];
			if (String.IsNullOrWhiteSpace(sTemp))
				return defaultValue;
			bool result;
			if (Boolean.TryParse(sTemp, out result))
				return result;
			// else
			throw new ApplicationException("Invalid configuration setting " + settingName);
		}

		internal static int ReadConfigurationInt(string settingName, int defaultValue)
		{
			string sTemp = ConfigurationManager.AppSettings[settingName];
			if (String.IsNullOrWhiteSpace(sTemp))
				return defaultValue;
			int result;
			if (Int32.TryParse(sTemp, out result))
				return result;
			// else
			throw new ApplicationException("Invalid configuration setting " + settingName);
		}

		private static byte[] GetPersistentEncryptionKey()
		{
			string key = System.Configuration.ConfigurationManager.AppSettings["EncryptionKey"];
			// TODO: Remove this
			if (String.IsNullOrEmpty(key))
				key = "dummy";
			return GetFixedSizeByteArray(key, 16);
		}

		private static byte[] GetInitializationVector()
		{
			return GetFixedSizeByteArray("This is the initialization vector.", 16);
		}

		private static byte[] GetFixedSizeByteArray(string content, int byteSize)
		{
			byte[] keyBuf = UnicodeEncoding.UTF8.GetBytes(content);
			int size = byteSize;
			byte[] keyResult = new byte[size];
			int len = keyBuf.Length;
			for (int i = 0; i < size; i++)
				keyResult[i] = i < len ? keyBuf[i] : (byte)'_';
			return keyResult;
		}

		/// <summary>
		/// This method returns the MIME type of the specified file.
		/// </summary>
		/// <param name="fileName">The file name (including extension) of the file. (The MIME type is determined from the extension, so the file name must have an extension.)</param>
		/// <param name="onlyImages">Indicates whether or not all known MIME types should be checked, or only a small number of supported image MIME types.</param>
		/// <returns>A string containing the MIME type for the file.</returns>
		public static string GetMimeType(string fileName, bool onlyImages = false)
		{
			// Validate parameters; fileName must not be null or empty.
			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException();

			// Extract the file name extension from the fileName; throw an exception if the extension is missing.
			string ext = Path.GetExtension(fileName).TrimStart('.').ToLower();
			if (string.IsNullOrEmpty(ext))
				throw new ArgumentNullException();

			// Check to see if the extension is one of the known image types.
			switch (ext)
			{
				case "jpg":
				case "jpe":
				case "jpeg":
					return System.Net.Mime.MediaTypeNames.Image.Jpeg; // "image/jpeg"
				case "png":
				case "pnz":
					return "image/png";
				case "gif":
					return System.Net.Mime.MediaTypeNames.Image.Gif; // "image/gif"
				case "bmp":
				case "dib":
					return "image/bmp";
			}

			// If we are only checking for images, and we get this far, throw an exception.
			// Otherwise, return the appropriate non-image MIME type.
			if (onlyImages)
				throw new System.ArgumentException("The requested filename was not a supported image.");
			else
				switch (ext)
				{
					case "xml":
					case "cmp":
					case "ans":
					case "anx":
						return System.Net.Mime.MediaTypeNames.Text.Xml;
					case "xaml":
						return "application/xaml+xml";
					case "htm":
					case "html":
						return System.Net.Mime.MediaTypeNames.Text.Html; //"text/html";
					case "css":
						return "text/css";
					case "js":
						return "text/javascript";
					case "vbs":
						return "text/vbscript";
					case "xap":
						return "application/x-silverlight-app";
					case "tif":
					case "tiff":
						return System.Net.Mime.MediaTypeNames.Image.Tiff; //"image/tiff";
					case "jfif":
						return "image/pjpeg";
					case "ico":
						return "image/x-icon";
					case "svg":
						return "image/svg+xml";
					case "rtf":
						return /*"application/msword"*/System.Net.Mime.MediaTypeNames.Application.Rtf; // See http://en.wikipedia.org/wiki/Rich_Text_Format
					case "doc":
					case "dot":
						return "application/msword";
					case "docx": // See http://blogs.msdn.com/b/vsofficedeveloper/archive/2008/05/08/office-2007-open-xml-mime-types.aspx
						return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
					case "dotx":
						return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
					case "docm":
						return "application/vnd.ms-word.document.macroEnabled.12";
					case "dotm":
						return "application/vnd.ms-word.template.macroEnabled.12";
					case "wpd":
						return "application/wordperfect";
					case "pdf":
					case "hpt":
					case "hpd":
						return System.Net.Mime.MediaTypeNames.Application.Pdf; // "application/pdf"
					case "hft":
						return "application/x-hotdocs-hft";
					case "hfd":
						return "application/x-hotdocs-hfd";
					case "txt":
					case "ttx":
						return "text/plain";
					case "zip":
						return "application/zip";
				}
			return System.Net.Mime.MediaTypeNames.Application.Octet; // "application/octet-stream";
		}



	}

	#region Data sources configuration file section handler

	public class CustomDataSourcesSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(DataSourcesCollection))]
		public DataSourcesCollection DataSources
		{
			get
			{
				return (DataSourcesCollection)base[""];
			}
		}
	}

	public class DataSourcesCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new DataSourceElement();
		}

		protected override Object GetElementKey(ConfigurationElement element)
		{
			return ((DataSourceElement)element).Name;
		}

		public DataSourceElement this[int index]
		{
			get
			{
				return (DataSourceElement)BaseGet(index);
			}
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		new public DataSourceElement this[string Name]
		{
			get
			{
				return (DataSourceElement)BaseGet(Name);
			}
		}

		public int IndexOf(DataSourceElement dataServiceElement)
		{
			return BaseIndexOf(dataServiceElement);
		}

		public void Add(DataSourceElement dataServiceElement)
		{
			BaseAdd(dataServiceElement);
		}

		protected override void BaseAdd(ConfigurationElement element)
		{
			BaseAdd(element, false);
		}

		public void Remove(DataSourceElement dataServiceElement)
		{
			if (BaseIndexOf(dataServiceElement) >= 0)
				BaseRemove(dataServiceElement.Name);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Remove(string name)
		{
			BaseRemove(name);
		}

		public void Clear()
		{
			BaseClear();
		}
	}

	public class DataSourceElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get
			{
				return (string)this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		[ConfigurationProperty("address", IsRequired = true)]
		public string Address
		{
			get
			{
				return (string)this["address"];
			}
			set
			{
				this["address"] = value;
			}
		}
	}

	#endregion
}
