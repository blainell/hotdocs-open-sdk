<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Embedded.aspx.cs" Inherits="Embedded" %>

<%@ Register Src="Banner.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= Header1.SiteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
	<script type="text/javascript" src="http://files.hotdocs.ws/download/easyXDM.min.js"></script>
	<script type="text/javascript" src="http://files.hotdocs.ws/download/hotdocs.js"></script>
	<script type="text/javascript">
		function LoadEmbeddedInterview(bShow)
		{
			if (bShow == "True")
			{
				document.getElementById('TemplateInterview').style.display = 'block';

				var cloudSvcUrl = '<%= SamplePortal.Settings.CloudServicesAddress %>';
				if (cloudSvcUrl != "")
					HD$.CloudServicesAddress = cloudSvcUrl;
				HD$.CreateInterviewFrame('TemplateInterview', '<%= GetSessionID() %>');
			}
		}
	</script>
</head>
<body onload="LoadEmbeddedInterview('<%= ShouldShowInterview %>');">
	<uc1:Header ID="Header1" runat="server" HomeLink="true" />
	<form id="form1" runat="server">
		<p>Select Template > Answers > Complete Interview</p>
		<asp:DataGrid ID="tplGrid" runat="server" CssClass="DataGrid" DataSource="<%# _tplData %>" AllowPaging="False" AllowSorting="False" AutoGenerateColumns="False" CellPadding="3" BorderColor="#99B2CC" OnItemDataBound="tplGrid_ItemDataBound" OnSelectedIndexChanged="tplGrid_SelectedIndexChanged">
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
		</asp:DataGrid>
		<asp:DataGrid ID="ansGrid" runat="server" CellPadding="3" AutoGenerateColumns="False" DataSource="<%# ansData %>" AllowPaging="False" AllowSorting="True" CssClass="DataGrid" OnItemDataBound="ansGrid_ItemDataBound" OnSelectedIndexChanged="ansGrid_SelectedIndexChanged">
			<AlternatingItemStyle CssClass="DataGridAlternateItem"></AlternatingItemStyle>
			<ItemStyle CssClass="DataGridItem"></ItemStyle>
			<HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
			<Columns>
				<asp:ButtonColumn Text="Select" SortExpression="Title" HeaderText="Title" CommandName="Select">
					<HeaderStyle Width="200px"></HeaderStyle>
					<ItemStyle HorizontalAlign="Left"></ItemStyle>
				</asp:ButtonColumn>
				<asp:BoundColumn Visible="False" DataField="Filename" SortExpression="Filename" HeaderText="File Name"></asp:BoundColumn>
				<asp:BoundColumn Visible="False" DataField="Title" SortExpression="Title" HeaderText="Title">
					<HeaderStyle Width="40%"></HeaderStyle>
				</asp:BoundColumn>
				<asp:BoundColumn DataField="Description" SortExpression="Description" HeaderText="Description"></asp:BoundColumn>
				<asp:BoundColumn Visible="False" DataField="DateCreated" SortExpression="DateCreated" HeaderText="Created"></asp:BoundColumn>
				<asp:BoundColumn DataField="LastModified" SortExpression="LastModified" HeaderText="Modified">
					<HeaderStyle Width="150px"></HeaderStyle>
				</asp:BoundColumn>
			</Columns>
			<PagerStyle CssClass="DataGridPager" Mode="NumericPages"></PagerStyle>
		</asp:DataGrid>
		<div id="TemplateInterview" style="width: 100%; height: 600px; border-bottom: 1px solid black; display: none;"></div>
	</form>

</body>
</html>
