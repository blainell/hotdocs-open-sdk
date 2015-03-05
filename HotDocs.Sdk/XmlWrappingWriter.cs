/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Xml;

namespace HotDocs.Sdk
{
	internal class XmlWrappingWriter : XmlWriter
	{
		protected XmlWriter _writer;

		public XmlWrappingWriter(XmlWriter baseWriter)
		{
			this.Writer = baseWriter;
		}

		public override void Close()
		{
			this._writer.Close();
		}

		protected override void Dispose(bool disposing)
		{
			((IDisposable)this._writer).Dispose();
		}

		public override void Flush()
		{
			this._writer.Flush();
		}

		public override string LookupPrefix(string ns)
		{
			return this._writer.LookupPrefix(ns);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this._writer.WriteBase64(buffer, index, count);
		}

		public override void WriteCData(string text)
		{
			this._writer.WriteCData(text);
		}

		public override void WriteCharEntity(char ch)
		{
			this._writer.WriteCharEntity(ch);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this._writer.WriteChars(buffer, index, count);
		}

		public override void WriteComment(string text)
		{
			this._writer.WriteComment(text);
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			this._writer.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteEndAttribute()
		{
			this._writer.WriteEndAttribute();
		}

		public override void WriteEndDocument()
		{
			this._writer.WriteEndDocument();
		}

		public override void WriteEndElement()
		{
			this._writer.WriteEndElement();
		}

		public override void WriteEntityRef(string name)
		{
			this._writer.WriteEntityRef(name);
		}

		public override void WriteFullEndElement()
		{
			this._writer.WriteFullEndElement();
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			this._writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteRaw(string data)
		{
			this._writer.WriteRaw(data);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this._writer.WriteRaw(buffer, index, count);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			this._writer.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteStartDocument()
		{
			this._writer.WriteStartDocument();
		}

		public override void WriteStartDocument(bool standalone)
		{
			this._writer.WriteStartDocument(standalone);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			this._writer.WriteStartElement(prefix, localName, ns);
		}

		public override void WriteString(string text)
		{
			this._writer.WriteString(text);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this._writer.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteValue(bool value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(DateTime value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(decimal value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(double value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(int value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(long value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(object value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(float value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteValue(string value)
		{
			this._writer.WriteValue(value);
		}

		public override void WriteWhitespace(string ws)
		{
			this._writer.WriteWhitespace(ws);
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				return this._writer.Settings;
			}
		}

		protected XmlWriter Writer
		{
			get
			{
				return this._writer;
			}
			set
			{
				this._writer = value;
			}
		}

		public override System.Xml.WriteState WriteState
		{
			get
			{
				return this._writer.WriteState;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this._writer.XmlLang;
			}
		}

		public override System.Xml.XmlSpace XmlSpace
		{
			get
			{
				return this._writer.XmlSpace;
			}
		}
	}
}