<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SaveAnswers.aspx.cs" Inherits="SaveAnswers" %>

<%@ Register Src="Banner.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= Header1.SiteName %></title>
	<meta name="viewport" content="initial-scale=1.0" />
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<uc1:Header ID="Header1" runat="server" Mode="Hidden" />
			<table id="pageContent" border="0">
				<tr>
					<td>
						<!-- TODO: Use CSS classes in this table instead of hard-coding styles. -->
						<table border="0" style="border-collapse: separate; border-spacing: 4px;">
							<tr>
								<td style="WIDTH: 160px">Title:</td>
								<td>
									<asp:TextBox ID="txtTitle" runat="server" Width="368px" CssClass="InputField"></asp:TextBox></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px; vertical-align: top;">Description:</td>
								<td>
									<asp:TextBox ID="txtDescription" runat="server" Width="368px" TextMode="MultiLine"
										Height="64px" CssClass="InputField"></asp:TextBox></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px">
									<br />
								</td>
								<td>
									<asp:Button ID="btnSave" runat="server" ToolTip="Save the answers from the interview." Text="Save Answers"
										CssClass="InputField" OnClick="btnSave_Click"></asp:Button>&nbsp;
									<asp:Label ID="lblStatus" runat="server" Visible="False">Status</asp:Label></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
