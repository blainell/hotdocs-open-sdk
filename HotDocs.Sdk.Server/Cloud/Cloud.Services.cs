/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk.Cloud;
using HotDocs.Sdk.Server.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HotDocs.Sdk.Server.Cloud
{
	/// <summary>
	/// This is the implementation of IServices that utilizes HotDocs Cloud Services to perform its work.
	/// </summary>
	public class Services : IServices
	{
		#region Private Members

		private string _subscriberID;
		private string _signingKey;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Constructor for Services, which requires the subscriber's ID and signing key.
		/// These are necessary to authenticate requests sent to HotDocs Cloud Services.
		/// </summary>
		/// <param name="subscriberId">A HotDocs Cloud Services subscriber ID.</param>
		/// <param name="signingKey">The signing key associated with the subscriber ID.</param>
		public Services(string subscriberId, string signingKey)
		{
			if (string.IsNullOrWhiteSpace(subscriberId))
				throw new ArgumentNullException("The subscriber ID is missing. Please check the value for SubscriberID in the config file and try again.");

			if (string.IsNullOrWhiteSpace(signingKey))
				throw new ArgumentNullException("The signing key is missing. Please check the value for SigningKey in the config file and try again.");

			_subscriberID = subscriberId;
			_signingKey = signingKey;
		}

		#endregion

		#region Public IServices Members

		/// <summary>
		/// Returns an HTML fragment suitable for inclusion in any standards-mode web page, which embeds a HotDocs interview
		/// directly in that web page.
		/// </summary>
		/// <param name="template">The template for which the interview will be requested.</param>
		/// <param name="answers">The initial set of answers to include in the interview.</param>
		/// <param name="settings">Settings that define various interview behavior.</param>
		/// <param name="markedVariables">A collection of variables that should be marked with special formatting in the interview.</param>
		/// <param name="logRef">A string to display in logs related to this request.</param>
		/// <returns>An object which contains an HTML fragment to be inserted in a web page to display the interview.</returns>
		public InterviewResult GetInterview(Template template, TextReader answers, InterviewSettings settings, IEnumerable<string> markedVariables, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			string logStr = logRef == null ? string.Empty : logRef;

			if (template == null)
				throw new ArgumentNullException("template", string.Format(@"Cloud.Services.GetInterview: the ""template"" parameter passed in was null, logRef: {0}", logStr));

			if (settings == null)
				settings = new InterviewSettings();

			// Set the template locator setting so the interview will know where to find images and interview definitions.
			settings.TemplateLocator = template.CreateLocator();

			// Configure interview settings
			settings.Settings["OmitImages"] = "true"; // Instructs HDS not to return images used by the interview; we'll get them ourselves from the template folder.
			settings.Settings["OmitDefinitions"] = "true"; // Instructs HDS not to return interview definitions; we'll get them ourselves from the template folder.
			settings.MarkedVariables = (string[])(markedVariables ?? new string[0]);

			// Get the interview.
			InterviewResult result = new InterviewResult();
			BinaryObject[] interviewFiles = null;
			using (var client = new SoapClient(_subscriberID, _signingKey))
			{
				interviewFiles = client.GetInterview(
					template,
					answers == null ? "" : answers.ReadToEnd(),
					settings,
					logRef
				);

				// Throw an exception if we do not have exactly one interview file.
				// Although interviewFiles could potentially contain more than one item, the only one we care about is the
				// first one, which is the HTML fragment. All other items, such as interview definitions (.JS and .DLL files)
				// or dialog element images are not needed, because we can get them out of the package file instead. 
				// We enforce this by setting the OmitImages and OmitDefinitions values above, so we will always have exactly one item here.
				if (interviewFiles.Length != 1)
					throw new Exception();

				StringBuilder htmlFragment = new StringBuilder(Util.ExtractString(interviewFiles[0]));

				Util.AppendSdkScriptBlock(htmlFragment, template, settings);
				result.HtmlFragment = htmlFragment.ToString();
			}

			return result;
		}

		/// <summary>
		/// Assembles a document from the given template, answers and settings.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="settings"></param>
		/// <include file="../../Shared/Help.xml" path="Help/string/param[@name='logRef']"/>
		/// <returns></returns>
		public AssembleDocumentResult AssembleDocument(Template template, System.IO.TextReader answers, AssembleDocumentSettings settings, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			string logStr = logRef == null ? string.Empty : logRef;
			if (template == null)
				throw new ArgumentNullException("template", "The template must not be null, logRef: " + logStr);

			if (settings == null)
				settings = new AssembleDocumentSettings();

			AssembleDocumentResult result = null;
			using (var client = new SoapClient(_subscriberID, _signingKey))
			{
				AssemblyResult asmResult = client.AssembleDocument(
					template,
					answers == null ? "" : answers.ReadToEnd(),
					settings,
					logRef
				);

				MemoryStream document = null;
				StreamReader ansRdr = null;
				DocumentType docType = settings.Format;
				List<NamedStream> supportingFiles = new List<NamedStream>();

				// Build the list of pending assemblies.
				List<Template> pendingAssemblies = new List<Template>();
				if (asmResult.PendingAssemblies != null)
				{
					foreach (PendingAssembly asm in asmResult.PendingAssemblies)
					{
						pendingAssemblies.Add(new Template(asm.TemplateName, template.Location.Duplicate(), asm.Switches));
					}
				}

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
						ansRdr.ReadToEnd(),
						pendingAssemblies.ToArray(),
						asmResult.UnansweredVariables
					);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns metadata about the variables/types (and optionally dialogs &amp; mapping info)
		/// for the indicated template's interview.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="includeDialogs"></param>
		/// <param name="logRef"></param>
		/// <returns></returns>
		public ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			string logStr = logRef == null ? string.Empty : logRef;
			if (template == null)
				throw new ArgumentNullException("template", @"Cloud.Services.GetComponentInfo: The ""template"" parameter must not be null, logRef: " + logStr);

			ComponentInfo result;
			using (var client = new SoapClient(_subscriberID, _signingKey))
			{
				result = client.GetComponentInfo(template, includeDialogs, logRef);
			}
			return result;
		}

		/// <summary>
		/// This method overlays any answer collections passed into it, into a single XML answer collection.
		/// </summary>
		/// <param name="answers"></param>
		/// <param name="logRef"></param>
		/// <returns>The consolidated XML answer collection.</returns>
		public string GetAnswers(IEnumerable<System.IO.TextReader> answers, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
			string logStr = logRef == null ? string.Empty : logRef;
			if (answers == null)
				throw new ArgumentNullException("answers", "The answers collection must not be null, logRef: " + logStr);

			BinaryObject combinedAnswers;
			using (SoapClient client = new SoapClient(_subscriberID, _signingKey))
			{
				var answerObjects = (from answer in answers select Util.GetBinaryObjectFromTextReader(answer)).ToArray();
				combinedAnswers = client.GetAnswers(answerObjects, logRef);
			}
			return Util.ExtractString(combinedAnswers);
		}

		/// <summary>
		/// This method does nothing in the case of HotDocs Cloud Services because the template package already contains all 
		/// of the interview runtime ("support") files required to display an interview for the template. These files are built
		/// by HotDocs Developer at the time the package is created, and Cloud Services does not have the ability to re-create them.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="flags"></param>
		public void BuildSupportFiles(Template template, HDSupportFilesBuildFlags flags)
		{
			if (template == null)
				throw new ArgumentNullException("template", @"Cloud.Services.BuildSupportFiles: the ""template"" parameter passed in was null");
			// no op
		}

		/// <summary>
		/// This method does nothing in the case of HotDocs Cloud Services because the template package already contains all 
		/// of the interview runtime ("support") files required to display an interview for the template. These files are built
		/// by HotDocs Developer at the time the package is created, and Cloud Services simply uses the files from the package
		/// rather than building and caching them separately.
		/// </summary>
		/// <param name="template"></param>
		public void RemoveSupportFiles(Template template)
		{
			if (template == null)
				throw new ArgumentNullException("template", @"Cloud.Services.RemoveSupportFiles: the ""template"" parameter passed in was null");
			// no op
		}

		/// <summary>
		/// Retrieves a file required by the interview. This could be either an interview definition that contains the 
		/// variables and logic required to display an interview (questionaire) for the main template or one of its 
		/// inserted templates, or it could be an image file displayed on a dialog within the interview.
		/// </summary>
		/// <param name="templateLocator">A template locator string used to locate the template related to the requested file.</param>
		/// <param name="fileName">The file name of the image, or the file name of the template for which the interview
		/// definition is being requested. In either case, this value is passed as "template" on the query string by the browser interview.</param>
		/// <param name="fileType">The type of file being requested: img (image file), js (JavaScript interview definition), 
		/// or dll (Silverlight interview definition).</param>
		/// <returns>A stream containing the requested interview file, to be returned to the caller.</returns>
		public Stream GetInterviewFile(string templateLocator, string fileName, string fileType)
		{
			// Validate input parameters, creating defaults as appropriate.
			if (string.IsNullOrEmpty(templateLocator))
				throw new ArgumentNullException("templateLocator", @"Cloud.Services.GetInterviewFile: the ""templateLocator"" parameter passed in was null or empty");

			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName", @"Cloud.Services.GetInterviewFile: the ""fileName"" parameter passed in was null or empty");

			if (string.IsNullOrEmpty(fileType))
				throw new ArgumentNullException("fileType", @"Cloud.Services.GetInterviewFile: the ""fileType"" parameter passed in was null or empty");

			// Locate the template, which we will use to find the image or interview definition file.
			Template template = Template.Locate(templateLocator);

			// Return an image or interview definition from the template.
			return template.Location.GetFile(fileName + (fileType.ToLower() == "img" ? "" : "." + fileType));
		}

		#endregion

	}
}
