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
	/// The response from a HotDocs interview that includes answers entered by the user.
	/// </summary>
	public static class InterviewResponse
	{
		/// <summary>
		/// This method extracts the answers that were posted from a HotDocs interview.
		/// </summary>
		/// <param name="form">The form in which the answers were posted.</param>
		/// <returns>A string of answers.</returns>
		public static string GetAnswers(System.Collections.Specialized.NameValueCollection form)
		{
			// convert from a NameValueCollection (typically from <System.Web.HttpRequest>.Form)
			// to a string.  With HotDocs 2007 SR1 and earlier, this will be plain XML.
			// For HotDocs 2007 SR2 (and later) stateful interviews, this will be Base64-encoded XML.
			// For HotDocs 10.1 (and later) stateless interviews, this will be in the [HDANS] format.
			string[] hdInfoValues = form.GetValues("HDInfo");
			if (hdInfoValues != null && hdInfoValues.Length > 0)
				return string.Join(string.Empty, hdInfoValues);
			else
				return string.Empty;
		}

		/// <summary>
		/// This method extracts the answers that were posted from a HotDocs interview and returns them in a stream.
		/// </summary>
		/// <param name="form">The form in which the answers were posted.</param>
		/// <returns>A stream of answers.</returns>
		public static System.IO.Stream GetAnswerStream(System.Collections.Specialized.NameValueCollection form)
		{
			return new System.IO.MemoryStream(Encoding.UTF8.GetBytes(GetAnswers(form)));
		}
	}
}
