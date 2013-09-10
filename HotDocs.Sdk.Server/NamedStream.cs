/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server
{
	/// <summary>
	/// This class encapsulates a named file that is returned as part of the result of AssembleDocument, ConvertDocument or GetInterview.
	/// It is typically used to return files (such as graphics) that are supplementary to the primary document that is returned.
	/// Instances of this class are created by the SDK and are meant to be consumed by the host application
	/// (typically in response to a request from the browser for the file).
	/// Instances should always be disposed of to ensure streams are closed properly.
	/// <para>Use of this class can generally be avoided by other means (i.e. the host application already knowing where, on disk,
	/// to find the requested file), but it is there as an option if necessary.</para>
	/// </summary>
	public class NamedStream : IDisposable
	{
		public NamedStream(string fileName, System.IO.Stream content)
		{
			if (String.IsNullOrEmpty(fileName))
				throw new ArgumentException("missing fileName", fileName);
			if (content == null)
				throw new ArgumentNullException("content");

			FileName = fileName;
			Content = content;
		}

		public string FileName { get; private set; }
		public System.IO.Stream Content { get; private set; }
		private bool disposed = false; // to detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (Content != null)
					{
						Content.Dispose();
						Content = null;
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
