<%@ Control Language="C#" %>
			
<My:JSScript ID="JSScriptObj" Type="File" Value="~/Resources/JavaScript/User.RolesByOrganization.js" runat="server" />

<ajax:TabContainer ID="TabContainer" runat="server">
    <ajax:TabPanel ID="TabPanelProfile" HeaderText="<%$ Resources:Membership, Profile%>" runat="server">
		<ContentTemplate>						
		    <table cellpadding="0" cellspacing="0" class="table6col">
	            <tr>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.User_Organization%>: </td>
			        <td class="c2" nowrap="nowrap">
			            <My:OrganizationSelector ID="OrganizationSelector" MaxItemCount="50" runat="server" /><label for="<%= this.OrganizationSelector.ClientID %>" class="required">*</label>
			        </td>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.User_Name%>: </td>
			        <td class="span" colspan="3" nowrap="nowrap">
				        <My:TextBox ID="TextBoxUserName" CssClass="textboxShort" MaxLength="31" runat="server" /><label for="<%= this.TextBoxUserName.ClientID %>" class="required">*</label>
			        </td>
		        </tr>
		        <asp:PlaceHolder ID="PlaceHolderPassword" runat="server">
					<tr>
						<td class="c1" nowrap="nowrap"><%= Resources.Membership.User_Password%>: </td>
						<td class="c2" nowrap="nowrap">
							<My:TextBox ID="TextBoxPassword" CssClass="textboxShort" TextMode="Password" MaxLength="31" runat="server" /><label for="<%= this.TextBoxPassword.ClientID %>" class="required">*</label>
						</td>
						<td class="c1" nowrap="nowrap"><%= Resources.Membership.User_ConfirmPassword%>: </td>
						<td class="span" colspan="3" nowrap="nowrap">
							<My:TextBox ID="TextBoxConfirmPassword" CssClass="textboxShort" TextMode="Password" MaxLength="31" runat="server" /><label for="<%= this.TextBoxConfirmPassword.ClientID %>" class="required">*</label>
						</td>
					</tr>
		        </asp:PlaceHolder>
		        <tr>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.User_DisplayName%>: </td>
			        <td class="c2" nowrap="nowrap">
				        <My:TextBox ID="TextBoxDisplayName" CssClass="textboxShort" MaxLength="255" runat="server" /><label for="<%= this.TextBoxDisplayName.ClientID %>" class="required">*</label>
			        </td>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.Email%>: </td>
			        <td class="c2" nowrap="nowrap"><My:TextBox ID="TextBoxEmail" CssClass="textboxShort" MaxLength="255" runat="server" /></td>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.Mobile%>: </td>
			        <td class="c2" nowrap="nowrap"><My:TextBox ID="TextBoxMobile" CssClass="textboxShort" runat="server" /></td>
		        </tr>
		        
		        <tr>
	                <td colspan="6"><hr /></td>
	            </tr>
		            
		        <My:ExtensionDataForm ID="UserExtensionDataForm" runat="server" />
		        
				<tr>
	                <td colspan="6"><hr /></td>
	            </tr>
		        
		        <tr>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.User_Description%>: </td>
			        <td colspan="5" class="span"><My:TextBox ID="TextBoxComment" TextMode="MultiLine" CssClass="textarea textboxLong" runat="server" /></td>
		        </tr>
		        <tr>
			        <td class="c1" nowrap="nowrap"><%= Resources.Membership.Status%>: </td>
			        <td colspan="5" class="span">
				        <asp:RadioButtonList ID="RadioButtonListStatus" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server">
					        <asp:ListItem Selected="True" Text="<%$ Resources:Membership, Enabled %>" Value="True" />
					        <asp:ListItem Text="<%$ Resources:Membership, Disabled %>" Value="False" />
				        </asp:RadioButtonList>
			        </td>
		        </tr>
		        <asp:PlaceHolder ID="PlaceHolderOperateContext" Visible="false" runat="server">
		            <tr>
		                <td colspan="6"><hr /></td>
		            </tr>
		            <tr>
			            <td class="c1" nowrap="true"><%= Resources.Membership.CreatedOn%>: </td>
			            <td class="c2"><My:TextBox ID="TextBoxCreationDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" /></td>
			            <td class="c1" nowrap="true"><%= Resources.Membership.User_LastLogon%>: </td>
			            <td class="c2"><My:TextBox ID="TextBoxLastLoginDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" /></td>
			            <td class="c1" nowrap="true"><%= Resources.Membership.User_LastActive%>: </td>
			            <td class="c2"><My:TextBox ID="TextBoxLastActivityDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" /></td>
			        </tr>
		            <tr>
			            <td class="c1" nowrap="true"><%= Resources.Membership.User_LastLock%>: </td>
			            <td class="c2"><My:TextBox ID="TextBoxLockedOutDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" /></td>
			            <td class="c1" nowrap="true"><%= Resources.Membership.User_PasswordChangedOn%>: </td>
			            <td class="c2"><My:TextBox ID="TextBoxLastPasswordChangedDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" /></td>
			            <td class="c1" nowrap="true"><%= Resources.Membership.ModifiedOn%>: </td>
			            <td class="c2"><My:TextBox ID="TextBoxLastUpdatedDate" CssClass="textboxShort readonly" ReadOnly="true" runat="server" /></td>
		            </tr>
		        </asp:PlaceHolder>
		        <tr>
					<td colspan="6"><hr /></td>
		        </tr>
		        <tr>
					<td class="c1"><%= Resources.Membership.User_Roles%>: </td>
					<td class="span" colspan="5">
						<asp:Panel ID="PanelRoleContainer" Wrap="false" runat="server" />
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