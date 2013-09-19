<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Src="Banner.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= Header1.SiteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />

	<!-- Load jQuery 1.x for IE8, and jQuery 2.x for all other browsers. -->
	<!--[if lt IE 9]>
    <script type="text/javascript" src="<%= _javascriptUrl %>/jquery.js" id="jquery"></script>
	<![endif]-->
	<!--[if gte IE 9]><!-->
	<script type="text/javascript" src="<%= _javascriptUrl %>/jquery2.js" id="jquery"></script>
	<!--<![endif]-->
	<script type="text/javascript" src="scripts/sampleportal.js"></script>
	<script type="text/javascript" src="<%= _javascriptUrl %>/Silverlight.js"></script>
	<script type="text/javascript">
		document.cookie = 'SilverlightAvailable=' + Silverlight.isInstalled('5.0');
	</script>
</head>
<body>
	<form id="form1" runat="server">
		<div>
			<uc1:Header ID="Header1" runat="server" Mode="Manage" />
			<table id="pageContent" border="0">
				<tr>
					<td>
						<table id="DataGridSearchTable" border="0">
							<tr>
								<td style="vertical-align: bottom;">Select a Template:</td>
								<td>
									<div class="hd-sp-searchbox">
										<div>Search:&nbsp;</div>
										<div>
											<asp:TextBox ID="txtSearch" runat="server" CssClass="InputField" onkeyup="txtSearch_FilterSearchText()"></asp:TextBox>
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

    <script type="text/javascript">
        function txtSearch_FilterSearchText() {
            var txtSearchCtl = document.getElementById('<%=txtSearch.ClientID %>');
            var searchText = txtSearchCtl.value;
            var bChanged = false;
            if (searchText.indexOf("&", 0) != -1) {
                searchText = searchText.replace("&", "");
                bChanged = true;
            }
            if (searchText.indexOf("<", 0) != -1) {
                searchText = searchText.replace("<", "");
                bChanged = true;
            }
            if (bChanged) {
                txtSearchCtl.value = searchText;
            }
        }

    </script>

</body>
</html>
