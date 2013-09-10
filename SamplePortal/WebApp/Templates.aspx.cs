/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using SamplePortal;
using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

public partial class Templates : System.Web.UI.Page
{
	protected DataView _tplData;
	protected string _siteName = Settings.SiteName;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			ViewState["sortExpression"] = Settings.DefaultTemplateTableSortExpression;
			BindData(null);
		}
		// TODO: Would a StringBuilder be more readable here?
		string uploadUrl = Request.ServerVariables["HTTPS"].ToUpper() == "ON" ? "https://" : "http://";
		uploadUrl += Request.ServerVariables["SERVER_NAME"];
		if (Request.ServerVariables["SERVER_PORT"] != "80")
			uploadUrl += ":" + Request.ServerVariables["SERVER_PORT"];
		string pathInfo = Request.ServerVariables["PATH_INFO"];
		uploadUrl += pathInfo.Substring(0, pathInfo.LastIndexOf("/") + 1) + "upload.aspx";
		lblUploadURL.Text = uploadUrl;
	}

	protected void btnHome_Click(object sender, EventArgs e)
	{
		Response.Redirect("default.aspx");
	}

	public void BindData(string newSortExpression)
	{
		string sortExpression = Util.ToggleSortOrder(dataGrid, newSortExpression, ViewState["sortExpression"].ToString());

		_tplData = (new SamplePortal.Data.Templates()).SelectAll(sortExpression, txtSearch.Text);
		dataGrid.DataSource = _tplData;

		Util.SetNewPageSize(dataGrid, Settings.TemplateGridPageSize, _tplData.Count);
		dataGrid.DataBind();
		ViewState["sortExpression"] = sortExpression;
	}

	#region Search
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		dataGrid.CurrentPageIndex = 0;
		BindData(null);
	}
	protected void btnSearchClear_Click(object sender, EventArgs e)
	{
		txtSearch.Text = "";
		BindData(null);
	}
	#endregion

	#region Data Grid
	protected void dataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
	{
		// TODO: Would a switch statement make this code more readable?
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			LinkButton lnkDelete = (LinkButton)e.Item.Cells[1].Controls[0];
			lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this record?');");
		}

		if (e.Item.ItemType == ListItemType.EditItem)
		{
			// Enforce limits on the length of text in the title and description fields.
			((TextBox)e.Item.Cells[3].Controls[0]).Attributes.Add("maxlength", Settings.MaxTitleLength.ToString());
			((TextBox)e.Item.Cells[4].Controls[0]).Attributes.Add("maxlength", Settings.MaxDescriptionLength.ToString());
		}

		if (e.Item.ItemType == ListItemType.Pager)
			Util.CustomizePager(e);
	}
	protected void dataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
	{
		dataGrid.EditItemIndex = -1;
		BindData(null);
	}
	protected void dataGrid_EditCommand(object source, DataGridCommandEventArgs e)
	{
		ViewState["editf"] = dataGrid.Items[e.Item.ItemIndex].Cells[2].Text;
		dataGrid.EditItemIndex = e.Item.ItemIndex;
		BindData(null);
	}
	protected void dataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
	{
		using (SamplePortal.Data.Templates templates = new SamplePortal.Data.Templates())
		{
			// TODO: Use local variables as parameters to UpdateTemplate.
			templates.UpdateTemplate(ViewState["editf"].ToString(),
				((TextBox)e.Item.Cells[3].Controls[0]).Text,
				((TextBox)e.Item.Cells[4].Controls[0]).Text, null);
		}
		dataGrid.EditItemIndex = -1;
		BindData(null);
	}
	protected void dataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
	{
		dataGrid.CurrentPageIndex = e.NewPageIndex;
		BindData(null);
	}
	protected void dataGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
	{
		BindData(e.SortExpression);
	}
	protected void dataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
	{
		string tplf = e.Item.Cells[2].Text;
		(new SamplePortal.Data.Templates()).DeleteTemplate(tplf);
		BindData(null);
	}
	#endregion

	protected void LinkButton1_Click(object sender, EventArgs e)
	{
		// This will create an ".uploadconfig" file consisting of a site name and url, which HotDocs Developer will use to
		// automatically set up a command in the "Upload" menu for this site.
		using (MemoryStream mStream = new MemoryStream())
		{
			// The "uploadconfig" file is a simple text file with the site name on the first line and the upload URL on the second line.
			TextWriter tw = new StreamWriter(mStream);
			tw.WriteLine(_siteName);
			tw.WriteLine(lblUploadURL.Text);
			tw.Flush();

			Response.Clear();
			Response.AddHeader("Content-Disposition", "attachment; filename=config.hduploadconfig");
			Response.AddHeader("Content-Length", mStream.Length.ToString()); // fInfo.Length.ToString());
			Response.ContentType = Util.GetMimeType("config.hduploadconfig");
			Response.BinaryWrite(mStream.ToArray()); // TODO: Use mStream.CopyTo(Response.OutputStream) instead?
			Response.End();
		}
	}
}
