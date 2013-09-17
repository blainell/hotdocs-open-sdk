/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using SamplePortal;
using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
	protected DataView _tplData;
	protected HotDocs.Sdk.Server.WorkSession _session;
	protected static string _javascriptUrl = Settings.JavaScriptUrl;

	protected void Page_Load(object sender, EventArgs e)
	{
		_session = SamplePortal.Factory.GetWorkSession(this.Session);

		if (!IsPostBack)
		{
			ViewState["sortExpression"] = Settings.DefaultTemplateTableSortExpression;
			BindData(null);
		}
	}

	protected void BindData(string newSortExpression)
	{
		string sortExpression = SamplePortal.Util.ToggleSortOrder(tplGrid, newSortExpression, ViewState["sortExpression"].ToString());

		_tplData = (new SamplePortal.Data.Templates()).SelectAll(sortExpression, txtSearch.Text);
		tplGrid.DataSource = _tplData;

		Util.SetNewPageSize(tplGrid, Settings.TemplateGridPageSize, _tplData.Count);
		tplGrid.DataBind();
		ViewState["sortExpression"] = sortExpression;
	}

	

	#region Search

	protected void btnSearch_Click(object sender, EventArgs e)
	{
		tplGrid.CurrentPageIndex = 0;
		BindData(null);
	}
	protected void btnSearchClear_Click(object sender, EventArgs e)
	{
		txtSearch.Text = "";
		BindData(null);
	}

	#endregion

	#region DataGrid
	protected void tplGrid_SelectedIndexChanged(object sender, EventArgs e)
	{
		string packageId = tplGrid.SelectedItem.Cells[5].Text;
		if (string.IsNullOrEmpty(packageId) || packageId.IndexOfAny(new char[] { '/', '\\', ':' }) >= 0) //Don't allow path control elements in a package ID.
		{
			MessageBox.Show("Invalid template path");
			return;
		}

		if (!PackageCache.PackageExists(packageId))
		{
			MessageBox.Show("The selected template could not be found. Please contact your Sample Portal site administrator for assistance.");
			return;
		}

		//Open a new work session.
		try
		{
			_session = Factory.CreateWorkSession(Session, packageId);

			// Make sure that there is a cache for the assembled documents, and that it is empty.
			AssembledDocsCache cache = Factory.GetAssembledDocsCache(this.Session);
			cache.Clear();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
			return;
		}

		//Redirect to "select answer set" page
		Response.Redirect("SelAns.aspx");
	}

	protected void tplGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			LinkButton lnkSelect = (LinkButton)e.Item.Cells[0].Controls[0];
			lnkSelect.Text = e.Item.Cells[2].Text;
		}
	}

	protected void tplGrid_ItemCreated(object sender, DataGridItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Pager)
			Util.CustomizePager(e);
	}

	protected void tplGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
	{
		BindData(e.SortExpression);
	}

	protected void tplGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
	{
		tplGrid.CurrentPageIndex = e.NewPageIndex;
		BindData(null);
	}

	#endregion
}
