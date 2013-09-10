/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SamplePortal;
using SamplePortal.Data;
using System.IO;

public partial class SelAns : System.Web.UI.Page
{
	protected DataView ansData;
	protected HotDocs.Sdk.Server.WorkSession _session;
	public string _siteName = Settings.SiteName;

	protected void Page_Load(object sender, EventArgs e)
	{
		_session = SamplePortal.Factory.GetWorkSession(this.Session);

		if (_session == null)
			Response.Redirect("Default.aspx");

		//add client-side javascript for managing upload controls
		if (!ClientScript.IsClientScriptBlockRegistered("OnSelChange"))
		{
			System.Text.StringBuilder stb = new System.Text.StringBuilder();
			stb.Append("function OnSelChange(){");
			stb.Append("var fileCtrl = document.getElementById(\"fileUpload\");");
			stb.Append("if (fileCtrl) {");
			stb.Append("var uploadBtn = document.getElementById(\"rbUpload\");");
			stb.Append("if (uploadBtn)");
			stb.Append("fileCtrl.disabled = !uploadBtn.checked;");
			stb.Append("}");
			stb.Append("}");
			Type t = typeof(SelAns);
			ClientScript.RegisterClientScriptBlock(t, "OnSelChange", stb.ToString(), true);
		}

		if (!IsPostBack)
		{
			ViewState["sortExpression"] = Settings.DefaultAnswerTableSortExpression;
			BindData(null);		   
		}
	}

	#region Search buttons
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		ansGrid.CurrentPageIndex = 0;
		BindData(null);
	}
	protected void btnSearchClear_Click(object sender, EventArgs e)
	{
		txtSearch.Text = "";
		BindData(null);
	}
	#endregion

	public void BindData(string newSortExpression)
	{
		string sortExpression = Util.ToggleSortOrder(ansGrid, newSortExpression, ViewState["sortExpression"].ToString());

		ansData = (new SamplePortal.Data.Answers()).SelectAll(sortExpression,txtSearch.Text);
		ansGrid.DataSource = ansData;

		Util.SetNewPageSize(ansGrid, Settings.AnswerGridPageSize, ansData.Count);
		ansGrid.DataBind();
		ViewState["sortExpression"] = sortExpression;
	}
	protected void btnContinue_Click(object sender, EventArgs e)
	{
		if (Request.Form["AnsFileType"] == "upload")
		{
			// upload existing answer file:
			HttpPostedFile file = fileUpload.PostedFile;
			if (file != null)
			{
				string ext = Path.GetExtension(file.FileName).ToLower();
				if (ext == ".anx")
				{
					_session.AnswerCollection.ReadFile(file.FileName);
					Advance();
					return;
				}
			}
		}
		//else use new answer file:
		Advance();
	}
	protected void btnHome_Click(object sender, EventArgs e)
	{
		Response.Redirect("Default.aspx");
	}

	protected void ansGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
	{
		ansGrid.CurrentPageIndex = e.NewPageIndex;
		BindData(null);
	}
	protected void ansGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
	{
		BindData(e.SortExpression);
	}
	protected void ansGrid_ItemCreated(object sender, DataGridItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Pager)
			Util.CustomizePager(e);
	}
	protected void ansGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
	{
		if ((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem))
		{
			LinkButton lnkSelect = (LinkButton)e.Item.Cells[0].Controls[0];
			lnkSelect.Text = e.Item.Cells[2].Text;
		}
	}
	protected void ansGrid_SelectedIndexChanged(object sender, EventArgs e)
	{
		string answerfile = ansGrid.SelectedItem.Cells[1].Text;

		if (answerfile == "")
			answerfile = null;
		if (answerfile != null) // check for invalid answer file name
		{
			int delimpos = answerfile.LastIndexOfAny(new char[] { '/', '\\' });
			// answer filename must either contain no slashes, or else only slash must be "tmp\"
			if (delimpos >= 0 && (delimpos != Settings.TempLen - 1 || !answerfile.StartsWith(Settings.TempRelPath)))
				answerfile = null; // invalid answer file -- ignore it
		}

		if (answerfile != null)
		{
			string filePath = Path.Combine(Settings.AnswerPath, answerfile);
			_session.AnswerCollection.ReadFile(filePath);
		}

		Advance();
	}
	private void Advance()
	{
		HotDocs.Sdk.Server.WorkItem wi = _session.CurrentWorkItem;
		if (wi is HotDocs.Sdk.Server.InterviewWorkItem)
			Response.Redirect("Interview.aspx");
		else
			Response.Redirect("Disposition.aspx");
	}
}
