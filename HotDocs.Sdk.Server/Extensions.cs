/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server
{
	static class Extensions
	{
		public static string[] SplitPath(this string path)
		{
			string[] stringsRet;
			var separators = new char[] {
				Path.DirectorySeparatorChar,  
				Path.AltDirectorySeparatorChar  
			};
			stringsRet = path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			if (stringsRet != null)
			{
				int len0 = stringsRet[0].Length;
				if (stringsRet[0][len0 - 1] == ':')
					stringsRet[0] += Path.DirectorySeparatorChar;
			}
			return stringsRet;
		}
	}
}
