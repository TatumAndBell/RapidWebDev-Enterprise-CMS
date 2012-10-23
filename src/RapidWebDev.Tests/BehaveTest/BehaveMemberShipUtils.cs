/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.yi@RapidWebDev.org

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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BaoJianSoft.Platform;
using BaoJianSoft.Common;
using BaoJianSoft.Platform.Initialization;
using BaoJianSoft.Platform.Linq;
namespace BaoJianSoft.Tests.BehaveTest
{
    public class BehaveMemberShipUtils
    {
        #region public
        public UserObject CreateUserObject(string displayName, bool IsApproved)
        {
            IPlatformConfiguration platformConfiguration = SpringContext.Current.GetObject<IPlatformConfiguration>();

            UserObject userObject = new UserObject
            {
                OrganizationId = platformConfiguration.Organization.OrganizationId,
                Comment = "IT specialist",
                DisplayName = displayName,
                Email = "euliu@hotmail.com",
                IsApproved = IsApproved,
                MobilePin = "137641855XX",
                UserName = displayName
            };

            userObject["Birthday"] = new DateTime(1982, 2, 7);
            userObject["Sex"] = "Male";
            userObject["IdentityNo"] = "51010419820207XXXX";
            userObject["EmployeeNo"] = "200708200002";
            userObject["Department"] = "Simulation";
            userObject["Position"] = "Team Lead";
            userObject["PhoneNo"] = "021-647660XX";
            userObject["City"] = "ShangHai";
            userObject["Address"] = "MeiLong 2nd Cun, MingHang District";
            userObject["ZipCode"] = "210000";

            return userObject;
        }

        public UserObject AddExtensionFields(UserObject _object)
        {
            _object["YILO"] = "YILO";

            return _object;
        }

        #endregion
    }
}
