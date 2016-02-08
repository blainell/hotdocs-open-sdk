/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Xml;

namespace HotDocs.Sdk
{
    internal class XmlWrappingWriter : XmlWriter
    {
        public XmlWrappingWriter(XmlWriter baseWriter)
        {
            Writer = baseWriter;
        }

        public override XmlWriterSettings Settings
        {
            get { return Writer.Settings; }
        }

        protected XmlWriter Writer { get; set; }

        public override WriteState WriteState
        {
            get { return Writer.WriteState; }
        }

        public override string XmlLang
        {
            get { return Writer.XmlLang; }
        }

        public override XmlSpace XmlSpace
        {
            get { return Writer.XmlSpace; }
        }

        public override void Close()
        {
            Writer.Close();
        }

        protected override void Dispose(bool disposing)
        {
            ((IDisposable) Writer).Dispose();
        }

        public override void Flush()
        {
            Writer.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return Writer.LookupPrefix(ns);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            Writer.WriteBase64(buffer, index, count);
        }

        public override void WriteCData(string text)
        {
            Writer.WriteCData(text);
        }

        public override void WriteCharEntity(char ch)
        {
            Writer.WriteCharEntity(ch);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            Writer.WriteChars(buffer, index, count);
        }

        public override void WriteComment(string text)
        {
            Writer.WriteComment(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            Writer.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteEndAttribute()
        {
            Writer.WriteEndAttribute();
        }

        public override void WriteEndDocument()
        {
            Writer.WriteEndDocument();
        }

        public override void WriteEndElement()
        {
            Writer.WriteEndElement();
        }

        public override void WriteEntityRef(string name)
        {
            Writer.WriteEntityRef(name);
        }

        public override void WriteFullEndElement()
        {
            Writer.WriteFullEndElement();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            Writer.WriteProcessingInstruction(name, text);
        }

        public override void WriteRaw(string data)
        {
            Writer.WriteRaw(data);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            Writer.WriteRaw(buffer, index, count);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            Writer.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteStartDocument()
        {
            Writer.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            Writer.WriteStartDocument(standalone);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            Writer.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteString(string text)
        {
            Writer.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            Writer.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteValue(bool value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(DateTime value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(decimal value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(double value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(int value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(long value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(object value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(float value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteValue(string value)
        {
            Writer.WriteValue(value);
        }

        public override void WriteWhitespace(string ws)
        {
            Writer.WriteWhitespace(ws);
        }
    }
}