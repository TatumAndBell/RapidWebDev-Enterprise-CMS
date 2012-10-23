// * argument "options"
// options.useVistaLikeArrow: boolean - true to use vista like arrow into the treeview.
// options.fillBackground: boolean - true to fill background with deep color.
// options.cascadingMode: string -
//              1) 'full': cascading check/uncheck the parent and children when a node is checked/unchecked
//              2) 'none': no impact to other tree node when a tree node is checked/unchecked
//              3) 'standard': the parent will be checked when a node is checked. The children will be unchecked when the parent node is unchecked.
//                      standard means there allows the parent node been checked without any children checked, but all the ancestors have to be checked when a child is checked.

function TreeView(treeViewContainerId, dataSource, options)
{
    if (!options) alert("The TreeView argument options cannot be null.");
    
	var treeview;
	var hiddenFieldId = treeViewContainerId + "__hidden";
	var cachedCheckedNodeIds = new Array();
	var originalCheckedNodeIds = new Array();

	this.checkNodes = function(nodeIdArray)
	{
		var rootNode = treeview.getRootNode();
		_checkNodes(rootNode, nodeIdArray);
	}

	this.getCheckedNodeIdArray = function()
	{
		return $("#" + hiddenFieldId).val();
	}

	this.checkAll = function()
	{
	    var rootNode = treeview.getRootNode();
	    _checkAll(rootNode);
	}

	this.uncheckAll = function()
	{
	    var rootNode = treeview.getRootNode();
	    _uncheckAll(rootNode);
	}

	this.restore = function()
	{
	    this.uncheckAll();
	    this.checkNodes(originalCheckedNodeIds);
	}

	this.saveState = function()
	{
	    // save initial checked node ids for restore
	    Ext.each(cachedCheckedNodeIds, function(item) { originalCheckedNodeIds.push(item); });
	}

	this.disableCheckBox = function()
	{
	    var rootNode = treeview.getRootNode();
	    _disableCheckBox(rootNode);
	}

	function initializeTreeView()
	{
		var html = '<input id="' + hiddenFieldId + '" name="' + hiddenFieldId + '" type="hidden" />';
		Ext.DomHelper.insertHtml("beforeBegin", Ext.DomQuery.selectNode("#" + treeViewContainerId), html);

		treeview = new Ext.tree.TreePanel(
		{
			renderTo: treeViewContainerId,
			useArrows: options.useVistaLikeArrow,
			autoScroll: false,
			animate: false,
			animCollapse: false,
			enableDD: false,
			rootVisible: false,
			frame: options.fillBackground,
			root: { children: dataSource },
			listeners:
			{
				checkchange: function(node, checked)
				{
					_checkNode(node, checked);
				}
			}
		});
	}

	if (Ext.isReady) initializeTreeView();
	else Ext.onReady(function() { initializeTreeView(); });

	function _disableCheckBox(parent)
	{
	    if (!parent.hasChildNodes()) return;

	    parent.eachChild(function(child)
	    {
	    	if (child.ui.checkbox)
	    	{
	    		child.ui.checkbox.disabled = true;
	    		_disableCheckBox(child);
	    	}
	    });
	}

	function _uncheckAll(parent)
	{
	    if (!parent.hasChildNodes()) return;

	    parent.eachChild(function(child)
	    {
	        _changeNodeStatus(child, false);
	        _uncheckAll(child);
	    });
	}

	function _checkAll(parent)
	{
	    if (!parent.hasChildNodes()) return;

	    parent.eachChild(function(child)
	    {
	        _changeNodeStatus(child, true);
	        _checkAll(child);
	    });
	}

	function _checkNodes(node, nodeIdArray)
	{
		if (nodeIdArray.indexOf(node.id) > -1)
			_checkNode(node, true);

		node.eachChild(function(child) { _checkNodes(child, nodeIdArray); });
	}

	function _checkNode(node, checked)
	{
	    _changeNodeStatus(node, checked);

	    if (options.cascadingMode != 'none')
	    {
	        _checkChildren(node, checked);    
	        _checkParent(node, checked);
	    }
	}

	function _checkChildren(node, checked)
	{
	    if (checked && options.cascadingMode == 'standard') return;
		if (node.hasChildNodes())
		{
			node.eachChild(function(child)
			{
				_changeNodeStatus(child, checked);
				_checkChildren(child, checked);
			});
		}
	}

	function _checkParent(node, checked)
	{
	    if (!node.parentNode || !node.parentNode.ui.checkbox) return;
	    if (!checked && options.cascadingMode == 'standard') return;

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
		if (!node.ui.checkbox) return;
		
		node.ui.checkbox.checked = checked;
		if (!node.id || node.id.indexOf("ynode-") > -1) return;
		
		if (!checked)
		{
			if (cachedCheckedNodeIds.indexOf(node.id) > -1)
			{
				cachedCheckedNodeIds.remove(node.id);
				$("#" + hiddenFieldId).val(cachedCheckedNodeIds);
			}
		}
		else
		{
			if (cachedCheckedNodeIds.indexOf(node.id) == -1)
			{
				cachedCheckedNodeIds.push(node.id);
				$("#" + hiddenFieldId).val(cachedCheckedNodeIds);
			}
		}
	}
}