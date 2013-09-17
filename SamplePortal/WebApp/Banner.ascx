<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Banner.ascx.cs" Inherits="Header" %>

<iframe class="KeepAlive" src="./keep-alive.aspx" width="0" height="0" runat="server"></iframe>
<%if (Mode != HeaderState.Hidden)
  { %>
<table id="pageHeader" border="0">
	<tr>
		<td id="pageHeaderLeft">
			<div class="hd-sp-img hd-sp-img-header">
				<div class="hd-sp-title"><%= SiteName %></div>
			</div>
		</td>
		<td id="pageHeaderMiddle">
			<%if (Mode == HeaderState.Home)
	 { %>
			<a href="Default.aspx" class="HeaderLink" title="<%=HomeLinkText %>">&gt;&nbsp;Home</a>
			<%} %>
			<%if (Mode == HeaderState.Manage)
	 { %>
			<asp:LinkButton ID="btnManageAnswers" runat="server"
				ToolTip="Edit/Delete answer sets" OnClick="btnManageAnswers_Click"
				CssClass="HeaderLink">Manage&nbsp;Answers</asp:LinkButton><br />
			<asp:LinkButton ID="btnManageTemplates" runat="server"
				ToolTip="Edit/Delete templates" OnClick="btnManageTemplates_Click"
				CssClass="HeaderLink">Manage&nbsp;Templates</asp:LinkButton></td>
		<%} %>
		<td id="pageHeaderRight">
			<a href="http://www.hotdocs.com/products/server/" target="_blank" title="Powered by HotDocs Server">
				<div class="hd-sp-img hd-sp-img-hds"></div>
			</a>
		</td>
	</tr>
</table>
<%} %>
