/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Server.WebService
{
	internal class Assembly : AssemblyBase, IAssembly
	{
		internal Assembly(IAssemblyQueue parentAssemblyQueue) : base(parentAssemblyQueue)
		{
		}

		#region IAssembly Members

		public string GetInterview()
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();

			BinaryObject answers = PackageAnswerFile(_ansSet);

			InterviewOptions svcOpts = (InterviewOptions)0;
			if ((InterviewOptions & InterviewOptions.NoPreview) == InterviewOptions.NoPreview)
				svcOpts |= HotDocs.Sdk.Server.Contracts.InterviewOptions.NoPreview;
			if ((InterviewOptions & InterviewOptions.NoSave) == InterviewOptions.NoSave)
				svcOpts |= HotDocs.Sdk.Server.Contracts.InterviewOptions.NoSave;
			if ((InterviewOptions & InterviewOptions.OmitImages) == InterviewOptions.OmitImages)
				svcOpts |= HotDocs.Sdk.Server.Contracts.InterviewOptions.OmitImages;
			if ((InterviewOptions & InterviewOptions.ExcludeStateFromOutput) == InterviewOptions.ExcludeStateFromOutput)
				svcOpts |= HotDocs.Sdk.Server.Contracts.InterviewOptions.ExcludeStateFromOutput;

			//Get the interview.
			BinaryObject[] interviewFiles;
			using (Proxy client = new Proxy("BasicHttpBinding_IHDSvc"))
			{
				interviewFiles = client.GetInterview(
					TemplateFile, // template file
					answers == null ? null : new BinaryObject[] { answers }, // answers
					InterviewFormat,
					svcOpts, // instruct HDS not to return images used by the interview; we'll get them on-demand
					UnansweredVariablesList, // variables to highlight as unanswered
					FormActionUrl, // page to which interview will submit its answers
					ServerFilesUrl, // location (under this app's domain name) where HotDocs Server JS files are available
					StyleUrl, // URL of CSS stylesheet (typically called hdsuser.css).  hdssystem.css must exist in same directory.
					TempInterviewUrl, // interview images will be requested from GetImage.ashx, which will stream them from the template directory
					SaveAnswersPageUrl, //for the save answers button; if this is null the "Save Answers" button does not appear
					DocumentPreviewUrl, // document previews will be requested from here; if null the "Document Preview" button does not appear
					InterviewDefinitionUrl); //Silverlight interview DLLs will be requested from here -- careful with relative URLs!!
				// If relative path is used, it must be relative to HDServerFiles\js, NOT this page!

				SafeCloseClient(client);				
			}

			StringBuilder interview = new StringBuilder(Util.ExtractString(interviewFiles[0]));
			Util.AppendSdkScriptBlock(interview, MakeFullTemplatePath(TemplateFile));
			return interview.ToString();
		}

		public byte[] GetInterviewDefinition(string templateFile)
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();

			BinaryObject slInterview;
			using (Proxy client = new Proxy("BasicHttpBinding_IHDSvc"))
			{
				slInterview = client.GetInterviewDefinition(templateFile, null, InterviewFormat, null);
				SafeCloseClient(client);
			}
			return slInterview.Data;
		}

		public void Assemble(OutputFormat formats)
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();

			//Clear the cache.
			_docCache.DeleteAllFiles();

			//For a web service implementation, we assemble the documents upfront.
			DoAssembly(formats);//Get a file name and create the file.
		}

		public System.IO.Stream GetAssembledFile(OutputFormat format)
		{
			if (_assemblyComplete)
				throw new AssemblyCompleteException();

			string extension = GetFormatExtension(format);
			string filePath;
			if (!_docCache.IsFileFormatAvailable(extension, out filePath))//If the file is not in the cache,
				throw new Exception("The requested output was not available.");
			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
		}

		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Private methods
		////////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void DoAssembly(OutputFormat formats)
		{
			OutputFormat svcFmt = 0;
			if (formats.HasFlag(OutputFormat.Answers))
				svcFmt |= OutputFormat.Answers;
			if (formats.HasFlag(OutputFormat.Native))
				svcFmt |= OutputFormat.Native;
			if (formats.HasFlag(OutputFormat.PDF))
				svcFmt |= OutputFormat.PDF;
			if (formats.HasFlag(OutputFormat.PlainText))
				svcFmt |= OutputFormat.PlainText;
			if (formats.HasFlag(OutputFormat.HTML))
				svcFmt |= OutputFormat.HTML;
			if (formats.HasFlag(OutputFormat.HPD))
				svcFmt |= OutputFormat.HPD;
			if (formats.HasFlag(OutputFormat.HTMLwDataURIs))
				svcFmt |= OutputFormat.HTMLwDataURIs;
			if (formats.HasFlag(OutputFormat.MHTML))
				svcFmt |= OutputFormat.MHTML;

			//Do the AssembleDocument call.
			Proxy client = new Proxy("BasicHttpBinding_IHDSvc");
			BinaryObject answers = PackageAnswerFile(_ansSet);
			//Contracts.OutputFormat svcFmt = (Contracts.OutputFormat)(formats | DocumentFormats);
			AssemblyOptions options = AssembleMarkupDocument ? AssemblyOptions.MarkupView : AssemblyOptions.None;
			AssemblyResult results = client.AssembleDocument(
				TemplateFile, answers == null ? null : new BinaryObject[] { answers },
				svcFmt,
				options,
				null);

			//Save the output documents.
			for (int i = 0; i < results.Documents.Length; i++)
			{
				string ext = Path.GetExtension(results.Documents[i].FileName).ToLower();
				if (ext == ".anx")
				{
					BinaryObject resultingAnswers = results.Documents[i];
					_ansSet.ReadXml(Encoding.UTF8.GetString(resultingAnswers.Data));
				}
				else
				{
					string filePath = _docCache.GetDocFileName(ext);
					WriteDocumentFile(results.Documents[i], filePath);
				}
			}

			//Store the unanswered variables (marked) list.
			if (results.UnansweredVariables != null && results.UnansweredVariables.Length > 0)
			{
				string[] a = new string[results.UnansweredVariables.Length];
				for (int i = 0; i < results.UnansweredVariables.Length; i++)
					a[i] = results.UnansweredVariables[i];
				UnansweredVariablesList = a;
			}

			//Build the pending assemblies list.
			if (results.PendingAssemblies != null)
			{
				int cnt = results.PendingAssemblies.Length;
				for (int i = 0; i < cnt; i++)
				{
					string fileName = Path.GetFileName(results.PendingAssemblies[i].TemplateName);
					string templatePath = MakeFullTemplatePath(fileName);
					string switches = results.PendingAssemblies[i].Switches;

					HotDocs.Sdk.TemplateManifest manifest = HotDocs.Sdk.TemplateManifest.ParseManifest(templatePath, ManifestParseFlags.ParseTemplateInfo);

					IAssembly a = new Assembly(ParentAssemblyQueue);
					a.TemplateFile = fileName;
					a.Switches = switches;
					a.TemplateTitle = manifest.Title;

					_pendingAssemblies.Add(a);
				}
			}
		}

		private string MakeFullTemplatePath(string templateFile)
		{
			return Path.Combine(((Session)ParentAssemblyQueue.ParentSession).TemplateFolderPath, templateFile);
		}

		private BinaryObject PackageAnswerFile(string filename)
		{
			BinaryObject result = null;
			if (!String.IsNullOrEmpty(filename) && !filename.Contains("../") && !filename.Contains(@"..\"))
			{
				try
				{
					result = new BinaryObject();
					using (System.IO.FileStream fs = System.IO.File.OpenRead(filename))
					{
						result.DataEncoding = Encoding.UTF8.WebName; // expects answer files on disk to be utf-8 encoded.
						result.Data = new byte[fs.Length];
						fs.Read(result.Data, 0, Convert.ToInt32(fs.Length));
						// Note that strictly speaking we should probably check the BOM to determine the encoding and
						// strip the BOM off before sending this answer file to the web service, but I happen to know that
						// the web service handles this for us, so we'll leave it.
						// If we wanted to do it ourselves, instead of using a binary FileStream, we could use a textreader:
						// using (fs = (TextReader) new StringReader(filename))
						// and that would detect the encoding, read the whole file as a string (without the BOM), and
						// then we could re-encode it in the BinaryObject with any encoding we wanted.
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Unable to read requested answer file.", ex);
				}
			}
			return result;
		}

		private BinaryObject PackageAnswerFile(AnswerCollection ansSet)
		{
			BinaryObject result = null;
			if (ansSet != null)
			{
				try
				{
					result = new BinaryObject();
					result.DataEncoding = Encoding.UTF8.WebName; // expects answer files on disk to be utf-8 encoded.
					result.Data = Encoding.UTF8.GetBytes(ansSet.XmlAnswers);
					// Note that strictly speaking we should probably check the BOM to determine the encoding and
					// strip the BOM off before sending this answer file to the web service, but I happen to know that
					// the web service handles this for us, so we'll leave it.
					// If we wanted to do it ourselves, instead of using a binary FileStream, we could use a textreader:
					// using (fs = (TextReader) new StringReader(filename))
					// and that would detect the encoding, read the whole file as a string (without the BOM), and
					// then we could re-encode it in the BinaryObject with any encoding we wanted.
				}
				catch (Exception ex)
				{
					throw new Exception("Unable serialize the answers.", ex);
				}
			}
			return result;
		}

		private static void SafeCloseClient(Proxy client)
		{
			// this approach modeled on http://msdn.microsoft.com/en-us/library/aa355056.aspx
			// Bottom line is that calling Dispose() on a client causes Close() to be called,
			// and Close() can throw exceptions (see below).  So don't dispose one without first
			// calling Close() and handling the network-related exceptions that might happen.
			try
			{
				client.Close();
			}
			catch (System.ServiceModel.CommunicationException)
			{
				client.Abort();
			}
			catch (TimeoutException)
			{
				client.Abort();
			}
			catch (Exception)
			{
				client.Abort();
				throw;
			}
		}


		private void WriteDocumentFile(BinaryObject docFile, string filePath)
		{
			using (FileStream outFile = File.OpenWrite(filePath))
			{
				outFile.Write(docFile.Data, 0, docFile.Data.Length);
			}
		}
	}
}
