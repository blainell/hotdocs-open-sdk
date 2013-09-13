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
	/// <summary>
	/// This <c>Extensions</c> static class provides extension methods for classes used in the HotDocs.Server.Sdk project.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// <c>SplitPath</c> splits up a full path into collection of strings that consist of folders plus a file name.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static IEnumerable<string> SplitPath(this string path)
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


		/// <summary>
		/// <c>GetCount<T>"</c> returns the number of items in a collection, or array
		/// </summary>
		/// <typeparam name="T">The element type of the collection, or array</typeparam>
		/// <param name="source">The collection or array</param>
		/// <returns></returns>
		public static int GetCount<T>(this IEnumerable<T> source)
		{
			ICollection<T> c = source as ICollection<T>;
			if (c != null)
				return c.Count;

			int result = 0;
			using (IEnumerator<T> enumerator = source.GetEnumerator())
			{
				while (enumerator.MoveNext())
					result++;
			}
			return result;
		}
	}
}
