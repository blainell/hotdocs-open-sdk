/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML documentation.
using System;
using System.Collections.Generic;
using System.IO;

namespace SamplePortal
{
	//The AssembledDocsCache cache manages the documents assembled by a set of templates--one document
	// for each template. This class does not yet manage ancillary files such as images for HTML.
	// Use one AssembledDocsCache object per user session.
	public class AssembledDocsCache : IDisposable
	{
		//Construct an AssembledDocsCache object. Pass the folder intended to contain all assembled
		// document folders. (Assembled documents will be stored in a sub-folder of parentFolder.)
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

		//Serialize a document and associate the document with a template's path.
		public void AddDoc(HotDocs.Sdk.Server.Document document)
		{
			try
			{
				string docPath = Path.Combine(_folder, Path.GetRandomFileName() + document.FileExtension);

				//Write the document to a file.
				using (FileStream outputStream = File.Create(docPath))
				{
					long len = document.Content.Length;
					byte[] buffer = new byte[len];
					document.Content.Read(buffer, 0, (int)len);
					outputStream.Write(buffer, 0, (int)len);
				}

				SamplePortal.Document doc = new Document(docPath, document.Source.GetTitle());
				_docs.Add(doc);
			}
			catch (Exception)
			{
			}
		}

		//Get a stream to a serialized document associated with a template path.
		public System.IO.Stream GetDoc(int index)
		{
			return File.OpenRead(_docs[index].DocumentFilePath);
		}

		//Return the number of documents.
		public int Count
		{
			get
			{
				return _docs.Count;
			}
		}

		public Document[] ToArray()
		{
			return _docs.ToArray();
		}

		private string _folder = "";//The folder for storing documents.
		private List<Document> _docs = new List<Document>();//The documents with the upper-case template path as the key.
	}
}
