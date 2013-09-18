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
		function LoadEmbeddedInterview()
		{
			var cloudSvcUrl = '<%= SamplePortal.Settings.CloudServicesAddress %>';
			if (cloudSvcUrl != "")
				HD$.CloudServicesAddress = cloudSvcUrl;
			HD$.CreateInterviewFrame('interview', '<%= GetSessionID() %>');
		}
	</script>
</head>
<body onload="LoadEmbeddedInterview();">
	<form id="form1" runat="server">
		<uc1:Header ID="Header1" runat="server" HomeLink="true" />
		<div id="interview" style="width: 100%; height: 600px; border-bottom: 1px solid black;"></div>
	</form>
</body>
</html>
