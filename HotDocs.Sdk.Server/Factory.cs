/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HotDocs.Sdk.Server
{
	public enum HdProtocol
	{
		Local,
		WebService,
		Cloud
	}
	public class Factory
	{
		/// <summary>
		/// Create an instance of the IServices interface using the requested protocol.
		/// </summary>
		/// <param name="protocol"></param>
		/// <param name="tempFolderPath"></param>
		/// <returns></returns>
		public static IServices CreateServices(HdProtocol protocol)
		{
			switch (protocol)
			{
				case HdProtocol.Cloud:
					throw new Exception("Create HotDocs.Sdk.Server.Cloud.Services directly.");
				case HdProtocol.Local:
					throw new Exception("Create HotDocs.Sdk.Server.Local.Services directly.");
				case HdProtocol.WebService:
					throw new Exception("Create HotDocs.Sdk.Server.WebService.Services directly.");
				default:
					throw new NotSupportedException();
			}
		}
		//private static string GetRootedPath(string path)
		//{
		//	if (!Path.IsPathRooted(path))
		//	{
		//		string siteRoot = System.Web.Hosting.HostingEnvironment.MapPath("~");
		//		string siteRootParent = Directory.GetParent(siteRoot).FullName;
		//		path = Path.Combine(siteRootParent, "Files", path);
		//	}
		//	return path;
		//}
	}
}
