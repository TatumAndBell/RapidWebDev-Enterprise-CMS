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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using RapidWebDev.Common;
using RapidWebDev.UI.Controls;
using RapidWebDev.UI.Properties;

namespace RapidWebDev.UI.DynamicPages.Configurations
{
	/// <summary>
	/// Class to configure gridview field value transform strategy.
	/// </summary>
	public class GridViewFieldValueTransformConfiguration
	{
		/// <summary>
		/// Sets/gets gridView field value transform type.
		/// </summary>
		public GridViewFieldValueTransformTypes TransformType { get; set; }

		/// <summary>
		/// Variable output string in the rendered cell. 
		/// It supports variables defined like "http://www.rapidwebdev.org/article.aspx?id=$Id: GridViewFieldValueTransformConfiguration.cs 486 2009-12-11 17:00:53Z eungeliu $".
		/// The variable should be an accessible property of the object in data source, or existed variable in IApplicationContext.LabelVariables, or an existed globalization resources path.
		/// </summary>
		public string VariableString { get; set; }

		/// <summary>
		/// The format parameter for converting the field value to string. E.g, DateTime.Now.ToString(string format).
		/// </summary>
		public string ToStringParameter { get; set; }

		/// <summary>
		/// Use callback implementation to process output field value.
		/// The implementation must implement the interface IGridViewFieldValueTransformCallback.
		/// </summary>
		public IGridViewFieldValueTransformCallback Callback { get; set; }

		/// <summary>
		/// Like a switch...case... syntax in C# language. 
		/// If field value matches a Case, attribute Output of the Case will be rendered.
		/// If no Case matched, the field value will be output directly as default.
		/// </summary>
		public Collection<GridViewFieldValueTransformSwitchCase> SwitchCases { get; set; }

		/// <summary>
		/// Empty Constructor
		/// </summary>
		public GridViewFieldValueTransformConfiguration()
		{
		}

		/// <summary>
		/// Construct GridViewFieldValueTransformConfiguration instance from xml element.
		/// </summary>
		/// <param name="transformElement"></param>
		/// <param name="xmlParser"></param>
		public GridViewFieldValueTransformConfiguration(XmlElement transformElement, XmlParser xmlParser)
		{
			switch (transformElement.LocalName)
			{
				case "Transform-ToString-Parameter":
					this.TransformType = GridViewFieldValueTransformTypes.ToStringParameter;
					this.ToStringParameter = xmlParser.ParseString(transformElement, "@Value");
					return;

				case "Transform-VariableString":
					this.TransformType = GridViewFieldValueTransformTypes.VariableString;
					this.VariableString = xmlParser.ParseString(transformElement, "@Value");
					return;

				case "Transform-Callback":
					this.TransformType = GridViewFieldValueTransformTypes.Callback;
					string callbackTypeName = xmlParser.ParseString(transformElement, "@Type");
					Type callbackType = Kit.GetType(callbackTypeName);
					if (callbackType != null)
					{
						ConstructorInfo callbackConstructor = callbackType.GetConstructor(Type.EmptyTypes);
						this.Callback = callbackConstructor.Invoke(null) as IGridViewFieldValueTransformCallback;
					}
					else
						throw new ConfigurationErrorsException(string.Format(Resources.DP_ConfiguredTransformCallbackTypeNotExists, callbackTypeName));

					return;

				case "Transform-Switch":
					this.TransformType = GridViewFieldValueTransformTypes.Switch;
					this.SwitchCases = new Collection<GridViewFieldValueTransformSwitchCase>();
					XmlNodeList switchCaseNodes = transformElement.SelectNodes("p:Case", xmlParser.NamespaceManager);
					foreach (XmlElement switchCaseElement in switchCaseNodes.Cast<XmlElement>())
						this.SwitchCases.Add(new GridViewFieldValueTransformSwitchCase(switchCaseElement, xmlParser));

					return;
			}
		}
	}
}

