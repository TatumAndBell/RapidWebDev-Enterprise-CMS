<%@ Control Language="C#" %>

<asp:Panel ID="PanelContainer" runat="server">
	<table cellpadding="0" cellspacing="0" class="table6col">
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationCode %>: </td>
			<td class="c2" nowrap="nowrap">
				<My:TextBox ID="TextBoxOrganizationCode" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxOrganizationCode.ClientID %>" class="required">*</label>
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationName %>: </td>
			<td class="c2" nowrap="nowrap">
				<My:TextBox ID="TextBoxOrganizationName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxOrganizationName.ClientID %>" class="required">*</label>
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationParent %>: </td>
			<td class="c2" nowrap="nowrap">
				<My:OrganizationSelector ID="ParentOrganizationSelector" MaxItemCount="50" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationTypeShortDesc%>: </td>
			<td class="c2" nowrap="nowrap">
				<My:ComboBox ID="DropDownListOrganizationType" Mode="Local" Editable="false" ForceSelection="true" runat="server" /><label for="<%= this.DropDownListOrganizationType.ClientID %>" class="required">*</label>
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Area%>: </td>
			<td class="c2" nowrap="nowrap">
				<My:HierarchySelector ID="AssociatedAreaSelector" MaxItemCount="50" Cascading="SingleCheck" runat="server" />
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Status %>: </td>
			<td class="c2" nowrap="nowrap">
			    <asp:RadioButtonList ID="RadioButtonListOrganizationStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server">
					<asp:ListItem Selected="True" Text = "<%$ Resources:Membership, Enabled %>" Value="Enabled"></asp:ListItem>
			        <asp:ListItem Text = "<%$ Resources:Membership, Pending %>" Value="Pending" ></asp:ListItem>
					<asp:ListItem Text="<%$ Resources:Membership, Disabled %>" Value="Disabled" ></asp:ListItem>
			    </asp:RadioButtonList>
			</td>
		</tr>
		
		<tr>
			<td colspan="6"><hr /></td>
		</tr>
		
		<My:ExtensionDataForm ID="OrganizationExtensionDataForm" runat="server" />
		
		<tr>
			<td colspan="6"><hr /></td>
		</tr>
		
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.OrganizationDescription %>: </td>
			<td class="span" colspan="5">
				<My:TextBox ID="TextBoxDescription" CssClass="textarea textboxLong" TextMode="MultiLine" runat="server" />
			</td>
		</tr>
		
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
					<My:TextBox ID="TextBoxCreatedOn" CssClass="textboxShort readonly" ReadOnly="true" runat="server" />
				</td>
			</tr>
			<tr>
				<td class="c1" nowrap="nowrap"><%= Resources.Membership.ModifiedBy %>: </td>
				<td class="c2">
					<My:UserLink ID="UserLinkLastModifiedBy" runat="server" />
				</td>
				<td class="c1" nowrap="nowrap"><%= Resources.Membership.ModifiedOn %>: </td>
				<td class="span" colspan="3">
					<My:TextBox ID="TextBoxLastModifiedOn" CssClass="textboxShort readonly" ReadOnly="true" runat="server" />
				</td>
			</tr>
		</asp:PlaceHolder>
	</table>
</asp:Panel>