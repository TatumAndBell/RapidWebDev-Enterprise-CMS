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
using System.Web.Security;
using RapidWebDev.ExtensionModel;
using System.Runtime.Serialization;
using RapidWebDev.Common;

namespace RapidWebDev.Platform
{
	/// <summary>
	///  Information element of user object.
	/// </summary>
	[Serializable]
    [DataContract(Namespace = ServiceNamespaces.Platform)]
	public class UserObject : AbstractExtensionBizObject, ICloneable
	{
		/// <summary>
		/// Construct user business object.
		/// </summary>
		public UserObject()
		{
		}

		/// <summary>
		/// Construct user business object.
		/// </summary>
		/// <param name="membershipUser"></param>
		internal UserObject(MembershipUser membershipUser) : this()
		{
			this.UserId = (Guid)membershipUser.ProviderUserKey;
			this.UserName = membershipUser.UserName;
			this.Email = membershipUser.Email;
			this.Comment = membershipUser.Comment;
			this.IsApproved = membershipUser.IsApproved;
			this.IsLockedOut = membershipUser.IsLockedOut;
			this.IsOnline = membershipUser.IsOnline;
			this.CreationDate = membershipUser.CreationDate;
			this.LastActivityDate = membershipUser.LastActivityDate;
			this.LastLockoutDate = membershipUser.LastLockoutDate;
			this.LastLoginDate = membershipUser.LastLoginDate;
			this.LastPasswordChangedDate = membershipUser.LastPasswordChangedDate;
			this.PasswordQuestion = membershipUser.PasswordQuestion;
		}

		/// <summary>
		/// Gets application id.
		/// </summary>
        [DataMember]
		public Guid ApplicationId { get; set; }

		/// <summary>
		/// Sets/gets user id.
		/// </summary>
        [DataMember]
        public Guid UserId { get; set; }

		/// <summary>
		/// Sets/gets the organization id which the user belongs to.
		/// </summary>
        [DataMember]
        public Guid OrganizationId { get; set; }

		/// <summary>
		/// Sets/gets the logon name of the membership user.
		/// </summary>
        [DataMember]
        public string UserName { get; set; }

		/// <summary>
		/// Sets/gets the display name of the membership user.
		/// </summary>
        [DataMember]
        public string DisplayName { get; set; }

		/// <summary>
		/// Sets/gets or sets the e-mail address for the membership user.
		/// </summary>
        [DataMember]
        public string Email { get; set; }

		/// <summary>
		/// Sets/gets the password question for the membership user.
		/// </summary>
        [DataMember]
        public string PasswordQuestion { get; set; }

		/// <summary>
		/// Sets/gets or sets the mobile pin for the membership user.
		/// </summary>
        [DataMember]
        public string MobilePin { get; set; }

		/// <summary>
		/// Sets/gets or sets application-specific information for the membership user.
		/// </summary>
        [DataMember]
        public string Comment { get; set; }
		
		/// <summary>
		/// Sets/gets or sets whether the membership user can be authenticated.
		/// </summary>
        [DataMember]
        public bool IsApproved { get; set; }
		
		/// <summary>
		/// Gets a value indicating whether the membership user is locked out and unable to be validated.
		/// </summary>
        [DataMember]
        public bool IsLockedOut { get;  set; }
		
		/// <summary>
		/// Gets whether the user is currently online.
		/// </summary>
        [DataMember]
        public bool IsOnline { get;  set; }

		/// <summary>
		/// Gets the date and time when the user was added to the membership data store.
		/// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }

		/// <summary>
		/// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
		/// </summary>
        [DataMember]
        public DateTime LastActivityDate { get; set; }
		
		/// <summary>
		/// Gets the most recent date and time that the membership user was locked out.
		/// </summary>
        [DataMember]
        public DateTime LastLockoutDate { get;  set; }
		
		/// <summary>
		/// Gets or sets the date and time when the user was last authenticated.
		/// </summary>
        [DataMember]
        public DateTime LastLoginDate { get;  set; }
		
		/// <summary>
		/// Gets the date and time when the membership user's password was last updated.
		/// </summary>
        [DataMember]
        public DateTime LastPasswordChangedDate { get;  set; }

		/// <summary>
		/// Gets user last updated datetime.
		/// </summary>
        [DataMember]
        public DateTime LastUpdatedDate { get;  set; }

		/// <summary>
		/// Returns display identifier of current user as "DisplayName".
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.DisplayName;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (this.UserId == default(Guid))
				return base.GetHashCode();
			else
				return this.UserId.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			UserObject userObject = obj as UserObject;
			if (userObject == null) return false;

			return this.GetHashCode() == userObject.GetHashCode();
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion

		/// <summary>
		/// Get the copy of current object.
		/// </summary>
		/// <returns></returns>
		public UserObject Clone()
		{
			UserObject copy = new UserObject
			{
				UserId = this.UserId,
				UserName = this.UserName,
				ApplicationId = this.ApplicationId,
				Comment = this.Comment,
				DisplayName = this.DisplayName,
				OrganizationId = this.OrganizationId,
				Email = this.Email,
				IsApproved = this.IsApproved,
				CreationDate = this.CreationDate,
				IsLockedOut = this.IsLockedOut,
				IsOnline = this.IsOnline,
				LastActivityDate = this.LastActivityDate,
				LastLockoutDate = this.LastLockoutDate,
				LastLoginDate = this.LastLoginDate,
				LastPasswordChangedDate = this.LastPasswordChangedDate,
				MobilePin = this.MobilePin,
				PasswordQuestion = this.PasswordQuestion,
				LastUpdatedDate = this.LastUpdatedDate,
				ExtensionDataTypeId = this.ExtensionDataTypeId
			};

			this.ClonePropertiesTo(copy);
			return copy;
		}
	}
}

