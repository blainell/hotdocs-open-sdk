/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;

public partial class keep_alive : System.Web.UI.Page
{
	//this page exists to renew the HttpSessionState object to keep the HotDocs Session object from expiring
	protected void Page_Load(object sender, EventArgs e)
	{
		Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 60) - 10));
	}
}
