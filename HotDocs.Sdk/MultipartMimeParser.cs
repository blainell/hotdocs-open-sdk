/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     This class parses a multipart MIME stream.
    /// </summary>
    public sealed class MultipartMimeParser : IDisposable
    {
        #region Delegates

        public delegate Stream StreamGetter(Dictionary<string, string> headers);

        #endregion

        #region Constructors

        public MultipartMimeParser()
        {
            _splitter = new StreamSplitter();
            _scratchPad = new byte[4*1024]; // Big enough for a really big MIME part header
            _scratchPadStream = new MemoryStream(_scratchPad, true);
        }

        #endregion

        #region Dispose methods

        public void Dispose() // Since this class is sealed, we don't need to implement the full dispose pattern.
        {
            if (_scratchPadStream != null)
            {
                _scratchPadStream.Dispose();
                _scratchPadStream = null;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Writes the MIME parts from streamIn to streams that are provided by the consumer.
        ///     If the output stream is a FileStream, it is disposed after the data is written to the file.
        /// </summary>
        /// <param name="streamIn"></param>
        /// <param name="outputStreamGetter">A delegate that provides an output stream based on MIME part header values.</param>
        /// <param name="boundary"></param>
        public void WritePartsToStreams(Stream streamIn, StreamGetter outputStreamGetter, string boundary)
        {
            var boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary);
            _splitter.Init(streamIn);

            _splitter.WriteUntilPattern(Stream.Null, boundaryBytes); // Discard everything until the first boundary.
            _splitter.WriteBytes(Stream.Null, 2); // Skip CR-LF

            do
            {
                // Note that if streamOut is Stream.Null, it doesn't hurt anything to dispose it.
                var streamOut = outputStreamGetter(GetHeaders());
                _splitter.WriteUntilPattern(streamOut, boundaryBytes);

                // Grab the 2 bytes that follow the boundary.
                // The two bytes will be CR-LF for normal boundaries, but they will
                // be "--" for the terminating boundary.
                _scratchPadStream.Position = 0;
                _splitter.WriteBytes(_scratchPadStream, 2);

                if (streamOut is FileStream)
                {
                    streamOut.Dispose();
                }
            } while (!(_scratchPad[0] == 0x2d && _scratchPad[1] == 0x2d));
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Gets the MIME part headers and returns them in a dictionary.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetHeaders()
        {
            var headers = new Dictionary<string, string>();
            string line;

            do
            {
                _scratchPadStream.Position = 0;
                _splitter.WriteUntilPattern(_scratchPadStream, _CRLF);
                line = Encoding.ASCII.GetString(_scratchPad, 0, (int) _scratchPadStream.Position);
                if (line.Length > 0)
                {
                    var header = line.Split(_headerDelimiters, 2);
                    headers.Add(header[0].Trim(), header[1].Trim());
                }
            } while (line != "");

            return headers;
        }

        #endregion

        #region Private fields

        private readonly StreamSplitter _splitter;
        private readonly byte[] _scratchPad;
        private MemoryStream _scratchPadStream;
        private readonly char[] _headerDelimiters = {':'};
        private readonly byte[] _CRLF = {0x0d, 0x0a};

        #endregion
    }
}