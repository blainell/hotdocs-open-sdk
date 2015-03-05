<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Answers.aspx.cs" Inherits="answers" %>

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
			<table id="pageContent" border="0">
				<tr>
					<td>
						<table id="DataGridSearchTable" border="0">
							<tr>
								<td style="vertical-align: bottom">Manage Answer Sets:</td>
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
						<asp:DataGrid ID="ansGrid" runat="server" CssClass="DataGrid" CellPadding="3" AllowSorting="True" AllowPaging="True" DataSource="<%# _ansData %>" AutoGenerateColumns="False" BorderColor="#99B2CC" OnCancelCommand="ansGrid_CancelCommand" OnDeleteCommand="ansGrid_DeleteCommand" OnEditCommand="ansGrid_EditCommand" OnItemCreated="ansGrid_ItemCreated" OnPageIndexChanged="ansGrid_PageIndexChanged" OnSortCommand="ansGrid_SortCommand" OnUpdateCommand="ansGrid_UpdateCommand">
							<SelectedItemStyle CssClass="DataGridSelectedItem"></SelectedItemStyle>
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
								<asp:BoundColumn Visible="False" DataField="Filename" SortExpression="Filename" ReadOnly="True" HeaderText="File Name"></asp:BoundColumn>
								<asp:BoundColumn DataField="Title" SortExpression="Title" HeaderText="Title" ItemStyle-CssClass="hd-sp-gridItem">
									<HeaderStyle Width="35%"></HeaderStyle>
								</asp:BoundColumn>
								<asp:BoundColumn DataField="Description" SortExpression="Description" HeaderText="Description" ItemStyle-CssClass="hd-sp-gridItem"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="DateCreated" SortExpression="DateCreated" ReadOnly="True"
									HeaderText="Created"></asp:BoundColumn>
								<asp:BoundColumn DataField="LastModified" SortExpression="LastModified" ReadOnly="True" HeaderText="Modified">
									<HeaderStyle Width="150px"></HeaderStyle>
								</asp:BoundColumn>
							</Columns>
							<PagerStyle CssClass="DataGridPager" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
			<input type="text" style="DISPLAY: none; VISIBILITY: hidden" /><!-- workaround for weird IE behavior when auto-submitting via Enter key -->
		</div>
	</form>
</body>
</html>
