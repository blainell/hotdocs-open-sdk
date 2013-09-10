/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using HotDocs.Sdk;
using System.Configuration;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Server.Local
{
	internal class Assembly : AssemblyBase, IAssembly
	{
		internal Assembly(IAssemblyQueue parentAssemblyQueue) : base(parentAssemblyQueue)
		{
			Initialize(parentAssemblyQueue.ParentSession);
		}

		internal Assembly(ISession parentSession) : base(parentSession)
		{
			Initialize(parentSession);
		}

		private void Initialize(ISession session)
		{
			_interviewTempPath = CreateTempFolder(session);
		}

		#region IDisposable Members

		public override void Dispose()
		{
			base.Dispose();
			Util.SafeDeleteFolder(_interviewTempPath);
		}

		#endregion

		#region IAssembly Members

		public string GetInterview()
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();

			HotDocs.Server.Interop.interviewFormat format;
			switch (InterviewFormat)
			{
				case InterviewFormat.JavaScript:
					format = HotDocs.Server.Interop.interviewFormat.javascript;
					break;
				case InterviewFormat.Silverlight:
					format = HotDocs.Server.Interop.interviewFormat.Silverlight;
					break;
				case InterviewFormat.Unspecified:
					format = HotDocs.Server.Interop.interviewFormat.Unspecified;
					break;
				default:
					format = HotDocs.Server.Interop.interviewFormat.Unspecified;
					break;
			}

			HotDocs.Server.Interop.HDInterviewOptions options = HotDocs.Server.Interop.HDInterviewOptions.intOptNone;
			if ((_interviewOptions & InterviewOptions.NoPreview) != 0)
				options |= HotDocs.Server.Interop.HDInterviewOptions.intOptNoPreview;
			if ((_interviewOptions & InterviewOptions.NoSave) != 0)
				options |= HotDocs.Server.Interop.HDInterviewOptions.intOptNoSave;
			if ((_interviewOptions & InterviewOptions.OmitImages) != 0)
				options |= HotDocs.Server.Interop.HDInterviewOptions.intOptNoImages;

			StringBuilder interview;
			using (HotDocs.Server.AnswerCollection ansColl = new HotDocs.Server.AnswerCollection())
			{
				ansColl.XmlAnswers = _ansSet.XmlAnswers;

				interview = new StringBuilder(_app.GetInterview(
					FullTemplatePath,
					null,
					format,
					options,
					_serverFilesUrl,
					_styleUrl,
					ansColl,
					_formActionUrl,
					_templateTitle,
					_interviewDefinitionUrl,
					_interviewTempPath,
					_tempInterviewUrl,
					_saveAnswersPageUrl,
					_documentPreviewUrl));
			}
			Util.AppendSdkScriptBlock(interview, FullTemplatePath);
			return interview.ToString();
		}

		public byte[] GetInterviewDefinition(string templateFile)
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();
			HotDocs.Server.Interop.interviewFormat format = HotDocs.Server.Interop.interviewFormat.Unspecified;
			switch (_interviewFormat)
			{
				case InterviewFormat.Unspecified:
					format = HotDocs.Server.Interop.interviewFormat.Unspecified;
					break;
				case InterviewFormat.JavaScript:
					format = HotDocs.Server.Interop.interviewFormat.javascript;
					break;
				case InterviewFormat.Silverlight:
					format = HotDocs.Server.Interop.interviewFormat.Silverlight;
					break;
			}
			string path = _app.GetInterviewDefinitionFromTemplate(MakeFullTemplatePath(templateFile), null, format);
			return File.ReadAllBytes(path);
		}

		HotDocs.Server.OutputOptions GetOutputOptions(string inputExt, string outputExt)
		{
			if (IsPdfExtension(inputExt))
				return null;//We don't convert from PDF to anything else right now.

			HotDocs.Server.OutputOptions outputOptions = null;
			if (string.Compare(inputExt, outputExt, true) != 0)
			{
				if (string.Compare(outputExt, ".pdf", true) == 0)
				{
					HotDocs.Server.PdfOutputOptions pdfOpts = new HotDocs.Server.PdfOutputOptions();

					//Set whatever PDF output options are desired.

					outputOptions = pdfOpts;
				}
				else if (string.Compare(outputExt, ".html", true) == 0)
				{
					HotDocs.Server.HtmlOutputOptions htmOpts = new HotDocs.Server.HtmlOutputOptions();

					//Set whatever HTML output options are desired.

					outputOptions = htmOpts;
				}
				else if (string.Compare(outputExt, ".mhtml", true) == 0)
				{

				}
				else if (string.Compare(outputExt, ".txt", true) == 0)
				{
					HotDocs.Server.TextOutputOptions txtOpts = new HotDocs.Server.TextOutputOptions();

					//Set whatever text output options are desired.

					outputOptions = txtOpts;
				}
			}
			return outputOptions;
		}

		public void Assemble(OutputFormat formats)
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();

			//Just indicate which formats are allowed. For the local implementation, assembly is delayed
			// until the file is asked for.
			_formats = formats;

			//Clear the cache.
			_docCache.DeleteAllFiles();
		}

		public System.IO.Stream GetAssembledFile(OutputFormat format)
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();
			if ((format & _formats) == 0)
				return null;

			string extension = GetFormatExtension(format);
			string filePath;
			if (!_docCache.IsFileFormatAvailable(extension, out filePath))//If the file is not in the cache,
			{
				DoAssembly(format);//Get a file name and create the file.
				if (!_docCache.IsFileFormatAvailable(extension, out filePath))//If the file is not in the cache,
					throw new Exception("The requested output was not available after assembly.");
			}
			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
		}

		#endregion

		private OutputFormat _formats = 0;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Protected
		////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void DoAssembly(OutputFormat formats)
		{
			string fullTemplatePath = Path.Combine(((Session)ParentAssemblyQueue.ParentSession).TemplateFolderPath, _templatePath);
			string tplExt = Path.GetExtension(_templatePath).ToLower();
			string docFilePath;

			// If any output formats are being requested that have already been assembled, clear those flags.
			foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
			{
				if (format != OutputFormat.None && formats.HasFlag(format) && _docCache.IsFileFormatAvailable(GetFormatExtension(format), out docFilePath))
				{
					formats &= ~format;
				}
			}

			if (tplExt == ".hpt")
			{
				if ((formats & (OutputFormat.Native | OutputFormat.PDF | OutputFormat.HPD)) != 0)
				{
					using (HotDocs.Server.AnswerCollection ansColl = new HotDocs.Server.AnswerCollection())
					{
						ansColl.XmlAnswers = _ansSet.XmlAnswers;

						if ((formats & (OutputFormat.Native | OutputFormat.PDF)) != 0)
						{
							docFilePath = _docCache.GetDocFileName(".pdf");
							DoAssembly(
								fullTemplatePath,
								AssembleMarkupDocument ? HotDocs.Server.Interop.HDAssemblyOptions.asmOptMarkupView : HotDocs.Server.Interop.HDAssemblyOptions.asmOptNone,
								ansColl,
								docFilePath);
						}

						if (formats.HasFlag(OutputFormat.HPD))
						{
							docFilePath = _docCache.GetDocFileName(".hpd");
							DoAssembly(
								fullTemplatePath,
								AssembleMarkupDocument ? HotDocs.Server.Interop.HDAssemblyOptions.asmOptMarkupView : HotDocs.Server.Interop.HDAssemblyOptions.asmOptNone,
								ansColl,
								docFilePath);
						}
					}
				}
			}
			else
			{
				// We'll need the native format in order to convert to the requested format.
				if (!_docCache.IsFileFormatAvailable(DocumentExtension, out docFilePath))
				{
					using (HotDocs.Server.AnswerCollection ansColl = new HotDocs.Server.AnswerCollection())
					{
						ansColl.XmlAnswers = _ansSet.XmlAnswers;
						docFilePath = _docCache.GetDocFileName(DocumentExtension);
						DoAssembly(
							fullTemplatePath,
							AssembleMarkupDocument ? HotDocs.Server.Interop.HDAssemblyOptions.asmOptMarkupView : HotDocs.Server.Interop.HDAssemblyOptions.asmOptNone,
							ansColl,
							docFilePath);
					}
				}

				//Convert to each non-native format requested.
				if (tplExt == ".rtf" || tplExt == ".docx" || tplExt == ".ttx")
				{
					if (formats.HasFlag(OutputFormat.PDF))
					{
						string filePath = _docCache.GetDocFileName(".pdf");
						HotDocs.Server.OutputOptions outputOptions = GetOutputOptions(tplExt, ".pdf");
						HotDocs.Server.UtilityTools.ConvertFile(docFilePath, filePath, outputOptions);
					}
					if ((formats & (OutputFormat.HTML | OutputFormat.HTMLwDataURIs | OutputFormat.MHTML)) != 0)
					{
						string filePath = _docCache.GetDocFileName(".html");
						HotDocs.Server.OutputOptions outputOptions = GetOutputOptions(tplExt, ".html");
						HotDocs.Server.UtilityTools.ConvertFile(docFilePath, filePath, outputOptions);
					}
					if (formats.HasFlag(OutputFormat.HTMLwDataURIs))
					{
						string filePath = _docCache.GetDocFileName(".html");
						File.WriteAllText(filePath, EmbedImagesInURIs(filePath));  // Overwrite .html file.  If the consumer requested both HTML and HTMLwDataURIs, they'll only get the latter.
					}
					if (formats.HasFlag(OutputFormat.MHTML))
					{
						string htmlFilePath = _docCache.GetDocFileName(".html");
						string mhtmlFilePath = _docCache.GetDocFileName(".mhtml");
						File.WriteAllText(mhtmlFilePath, HtmlToMultiPartMime(htmlFilePath));
					}

				}
				if (tplExt != ".ttx")
				{
					if (formats.HasFlag(OutputFormat.PlainText))
					{
						string filePath = _docCache.GetDocFileName(".txt");
						HotDocs.Server.OutputOptions outputOptions = GetOutputOptions(tplExt, ".txt");
						HotDocs.Server.UtilityTools.ConvertFile(docFilePath, filePath, outputOptions);
					}
				}
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Private
		////////////////////////////////////////////////////////////////////////////////////////////////////

		private HotDocs.Server.Application _app = new HotDocs.Server.Application();

		private string CreateTempFolder(ISession session)
		{
			string tempDir = Path.Combine(session.TempFolderPath, Path.GetRandomFileName());
			Directory.CreateDirectory(tempDir);
			return tempDir;
		}

		private bool IsPdfExtension(string ext)
		{
			return
					string.Compare(ext, ".hpt", true) == 0
				|| string.Compare(ext, ".hpd", true) == 0
				|| string.Compare(ext, ".pdf", true) == 0;
		}

		private string FullTemplatePath
		{
			get
			{
				return MakeFullTemplatePath(_templatePath);
			}
		}

		private string MakeFullTemplatePath(string templateFile)
		{
			return Path.Combine(((Session)ParentAssemblyQueue.ParentSession).TemplateFolderPath, templateFile);
		}

		private void DoAssembly(string fullTemplatePath, HotDocs.Server.Interop.HDAssemblyOptions options, HotDocs.Server.AnswerCollection ansColl, string docFilePath)
		{
			_app.AssembleDocument(fullTemplatePath, options, ansColl, docFilePath, null);

			foreach (string cmdLine in _app.PendingAssemblyCmdLineStrings)
			{
				string path, switches;
				Util.ParseHdAsmCmdLine(cmdLine, out path, out switches);

				Assembly asm = new Assembly(ParentAssemblyQueue.ParentSession);//Maybe we should pass null for the assembly queue since the assembly isn't officially queued up yet.
				asm.Switches = switches;
				asm.TemplateFile = path;

				string asmPath = MakeFullTemplatePath(path);
				HotDocs.Server.TemplateInfo ti = new HotDocs.Server.TemplateInfo(asmPath);
				asm.TemplateTitle = ti.TemplateTitle;

				_pendingAssemblies.Add(asm);
			}

			List<string> unansList = new List<string>();
			unansList.AddRange(_app.UnansweredVariablesList);
			UnansweredVariablesList = unansList.ToArray();
		}

		private static string EmbedImagesInURIs(string fileName)
		{
			string html = null;
			// Loads the html file content from a byte[]
			html = File.ReadAllText(fileName); // TODO: Do we need to worry about BOM?    BOMEncoding.GetString(sec.GetFile(fileName), Encoding.Default);
			string targetFilenameNoExtention = Path.GetFileName(fileName).Replace(Path.GetExtension(fileName), "");
			// Iterates looking for images associated with the html file requested.
			foreach (string img in Directory.EnumerateFiles(Path.GetDirectoryName(fileName)))
			{
				string ext = Path.GetExtension(img).ToLower().Remove(0, 1);
				// Encode only the images that are related to the html file
				if (Path.GetFileName(img).StartsWith(targetFilenameNoExtention) && ((ext == "jpg") || (ext == "jpeg") || (ext == "gif") || (ext == "png") || (ext == "bmp")))
				{
					// Load the content of the image file
					byte[] data = File.ReadAllBytes(img);
					// Replace the html src attribute that points to the image file to its base64 content
					html = html.Replace(@"src=""" + Path.GetFileName(img), @"src=""data:" + getImageMimeType(ext) + ";base64," + Convert.ToBase64String(data));
//					html = html.Replace(@"src=""file://" + img, @"src=""data:" + getImageMimeType(ext) + ";base64," + Convert.ToBase64String(data));
				}
			}
			return html;
		}

		private static string getImageMimeType(string extension)
		{
			switch (extension)
			{
				case "bmp":
				case "dib":
					return "image/bmp";
				case "gif":
					return "image/gif";
				case "jpe":
				case "jpeg":
				case "jpg":
					return "image/jpeg";
				case "jfif":
					return "image/pjpeg";
				case "png":
				case "pnz":
					return "image/png";
				case "svg":
					return "image/svg+xml";
				case "tif":
				case "tiff":
					return "image/tiff";
				case "ico":
					return "image/x-icon";
				default:
					return "image/x-" + extension;
			}
		}

		private static string CreateMimePart(string contentBoundary, string contentType, string contentID, string content, string transferEncoding)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(contentBoundary);
			sb.AppendLine("Content-Type: " + contentType);
			if (!String.IsNullOrEmpty(transferEncoding))
				sb.AppendLine("Content-Transfer-Encoding: " + transferEncoding);
			if (!String.IsNullOrEmpty(contentID))
				sb.AppendLine("Content-ID: " + contentID);
			sb.AppendLine();
			sb.AppendLine(content);
			sb.AppendLine();
			return sb.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="htmlFileName"></param>
		/// <returns></returns>
		private static string HtmlToMultiPartMime(string htmlFileName)
		{
			/*
			 * MIME-Version: 1.0
			 * Content-Type: multipart/mixed; boundary="frontier"
			 * 
			 * This is a message with multiple parts in MIME format.
			 * --frontier
			 * Content-Type: text/plain
			 * 
			 * This is the body of the message.
			 * --frontier
			 * Content-Type: application/octet-stream
			 * Content-Transfer-Encoding: base64
			 *
			 * PGh0bWw+CiAgPGhlYWQ+CiAgPC9oZWFkPgogIDxib2R5PgogICAgPHA+VGhpcyBpcyB0aGUg
			 * Ym9keSBvZiB0aGUgbWVzc2FnZS48L3A+CiAgPC9ib2R5Pgo8L2h0bWw+Cg==
			 * --frontier--
			 */
			StringBuilder result = new StringBuilder();
			string html = File.ReadAllText(htmlFileName);
			// Extract all images from the html document.
			string imageFilesExpression = @"src=""(.+[.](png|jpg|jpeg|jpe|jfif|gif|bmp|tif|tiff|wmf|ico))[""]";

			// Extracts and maps image files to mime multipart sections
			Dictionary<string, string> imageMapping = (from Match m in Regex.Matches(html, imageFilesExpression, RegexOptions.IgnoreCase)
														select m.Groups[1].Value).Distinct().ToDictionary(v => v, v => Regex.Replace(Guid.NewGuid().ToString(), "{|}|-", "") + Path.GetExtension(v).ToLower());
			// Updates the html image references to use the multipart mapping
			foreach (KeyValuePair<string, string> pair in imageMapping)
				html = html.Replace(pair.Key, "cid:" + pair.Value);

			// Writes the header
			string boundary = "------=_NextPart_" + Regex.Replace(Guid.NewGuid().ToString(), "{|}", "");
			result.AppendLine("MIME-Version: 1.0");
			result.AppendLine("Content-Type: multipart/related;    boundary=\"" + boundary + "\";");
			result.AppendLine();
			result.AppendLine("This is a multi-part message in MIME format.");
			result.AppendLine();
			// Write the content parts
			boundary = "--" + boundary;
			// Appends the HTML
			result.Append(CreateMimePart(boundary, "text/html; charset=utf-8", null, html, null));
			// Appends all the image files
			foreach (KeyValuePair<string, string> pair in imageMapping)
			{
				result.Append(CreateMimePart(boundary,
												getImageMimeType(Path.GetExtension(pair.Key).ToLower().Remove(0, 1)),
												"<" + pair.Value + ">",
												Convert.ToBase64String(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(htmlFileName), pair.Key))),
												"base64"
												));
			}
			// End of the content
			result.Append(boundary + "--");

			return result.ToString();
		}


	}
}
