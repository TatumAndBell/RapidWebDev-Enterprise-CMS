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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using BaoJianSoft.Common;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Linq;
using BaoJianSoft.RapidWeb;
using BaoJianSoft.RapidWeb.DynamicPages;
using BaoJianSoft.RapidWeb.Controls;

namespace BaoJianSoft.Tests.BehaveTest
{
    public class BehaveOrganizationUtils
    {
        public OrganizationObject CreateOrganizationObject(Guid _OrganizationTypeId,string Name,string code)
        {
           
            
            OrganizationObject shOrganization = new OrganizationObject
            {
                OrganizationCode = code,
                OrganizationName = Name,
                OrganizationTypeId = _OrganizationTypeId,
                Status = OrganizationStatus.Enabled,
                Description = "sh-desc"
            };

            return shOrganization;
        }

        public OrganizationTypeObject CreateOrganizationTypeOject(string _Name,string _Domain)
        {
            OrganizationTypeObject department = new OrganizationTypeObject { Name = _Name, Domain = _Domain, Description = "department-desc" };

            return department;
        }
    }
}
