<%@ Control Language="C#" %>

<table cellpadding="0" cellspacing="0" class="table6col">
	<tr>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.FieldType %>: </td>
		<td class="c2" nowrap="true">
			<asp:DropDownList ID="DropDownListFieldMetadataType" AutoPostBack="true" Width="154px" runat="server">
				<asp:ListItem Value="" Text="" Selected="True" />
				<asp:ListItem Value="String" Text="<%$ Resources:ExtensionModel, String %>" />
				<asp:ListItem Value="DateTime" Text="<%$ Resources:ExtensionModel, DateTime %>" />
				<asp:ListItem Value="Date" Text="<%$ Resources:ExtensionModel, Date %>" />
				<asp:ListItem Value="Integer" Text="<%$ Resources:ExtensionModel, Integer %>" />
				<asp:ListItem Value="Decimal" Text="<%$ Resources:ExtensionModel, Decimal %>" />
				<asp:ListItem Value="Enumeration" Text="<%$ Resources:ExtensionModel, Enumeration %>" />
			</asp:DropDownList>
		</td>
		<td class="c1">&nbsp;</td>
		<td class="c2">&nbsp;</td>
		<td class="c1">&nbsp;</td>
		<td class="c2">&nbsp;</td>
	</tr>
</table>

<hr />

<asp:Panel ID="PanelFieldMetadata" EnableViewState="false" runat="server" />