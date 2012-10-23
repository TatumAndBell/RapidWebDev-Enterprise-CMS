<%@ Control Language="C#" %>

<table cellpadding="0" cellspacing="0" class="table6col">
	<tr>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.FieldName %>: </td>
		<td class="c2" nowrap="true">
			<My:TextBox ID="TextBoxName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxName.ClientID %>" class="required">*</label>
		</td>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.IsRequired%>: </td>
		<td class="c2" nowrap="true">
			<My:CheckBox ID="CheckBoxRequired" Text="<%$ Resources:ExtensionModel, Yes %>" runat="server" />
		</td>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.Ordinal%>: </td>
		<td class="c2" nowrap="true">
			<My:IntegerTextBox ID="IntegerTextBoxOrdinal" CssClass="textboxShort" AllowNegative="false" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.FieldGroup%>: </td>
		<td class="c2" nowrap="true">
			<My:TextBox ID="TextBoxFieldGroup" CssClass="textboxShort" MaxLength="127" runat="server" />
		</td>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.Priviledge%>: </td>
		<td class="span" nowrap="true" colspan="3">
			<asp:DropDownList ID="DropDownListPriviledge" Width="154px" runat="server">
				<asp:ListItem Text="<%$ Resources:ExtensionModel, Public %>" Value="Public" Selected="True" />
				<asp:ListItem Text="<%$ Resources:ExtensionModel, EditProtectedOnly %>" Value="EditProtectedOnly" />
				<asp:ListItem Text="<%$ Resources:ExtensionModel, BothEditAndViewProtected %>" Value="BothEditAndViewProtected" />
			</asp:DropDownList>
		</td>
	</tr>
	<tr>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.DefaultValue%>: </td>
		<td class="c2" nowrap="true">
			<My:DecimalTextBox ID="DecimalTextBoxDefaultValue" CssClass="textboxShort" runat="server" />
		</td>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.MinValue%>: </td>
		<td class="c2" nowrap="true">
			<My:DecimalTextBox ID="DecimalTextBoxMinValue" CssClass="textboxShort" runat="server" />
		</td>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.MaxValue%>: </td>
		<td class="c2" nowrap="true">
			<My:DecimalTextBox ID="DecimalTextBoxMaxValue" CssClass="textboxShort" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="c1" nowrap="nowrap"><%= Resources.ExtensionModel.Description%>: </td>
		<td class="span" colspan="5" nowrap="true">
			<My:TextBox ID="TextBoxDescription" CssClass="textboxLong" MaxLength="255" runat="server" />
		</td>
	</tr>
</table>