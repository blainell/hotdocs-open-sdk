using SamplePortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Header : System.Web.UI.UserControl
{
	private string _homeLinkText = "Return to the home page";

	protected string _siteName = Settings.SiteName;

	public enum HeaderState { Home, Manage, NoLinks, Hidden }

	protected void Page_Load(object sender, EventArgs e)
	{

	}

	protected void btnManageAnswers_Click(object sender, EventArgs e)
	{
		Response.Redirect("Answers.aspx");
	}

	protected void btnManageTemplates_Click(object sender, EventArgs e)
	{
		Response.Redirect("Templates.aspx");
	}

	public HeaderState Mode { get; set; }

	public string HomeLinkText
	{
		get { return _homeLinkText; }
		set { _homeLinkText = value; }
	}

}