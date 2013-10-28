/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

using SamplePortal;
using SamplePortal.Data;
using System;
using System.Data;
using System.IO;

/// <summary>
/// This page demonstrates how to save answers mid-interview using the SavePageUrl property of the Assembly object.
/// When this page is processed, a HotDocs Server interview will be displayed in the browser and the user will
/// have clicked the "Save Answers" button. The JavaScript posts the answers to this page. The HTML returned from
/// this page will be displayed in the interview.
/// 
/// This page needs to gather the answers, figure out where to save the answers, and save them.
/// </summary>
public partial class SaveAnswers : System.Web.UI.Page
{
	private string _ansFilename;
	private string _ansPath;

	protected HotDocs.Sdk.Server.WorkSession _session;

	protected void Page_Load(object sender, EventArgs e)
	{
		_session = SamplePortal.Factory.GetWorkSession(this.Session);
		if (_session == null)
			Response.Redirect("Default.aspx");
		_ansPath = _session.AnswerCollection.FilePath;
		_ansFilename = Path.GetFileName(_ansPath);
		if (!IsPostBack) // first arrival on this page
		{
			Util.SweepTempDirectories(); // first some housekeeping

			// Set the max length on the title and description fields. These were previously set in the ASPX page,
			// but ASP.NET drops them for multi-line fields when rendering the page.
			// So we set them manually here using values configurable in the web.config.
			txtTitle.Attributes.Add("maxlength", Settings.MaxTitleLength.ToString());
			txtDescription.Attributes.Add("maxlength", Settings.MaxDescriptionLength.ToString());

			ViewState["answers"] = HotDocs.Sdk.Server.InterviewResponse.GetAnswers(Request.Form); // get the answers from the browser

			if (_session.AnswerCollection.FilePath.Length > 0)
			{
				using (Answers answers = new Answers())
				{
					DataView ansData = answers.SelectFile(_ansFilename);
					// pre-populate answer set title and description
					txtTitle.Text = ansData[0]["Title"].ToString();
					txtDescription.Text = ansData[0]["Description"].ToString();
				}
			}
		}
	}
	protected void btnSave_Click(object sender, EventArgs e)
	{
		if (txtTitle.Text.Length == 0) // a title is required
		{
			lblStatus.Text = "Error: Please enter an answer set title.";
			lblStatus.CssClass = "ErrorMessage";
		}
		else
		{
			//Overlay the new answers on the pre-existing answers.
			string ansStr = ViewState["answers"].ToString();
			TextReader rdr = new StringReader(ansStr);
			_session.AnswerCollection.OverlayXml(HotDocs.Sdk.Server.InterviewAnswerSet.GetDecodedInterviewAnswers(rdr));

			//Do the save.
			Util.SaveAnswers(_session.AnswerCollection, txtTitle.Text, txtDescription.Text);

			//Update the page content.
			lblStatus.Text = "Answer set saved.";
			lblStatus.CssClass = "SuccessMessage";
		}
		lblStatus.Visible = true;
	}
}

