<%@ Control Language="C#" %>

<asp:Panel ID="PanelContainer" runat="server">
	<table cellpadding="0" cellspacing="0" class="table6col">
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationType_Name%>: </td>
			<td class="c2" nowrap="true">
				<My:TextBox ID="TextBoxName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxName.ClientID %>" class="required">*</label>
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationType_Description%>: </td>
			<td class="span" colspan="3" nowrap="true">
				<My:TextBox ID="TextBoxDescription" CssClass="textarea textboxLong" MaxLength="255" runat="server" />
			</td>
		</tr>
	</table>
</asp:Panel>