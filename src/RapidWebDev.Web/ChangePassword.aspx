<%@ Page Language="C#" MasterPageFile="~/LayoutCenter.master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="RapidWebDev.Web.ChangePassword" Title="<%$ Resources:Membership, ChangePassword%>" %>
<asp:Content ID="ContentMain" ContentPlaceHolderID="Main" runat="server">
	<My:JSScript Type="File" Value="~/resources/javascript/ext-jquery-adapter.js" runat="server" />
	<My:JSScript Type="File" Value="~/resources/javascript/ext-all.js" runat="server" />
	<My:JSScript Type="File" Value="~/resources/javascript/MessageBox.js" runat="server" />
	
	<asp:ScriptManager ID="ScriptManagerObj" EnablePartialRendering="true" EnableScriptGlobalization="true" runat="server" />
	
	<asp:UpdatePanel ID="UpdatePanelChangePassword" runat="server">
        <ContentTemplate>
			<My:Container ID="PasswordContainer" Frame="true" Width="360px" HeaderText="<%$ Resources:Membership, ChangePassword%>" runat="server">
                <table cellpadding="0" cellspacing="0" class="table2col">
                    <tr>
                        <td class="c1"><%= Resources.Membership.ChangePassword_OriginalPassword%>: </td>
                        <td class="c2">
                            <My:TextBox ID="TextBoxOldPassword" TextMode="Password" CssClass="textbo150" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="c1"><%= Resources.Membership.ChangePassword_NewPassword%>: </td>
                        <td class="c2">
                            <My:TextBox ID="TextBoxNewPassword" TextMode="Password" CssClass="textbo150" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="c1"><%= Resources.Membership.ChangePassword_ConfirmPassword%>: </td>
                        <td class="c2">
                            <My:TextBox ID="TextBoxConfirmPassword" TextMode="Password"  CssClass="textbo150" runat="server" />
                        </td>
                    </tr>
                    <tr>
						<td colspan="2" class="span" style="text-align:center"><My:Button ID="ButtonSave" Text="<%$ Resources:Membership, Confirm%>" runat="server" /></td>
                    </tr>
                </table>
			</My:Container>
            
            <My:MessagePanel ID="MessagePanelChangePassword" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>
</asp:Content>
