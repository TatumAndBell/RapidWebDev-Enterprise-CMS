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
using BaoJianSoft.Common;
using BaoJianSoft.Common.Data;
using BaoJianSoft.Platform;
using BaoJianSoft.Platform.Linq;
using NBehave.Narrator.Framework;
using NBehave.Spec.NUnit;
using NBehave.Spec.NUnit.Specs;

namespace BaoJianSoft.Tests.BehaveTest.Stories.SequenceNo
{
    public class CreateSequenceNoStory : SetupFixture
    {
        private ISequenceNoApi _sequenceNoApi;

        public CreateSequenceNoStory()
        {
            Console.WriteLine("=================Setup===================");

           
            base.GlobalSetup();

            _sequenceNoApi = SpringContext.Current.GetObject<ISequenceNoApi>();

            Console.WriteLine("============Ending Setup===================");
        }

        public void CleanUp()
        {
            Console.WriteLine("============Clean Up====================");

            using (MembershipDataContext ctx = DataContextFactory.Create<MembershipDataContext>())
            {
                IAuthenticationContext authenticationContext = SpringContext.Current.GetObject<IAuthenticationContext>();
                ctx.SequenceNos.Delete(s => s.ApplicationId == authenticationContext.ApplicationId);
                ctx.SubmitChanges();
            }
          


            Console.WriteLine("========Ending Clean Up====================");
        }
    }
}
