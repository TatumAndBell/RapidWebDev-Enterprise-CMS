<%@ Control Language="C#" %>

<asp:Panel ID="PanelContainer" runat="server">
	<table cellpadding="0" cellspacing="0" class="table6col">
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Name %>: </td>
			<td class="c2" nowrap="nowrap">
				<My:TextBox ID="TextBoxName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxName.ClientID %>" class="required">*</label>
			</td>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.ParentHierarchyData%>: </td>
			<td class="span" colspan="3" nowrap="nowrap">
				<My:ComboBox ID="ComboBoxParentHierarchyData" Mode="Local" Editable="true" ForceSelection="true" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="c1" nowrap="nowrap"><%= Resources.Membership.Description%>: </td>
			<td class="span" colspan="5">
				<My:TextBox ID="TextBoxDescription" CssClass="textarea textboxLong" TextMode="MultiLine" runat="server" />
			</td>
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
				<td class="span" colspan="3">
					<My:TextBox ID="TextBoxLastUpdatedDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" />
				</td>
			</tr>
		</asp:PlaceHolder>
	</table>
</asp:Panel>