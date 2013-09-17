/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using SamplePortal;
using SamplePortal.Data;
using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class answers : System.Web.UI.Page
{
	protected DataView _ansData;
	protected static string _javascriptUrl = Settings.JavaScriptUrl;

	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsPostBack == false)
		{
			ViewState["sortExpression"] = Settings.DefaultAnswerTableSortExpression;
			BindData(null);
		}
	}

	public void BindData(string newSortExpression)
	{
		string sortExpression = Util.ToggleSortOrder(ansGrid, newSortExpression, ViewState["sortExpression"].ToString());

		_ansData = (new Answers()).SelectAll(sortExpression, txtSearch.Text);

		ansGrid.DataSource = _ansData;
		Util.SetNewPageSize(ansGrid, Settings.AnswerGridPageSize, _ansData.Count);
		ansGrid.DataBind();
		ViewState["sortExpression"] = sortExpression;
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

	protected void ansGrid_ItemCreated(object sender, DataGridItemEventArgs e)
	{
		switch (e.Item.ItemType)
		{
			case ListItemType.Item:
			case ListItemType.AlternatingItem:
				LinkButton lnkDelete = (LinkButton)e.Item.Cells[1].Controls[0];
				lnkDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete this record?');");
				break;
			case ListItemType.EditItem:
				// Enforce limits on the length of text in the title and description fields.
				((TextBox)e.Item.Cells[3].Controls[0]).Attributes.Add("maxlength", Settings.MaxTitleLength.ToString());
				((TextBox)e.Item.Cells[4].Controls[0]).Attributes.Add("maxlength", Settings.MaxDescriptionLength.ToString());
				break;
			case ListItemType.Pager:
				Util.CustomizePager(e);
				break;
		}
	}

	protected void ansGrid_SortCommand(object source, DataGridSortCommandEventArgs e)
	{
		BindData(e.SortExpression);
	}
	protected void ansGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
	{
		ansGrid.CurrentPageIndex = e.NewPageIndex;
		BindData(null);
	}
	protected void ansGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
	{
		string answerFileName = e.Item.Cells[2].Text;
		using (Answers answers = new Answers())
		{
			answers.DeleteAnswerFile(answerFileName);
		}
		System.IO.File.Delete(System.IO.Path.Combine(Settings.AnswerPath, answerFileName));
		BindData(null);
	}
	protected void ansGrid_EditCommand(object source, DataGridCommandEventArgs e)
	{
		ViewState["editf"] = ansGrid.Items[e.Item.ItemIndex].Cells[2].Text;
		ansGrid.EditItemIndex = e.Item.ItemIndex;
		BindData(null);
	}
	protected void ansGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
	{
		using (Answers answers = new Answers())
		{
			string fileName = ViewState["editf"].ToString();
			string title = ((TextBox)e.Item.Cells[3].Controls[0]).Text;
			string description = ((TextBox)e.Item.Cells[4].Controls[0]).Text;

			answers.UpdateAnswerFile(fileName, title, description);
		}
		ansGrid.EditItemIndex = -1;
		BindData(null);
	}
	protected void ansGrid_CancelCommand(object source, DataGridCommandEventArgs e)
	{
		ansGrid.EditItemIndex = -1;
		BindData(null);
	}
}
