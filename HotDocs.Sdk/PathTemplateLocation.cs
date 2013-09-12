/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add appropriate unit tests.

using System;
using System.IO;

namespace HotDocs.Sdk
{
	/// <summary>
	/// <c>PathTemplateLocation</c> is a <c>TemplateLocation</c> class that represents the directory path
	/// for a template that simply resides as a file in the file system. The template does not reside in
	/// a package or database, for example.
	/// </summary>
	public class PathTemplateLocation : TemplateLocation
	{
		private string _templateDir;//The directory where the template resides.

		/// <summary>
		/// Construct a <c>PathTemplateLocation</c> representing the path of the directory containing the
		/// template file.
		/// </summary>
		/// <param name="templateDir">The path of the directory containing the template file.</param>
		public PathTemplateLocation(string templateDir)
		{
			if (templateDir == null || templateDir == "")
				throw new Exception("Invalid templateDir parameter.");
			if (!Directory.Exists(templateDir))
				throw new Exception("The folder \"" + templateDir + "\" does not exist.");
			_templateDir = templateDir;
		}
		/// <summary>
		/// Returns a new duplicate instance of this object. Overrides <see cref="TemplateLocation.Duplicate"/>.
		/// </summary>
		/// <returns></returns>
		public override TemplateLocation Duplicate()
		{
			return new PathTemplateLocation(_templateDir);
		}
		/// <summary>
		/// Returns a stream for a file in the template's directory. Overrides <see cref="TemplateLocation.GetFile"/>.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns></returns>
		public override Stream GetFile(string fileName)
		{
			string filePath = Path.Combine(GetTemplateDirectory(), fileName);
			//Note that if the file path does not exist, the FileStream constructor will throw an exception.
			return new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}
		/// <summary>
		/// Returns the template's directory. Overrides <see cref="TemplateLocation.GetTemplateDirectory"/>.
		/// </summary>
		/// <returns></returns>
		public override string GetTemplateDirectory()
		{
			return _templateDir;
		}
		/// <summary>
		/// Serialize the content of this object. Overrides <see cref="TemplateLocation.SerializeContent"/>.
		/// </summary>
		/// <returns></returns>
		protected override string SerializeContent()
		{
			return _templateDir;
		}
		/// <summary>
		/// Set the fields and properties of this object by deserializing a string returned from SerializeContent.
		/// Overrides <see cref="TemplateLocation.DeserializeContent"/>.
		/// </summary>
		/// <param name="content">A content string returned from SerializeContent.</param>
		protected override void DeserializeContent(string content)
		{
			if (content == null || content == "")
				throw new Exception("Invalid content parameter.");
			if (!Directory.Exists(content))
				throw new Exception("The folder \"" + content + "\" does not exist.");

			_templateDir = content;
		}
	}
}
