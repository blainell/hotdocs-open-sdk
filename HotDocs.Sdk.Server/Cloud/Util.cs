/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using System.IO.Packaging;
using System.Xml;
using System.Xml.XPath;

namespace HotDocs.Sdk.Server.Cloud
{
	//public static class Util
	//{
	//	public static string GetMainTemplateFileName(string packagePath)
	//	{
	//		using (Package package = Package.Open(packagePath, FileMode.Open, FileAccess.Read))
	//		{
	//			Uri manifestUri = PackUriHelper.CreatePartUri(new Uri("manifest.xml", UriKind.Relative));
	//			PackagePart manifestPart = package.GetPart(manifestUri);

	//			XPathDocument doc;
	//			using (Stream stream = manifestPart.GetStream(FileMode.Open, FileAccess.Read))
	//			{
	//				doc = new XPathDocument(stream);
	//			}
	//			XPathNavigator nav = doc.CreateNavigator();
	//			XmlNamespaceManager nsMgr = new XmlNamespaceManager(nav.NameTable);
	//			nsMgr.AddNamespace("p", "http://www.hotdocs.com/schemas/package_manifest/2012"); // TODO: If we support old packages, we need to also look in namespace foo/foo/foo/2011
	//			nav = nav.SelectSingleNode("/p:packageManifest/p:mainTemplate/@fileName", nsMgr);

	//			return nav.Value;
	//		}
	//	}
	//}
}
