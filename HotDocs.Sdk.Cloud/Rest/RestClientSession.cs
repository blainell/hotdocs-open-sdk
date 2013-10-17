/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk.Server.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace HotDocs.Sdk.Cloud
{
	public sealed partial class RestClient : ClientBase
	{
		#region Public methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="billingRef"></param>
		/// <param name="answers"></param>
		/// <param name="markedVariables"></param>
		/// <param name="interviewFormat"></param>
		/// <param name="outputFormat"></param>
		/// <param name="settings"></param>
		/// <param name="theme"></param>
		/// <param name="showDownloadLinks"></param>
		/// <param name="docUrl"></param>
		/// <param name="altDocUrl"></param>
		/// <param name="postUrl"></param>
		/// <returns></returns>
		public string CreateSession(
			Template template,
			string billingRef = null,
			string answers = null,
			string[] markedVariables = null,
			InterviewFormat interviewFormat = InterviewFormat.JavaScript,
			OutputFormat outputFormat = OutputFormat.Native,
			Dictionary<string, string> settings = null,
			string theme = null,
			bool showDownloadLinks = true,
			string docUrl = null,
			string altDocUrl = null,
			string postUrl = null
		)
		{
			return (string)TryWithoutAndWithPackage(
				(uploadPackage) => CreateSessionImpl(
					template,
					billingRef,
					answers,
					markedVariables,
					interviewFormat,
					outputFormat,
					settings,
					theme,
					showDownloadLinks,
					docUrl,
					altDocUrl,
					postUrl,
					uploadPackage)
			);
		}

		/// <summary>
		/// Resumes a saved session.
		/// </summary>
		/// <param name="state">The serialized state of the interrupted session, i.e. the "snapshot".</param>
		/// <param name="locationGetter">A delegate that takes a package ID and returns the template location.</param>
		/// <returns>A session ID to be passed into the JavaScript HD$.CreateInterviewFrame call.</returns>
		public string ResumeSession(string state, Func<string, PackageTemplateLocation> locationGetter = null)
		{
			if (locationGetter != null)
			{
				return (string)TryWithoutAndWithPackage(
					(uploadPackage) => ResumeSessionImpl(state, locationGetter, uploadPackage));
			}
			return ResumeSessionImpl(state, locationGetter, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionId"></param>
		/// <param name="fileName"></param>
		/// <param name="localPath"></param>
		public void GetSessionDoc(string sessionId, string fileName, string localPath)
		{
			string url = string.Format("{0}/embed/session/{1}/docs/{2}", EndpointAddress, sessionId, fileName);
			using (var webClient = new WebClient())
			{
				webClient.DownloadFile(url, localPath);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionId"></param>
		/// <returns></returns>
		public string[] GetSessionDocList(string sessionId)
		{
			string url = string.Format("{0}/embed/session/{1}/docs", EndpointAddress, sessionId);
			string list;
			using (var webClient = new WebClient())
			{
				list = webClient.DownloadString(url);
			}
			return list.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sessionId"></param>
		/// <returns></returns>
		public string GetSessionState(string sessionId)
		{
			string url = string.Format("{0}/embed/session/{1}/state", EndpointAddress, sessionId);
			using (var webClient = new WebClient())
			{
				return webClient.DownloadString(url);
			}
		}
		#endregion

		#region Private methods
		private string CreateSessionImpl(
			Template template,
			string billingRef,
			string answers,
			string[] markedVariables,
			InterviewFormat interviewFormat,
			OutputFormat outputFormat,
			Dictionary<string, string> settings,
			string theme,
			bool showDownloadLinks,
			string docUrl,
			string altDocUrl,
			string postUrl,
			bool uploadPackage)
		{
			if (!(template.Location is PackageTemplateLocation))
				throw new Exception("HotDocs Cloud Services requires the use of template packages. Please use a PackageTemplateLocation derivative.");
			PackageTemplateLocation packageTemplateLocation = (PackageTemplateLocation)template.Location;

			if (uploadPackage)
			{
				UploadPackage(packageTemplateLocation.PackageID, billingRef, packageTemplateLocation.GetPackageStream());
			}

			var timestamp = DateTime.UtcNow;

			string hmac = HMAC.CalculateHMAC(
				SigningKey,
				timestamp,
				SubscriberId,
				packageTemplateLocation.PackageID,
				billingRef,
				interviewFormat,
				outputFormat,
				null); // Additional settings = null for this app

			StringBuilder urlBuilder = new StringBuilder(string.Format(
				"{0}/embed/newsession/{1}/{2}?interviewformat={3}&outputformat={4}",
				EndpointAddress, SubscriberId, packageTemplateLocation.PackageID,
				interviewFormat.ToString(), outputFormat.ToString()));

			if (markedVariables != null && markedVariables.Length > 0)
			{
				urlBuilder.AppendFormat("&markedvariables={0}", string.Join(",", markedVariables));
			}

			if (!string.IsNullOrEmpty(postUrl))
			{
				urlBuilder.AppendFormat("&posturl={0}", postUrl);
			}

			if (!string.IsNullOrEmpty(theme))
			{
				urlBuilder.AppendFormat("&theme={0}", theme);
			}

			if (!string.IsNullOrEmpty(billingRef))
			{
				urlBuilder.AppendFormat("&billingref={0}", billingRef);
			}

			if (showDownloadLinks)
			{
				urlBuilder.Append("&showdownloadlinks=true");
			}

			if (!string.IsNullOrEmpty(altDocUrl))
			{
				urlBuilder.AppendFormat("&altdocurl={0}", altDocUrl);
			}

			if (!string.IsNullOrEmpty(docUrl))
			{
				urlBuilder.AppendFormat("&docurl={0}" + docUrl);
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlBuilder.ToString());
			request.Method = "POST";
			request.ContentType = "text/xml";
			request.Headers["x-hd-date"] = timestamp.ToString("r");
			request.Headers[HttpRequestHeader.Authorization] = hmac;
			request.ContentLength = answers != null ? answers.Length : 0;

			if (!string.IsNullOrEmpty(ProxyServerAddress))
			{
				request.Proxy = new WebProxy(ProxyServerAddress);
			}
			else
			{
				request.Proxy = null;
			}

			Stream stream = request.GetRequestStream();
			if (answers != null)
			{
				byte[] data = Encoding.UTF8.GetBytes(answers);
				stream.Write(data, 0, data.Length);
			}
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());
			return reader.ReadLine();
		}

		private string ResumeSessionImpl(string state, Func<string, PackageTemplateLocation> locationGetter, bool uploadPackage)
		{
			if (uploadPackage)
			{
				string base64 = state.Split('#')[0];
				string json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
				JavaScriptSerializer jss = new JavaScriptSerializer();
				var stateDict = jss.Deserialize<dynamic>(json);
				string packageID = stateDict["PackageID"];
				string billingRef = stateDict["BillingRef"];

				UploadPackage(packageID, billingRef, locationGetter(packageID).GetPackageStream());
			}

			var timestamp = DateTime.UtcNow;

			string hmac = HMAC.CalculateHMAC(
				SigningKey,
				timestamp,
				SubscriberId,
				state);

			string url = string.Format("{0}/embed/resumesession/{1}", EndpointAddress, SubscriberId);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "text/xml";
			request.Headers["x-hd-date"] = timestamp.ToString("r");
			request.Headers[HttpRequestHeader.Authorization] = hmac;
			request.ContentLength = state.Length;

			if (!string.IsNullOrEmpty(ProxyServerAddress))
			{
				request.Proxy = new WebProxy(ProxyServerAddress);
			}
			else
			{
				request.Proxy = null;
			}

			Stream stream = request.GetRequestStream();
			byte[] data = Encoding.UTF8.GetBytes(state);
			stream.Write(data, 0, data.Length);

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			StreamReader reader = new StreamReader(response.GetResponseStream());
			return reader.ReadLine();
		}
		#endregion
	}
}
