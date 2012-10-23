// selection: Array of objects with the following properties.
// - Id
// - Name

// dataSourceSchema:
// - Url: string, external data source url
// - TextField: string
// - ValueField: string
// - ParentValueField: string

// globalization
// - LoadRemoteDataFailed
// - Loading
// - Title

// options:
// - Width: integer, width of button displayed in UI, defaults to 154
// - Enabled: boolean, whether the hierarchy selector is enabled, defaults to true
// - ModalDialogWidth: integer, width of popup hierarchy selector modal dialog, defaults to 480
// - ModalDialogHeight: integer, height of popup hierarchy selector modal dialog, defaults to 320
// - Cascading: enumeration as following, defaults to "full".
//              1) 'full': cascading check/uncheck the parent and children when a node is checked/unchecked
//              2) 'none': no impact to other tree node when a tree node is checked/unchecked
//              3) 'standard': the parent will be checked when a node is checked. The children will be unchecked when the parent node is unchecked.
//                  standard means there allows the parent node been checked without any children checked, but all the ancestors have to be checked when a child is checked.
//              4) 'singlecheck': only one node can be checked at a time.
//			  5) 'child-cascading': status of children are impacted by the parent but the parent is not impacted by the change of children.
// - onNodeCheckChanged: the callback is fired when the hierarchy tree node check-status is changed.
// - onHierarchySelectorDialogClosed: the callback is fired when hierarchy selector dialog is closed.

function HierarchySelector(id, uniqueName, containerId, dataSourceSchema, globalization, options, selection)
{
	var _defaultSelection = (selection == undefined || selection == null) ? new Array() : selection;
	var _cachedSelection = Ext.decode(Ext.encode(_defaultSelection));
	var _hiddenFieldId = id + "__hidden";

	var _hierarchySelectorId = id;
	var _hierarchySelectorQuickTipId = _hierarchySelectorId + "_quicktip";
	var _hierarchySelectorModalDialogId = id + "_modaldialog";
	var _hierarchySelectorTreeId = id + "_modaldialog_tree";
	
	var _displayMode = "ToolTip"
	var _width = options.Width ? options.Width : 154;
	var _enabled = options.Enabled ? options.Enabled : true;
	var _modalDialogWidth = options.ModalDialogWidth ? options.ModalDialogWidth : 480;
	var _modalDialogHeight = options.ModalDialogHeight ? options.ModalDialogHeight : 320;
	var _cascading = options.Cascading != null ? options.Cascading : "full";

	this.getValue = function()
	{
		if (_cachedSelection == null || _cachedSelection.length == 0) return null;
		return Ext.encode(_cachedSelection);
	}

	this.setValue = function(selection)
	{
		_cachedSelection = selection != null ? selection : new Array();
		$("#" + _hiddenFieldId).val(Ext.encode(_cachedSelection));
		
		refreshSelectorText();
	}

	this.reset = function()
	{
		this.setValue(_defaultSelection);
	}

	this.setDisplayMode = function(mode)
	{
		_displayMode = mode;
	}

	function refreshSelectorText()
	{
		var tooltip = "";
		for (var i = 0; i < _cachedSelection.length; i++)
		{
			if (tooltip != "") tooltip += ", ";
			tooltip += _cachedSelection[i].Name;
		}

		if (_displayMode == null || _displayMode.toLowerCase() == "tooltip")
		{
			var buttonText = _cachedSelection.length > 0 ? globalization.Title + " (" + _cachedSelection.length + ")" : globalization.Title;
			Ext.getCmp(_hierarchySelectorId).setText(buttonText);

			if (tooltip != "")
				Ext.getCmp(_hierarchySelectorId).setTooltip({ text: tooltip, dismissDelay: 0 });
			else
				Ext.getCmp(_hierarchySelectorId).setTooltip(null);
		}
		else
		{
			var buttonText = tooltip.length > 0 ? tooltip : globalization.Title;
			Ext.getCmp(_hierarchySelectorId).setText(buttonText);
		}
	}
	
	function isTheNodeValueSelected(id)
	{
		if (id == null) return false;
		for(var i=0; i<_cachedSelection.length; i++)
			if(_cachedSelection[i].Id.toLowerCase() == id.toLowerCase()) return true;
			
		return false;
	}
	
	function addSelectedItem(item)
	{
		_cachedSelection.push(item);
		$("#" + _hiddenFieldId).val(Ext.encode(_cachedSelection));
		refreshSelectorText();
	}
	
	function removeSelectedItem(id)
	{
		if (id == null) return false;
		
		var item = null;
		for(var i=0; i<_cachedSelection.length; i++)
		{
			if(_cachedSelection[i].Id.toLowerCase() == id.toLowerCase()) 
			{
				item = _cachedSelection[i];
				break;
			}
		}
		
		if(item == null) return;
		_cachedSelection.remove(item);
		$("#" + _hiddenFieldId).val(Ext.encode(_cachedSelection));

		refreshSelectorText();
	}

	function initializeUI()
	{
		Ext.QuickTips.init();
		
		var html = '<input id="' + _hiddenFieldId + '" name="' + uniqueName + '" type="hidden" />';
		Ext.DomHelper.insertHtml("beforeBegin", Ext.DomQuery.selectNode("#" + containerId), html);
		
		var buttonUI = new Ext.Button(
		{
			id: _hierarchySelectorId,
			renderTo: containerId,
			width: _width,
			stateful: false,
			disabled: !_enabled,
			text: globalization.Title,
			listeners:
			{
				click: function(button, e)
				{
					showHierarchySelectorModalDialog(globalization.Title);
				}
			}
		});

		refreshSelectorText();
	}

	if (Ext.isReady) initializeUI();
	else Ext.onReady(function() { initializeUI(); });

	function showHierarchySelectorModalDialog(headerText)
	{
		var modalDialog = new Ext.Window(
		{
			id: _hierarchySelectorModalDialogId,
			draggable: true,
			modal: true,
			resizable: false,
			closeAction: 'close',
			autoScroll: false,
			plain: true,
			title: headerText,
			stateful: false,
			items: new Ext.form.Label({ text: globalization.Loading }),
			listeners:
			{
				close: function(p)
				{
					if (options.onHierarchySelectorDialogClosed)
						options.onHierarchySelectorDialogClosed();
				}
			}
		});

		modalDialog.render(document.body);
		modalDialog.setWidth(_modalDialogWidth);
		modalDialog.setHeight(_modalDialogHeight);

		modalDialog.center();
		modalDialog.show();

		loadRemoteData();
	}

	function loadRemoteData()
	{
		Ext.Ajax.request(
		{
			url: dataSourceSchema.Url,
			method: "GET",
			success: onLoadRemoteDataSuccessfully,
			failure: onLoadRemoteDataFailed
		});
	}

	function onLoadRemoteDataSuccessfully(response, options)
	{
		var hierarchyDataArray = Ext.decode(response.responseText);
		var tree = new Ext.tree.TreePanel(
		{
			id: _hierarchySelectorTreeId,
			useArrows: false,
			autoScroll: true,
			animate: false,
			animCollapse: false,
			enableDD: false,
			rootVisible: false,
			frame: false,
			forceLayout: true,
			root: convertHierarchyDataToExtTreeRootNode(hierarchyDataArray),
			listeners:
			{
				checkchange: function(node, checked)
				{
					_onNodeCheckChanged(node, checked);
				}
			}
		});
		
		var modalDialog = Ext.getCmp(_hierarchySelectorModalDialogId);
		modalDialog.removeAll();
		modalDialog.add(tree);
		modalDialog.doLayout();

		tree.show();
		tree.setHeight(modalDialog.getInnerHeight());
	}

	function convertHierarchyDataToExtTreeRootNode(hierarchyDataArray)
	{
		var treeNodes = new Array();
		for (var i = 0; i < hierarchyDataArray.length; i++)
		{
			var hierarchyData = hierarchyDataArray[i];
			var nodeParentValue = "";
			eval("nodeParentValue=hierarchyData." + dataSourceSchema.ParentValueField);

			if (nodeParentValue == null || nodeParentValue == "")
			{
				var nodeText = "";
				var nodeValue = "";
				eval("nodeText=hierarchyData." + dataSourceSchema.TextField);
				eval("nodeValue=hierarchyData." + dataSourceSchema.ValueField);
				var isChecked = isTheNodeValueSelected(nodeValue);
				var treeNode = { checked: isChecked, text: nodeText, id: nodeValue, editable: false, expanded: true };				
				convertHierarchyDataToExtTreeDataSource(treeNode, hierarchyDataArray);

				if (treeNode.children == undefined || treeNode.children == null)
					treeNode.leaf = true;
				else
					treeNode.leaf = false;

				treeNodes.push(treeNode);
			}
		}

		return { children: treeNodes };
	}

	function convertHierarchyDataToExtTreeDataSource(parentTreeNode, hierarchyDataArray)
	{
		for (var i = 0; i < hierarchyDataArray.length; i++)
		{
			var hierarchyData = hierarchyDataArray[i];
			var nodeParentValue = "";
			eval("nodeParentValue=hierarchyData." + dataSourceSchema.ParentValueField);
			if (nodeParentValue != parentTreeNode.id) continue;
			
			if (parentTreeNode.children == undefined || parentTreeNode.children == null)
				parentTreeNode.children = new Array();

			var nodeText = "";
			var nodeValue = "";
			eval("nodeText=hierarchyData." + dataSourceSchema.TextField);
			eval("nodeValue=hierarchyData." + dataSourceSchema.ValueField);

			var isChecked = isTheNodeValueSelected(nodeValue);
			var treeNode = { checked: isChecked, text: nodeText, id: nodeValue, editable: false, expanded: true };
			parentTreeNode.children.push(treeNode);

			convertHierarchyDataToExtTreeDataSource(treeNode, hierarchyDataArray);
			if (treeNode.children == undefined || treeNode.children == null)
				treeNode.leaf = true;
			else
				treeNode.leaf = false;
		}
	}

	function onLoadRemoteDataFailed(response, options)
	{
		var exceptionLabel = new Ext.form.Label({ text: globalization.LoadRemoteDataFailed });
		var modalDialog = Ext.getCmp(_hierarchySelectorModalDialogId);
		modalDialog.removeAll();
		modalDialog.add(exceptionLabel);
		modalDialog.doLayout();
	}

	function _onNodeCheckChanged(node, checked)
	{
		_changeNodeStatus(node, checked);

		if (_cascading == 'full' || _cascading == 'standard')
		{
			_checkChildren(node, checked);
			_checkParent(node, checked);
		}
		else if (_cascading == "singlecheck")
		{
			if (checked)
			{
				var checkedNodes = Ext.getCmp(_hierarchySelectorTreeId).getChecked();
				for (var i = 0; i < checkedNodes.length; i++)
				{
					if (checkedNodes[i].id != node.id)
						_changeNodeStatus(checkedNodes[i], false);
				}
			}
		}
		else if (_cascading == "child-cascading")
		{
			_checkNodes4ChildCascading(node, checked, true);
		}

		if (options.onNodeCheckChanged)
			options.onNodeCheckChanged(checked);
	}

	function _checkNodes4ChildCascading(node, checked, whetherToCascadeChildren)
	{
		if (whetherToCascadeChildren)
			_checkChildren(node, checked);

		if (node.parentNode == null) return;
		if (checked)
		{
			var checkedChildrenNumber = 0;
			node.parentNode.eachChild(function(child)
			{
				if (child.ui.checkbox && child.ui.checkbox.checked)
				{
					checkedChildrenNumber++;
					return true;
				}
			});

			if (node.parentNode.childNodes.length == checkedChildrenNumber)
			{
				_changeNodeStatus(node.parentNode, true);
				_checkNodes4ChildCascading(node.parentNode, true, true);
			}
		}
		else
		{
			_changeNodeStatus(node.parentNode, false);
			_checkNodes4ChildCascading(node.parentNode, false, false);
		}
	}

	function _checkChildren(node, checked)
	{
		if (checked && _cascading == 'standard') return;
		if (node.hasChildNodes())
		{
			for (var i = 0; i < node.childNodes.length; i++)
			{
				var child = node.childNodes[i];
				_changeNodeStatus(child, checked);
				_checkChildren(child, checked);
			}
		}
	}

	function _checkParent(node, checked)
	{
		if (!node.parentNode || !node.parentNode.ui.checkbox) return;
		if (!checked && _cascading == 'standard') return;

		var parentNode = node.parentNode;
		if (!checked)
		{
			var hasOtherNodesChecked = false;
			parentNode.eachChild(function(child)
			{
				if (child.ui.checkbox && child.ui.checkbox.checked)
				{
					hasOtherNodesChecked = true;
					return true;
				}
			});

			if (hasOtherNodesChecked) return;
		}

		_changeNodeStatus(parentNode, checked);
		_checkParent(parentNode, checked);
	}

	function _changeNodeStatus(node, checked)
	{
		if (!node.ui) return;
		if (!node.ui.checkbox) return;

		node.ui.checkbox.checked = checked;
		if (!node.id || node.id.indexOf("ynode-") > -1) return;

		if (!checked)
		{
			if (isTheNodeValueSelected(node.id))
				removeSelectedItem(node.id);
		}
		else
		{
			if (!isTheNodeValueSelected(node.id))
				addSelectedItem({ Id:node.id, Name:node.text });
		}
	}
}