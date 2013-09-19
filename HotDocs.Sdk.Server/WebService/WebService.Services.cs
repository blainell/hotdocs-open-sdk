/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

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

		/// <summary>
		/// The <c>Services</c> constructor
		/// </summary>
		/// <param name="endPointName">The <c>endPointName</c> defines host to where web service calls will be made</param>
		/// <param name="templatePath">The <c>templatePath</c> is the base folder location where templates are stored</param>
		public Services(string endPointName, string templatePath)
		{
			if (string.IsNullOrWhiteSpace(endPointName))
				throw new ArgumentNullException("WebServices.Services constructor: The parameter 'endPointName' is empty or null");
			if (string.IsNullOrWhiteSpace(templatePath))
				throw new ArgumentNullException("WebServices.Services constructor: the parameter 'templatePath' is empty or null");
			if (Directory.Exists(templatePath) == false)
				throw new DirectoryNotFoundException(string.Format(@"WebServices.Services constructor: The parameter 'templatePath' folder does not exist at: ""{0}"".", templatePath));
			_endPointName = endPointName;
			_baseTemplateLocation = templatePath.ToLower();
		}

		#region IServices Members

		/// <summary>
		/// <c>GetInterview</c> returns an HTML fragment suitable for inclusion in any standards-mode web page, which embeds a HotDocs interview
		/// directly in that web page.
		/// </summary>
		/// <param name="template">An instance of the Template class, for which the interview will be requested.</param>
		/// <param name="answers">The initial set of answers to include in the interview.</param>
		/// <param name="settings">Settings that define various interview behaviors.</param>
		/// <param name="markedVariables">A collection of variables that should be marked with special formatting in the interview.</param>
		/// <param name="logRef">A string to display in logs related to this request.</param>
		/// <returns>An object which contains an HTML fragment to be inserted in a web page to display the interview.</returns>
		public InterviewResult GetInterview(Template template, TextReader answers, InterviewSettings settings, IEnumerable<string> markedVariables, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			string logStr = logRef == null ? string.Empty : logRef;
			
			if (template == null)
				throw new ArgumentNullException("template", string.Format(@"WebServices.Services.GetInterview: the ""template"" parameter passed in was null, logRef: {0}", logStr));
			
			if (settings == null)
				settings = new InterviewSettings();

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
					answers == null ? null : new BinaryObject[] { Util.GetBinaryObjectFromTextReader(answers) }, // answers
					settings.Format,
					itvOpts,
					markedVariables != null ? markedVariables.ToArray<string>() : null, // variables to highlight as unanswered
					settings.PostInterviewUrl, // page to which interview will submit its answers
					settings.InterviewRuntimeUrl, // location (under this app's domain name) where HotDocs Server JS files are available
					settings.StyleSheetUrl + "/" + settings.ThemeName + ".css", // URL of CSS stylesheet (typically called hdsuser.css).  hdssystem.css must exist in same directory.
					Util.GetInterviewImageUrl(settings, template), // interview images will be requested from GetInterviewFile.ashx, which will stream them from the template directory
					settings.DisableSaveAnswers != Tristate.True ? settings.SaveAnswersUrl : "", //for the save answers button; if this is null the "Save Answers" button does not appear
					settings.DisableDocumentPreview != Tristate.True ? settings.DocumentPreviewUrl : "", // document previews will be requested from here; if null the "Document Preview" button does not appear
					Util.GetInterviewDefinitionUrl(settings, template)); // Interview definitions (Silverlight or JavaScript) will be requested from here -- careful with relative URLs!!
				if (interviewFiles != null)
				{
					StringBuilder interview = new StringBuilder(Util.ExtractString(interviewFiles[0]));
					Util.AppendSdkScriptBlock(interview, template, settings);
					result.HtmlFragment = interview.ToString();
				}
				SafeCloseClient(client, logRef);
			}
			return result;

		}

		/// <summary>
		/// <c>AssembleDocument</c> assembles (creates) a document from the given template, answers and settings.
		/// </summary>
		/// <param name="template">An instance of the Template class, from which the document will be assembled.</param>
		/// <param name="answers">The set of answers that will be applied to the template to assemble the document</param>
		/// <param name="settings">settings that will be used to assemble the document. 
		/// These settings include the assembled document format (file extension), markup syntax, how to display fields with unanswered variables, etc</param>
		/// <param name="logRef">A string to display in logs related to this request.</param>
		/// <returns>returns information about the assembled document, the document type, the unanswered variables, the resulting answers, etc.</returns>
		public AssembleDocumentResult AssembleDocument(Template template, TextReader answers, AssembleDocumentSettings settings, string logRef)
		{
			string logStr = logRef == null ? string.Empty : logRef;
			if (template == null)
				throw new ArgumentNullException("template", string.Format(@"WebService.Services.AssembleDocument: the ""template"" parameter passed in was null, logRef: {0}", logStr));
			if (settings == null)
				settings = new AssembleDocumentSettings();
			AssembleDocumentResult result = null;
			AssemblyResult asmResult = null;
			OutputFormat outputFormat = ConvertFormat(settings.Format);
			AssemblyOptions assemblyOptions = ConvertOptions(settings);
			using (Proxy client = new Proxy(_endPointName))
			{
				string fileName = GetRelativePath(template.GetFullPath());
				asmResult = client.AssembleDocument(
					fileName,
					answers == null ? null : new BinaryObject[] { Util.GetBinaryObjectFromTextReader(answers) }, // answers
					outputFormat,
					assemblyOptions,
					null);
				SafeCloseClient(client, logRef);
			}
			if (asmResult != null)
			{
				result = ConvertAssemblyResult(template, asmResult, settings.Format);
			}
			return result;
		}

		/// <summary>
		/// <c>GetComponentInfo</c> returns metadata about the variables/types (and optionally dialogs and mapping info)
		/// for the indicated template's interview.
		/// </summary>
		/// <param name="template">An instance of the Template class, for which you are requesting component information.</param>
		/// <param name="includeDialogs">Whether to include dialog and mapping information in the returned results.</param>
		/// <param name="logRef">A string to display in logs related to this request.</param>
		/// <returns>returns the list of variables and dialogs (if includeDialogs is true) associated with the <c>template</c> parameter</returns>
		public ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string logRef)
		{
			string logStr = logRef == null ? string.Empty : logRef;
			if (template == null)
				throw new ArgumentNullException("template", string.Format(@"WebService.Services.GetComponentInfo: the ""template"" parameter passed in was null, logRef: {0}", logStr));
			ComponentInfo result;
			using (Proxy client = new Proxy(_endPointName))
			{
				string fileName = GetRelativePath(template.GetFullPath());
				result = client.GetComponentInfo(fileName, includeDialogs);
				SafeCloseClient(client, logRef);
			}
			return result;
		}

		/// <summary>
		/// <c>GetAnswers</c> overlays any answer collections passed into it, into a single XML answer collection.
		/// It has two primary uses: it can be used to combine multiple answer collections into a single
		/// answer collection; and/or it can be used to "resolve" or standardize an answer collection
		/// submitted from a browser interview (which may be specially encoded) into standard XML answers.
		/// </summary>
		/// <param name="answers">A sequence of answer collections. Each member of this sequence
		/// must be either an (encoded) interview answer collection or a regular XML answer collection.
		/// Each member will be successively overlaid (overlapped) on top of the prior members to
		/// form one consolidated answer collection.</param>
		/// <param name="logRef">A string to display in logs related to this request.</param>
		/// <returns></returns>
		public string GetAnswers(IEnumerable<TextReader> answers, string logRef)
		{
			string logStr = logRef == null ? string.Empty : logRef;
			if (answers == null)
				throw new ArgumentNullException("answers", string.Format(@"WebService.Services.GetAnswers: the ""answers"" parameter passed in was null, logRef: {0}", logStr));
			BinaryObject combinedAnswers;
			using (Proxy client = new Proxy(_endPointName))
			{
				var answerObjects = (from answer in answers select Util.GetBinaryObjectFromTextReader(answer)).ToArray();
				combinedAnswers = client.GetAnswers(answerObjects);
				SafeCloseClient(client, logRef);
			}
			return Util.ExtractString(combinedAnswers);
		}

		/// <summary>
		/// <c>BuildSupportFiles</c> generates (or regenerates) the supporting javascript files and Silverlight DLLs 
		/// for the supplied <c>template</c>
		/// </summary>
		/// <param name="template">An instance of the Template class, for which the supporting javascript files and 
		/// Silverlight DLLs will be generated</param>
		/// <param name="flags">A set of flags to control whether javascript or SilverLight files will be generated, 
		/// as well as whether to build files for templates included with an assemble instruction.</param>
		public void BuildSupportFiles(Template template, HDSupportFilesBuildFlags flags)
		{
			if (template == null)
				throw new ArgumentNullException("template", @"WebService.Services.BuildSupportFiles: the ""template"" parameter passed in was null");
			using (Proxy client = new Proxy(_endPointName))
			{
				string templateId = GetRelativePath(template.GetFullPath());
				string templateKey = template.FileName;
				string templateState = null;
				client.BuildSupportFiles(templateId, templateKey, flags, templateState);
				SafeCloseClient(client, null);
			}
		}

		/// <summary>
		/// <c>RemoveSupportFiles</c> removes support files (javascript and SilverLight) for the supplied <c>template</c>
		/// </summary>
		/// <param name="template">An instance of the Template class, for which the supporting javascript files and 
		/// Silverlight DLLs will be removed</param>
		public void RemoveSupportFiles(Template template)
		{
			if (template == null)
				throw new ArgumentNullException("template", @"WebService.Services.RemoveSupportFiles: the ""template"" parameter passed in was null");
			using (Proxy client = new Proxy(_endPointName))
			{
				string templateId = GetRelativePath(template.GetFullPath());
				string templateKey = template.FileName;
				string templateState = null;
				client.RemoveSupportFiles(templateId, templateKey, templateState);
				SafeCloseClient(client, null);
			}
		}

		/// <summary>
		/// Retrieves a file required by the interview. This could be either an interview definition that contains the 
		/// variables and logic required to display an interview (questionaire) for the main template or one of its 
		/// inserted templates, or it could be an image file displayed on a dialog within the interview.
		/// </summary>
		/// <param name="template">The template related to the requested file.</param>
		/// <param name="fileName">The file name of the image, or the file name of the template for which the interview
		/// definition is being requested. In either case, this value is passed as "template" on the query string by the browser interview.</param>
		/// <param name="fileType">The type of file being requested: img (image file), js (JavaScript interview definition), 
		/// or dll (Silverlight interview definition).</param>
		/// <returns>A stream containing the requested interview file, to be returned to the caller.</returns>
		public Stream GetInterviewFile(Template template, string fileName, string fileType)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (template == null)
				throw new ArgumentNullException("template", @"WebService.Services.GetInterviewFile: the ""template"" parameter passed in was null");

			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName", @"WebService.Services.GetInterviewFile: the ""fileName"" parameter passed in was null or empty");

			if (string.IsNullOrEmpty(fileType))
				throw new ArgumentNullException("fileType", @"WebService.Services.GetInterviewFile: the ""fileType"" parameter passed in was null or empty");

			// Return an image or interview definition from the template.
			switch (fileType.ToUpper())
			{
				case "IMG":
					return template.Location.GetFile(fileName);
				default:
					System.IO.Stream result = null;

					using (Proxy client = new Proxy(_endPointName))
					{
						string templateId = GetRelativePath(template.GetFullPath()); // The relative path to the template folder.
						string templateName = fileName; // The name of the template file for which the interview is being requested (e.g., demoempl.rtf). 
						string templateState = string.Empty; // We are using the templateId rather than template state since all we have to work with is a template locator.
						InterviewFormat format = InterviewFormat.JavaScript;
						BinaryObject binaryObject = client.GetInterviewDefinition(templateId, templateName, format, templateState);
						SafeCloseClient(client, null);
						result = new MemoryStream(binaryObject.Data);
					}
					return result;
			}
		}

		#endregion

		#region Private member methods:

		private static void SafeCloseClient(Proxy client, string logRef)
		{
			// this approach modeled on http://msdn.microsoft.com/en-us/library/aa355056.aspx
			// Bottom line is that calling Dispose() on a client causes Close() to be called,
			// and Close() can throw exceptions (see below).  So don't dispose one without first
			// calling Close() and handling the network-related exceptions that might happen.
			try
			{
				client.Close();
			}
			catch (System.ServiceModel.CommunicationException x1)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("SafeCloseClient: a CommunicationException occured when closing the web service proxy client: {0}\r\n" +
					"Stack Trace: {1}", x1.Message, x1.StackTrace));
				client.Abort();
			}
			catch (TimeoutException x2)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("SafeCloseClient: a TimeoutException occured when closing the web service proxy client: {0}\r\n" +
					"Stack Trace: {1}", x2.Message, x2.StackTrace));
				client.Abort();
			}
			catch (Exception x3)
			{
				System.Diagnostics.Trace.WriteLine(string.Format("SafeCloseClient: a general exception occured when closing the web service proxy client: {0}\r\n" +
					"Stack Trace: {1}", x3.Message, x3.StackTrace));
				client.Abort();
				throw;
			}
		}

		private OutputFormat ConvertFormat(DocumentType docType)
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
			List<NamedStream> supportingFiles = new List<NamedStream>();
			IEnumerable<Template> pendingAssemblies = null;
			if (asmResult.PendingAssemblies != null)
				pendingAssemblies = from pa in asmResult.PendingAssemblies
									select new Template(Path.GetFileName(pa.TemplateName), template.Location.Duplicate(), pa.Switches);
			for (int i = 0; i < asmResult.Documents.Length; i++)
			{
				switch (asmResult.Documents[i].Format)
				{
					case OutputFormat.Answers:
						ansRdr = new StreamReader(new MemoryStream(asmResult.Documents[i].Data));
						break;
					case OutputFormat.JPEG:
					case OutputFormat.PNG:
						// If the output document is plain HTML, we might also get additional image files in the 
						// AssemblyResult that we need to pass on to the caller.
						supportingFiles.Add(new NamedStream(asmResult.Documents[i].FileName, new MemoryStream(asmResult.Documents[i].Data)));
						break;
					default:
						document = new MemoryStream(asmResult.Documents[i].Data);
						if (docType == DocumentType.Native)
						{
							docType = Document.GetDocumentType(asmResult.Documents[i].FileName);
						}
						break;
				}
			}
			if (document != null)
			{
				result = new AssembleDocumentResult(
					new Document(template, document, docType, supportingFiles.ToArray(), asmResult.UnansweredVariables),
					ansRdr == null ? null : ansRdr.ReadToEnd(),
					pendingAssemblies,
					asmResult.UnansweredVariables
				);
			}
			return result;
		}

		private string GetRelativePath(string fullPath)
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
