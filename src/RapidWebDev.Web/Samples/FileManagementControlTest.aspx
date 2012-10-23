<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileManagementControlTest.aspx.cs" Inherits="RapidWebDev.Web.Samples.FileManagementControlTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/resources/css/ext-all.css" />
    <link rel="stylesheet" type="text/css" href="/resources/css/global.css" />
    <link rel="stylesheet" type="text/css" href="/resources/css/uploadify.css" />
    <script type="text/javascript" src="/resources/javascript/jquery.js"></script>
    <script type="text/javascript" src="/resources/javascript/ext-jquery-adapter.js"></script>
    <script type="text/javascript" src="/resources/javascript/ext-all.js"></script>
    <script type="text/javascript" src="/resources/javascript/uploadify/swfobject.js"></script>
    <script type="text/javascript" src="/resources/javascript/uploadify/jquery.uploadify.v2.1.0.min.js"></script>
    <script type="text/javascript" src="/resources/javascript/filemanagementcontrol.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<asp:ScriptManager ID="ScriptManagerObj" EnablePartialRendering="true" EnableScriptGlobalization="true" runat="server" />
		
		<asp:UpdatePanel ID="UpdatePanelObj" runat="server">
			<ContentTemplate>
				<asp:Panel ID="PageContainer" Width="600px" runat="server">
					ExternalObjectId: <asp:TextBox ID="TextBoxExternalObjectId" Width="250px" runat="server" />
					<My:FileManagementControl ID="ProductAttachmentFileManagementControl" FileCategory="ProductAttachment" RelationshipType="Attachment" runat="server" />
				</asp:Panel>
				
				<asp:Button ID="ButtonLoad" Text="Load" runat="server" /> 
				<asp:Button ID="ButtonSave" Text="Save" runat="server" /> <br />
				<asp:Label ID="LabelMessage" ForeColor="red" runat="server" />
			</ContentTemplate>
		</asp:UpdatePanel>
    </form>
</body>
</html>
