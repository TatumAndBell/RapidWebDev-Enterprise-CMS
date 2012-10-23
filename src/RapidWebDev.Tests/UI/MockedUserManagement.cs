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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using RapidWebDev.UI.DynamicPages;
using RapidWebDev.UI.DynamicPages.Configurations;

namespace RapidWebDev.Tests.UI
{
	public class MockedUserManagement : IDynamicPage
	{
		#region IDynamicPage Members

		public DynamicPageConfiguration Configuration { get; set; }

		public QueryResults Query(QueryParameter parameter)
		{
			return new QueryResults(0, new ArrayList());
		}

		public void Create()
		{
		}

		public void Update(string entityId)
		{
		}

		public void Delete(string entityId)
		{
		}

		public void Reset()
		{
		}

		public void LoadWritableEntity(string entityId)
		{
		}

		public void LoadReadOnlyEntity(string entityId)
		{
		}

		public void OnGridRowControlsBind(GridRowControlBindEventArgs e)
		{
		}

		public void OnLoad()
		{
		}

		public void OnDetailPanelLoad()
		{
		}

		public void OnPreRender()
		{
		}

		public void OnDetailPanelPreRender()
		{
		}

		public void BulkOperate(string commandArgument, Collection<string> entityIdCollection)
		{
			
		}

		#endregion

		#region IDynamicPage Members


		public void OnInit(IRequestHandler sender, EventArgs e)
		{
		}

		public void OnLoad(IRequestHandler sender, EventArgs e)
		{
		}

		public void OnPreRender(IRequestHandler sender, EventArgs e)
		{
		}


		public Action<RapidWebDev.UI.MessageTypes, string> ShowMessage
		{
			get;
			set;
		}

		public void SetupContextTempVariables(IRequestHandler sender, SetupApplicationContextVariablesEventArgs e)
		{
			
		}

		#endregion
	}
}

