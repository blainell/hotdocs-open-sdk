/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

	// TODO: Move all of the shared XML comments into a separate file and link to them in this code and all
	// derived class files.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.Contracts;

namespace HotDocs.Sdk.Server
{
	/// <summary>
	/// This is a stateless, object-oriented interface for basic communications with HotDocs Server, including
	/// getting interviews, assembling documents, and other simple, stateless operations.
	/// </summary>
	public interface IServices
	{
		/// <summary>
		/// GetInterview returns an HTML fragment suitable for inclusion in any standards-mode web page, which embeds a HotDocs interview
		/// directly in that web page.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="settings"></param>
		/// <param name="markedVariables"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='logRef']"/>
		/// <returns></returns>
		InterviewResult GetInterview(Template template, TextReader answers, InterviewSettings settings, IEnumerable<string> markedVariables, string logRef);

		/// <summary>
		/// Assemble a document from the given template, answers and settings.
		/// </summary>
		/// <param name="template">An instance of the Template class.</param>
		/// <param name="answers">Either an XML answer string, or a string containing encoded
		/// interview answers as posted from a HotDocs browser interview.</param>
		/// <param name="settings">An instance of the AssembleDocumentResult class.</param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='logRef']"/>
		/// <returns>An AssemblyResult object containing all the files and data resulting from the request.</returns>
		AssembleDocumentResult AssembleDocument(Template template, TextReader answers, AssembleDocumentSettings settings, string logRef);

		/// <summary>
		/// GetComponentInfo returns metadata about the variables/types (and optionally dialogs and mapping info)		/// for the indicated template's interview.
		/// </summary>
		/// <param name="template">An instance of the Template class, for which you are requesting component information.</param>
		/// <param name="includeDialogs">Whether to include dialog &amp; mapping information in the returned results.</param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='logRef']"/>
		/// <returns>The requested component information.</returns>
		ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string logRef);

		/// <summary>
		/// This method overlays any answer collections passed into it, into a single XML answer collection.
		/// It has two primary uses: it can be used to combine multiple answer collections into a single
		/// answer collection; and/or it can be used to "resolve" or standardize an answer collection
		/// submitted from a browser interview (which may be specially encoded) into standard XML answers.
		/// </summary>
		/// <param name="answers">A sequence of answer collections. Each member of this sequence
		/// must be either an (encoded) interview answer collection or a regular XML answer collection.
		/// Each member will be successively overlaid (overlapped) on top of the prior members to
		/// form one consolidated answer collection.</param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='logRef']"/>
		/// <returns>The consolidated XML answer collection.</returns>
		string GetAnswers(IEnumerable<TextReader> answers, string logRef);

		/// <summary>
		/// Build the server files for the specified template.
		/// </summary>
		/// <param name="template"></param>
		/// <param name="flags"></param>
		void BuildSupportFiles(Template template, HDSupportFilesBuildFlags flags);

		/// <summary>
		/// Remove the server files for the specified template.
		/// </summary>
		/// <param name="template"></param>
		void RemoveSupportFiles(Template template);

		// NOTE: The idea with GetInterviewDefinition and GetFile below is to be as stateless as possible, meaning:
		//    we want to require as little from the host application as possible, in regard to managing our state for us.
		//    So when the browser interview makes a request from a host app proxy page, the host app should ideally be able
		//    to pass the information it gets on to the SDK, and be giving the SDK everything it needs, rather than
		//    necessarily persisting a "session" object server-side to manage state.

		/// <summary>
		/// Retrieve an interview definition.
		/// </summary>
		/// <param name="state">The template state string, passed as "state" on the query string by the browser interview.</param>
		/// <param name="templateFile">The template file name, passed as "template" on the query string by the browser interview.</param>
		/// <param name="format">The requested format of interview definition, according to the "type" query string parameter.
		/// If type=="js", pass JavaScript; if type=="dll", pass Silverlight; otherwise pass Default.</param>
		/// <returns>A stream containing the requested interview definition, to be returned to the caller.</returns>
		Stream GetInterviewDefinition(string state, string templateFile, InterviewFormat format);
		// TODO: we should also probably return the necessary MIME type for the returned data.  Maybe we need a
		// InterviewDefinitionResult type?
		// Q: Should this somehow be combined with GetFile below?
		// Host apps working with Local or WS can use state, host apps working with CS... state is not so useful.  The templatelocator is needed.

		// TODO: should GetInterviewDefinition and GetFile be combined into the same thing?  The problem is, one takes a
		// stateString (as emitted by HotDocs Server itself), and the other takes a templateLocator (as specified by the SDK).
		// Q: How should the templateLocator get included as part of the request (from the browser to the host app) for a graphic?
		//    Could the templateLocator also be included in the same way when it requests an interview definition?

		// Get the template manifest for the specified template. Can optionally parse an entire template manifest spanning tree.
		//TemplateManifest GetManifest(string templateLocator, string templateFileName, ManifestParseFlags parseFlags);
		// Q: should TemplateManifest be enhanced to have everything GetComponentInfo needs?  (in other words, dialogs?)
	}
}
