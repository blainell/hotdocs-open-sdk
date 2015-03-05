/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace HotDocs.Sdk.Server.Contracts
{
	/// <summary>
	/// The following web methods are available in the basic HotDocs Server web service:
	/// </summary>
	[ServiceContract(Namespace="http://hotdocs.com/contracts/")]
	public interface IHDSvc
	{
		/// <summary>
		/// This web service retrieves metadata about variables and optionally dialog structure. The variable information
		/// is essentially the same as what is found in the .HVC file created when you publish a template for use with HotDocs Server,
		/// and the dialog information is a list of the dialog's variables and any answer source mappings that may exist for the dialog.
		/// </summary>
		/// <param name="templateID">The file name of the template.</param>
		/// <param name="includeDialogs">This indicates whether or not the returned data should include information about dialogs
		/// and their contents.</param>
		/// <returns>A <c>ComponentInfo</c> object with information about each variable (and optionally dialog) in the template's interview.</returns>
		[OperationContract]
		ComponentInfo GetComponentInfo(string templateID, bool includeDialogs);

		/// <summary>
		/// This web service returns the title and description for the specified template(s).
		/// </summary>
		/// <param name="templateIDs">This is an array of the template names for which template information is being requested.</param>
		/// <returns>An array of <c>TemplateInfo</c> objects, which contain each template's title and description.</returns>
		[OperationContract]
		TemplateInfo[] GetTemplateInfo(string[] templateIDs);

		/**
		<summary>
		Returns an HTML fragment containing the HotDocs interview for a given template. This fragment is suitable for insertion
		anywhere in the &lt;body&gt; element of an HTML page, and it takes care of loading all necessary external resources (scripts, css files, etc.).
		<para>See HotDocs Server help file for browser requirements for the different types of interviews.</para>
		<para>Silverlight interviews are suitable for insertion into most web pages (both standards-compliant and "quirks" mode), while JavaScript interviews currently require quirks mode on Internet Explorer 6 and later.</para>
		<para>Caller can omit:
		<list type="bullet">
		<item><description><c>saveAnswersUrl</c> to disable save answers button.</description></item>
		<item><description><c>previewUrl</c> to disable document previews.</description></item>
		<item><description><c>interviewDefinitionUrl</c> is required for Silverlight interviews and ignored for JavaScript interviews.</description></item>
		</list></para>
		</summary>
		<param name="templateID">The file name of the template.</param>
		<param name="answers">List of answer collections to be overlaid successively and used to pre-populate the interview. Each element in this array can be either an XML answer collection or an encoded answer set as posted from a browser interview.</param>
		<param name="format">A value from the <c>InterviewFormat</c> enumeration, indicating whether you are requesting a Silverlight or JavaScript interview.</param>
		<param name="options">One or more values from the <c>InterviewOptions</c> enumeration.</param>
		<param name="markedVariables">An array of variable names that should be "marked" or highlighted in the interview. Typically used to highlight unanswered variables when returning to a previously-submitted interview.</param>
		<param name="formActionUrl">The URL to which interview results will be posted when the user clicks "Finish". Sometimes referred to as the "disposition page."</param>
		<param name="serverFilesUrl">The base URL from which the HotDocs Server JavaScript files will be requested.</param>
		<param name="styleUrl">The URL of the user CSS file to be used. Note that the hdssystem.css style sheet must also be available in the same directory. Also, for Silverlight interviews, a .xaml style sheet with the same base file name and in the same directory will also be loaded.</param>
		<param name="tempInterviewUrl">The base URL from which this interview's custom images will be requested. It is the caller's responsibility to make any images returned from this web service available at this base URL (to which the appropriate file name will be appended).</param>
		<param name="saveAnswersUrl">The URL to which requests to save answers will be posted (null to disable save answers).</param>
		<param name="previewUrl">The URL to which requests for document previews will be posted (null to disable preview). (todo: document URL format)</param>
		<param name="interviewDefinitionUrl">The URL from which the interview will request compiled Silverlight interview DLLs (required for Silverlight interviews). (todo: document URL fromat)</param>
		<returns>Returns a <c>BinaryObject</c> array. The first object in this array is the HTML code that defines the HotDocs interview. Additional objects in the array, if any, are other files used by the interview, such as images referred to by various dialog elements.</returns>
		*/
		[OperationContract]
		BinaryObject[] GetInterview(string templateID, BinaryObject[] answers, InterviewFormat format, InterviewOptions options,
			string[] markedVariables, string formActionUrl, string serverFilesUrl, string styleUrl,
			string tempInterviewUrl, string saveAnswersUrl, string previewUrl, string interviewDefinitionUrl);

		/// <summary>
		/// This web service returns an interview definition file for the specified template.
		/// </summary>
		/// <remarks>This is necessary to facilitate Silverlight interviews working over web services. Callers must specify EITHER a templateID (which refers to a template file in the default template directory) OR a templateName and templateState.</remarks>
		/// <param name="templateID">The file name and extension for the template.  Caller should pass templateID or a templateState, but not both.</param>
		/// <param name="templateName">The name of the template whose interview definition is being requested.  Unnecessary if you have specified templateID.</param>
		/// <param name="format">Indicates the format whether to request a Silverlight or JavaScript interview definition.
		/// The only format currently supported is Silverlight, which means this web service will return a
		/// compiled Silverlight interview DLL.</param>
		/// <param name="templateState">An encrypted string (originating from a browser interview) identifying a template. Caller should pass templateID or templateState, but not both.</param>
		/// <returns>A <c>BinaryObject</c> with the requested interview definition.</returns>
		[OperationContract]
		BinaryObject GetInterviewDefinition(string templateID, string templateName, InterviewFormat format, string templateState);

		/// <summary>
		/// This web service returns a <c>BinaryObject</c> containing an answer collection created by successively overlaying each of the 
		/// supplied answer collections on top of each other.
		/// </summary>
		/// <remarks>This web method will overlay an array of answer collections.</remarks>
		/// <param name="answers">An array of answer collections. Each element in this array can be either an XML answer collection or an encoded answer set as posted from a browser interview.</param>
		/// <returns>A <c>BinaryObject</c> containing a single XML answer file.</returns>
		[OperationContract]
		BinaryObject GetAnswers(BinaryObject[] answers);

		/// <summary>
		/// This web service assembles a template using the provided answers.
		/// </summary>
		/// <remarks>For many template types, HotDocs Server supports multiple output formats, 
		/// and you can use one call to this web service to receive the same document in multiple formats.</remarks>
		/// <param name="templateID">The file name (not including the path) of the template to assemble.</param>
		/// <param name="answers">An array of answer collections to be used to assemble the document.
		/// If more than one answer collection is included, they are successively overlaid on top of each other before assembly.
		/// Each element in this array can be either an XML answer collection or an encoded answer set as posted from a browser interview.</param>
		/// <param name="format">This parameter determines the document format(s) to be delivered.
		/// It can be one or more compatible values from the <c>OutputFormat</c> enumeration.</param>
		/// <param name="options">One or more compatible values from the <c>AssemblyOptions</c> enumeration.</param>
		/// <param name="templateState">This parameter is either null if the template is located in the web service's default template
		/// directory (as specified in web.config), or it is an encrypted value encapsulating the location of the template.</param>
		/// <returns>An <c>AssemblyResult</c> object containing the assembled document in the requested output format(s),
		/// the resulting answer file (if requested), and information about unanswered variables and the assembly queue.</returns>
		[OperationContract]
		AssemblyResult AssembleDocument(string templateID, BinaryObject[] answers, OutputFormat format, AssemblyOptions options,
			string templateState);

		/// <summary>
		/// This web service builds the manifest and other server files for a template.
		/// </summary>
		/// <remarks>Usually, a manifest is built on the desktop with HotDocs Developer when publishing a template. When a template is installed on a server without a manifest, this function may be called to build the manifest. HotDocsServer will build and cache the JavaScript or Silverlight interview files upon first use. BuildSupportFiles may be used to prebuild the interview files so that they are already in the cache the first time they are requested.</remarks>
		/// <param name="templateID">The file name (not including the path) of the template to assemble.</param>
		/// <param name="templateKey">Version-specific identifier for the parent template, for templates not at a fixed location.</param>
		/// <param name="buildFlags">Specifies which files (besides the manifest) to build. See HDSupportFilesBuildFlags for details.</param>
		/// <param name="templateState">This parameter is either null if the template is located in the web service's default template
		/// directory (as specified in web.config), or it is an encrypted value encapsulating the location of the template.</param>
		[OperationContract]
		void BuildSupportFiles(string templateID, string templateKey, HDSupportFilesBuildFlags buildFlags, string templateState);

		/// <summary>
		/// This web service removes the manifest and other server files for a template.
		/// </summary>
		/// <param name="templateID">The file name (not including the path) of the template to assemble.</param>
		/// <param name="templateKey">Version-specific identifier for the parent template, for templates not at a fixed location.</param>
		/// <param name="templateState">This parameter is either null if the template is located in the web service's default template
		/// directory (as specified in web.config), or it is an encrypted value encapsulating the location of the template.</param>
		[OperationContract]
		void RemoveSupportFiles(string templateID, string templateKey, string templateState);
	}
}