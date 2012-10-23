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

namespace RapidWebDev.Common
{
	/// <summary>
	/// Xml parser provides common methods to parse xml node inner text/attribute value to value in specified type.
	/// </summary>
	public class XmlParser
	{
		private static XmlParser defaultXmlParser;

		/// <summary>
		/// Get default xml parser without xml namespace manager and schema.
		/// </summary>
		public static XmlParser Default
		{
			get
			{
				if (defaultXmlParser == null)
					defaultXmlParser = new XmlParser(null, null);

				return defaultXmlParser;
			}
		}

		/// <summary>
		/// Gets xml namespace manager instance.
		/// </summary>
		public XmlNamespaceManager NamespaceManager { get; private set; }

		/// <summary>
		/// Get the xml schema instance to validate xml file.
		/// </summary>
		public XmlSchema Schema { get; private set; }

		/// <summary>
		/// Constructor with xml namespace manager specified.
		/// </summary>
		/// <param name="xmlNamespaceManager"></param>
		/// <param name="xmlSchema"></param>
		public XmlParser(XmlNamespaceManager xmlNamespaceManager, XmlSchema xmlSchema)
		{
			this.NamespaceManager = xmlNamespaceManager;
			this.Schema = xmlSchema;
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to string.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <returns></returns>
		public string ParseString(XmlNode parentNode, string xpath)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return null;

			return xmlnode.InnerText ?? xmlnode.Value;
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to boolean.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public bool ParseBoolean(XmlNode parentNode, string xpath, bool defaultValue)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return defaultValue;

			return Kit.ToBool(xmlnode.InnerText ?? xmlnode.Value, defaultValue);
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to integer.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public int ParseInt(XmlNode parentNode, string xpath, int defaultValue)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return defaultValue;

			int result;
			if (int.TryParse(xmlnode.InnerText ?? xmlnode.Value, out result)) return result;

			return defaultValue;
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to integer.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public int? ParseInt(XmlNode parentNode, string xpath, int? defaultValue)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return defaultValue;

			int result;
			if (int.TryParse(xmlnode.InnerText ?? xmlnode.Value, out result)) return result;

			return defaultValue;
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to decimal.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public decimal? ParseDecimal(XmlNode parentNode, string xpath, decimal? defaultValue)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return null;

			decimal result;
			if (decimal.TryParse(xmlnode.InnerText ?? xmlnode.Value, out result)) return result;

			return defaultValue;
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to datetime.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public DateTime? ParseDateTime(XmlNode parentNode, string xpath, DateTime? defaultValue)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return null;

			DateTime result;
			if (DateTime.TryParse(xmlnode.InnerText ?? xmlnode.Value, out result)) return result;

			return defaultValue;
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to Unit.
		/// </summary>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <returns></returns>
		public Unit ParseUnit(XmlNode parentNode, string xpath)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return Unit.Empty;

			try
			{
				return new Unit(xmlnode.InnerText ?? xmlnode.Value);
			}
			catch
			{
				return Unit.Empty;
			}
		}

		/// <summary>
		/// Parse inner text/attribute value of specified xpath to enumeration.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parentNode"></param>
		/// <param name="xpath"></param>
		/// <returns></returns>
		public T ParseEnum<T>(XmlNode parentNode, string xpath)
		{
			XmlNode xmlnode = parentNode.SelectSingleNode(xpath, this.NamespaceManager);
			if (xmlnode == null) return default(T);

			string enumValue = xmlnode.InnerText ?? xmlnode.Value;
			if (string.IsNullOrEmpty(enumValue)) return default(T);

			try
			{
				return (T)Enum.Parse(typeof(T), enumValue, true);
			}
			catch
			{
				return default(T);
			}
		}
	}
}
