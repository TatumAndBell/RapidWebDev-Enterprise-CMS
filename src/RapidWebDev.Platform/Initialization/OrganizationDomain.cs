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

using System.Collections.Generic;
using System.Runtime.Serialization;
using RapidWebDev.Common;
using RapidWebDev.Common.Globalization;

namespace RapidWebDev.Platform.Initialization
{
	/// <summary>
	/// The platform supported organization domain configuration.
	/// </summary>
	public class OrganizationDomain
	{
		private string text;

		/// <summary>
		/// Sets/gets domain unique value
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Sets/gets domain display text.
		/// </summary>
		public string Text
		{
			get { return GlobalizationUtility.ReplaceGlobalizationVariables(this.text); }
			set { this.text = value; }
		}

		/// <summary>
		/// Returns organization domain Value instead.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.Value;
		}

		/// <summary>
		/// Determines whether the specified OrganizationDomain is equal to the current OrganizationDomain.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			OrganizationDomain domain = obj as OrganizationDomain;
			if (domain == null) return false;

			return this.GetHashCode() == domain.GetHashCode();
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.Value == null ? 0 : this.Value.GetHashCode();
		}
	}
}

