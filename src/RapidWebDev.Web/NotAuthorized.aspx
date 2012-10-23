<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ExceptionMaster.Master" Title="<%$ Resources:Common, NotAuthorized_PageTitle%>" %>

<asp:Content ID="ContentBody" ContentPlaceHolderID="Body" runat="server">
	<%= Resources.Common.NotAuthorized_Message%><br /><br />
	<a href="javascript:gotoLogin()"><%= Resources.Common.ReturnToDefaultPage%></a>。
</asp:Content>