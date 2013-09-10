/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

//TODO: Put IHttpHandler overrides in a #region block?

using HotDocs.Sdk.Server;
using System;
using System.IO;
using System.Web;

namespace SamplePortal
{
	public class HDServerFilesHandler : IHttpHandler
	{
		/// <summary>
		/// Handles an http request for any of the interview runtime files (e.g., any files in the "HDServerFiles" folder).
		/// </summary>
		/// <param name="context">The context of the request being made.</param>
		public void ProcessRequest(HttpContext context)
		{
			// This is the folder where we want the SDK to keep the locally cached interview runtime files.
			string cacheFolder = Path.Combine(Settings.CachePath, "HDServerFiles");
			if (!Directory.Exists(cacheFolder))
				Directory.CreateDirectory(cacheFolder);

			// Extract the name of the file being requested, including any subfolders, from the request Path. The only part we care about is what
			// appears after the last 'HDServerFiles' in the path. Since the path in web.config for the stylesheet URL is relative by default, 
			// JS and SL interviews may request files differently. For example, in a JS interview, the path for the user stylesheet will be relative 
			// to the root application URL like this: http://machinename/WebApp/HDServerFiles/stylesheets/theme.css. But for Silverlight interviews, 
			// it will be relative to the JavaScript folder where interview.xap lives, and the URL will look like this: 
			// http://machinename/WebApp/HDServerFiles/js/HDServerFiles/stylesheets/theme.css
			// To reconcile these differences when parsing the requested path, we look for the last instance of HDServerFiles rather than the first.

			string contentType;
			string requestPath = context.Request.Path;
			string fileToRequest = requestPath.Substring(requestPath.LastIndexOf("HDServerFiles") + "HDServerFiles".Length).TrimStart('/');

			// Try to get the file from the SDK cache.
			using (Stream s = HotDocs.Sdk.Server.Util.GetInterviewRuntimeFile(fileToRequest, cacheFolder, Settings.HDServerFilesUrl, out contentType))
			{
				if (s == null)
				{
					context.Response.StatusCode = 404; // HTTP_STATUS_NOT_FOUND
				}
				else
				{
					context.Response.ContentType = contentType;
					context.Response.Cache.SetCacheability(HttpCacheability.Public);
					context.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));
					s.CopyTo(context.Response.OutputStream);
				}
				context.Response.End();
			}
		}

		/// <summary>
		/// Indicates whether another request can reuse this IHttpHandler instance.
		/// </summary>
		public bool IsReusable
		{
			get
			{
				// This file handler has no state, so the handler can be reused as needed.
				return true;
			}
		}
	}
}