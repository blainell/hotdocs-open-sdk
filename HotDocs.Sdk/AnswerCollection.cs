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
        private Dictionary<string, Answer>[] _answers;
        private float _version;
        private string _filePath = "";

        public event EventHandler<AnswerChangedEventArgs> AnswerChanged;

        /// <summary>
        /// Default AnswerCollection constructor
        /// </summary>
        public AnswerCollection()
        {
            Clear();
        }

        public Dictionary<string, Answer> GetAnswerBucket(ValueType T)
        {
            int valTypeInt = (int)T;

            if (_answers[valTypeInt] == null)
                _answers[valTypeInt] = new Dictionary<string, Answer>();

            return _answers[valTypeInt];
        }

        /// <summary>
        /// Resets the answer collection. All answers are deleted, and the title is set to an empty string.
        /// </summary>
        public virtual void Clear()
        {
            int valueTypesCount = Enum.GetNames(typeof(ValueType)).Length;
            _answers = new Dictionary<string, Answer>[valueTypesCount];
            Title = String.Empty;
            _version = 1.1F;
            DTD = null;
        }

        public void OnAnswerChanged(Answer ans, int[] indices, ValueChangeType changeType)
        {
            if (AnswerChanged != null)
                AnswerChanged(ans, new AnswerChangedEventArgs(ans != null ? ans.Name : null, indices, changeType));
        }

        /// <summary>
        /// The title of the answer collection. When the answer collection is serialized as XML, the title appears as an attribute of the root AnswerSet node.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The version number of the answer collection. When the answer collection is serialized as XML, this version number appears as an attribute of the root AnswerSet node.
        /// </summary>
        public double Version
        {
            get { return _version; }
        }

        public string DTD { get; set; }

        /// <summary>
        /// The number of answers in the answer collection. Because HotDocs variables may be repeated, each of these answers may actually contain more than one value.
        /// </summary>
        public int AnswerCount
        {
            get
            {
                int result = 0;
                foreach (Dictionary<string, Answer> answerBucket in _answers)
                {
                    if (answerBucket != null)
                        result += answerBucket.Count;
                }
                return result;
            }
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
        public bool TryGetAnswer(string name, ValueType T, out Answer answer)
        {
            return GetAnswerBucket(T).TryGetValue(name, out answer);
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
            Dictionary<string, Answer> bucket = GetAnswerBucket(ans.Type);

            // If there is already an answer in this bucket, remove it.
            // The answer name must be unique within each bucket.
            if (bucket.ContainsKey(name))
                bucket.Remove(name);

            bucket.Add(name, ans);
            return ans;
        }

        /// <summary>
        /// Removes an answer from the answer collection.
        /// </summary>
        /// <param name="name">The name of the HotDocs variable whose answer you want to remove from the answer collection.</param>
        /// <returns>True if the answer collection contained an answer for the specified variable and it was successfully removed;
        /// otherwise, returns False.
        /// </returns>
        public bool RemoveAnswer(string name, ValueType T)
        {
            return GetAnswerBucket(T).Remove(name);
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
        /// <param name="path">The path of the answer file to write.</param>
        /// <param name="writeDontSave">Indicates whether or not answers that are marked as "do not save" should be written to the answer file.</param>
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
        /// <param name="writeDontSave">Indicates whether or not answers that are marked as "do not save" should be written to the answer file.</param>
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
                                Title = reader.Value;
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
                string s;
                switch (reader.Name)
                {
                    case "TextValue":
                        if (ans == null)
                            ans = CreateAnswer<TextValue>(answerName);
                        s = reader.ReadElementContentAsString(reader.Name, "");
                        if (unans || string.IsNullOrEmpty(s))
                            ans.InitValue<TextValue>(userModifiable ? TextValue.Unanswered : TextValue.UnansweredLocked, repeatStack);
                        else
                            ans.InitValue<TextValue>(new TextValue(s, userModifiable), repeatStack);
                        break;
                    case "NumValue":
                        if (ans == null)
                            ans = CreateAnswer<NumberValue>(answerName);
                        s = reader.ReadElementContentAsString(reader.Name, "");
                        if (unans || string.IsNullOrEmpty(s))
                            ans.InitValue<NumberValue>(userModifiable ? NumberValue.Unanswered : NumberValue.UnansweredLocked, repeatStack);
                        else
                            ans.InitValue<NumberValue>(new NumberValue(XmlConvert.ToDouble(s), userModifiable), repeatStack);
                        break;
                    case "DateValue":
                        if (ans == null)
                            ans = CreateAnswer<DateValue>(answerName);
                        s = reader.ReadElementContentAsString(reader.Name, "");
                        if (unans || string.IsNullOrEmpty(s))
                            ans.InitValue<DateValue>(userModifiable ? DateValue.Unanswered : DateValue.UnansweredLocked, repeatStack);
                        else
                        {
                            string[] dateParts = s.Split('/', ' ', '-', '.');
                            ans.InitValue<DateValue>(
                                new DateValue(
                                    Convert.ToInt32(dateParts[2]),
                                    Convert.ToInt32(dateParts[1]),
                                    Convert.ToInt32(dateParts[0]),
                                    userModifiable
                                ), repeatStack);
                        }
                        break;
                    case "TFValue":
                        if (ans == null)
                            ans = CreateAnswer<TrueFalseValue>(answerName);
                        s = reader.ReadElementContentAsString(reader.Name, "");
                        if (unans || string.IsNullOrEmpty(s))
                            ans.InitValue<TrueFalseValue>(userModifiable ? TrueFalseValue.Unanswered : TrueFalseValue.UnansweredLocked, repeatStack);
                        else
                        {
                            // LRS: although the value for a TFValue element should be lower case,
                            // The following line forces the conversion to lower case explicitly
                            // because JavaScript and Silverlight have historically (and incorrectly) set these
                            // values using upper case instead.  (Bug 4429)
                            ans.InitValue<TrueFalseValue>(new TrueFalseValue(XmlConvert.ToBoolean(s.ToLowerInvariant()), userModifiable), repeatStack);
                        }
                        break;
                    case "MCValue":
                        if (ans == null)
                            ans = CreateAnswer<MultipleChoiceValue>(answerName);
                        if (unans || reader.IsEmptyElement)
                        {
                            ans.InitValue<MultipleChoiceValue>(userModifiable ? MultipleChoiceValue.Unanswered : MultipleChoiceValue.UnansweredLocked, repeatStack);
                            reader.ReadInnerXml(); // read past the element and ignore it
                        }
                        else
                        {
                            // the MCValue claims to contain one or more SelValues
                            List<String> mcVals = new List<string>();
                            bool selValueUnans; // The fact that SelValue can have an unans attribute is against the official schema, but
                                                // some HotDocs code apparently wasn't aware of that. :-)
                            reader.ReadStartElement("MCValue");
                            reader.MoveToContent();
                            while (reader.Name == "SelValue")
                            {
                                selValueUnans = false;
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "unans")
                                        {
                                            selValueUnans = XmlConvert.ToBoolean(reader.Value);
                                            break;
                                        }
                                    }
                                    reader.MoveToElement();
                                }
                                s = reader.ReadElementContentAsString(reader.Name, "");
                                if (!selValueUnans
                                    && (!string.IsNullOrEmpty(s)
                                        || (s != null && mcVals.Count == 0)  // allow at most a single empty SelValue
                                        )
                                    )
                                {
                                    mcVals.Add(s);
                                }
                                reader.MoveToContent(); // skip whitespace before next element
                            }
                            if (mcVals.Count == 0) // as good as unanswered
                                ans.InitValue<MultipleChoiceValue>(userModifiable ? MultipleChoiceValue.Unanswered : MultipleChoiceValue.UnansweredLocked, repeatStack);
                            else
                                ans.InitValue<MultipleChoiceValue>(new MultipleChoiceValue(mcVals.ToArray(), userModifiable), repeatStack);
                            reader.ReadEndElement(); // read past MCValue end element
                        }
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

        public string GetXMLString(bool includeBOM, bool includeTransientAnswers)
        {
            StringBuilder result = new StringBuilder();

            if (includeBOM)
                result.Append('\xFEFF');

            using (var writer = new System.IO.StringWriter(result))
            {
                WriteXml(writer, includeTransientAnswers);
            }
            return result.ToString();
        }

        /// <summary>
        /// Writes the answer collection as a HotDocs XML answer file to the output stream.
        /// </summary>
        /// <param name="output">The stream to which to write the XML answer file.</param>
        /// <param name="writeDontSave">Indicates whether or not answers that are marked as "do not save" should be written to the answer file.</param>
        public void WriteXml(System.IO.Stream output, bool writeDontSave)
        {
            WriteXml(output, writeDontSave, true);
        }

        /// <summary>
        /// Writes the answer collection as a HotDocs XML answer file to the output stream.
        /// </summary>
        /// <param name="output">The stream to which to write the XML answer file.></param>
        /// <param name="writeDontSave">Indicates whether or not answers that are marked as "do not save" should be written to the answer file.</param>
        /// <param name="closeStream">Indicates whether the stream should be closed upon completion.</param>
        public void WriteXml(System.IO.Stream output, bool writeDontSave, bool closeStream)
        {
            var writer = new System.IO.StreamWriter(output, Encoding.UTF8);
            try
            {
                WriteXml(writer, writeDontSave);
            }
            finally
            {
                if (closeStream)
                {
                    writer.Close();
                }
                else
                {
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Writes the answer collection as a HotDocs XML answer file to the TextWriter.
        /// </summary>
        /// <param name="output">The TextWriter to which to write the XML answer file.</param>
        /// <param name="writeDontSave">Indicates whether or not answers that are marked as "do not save" should be written to the answer file.</param>
        public void WriteXml(System.IO.TextWriter output, bool writeDontSave)
        {
            output.Write("<?xml version=\"1.0\" encoding=\"");
            output.Write(output.Encoding.WebName);//Write out the IANA-registered name of the encoding.
            output.WriteLine("\" standalone=\"yes\"?>");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.OmitXmlDeclaration = true; // because we emitted it manually above
            bool forInterview = true; // ensures the userModifiable and userExtendible attributes are output
            using (XmlWriter writer = new AnswerXmlWriter(output, settings, forInterview))
            {
                writer.WriteStartDocument(true);
                if (!String.IsNullOrEmpty(DTD))
                    writer.WriteRaw(DTD);
                writer.WriteStartElement("AnswerSet");
                writer.WriteAttributeString("title", Title);
                writer.WriteAttributeString("version", XmlConvert.ToString(_version));
                IEnumerator<Answer> answerEnumerator = GetEnumerator();
                while (answerEnumerator.MoveNext())
                    answerEnumerator.Current.WriteXml(writer, writeDontSave);
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
            foreach (Dictionary<string, Answer> answerBucket in _answers)
            {
                if (answerBucket != null)
                {
                    foreach (Answer answer in answerBucket.Values)
                        yield return answer;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public enum ValueChangeType { None, Changed, BecameAnswered, BecameUnanswered, IndexShift }

    public class AnswerChangedEventArgs : EventArgs
    {
        private readonly string _variableName;
        private readonly int[] _indices;
        private readonly ValueChangeType _changeType;

        public string VariableName { get { return _variableName; } }
        public int[] Indices { get { return _indices; } }
        public ValueChangeType ChangeType { get { return _changeType; } }

        public AnswerChangedEventArgs(string variableName, int[] indices, ValueChangeType changeType)
        {
            _variableName = variableName;
            _indices = indices;
            _changeType = changeType;
        }
    }

}
