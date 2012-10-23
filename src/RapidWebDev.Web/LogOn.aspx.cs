/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Eunge, Legal Name: Jian Liu, Email: eunge.liu@RapidWebDev.org

	The GNU Library General Public License (LGPL) used in RapidWebDev is 
	intended to guarantee your freedom to share and change free software - to 
	make sure the software is free for all its users.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Library General Public License (LGPL) for more details.

	You should have received a copy of the GNU Library General Public License (LGPL)
	along with this program.  
	If not, see http://www.rapidwebdev.org/Content/ByUniqueKey/OpenSourceLicense
 ****************************************************************************************************/

using System;
using System.Web.Security;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.Platform;
using MembershipLoginStatus = RapidWebDev.Platform.LoginResults;

namespace RapidWebDev.Web
{
    public partial class LogOn : SupportDocumentReadyPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.ButtonLogin.Click += new EventHandler(ButtonLogin_Click);
        }

        void ButtonLogin_Click(object sender, EventArgs e)
        {
            IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
            try
            {
                LoginResults loginResults = authenticationContext.Login(this.textboxUser.Text, this.textboxPassword.Text);
                switch (loginResults)
                {
                    case MembershipLoginStatus.InvalidCredential:
                        this.MessagePanelLogin.ShowError(Resources.Common.Logon_InvalidUserCredential);
                        break;
                    case MembershipLoginStatus.InvalidOrganization:
						this.MessagePanelLogin.ShowError(Resources.Common.Logon_InvalidOrganizationCredential);
                        break;
					case LoginResults.LockedOut:
						this.MessagePanelLogin.ShowError(Resources.Common.Logon_UserIsLockedOut);
						break;
					case LoginResults.Successful:
						FormsAuthentication.RedirectFromLoginPage(this.textboxUser.Text, true);
						break;
                }
            }
            catch (Exception exp)
            {
                //Logger.Instance(this).Error(exp);
				this.MessagePanelLogin.ShowError(Resources.Common.Logon_UnknowException);
            }
        }
    }
}
