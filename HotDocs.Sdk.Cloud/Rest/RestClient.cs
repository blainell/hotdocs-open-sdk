/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using HotDocs.Sdk.Server.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;

namespace HotDocs.Sdk.Cloud
{
	/// <summary>
	/// The RESTful implementation of the Core Services client.
	/// </summary>
	public sealed partial class RestClient : ClientBase, IDisposable
	{
		#region Private fields
		private readonly string _defaultOutputDir = Path.GetTempPath();
		private MultipartMimeParser _parser = new MultipartMimeParser();
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a Client object.
		/// </summary>
		public RestClient(
			string subscriberId,
			string signingKey,
			string outputDir = null,
			string hostAddress = null,
			string proxyServerAddress = null)
			: base(subscriberId, signingKey, hostAddress, "", proxyServerAddress)
		{
			OutputDir = outputDir ?? _defaultOutputDir;
			SetTcpKeepAlive();
		}
		#endregion

		#region Public properties

		/// <summary>
		/// The name of the folder where output files will get created.
		/// </summary>
		public string OutputDir { get; set; }

		#endregion

		#region Public methods

		/// <summary>
		/// Uploads a package to HotDocs Cloud Services. Since this method throws an exception if the package already exists in the
		/// HotDocs Cloud Services cache, only call it when necessary.
		/// </summary>
		/// <include file="../../Shared/Help.xml" path="Help/string/param[@name='packageID']"/>
		/// <include file="../../Shared/Help.xml" path="Help/string/param[@name='billingRef']"/>
		/// <param name="packageFile">The file name and path of the package file to be uploaded.</param>
		public void UploadPackage(
			string packageID,
			string billingRef,
			string packageFile)
		{
			if (!string.IsNullOrEmpty(packageFile))
			{
				using (var packageStream = new FileStream(packageFile, FileMode.Open, FileAccess.Read))
				{
					UploadPackage(packageID, billingRef, packageStream);
				}
			}
		}

		/// <summary>
		/// Uploads a package to HotDocs Cloud Services. Since this method throws an exception if the package already exists in the
		/// HotDocs Cloud Services cache, only call it when necessary.
		/// </summary>
		/// <include file="../../Shared/Help.xml" path="Help/string/param[@name='packageID']"/>
		/// <include file="../../Shared/Help.xml" path="Help/string/param[@name='billingRef']"/>
		/// <param name="packageStream">A stream containing the package to upload.</param>
		public void UploadPackage(
			string packageID,
			string billingRef,
			Stream packageStream)
		{
			var timestamp = DateTime.UtcNow;

			string hmac = HMAC.CalculateHMAC(
				SigningKey,
				timestamp,
				SubscriberId,
				packageID,
				null,
				true,
				billingRef);

			StringBuilder urlBuilder = new StringBuilder(string.Format(
				"{0}/RestfulSvc.svc/{1}/{2}", EndpointAddress, SubscriberId, packageID));

			if (!string.IsNullOrEmpty(billingRef))
			{
				urlBuilder.AppendFormat("?billingRef={0}", billingRef);
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlBuilder.ToString());
			request.Method = "PUT";
			request.ContentType = "application/binary";
			request.Headers["x-hd-date"] = timestamp.ToString("r");
			request.Headers[HttpRequestHeader.Authorization] = hmac;

			if (!string.IsNullOrEmpty(ProxyServerAddress))
			{
				request.Proxy = new WebProxy(ProxyServerAddress);
			}

			packageStream.CopyTo(request.GetRequestStream());

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				// Throw away the response, which will be empty.
			}
		}
		#endregion

		#region Protected internal implementations of abstract methods from the base class

		/// <summary>
		/// 
		/// </summary>
		/// <param name="func"></param>
		/// <returns></returns>
		protected internal override object TryWithoutAndWithPackage(Func<bool, object> func)
		{
			try
			{
				return func(false);
			}
			catch (WebException ex)
			{
				using (HttpWebResponse httpResponse = (HttpWebResponse)ex.Response)
				{
					using (Stream data = httpResponse.GetResponseStream())
					{
						string text = new StreamReader(data).ReadToEnd();
						if (text.Contains("HotDocs.Cloud.Storage.PackageNotFoundException"))
						{
							return func(true);
						}
						throw;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="options"></param>
		/// <param name="billingRef"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal override AssemblyResult AssembleDocumentImpl(
			Template template,
			string answers,
			AssembleDocumentSettings options,
			string billingRef,
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
				template.FileName,
				false,
				billingRef,
				options.Format,
				options.Settings);

			StringBuilder urlBuilder = new StringBuilder(string.Format(
				"{0}/RestfulSvc.svc/assemble/{1}/{2}/{3}?format={4}&billingref={5}",
				EndpointAddress, SubscriberId, packageTemplateLocation.PackageID, template.FileName ?? "",
				options.Format.ToString(), billingRef));

			if (options.Settings != null)
			{
				foreach (KeyValuePair<string, string> kv in options.Settings)
				{
					urlBuilder.AppendFormat("&{0}={1}", kv.Key, kv.Value ?? "");
				}
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlBuilder.ToString());
			request.Method = "POST";
			request.ContentType = "text/xml";
			request.Headers["x-hd-date"] = timestamp.ToString("r");
			request.Headers[HttpRequestHeader.Authorization] = hmac;
			request.Timeout = 10 * 60 * 1000; // Ten minute timeout
			request.ContentLength = answers != null ? answers.Length : 0L;

			if (!string.IsNullOrEmpty(ProxyServerAddress))
			{
				request.Proxy = new WebProxy(ProxyServerAddress);
			}

			if (answers != null)
			{
				byte[] data = Encoding.UTF8.GetBytes(answers);
				request.GetRequestStream().Write(data, 0, data.Length);
			}
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			Directory.CreateDirectory(OutputDir);
			using (var resultsStream = new MemoryStream())
			{
				// Each part is written to a file named after the Content-ID value,
				// except for the AssemblyResult part, which has a Content-ID of "XML0",
				// and is parsed into an AssemblyResult object.
				_parser.WritePartsToStreams(
					response.GetResponseStream(),
					h =>
					{
						string id;
						if (h.TryGetValue("Content-ID", out id))
						{
							// Remove angle brackets if present
							if (id.StartsWith("<") && id.EndsWith(">"))
							{
								id = id.Substring(1, id.Length - 2);
							}

							if (id.Equals("XML0", StringComparison.OrdinalIgnoreCase))
							{
								return resultsStream;
							}

							return new FileStream(Path.Combine(OutputDir, id), FileMode.Create);
						}
						return Stream.Null;
					},
					(new ContentType(response.ContentType)).Boundary);

				if (resultsStream.Position > 0)
				{
					resultsStream.Position = 0;
					var serializer = new XmlSerializer(typeof(AssemblyResult));
					return (AssemblyResult)serializer.Deserialize(resultsStream);
				}
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="answers"></param>
		/// <param name="options"></param>
		/// <param name="billingRef"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal override BinaryObject[] GetInterviewImpl(
			Template template,
			string answers,
			InterviewSettings options,
			string billingRef,
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
				template.FileName,
				false,
				billingRef,
				options.Format,
				options.InterviewImageUrl,
				options.Settings);

			StringBuilder urlBuilder = new StringBuilder(string.Format(
				"{0}/RestfulSvc.svc/interview/{1}/{2}/{3}?format={4}&markedvariables={5}&tempimageurl={6}&billingref={7}",
				EndpointAddress, SubscriberId, packageTemplateLocation.PackageID, template.FileName, options.Format.ToString(),
				options.MarkedVariables != null ? string.Join(",", options.MarkedVariables) : null, options.InterviewImageUrl, billingRef));

			if (options.Settings != null)
			{
				foreach (KeyValuePair<string, string> kv in options.Settings)
				{
					urlBuilder.AppendFormat("&{0}={1}", kv.Key, kv.Value ?? "");
				}
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlBuilder.ToString());
			request.Method = "POST";
			request.ContentType = "text/xml";
			request.Headers["x-hd-date"] = timestamp.ToString("r");
			request.Headers[HttpRequestHeader.Authorization] = hmac;
			request.ContentLength = answers != null ? answers.Length : 0L;

			if (!string.IsNullOrEmpty(ProxyServerAddress))
			{
				request.Proxy = new WebProxy(ProxyServerAddress);
			}

			using (Stream stream = request.GetRequestStream())
			{
				if (answers != null)
				{
					byte[] data = Encoding.UTF8.GetBytes(answers);
					stream.Write(data, 0, data.Length);
				}
			}

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				Directory.CreateDirectory(OutputDir);
				using (var resultsStream = new MemoryStream())
				{
					// Each part is written to a file named after the Content-ID value,
					// except for the BinaryObject[] part, which has a Content-ID of "XML0",
					// and is parsed into an BinaryObject[] object.
					_parser.WritePartsToStreams(
						response.GetResponseStream(),
						h =>
						{
							string id;
							if (h.TryGetValue("Content-ID", out id))
							{
								// Remove angle brackets if present
								if (id.StartsWith("<") && id.EndsWith(">"))
								{
									id = id.Substring(1, id.Length - 2);
								}

								if (id.Equals("XML0", StringComparison.OrdinalIgnoreCase))
								{
									return resultsStream;
								}

								// The following stream will be closed by the parser
								return new FileStream(Path.Combine(OutputDir, id), FileMode.Create);
							}
							return Stream.Null;
						},
						(new ContentType(response.ContentType)).Boundary);

					if (resultsStream.Position > 0)
					{
						resultsStream.Position = 0;
						var serializer = new XmlSerializer(typeof(BinaryObject[]));
						return (BinaryObject[])serializer.Deserialize(resultsStream);
					}
					return null;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="template"></param>
		/// <param name="includeDialogs"></param>
		/// <param name="billingRef"></param>
		/// <param name="uploadPackage"></param>
		/// <returns></returns>
		protected internal override ComponentInfo GetComponentInfoImpl(
			Template template,
			bool includeDialogs,
			string billingRef,
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
				template.FileName,
				false,
				billingRef,
				includeDialogs);

			StringBuilder urlBuilder = new StringBuilder(string.Format(
				"{0}/RestfulSvc.svc/componentinfo/{1}/{2}/?includedialogs={3}&billingref={4}",
				EndpointAddress, SubscriberId, packageTemplateLocation.PackageID, includeDialogs.ToString(), billingRef));

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlBuilder.ToString());
			request.Method = "GET";
			request.Headers["x-hd-date"] = timestamp.ToString("r");
			request.Headers[HttpRequestHeader.Authorization] = hmac;

			if (!string.IsNullOrEmpty(ProxyServerAddress))
			{
				request.Proxy = new WebProxy(ProxyServerAddress);
			}

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			var serializer = new XmlSerializer(typeof(ComponentInfo));
			MemoryStream stream = new MemoryStream();
			return (ComponentInfo)serializer.Deserialize(response.GetResponseStream());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="answers"></param>
		/// <param name="billingRef"></param>
		/// <returns></returns>
		protected internal override BinaryObject GetAnswersImpl(BinaryObject[] answers, string billingRef)
		{
			throw new NotImplementedException(); // The REST client does not support GetAnswers.
		}
		#endregion

		#region Dispose methods

		/// <summary>
		/// 
		/// </summary>
		public void Dispose() // Since this class is sealed, we don't need to implement the full dispose pattern.
		{
			if (_parser != null)
			{
				_parser.Dispose();
				_parser = null;
			}
		}
		#endregion
	}
}
