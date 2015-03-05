<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Interview.aspx.cs" Inherits="Interview" %>

<%@ Register Src="Banner.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE HTML>
<html>
<head runat="server">
	<title><%= Header1.SiteName %></title>
	<meta name="viewport" content="initial-scale=1.0" />
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet">
</head>
<body id="theBody" runat="server">
	<uc1:Header ID="Header1" runat="server" Mode="Home" HomeLinkText="Cancel assembly and return to the home page" />
	<table id="SP-Interview-Container" border="0">
		<tr>
			<td>
				<div id="interviewContent"><%= this.GetInterview() %></div>
			</td>
		</tr>
	</table>
</body>
</html>

