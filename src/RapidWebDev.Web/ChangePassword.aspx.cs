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
using System.Text.RegularExpressions;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.Platform;
using RapidWebDev.UI;
using AspNetMembership = System.Web.Security.Membership;

namespace RapidWebDev.Web
{
    public partial class ChangePassword : SupportDocumentReadyPage
	{
		private static IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();

        protected override void OnInit(EventArgs e)
        {
			if (!authenticationContext.Identity.IsAuthenticated)
				WebUtility.NavigatePageFrameTo(WebUtility.NotAuthorizedUrl);

            base.OnInit(e);
            this.ButtonSave.Click += new EventHandler(ButtonSave_Click);
        }

        void ButtonSave_Click(object sender, EventArgs e)
        {
            if (this.TextBoxOldPassword.Text.Trim().Length == 0)
            {
				this.MessagePanelChangePassword.ShowWarning(Resources.Membership.ChangePassword_OriginalPasswordCannotBeEmpty);
                return;
            }

            if (this.TextBoxNewPassword.Text.Trim().Length == 0)
            {
				this.MessagePanelChangePassword.ShowWarning(Resources.Membership.ChangePassword_NewPasswordCannotBeEmpty);
                return;
            }

            if (this.TextBoxNewPassword.Text != this.TextBoxConfirmPassword.Text)
            {
				this.MessagePanelChangePassword.ShowWarning(Resources.Membership.ChangePassword_NewPasswordNotEqualToConfirmPassword);
                return;
            }

            if (this.TextBoxNewPassword.Text.Trim().Length < AspNetMembership.MinRequiredPasswordLength)
            {
				this.MessagePanelChangePassword.ShowWarning(Resources.Membership.ChangePassword_PasswordLessThanRequired, AspNetMembership.MinRequiredPasswordLength);
                return;
            }

            if (!Kit.IsEmpty(AspNetMembership.PasswordStrengthRegularExpression))
            {
                Regex regex = new Regex(AspNetMembership.PasswordStrengthRegularExpression, RegexOptions.Compiled);
                if (!regex.IsMatch(this.TextBoxNewPassword.Text.Trim()))
                {
					this.MessagePanelChangePassword.ShowWarning(Resources.Membership.ChangePassword_PasswordStrengthInvalid);
                    return;
                }
            }

            try
            {
                IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
                bool successful = SpringContext.Current.GetObject<IMembershipApi>().ChangePassword(authenticationContext.User.UserId, this.TextBoxOldPassword.Text, this.TextBoxNewPassword.Text);
                if (!successful)
                {
					this.MessagePanelChangePassword.ShowError(Resources.Membership.ChangePassword_ChangePasswordFailed);
                    return;
                }

				this.MessagePanelChangePassword.ShowConfirm(Resources.Membership.ChangePassword_ChangedPasswordSuccessfully);
            }
            catch (Exception exp)
            {
                //Logger.Instance(this).Info(exp);
				this.MessagePanelChangePassword.ShowError(string.Format(Resources.Membership.ChangePassword_ChangePasswordFailedWithException, exp.Message));
            }
        }
    }
}

