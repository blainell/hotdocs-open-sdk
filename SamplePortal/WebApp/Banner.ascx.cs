using SamplePortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Header : System.Web.UI.UserControl
{
	public enum HeaderState { Home, Manage, NoLinks, Hidden }

	public HeaderState Mode { get; set; }

	public string SiteName { get; set; }

	public string HomeLinkText { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
		// Set some default property values if they were not set already.
		if (string.IsNullOrEmpty(SiteName)) 
			SiteName = Settings.SiteName;
		if (string.IsNullOrEmpty(HomeLinkText)) 
			HomeLinkText = "Return to the home page";
	}

	protected void btnManageAnswers_Click(object sender, EventArgs e)
	{
		Response.Redirect("Answers.aspx");
	}

	protected void btnManageTemplates_Click(object sender, EventArgs e)
	{
		Response.Redirect("Templates.aspx");
	}


}