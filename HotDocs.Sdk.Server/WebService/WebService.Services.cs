/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Server.WebService
{
	/// <summary>
	/// This <c>Services</c> class is an implementation of the IServices interface that uses HTTP or HTTPS web services calls to connect
	/// to HotDocs Server for each of the IServices interface methods.
	/// </summary>
	public class Services : IServices
	{
		private string _endPointName;
		private string _baseTemplateLocation;

		public Services(string endPointName, string templatePath)
		{
			if (string.IsNullOrWhiteSpace(endPointName))
				throw new ArgumentNullException("The web service end point is missing. " +
					"Please check the value for WebServiceEndPoint in the config file and try again.");
			if (string.IsNullOrWhiteSpace(templatePath))
				throw new ArgumentNullException("The base template location is missing. " + 
					"Please check the value for TemplatePath in the config file and try again.");
			if (Directory.Exists(templatePath) == false)
				throw new ArgumentNullException(@"The base template location is does not exist at: "" + _baseTemplateLocation + "".  Please check the value defined as TemplatePath in the config file and try again.  ");
			_endPointName = endPointName;
			_baseTemplateLocation = templatePath.ToLower();
		}

		#region IServices Members

		public InterviewResult GetInterview(Template template, TextReader answers, InterviewSettings settings, IEnumerable<string> markedVariables, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (settings == null)
				settings = new InterviewSettings();

			// Add the query string to the interview image url so dialog element images can be located.
			settings.InterviewImageUrlQueryString = "?loc=" + template.CreateLocator() + "&img=";

			// Configure interview options
			InterviewOptions itvOpts = InterviewOptions.OmitImages; // Instructs HDS not to return images used by the interview; we'll get them ourselves from the template folder.
			if (settings.DisableDocumentPreview == Tristate.True)
				itvOpts |= InterviewOptions.NoPreview; // Disables (omits) the Document Preview button on the interview toolbar.
			if (settings.DisableSaveAnswers == Tristate.True)
				itvOpts |= InterviewOptions.NoSave; // Disables (omits) the Save Answers button on the interview toolbar.
			if (settings.RoundTripUnusedAnswers != Tristate.True)
				itvOpts |= InterviewOptions.ExcludeStateFromOutput; // Prevents original answer file from being encrypted and sent to the interview and then posted back at the end.

			// Get the interview.
			InterviewResult result = new InterviewResult();
			BinaryObject[] interviewFiles = null;

			using (Proxy client = new Proxy(_endPointName))
			{
				string fileName = GetRelativePath(template.GetFullPath());
				interviewFiles = client.GetInterview(
					fileName,
					answers == null ? null : new BinaryObject[] { GetBinaryObjectFromTextReader(answers) }, // answers
					settings.Format,
					itvOpts,
					markedVariables != null ? markedVariables.ToArray<string>() : null, // variables to highlight as unanswered
					settings.PostInterviewUrl, // page to which interview will submit its answers
					settings.InterviewRuntimeUrl, // location (under this app's domain name) where HotDocs Server JS files are available
					settings.StyleSheetUrl + "/" + settings.ThemeName + ".css", // URL of CSS stylesheet (typically called hdsuser.css).  hdssystem.css must exist in same directory.
					settings.InterviewImageUrl, // interview images will be requested from GetImage.ashx, which will stream them from the template directory
					settings.DisableSaveAnswers != Tristate.True ? settings.SaveAnswersUrl : "", //for the save answers button; if this is null the "Save Answers" button does not appear
					settings.DisableDocumentPreview != Tristate.True ? settings.DocumentPreviewUrl : "", // document previews will be requested from here; if null the "Document Preview" button does not appear
					settings.InterviewDefinitionUrl); //Silverlight interview DLLs will be requested from here -- careful with relative URLs!!
				if (interviewFiles != null)
				{
					StringBuilder interview = new StringBuilder(Util.ExtractString(interviewFiles[0]));
					Util.AppendSdkScriptBlock(interview, template, settings);
					result.HtmlFragment = interview.ToString();
				}
				SafeCloseClient(client);
			}
			return result;

		}

		public AssembleDocumentResult AssembleDocument(Template template, TextReader answers, AssembleDocumentSettings settings, string logRef)
		{
			AssembleDocumentResult result = null;
			AssemblyResult asmResult = null;
			OutputFormat outputFormat = ConvertFormat(settings.Format);
			AssemblyOptions assemblyOptions = ConvertOptions(settings);
			using (Proxy client = new Proxy(_endPointName))
			{
				string fileName = GetRelativePath(template.GetFullPath());
				asmResult = client.AssembleDocument(
					fileName,
					answers == null ? null : new BinaryObject[] { GetBinaryObjectFromTextReader(answers) }, // answers
					outputFormat,
					assemblyOptions,
					null);
				SafeCloseClient(client);
			}
			if (asmResult != null)
			{
				result = ConvertAssemblyResult(template, asmResult, settings.Format);
			}
			return result;
		}

		public ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string logRef)
		{
			ComponentInfo result;
			using (Proxy client = new Proxy(_endPointName))
			{
				string fileName = GetRelativePath(template.GetFullPath());
				result = client.GetComponentInfo(fileName, includeDialogs);
				SafeCloseClient(client);
			}
			return result;
		}

		public string GetAnswers(IEnumerable<TextReader> answers, string logRef)
		{
			BinaryObject combinedAnswers;
			using (Proxy client = new Proxy(_endPointName))
			{
				var answerObjects = (from answer in answers select GetBinaryObjectFromTextReader(answer)).ToArray();
				combinedAnswers = client.GetAnswers(answerObjects);
				SafeCloseClient(client);
			}
			return Util.ExtractString(combinedAnswers);
		}

		public void BuildSupportFiles(Template template, HDSupportFilesBuildFlags flags)
		{
			using (Proxy client = new Proxy(_endPointName))
			{
				string templateId = template.FileName;
				string templateKey = template.Location.Key;
				string templateState = null;
				client.BuildSupportFiles(templateId, templateKey, flags, templateState);
				SafeCloseClient(client);
			}
		}

		public void RemoveSupportFiles(Template template)
		{
			using (Proxy client = new Proxy(_endPointName))
			{
				string templateId = template.FileName;
				string templateKey = template.Location.Key;
				string templateState = null;
				client.RemoveSupportFiles(templateId, templateKey, templateState);
				SafeCloseClient(client);
			}
		}

		public System.IO.Stream GetInterviewDefinition(string state, string templateFile, InterviewFormat format)
		{
			System.IO.Stream result = null;

			using (Proxy client = new Proxy(_endPointName))
			{
				//TODO: Research this to see if the same relative path is needed above. also check parameter duplication
				string templateId = templateFile;
				string templateName = templateFile;
				string templateState = state;
				BinaryObject binaryObject = client.GetInterviewDefinition(templateId, templateName, format, templateState);
				SafeCloseClient(client);
				result = new MemoryStream(binaryObject.Data);
			}
			return result;
		}

		#endregion

		#region Private member methods:

		private static void SafeCloseClient(Proxy client)
		{
			//TODO: research this and decide what works best
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

		BinaryObject GetBinaryObjectFromTextReader(TextReader textReader)
		{
			string allText = textReader.ReadToEnd();
			return new BinaryObject
					{
						Data = Encoding.UTF8.GetBytes(allText),
						DataEncoding = "UTF-8"
					};
		}

		OutputFormat ConvertFormat(DocumentType docType)
		{
			OutputFormat format = OutputFormat.None;
			switch (docType)
			{
				case DocumentType.HFD:
					format = OutputFormat.HFD;
					break;
				case DocumentType.HPD:
					format = OutputFormat.HPD;
					break;
				case DocumentType.HTML:
					format = OutputFormat.HTML;
					break;
				case DocumentType.HTMLwDataURIs:
					format = OutputFormat.HTMLwDataURIs;
					break;
				case DocumentType.MHTML:
					format = OutputFormat.MHTML;
					break;
				case DocumentType.Native:
					format = OutputFormat.Native;
					break;
				case DocumentType.PDF:
					format = OutputFormat.PDF;
					break;
				case DocumentType.PlainText:
					format = OutputFormat.PlainText;
					break;
				case DocumentType.WordDOC:
					format = OutputFormat.DOCX;
					break;
				case DocumentType.WordDOCX:
					format = OutputFormat.DOCX;
					break;
				case DocumentType.WordPerfect:
					format = OutputFormat.WPD;
					break;
				case DocumentType.WordRTF:
					format = OutputFormat.RTF;
					break;
				case DocumentType.XML:
					// Note: Contracts.OutputFormat does not have an XML document type.
					format = OutputFormat.None;
					break;
				default:
					format = OutputFormat.None;
					break;
			}
			// Always include the Answers output
			format |= OutputFormat.Answers;
			return format;
		}

		AssemblyOptions ConvertOptions(AssembleDocumentSettings settings)
		{
			AssemblyOptions assemblyOptions = new AssemblyOptions();
			assemblyOptions = AssemblyOptions.None;
			if (settings.UseMarkupSyntax == Tristate.True)
				assemblyOptions |= AssemblyOptions.MarkupView;
			return assemblyOptions;
		}

		//TODO: move this over to the Util class so that both WS and Cloud implementions of the IService class can use it
		AssembleDocumentResult ConvertAssemblyResult(Template template, AssemblyResult asmResult, DocumentType docType)
		{
			AssembleDocumentResult result = null;
			MemoryStream document = null;
			StreamReader ansRdr = null;
			Template[] pendingAssemblies = new Template[asmResult.PendingAssemblies == null ? 0 : asmResult.PendingAssemblies.Length];
			if (asmResult.PendingAssemblies != null)
			{
				
			for (int i = 0; i < asmResult.PendingAssemblies.Length; i++)
			{
				string templateName = Path.GetFileName(asmResult.PendingAssemblies[i].TemplateName);
				string switches = asmResult.PendingAssemblies[i].Switches;
				Template pendingTemplate = new Template(templateName, template.Location.Duplicate(), switches);
				pendingAssemblies[i] = pendingTemplate;
			}
			}
			for (int i = 0; i < asmResult.Documents.Length; i++)
			{
				switch (asmResult.Documents[i].Format)
				{
					case OutputFormat.Answers:
						ansRdr = new StreamReader(new MemoryStream(asmResult.Documents[i].Data));
						break;
					default:
						document = new MemoryStream(asmResult.Documents[i].Data);
						if (docType == DocumentType.Native)
						{
							docType = Document.GetDocumentType(asmResult.Documents[i].FileName);
						}
						break;
				}

				// TODO: If we are requesting an HTML page, there might be additional images that need to be in the supporting files.
			}
			if (document != null)
			{
				NamedStream[] supportingFiles = null;
				result = new AssembleDocumentResult(
					new Document(template, document, docType, supportingFiles, asmResult.UnansweredVariables),
					ansRdr == null ? null : ansRdr.ReadToEnd(),
					pendingAssemblies,
					asmResult.UnansweredVariables
				);
			}
			return result;
		}

		string GetRelativePath(string fullPath)
		{
			string sRet = string.Empty;
			string full = fullPath.ToLower();
			int i = full.IndexOf(_baseTemplateLocation);
			if (i == 0)
				sRet = full.Substring(_baseTemplateLocation.Length + 1);
			else
			{
				throw new Exception(string.Format(@"Error: The configured TemplatePath location ""{0}"" does not match the location of the current template ""{1}""", 
					_baseTemplateLocation, fullPath));
			}
			return sRet;
		}

		#endregion //  Private member methods:
	}
}
