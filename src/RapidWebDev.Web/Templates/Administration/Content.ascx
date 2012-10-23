<%@ Control Language="C#" %>

<My:JSScript Type="Block" runat="server">
function previewWiki()
{
	var previewPageUrl = "http://localhost:55947/WikiPreview/View";

	var tempForm = document.createElement("form");
	tempForm.action = previewPageUrl;
	tempForm.method = "post";
	document.body.appendChild(tempForm);

	// Subject
	var tempInput = document.createElement("input");
	tempInput.type = "hidden";
	tempInput.name = "Subject";
	tempInput.value = $(".wikisubject").val();
	tempForm.appendChild(tempInput);

	// Body
	tempInput = document.createElement("input");
	tempInput.type = "hidden";
	tempInput.name = "Body";
	tempInput.value = $(".wikibody").val();
	tempForm.appendChild(tempInput);
	
	// ContentType
	tempInput = document.createElement("input");
	tempInput.type = "hidden";
	tempInput.name = "ContentType";
	tempInput.value = window.contentType;
	tempForm.appendChild(tempInput);

	window.open(previewPageUrl, "popupwindow", "width=960,height=640");
	tempForm.target = "popupwindow";
	tempForm.submit();　
}
</My:JSScript>

<asp:Panel ID="PanelContainer" runat="server">
	<table cellpadding="0" cellspacing="0" class="table6col">
		<tr>
			<td class="c1" nowrap="nowrap">Subject: </td>
			<td class="c2">
				<My:TextBox ID="TextBoxName" CssClass="wikisubject textboxShort" MaxLength="255" runat="server" /><span class="required">*</span>
			</td>
			<td class="c1" nowrap="nowrap">UniqueKey: </td>
			<td class="c2">
				<My:TextBox ID="TextBoxValue" CssClass="textboxShort" MaxLength="255" runat="server" />
			</td>
			<td class="c1" nowrap="nowrap">Status: </td>
			<td class="c2">
				<asp:RadioButtonList ID="RadioButtonListStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server">
					<asp:ListItem Selected="True" Text = "<%$ Resources:Membership, Enabled %>" Value="NotDeleted"></asp:ListItem>
					<asp:ListItem Text = "<%$ Resources:Membership, Disabled %>" Value="Deleted" ></asp:ListItem>
				</asp:RadioButtonList>
			</td>
		</tr>
		<tr>
			<td class="c1" nowrap="nowrap">
				Body: <br /><br />
				<a href="http://codeplex.codeplex.com/Wikipage?title=CodePlex%20Wiki%20Markup%20Guide" target="_blank" style="text-decoration:underline">Wiki Markup</a><br />
				<a href="#" onclick="previewWiki()" style="text-decoration:underline">Preview</a>
			</td>
			<td class="span" colspan="5">
				<My:TextBox ID="TextBoxDescription" CssClass="wikibody textarea textboxLong" Height="208px" TextMode="MultiLine" runat="server" />
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
				<td class="span" colspan="3">
					<My:TextBox ID="TextBoxLastUpdatedDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" />
				</td>
			</tr>
		</asp:PlaceHolder>
	</table>
</asp:Panel>