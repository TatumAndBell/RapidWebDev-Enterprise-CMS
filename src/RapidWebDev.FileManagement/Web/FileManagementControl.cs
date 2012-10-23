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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using RapidWebDev.Common;
using RapidWebDev.Common.Web;
using RapidWebDev.FileManagement.Properties;
using RapidWebDev.UI;

namespace RapidWebDev.FileManagement.Web
{
	/// <summary>
	/// FileManagement Control
	/// </summary>
	[ToolboxData("<{0}:FileManagementControl runat=\"server\"/>")]
	public class FileManagementControl : WebControl, INamingContainer, ITextControl, IPostBackDataHandler
	{
		private readonly static JavaScriptSerializer serializer = new JavaScriptSerializer();
		private static IFileManagementApi fileManagementApi = SpringContext.Current.GetObject<IFileManagementApi>();
		private static IFileBindingApi fileBindingApi = SpringContext.Current.GetObject<IFileBindingApi>();
		private static IPermissionBridge permissionBridge = SpringContext.Current.GetObject<IPermissionBridge>();
		private IEnumerable<FileWebObject> associatedFileObjects;

		#region ITextControl Members

		string ITextControl.Text
		{
			get { return "FileManagementControl"; }
			set { }
		}

		#endregion

		/// <summary>
		/// True when the control is allowed to upload new files, defaults to True.
		/// </summary>
		public bool Uploadable { get; set; }

		/// <summary>
		/// True when the control is allowed to delete existed files, defaults to True.
		/// </summary>
		public bool Deletable { get; set; }

		/// <summary>
		/// True when the control is readonly which cannot either upload new files or delete existed files, defaults to False.
		/// </summary>
		public bool ReadOnly { get; set; }

		/// <summary>
		/// True when the control supports to upload multiple files, defaults to True.
		/// </summary>
		public bool MultipleUpload { get; set; }

		/// <summary>
		/// Maximum files inner of the file management control, defaults to 10.
		/// The property doesn't work if the property MultipleUpload equals to False.
		/// </summary>
		public int MaximumFileCount { get; set; }

		/// <summary>
		/// Category of the managed files by the control, defaults to null.
		/// The control checks whether the user has permission "FileManagement.{Category}.Upload".
		/// </summary>
		public string FileCategory { get; set; }

		/// <summary>
		/// Whether to show confirmation dialog for file removal, defaults to False.
		/// </summary>
		public bool EnableFileRemoveConfirmation { get; set; }

		/// <summary>
		/// Title of file uploading dialog, defaults to the globalized message "Resources.FileUploadDialogTitle".
		/// </summary>
		public string FileUploadDialogTitle { get; set; }

		/// <summary>
		/// The relationship type between the external object and managing files.
		/// </summary>
		public string RelationshipType { get; set; }

		/// <summary>
		/// Sets/gets the external object id which the managing files are associated with.
		/// </summary>
		public Guid ExternalObjectId { get; set; }

		/// <summary>
		/// Gets count of associated files to the external object.
		/// </summary>
		public int AssociatedFileCount
		{
			get 
			{
				if (this.associatedFileObjects == null)
				{
					if (this.ExternalObjectId == Guid.Empty) return 0;
					this.associatedFileObjects = fileBindingApi.FindBoundFiles(this.ExternalObjectId, this.RelationshipType).Select(f => new FileWebObject(f.Id, f.FileName, f.BytesCount));
				}

				return this.associatedFileObjects.Count();
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public FileManagementControl()
		{
			this.Uploadable = true;
			this.Deletable = true;
			this.MultipleUpload = true;
			this.MaximumFileCount = 10;
		}

		#region IPostBackDataHandler Members

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			string postValue = postCollection[postDataKey];
			if (string.IsNullOrEmpty(postValue))
				this.associatedFileObjects = new List<FileWebObject>();
			else
				this.associatedFileObjects = serializer.Deserialize<List<FileWebObject>>(postValue);

			return true;
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
		}

		#endregion

		/// <summary>
		/// Get change list of managing files. The method should be called before saving.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<FileChangeTypes, FileWebObject>> GetChangeList()
		{
			if (this.ExternalObjectId == Guid.Empty)
				throw new InvalidProgramException(Resources.InvalidExternalObjectId);

			if (this.associatedFileObjects == null)
				return new List<KeyValuePair<FileChangeTypes, FileWebObject>>();

			IEnumerable<FileWebObject> originalAssociatedFileObjects = fileBindingApi.FindBoundFiles(this.ExternalObjectId, this.RelationshipType).Select(f => new FileWebObject(f.Id, f.FileName, f.BytesCount));
			IEnumerable<FileWebObject> addedFileObjects = this.associatedFileObjects.Except(originalAssociatedFileObjects);
			IEnumerable<FileWebObject> removedFileObjects = originalAssociatedFileObjects.Except(this.associatedFileObjects);

			List<KeyValuePair<FileChangeTypes, FileWebObject>> results = new List<KeyValuePair<FileChangeTypes, FileWebObject>>();
			foreach (FileWebObject addedFileObject in addedFileObjects)
				results.Add(new KeyValuePair<FileChangeTypes, FileWebObject>(FileChangeTypes.Added, addedFileObject));

			foreach (FileWebObject removedFileObject in removedFileObjects)
				results.Add(new KeyValuePair<FileChangeTypes, FileWebObject>(FileChangeTypes.Removed, removedFileObject));

			return results;
		}

		/// <summary>
		/// Persist the managing files with the external object.
		/// </summary>
		public void Save()
		{
			if (this.ExternalObjectId == Guid.Empty)
				throw new InvalidProgramException(Resources.InvalidExternalObjectId);

			if (this.associatedFileObjects != null)
			{
				fileBindingApi.Unbind(this.ExternalObjectId, this.RelationshipType);
				fileBindingApi.Bind(this.RelationshipType, this.ExternalObjectId, this.associatedFileObjects.Select(f => f.Id));
			}
		}

		/// <summary>
		/// Raises the System.Web.UI.Control.PreRender event.
		/// </summary>
		/// <param name="e">An System.EventArgs object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (string.IsNullOrEmpty(this.FileCategory))
				throw new InvalidProgramException(Resources.FileCategoryCannotBeEmpty);

			const string JavaScriptDeclarationTemplate = @"window.{ControlId}_Variable = new FileManagementControl(""{ControlId}"", ""{UniqueId}"", ""{ControlId}_Container"", {Settings}, {Resources}, {DataSource});";
			string javaScript = JavaScriptDeclarationTemplate.Replace("{ControlId}", this.ClientID)
				.Replace("{UniqueId}", this.UniqueID)
				.Replace("{Settings}", this.BuildSettingsJson())
				.Replace("{Resources}", this.BuildResourcesJson())
				.Replace("{DataSource}", this.BuildDataSourceJson());

			ClientScripts.RegisterScriptBlock(javaScript);
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The System.Web.UI.HtmlTextWriter object that receives the control content.</param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render(writer);
			writer.Write(@"<div id=""{0}_Container""></div>", this.ClientID);
		}

		private string BuildSettingsJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				string fileCategory = !string.IsNullOrEmpty(this.FileCategory) ? this.FileCategory : "NULL";
				string fileUploadPermission = string.Format(CultureInfo.InvariantCulture, "FileManagement.{0}.Upload", fileCategory);
				bool uploadable = permissionBridge.HasPermission(fileUploadPermission) && this.Uploadable;
				jsonWriter.WritePropertyName("Uploadable");
				jsonWriter.WriteValue(uploadable);

				string fileDeletePermission = string.Format(CultureInfo.InvariantCulture, "FileManagement.{0}.Delete", fileCategory);
				bool deletable = permissionBridge.HasPermission(fileDeletePermission) && this.Deletable;
				jsonWriter.WritePropertyName("Deletable");
				jsonWriter.WriteValue(deletable);

				jsonWriter.WritePropertyName("ReadOnly");
				jsonWriter.WriteValue(this.ReadOnly);

				jsonWriter.WritePropertyName("MultipleUpload");
				jsonWriter.WriteValue(this.MultipleUpload);

				jsonWriter.WritePropertyName("MaximumFileCount");
				jsonWriter.WriteValue(this.MaximumFileCount);

				jsonWriter.WritePropertyName("FileCategory");
				jsonWriter.WriteValue(fileCategory);

				jsonWriter.WritePropertyName("EnableFileRemoveConfirmation");
				jsonWriter.WriteValue(this.EnableFileRemoveConfirmation);

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		private string BuildResourcesJson()
		{
			StringBuilder jsonBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(jsonBuilder))
			using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
			{
				jsonWriter.WriteStartObject();

				jsonWriter.WritePropertyName("FlashUploaderUri");
				jsonWriter.WriteValue(Kit.ResolveAbsoluteUrl("~/resources/flash/uploadify/uploadify.swf"));

				jsonWriter.WritePropertyName("ProcessingScriptUri");
				jsonWriter.WriteValue(Kit.ResolveAbsoluteUrl("~/FileUploadService.svc"));

				jsonWriter.WritePropertyName("CancelImageUri");
				jsonWriter.WriteValue(Kit.ResolveAbsoluteUrl("~/resources/images/cancel.png"));

				jsonWriter.WritePropertyName("FileRemoveImageUri");
				jsonWriter.WriteValue(Kit.ResolveAbsoluteUrl("~/resources/images/delete.gif"));

				jsonWriter.WritePropertyName("FileRemoveConfirmation");
				jsonWriter.WriteValue(Resources.FileRemoveConfirmationMessage);

				jsonWriter.WritePropertyName("ShowUploadDialogButtonText");
				jsonWriter.WriteValue(Resources.ShowUploadDialogButtonText);

				jsonWriter.WritePropertyName("UploadButtonText");
				jsonWriter.WriteValue(Resources.UploadButtonText);

				jsonWriter.WritePropertyName("UploadCancelButtonText");
				jsonWriter.WriteValue(Resources.UploadCancelButtonText);

				jsonWriter.WritePropertyName("FileUploadDialogTitle");
				jsonWriter.WriteValue(this.FileUploadDialogTitle ?? Resources.FileUploadDialogTitle);

				jsonWriter.WritePropertyName("FileUploadToolTip");
				jsonWriter.WriteValue(Resources.FileUploadToolTip);

				jsonWriter.WriteEndObject();
			}

			return jsonBuilder.ToString();
		}

		private string BuildDataSourceJson()
		{
			if (this.ExternalObjectId == Guid.Empty)
				return "[]";

			this.associatedFileObjects = fileBindingApi.FindBoundFiles(this.ExternalObjectId, this.RelationshipType).Select(f => new FileWebObject(f.Id, f.FileName, f.BytesCount));
			List<FileWebObject> fileWebObjects = this.associatedFileObjects.ToList();
			return serializer.Serialize(fileWebObjects);
		}
	}

	/// <summary>
	/// File change types.
	/// </summary>
	public enum FileChangeTypes
	{
		/// <summary>
		/// Added
		/// </summary>
		Added,

		/// <summary>
		/// Removed
		/// </summary>
		Removed
	}
}