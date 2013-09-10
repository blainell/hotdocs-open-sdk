/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server
{
	/// <summary>
	/// The results of assembling a document. An object of this type is returned by AssembleDocument().
	/// </summary>
	public class AssembleDocumentResult : IDisposable
	{
		// internal constructor
		//TODO: Consider using IEnumerable instead of arrays?
		public AssembleDocumentResult(Document document, string answers, Template[] pendingAssemblies, string[] unansweredVariables)
		{
			Document = document;
			if (answers != null)
				Answers = answers;
			else
				Answers = String.Empty;
			PendingAssemblies = pendingAssemblies;
			UnansweredVariables = unansweredVariables;
		}

		/// <summary>
		/// 
		/// </summary>
		public Document Document { get; private set; }

		/// <summary>
		/// The post-assembly answer file (potentially modified from before the assembly, since assembly can have side effects)
		/// </summary>
		public string Answers { get; private set; }

		/// <summary>
		/// An array of assemblies that need to be completed after this assembly is finished
		/// (results of ASSEMBLE instructions in the assembled template).
		/// </summary>
		public Template[] PendingAssemblies { get; private set; }

		/// <summary>
		/// An array of variable names for which answers were called for during assembly,
		/// but for which no answer was included in the answer collection.
		/// </summary>
		public string[] UnansweredVariables { get; private set; }

		/// <summary>
		/// "Extracts" a Document object from this AssemblyResult instance.  This essentially
		/// shifts responsibility for disposing of the Content and SupportingFiles members from
		/// this AssemblyResult to the returned Document.  This is used when a WorkSession
		/// aggregates the AssemblyResults for one or more documents into an array of DocumentResults
		/// and persists the rest of the AssemblyResult members elsewhere.
		/// </summary>
		/// <returns>The "extracted" Document, now the caller's responsibility to Dispose().</returns>
		internal Document ExtractDocument()
		{
			var result = Document;
			// pass ownership of the document & supporting files over to the returned Document
			// (so they are disposed when the new Document is disposed, not this object)
			Document = null;
			return result;
		}

		private bool disposed = false; // to detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (Document != null)
					{
						Document.Dispose();
						Document = null;
					}
				}
				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

	}

}
