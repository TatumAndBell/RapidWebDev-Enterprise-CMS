<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HierarchySelectorTests.aspx.cs" Inherits="RapidWebDev.Web.Samples.HierarchySelectorTests" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/resources/css/ext-all.css" />
    <link rel="stylesheet" type="text/css" href="/resources/css/global.css" />
    <script type="text/javascript" src="/resources/javascript/jquery.js"></script>
    <script type="text/javascript" src="/resources/javascript/ext-jquery-adapter.js"></script>
    <script type="text/javascript" src="/resources/javascript/ext-all.js"></script>
    <script type="text/javascript" src="/resources/javascript/hierarchyselector.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="hierarchySelectorContainer"></div>
		<script type="text/javascript">
			var dataSource =
			{
				Url: "/services/HierarchyService.svc/json/GetAllHierarchyData/Area",
				TextField: "Name",
				ValueField: "Id",
				ParentValueField: "ParentHierarchyDataId"
			};

			var globalization =
			{
				LoadRemoteDataFailed: "Failed to load remote data.",
				Loading: "Loading",
				Title: "Hierarchy Selector"
			};

			//var selector = new HierarchySelector("hierarchySelector", "xxx", "hierarchySelectorContainer", dataSource, globalization, { Cascading: "full" });

			window.QueryPanel_HierarchyDataManagement__QueryFieldControl2_Variable = new HierarchySelector('hierarchySelector', 'hierarchySelector', 'hierarchySelectorContainer', { "Url": "/services/HierarchyService.svc/json/GetAllHierarchyData/Area", "TextField": "Name", "ValueField": "Id", "ParentValueField": "ParentHierarchyDataId" }, { "LoadRemoteDataFailed": "Load remote data failed.", "Loading": "Loading, please wait for a while.", "Title": "Area Selector" }, { "Width": 154, "Enabled": true, "ModalDialogWidth": 480, "ModalDialogHeight": 320, "Cascading": "full" });

			function onbuttonclick()
			{
				alert(QueryPanel_HierarchyDataManagement__QueryFieldControl2_Variable.getValue());
			}
		</script>
		
		<br /><br />
		<input type="button" onclick="onbuttonclick()" value="show" />
    </form>
</body>
</html>
