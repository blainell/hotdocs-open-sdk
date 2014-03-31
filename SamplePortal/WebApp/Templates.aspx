<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Templates.aspx.cs" Inherits="Templates" %>

<%@ Register Src="Banner.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= Header1.SiteName %></title>
	<meta name="viewport" content="initial-scale=1.0" />
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
	<!-- Load jQuery 1.x for IE8, and jQuery 2.x for all other browsers. -->
	<!--[if lt IE 9]>
    <script type="text/javascript" src="<%= _javascriptUrl %>/jquery.js" id="jquery"></script>
	<![endif]-->
	<!--[if gte IE 9]><!-->
	<script type="text/javascript" src="<%= _javascriptUrl %>/jquery2.js" id="jquery"></script>
	<!--<![endif]-->
	<script type="text/javascript" src="scripts/sampleportal.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<uc1:Header ID="Header1" runat="server" Mode="Home" />
			<table id="pageContent">
				<tr>
					<td>
						<table id="DataGridSearchTable">
							<tr>
								<td>Manage Templates:</td>
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
						<asp:DataGrid ID="dataGrid" runat="server" AutoGenerateColumns="False" DataSource="<%# _tplData %>" AllowPaging="True" AllowSorting="True" CellPadding="3" CssClass="DataGrid" OnCancelCommand="dataGrid_CancelCommand" OnDeleteCommand="dataGrid_DeleteCommand" OnEditCommand="dataGrid_EditCommand" OnItemCreated="dataGrid_ItemCreated" OnPageIndexChanged="dataGrid_PageIndexChanged" OnSortCommand="dataGrid_SortCommand" OnUpdateCommand="dataGrid_UpdateCommand">
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
