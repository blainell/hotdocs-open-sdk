<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Interview.aspx.cs" Inherits="Interview" %>

<!DOCTYPE HTML>
<html>
<head runat="server">
	<title><%= _siteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet">
</head>
<body id="theBody" runat="server">
	<iframe id="KeepAlive" src="./keep-alive.aspx" width="0" height="0" runat="server"></iframe>
	<table id="pageHeader" border="0">
		<tr>
			<td id="pageHeaderLeft">
				<div class="hd-sp-img hd-sp-img-header">
					<div class="hd-sp-title"><%= _siteName %></div>
				</div>
			</td>
			<td id="pageHeaderMiddle"><a id="btnHome" href="Default.aspx" class="HeaderLink" title="Cancel assembly and return to the home page">&gt;&nbsp;Home</a></td>
			<td id="pageHeaderRight">
				<a href="http://www.hotdocs.com/products/server/" target="_blank" title="Powered by HotDocs Server">
					<div class="hd-sp-img hd-sp-img-hds"></div>
				</a>
			</td>
		</tr>
	</table>
	<table id="SP-Interview-Container" border="0">
		<tr>
			<td>
				<div id="interviewContent"><%= this.GetInterview() %></div>
			</td>
		</tr>
	</table>
</body>
</html>

