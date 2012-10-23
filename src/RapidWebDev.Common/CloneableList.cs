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
using System.Linq;
using System.Text;

namespace RapidWebDev.Common
{
	/// <summary>
	/// Cloneable list which supports to get a deep copy with all cloned contained objects.
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	[Serializable]
	public class CloneableList<TValue> : List<TValue>, ICloneable
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CloneableList() : base ()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="inputObjects"></param>
		public CloneableList(IEnumerable<TValue> inputObjects) : base(inputObjects)
		{
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Get a deep copy of current dictionary instance.
		/// </summary>
		/// <returns></returns>
		public CloneableList<TValue> Clone()
		{
			CloneableList<TValue> copy = new CloneableList<TValue>();
			foreach (TValue value in this)
			{
				TValue newValue = value;
				ICloneable cloneableValue = newValue as ICloneable;
				if (cloneableValue != null)
					newValue = (TValue)cloneableValue.Clone();

				copy.Add(newValue);
			}

			return copy;
		}
	}
}

