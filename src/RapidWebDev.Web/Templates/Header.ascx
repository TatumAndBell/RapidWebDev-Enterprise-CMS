<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="RapidWebDev.Web.Templates.Header, RapidWebDev.Web" %>

<script type="text/javascript">
	function showCopyright()
	{
		var options = { width: 960, height: 600, draggable: false, modal: true, resizable: false };
		window.ModalWindowHandler.Show('<%= Resources.Common.License %>', '/Copyright.aspx', options);
	}
</script>
<table cellpadding="0" cellspacing="0" border="0" style="width:100%" class="headtemplate">
	<tr>
		<td style="padding:4px 0 4px 12px"><a href="http://www.rapidwebdev.org/home.mvc" target="_blank"><img src="/resources/images/logo.png" alt="RapidWebDev Logo" title="" /></a></td>
		<td class="center">
			<table cellpadding="0" cellspacing="0" style="width:100%">
				<tr>
					<td align="left" style="padding-left: 6px;">&nbsp;</td>
					<td align="right">
						<My:UserLink ID="CurrentLogOnUser" runat="server" />&nbsp;
						<a href="#" onclick="showCopyright()"><%= Resources.Common.License %></a>&nbsp;
						<a href="/LogOut.aspx" title=""><%= Resources.Common.Logout %></a>
					</td>
				</tr>
			</table>
		</td>
		<td class="right">&nbsp;</td>
	</tr>
</table>