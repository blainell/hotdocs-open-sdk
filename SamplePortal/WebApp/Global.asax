<%@ Application Language="C#" %>

<script runat="server">

	void Application_Start(object sender, EventArgs e) 
	{
		// Code that runs on application startup
		HotDocs.Sdk.TemplateLocation.RegisterLocation(typeof(HotDocs.Sdk.PackagePathTemplateLocation));
	}
	
	void Application_End(object sender, EventArgs e) 
	{
		//  Code that runs on application shutdown

	}
		
	void Application_Error(object sender, EventArgs e) 
	{
		// Code that runs when an unhandled error occurs

	}

	void Session_Start(object sender, EventArgs e) 
	{
		// Code that runs when a new session is started

		//Associate the HotDocs session with the ASPX session.
		SamplePortal.Factory.GetWorkSession(this.Session);
	}

	void Session_End(object sender, EventArgs e) 
	{
		// Code that runs when a session ends. 
		// Note: The Session_End event is raised only when the sessionstate mode
		// is set to InProc in the Web.config file. If session mode is set to StateServer 
		// or SQLServer, the event is not raised.

		SamplePortal.Factory.RetireAssembledDocsCache(this.Session);
		SamplePortal.Factory.RetireWorkSession(this.Session);
	}

</script>
