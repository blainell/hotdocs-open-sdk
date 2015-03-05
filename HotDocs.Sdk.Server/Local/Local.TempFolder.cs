/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server.Local
{
	internal class TempFolder : IDisposable
	{
		public string Path { get; private set; }
		private bool disposed = false; // to detect redundant calls

		public TempFolder(string parentTempFolder)
		{
			Path = System.IO.Path.Combine(parentTempFolder, System.IO.Path.GetRandomFileName());
			System.IO.Directory.CreateDirectory(Path);
		}

		private void Delete()
		{
			if (String.IsNullOrEmpty(Path))
				throw new InvalidOperationException("Temporary directory has already been deleted");

			System.IO.Directory.Delete(Path, true);
			Path = null;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (Path != null)
					{
						Delete();
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
