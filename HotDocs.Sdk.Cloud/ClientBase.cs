/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Contracts;
using System.IO;
using System.Configuration;

namespace HotDocs.Sdk.Cloud
{
	/// <summary>
	/// An abstract base class for clients that communicate with HotDocs Cloud Services.
	/// </summary>
	public abstract class ClientBase
	{
		#region Constructors

		/// <summary>
		/// A constructor used to create a client that communicates with HotDocs Cloud Services.
		/// </summary>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='subscriberID']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='signingKey']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='hostAddress']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='servicePath']"/>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='proxyServerAddress']"/>
		protected internal ClientBase(
			string subscriberID,
			string signingKey,
			string hostAddress,
			string servicePath,
			string proxyServerAddress)
		{
			if (hostAddress == null)
			{
				hostAddress = ConfigurationManager.AppSettings["CloudServicesAddress"];
				if (string.IsNullOrEmpty(hostAddress))
					hostAddress = "https://cloud.hotdocs.ws";
			}
			SubscriberId = subscriberID;
			SigningKey = signingKey;
			EndpointAddress = hostAddress + servicePath;
			ProxyServerAddress = proxyServerAddress;
		}
		#endregion

		#region Public properties

		/// <summary>
		/// <include file="../Shared/Help.xml" path="Help/string/param[@name='subscriberID']/child::node()"/>
		/// </summary>
		public string SubscriberId { get; set; }

		/// <summary>
		/// The Subscriber's unique signing key.
		/// </summary>
		public string SigningKey { get; set; }

		/// <summary>
		/// Specifies an alternate address for the Cloud Services web service (e.g., https://127.0.0.1).
		/// </summary>
		public string EndpointAddress { get; set; }

		/// <summary>
		/// Specifies the proxy server address (e.g., http://myfiddlermachine:8888).
		/// </summary>
		public string ProxyServerAddress { get; set; }
		#endregion

		#region Public methods

		/// <summary>
		/// Assembles a document from the specified template and answersets.
		/// </summary>
		/// <param name="template">The template to use with the request.</param>
		/// <param name="answers">The answers to use with the request.</param>
		/// <param name="options">The options to use when assembling the document.</param>
		/// <param name="billingRef">This parameter lets you specify information that will be included in usage logs for this call. For example, you can use a string to uniquely identify the end user that initiated the request and/or the context in which the call was made. When you review usage logs, you can then see which end users initiated each request. That information could then be used to pass costs on to those end users if desired.</param>
		/// <returns></returns>
		public AssemblyResult AssembleDocument(Template template, string answers, AssembleDocumentSettings options, string billingRef)
		{
			return (AssemblyResult)TryWithoutAndWithPackage(
				uploadPackage => AssembleDocumentImpl(template, answers, options, billingRef, uploadPackage));
		}

		/// <summary>
		/// Gets an interview for the specified template.
		/// </summary>
		/// <param name="template">The template to use with the request.</param>
		/// <param name="answers">The answers to use with the request.</param>
		/// <param name="options">The options to use when getting an interview.</param>
		/// <param name="billingRef">This parameter lets you specify information that will be included in usage logs for this call. For example, you can use a string to uniquely identify the end user that initiated the request and/or the context in which the call was made. When you review usage logs, you can then see which end users initiated each request. That information could then be used to pass costs on to those end users if desired.</param>
		/// <returns></returns>
		public BinaryObject[] GetInterview(Template template, string answers, InterviewSettings options, string billingRef)
		{
			return (BinaryObject[])TryWithoutAndWithPackage(
				uploadPackage => GetInterviewImpl(template, answers, options, billingRef, uploadPackage));
		}

		/// <summary>
		/// Gets component information for the specified template.
		/// </summary>
		/// <param name="template">Template to assemble</param>
		/// <param name="includeDialogs">This indicates whether or not the returned data should include information about dialogs and their contents.</param>
		/// <param name="billingRef">This parameter lets you specify information that will be included in usage logs for this call. For example, you can use a string to uniquely identify the end user that initiated the request and/or the context in which the call was made. When you review usage logs, you can then see which end users initiated each request. That information could then be used to pass costs on to those end users if desired.</param>
		/// <returns></returns>
		public ComponentInfo GetComponentInfo(Template template, bool includeDialogs, string billingRef)
		{
			return (ComponentInfo)TryWithoutAndWithPackage(
				uploadPackage => GetComponentInfoImpl(template, includeDialogs, billingRef, uploadPackage));
		}

		/// <summary>
		/// Combines the provided answersets to form an aggregate answerset.
		/// </summary>
		/// <param name="answers">The answers to use with the request.</param>
		/// <param name="billingRef">This parameter lets you specify information that will be included in usage logs for this call. For example, you can use a string to uniquely identify the end user that initiated the request and/or the context in which the call was made. When you review usage logs, you can then see which end users initiated each request. That information could then be used to pass costs on to those end users if desired.</param>
		/// <returns></returns>
		public BinaryObject GetAnswers(BinaryObject[] answers, string billingRef)
		{
			return GetAnswersImpl(answers, billingRef);
		}
		#endregion

		#region Protected internal abstract methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="options"></param>
		/// <param name="billingRef"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal abstract AssemblyResult AssembleDocumentImpl(
			Template template,
			string answers,
			AssembleDocumentSettings options,
			string billingRef,
			bool uploadPackage);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="options"></param>
		/// <param name="billingRef"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal abstract BinaryObject[] GetInterviewImpl(
			Template template,
			string answers,
			InterviewSettings options,
			string billingRef,
			bool uploadPackage);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="includeDialogs"></param>
		/// <param name="billingRef"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal abstract ComponentInfo GetComponentInfoImpl(
			Template template,
			bool includeDialogs,
			string billingRef,
			bool uploadPackage);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="answers"></param>
		/// <param name="billingRef"></param>
		/// <returns></returns>
		protected internal abstract BinaryObject GetAnswersImpl(
			BinaryObject[] answers,
			string billingRef);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="func"></param>
		/// <returns></returns>
		protected internal abstract object TryWithoutAndWithPackage(Func<bool, object> func);
		#endregion

		#region Protected internal methods

		/// <summary>
		/// 
		/// </summary>
		protected internal void SetTcpKeepAlive()
		{
			// Turn on TCP keep-alive, and set the keep-alive time to 50 seconds.
			// This will ensure that the Azure load balancer doesn't terminate the connection.
			// Note that this will not work if the web requests go through a proxy server.
			var servicePoint = ServicePointManager.FindServicePoint(new Uri(EndpointAddress));
			if (servicePoint != null)
			{
				servicePoint.SetTcpKeepAlive(true, 50 * 1000, 10 * 1000);
				servicePoint.ConnectionLimit = 128;
			}
			else
			{
				ServicePointManager.SetTcpKeepAlive(true, 50 * 1000, 10 * 1000);
				ServicePointManager.DefaultConnectionLimit = 128;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="location"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal BinaryObject GetPackageIfNeeded(PackageTemplateLocation location, bool uploadPackage)
		{
			if (!uploadPackage)
				return null;

			using (Stream stream = location.GetPackageStream())
			{
				byte[] data;
				if (stream.CanSeek)
				{
					data = new byte[stream.Length];
					using (MemoryStream memStream = new MemoryStream(data))
					{
						stream.CopyTo(memStream);
					}
				}
				else
				{
					// This is very inefficient, but what are you gonna do?
					using (MemoryStream memStream = new MemoryStream())
					{
						stream.CopyTo(memStream);
						data = memStream.ToArray();
					}
				}

				return new BinaryObject()
				{
					FileName = "",
					DataEncoding = null,
					Data = data
				};
			}
		}

		#endregion
	}
}
