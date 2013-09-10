<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SelAns.aspx.cs" Inherits="SelAns" %>

<!DOCTYPE HTML>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title><%= _siteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
</head>
<body onload="OnSelChange()">
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
						<p><strong>Choose an answer set to begin the assembly.</strong></p>
						<p>
							Either select one of these, and click Continue...<br />
						</p>
						<table>
							<tr>
								<td style="width:16px;">&nbsp;</td>
								<td style="vertical-align:top;">
									<input type="radio" id="rbNew" checked="checked" value="new" name="AnsFileType" onclick="OnSelChange()" /></td>
								<td style="vertical-align:top;">
									<label for="rbNew">Begin using a new, empty answer set</label></td>
							</tr>
							<tr>
								<td style="width:16px;">&nbsp;</td>
								<td style="vertical-align:top;">
									<input type="radio" id="rbUpload" value="upload" name="AnsFileType" onclick="OnSelChange()" /></td>
								<td style="vertical-align:top;">
									<label for="rbUpload">Use a HotDocs answer file from my PC:</label>
									&nbsp;&nbsp;&nbsp;
									<input class="InputField" id="fileUpload" disabled="disabled" type="file" name="fileUpload" accept=".anx" runat="server" />
								</td>
							</tr>
							<tr>
								<td style="width:16px;">&nbsp;</td>
								<td style="vertical-align:top;" colspan="2">
									<asp:Button ID="btnContinue" runat="server" Text="Continue" ToolTip="Proceed to interview" CssClass="InputField" OnClick="btnContinue_Click"></asp:Button></td>
							</tr>
						</table>
						&nbsp;<br />
						... or select an answer set stored on the server:
						<table id="DataGridSearchTable" border="0">
							<tr>
								<td style="vertical-align:bottom;">Saved&nbsp;Answers:</td>
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
						<asp:DataGrid ID="ansGrid" runat="server" BorderColor="#99B2CC" CellPadding="3" AutoGenerateColumns="False" DataSource="<%# ansData %>" AllowPaging="True" AllowSorting="True" CssClass="DataGrid" OnItemCreated="ansGrid_ItemCreated" OnItemDataBound="ansGrid_ItemDataBound" OnPageIndexChanged="ansGrid_PageIndexChanged" OnSelectedIndexChanged="ansGrid_SelectedIndexChanged" OnSortCommand="ansGrid_SortCommand">
							<AlternatingItemStyle CssClass="DataGridAlternateItem"></AlternatingItemStyle>
							<ItemStyle CssClass="DataGridItem"></ItemStyle>
							<HeaderStyle CssClass="DataGridHeader"></HeaderStyle>
							<Columns>
								<asp:ButtonColumn Text="Select" SortExpression="Title" HeaderText="Title" CommandName="Select">
									<HeaderStyle HorizontalAlign="Left" Width="200px" CssClass="DataGridHeader"></HeaderStyle>
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
					</td>
				</tr>
			</table>
			<input type="text" style="DISPLAY: none; VISIBILITY: hidden" /><!-- workaround for weird IE behavior when auto-submitting via Enter key -->
		</div>
	</form>
</body>
</html>
