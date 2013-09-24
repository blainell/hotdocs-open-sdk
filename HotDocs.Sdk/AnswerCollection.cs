/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace HotDocs.Sdk
{
	/// <summary>
	/// A collection of HotDocs answers.
	/// </summary>
	public class AnswerCollection : IEnumerable<Answer>
	{
		private Dictionary<string, Answer> _answers;
		private string _title;
		private float _version;
		private string _filePath = "";

		/// <summary>
		/// Default AnswerCollection constructor
		/// </summary>
		public AnswerCollection()
		{
			Clear();
		}

		/// <summary>
		/// Resets the answer collection. All answers are deleted, and the title is set to an empty string.
		/// </summary>
		public virtual void Clear()
		{
			_answers = new Dictionary<string, Answer>();
			_title = String.Empty;
			_version = 1.1F;
		}

		/// <summary>
		/// The title of the answer collection. When the answer collection is serialized as XML, the title appears as an attribute of the root AnswerSet node.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		/// <summary>
		/// The version number of the answer collection. When the answer collection is serialized as XML, this version number appears as an attribute of the root AnswerSet node.
		/// </summary>
		public double Version
		{
			get { return _version; }
		}

		/// <summary>
		/// The number of answers in the answer collection. Because HotDocs variables may be repeated, each of these answers may actually contain more than one value.
		/// </summary>
		public int AnswerCount
		{
			get { return _answers.Count; }
		}

		/// <summary>
		/// An XML string representation of the entire answer collection.
		/// </summary>
		public string XmlAnswers
		{
			get
			{
				StringBuilder result = new StringBuilder();
				using (var writer = new System.IO.StringWriter(result))
				{
					WriteXml(writer, true);
				}
				return result.ToString();
			}
		}

		/// <summary>
		/// Gets the answer for the specified HotDocs variable if it exists in the answer collection.
		/// </summary>
		/// <param name="name">The name of the HotDocs variable whose answer you want to retrieve.</param>
		/// <param name="answer">The Answer for the requested variable (if it exists).</param>
		/// <returns>True if the answer collection contains an answer for the specified variable; otherwise, returns False.</returns>
		public bool TryGetAnswer(string name, out Answer answer)
		{
			return _answers.TryGetValue(name, out answer);
		}

		/// <summary>
		/// Creates a new answer.
		/// </summary>
		/// <typeparam name="T">The type of answer to create.</typeparam>
		/// <param name="name">The name of the answer.</param>
		/// <returns>An answer.</returns>
		public Answer CreateAnswer<T>(string name) where T : IValue
		{
			Answer ans = Answer.Create<T>(this, name);
			_answers[name] = ans;
			return ans;
		}

		/// <summary>
		/// Removes an answer from the answer collection.
		/// </summary>
		/// <param name="name">The name of the HotDocs variable whose answer you want to remove from the answer collection.</param>
		/// <returns>True if the answer collection contained an answer for the specified variable and it was successfully removed;
		/// otherwise, returns False.
		/// </returns>
		public bool RemoveAnswer(string name)
		{
			return _answers.Remove(name);
		}

		/// <summary>
		/// Read an answer file into this answer collection. Store the answer file name in the FilePath property.
		/// Currently, the only supported file format is XML.
		/// </summary>
		/// <param name="path">The file path to an XML answer file.</param>
		public void ReadFile(string path)
		{
			using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
			{
				ReadXml(fs);
			}
			_filePath = path;
		}

		/// <summary>
		/// Reads an answer file from a stream. Since it comes from a stream, the answer file name is empty.
		/// </summary>
		/// <param name="fileStream">A stream containing an XML answer file.</param>
		public void ReadFile(Stream fileStream)
		{
			ReadXml(fileStream);
			_filePath = string.Empty;
		}

		/// <summary>
		/// Write this answer collection to a file. Store the answer file name in the FilePath property.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="writeDontSave"></param>
		public void WriteFile(string path, bool writeDontSave)
		{
			using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
			{
				WriteXml(fs, writeDontSave);
			}
			_filePath = path;
		}

		/// <summary>
		/// Write this answer collection to the file designated by the FilePath property.
		/// </summary>
		/// <param name="writeDontSave"></param>
		public void WriteFile(bool writeDontSave)
		{
			WriteFile(_filePath, writeDontSave);
		}

		/// <summary>
		/// Access the path of the file last read from by ReadFile(path) or written to by WriteFile(path).
		/// </summary>
		public string FilePath
		{
			get
			{
				return _filePath;
			}
		}

		/// <summary>
		/// Reads an XML answer file into the answer collection.
		/// </summary>
		/// <param name="input">A string of XML containing a HotDocs XML answer file.</param>
		public void ReadXml(string input)
		{
			using (var rdr = new System.IO.StringReader(input))
			{
				ReadXml(rdr);
			}
		}

		/// <summary>
		/// Reads a stream into the answer collection.
		/// </summary>
		/// <param name="input">A stream containing a HotDocs answer file.</param>
		public void ReadXml(System.IO.Stream input)
		{
			using (var rdr = new System.IO.StreamReader(input, true))
			{
				ReadXml(rdr);
			}
		}

		/// <summary>
		/// Reads a TextReader into the answer collection.
		/// </summary>
		/// <param name="input">A TextReader containing a HotDocs answer file, clearing existing answers.</param>
		public void ReadXml(System.IO.TextReader input)
		{
			Clear();
			OverlayXml(input);
		}

		/// <summary>
		/// Reads a TextReader into the answer collection without first clearing existing answers.
		/// </summary>
		/// <param name="input">A TextReader containing a HotDocs answer file.</param>
		public void OverlayXml(System.IO.TextReader input)
		{
			// create an XmlTextReader
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Auto;
			settings.IgnoreWhitespace = false;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreComments = true;
			settings.ValidationType = ValidationType.None;
			//settings.ProhibitDtd = false; // obsolete in .NET 4.0, replaced with:
			settings.DtdProcessing = DtdProcessing.Ignore; // .NET 4.0 only
			using (XmlReader reader = XmlReader.Create(input, settings))
			{
				// then read the XML and create the answers...
				reader.MoveToContent();
				if (reader.Name == "AnswerSet")
				{
					if (reader.HasAttributes)
					{
						while (reader.MoveToNextAttribute())
						{
							if (reader.Name == "title")
								_title = reader.Value;
							else if (reader.Name == "version")
								_version = XmlConvert.ToSingle(reader.Value);
						}
					}
				}
				else
					throw new XmlException("Expected an AnswerSet element.");

				int[] repeatStack = RepeatIndices.Empty;

				// read answers:
				while (reader.Read())
				{
					reader.MoveToContent();
					if (reader.Name == "Answer" && reader.HasAttributes)
					{
						string answerName = null;
						bool? answerSave = null;
						bool? userExtendible = null;
						while (reader.MoveToNextAttribute())
						{
							if (reader.Name == "name")
							{
								answerName = TextValue.XMLUnescape(reader.Value).Trim();
							}
							else if (reader.Name == "save")
							{
								switch (reader.Value)
								{
									case "true":
										answerSave = true;
										break;
									case "false":
										answerSave = false;
										break;
									// else no change
								}
							}
							else if (reader.Name == "userExtendible")
							{
								userExtendible = XmlConvert.ToBoolean(reader.Value);
							}
						}
						if (String.IsNullOrEmpty(answerName))
							throw new XmlException("Answer name is missing.");

						Answer ans = null;

						reader.Read();
						reader.MoveToContent();
						ReadValue(reader, ref ans, answerName, repeatStack);

						if (answerSave.HasValue && !answerSave.Value)
							ans.Save = false;
						if (userExtendible.HasValue)
							ans.UserExtendible = userExtendible.Value;
					}
				}
			}
		}

		private void ReadValue(XmlReader reader, ref Answer ans, string answerName, int[] repeatStack)
		{
			if (reader.Name == "RptValue")
			{
				if (!reader.IsEmptyElement)
				{
					repeatStack = RepeatIndices.Push(repeatStack);
					reader.Read(); // read past RptValue element
					reader.MoveToContent();
					while (reader.Name != "RptValue" || reader.NodeType != XmlNodeType.EndElement)
					{
						ReadValue(reader, ref ans, answerName, repeatStack);
						RepeatIndices.Increment(repeatStack);
					}
					reader.ReadEndElement();
					reader.MoveToContent();
				}
				else
				{
					reader.Read(); // just read past empty element
					reader.MoveToContent();
				}
			}
			else // scalar value
			{
				// check for unanswered/user modifiable attributes
				bool unans = false;
				bool userModifiable = true;
				if (reader.HasAttributes)
				{
					while (reader.MoveToNextAttribute())
					{
						if (reader.Name == "unans")
						{
							unans = XmlConvert.ToBoolean(reader.Value);
						}
						else if (reader.Name == "userModifiable")
						{
							userModifiable = XmlConvert.ToBoolean(reader.Value);
						}
					}
					reader.MoveToElement();
				}
				// get value
				switch (reader.Name)
				{
					case "TextValue":
						if (ans == null)
							ans = CreateAnswer<TextValue>(answerName);
						if (unans)
							ans.InitValue<TextValue>(userModifiable ? TextValue.Unanswered : TextValue.UnansweredLocked, repeatStack);
						else
						{
							reader.ReadStartElement("TextValue");
							ans.InitValue<TextValue>(new TextValue(reader.Value, userModifiable), repeatStack);
							reader.Read(); // move past text element
						}
						reader.Read(); // read past TextValue element
						reader.MoveToContent();
						break;
					case "NumValue":
						if (ans == null)
							ans = CreateAnswer<NumberValue>(answerName);
						if (unans)
							ans.InitValue<NumberValue>(userModifiable ? NumberValue.Unanswered : NumberValue.UnansweredLocked, repeatStack);
						else
						{
							reader.ReadStartElement("NumValue");
							ans.InitValue<NumberValue>(new NumberValue(XmlConvert.ToDouble(reader.Value), userModifiable), repeatStack);
							reader.Read(); // move past text element
						}
						reader.Read(); // read past NumValue element
						reader.MoveToContent();
						break;
					case "DateValue":
						if (ans == null)
							ans = CreateAnswer<DateValue>(answerName);
						if (unans)
							ans.InitValue<DateValue>(userModifiable ? DateValue.Unanswered : DateValue.UnansweredLocked, repeatStack);
						else
						{
							reader.ReadStartElement("DateValue");
							string[] dateParts = reader.Value.Split('/', ' ', '-', '.');
							ans.InitValue<DateValue>(new DateValue(
								Convert.ToInt32(dateParts[2]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[0]), userModifiable
								), repeatStack);
							reader.Read(); // move past text element
						}
						reader.Read(); // read past DateValue element
						reader.MoveToContent();
						break;
					case "TFValue":
						if (ans == null)
							ans = CreateAnswer<TrueFalseValue>(answerName);
						if (unans)
							ans.InitValue<TrueFalseValue>(userModifiable ? TrueFalseValue.Unanswered : TrueFalseValue.UnansweredLocked, repeatStack);
						else
						{
							reader.ReadStartElement("TFValue");
							// LRS: although the value for a TFValue element should be lower case,
							// The following line forces the conversion to lower case explicitly
							// because JavaScript and Silverlight have historically (and incorrectly) set these
							// values using upper case instead.  (TFS 4429)
							ans.InitValue<TrueFalseValue>(new TrueFalseValue(XmlConvert.ToBoolean(reader.Value.ToLowerInvariant()), userModifiable), repeatStack);
							reader.Read(); // move past text element
						}
						reader.Read(); // read past TFValue element
						reader.MoveToContent();
						break;
					case "MCValue":
						if (ans == null)
							ans = CreateAnswer<MultipleChoiceValue>(answerName);
						if (unans)
							ans.InitValue<MultipleChoiceValue>(userModifiable ? MultipleChoiceValue.Unanswered : MultipleChoiceValue.UnansweredLocked, repeatStack);
						else
						{
							List<String> mcVals = new List<string>();
							reader.ReadStartElement("MCValue");
							reader.MoveToContent();
							while (reader.Name == "SelValue")
							{
								reader.ReadStartElement("SelValue");
								mcVals.Add(reader.Value);
								reader.Read(); // skip past value
								reader.ReadEndElement();
								reader.MoveToContent();
							}
							ans.InitValue<MultipleChoiceValue>(new MultipleChoiceValue(mcVals.ToArray(), userModifiable), repeatStack);
						}
						reader.Read(); // read past MCValue element
						reader.MoveToContent();
						break;
					default: // anything else -- database values, clause library values, document text (span) values
						if (ans == null)
							ans = CreateAnswer<UnknownValue>(answerName);
						ans.InitValue<UnknownValue>(new UnknownValue(reader.ReadOuterXml()), repeatStack);
						break;
				}
				reader.MoveToContent(); // skip any white space to move to the next element
			}
		}

		/// <summary>
		/// Writes the answer collection as a HotDocs XML answer file to the output stream.
		/// </summary>
		/// <param name="output">The stream to which to write the XML answer file.</param>
		/// <param name="writeDontSave"></param>
		public void WriteXml(System.IO.Stream output, bool writeDontSave)
		{
			using (var writer = new System.IO.StreamWriter(output, Encoding.UTF8))
			{
				WriteXml(writer, writeDontSave);
			}
		}

		/// <summary>
		/// Writes the answer collection as a HotDocs XML answer file to the TextWriter.
		/// </summary>
		/// <param name="output">The TextWriter to which to write the XML answer file.</param>
		/// <param name="writeDontSave"></param>
		public void WriteXml(System.IO.TextWriter output, bool writeDontSave)
		{
			output.Write("<?xml version=\"1.0\" encoding=\"");
			output.Write(output.Encoding.WebName);//Write out the IANA-registered name of the encoding.
			output.WriteLine("\" standalone=\"yes\"?>");
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			settings.OmitXmlDeclaration = true; // because we emitted it manually above
			bool forInterview = true;
			using (XmlWriter writer = new AnswerXmlWriter(output, settings, forInterview))
			{
				writer.WriteStartDocument(true);
				writer.WriteStartElement("AnswerSet");
				writer.WriteAttributeString("title", _title);
				writer.WriteAttributeString("version", XmlConvert.ToString(_version));
				//writer.WriteAttributeString("useMangledNames", XmlConvert.ToString(false));
				foreach (Answer ans in _answers.Values)
				{
					ans.WriteXml(writer, writeDontSave);
				}
				writer.WriteEndElement();
			}
		}

		#region IEnumerable<Answer> Members

		/// <summary>
		/// Gets an enumerator to iterate through the answers in the answer collection.
		/// </summary>
		/// <returns>An IEnumerator you can use to iterate the answers.</returns>
		public IEnumerator<Answer> GetEnumerator()
		{
			foreach (Answer answer in _answers.Values)
				yield return answer;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
