/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HotDocs.Sdk.Server.Cloud
{
	internal class Session : SessionBase, ICloudSession
	{
		internal Session(string tempFolderPath)
			: base(tempFolderPath)
		{
		}

		#region ICloudSession Members

		public string SubscriberId
		{
			get;
			set;
		}

		public string SigningKey
		{
			get;
			set;
		}

		public string BillingRef
		{
			get;
			set;
		}

		public string InterviewFilesBaseDir
		{
			get;
			set;
		}

		public TemplateProperties[] GetTemplateProperties(string[] masterTemplates)
		{
			throw new NotImplementedException();
		}

		public IAssembly AddMasterAssembly(string templateFile)
		{
			throw new NotImplementedException();
		}

		public IAssembly AddMasterAssembly(string packageId, string packageFile, string templateId = null)
		{
			throw new NotImplementedException();

			//AssemblyQueue q = new AssemblyQueue(packageId, packageFile, templateId, BuildAssembly, this);
			//_assemblyQueues.Add(q);
			//return q.MasterAssembly;
		}

		#endregion

		private IAssembly BuildAssembly(string packageId, string packagePath, string templateId, AssemblyQueue q)
		{
			throw new NotImplementedException();

			////Build the assembly queue and assembly objects with the requisite defaults.
			//if (templateId == null)
			//{
			//	// If no template specified, use the main template.
			//	templateId = HotDocs.Sdk.Server.Cloud.Util.GetMainTemplateFileName(
			//		Path.Combine(TemplateFolderPath, packagePath));
			//}
			//Cloud.Assembly asm = new Cloud.Assembly(q);
			//asm.TemplateFile = packageId;
			//asm.TemplateId = templateId;
			//asm.ServerFilesUrl = DefaultHotDocsJavascriptUrl;
			//asm.StyleUrl = DefaultStyleUrl;

			//return asm;
		}
	}
}
