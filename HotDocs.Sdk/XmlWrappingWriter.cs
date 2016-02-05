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
			this.Writer = baseWriter;
		}

		public override void Close()
		{
			this.Writer.Close();
		}

		protected override void Dispose(bool disposing)
		{
			((IDisposable)this.Writer).Dispose();
		}

		public override void Flush()
		{
			this.Writer.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return this.Writer.LookupPrefix(ns);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.Writer.WriteBase64(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			this.Writer.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this.Writer.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.Writer.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			this.Writer.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this.Writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			this.Writer.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			this.Writer.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			this.Writer.WriteEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			this.Writer.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			this.Writer.WriteFullEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this.Writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteRaw(string data)
		{
			this.Writer.WriteRaw(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.Writer.WriteRaw(buffer, index, count);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this.Writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument()
		{
			this.Writer.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this.Writer.WriteStartDocument(standalone);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this.Writer.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteString(string text)
		{
			this.Writer.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.Writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteValue(bool value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(object value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			this.Writer.WriteValue(value);
		}

		public override void WriteWhitespace(string ws)
		{
			this.Writer.WriteWhitespace(ws);
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this.Writer.Settings;
			}
		}

		protected XmlWriter Writer { get; set; }

	    public override System.Xml.WriteState WriteState
		{
			get
			{
				return this.Writer.WriteState;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.Writer.XmlLang;
			}
		}

		public override System.Xml.XmlSpace XmlSpace
		{
			get
			{
				return this.Writer.XmlSpace;
			}
		}
	}
}