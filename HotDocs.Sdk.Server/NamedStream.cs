/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.IO;

namespace HotDocs.Sdk.Server
{
    /// <summary>
    ///     This class encapsulates a named file that is returned as part of the result of AssembleDocument, ConvertDocument or
    ///     GetInterview.
    ///     It is typically used to return files (such as graphics) that are supplementary to the primary document that is
    ///     returned.
    ///     Instances of this class are created by the SDK and are meant to be consumed by the host application
    ///     (typically in response to a request from the browser for the file).
    ///     Instances should always be disposed of to ensure streams are closed properly.
    ///     <para>
    ///         Use of this class can generally be avoided by other means (i.e. the host application already knowing where, on
    ///         disk,
    ///         to find the requested file), but it is there as an option if necessary.
    ///     </para>
    /// </summary>
    public class NamedStream : IDisposable
    {
        private bool disposed; // to detect redundant calls

        /// <summary>
        ///     <c>NamedStream</c> constructor creates an instance with the <c>fileName</c> and <c>content</c> parameters.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="content">The file contents.</param>
        public NamedStream(string fileName, Stream content)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("missing fileName", fileName);
            if (content == null)
                throw new ArgumentNullException("content");

            FileName = fileName;
            Content = content;
        }

        /// <summary>
        ///     <c>FileName</c> is the given name for the assocated Stream from the 'Content' property.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        ///     <c>Content</c> returns the IO stream assocated with this object.
        /// </summary>
        public Stream Content { get; private set; }

        /// <summary>
        ///     <c>Dispose</c> implements the <c>IDisposable</c> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Calls <c>Dispose</c> from the <c>IDisposable</c> interface
        /// </summary>
        /// <param name="disposing">Indicates whether or not managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing && Content != null)
                {
                    Content.Dispose();
                    Content = null;
                }
                disposed = true;
            }
        }
    }
}