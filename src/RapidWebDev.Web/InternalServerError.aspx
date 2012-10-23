<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ExceptionMaster.Master" Title="<%$ Resources:Common, InternalServerError%>" %>

<asp:Content ID="ContentBody" ContentPlaceHolderID="Body" runat="server">
	<%= Resources.Common.InternalServerError_Message %><br /><br />
	<a href="Default.aspx"><%= Resources.Common.ReturnToDefaultPage%></a>
</asp:Content>