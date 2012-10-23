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
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.Platform
{
	/// <summary>
	/// The permission instance parsed from string-typed permission value. 
	/// It provides the method "Contains" to check whether a permission is included/implied in another.
	/// </summary>
	[Serializable]
    [DataContract(Namespace = ServiceNamespaces.Platform)]
	public class PermissionObject : IEquatable<PermissionObject>
	{
        
		private string[] segments;

		/// <summary>
		/// The explicit permission value which associated with users and roles.
		/// </summary>
        [DataMember]
		public string PermissionValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
		public string[] Segments
		{
			get
			{
				if (string.IsNullOrEmpty(this.PermissionValue))
					return new string[0];

				if (this.segments == null)
					this.segments = PermissionValue.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

				return this.segments;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public PermissionObject()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="permissionValue"></param>
		public PermissionObject(string permissionValue)
		{
			this.PermissionValue = permissionValue;
		}

		/// <summary>
		/// Get true if the permission value equals to EveryOne.
		/// </summary>
        [DataMember]
        public bool IsEveryOne
        {
            get { return string.Equals(this.PermissionValue, "EveryOne", StringComparison.OrdinalIgnoreCase); }
            set { }
        }

		/// <summary>
		/// Get true if the permission value equals to Anonymous.
		/// </summary>
        [DataMember]
        public bool IsAnonymous
		{
			get { return string.Equals(this.PermissionValue, "Anonymous", StringComparison.OrdinalIgnoreCase); }
            set { }
		}

		/// <summary>
		/// Returns true when this permission covers the other.<br/>
		/// The check algorithm is,<br/>
		/// 1) If other equals to "EveryOne" or "Anonymous", returns true. <br/>
		/// 2) Otherwise, if current permission value equals to "EveryOne" or "Anonymous", returns false. <br/>
		/// 3) Then the method compares each segment of "this" and "other" PermissionObject. 
		/// The comparing algorithm is implied in the following example. 
		/// "XXX.All" contains all permission values started with "XXX." and "XXX" itself, e.g. "XXX.Update", "XXX.Delete", "XXX.View".
		/// "XXX.YYY" contains all permission values started with "XXX.YYY" and ended with ".View", e.g. "XXX.YYY.View", "XXX.YYY.ZZZ.View".
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Contains(string other)
		{
			return this.Contains(new PermissionObject(other));
		}

		/// <summary>
		/// Returns true when this permission covers the other.<br/>
		/// The check algorithm is,<br/>
		/// 1) If other equals to "EveryOne" or "Anonymous", returns true. <br/>
		/// 2) Otherwise, if current permission value equals to "EveryOne" or "Anonymous", returns false. <br/>
		/// 3) Then the method compares each permission segment of "this" and "other" PermissionObject. 
		/// The comparing algorithm is implied in the following example. 
		/// "XXX.All" contains all permission values started with "XXX." and "XXX" itself, e.g. "XXX.Update", "XXX.Delete", "XXX.View".
		/// "XXX.YYY" contains all permission values started with "XXX.YYY" and ended with ".View", e.g. "XXX.YYY.View", "XXX.YYY.ZZZ.View".
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Contains(PermissionObject other)
		{
			if (other == null) return false;
			if (string.IsNullOrEmpty(other.PermissionValue)) return false;
			if (string.Equals(other.PermissionValue, "EveryOne", StringComparison.OrdinalIgnoreCase)) return true;
			if (string.Equals(other.PermissionValue, "Anonymous", StringComparison.OrdinalIgnoreCase)) return true;

			if (string.IsNullOrEmpty(this.PermissionValue)) return false;
			if (string.Equals(this.PermissionValue, "EveryOne", StringComparison.OrdinalIgnoreCase)) return false;
			if (string.Equals(this.PermissionValue, "Anonymous", StringComparison.OrdinalIgnoreCase)) return false;

			int thisSegmentIndex = 0;
			while (thisSegmentIndex < this.Segments.Length)
			{
				string thisSegment = this.Segments[thisSegmentIndex];
				if (string.Equals(thisSegment, "All", StringComparison.OrdinalIgnoreCase))
					return true;

				string otherSegment = null;
				if (other.Segments.Length > thisSegmentIndex)
					otherSegment = other.Segments[thisSegmentIndex];

				if (otherSegment == null)
					return false;
				else if (!string.Equals(thisSegment, otherSegment, StringComparison.OrdinalIgnoreCase))
					return false;

				thisSegmentIndex++;
			}

			if (thisSegmentIndex < other.Segments.Length)
			{
				if (string.Equals(other.Segments.Last(), "View", StringComparison.OrdinalIgnoreCase))
					return true;
				else
					return false;
			}

			return true;
		}

		/// <summary>
		/// Returns explicit permission value which associated with users and roles.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.PermissionValue;
		}

		/// <summary>
		/// Returns true if this instance equals to target object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;

			return this.GetHashCode() == obj.GetHashCode();
		}

		/// <summary>
		/// Get hash code of this instance.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return string.IsNullOrEmpty(this.PermissionValue) ? base.GetHashCode() : this.PermissionValue.GetHashCode();
		}

		/// <summary>
		/// Convert PermissionObject to string implicitly.
		/// </summary>
		/// <param name="permissionObject"></param>
		/// <returns></returns>
		public static implicit operator string(PermissionObject permissionObject)
		{
			return permissionObject.PermissionValue;
		}

		/// <summary>
		/// Convert string to PermissionObject implicitly.
		/// </summary>
		/// <param name="permissionValue"></param>
		/// <returns></returns>
		public static implicit operator PermissionObject(string permissionValue)
		{
			return new PermissionObject(permissionValue);
		}

		#region IEquatable<PermissionObject> Members

		bool IEquatable<PermissionObject>.Equals(PermissionObject other)
		{
			if (other == null) return false;
			return string.Equals(this.PermissionValue, other.PermissionValue, StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}

	internal static class PermissionObjectEnumerableExtensions
	{
		const string DeniedPermissionValue = ".Denied";
		const string AllPermissionValue = ".All";

		/// <summary>
		/// Filter the denied permissions in the enumerable permissions.
		/// </summary>
		/// <param name="permissions"></param>
		/// <returns></returns>
		public static IEnumerable<PermissionObject> FilterDeniedPemissions(this IEnumerable<PermissionObject> permissions)
		{
			IEnumerable<PermissionObject> deniedPermissionObjects = from p in permissions
																	where p.PermissionValue != null && p.PermissionValue.EndsWith(DeniedPermissionValue, StringComparison.OrdinalIgnoreCase)
																	let deniedPermissionValue = p.PermissionValue.Substring(0, p.PermissionValue.Length - DeniedPermissionValue.Length)
																	select new PermissionObject(string.Concat(deniedPermissionValue, AllPermissionValue));

			if (deniedPermissionObjects.Count() == 0) return permissions;

			IEnumerable<PermissionObject> targetPermissionObjects = from p in permissions
																	where p.PermissionValue == null || !p.PermissionValue.EndsWith(DeniedPermissionValue, StringComparison.OrdinalIgnoreCase)
																	select p;

			List<PermissionObject> returnValue = new List<PermissionObject>();
			foreach (PermissionObject deniedPermissionObject in deniedPermissionObjects)
			{
				foreach (PermissionObject targetPermissionObject in targetPermissionObjects)
				{
					if (targetPermissionObject.IsEveryOne || targetPermissionObject.IsAnonymous)
						returnValue.Add(targetPermissionObject);

					if(!deniedPermissionObject.Contains(targetPermissionObject))
						returnValue.Add(targetPermissionObject);
				}
			}

			returnValue.AddRange(deniedPermissionObjects.Select(p => (PermissionObject)(string.Concat(p.PermissionValue.Substring(0, p.PermissionValue.Length - AllPermissionValue.Length), DeniedPermissionValue))));
			return returnValue;
		}

		/// <summary>
		/// Merge permissions of the user with its roles. The both positive and denied permissions of the user override the conflicting permissions from the roles.
		/// </summary>
		/// <param name="userPermissions"></param>
		/// <param name="rolePermissions"></param>
		/// <returns></returns>
		public static IEnumerable<PermissionObject> MergeUserAndRolePermissions(this IEnumerable<PermissionObject> userPermissions, IEnumerable<PermissionObject> rolePermissions)
		{
			IEnumerable<PermissionObject> targetRolePermissions = from rolePermission in rolePermissions
																	where rolePermission.PermissionValue == null || !rolePermission.PermissionValue.EndsWith(DeniedPermissionValue, StringComparison.OrdinalIgnoreCase)
																	select rolePermission;

			IEnumerable<PermissionObject> targetUserPermissions = from userPermission in userPermissions
																		where userPermission.PermissionValue == null || !userPermission.PermissionValue.EndsWith(DeniedPermissionValue, StringComparison.OrdinalIgnoreCase)
																		select userPermission;

			IEnumerable<PermissionObject> targetPositivePermissions = targetUserPermissions.Union(targetRolePermissions);

			IEnumerable<PermissionObject> deniedUserPermissionObjects = from userPermission in userPermissions
																	where userPermission.PermissionValue != null && userPermission.PermissionValue.EndsWith(DeniedPermissionValue, StringComparison.OrdinalIgnoreCase)
																	let deniedPermissionValue = userPermission.PermissionValue.Substring(0, userPermission.PermissionValue.Length - DeniedPermissionValue.Length)
																	select new PermissionObject(string.Concat(deniedPermissionValue, AllPermissionValue));

			if (deniedUserPermissionObjects.Count() == 0)
				return targetPositivePermissions;

			List<PermissionObject> returnValue = new List<PermissionObject>();
			foreach (PermissionObject deniedUserPermissionObject in deniedUserPermissionObjects)
			{
				foreach (PermissionObject targetPositivePermission in targetPositivePermissions)
				{
					if (targetPositivePermission.IsEveryOne || targetPositivePermission.IsAnonymous)
						returnValue.Add(targetPositivePermission);

					if (!deniedUserPermissionObject.Contains(targetPositivePermission))
						returnValue.Add(targetPositivePermission);
				}
			}

			returnValue.AddRange(deniedUserPermissionObjects.Select(p => (PermissionObject)(string.Concat(p.PermissionValue.Substring(0, p.PermissionValue.Length - AllPermissionValue.Length), DeniedPermissionValue))));
			return returnValue;
		}
	}
}

