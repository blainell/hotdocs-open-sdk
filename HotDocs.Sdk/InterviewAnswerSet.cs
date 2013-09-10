/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server
{
	/// <summary>
	/// Represents the answers returned from a HotDocs interview.
	/// </summary>
	public class InterviewAnswerSet : HotDocs.Sdk.AnswerCollection
	{
		private string _originalAnswers; // base64-encoded, encrypted answers being round-tripped back to the server

		/// <summary>
		/// Constructor for InterviewAnswerSet.
		/// </summary>
		public InterviewAnswerSet()
			: base()
		{
		}

		/// <summary>
		/// Clears the answers.
		/// </summary>
		public override void Clear()
		{
			_originalAnswers = "";
			base.Clear();
		}

		/// <summary>
		/// Decodes answers from an interview passed as a string.
		/// </summary>
		/// <param name="input">String that contains answers to decode.</param>
		public void DecodeInterviewAnswers(string input)
		{
			using (var rdr = new System.IO.StringReader(input))
			{
				DecodeInterviewAnswers(rdr);
			}
		}

		/// <summary>
		/// Decodes answers from an interview passed as a stream.
		/// </summary>
		/// <param name="input">Stream that contains answers to decode.</param>
		public void DecodeInterviewAnswers(System.IO.Stream input)
		{
			using (var rdr = new System.IO.StreamReader(input, Encoding.UTF8, true))
			{
				DecodeInterviewAnswers(rdr);
			}
		}

		/// <summary>
		/// Decodes answers from an interview passed as a TextReader.
		/// </summary>
		/// <param name="input">TextReader that contains answers to decode.</param>
		public static System.IO.TextReader GetDecodedInterviewAnswers(System.IO.TextReader input)
		{
			string originalAnswers = null;
			return GetDecodedInterviewAnswers(input, ref originalAnswers);
		}
		private static System.IO.TextReader GetDecodedInterviewAnswers(System.IO.TextReader input, ref string originalAnswers)
		{
			int firstc = input.Peek();
			if (firstc != -1)
			{
				char first = (char)firstc;
				if (first == '[') // looks like answers from stateless interview (HDANS format)
				{
					// answers probably start with "[HDANS(" etc.
					string ansPkg = input.ReadToEnd();
					if (ansPkg.StartsWith("[HDSANS(", StringComparison.OrdinalIgnoreCase))
					{
						int end = ansPkg.IndexOf(")]", 8);
						if (end > 0)
						{
							var lens = ansPkg.Substring(8, end - 8).Split(',');
							if (lens.Length == 2)
							{
								int ansLen = int.Parse(lens[0]);
								int orgLen = int.Parse(lens[1]);
								if (ansPkg[end + 2 + ansLen] == '|')
								{
									if (orgLen > 0 && originalAnswers != null)
										originalAnswers = ansPkg.Substring(end + ansLen + 3, orgLen);
									// decode & then return XML
									ansPkg = Encoding.UTF8.GetString(Convert.FromBase64String(ansPkg.Substring(end + 2, ansLen)));
									if (ansPkg[0] == 0xFEFF)
										ansPkg = ansPkg.Substring(1);
									return new System.IO.StringReader(ansPkg);
								}
							}
						}
					}
					// else
					throw new ArgumentException("Error parsing interview answers.");
				}
				else if (first == '<') // looks like bare XML answers
				{
					// answers probably start with "<?xml "
					return input;
				}
				else // otherwise should be base64 encoded UTF16 XML
				{
					byte [] buffer = Convert.FromBase64String(input.ReadToEnd());
					string decoded = Encoding.Unicode.GetString(buffer);//This was "Unicode" (not "UTF8").
					if (decoded.Length > 0 && decoded[0] == '\xfeff')
						decoded = decoded.Substring(1);
					return new System.IO.StringReader(decoded);
				}
			}
			return new System.IO.StringReader("");
		}

		/// <summary>
		/// Decodes answers from an interview passed as a TextReader.
		/// </summary>
		/// <param name="input">TextReader that contains answers to decode.</param>
		public void DecodeInterviewAnswers(System.IO.TextReader input)
		{
			System.IO.TextReader decoded = GetDecodedInterviewAnswers(input, ref _originalAnswers);
			ReadXml(decoded);
		}

		/// <summary>
		/// Encodes interview answers.
		/// </summary>
		/// <returns>A string of encoded answers.</returns>
		public string EncodeInterviewAnswers()
		{
			if (String.IsNullOrEmpty(_originalAnswers)) // stateful interview -- no "original" answers to overlay
			{
				// output legacy (stateful) answer set -- Base64/UTF16 encoded
				return Convert.ToBase64String(Encoding.Unicode.GetBytes(XmlAnswers), Base64FormattingOptions.None);
			}
			else // stateless interview
			{
				// output stateless answer format -- Base64/UTF8 with original (encrypted) answers tagging along
				StringBuilder output = new StringBuilder();
				string encAns = Convert.ToBase64String(Encoding.UTF8.GetBytes(XmlAnswers), Base64FormattingOptions.None);
				output.AppendFormat("[HDSANS({0:D},{1:D})]", encAns.Length, _originalAnswers.Length);
				output.Append(encAns);
				output.Append("|");
				output.Append(_originalAnswers);
				return output.ToString();
			}
		}

		/// <summary>
		/// Encodes interview answers and outputs them to a stream.
		/// </summary>
		/// <param name="output">Stream to which to output the answers.</param>
		public void EncodeInterviewAnswers(System.IO.Stream output)
		{
			using (var writer = new System.IO.StreamWriter(output, Encoding.UTF8))
			{
				EncodeInterviewAnswers(writer);
			}
		}

		/// <summary>
		/// Encodes interview answwers and outputs them to a TextWriter.
		/// </summary>
		/// <param name="output">TextWriter to which to output the answers.</param>
		public void EncodeInterviewAnswers(System.IO.TextWriter output)
		{
			if (String.IsNullOrEmpty(_originalAnswers)) // stateful interview -- no "original" answers to overlay
			{
				// output legacy (stateful) answer set -- Base64/UTF16 encoded
				output.Write(Convert.ToBase64String(Encoding.Unicode.GetBytes(XmlAnswers), Base64FormattingOptions.None));
			}
			else // stateless interview
			{
				// output stateless answer format -- Base64/UTF8 with original (encrypted) answers tagging along
				string encAns = Convert.ToBase64String(Encoding.UTF8.GetBytes(XmlAnswers), Base64FormattingOptions.None);
				output.Write("[HDSANS({0:D},{1:D})]", encAns.Length, _originalAnswers.Length);
				output.Write(encAns);
				output.Write("|");
				output.Write(_originalAnswers);
			}
		}
	}
}
