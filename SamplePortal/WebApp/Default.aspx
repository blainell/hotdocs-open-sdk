<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= _siteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
	<script type="text/javascript" src="<%= _javascriptUrl %>/Silverlight.js"></script>
	<script type="text/javascript">
		document.cookie = 'SilverlightAvailable=' + Silverlight.isInstalled('5.0');
	</script>
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
						<asp:LinkButton ID="btnManageAnswers" runat="server"
							ToolTip="Edit/Delete answer sets" OnClick="btnManageAnswers_Click"
							CssClass="HeaderLink">Manage&nbsp;Answers</asp:LinkButton><br />
						<asp:LinkButton ID="btnManageTemplates" runat="server"
							ToolTip="Edit/Delete templates" OnClick="btnManageTemplates_Click"
							CssClass="HeaderLink">Manage&nbsp;Templates</asp:LinkButton></td>
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
						<table id="DataGridSearchTable" border="0">
							<tr>
								<td style="vertical-align:bottom;">Select a Template:</td>
								<td>
									<div class="hd-sp-searchbox">
										<div>Search:&nbsp;</div>
										<div>
											<asp:TextBox ID="txtSearch" runat="server" CssClass="InputField"></asp:TextBox>
										</div>
										<div>
											<asp:LinkButton ID="btnSearch" runat="server" ToolTip="Search" OnClick="btnSearch_Click"><div class="hd-sp-img hd-sp-img-search" ></div></asp:LinkButton>
										</div>
										<div>
											<asp:LinkButton ID="btnSearchClear" runat="server" ToolTip="Clear the search" OnClick="btnSearchClear_Click"><div class="hd-sp-img hd-sp-img-clear" ></div></asp:LinkButton>
										</div>
									</div>
								</td>
							</tr>
						</table>
						<asp:DataGrid ID="tplGrid" runat="server" CssClass="DataGrid" DataSource="<%# _tplData %>" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CellPadding="3" BorderColor="#99B2CC" OnItemCreated="tplGrid_ItemCreated" OnItemDataBound="tplGrid_ItemDataBound" OnPageIndexChanged="tplGrid_PageIndexChanged" OnSelectedIndexChanged="tplGrid_SelectedIndexChanged" OnSortCommand="tplGrid_SortCommand">
							<AlternatingItemStyle CssClass="DataGridAlternateItem"></AlternatingItemStyle>
							<ItemStyle CssClass="DataGridItem"></ItemStyle>
							<HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
							<Columns>
								<asp:ButtonColumn Text="Select" SortExpression="Title" HeaderText="Title" CommandName="Select">
									<HeaderStyle HorizontalAlign="Left" Width="40%" CssClass="DataGridHeader"></HeaderStyle>
								</asp:ButtonColumn>
								<asp:BoundColumn Visible="False" DataField="Filename" SortExpression="Filename" HeaderText="File Name"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="Title" SortExpression="Title" HeaderText="Title">
									<HeaderStyle Width="300px"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Description" SortExpression="Description" HeaderText="Description"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="DateAdded" SortExpression="DateAdded" HeaderText="DateAdded"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="PackageID" SortExpression="PackageID" HeaderText="Package ID"></asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="DataGridPager" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid></td>
				</tr>
			</table>
			<input style="DISPLAY: none; VISIBILITY: hidden" type="text" /><!-- workaround for weird IE behavior when auto-submitting via Enter key -->
		</div>
	</form>
</body>
</html>
