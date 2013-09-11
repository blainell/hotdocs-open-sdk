<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Upload.aspx.cs" Inherits="Upload" %>

<%@ Register Src="Banner.ascx" TagName="Header" TagPrefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>
		<%= _siteName %></title>
	<link href="css/SamplePortal.css" type="text/css" rel="stylesheet" />
</head>
<body>
	<uc1:Header ID="Header1" runat="server" Mode="NoLinks" />

	<div style="padding: 10px;">

		<h3>Template Package Upload</h3>

		<asp:Panel ID="panelHDFinished" runat="server" Visible="false">
			<asp:Label ID="lblStatus" runat="server" />
		</asp:Panel>

		<asp:Panel ID="panelOverwrite" runat="server" Visible="False">
			<form id="Form1" runat="server">
				<p>
					<strong>Overwrite Warning </strong>
				</p>
				<p>
					The following templates already exist on the server:
				</p>
				<p>
					<asp:Label ID="lblOverwriteList" runat="server" Visible="true" />
				</p>
				<p>
					Please exercise care when overwriting template files on the server.
				</p>
				<p>
					Would you like to overwrite the conflicting files?
				</p>
				<p>
					<asp:Button ID="btnContinue" runat="server" Text="Overwrite" CssClass="InputField"
						OnClick="btnContinue_Click"></asp:Button>&nbsp;
			<asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="InputField" OnClick="btnCancel_Click"></asp:Button>
				</p>
			</form>
		</asp:Panel>

		<asp:Panel ID="panelHDForm" runat="server" Visible="false">
			<p>
				HotDocs has established communication with
			<% =_siteName %>, and is ready to upload the packages you selected. Review the titles
			and descriptions below, and then click Upload to continue.
			</p>
			<form id="HD_Templates_Upload_Form" runat="server" enctype="multipart/form-data">
				<asp:HiddenField ID="TotalTemplateCount" runat="server" />
				<asp:Label ID="GridLabel" runat="server" />
				<table id="UploadedTemplatesGrid" runat="server"></table>
				<asp:Button ID="submitBtn" runat="server" Text="Upload" />
			</form>
		</asp:Panel>

		<asp:Panel ID="panelHDError" runat="server" Visible="false">
			<p>
				An unexpected error has occurred. An expected file was not included in the upload. Please contact technical support for assistance.
			</p>
		</asp:Panel>
	</div>
</body>
</html>
