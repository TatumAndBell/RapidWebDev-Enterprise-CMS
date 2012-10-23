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
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using RapidWebDev.ExtensionModel;
using RapidWebDev.ExtensionModel.Linq;

namespace RapidWebDev.Tests.ExtensionModel
{
	/// <summary>
	/// 用于测试ExtensionObjectSerializer序列化的实体类
	/// </summary>
	public class MockExtensionObject4Serializer : AbstractExtensionObject
	{
		public override string ExtensionData { get; set; }
		public override Guid ExtensionDataTypeId { get; set; }
	}
}

