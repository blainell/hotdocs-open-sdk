/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.IO;
using System.Text;

namespace HotDocs.Sdk
{
    /// <summary>
    ///     Represents the answers returned from a HotDocs interview.
    /// </summary>
    public class InterviewAnswerSet : AnswerCollection
    {
        private string _originalAnswers; // base64-encoded, encrypted answers being round-tripped back to the server

        /// <summary>
        ///     Clears the answers.
        /// </summary>
        public override void Clear()
        {
            _originalAnswers = "";
            base.Clear();
        }

        /// <summary>
        ///     Decodes answers from an interview passed as a string.
        /// </summary>
        /// <param name="input">String that contains answers to decode.</param>
        public void DecodeInterviewAnswers(string input)
        {
            using (var rdr = new StringReader(input))
            {
                DecodeInterviewAnswers(rdr);
            }
        }

        /// <summary>
        ///     Decodes answers from an interview passed as a stream.
        /// </summary>
        /// <param name="input">Stream that contains answers to decode.</param>
        public void DecodeInterviewAnswers(Stream input)
        {
            using (var rdr = new StreamReader(input, Encoding.UTF8, true))
            {
                DecodeInterviewAnswers(rdr);
            }
        }

        /// <summary>
        ///     Decodes answers from an interview passed as a TextReader.
        /// </summary>
        /// <param name="input">TextReader that contains answers to decode.</param>
        public static TextReader GetDecodedInterviewAnswers(TextReader input)
        {
            string originalAnswers = null;
            return GetDecodedInterviewAnswers(input, ref originalAnswers);
        }

        private static TextReader GetDecodedInterviewAnswers(TextReader input, ref string originalAnswers)
        {
            var firstc = input.Peek();
            if (firstc != -1)
            {
                var first = (char) firstc;
                if (first == '[') // looks like answers from stateless interview (HDANS format)
                {
                    // answers probably start with "[HDANS(" etc.
                    var ansPkg = input.ReadToEnd();
                    if (ansPkg.StartsWith("[HDSANS(", StringComparison.OrdinalIgnoreCase))
                    {
                        var end = ansPkg.IndexOf(")]", 8);
                        if (end > 0)
                        {
                            var lens = ansPkg.Substring(8, end - 8).Split(',');
                            if (lens.Length == 2)
                            {
                                var ansLen = int.Parse(lens[0]);
                                var orgLen = int.Parse(lens[1]);
                                if (ansPkg[end + 2 + ansLen] == '|')
                                {
                                    if (orgLen > 0 && originalAnswers != null)
                                        originalAnswers = ansPkg.Substring(end + ansLen + 3, orgLen);

                                    // decode & then return XML
                                    ansPkg =
                                        Encoding.UTF8.GetString(
                                            Convert.FromBase64String(ansPkg.Substring(end + 2, ansLen)));
                                    if (ansPkg[0] == 0xFEFF)
                                        ansPkg = ansPkg.Substring(1);
                                    return new StringReader(ansPkg);
                                }
                            }
                        }
                    }
                    // else
                    throw new ArgumentException("Error parsing interview answers.");
                }
                if (first == '<') // looks like bare XML answers
                {
                    // answers probably start with "<?xml "
                    return input;
                }
                var buffer = Convert.FromBase64String(input.ReadToEnd());
                var decoded = Encoding.Unicode.GetString(buffer); //This was "Unicode" (not "UTF8").
                if (decoded.Length > 0 && decoded[0] == '\xfeff')
                    decoded = decoded.Substring(1);
                return new StringReader(decoded);
            }
            return new StringReader("");
        }

        /// <summary>
        ///     Decodes answers from an interview passed as a TextReader.
        /// </summary>
        /// <param name="input">TextReader that contains answers to decode.</param>
        public void DecodeInterviewAnswers(TextReader input)
        {
            Clear();
            var decoded = GetDecodedInterviewAnswers(input, ref _originalAnswers);
            OverlayXml(decoded);
        }

        /// <summary>
        ///     Encodes interview answers.
        /// </summary>
        /// <returns>A string of encoded answers.</returns>
        public string EncodeInterviewAnswers()
        {
            if (string.IsNullOrEmpty(_originalAnswers)) // stateful interview -- no "original" answers to overlay
            {
                // output legacy (stateful) answer set -- Base64/UTF16 encoded
                return Convert.ToBase64String(Encoding.Unicode.GetBytes(XmlAnswers), Base64FormattingOptions.None);
            }
            // output stateless answer format -- Base64/UTF8 with original (encrypted) answers tagging along
            var output = new StringBuilder();
            var encAns = Convert.ToBase64String(Encoding.UTF8.GetBytes(XmlAnswers), Base64FormattingOptions.None);
            output.AppendFormat("[HDSANS({0:D},{1:D})]", encAns.Length, _originalAnswers.Length);
            output.Append(encAns);
            output.Append("|");
            output.Append(_originalAnswers);
            return output.ToString();
        }

        /// <summary>
        ///     Encodes interview answers and outputs them to a stream.
        /// </summary>
        /// <param name="output">Stream to which to output the answers.</param>
        public void EncodeInterviewAnswers(Stream output)
        {
            using (var writer = new StreamWriter(output, Encoding.UTF8))
            {
                EncodeInterviewAnswers(writer);
            }
        }

        /// <summary>
        ///     Encodes interview answwers and outputs them to a TextWriter.
        /// </summary>
        /// <param name="output">TextWriter to which to output the answers.</param>
        public void EncodeInterviewAnswers(TextWriter output)
        {
            if (string.IsNullOrEmpty(_originalAnswers)) // stateful interview -- no "original" answers to overlay
            {
                // output legacy (stateful) answer set -- Base64/UTF16 encoded
                output.Write(Convert.ToBase64String(Encoding.Unicode.GetBytes(XmlAnswers), Base64FormattingOptions.None));
            }
            else // stateless interview
            {
                // output stateless answer format -- Base64/UTF8 with original (encrypted) answers tagging along
                var encAns = Convert.ToBase64String(Encoding.UTF8.GetBytes(XmlAnswers), Base64FormattingOptions.None);
                output.Write("[HDSANS({0:D},{1:D})]", encAns.Length, _originalAnswers.Length);
                output.Write(encAns);
                output.Write("|");
                output.Write(_originalAnswers);
            }
        }
    }
}