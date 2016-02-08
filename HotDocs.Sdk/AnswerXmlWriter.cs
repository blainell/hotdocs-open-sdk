/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System.IO;
using System.Xml;

namespace HotDocs.Sdk
{
    internal class AnswerXmlWriter : XmlWrappingWriter
    {
        private readonly bool _forInterview;
        private bool _skipping;

        public AnswerXmlWriter(TextWriter output, XmlWriterSettings settings, bool forInterview)
            : base(Create(output, settings))
        {
            _forInterview = forInterview;
            _skipping = false;
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if (_forInterview || (localName != "userModifiable" && localName != "userExtendible"))
            {
                _skipping = false;
                base.WriteStartAttribute(prefix, localName, ns);
            }
            else
            {
                _skipping = true;
            }
        }

        public override void WriteEndAttribute()
        {
            if (_skipping)
            {
                _skipping = false;
            }
            else
            {
                base.WriteEndAttribute();
            }
        }

        public override void WriteEndElement()
        {
            _skipping = false;
            base.WriteEndElement();
        }
    }
}