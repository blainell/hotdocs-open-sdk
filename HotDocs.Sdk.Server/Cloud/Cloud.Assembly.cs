/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.Cloud;

namespace HotDocs.Sdk.Server.Cloud
{
//	internal class Assembly : AssemblyBase, IAssembly
//	{
//		internal Assembly(IAssemblyQueue parentAssemblyQueue) : base(parentAssemblyQueue)
//		{
//		}

//		#region IAssembly Members

//		public string GetInterview()
//		{
//			throw new NotImplementedException();

//			//if (_assemblyComplete)
//			//	throw new AssemblyCompleteException();

//			//Contracts.InterviewOptions svcOpts = 0;
//			//if ((InterviewOptions & InterviewOptionFlags.NoPreview) == InterviewOptionFlags.NoPreview)
//			//	svcOpts |= Contracts.InterviewOptions.NoPreview;
//			//if ((InterviewOptions & InterviewOptionFlags.NoSave) == InterviewOptionFlags.NoSave)
//			//	svcOpts |= Contracts.InterviewOptions.NoSave;
//			//if ((InterviewOptions & InterviewOptionFlags.OmitImages) == InterviewOptionFlags.OmitImages)
//			//	svcOpts |= Contracts.InterviewOptions.OmitImages;

//			////Contracts.InterviewFormat intvwFmt = Contracts.InterviewFormat.JavaScript;
//			////if (InterviewFormat == InterviewFormat.Silverlight)
//			////	intvwFmt = Contracts.InterviewFormat.Silverlight;
//			//Dictionary<string, string> settings = new Dictionary<string, string>
//			//{
//			//	{"HotDocsJsUrl", ServerFilesUrl},
//			//	{"HotDocsCssUrl", StyleUrl},
//			//	{"InterviewDefUrl", InterviewDefinitionUrl},
//			//	{"DocPreviewUrl", DocumentPreviewUrl},
//			//	{"SaveAnswersPageUrl", SaveAnswersPageUrl},
//			//	{"FormActionurl", FormActionUrl}
//			//};
//			//var getInterviewOptions = new GetInterviewOptions(InterviewFormat, UnansweredVariablesList, TempInterviewUrl, settings);

//			////Get the interview.
//			//Contracts.BinaryObject[] interviewFiles;
//			//using (var client = new Client(Session.SubscriberId, Session.SigningKey))
//			//{
//			//	interviewFiles = client.GetInterview(Template, AnswerCollection.XmlAnswers, getInterviewOptions, Session.BillingRef);
//			//}

//			//string interviewFilesDir = ((HotDocs.Sdk.Server.Cloud.AssemblyQueue)AssemblyQueue).InterviewFilesDir;
//			//Directory.CreateDirectory(interviewFilesDir);

//			//if (!string.IsNullOrEmpty(interviewFilesDir))
//			//{
//			//	foreach (Contracts.BinaryObject bo in interviewFiles.Skip(1))
//			//	{
//			//		File.WriteAllBytes(Path.Combine(interviewFilesDir, bo.FileName), bo.Data);
//			//	}
//			//}

//			//if (interviewFiles.Length > 0 && !string.IsNullOrEmpty(interviewFiles[1].FileName))
//			//{
//			//	File.WriteAllText(Path.Combine(interviewFilesDir, "master_template"), interviewFiles[1].FileName);
//			//}

//			//return ExtractString(interviewFiles[0]);
//		}

//		public byte[] GetInterviewDefinition(string templateFile)
//		{
//			string interviewFilesDir = ((HotDocs.Sdk.Server.Cloud.AssemblyQueue)AssemblyQueue).InterviewFilesDir;
//			return File.ReadAllBytes(Path.Combine(interviewFilesDir, TemplateId + (InterviewFormat == InterviewFormat.JavaScript ? ".js" : ".dll")));
//		}

//		public override string DocumentExtension
//		{
//			get
//			{
//				string ext = System.IO.Path.GetExtension(TemplateId);
//				if (ext == ".docx")
//					return ".docx";
//				if (ext == ".rtf")
//					return ".rtf";
//				if (ext == ".wpt")
//					return ".wpd";
//				if (ext == ".hpt")
//					return ".hpd";
//				if (ext == ".hft")
//					return ".pdf";
//				if (ext == ".ttx")
//					return ".txt";
//				throw new Exception("Unexpected template type.");
//			}
//		}

//		public void Assemble(OutputFormat formats)
//		{
//			if (_assemblyComplete)
//				throw new AssemblyCompleteException();

//			//Clear the cache.
//			_docCache.DeleteAllFiles();

//			//For a web service implementation, we assemble the documents upfront.
//			string extension = DocumentExtension;
//			DoAssembly(formats);//Get a file name and create the file.
//		}

//		public System.IO.Stream GetAssembledFile(OutputFormat format)
//		{
//			if (_assemblyComplete)
//				throw new AssemblyCompleteException();

//			string extension = GetFormatExtension(format);
//			string filePath;
//			if (!_docCache.IsFileFormatAvailable(extension, out filePath))//If the file is not in the cache,
//				throw new Exception("The requested output was not available.");
//			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
//		}

//		#endregion

//		private ICloudSession Session
//		{
//			get
//			{
//				return (ICloudSession)ParentAssemblyQueue.ParentSession;
//			}
//		}

//		private AssemblyQueue AssemblyQueue
//		{
//			get
//			{
//				return ((AssemblyQueue)ParentAssemblyQueue);
//			}
//		}

//		public string TemplateId { get; set; }

//		private Template _template;
//		private Template Template
//		{
//			get
//			{
//				if (_template == null)
//				{
//					_template = new Template(AssemblyQueue.PackageId, TemplateId, @"C:\Program Files (x86)\HotDocs Server\Sample Portal\Files\Templates\" + AssemblyQueue.PackageFile);
//				}
//				return _template;
//			}
//		}

//		private static string ExtractString(Contracts.BinaryObject obj)
//		{
//			string extractedString;
//			Encoding enc = null;

//			if (obj.DataEncoding != null)
//			{
//				try
//				{
//					enc = System.Text.Encoding.GetEncoding(obj.DataEncoding);
//				}
//				catch (ArgumentException)
//				{
//					enc = null;
//				}
//			}

//			// BinaryObjects containing textual information from the HotDocs web service
//			// should always have DataEncoding set to the official IANA name of a text encoding.
//			// Therefore enc should always be non-null at this point.  However, in case of
//			// unexpected input, we include some alternative methods below.
//			System.Diagnostics.Debug.Assert(enc != null);

//			if (enc != null)
//			{
//				extractedString = enc.GetString(obj.Data);
//			}
//			else
//			{
//				using (var ms = new MemoryStream(obj.Data))
//				{
//					using (var tr = (TextReader)new StreamReader(ms))
//					{
//						extractedString = tr.ReadToEnd();
//					}
//				}
//			}
//			// discard BOM if there is one
//			if (extractedString[0] == 0xFEFF)
//				return extractedString.Substring(1);
//			else
//				return extractedString;
//		}

//		protected override void DoAssembly(OutputFormat formats)
//		{
//			Contracts.OutputFormat outputFormat = 0;
//			if ((formats & OutputFormat.Answers) != 0)
//				outputFormat |= Contracts.OutputFormat.Answers;
//			if ((formats & OutputFormat.Native) != 0)
//				outputFormat |= Contracts.OutputFormat.Native;
//			if ((formats & OutputFormat.PDF) != 0)
//				outputFormat |= Contracts.OutputFormat.PDF;
//			if ((formats & OutputFormat.PlainText) != 0)
//				outputFormat |= Contracts.OutputFormat.PlainText;
//			if ((formats & OutputFormat.HTML) != 0)
//			{
////				if (supportsInlineImages)
////					outputFormat |= Contracts.OutputFormat.HTMLwDataURIs;
////				else
//					outputFormat |= Contracts.OutputFormat.HTML;
//			}

//			//Do the AssembleDocument call.
//			var assemblyOptions = AssembleMarkupDocument ? Contracts.AssemblyOptions.MarkupView : Contracts.AssemblyOptions.None;
//			var options = new AssembleDocumentOptions(outputFormat, null); // TODO: Should we be including some settings?

//			Contracts.AssemblyResult results;
//			using (var client = new Client(Session.SubscriberId, Session.SigningKey))
//			{
//				results = client.AssembleDocument(Template, AnswerCollection.XmlAnswers, options, Session.BillingRef);
//			}

//			//Save the output documents.
//			for (int i = 0; i < results.Documents.Length; i++)
//			{
//				string ext = Path.GetExtension(results.Documents[i].FileName).ToLower();
//				if (ext == ".anx")
//				{
//					Contracts.BinaryObject resultingAnswers = results.Documents[i];
//					_ansSet.ReadXml(Encoding.UTF8.GetString(resultingAnswers.Data));
//				}
//				else
//				{
//					string filePath = _docCache.GetDocFileName(ext);
//					WriteDocumentFile(results.Documents[i], filePath);
//				}
//			}

//			//Store the unanswered variables (marked) list.
//			if (results.UnansweredVariables != null && results.UnansweredVariables.Length > 0)
//			{
//				string[] a = new string[results.UnansweredVariables.Length];
//				for (int i = 0; i < results.UnansweredVariables.Length; i++)
//					a[i] = results.UnansweredVariables[i];
//				UnansweredVariablesList = a;
//			}

//			//Build the pending assemblies list.
//			if (results.PendingAssemblies != null)
//			{
//				int cnt = results.PendingAssemblies.Length;
//				for (int i = 0; i < cnt; i++)
//				{
//					string fileName = results.PendingAssemblies[i].TemplateName;
//					string switches = results.PendingAssemblies[i].Switches;

//					IAssembly a = new Assembly(ParentAssemblyQueue);
//					a.TemplateFile = fileName;
//					a.Switches = switches;

//					_pendingAssemblies.Add(a);
//				}
//			}
//		}

//		private void WriteDocumentFile(Contracts.BinaryObject docFile, string filePath)
//		{
//			using (FileStream outFile = File.OpenWrite(filePath))
//			{
//				outFile.Write(docFile.Data, 0, docFile.Data.Length);
//			}
//		}

//	}
}
