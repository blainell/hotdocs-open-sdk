/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server.Local
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	// Class: HdSession
	////////////////////////////////////////////////////////////////////////////////////////////////////
	internal class Session : HotDocs.Sdk.Server.SessionBase, HotDocs.Sdk.Server.ISession
	{
		internal Session(string tempFolderPath) : base(tempFolderPath)
		{
		}

		#region ISession Members

		public TemplateProperties[] GetTemplateProperties(string[] masterTemplates)
		{
			List<TemplateProperties> tiList = new List<TemplateProperties>();
			foreach (string masterFilename in masterTemplates)
			{
				string tplPath = masterFilename;
				using (HotDocs.Server.TemplateInfo hdti = new HotDocs.Server.TemplateInfo(tplPath))
				{
					HotDocs.Sdk.Server.TemplateProperties tp = new HotDocs.Sdk.Server.TemplateProperties();
					tp.TemplateFile = masterFilename;
					tp.TemplateTitle = hdti.TemplateTitle;
					tp.TemplateDescription = hdti.TemplateDescription;
					tiList.Add(tp);
				}
			}
			return tiList.ToArray();
		}

		public IAssembly AddMasterAssembly(string templateFile)
		{
			AssemblyQueue q = new AssemblyQueue(templateFile, BuildAssembly, this);
			_assemblyQueues.Add(q);
			return q.MasterAssembly;
		}

		#endregion

		private IAssembly BuildAssembly(string templateFile, AssemblyQueue q)
		{
			//Build the assembly queue and assembly objects with the requisite defaults.
			Assembly asm = new Assembly(q);
			asm.TemplateFile = templateFile;
			asm.ServerFilesUrl = DefaultHotDocsJavascriptUrl;
			asm.StyleUrl = DefaultStyleUrl;

			return asm;
		}
	}
}
