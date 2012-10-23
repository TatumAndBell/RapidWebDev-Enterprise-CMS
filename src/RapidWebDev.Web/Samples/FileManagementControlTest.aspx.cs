using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RapidWebDev.Web.Samples
{
	public partial class FileManagementControlTest : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack && string.IsNullOrEmpty(this.TextBoxExternalObjectId.Text))
				this.TextBoxExternalObjectId.Text = Guid.NewGuid().ToString();

			this.ButtonLoad.Click += new EventHandler(ButtonLoad_Click);
			this.ButtonSave.Click += new EventHandler(ButtonSave_Click);
		}

		void ButtonLoad_Click(object sender, EventArgs e)
		{
			this.ProductAttachmentFileManagementControl.ExternalObjectId = new Guid(this.TextBoxExternalObjectId.Text);
		}

		void ButtonSave_Click(object sender, EventArgs e)
		{
			int associatedFileCount = this.ProductAttachmentFileManagementControl.AssociatedFileCount;
			this.ProductAttachmentFileManagementControl.ExternalObjectId = new Guid(this.TextBoxExternalObjectId.Text);
			this.ProductAttachmentFileManagementControl.Save();
			this.LabelMessage.Text = string.Format("Total {0} files saved successfully.", associatedFileCount);
		}
	}
}