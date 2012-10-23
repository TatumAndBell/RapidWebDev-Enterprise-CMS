<%@ Control Language="C#" %>

<ajax:TabContainer ID="TabContainer" runat="server">
	<ajax:TabPanel ID="TabPanelProfile" HeaderText="<%$ Resources:Membership, Profile%>" runat="server">
		<ContentTemplate>
			<table cellpadding="0" cellspacing="0" class="table6col">
				<tr>
					<td class="c1" nowrap="nowrap"><%= Resources.Membership.Role_Name%>: </td>
					<td class="c2" nowrap="nowrap">
						<My:TextBox ID="TextBoxName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxName.ClientID %>" class="required">*</label>
					</td>
					<td class="c1" nowrap="nowrap"><%= Resources.Membership.Role_Description%>: </td>
					<td class="span" colspan="3">
						<My:TextBox ID="TextBoxDescription" CssClass="textboxLong" MaxLength="255" runat="server" />
					</td>
				</tr>
			</table>
		</ContentTemplate>
	</ajax:TabPanel>
	<ajax:TabPanel ID="TabPanelPermission" HeaderText="<%$ Resources:Membership, Permission%>" runat="server">
		<ContentTemplate>
			<My:PermissionTreeView ID="PermissionTreeView" runat="server" />
		</ContentTemplate>
	</ajax:TabPanel>
</ajax:TabContainer>