<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileUploadTests.aspx.cs" Inherits="RapidWebDev.Web.Samples.FileUploadTests" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/resources/css/ext-all.css" />
    <link rel="stylesheet" type="text/css" href="/resources/css/global.css" />
    <link rel="stylesheet" type="text/css" href="/resources/css/uploadify.css" />
    <script type="text/javascript" src="/resources/javascript/jquery.js"></script>
    <script type="text/javascript" src="/resources/javascript/ext-jquery-adapter.js"></script>
    <script type="text/javascript" src="/resources/javascript/ext-all.js"></script>
    <script type="text/javascript" src="/resources/javascript/uploadify/swfobject.js"></script>
    <script type="text/javascript" src="/resources/javascript/uploadify/jquery.uploadify.v2.1.0.min.js"></script>
    <script type="text/javascript" src="/resources/javascript/filemanagementcontrol.js"></script>
    
    <style type="text/css">
    	.filemanagementcontrol { margin: 2px; }
    	
		.filemanagementcontrol ul.filelist li
		{
			background-image: url('/resources/images/dot.png');
			background-repeat: no-repeat;
			background-position: 0 8px;
			padding-left: 9px;
			margin-bottom: 4px;
			line-height: 18px;
		}
    </style>
</head>
<body>
    <form id="form1" runat="server">
		<div id="container" style="margin:12px; border:1px solid gray">
			
		</div>
		<script type="text/javascript">
			var controlId = "uploadsample";
			var uniqueId = "uploadsample";
			var containerId = "container";
			var uploadFileControlId = "fileupload";

			var settings = { MultipleUpload: true, Uploadable: true, Deletable: true, MaximumFileCount: 5, FileCategory: "ProductAttachment", EnableFileRemoveConfirmation: true };
			var resources =
			{
				FlashUploaderUri: "/resources/flash/uploadify/uploadify.swf",
				ProcessingScriptUri: "FileUploadService.svc",
				CancelImageUri: "/resources/images/cancel.png",
				FileRemoveImageUri: "/resources/images/delete.gif",
				FileRemoveConfirmation: "Are you sure to remove this file?",
				ShowUploadDialogButtonText: "Upload More",
				UploadButtonText: "Upload",
				UploadCancelButtonText: "Cancel",
				FileUploadDialogTitle: "Upload Product Attachment",
				FileUploadToolTip: "Click the button below to select file locally."
			};
			
			var dataSource = [
				{ Id: "1", FileName: "jquery.uploadify.js", IconUri: "/resources/images/doc.gif", Size: 1056 },
				{ Id: "2", FileName: "fileuploadtests.\"a\"spx", IconUri: "/resources/images/doc.gif", Size: 2938 },
				{ Id: "3", FileName: "我的 Adobe Captivate 项'目.doc", IconUri: "/resources/images/doc.gif", Size: 13}];

			var control = new FileManagementControl(controlId, uniqueId, containerId, settings, resources, dataSource)

			function onshowbuttonclick()
			{
				//$("#fileupload").uploadifyUpload();
				control.show();
			}
			
			function onaddbuttonclick()
			{
				control.addFile({ FileName: "我的 Adobe Captivate 项'目2.doc", IconUri: "/resources/images/doc.gif", Size: 1323 });
			}

			function onremovebuttonclick()
			{
				control.removeFile(2);
			}
		</script>
		
		<br /><br />
		<input type="button" onclick="onshowbuttonclick()" value="show" />
		<input type="button" onclick="onaddbuttonclick()" value="add" />
		<input type="button" onclick="onremovebuttonclick()" value="remove" />
    </form>
</body>
</html>
