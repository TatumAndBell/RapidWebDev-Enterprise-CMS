<%@ Page Language="C#" MasterPageFile="~/LayoutCenter.Master" AutoEventWireup="true" CodeBehind="LogOn.aspx.cs" Inherits="RapidWebDev.Web.LogOn" Title="<%$ Resources:Common, Logon_PageTitle%>" %>
<asp:Content ID="ContentMain" ContentPlaceHolderID="Main" runat="server">
	
	<script type="text/javascript">
		Ext.onReady(function()
		{
			resizeWin();
			$("div.content").css("vertical-align", "middle");
			$("#<%=this.textboxUser.ClientID %>").focus();

			// 在用户输入框中敲回车
			$("#<%=this.textboxUser.ClientID %>").keydown(function(event)
			{
				if (event.keyCode == 13)
				{
					$("#<%=this.textboxPassword.ClientID %>").focus();
					return false;
				}
			});

			// 在密码输入框中敲回车
			$("#<%=this.textboxPassword.ClientID %>").keydown(function(event)
			{
				if (event.keyCode == 13)
				{
					var clientEvent = $("#<%=this.ButtonLogin.ClientID %>").attr("href");
					eval(clientEvent);
					return false;
				}
			});
		});

		$(window).resize(function() {
			resizeWin();
		});

		function resizeWin()
		{
			var h = document.documentElement.clientHeight;
			$("div.content").height(h - 10);
		}
	</script>
	
	<style type="text/css">	
		.logocontainer
		{
			height: 580px; 
			width: 800px;
			background-color: #E8E8E8; 
			background-image: url(resources/images/logo_background.jpg);
			background-repeat: no-repeat;
		}
		
		.logoblock
		{
			padding-left: 45px;
			padding-top: 130px;
		}
	</style>
	
	<asp:ScriptManager ID="ScriptManagerObj" EnablePartialRendering="true" EnableScriptGlobalization="true" runat="server" />
	
	<table cellpadding="0" cellspacing="0" style="height:100%; width:100%">
		<tr>
			<td>
				<div class="logocontainer">
					<asp:Panel ID="PanelContainer" DefaultButton="ButtonLogin" CssClass="logoblock" runat="server">
						<asp:UpdatePanel ID="UpdatePanelObj" runat="server">
							<ContentTemplate>
								<My:Container ID="ContainerLoginSection" Frame="true" Width="360px" runat="server">
									<h4><%= Resources.Common.Logon_ContainerHeader %></h4>
									<table cellpadding="0" cellspacing="0" style="width:100%">
										<tr>
											<td rowspan="3" style="width: 96px;" valign="top">
												<img src="/Resources/Images/login.gif" alt="" />
											</td>
											<td style="width: 65px; padding:2px 10px 2px 10px" nowrap="true"><%= Resources.Common.Logon_UserName%>: </td>
											<td style="padding-top: 2px; padding-bottom:2px; width: 80%">
												<My:TextBox ID="textboxUser" CssClass="textboxShort" runat="server" />
												<asp:RequiredFieldValidator ID="RequiredFieldValidator4textboxUser" ControlToValidate="textboxUser"
													EnableClientScript="true" Display="None" SetFocusOnError="true" ValidationGroup="Login" runat="server" />
											</td>
										</tr>
										<tr>
											<td style="padding:2px 10px 2px 10px"><%= Resources.Common.Logon_Password%>: </td>
											<td style="padding-top: 2px; padding-bottom:2px">
												<My:TextBox ID="textboxPassword" CssClass="textboxShort" runat="server" TextMode="Password" />
												<asp:RequiredFieldValidator ID="RequiredFieldValidator4textboxPass" ControlToValidate="textboxPassword"
													EnableClientScript="true" Display="None" SetFocusOnError="true" ValidationGroup="Login" runat="server" />
											</td>
										</tr>
										<tr>
											<td style="padding:2px 10px 2px 10px">&nbsp;</td>
											<td style="padding-top: 2px; padding-bottom:2px">
												<My:Button ID="ButtonLogin" Text="<%$ Resources:Common, Logon%>" ValidationGroup="Login" runat="server" />
											</td>
										</tr>
									</table>
								</My:Container>
								<My:MessagePanel ID="MessagePanelLogin" runat="server" />
							</ContentTemplate>
						</asp:UpdatePanel>
					</asp:Panel>
				</div>
			</td>
		</tr>
	</table>
</asp:Content>
