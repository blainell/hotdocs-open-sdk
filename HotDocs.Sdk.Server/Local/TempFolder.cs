/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.IO;

namespace HotDocs.Sdk.Server.Local
{
    internal class TempFolder : IDisposable
    {
        private bool disposed; // to detect redundant calls

        public TempFolder(string parentTempFolder)
        {
            Path = System.IO.Path.Combine(parentTempFolder, System.IO.Path.GetRandomFileName());
            Directory.CreateDirectory(Path);
        }

        public string Path { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Delete()
        {
            if (string.IsNullOrEmpty(Path))
                throw new InvalidOperationException("Temporary directory has already been deleted");

            Directory.Delete(Path, true);
            Path = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing && Path != null)
                {
                    Delete();
                }
                disposed = true;
            }
        }
    }
}