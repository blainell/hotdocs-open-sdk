/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.

using System.IO;

namespace HotDocs.Sdk
{
	public class PathTemplateLocation : TemplateLocation
	{
		public PathTemplateLocation(string templateDir)
		{
			_templateDir = templateDir;
		}

		public override TemplateLocation Duplicate()
		{
			return new PathTemplateLocation(_templateDir);
		}

		//TODO: Allow for readonly files.
		public override Stream GetFile(string fileName)
		{
			string filePath = Path.Combine(GetTemplateDirectory(), fileName);
			return new FileStream(filePath, FileMode.Open);
		}

		public override string GetTemplateDirectory()
		{
			return _templateDir;
		}

		protected override string SerializeContent()
		{
			return _templateDir;
		}

		protected override void DeserializeContent(string content)
		{
			_templateDir = content;
		}

		private string _templateDir;
	}
}
