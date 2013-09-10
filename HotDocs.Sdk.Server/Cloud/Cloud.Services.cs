/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Add XML comments where missing.
//TODO: Add method parameter validation.
//TODO: Add appropriate unit tests.

using HotDocs.Sdk.Cloud;
using HotDocs.Sdk.Server.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HotDocs.Sdk.Server.Cloud
{
	/// <summary>
	/// This is the implementation of IServices that utilizes HotDocs Cloud Services
	/// to perform its work.
	/// </summary>
	public class Services : IServices
	{
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

		#region IServices Members

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
			if (settings == null)
				settings = new InterviewSettings();

			string templateLocator = template.CreateLocator();

			// Add the query string to the interview image url so dialog element images can be located.
			settings.InterviewImageUrlQueryString = "?loc=" + templateLocator + "&img=";

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

				// Although interviewFiles could potentially contain more than one item, the only one we care about is the
				// first one, which is the HTML fragment. All other items, such as interview definitions (.JS and .DLL files)
				// or dialog element images are not needed, because we can get them out of the package file instead. 
				// We enforce this by setting the OmitImages and OmitDefinitions values above, so we will always have exactly one item here.
				Debug.Assert(interviewFiles.Length == 1); // TODO: Is Assert the right thing to be doing here?
				StringBuilder htmlFragment = new StringBuilder(Util.ExtractString(interviewFiles[0]));

				Util.AppendSdkScriptBlock(htmlFragment, template, settings);
				result.HtmlFragment = htmlFragment.ToString();

				// Replace the state string in the html fragment with a template locator.
				result.HtmlFragment = Regex.Replace(result.HtmlFragment, "stateString=[^&]+", "stateString=" + templateLocator);
			}

			return result;
		}

		/// <summary>
		/// Assembles a document from the given template, answers and options.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="settings"></param>
		/// <include file="../../Shared/Help.xml" path="Help/string/param[@name='logRef']"/>
		/// <returns></returns>
		public AssembleDocumentResult AssembleDocument(Template template, System.IO.TextReader answers, AssembleDocumentSettings settings, string logRef)
		{
			// Validate input parameters, creating defaults as appropriate.
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
				NamedStream[] supportingFiles = null;

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
					result = new AssembleDocumentResult(
						new Document(template, document, docType, supportingFiles, asmResult.UnansweredVariables),
						ansRdr.ReadToEnd(),
						pendingAssemblies.ToArray(),
						asmResult.UnansweredVariables
					);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns metadata about the variables/types (and optionally dialogs & mapping info)
		/// for the indicated template's interview.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="includeDialogs"></param>
		/// <param name="logRef"></param>
		/// <returns></returns>
		public ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string logRef)
		{
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
			string result = "";
			using (var client = new SoapClient(_subscriberID, _signingKey))
			{
				//TODO: use new GetBinaryObjectFromTextReader here
				BinaryObject[] answersObj = new BinaryObject[answers.Count()];
				int i = 0;
				foreach (TextReader tr in answers)
				{
					answersObj[i++] = new BinaryObject
					{
						Data = Encoding.UTF8.GetBytes(tr.ReadToEnd()),
						DataEncoding = "UTF-8"
					};
				}

				result = Util.ExtractString(client.GetAnswers(answersObj, logRef));
			}
			return result;
		}
		// TODO: Explain why it is not supported (not applicable) and also see if there's something better than an exception so people don't have problems switching from one service type to the other.
		/// <summary>
		/// Not supported in HotDocs Cloud Services.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="flags"></param>
		public void BuildSupportFiles(Template template, HDSupportFilesBuildFlags flags)
		{
			throw new NotSupportedException("BuildSupportFiles is not applicable to HotDocs Cloud Services.");
		}

		// TODO: Explain why it is not supported (not applicable) and also see if there's something better than an exception so people don't have problems switching from one service type to the other.
		/// <summary>
		/// Not supported in HotDocs Cloud Services.
		/// </summary>
		/// <param name="template"></param>
		public void RemoveSupportFiles(Template template)
		{
			throw new NotSupportedException("RemoveSupportFiles is not applicable to HotDocs Cloud Services.");
		}

		/// <summary>
		/// Returns the interview definition for the specified template.
		/// </summary>
		/// <param name="state">An encoded string used to find the original package associated with the requested interview.</param>
		/// <param name="templateFile">The name of the template for which the definition is being requested.</param>
		/// <param name="format">The format of the interview definition being requested (Silverlight or JavaScript).</param>
		/// <returns>A stream containing the requested interview definition.</returns>
		public System.IO.Stream GetInterviewDefinition(string state, string templateFile, InterviewFormat format)
		{
			Template t = Template.Locate(state);
			return t.Location.GetFile(templateFile + (format == InterviewFormat.Silverlight ? ".dll" : ".js"));
		}

		#endregion

		private string _subscriberID;
		private string _signingKey;

	}
}
