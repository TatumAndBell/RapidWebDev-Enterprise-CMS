<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ExceptionMaster.Master" Title="<%$ Resources:Common, PageNotFound_PageTitle%>" %>

<asp:Content ID="ContentBody" ContentPlaceHolderID="Body" runat="server">
	<%= Resources.Common.PageNotFound_Message%><br /><br />
	<a href="/Default.aspx"><%= Resources.Common.ReturnToDefaultPage%></a>
</asp:Content>