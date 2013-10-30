/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;

namespace SamplePortal
{
	/// <summary>
	/// A <c>Document</c> object represents a single document stored in an <c>AssembledDocsCache</c> object.
	/// </summary>
	public class Document
	{
		private string m_templateTitle = "";
		private string m_documentFilePath = "";

		/// <summary>
		/// Construct a new Document object.
		/// </summary>
		/// <param name="documentFilePath">The file name and path of the document.</param>
		/// <param name="templateTitle">The title of the template associated with the document.</param>
		public Document(string documentFilePath, string templateTitle)
		{
			TemplateTitle = templateTitle;
			DocumentFilePath = documentFilePath;
		}
		/// <summary>
		/// The title of the template associated with the document.
		/// </summary>
		public string TemplateTitle
		{
			get
			{
				return m_templateTitle;
			}
			set
			{
				m_templateTitle = value;
			}
		}
		/// <summary>
		/// The path of the assembled document.
		/// </summary>
		public string DocumentFilePath
		{
			get
			{
				return m_documentFilePath;
			}
			set
			{
				m_documentFilePath = value;
			}
		}
	}

	/// <summary>
	/// The AssembledDocsCache cache manages the documents assembled by a set of templates--one document
	///  for each template. This class does not yet manage ancillary files such as images for HTML.
	///  Use one AssembledDocsCache object per user session.
	/// </summary>
	public class AssembledDocsCache : IDisposable
	{
		/// <summary>
		///Construct an AssembledDocsCache object. 
		/// document folders. (Assembled documents will be stored in a sub-folder of parentFolder.) 
		/// </summary>
		/// <param name="parentFolder">The folder intended to contain all assembled document folders.
		/// (Assembled documents will be stored in a sub-folder of parentFolder.)</param>
		public AssembledDocsCache(string parentFolder)
		{
			do
			{
				_folder = Path.Combine(parentFolder, Path.GetRandomFileName());
			} while (Directory.Exists(_folder));
			Directory.CreateDirectory(_folder);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Util.SafeDeleteFolder(_folder);
		}

		#endregion

		/// <summary>
		/// Clear all items from the cache.
		/// </summary>
		public void Clear() {
			_docs.Clear();
		}

		/// <summary>
		/// Store a document and associate the document with its template's path.
		/// </summary>
		/// <param name="document">The document to be stored.</param>
		public void AddDoc(HotDocs.Sdk.Server.Document document)
		{
			try
			{
				string docPath = Path.Combine(_folder, Path.GetRandomFileName() + document.FileExtension);

				//Write the document to a file.
				using (FileStream outputStream = File.Create(docPath))
				{
					// Make sure we copy from the start of the memory stream.
					document.Content.Position = 0;
					document.Content.CopyTo(outputStream);
				}

				SamplePortal.Document doc = new Document(docPath, document.Source.Title);
				_docs.Add(doc);
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Returns a document as a stream.
		/// </summary>
		/// <param name="index">The index of the document to return.</param>
		/// <returns></returns>
		public System.IO.Stream GetDoc(int index)
		{
			return File.OpenRead(_docs[index].DocumentFilePath);
		}

		/// <summary>
		/// Returns the number of documents stored.
		/// </summary>
		public int Count
		{
			get
			{
				return _docs.Count;
			}
		}

		/// <summary>
		/// Returns the list of documents as an array of Document objects.
		/// </summary>
		/// <returns></returns>
		public Document[] ToArray()
		{
			return _docs.ToArray();
		}

		private string _folder = "";//The folder for storing documents.
		private List<Document> _docs = new List<Document>();//The documents with the upper-case template path as the key.
	}
}
