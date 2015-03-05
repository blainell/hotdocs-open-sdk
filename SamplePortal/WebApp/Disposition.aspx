<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Disposition.aspx.cs" Inherits="Disposition" %>

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
			<uc1:Header ID="Header1" runat="server" HomeLink="true" />
			<table id="pageContent" border="0">
				<tr>
					<td>
						<asp:Panel ID="pnlError" CssClass="ErrorMessage" runat="server" Visible="False">
							<p>
								<strong>Error:</strong>
								<asp:Label ID="lblError" runat="server">Label</asp:Label>
							</p>
						</asp:Panel>
						<p>
							<asp:Panel ID="pnlNextAsm" runat="server" Visible="False" CssClass="WarnPanel">
								<strong>Note:</strong>
								<asp:Label ID="lblContinue" runat="server"></asp:Label>
								<br />
								<asp:Button ID="btnNextInterview" runat="server" CssClass="SpacedItem" Text="Next Interview" OnClick="btnNextInterview_Click"></asp:Button>
							</asp:Panel>
						</p>
						<p>You may save your answers on the server for later reuse:</p>
						<table border="0" style="border-collapse: separate; border-spacing: 4px;">
							<tr>
								<td style="WIDTH: 160px">
									<asp:Label ID="lblTitle" runat="server" Width="152px">Answer Set Title:</asp:Label></td>
								<td>
									<asp:TextBox ID="txtTitle" runat="server" CssClass="InputField" Width="368px"></asp:TextBox></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px; vertical-align: top;">
									<asp:Label ID="lblDesc" runat="server">Answer Set Description:</asp:Label></td>
								<td>
									<asp:TextBox ID="txtDescription" runat="server" CssClass="InputField" Height="64px" Width="368px"
										TextMode="MultiLine"></asp:TextBox></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px">
									<br />
								</td>
								<td>
									<asp:Button ID="btnSave" runat="server" ToolTip="Save the answers from the interview." CssClass="InputField"
										Text="Save Answers" OnClick="btnSave_Click"></asp:Button>&nbsp;<asp:Label ID="lblStatus" runat="server" Visible="False">Status</asp:Label></td>
							</tr>
							<tr>
								<td style="WIDTH: 160px">
									<br />
								</td>
								<td></td>
							</tr>
						</table>
						<p>
							<asp:Panel ID="panelDocDownload" runat="server">
								<br />
								<strong>You can also download 
									the&nbsp;assembled documents listed&nbsp;below.</strong><br />
								&nbsp; 
								<asp:DataGrid ID="docGrid" runat="server" CssClass="DataGrid" ShowFooter="True" BorderStyle="Solid"
									AutoGenerateColumns="False" CellPadding="3" BorderWidth="1px"
									BorderColor="#DFDFC0" OnItemCommand="docGrid_ItemCommand" OnItemDataBound="docGrid_ItemDataBound">
									<AlternatingItemStyle CssClass="DataGridAlternateItem"></AlternatingItemStyle>
									<ItemStyle CssClass="DataGridItem"></ItemStyle>
									<HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
									<Columns>
										<asp:BoundColumn Visible="False" DataField="DocumentFilePath" ReadOnly="True" HeaderText="DocumentFile"></asp:BoundColumn>
										<asp:TemplateColumn>
											<HeaderStyle Width="80px"></HeaderStyle>
											<ItemStyle HorizontalAlign="Center"></ItemStyle>
											<ItemTemplate>
												<asp:LinkButton ID="btnDownload" Text="Download" runat="server" CommandName="Download"></asp:LinkButton>
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:TemplateColumn>
											<HeaderStyle Width="30px"></HeaderStyle>
											<ItemStyle VerticalAlign="Middle"></ItemStyle>
											<ItemTemplate>
												<div style="text-align: center;">
													<asp:LinkButton ID="btnImgTemplate" runat="server" Width="16" Height="16" CommandName="Download"></asp:LinkButton>
												</div>
											</ItemTemplate>
										</asp:TemplateColumn>
										<asp:BoundColumn DataField="TemplateTitle" SortExpression="TemplateTitle" ReadOnly="True" HeaderText="Title">
											<ItemStyle VerticalAlign="Middle"></ItemStyle>
											<FooterStyle VerticalAlign="Middle"></FooterStyle>
										</asp:BoundColumn>
									</Columns>
									<PagerStyle CssClass="DataGridPager" Mode="NumericPages"></PagerStyle>
								</asp:DataGrid>
							</asp:Panel>
						</p>
						<p>&nbsp;</p>
					</td>
				</tr>
			</table>
		</div>
	</form>
</body>
</html>
