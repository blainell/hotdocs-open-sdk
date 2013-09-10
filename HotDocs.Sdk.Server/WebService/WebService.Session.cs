/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server.WebService
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

		public HotDocs.Sdk.Server.TemplateProperties[] GetTemplateProperties(string[] masterTemplates)
		{
			List<HotDocs.Sdk.Server.TemplateProperties> list = 
				new List<HotDocs.Sdk.Server.TemplateProperties>(masterTemplates.Count());
			Proxy client = new Proxy("BasicHttpBinding_IHDSvc");
			HotDocs.Sdk.Server.Contracts.TemplateInfo[] stiArray = client.GetTemplateInfo(masterTemplates.ToArray());
			int cnt = stiArray.Count();
			for (int i=0 ; i<cnt ; i++)
			{
				HotDocs.Sdk.Server.Contracts.TemplateInfo sti = stiArray[i];
				HotDocs.Sdk.Server.TemplateProperties tp = new HotDocs.Sdk.Server.TemplateProperties();
				tp.TemplateFile = masterTemplates[i];
				tp.TemplateTitle = sti.Title;
				tp.TemplateDescription = sti.Description;

				list.Add(tp);
			}
			return list.ToArray();
		}

		//Consider: Find a less-convoluted means of creating a new assembly queue. The convolution is caused by the assembly
		// queue requiring a primary assembly. Perhaps this should be addressed outside the context of its constructor.
		// Another complication is that the assembly queue requires an IHdSession object. To facilitate moving this
		// method to the base class, HdSessionBase could inherit IHdSession.
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
