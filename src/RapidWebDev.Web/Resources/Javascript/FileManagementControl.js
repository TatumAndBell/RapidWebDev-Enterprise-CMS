// settings:
// - Uploadable : boolean, True when the control is allowed to upload new files, defaults to True.
// - Deletable : boolean, True when the control is allowed to delete existed files, defaults to True.
// - ReadOnly : boolean, True when the control is readonly which cannot either upload new files or delete existed files, defaults to False.
// - MultipleUpload : boolean, True when the control supports to upload multiple files, defaults to True.
// - MaximumFileCount : integer, Maximum count of files managed in the control.
// - FileCategory: string, the category of files uploaded by the control
// - EnableFileRemoveConfirmation: boolean

// resources:
// - FlashUploaderUri: string
// - ProcessingScriptUri: string
// - CancelImageUri: string
// - FileRemoveImageUri: string
// - FileRemoveConfirmation: string
// - ShowUploadDialogButtonText: string
// - UploadButtonText: string
// - UploadCancelButtonText: string
// - FileUploadDialogTitle: string
// - FileUploadToolTip: string

// dataSource: Array
// - Id: string
// - FileName: string
// - DownloadUri: string
// - IconUri: string
// - Size: int, in KB

function FileManagementControl(controlId, uniqueId, containerId, settings, resources, dataSource)
{
	var hiddenControlId = controlId + "_hidden";
	var fileListContainerId = controlId + "_fliecontainer";
	var uploadFileControlId = controlId + "_fileupload";
	var buttonsContainerid = controlId + "_buttons_container";
	var cacheDataSource = (dataSource == undefined || dataSource == null) ? new Array() : dataSource;
	var fileUploadDialog;

	function initializeFileList()
	{
		var html = "<div class='filemanagementcontrol'>";
		html += "<input type='hidden' id='" + hiddenControlId+ "' name='"+uniqueId+"' />";
		html += "<ul id='" + fileListContainerId + "' class='filelist'>"
		var fileRemoveButtonIdArray = new Array();
		for (var i = 0; i < cacheDataSource.length; i++)
		{
			var fileObject = cacheDataSource[i];
			var removeButtonHtml = "";
			if (settings.Deletable && !settings.ReadOnly)
			{
				var fileRemoveButtonId = "fileRemoveButton_" + fileObject.Id;
				removeButtonHtml = "| <span id='" + fileRemoveButtonId + "' style='cursor: pointer'><img src='" + resources.FileRemoveImageUri + "' fileid='" + fileObject.Id + "' /></span>";
				fileRemoveButtonIdArray.push(fileRemoveButtonId);
			}

			html += "<li fileid='" + fileObject.Id + "'><a href='" + fileObject.DownloadUri + "'>" + fileObject.FileName + " (" + fileObject.Size + " KB) <span><img src='" + fileObject.IconUri + "' /></span></a> " + removeButtonHtml + "</li>";
		}

		html += "</ul>";
		if (settings.Uploadable)
			html += "<div id='" + buttonsContainerid + "' class='buttons'><div>";

		html += "</div>";
		$("#" + containerId).html(html);

		if (settings.Uploadable && !settings.ReadOnly)
		{
			var uploadButton = new Ext.Button(
			{
				text: resources.ShowUploadDialogButtonText,
				listeners: { click: function() { showFileUploadDialog(); } }
			});
			
			uploadButton.render(buttonsContainerid);
		}

		$("#" + hiddenControlId).val(Ext.encode(cacheDataSource));
		
		// attach "click" event handler to file remove buttons.
		for (var i = 0; i < fileRemoveButtonIdArray.length; i++)
		{
			var fileRemoveButtonId = fileRemoveButtonIdArray[i];
			Ext.EventManager.addListener(fileRemoveButtonId, "click", function(evt, t, o)
			{
				if (settings.EnableFileRemoveConfirmation && !window.confirm(resources.FileRemoveConfirmation)) return;
				var fileId = t.getAttribute("fileid");
				removeFile(fileId);
			}, this);
		}
	}

	if (Ext.isReady) initializeFileList();
	else Ext.onReady(function() { initializeFileList(); });

	function addFile(fileObject)
	{
		var fileRemoveButtonId = null;
		if (settings.Deletable && !settings.ReadOnly)
		{
			fileRemoveButtonId = "fileRemoveButton_" + fileObject.Id;
			removeButtonHtml = "| <span id='" + fileRemoveButtonId + "' style='cursor: pointer'><img src='" + resources.FileRemoveImageUri + "' fileid='" + fileObject.Id + "' /></span>";
		}

		var html = "<li fileid='" + fileObject.Id + "'><a href='" + fileObject.DownloadUri + "'>" + fileObject.FileName + " (" + fileObject.Size + " KB) <span><img src='" + fileObject.IconUri + "' /></span></a> " + removeButtonHtml + "</li>";
		$("#" + fileListContainerId).append(html);
		cacheDataSource.push(fileObject);
		$("#" + hiddenControlId).val(Ext.encode(cacheDataSource));

		if (fileRemoveButtonId != null)
		{
			Ext.EventManager.addListener(fileRemoveButtonId, "click", function(evt, t, o)
			{
				if (settings.EnableFileRemoveConfirmation && !window.confirm(resources.FileRemoveConfirmation))
				var fileId = t.getAttribute("fileid");
				removeFile(fileId);
			}, this);
		}
	}

	function removeFile(fileId)
	{
		var selector = "#" + fileListContainerId + " li[fileid=" + fileId + "]";
		var results = $(selector);
		if (results.length == 0) return;

		results.remove();
		var newCacheDataSource = new Array();
		for (var i = 0; i < cacheDataSource.length; i++)
		{
			var fileObject = cacheDataSource[i];
			if (fileObject.Id != fileId)
				newCacheDataSource.push(fileObject);
		}

		cacheDataSource = newCacheDataSource;
		$("#" + hiddenControlId).val(Ext.encode(cacheDataSource));
	}

	function showFileUploadDialog()
	{
		if (settings.MaximumFileCount <= cacheDataSource.length) return;

		if (fileUploadDialog == null)
		{
			var uploadButtonContainerId = containerId + "_uploadButtonContainerId";
			fileUploadDialog = new Ext.Window(
			{
				draggable: true,
				modal: true,
				resizable: false,
				closeAction: 'hide',
				autoScroll: true,
				plain: false,
				title: resources.FileUploadDialogTitle,
				stateful: false,
				html: "<div style='padding: 4px'><h2 style='margin-bottom:2px; color:#333333'>" + resources.FileUploadToolTip + "</h2><div><input type='file' id='" + uploadFileControlId + "' /></div><hr/><div id='" + uploadButtonContainerId + "'></div></div>"
			});

			fileUploadDialog.render(document.body);
			fileUploadDialog.setWidth(420);
			fileUploadDialog.setHeight(400);

			$("#" + uploadFileControlId).uploadify(
			{
				uploader: resources.FlashUploaderUri,
				script: resources.ProcessingScriptUri,
				cancelImg: resources.CancelImageUri,
				folder: '/',
				fileDataName: 'rapidwebdev.filemanagement',
				multi: settings.MultipleUpload,
				queueSizeLimit: settings.MaximumFileCount - cacheDataSource.length,
				scriptData: { category: (settings.FileCategory == undefined || settings.FileCategory == null) ? "none" : settings.FileCategory },
				onComplete: function(event, queueID, fileObj, response, data)
				{
					var fileObject = Ext.decode(response);
					addFile(fileObject);
				},
				onAllComplete: function(event, data)
				{
					if (fileUploadDialog != null)
						fileUploadDialog.hide();
				}
			});

			var uploadButton = new Ext.Button(
			{
				text: resources.UploadButtonText,
				renderTo: uploadButtonContainerId,
				style: "display:inline",
				listeners:
				{
					click: function()
					{
						$("#" + uploadFileControlId).uploadifyUpload();
					}
				}
			});

			new Ext.form.Label({ text: " ", renderTo: uploadButtonContainerId });

			var uploadCancelButton = new Ext.Button(
			{
				text: resources.UploadCancelButtonText,
				renderTo: uploadButtonContainerId,
				style: "display:inline",
				listeners:
				{
					click: function()
					{
						if (fileUploadDialog != null) fileUploadDialog.hide();
					}
				}
			});
		}

		fileUploadDialog.center();
		fileUploadDialog.show();
		fileUploadDialog.doLayout();
		$("#" + uploadFileControlId).uploadifySettings("queueSizeLimit", settings.MaximumFileCount - cacheDataSource.length);
	}
}