/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace HotDocs.Sdk.Server.Contracts
{
	/// <summary>
	/// The Core service contract
	/// </summary>
	[ServiceContract(Namespace = "http://hotdocs.com/contracts/")]
	public interface ICore
	{
		/// <summary>
		/// 
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='subscriberID']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='packageID']"/>
		/// <param name="templateName"></param>
		/// <param name="answers"></param>
		/// <param name="format"></param>
		/// <param name="outputOptions"></param>
		/// <param name="settings"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='billingRef']"/>
		/// <include file="../Shared/Help.xml" path="Help/DateTime/param[@name='timestamp']"/>
		/// <param name="templatePackage"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='hmac']"/>
		/// <returns></returns>
		[ServiceKnownType(typeof(BasicOutputOptions))]
		[ServiceKnownType(typeof(PdfOutputOptions))]
		[OperationContract]
		AssemblyResult AssembleDocument(
			string subscriberID,
			string packageID,
			string templateName,
			BinaryObject[] answers,
			OutputFormat format,
			OutputOptions outputOptions,
			Dictionary<string, string> settings,
			string billingRef,
			DateTime timestamp,
			BinaryObject templatePackage,
			string hmac);

		/// <summary>
		/// 
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='subscriberID']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='packageID']"/>
		/// <param name="templateName"></param>
		/// <param name="answers"></param>
		/// <param name="format"></param>
		/// <param name="markedVariables"></param>
		/// <param name="tempImageUrl"></param>
		/// <param name="settings"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='billingRef']"/>
		/// <include file="../Shared/Help.xml" path="Help/DateTime/param[@name='timestamp']"/>
		/// <param name="templatePackage"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='hmac']"/>
		/// <returns></returns>
		[OperationContract]
		BinaryObject[] GetInterview(
			string subscriberID,
			string packageID,
			string templateName,
			BinaryObject[] answers,
			InterviewFormat format,
			string[] markedVariables,
			string tempImageUrl,
			Dictionary<string, string> settings,
			string billingRef,
			DateTime timestamp,
			BinaryObject templatePackage,
			string hmac);

		/// <summary>
		/// This web service retrieves metadata about variables and optionally dialog structure. The variable information
		/// is essentially the same as what is found in the .HVC file created when you publish a template for use with HotDocs Server,
		/// and the dialog information is a list of the dialog's variables and any answer source mappings that may exist for the dialog.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='subscriberID']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='packageID']"/>
		/// <param name="templateName">The file name of the template.</param>
		/// <param name="includeDialogs">This indicates whether or not the returned data should include information about dialogs and their contents.</param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='billingRef']"/>
		/// <include file="../Shared/Help.xml" path="Help/DateTime/param[@name='timestamp']"/>
		/// <param name="templatePackage"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='hmac']"/>
		/// <returns>A <c>ComponentInfo</c> object with information about each variable (and optionally dialog) in the template's interview.</returns>
		[OperationContract]
		ComponentInfo GetComponentInfo(
			string subscriberID,
			string packageID,
			string templateName,
			bool includeDialogs,
			string billingRef,
			DateTime timestamp,
			BinaryObject templatePackage,
			string hmac);

		/// <summary>
		/// Combines the provided answersets to form an aggregate answerset.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='subscriberID']"/>
		/// <param name="answers"></param>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='billingRef']"/>
		/// <include file="../Shared/Help.xml" path="Help/DateTime/param[@name='timestamp']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='hmac']"/>
		/// <returns></returns>
		[OperationContract]
		BinaryObject GetAnswers(
			string subscriberID,
			BinaryObject[] answers,
			string billingRef,
			DateTime timestamp,
			string hmac);
	}
}
