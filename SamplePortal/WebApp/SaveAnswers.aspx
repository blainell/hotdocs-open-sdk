<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SaveAnswers.aspx.cs" Inherits="SaveAnswers" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><%= _siteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
		  <iframe id="KeepAlive" src="./keep-alive.aspx" width="0" height="0" runat="server"></iframe>
            <table id="pageContent" border="0">
				<tr>
					<td>
						<!-- TODO: Use CSS classes in this table instead of hard-coding styles. -->
						<table border="0" style="border-collapse:separate; border-spacing:4px;">
							<tr>
								<td style="WIDTH: 160px">Title:</td>
								<td><asp:textbox id="txtTitle" runat="server" Width="368px" CssClass="InputField"></asp:textbox></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px; vertical-align:top;">Description:</td>
								<td><asp:textbox id="txtDescription" runat="server" Width="368px" TextMode="MultiLine"
										Height="64px" CssClass="InputField"></asp:textbox></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px"><br/>
								</td>
								<td><asp:button id="btnSave" runat="server" ToolTip="Save the answers from the interview." Text="Save Answers"
										CssClass="InputField" OnClick="btnSave_Click"></asp:button>&nbsp;
									<asp:label id="lblStatus" runat="server" Font-Italic="True" Visible="False">Status</asp:label></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
        </div>
    </form>
</body>
</html>
