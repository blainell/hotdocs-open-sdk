<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Templates.aspx.cs" Inherits="Templates" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= _siteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<iframe id="KeepAlive" src="./keep-alive.aspx" width="0" height="0" runat="server"></iframe>
			<table id="pageHeader" border="0">
				<tr>
					<td id="pageHeaderLeft">
						<div class="hd-sp-img hd-sp-img-header">
							<div class="hd-sp-title"><%= _siteName %></div>
						</div>
					</td>
					<td id="pageHeaderMiddle">
						<asp:LinkButton ID="btnHome" runat="server" ToolTip="Return to the home page" OnClick="btnHome_Click" CssClass="HeaderLink">&gt;&nbsp;Home</asp:LinkButton></td>
					<td id="pageHeaderRight">
						<a href="http://www.hotdocs.com/products/server/" target="_blank" title="Powered by HotDocs Server">
							<div class="hd-sp-img hd-sp-img-hds"></div>
						</a>
					</td>
				</tr>
			</table>
			<table id="pageContent" border="0">
				<tr>
					<td>
						<!-- TODO: Use CSS classes in this table instead of hard-coding styles. -->
						<table id="DataGridSearchTable" border="0">
							<tr>
								<td style="vertical-align:bottom;">Manage Templates:</td>
								<td>
									<div class="hd-sp-searchbox">
										<div>Search:&nbsp;</div>
										<div>
											<asp:TextBox ID="txtSearch" runat="server" CssClass="InputField"></asp:TextBox>
										</div>
										<div>
											<asp:LinkButton ID="btnSearch" runat="server" ToolTip="Search" OnClick="btnSearch_Click">
												<div class="hd-sp-img hd-sp-img-search" ></div>
											</asp:LinkButton>
										</div>
										<div>
											<asp:LinkButton ID="btnSearchClear" runat="server" ToolTip="Clear the search" OnClick="btnSearchClear_Click">
												<div class="hd-sp-img hd-sp-img-clear" ></div>
											</asp:LinkButton>
										</div>
									</div>
								</td>
							</tr>
						</table>
						<asp:DataGrid ID="dataGrid" runat="server" BorderColor="#99B2CC" AutoGenerateColumns="False" DataSource="<%# _tplData %>" AllowPaging="True" AllowSorting="True" CellPadding="3" CssClass="DataGrid" OnCancelCommand="dataGrid_CancelCommand" OnDeleteCommand="dataGrid_DeleteCommand" OnEditCommand="dataGrid_EditCommand" OnItemCreated="dataGrid_ItemCreated" OnPageIndexChanged="dataGrid_PageIndexChanged" OnSortCommand="dataGrid_SortCommand" OnUpdateCommand="dataGrid_UpdateCommand">
							<EditItemStyle CssClass="EditItemStyle"></EditItemStyle>
							<AlternatingItemStyle CssClass="DataGridAlternateItem"></AlternatingItemStyle>
							<ItemStyle CssClass="DataGridItem"></ItemStyle>
							<HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
							<Columns>
								<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="Update" CancelText="Cancel" EditText="Edit">
									<HeaderStyle HorizontalAlign="Center" Width="40px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:EditCommandColumn>
								<asp:ButtonColumn Text="Delete" CommandName="Delete">
									<HeaderStyle HorizontalAlign="Center" Width="50px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
								</asp:ButtonColumn>
								<asp:BoundColumn DataField="Filename" SortExpression="Filename" ReadOnly="True" HeaderText="File Name"></asp:BoundColumn>
								<asp:BoundColumn DataField="Title" SortExpression="Title" HeaderText="Title" ItemStyle-CssClass="hd-sp-gridItem"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" SortExpression="Description" HeaderText="Description" ItemStyle-CssClass="hd-sp-gridItem"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="DateAdded" SortExpression="DateAdded" HeaderText="DateAdded"></asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="DataGridPager" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid>
						<p>
							To upload additional templates from HotDocs Developer, right-click the templates
						in your library and use the <b>Upload</b> command in the shortcut menu. The URL to use
						for uploading templates to this site is <b>
							<asp:Label ID="lblUploadURL" runat="server"></asp:Label></b>.
						</p>
						<p>
							<asp:LinkButton ID="DownloadConfigFileLink" runat="server"
								OnClick="LinkButton1_Click">Click here to download a file</asp:LinkButton>
							that will automatically configure HotDocs Developer 10.2 (or greater) to include
						a command in the <b>Upload</b> menu for uploading templates to the URL listed above.
						Once the file is downloaded, click Open to configure the upload site.
						</p>
					</td>
				</tr>
			</table>
			<input type="text" style="DISPLAY: none; VISIBILITY: hidden" /><!-- workaround for weird IE behavior when auto-submitting via Enter key -->
		</div>
	</form>
</body>
</html>
