/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotDocs.Sdk.Server
{
	/// <summary>
	/// Return result of GetInterview.
	/// </summary>
	public class InterviewResult
	{
		/// <summary>
		/// The requested interview: an HTML fragment that can be inserted into the HTML of your host application's page.
		/// Note that the DOCTYPE of the page where this is inserted must be such that Internet Explorer renders it in Standards Mode!
		/// </summary>
		public string HtmlFragment { get; internal set; }
	}
}
