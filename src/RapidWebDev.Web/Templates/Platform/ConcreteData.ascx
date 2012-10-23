<%@ Control Language="C#" %>

<asp:Panel ID="PanelContainer" runat="server">
	<table cellpadding="0" cellspacing="0" class="table6col">
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Name %>: </td>
			<td class="c2" nowrap="nowrap">
				<My:TextBox ID="TextBoxName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxName.ClientID %>" class="required">*</label>
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Value %>: </td>
			<td class="c2" nowrap="nowrap">
				<My:TextBox ID="TextBoxValue" CssClass="textboxShort" MaxLength="255" runat="server" />
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Status %>: </td>
			<td class="c2" nowrap="nowrap">
				<asp:RadioButtonList ID="RadioButtonListStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server">
					<asp:ListItem Selected="True" Text = "<%$ Resources:Membership, Enabled %>" Value="NotDeleted"></asp:ListItem>
			        <asp:ListItem Text = "<%$ Resources:Membership, Disabled %>" Value="Deleted" ></asp:ListItem>
			    </asp:RadioButtonList>
			</td>
		</tr>
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Description%>: </td>
			<td class="span" colspan="5">
				<My:TextBox ID="TextBoxDescription" CssClass="textarea textboxLong" TextMode="MultiLine" runat="server" />
			</td>
		</tr>
		
		<tr>
			<td colspan="6"><hr /></td>
		</tr>
		
		<My:ExtensionDataForm ID="ExtensionDataForm" runat="server" />
		
		<asp:PlaceHolder ID="PlaceHolderOperatorContext" Visible="false" runat="server">
			<tr>
				<td colspan="6"><hr /></td>
			</tr>
			<tr>
				<td class="c1" nowrap="nowrap"><%= Resources.Membership.CreatedBy %>: </td>
				<td class="c2">
					<My:UserLink ID="UserLinkCreatedBy" runat="server" />
				</td>
				<td class="c1" nowrap="nowrap"><%= Resources.Membership.CreatedOn %>: </td>
				<td class="span" colspan="3">
					<My:TextBox ID="TextBoxCreatedDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="c1" nowrap="nowrap"><%= Resources.Membership.ModifiedBy %>: </td>
				<td class="c2">
					<My:UserLink ID="UserLinkLastUpdatedBy" runat="server" />
				</td>
				<td class="c1" nowrap="nowrap"><%= Resources.Membership.ModifiedOn %>: </td>
				<td class="span" colspan="3" nowrap="nowrap">
					<My:TextBox ID="TextBoxLastUpdatedDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" />
				</td>
			</tr>
		</asp:PlaceHolder>
	</table>
</asp:Panel>