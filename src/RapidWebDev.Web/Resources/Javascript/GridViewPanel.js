// properties of parameter _gridConfig.
// 1) ClientId: string, the client ID of grid panel in client.
// 2) ViewConfig: object with properties as following 2.X
//		2.1) FieldName : string, the field displayed in row view segment.
//		2.2) TagName : string, the html tag to wrap output content in row view segment.
//		2.3) Css : string, apply custom CSS classes to row view during rendering
// 3) HeaderText: string, grid panel header text.
// 4) Height: integer, grid panel height.
// 5) ShowCheckBoxColumn: boolean, true indicates to show checkbox-column in the grid.
// 6) ShowEditButton: not-null reference means to display edit button in grid rows.
//      6.1) DisplayAsImage: boolean, display edit button as linked image.
//      6.2) Text: string, edit button text if DisplayAsImage is set to false explicitly.
// 7) ShowViewButton: not-null reference means to display view button in grid rows.
//      7.1) DisplayAsImage: boolean, display view button as linked image.
//      7.2) Text: string, view button text if DisplayAsImage is set to false explicitly.
// 8) ShowDeleteButton: not-null reference means to display delete button in grid rows.
//      8.1) DisplayAsImage: boolean, display delete button as linked image.
//      8.2) Text: string, delete button text if DisplayAsImage is set to false explicitly.
// 9) Columns: array of column object defined as following,
//      9.1) Renderer: string, please refer to ExtJs API Documentation, ColumnModel ->  setRenderer for the more information
//      9.2) FieldName: string, The bound field name.
//      9.3) HeaderText: string, The column header text.
//      9.4) Sortable: boolean, True indicates the column is sortable.
//      9.5) Resizable: boolean, False to disable column resizing.
//      9.6) Css: string, Set custom CSS for all table cells in the column.
//      9.7) Width: integer, The initial width in pixels of the column.
//      9.8) Align: string, Set the CSS text-align property of the column.
//      9.9) Hidden: boolean, True to hide the column initially. Defaults to false. 

// properties of parameter _detailPanelModalWindowConfig.
// 1) Width: integer
// 2) Height: integer
// 3) Resizable: boolean
// 4) Draggable: boolean

// properties of parameter _aggregatePanelModalWindowConfig.
// 1) Width: integer
// 2) Height: integer
// 3) Resizable: boolean
// 4) Draggable: boolean

// properties of parameter _JsonStoreConfig.
// 1) Root: string
// 2) TotalProperty: string
// 3) IdProperty: string
// 4) Fields: string array
// 5) PageSize: integer
// 6) DefaultSortField: string, the default sort field name which should be included in Fields.
// 7) DefaultSortDirection: string, the candidate values are ASC | DESC.

// properties of parameter _globalizationResources
// 1) DeleteConfirmDialogTitle: string, an example as 'Delete Confirmation'
// 2) DeleteConfirmMessage: string, an example as 'Are you sure to delete the record?'
// 3) PagingDisplayMessageTemplate: string, Note that this string is formatted using the braced numbers 0-2 as tokens that are replaced by the values for start, end and total respectively. Example as 'Displaying records {0} - {1} of {2}'
// 4) EmptyGridPanelMessage: string, an example as 'No record found'
// 5) PreviewButtonText: string, an example as 'Show Preview'
// 6) GridPanelHeadCheckBoxToolTip: string, an example as 'The checkbox checked in column header means that all records by the query includes not displayed ones in this grid page will be processed together.'
// 7) EditableDetailPanelHeaderText: string, detail panel header text in editable mode
// 8) ViewableDetailPanelHeaderText: string, detail panel header text in viewable mode
// 9) AggregatePanelHeaderText: string, aggregate panel header text

function OnGridHeadCheckBoxClicked(checkbox, gridId)
{
    $('.gridRowCheckBox', '#' + gridId).attr('checked', checkbox.checked);
}

function OnGridRowCheckBoxClicked(checkbox, gridId)
{
    if (!checkbox.checked)
        $('.gridHeadCheckBox', '#' + gridId).attr('checked', false);
}

// Resolve a collection of key-value pair from query panel form.
function ResolvePostValuesFromQueryPanel()
{
    var controlValues = new Array();
    if (!window.QueryFieldControlVariablesAccessorId) return controlValues;
    
    var queryFieldControlString = Ext.query('#' + window.QueryFieldControlVariablesAccessorId)[0].value;
    var queryFieldControls = queryFieldControlString.split(';');
    for (var i = 0; i < queryFieldControls.length; i++)
    {
        var queryFieldControl = queryFieldControls[i];
        var queryFieldControlKeyValuePair = queryFieldControl.split(':');
        var paramName = queryFieldControlKeyValuePair[0].trim();
        var getParamValueJs = 'window.' + queryFieldControlKeyValuePair[1] + '.getValue();';
        var rawParamValue = eval(getParamValueJs);
        if (rawParamValue != undefined && rawParamValue != null)
        {
        	var paramValue;
        	if (rawParamValue instanceof Date)
        		paramValue = rawParamValue.format('Y-m-d H:i:s');
        	else
        	    paramValue = rawParamValue.toString();
        		
        	controlValues.push({ 'Name': paramName, 'Value': paramValue });
        }
    }

    return controlValues;
}

function GridViewPanelClass(
    _variableName,
	_gridConfig,
	_detailPanelModalWindowConfig,
	_aggregatePanelModalWindowConfig,
	_JsonStoreConfig,
	_globalizationResources)
{
    var objectId;
	var extGridPanelId = _gridConfig.ClientId + '_ExtGrid';
	var detailPanelIframeId = 'DynamicPageDetailPanelModalWindowManifestIframe';
	var detailPanelUrlTemplate = 'DetailPanel.svc?{0}';
	var aggregatePanelIframeId = 'DynamicPageAggregatePanelModalWindowManifestIframe';
	var aggregatePanelUrlTemplate = 'AggregatePanel.svc?{0}';
	var dynamicPageDataServiceUrl = 'DynamicPageDataService.svc';

	this.DynamicPageGridPanelJsonStoreObj = null;
	this.DynamicPageGridPanelPagingBarObj = null;
	this.DynamicPageGridPanelObj = null;
	this.DynamicPageGridPanelHeadCheckBoxToolTipObj = null;

	this.ExecuteQuery = function(reload, extraParameters)
	{
	    if (!this.DynamicPageGridPanelJsonStoreObj) return;
	    var postValues = ResolvePostValuesFromQueryPanel();
	    var baseParams = new Object();
	    Ext.each(postValues, function(postValue)
	    {
	        if (postValue.Value)
	        {
	        	var evalCode = String.format('baseParams.{0} = postValue.Value;', postValue.Name);
	            eval(evalCode);
	        }
	    });

	    if (window.GlobalDynamicPageDataServicePostVariables)
	    {
	        Ext.each(window.GlobalDynamicPageDataServicePostVariables, function(globalVariable)
	        {
	            if (globalVariable.Value)
	            {
	            	var evalCode = String.format('baseParams.{0} = globalVariable.Value;', globalVariable.Name);
	                eval(evalCode);
	            }
	        });
	    }

	    if (extraParameters)
	    {
	        Ext.each(extraParameters, function(extraParameter)
	        {
	            if (extraParameter.Name && extraParameter.Value)
	            {
	            	var evalCode = String.format('baseParams.{0} = extraParameter.Value;', extraParameter.Name);
	                eval(evalCode);
	            }
	        });
	    }

	    this.DynamicPageGridPanelJsonStoreObj.baseParams = baseParams;
	    if (!reload)
	        this.DynamicPageGridPanelJsonStoreObj.load(
			{
			    params: { start: 0, limit: _JsonStoreConfig.PageSize }
			});
	    else
	        this.DynamicPageGridPanelJsonStoreObj.reload();
	}

	this.ShowDetailPanelWindow = function(entityId, mode)
	{
		if (!_detailPanelModalWindowConfig)
			return;

		if (window.DynamicPageDetailPanelModalWindow)
			window.DynamicPageDetailPanelModalWindow.destroy();

		var iframeElement = window.frameElement;
		var detailPanelModelWindowWidth = frameElement ? iframeElement.clientWidth * 0.9 : _detailPanelModalWindowConfig.Width;
		var detailPanelModelWindowHeight = frameElement ? iframeElement.clientHeight * 0.95 : _detailPanelModalWindowConfig.Height;
		detailPanelModelWindowWidth = Math.min(detailPanelModelWindowWidth, _detailPanelModalWindowConfig.Width);
		detailPanelModelWindowHeight = Math.min(detailPanelModelWindowHeight, _detailPanelModalWindowConfig.Height);

		var windowTitle = mode.toString().toUpperCase() != "VIEW" ? _globalizationResources.EditableDetailPanelHeaderText : _globalizationResources.ViewableDetailPanelHeaderText;
		var iframeQueryString = 'entityid=' + entityId + '&rendermode=' + mode + '&stamp=' + (new Date()).format('YmdHisu');
		if (window.GlobalDynamicPageDataServicePostVariables)
		{
			Ext.each(window.GlobalDynamicPageDataServicePostVariables, function(globalVariable)
			{
				if (globalVariable.Value)
					iframeQueryString += '&' + globalVariable.Name + '=' + globalVariable.Value;
			});
		}

		var iframeUrl = String.format(detailPanelUrlTemplate, iframeQueryString);
		window.DynamicPageDetailPanelModalWindow = new Ext.Window(
		{
			layout: 'fit',
			width: detailPanelModelWindowWidth,
			height: detailPanelModelWindowHeight,
			plain: true,
			modal: true,
			resizable: _detailPanelModalWindowConfig.Resizable,
			draggable: _detailPanelModalWindowConfig.Draggable,
			title: windowTitle,
			items: new Ext.Panel(
			{
				header: false,
				closable: true,
				autoScroll: true,
				html: '<iframe id="' + detailPanelIframeId + '" src="' + iframeUrl + '" frameborder="0" width="100%"></iframe>',
				listeners:
				{
					resize: function(component, adjWidth, adjHeight, rawWidth, rawHeight)
					{
						if (component == undefined) return;
						if (rawHeight != undefined)
						{
							$("#" + detailPanelIframeId).height(rawHeight - 4);
						}
						else
						{
							var height = component.getHeight();
							$("#" + detailPanelIframeId).height(height - 4);
						}

						$("#" + detailPanelIframeId).width("100%");
					}
				}
			}),
			listeners:
			{
				beforeclose: function(p)
				{
					var iframeElement = Ext.getDom(detailPanelIframeId);
					if (iframeElement)
					{
						if (Ext.isIE) iframeElement.src = "javascript:false";
						Ext.removeNode(iframeElement);
						Ext.destroy(iframeElement);
					}
				}
			}
		});

		window.DynamicPageDetailPanelModalWindow.render(document.body);
		window.DynamicPageDetailPanelModalWindow.show();
	}

	this.HideDetailPanelWindow = function()
	{
		if (window.DynamicPageDetailPanelModalWindow)
		{
			var iframeElement = Ext.getDom(detailPanelIframeId);
			if (iframeElement)
			{
				if(Ext.isIE) iframeElement.src = "javascript:false";
				Ext.removeNode(iframeElement);
				Ext.destroy(iframeElement);
			}
			
			window.DynamicPageDetailPanelModalWindow.destroy();
		}
	}

	this.ShowAggregatePanelWindow = function(commandArgument, gridSelectionRequired, gridSelectionRequiredWarningMessage)
	{
		if (!_aggregatePanelModalWindowConfig)
			return;

		if (window.DynamicPageAggregatePanelModalWindow)
			window.DynamicPageAggregatePanelModalWindow.destroy();

		// save grid selection into the cookie so that the aggregate web page handler can gather them from the cookie. 
		var selectedEntityIdByQueryKey = objectId + "_SelectedEntityIdByQuery";
		var selectedEntityIdCollectionKey = objectId + "_SelectedEntityIdCollection";
		Ext.state.Manager.clear(selectedEntityIdByQueryKey);
		Ext.state.Manager.clear(selectedEntityIdCollectionKey);

		var gridIdSelector = "#" + _gridConfig.ClientId;
		var whetherHeadCheckBoxChecked = $("#gridHeadCheckBox", gridIdSelector).attr("checked");
		// save current query criterias if the checkbox in column header is checked. That means all records found by the query will be processed in aggregate web page handler.
		if (whetherHeadCheckBoxChecked)
		{
			var postValues = ResolvePostValuesFromQueryPanel();
			Ext.state.Manager.set(selectedEntityIdByQueryKey, Ext.util.JSON.encode(postValues));
		}
		// save checked entity ids into the cookie.
		else
		{
			var checkedCheckBoxes = $(".gridRowCheckBox:checked", gridIdSelector);
			
			// there must have grid data selected before sending request to web services
			if (gridSelectionRequired == true && checkedCheckBoxes.length == 0)
			{
				RWD.MessageBox.Warn("Warning", gridSelectionRequiredWarningMessage);
				return;
			}

			var checkedEntityIdArray = new Array();
			$.each(checkedCheckBoxes, function(index, checkBox)
			{
				var entityId = checkBox.id.split("_")[1];
				checkedEntityIdArray.push(entityId);
			});

			Ext.state.Manager.set(selectedEntityIdCollectionKey, Ext.util.JSON.encode(checkedEntityIdArray));
		}

		var iframeElement = window.frameElement;
		var aggregatePanelModelWindowWidth = frameElement ? iframeElement.clientWidth * 0.9 : _aggregatePanelModalWindowConfig.Width;
		var aggregatePanelModelWindowHeight = frameElement ? iframeElement.clientHeight * 0.95 : _aggregatePanelModalWindowConfig.Height;
		aggregatePanelModelWindowWidth = Math.min(aggregatePanelModelWindowWidth, _aggregatePanelModalWindowConfig.Width);
		aggregatePanelModelWindowHeight = Math.min(aggregatePanelModelWindowHeight, _aggregatePanelModalWindowConfig.Height);

		var iframeQueryString = 'stamp=' + (new Date()).format('YmdHisu') + '&CommandArgument=' + commandArgument;
		if (window.GlobalDynamicPageDataServicePostVariables)
		{
			Ext.each(window.GlobalDynamicPageDataServicePostVariables, function(globalVariable)
			{
				if (globalVariable.Value)
					iframeQueryString += '&' + globalVariable.Name + '=' + globalVariable.Value;
			});
		}

		var iframeUrl = String.format(aggregatePanelUrlTemplate, iframeQueryString);
		window.DynamicPageAggregatePanelModalWindow = new Ext.Window(
		{
			layout: 'fit',
			width: aggregatePanelModelWindowWidth,
			height: aggregatePanelModelWindowHeight,
			plain: true,
			modal: true,
			resizable: _aggregatePanelModalWindowConfig.Resizable,
			draggable: _aggregatePanelModalWindowConfig.Draggable,
			title: _globalizationResources.AggregatePanelHeaderText,
			items: new Ext.Panel(
			{
				header: false,
				closable: true,
				autoScroll: true,
				html: '<iframe id="' + aggregatePanelIframeId + '" src="' + iframeUrl + '" frameborder="0" width="100%"></iframe>',
				listeners:
				{
					resize: function(component, adjWidth, adjHeight, rawWidth, rawHeight)
					{
						//alert("adjWidth: " + adjWidth + ", adjHeight: " + adjHeight + ", rawWidth: " + rawWidth + ", rawHeight: " + rawHeight);
						if (component == undefined) return;
						if (rawHeight != undefined)
						{
							$("#" + aggregatePanelIframeId).height(rawHeight - 4);
						}
						else
						{
							var height = component.getHeight();
							$("#" + aggregatePanelIframeId).height(height - 4);
						}

						$("#" + aggregatePanelIframeId).width("100%");
					}
				}
			}),
			listeners:
			{
				beforeclose: function(p)
				{
					var iframeElement = Ext.getDom(aggregatePanelIframeId);
					if (iframeElement)
					{
						if (Ext.isIE) iframeElement.src = "javascript:false";
						Ext.removeNode(iframeElement);
						Ext.destroy(iframeElement);
					}
				}
			}
		});

		window.DynamicPageAggregatePanelModalWindow.render(document.body);
		window.DynamicPageAggregatePanelModalWindow.show();
	}

	this.HideAggregatePanelWindow = function()
	{
	    if (window.DynamicPageAggregatePanelModalWindow)
	    {
	    	// clear the cookie of records selection
	    	var selectedEntityIdByQueryKey = objectId + "_SelectedEntityIdByQuery";
	    	var selectedEntityIdCollectionKey = objectId + "_SelectedEntityIdCollection";
	    	Ext.state.Manager.clear(selectedEntityIdByQueryKey);
	    	Ext.state.Manager.clear(selectedEntityIdCollectionKey);
	    	
	        var iframeElement = Ext.getDom(aggregatePanelIframeId);
	        if (iframeElement)
	        {
	            if (Ext.isIE) iframeElement.src = "javascript:false";
	            Ext.removeNode(iframeElement);
	            Ext.destroy(iframeElement);
	        }

	        window.DynamicPageAggregatePanelModalWindow.destroy();
	    }
	}

	this.ResizeGridViewPanel = function()
	{
		Ext.onReady(function()
		{
			DoResize(this.DynamicPageGridPanelObj);
			Ext.EventManager.onWindowResize(function()
			{
				DoResize(this.DynamicPageGridPanelObj);
			}, this);
		}, this);

		function DoResize(gridPanelObj)
		{
			var totalControlsHeight = 0;
			var controlsCount = 0;

			// calculate height of query panel
			var queryPanelSearcher = Ext.DomQuery.select(".querypanel");
			if (queryPanelSearcher.length > 0)
			{
				Ext.each(queryPanelSearcher, function(queryPanel)
				{
					if ($(queryPanel).css("display") == "none") return;
					var height = queryPanel.offsetHeight;
					totalControlsHeight += height;
					controlsCount++;
				});
			}
			else
				totalControlsHeight += 6; // there are 6px vertical gap for each query panel.

			// calculate height of button panels
			var buttonPanelSearcher = Ext.DomQuery.select(".buttonpanel");
			if (buttonPanelSearcher.length > 0)
			{
				Ext.each(buttonPanelSearcher, function(buttonPanel)
				{
					if ($(buttonPanel).css("display") == "none") return;
					var height = buttonPanel.offsetHeight;
					totalControlsHeight += height;
					controlsCount++;
				});

				totalControlsHeight -= (buttonPanelSearcher.length - 1) * 2;
			}

			var bodyHeight = document.documentElement.clientHeight;
			var gridViewPanelHeight = bodyHeight - totalControlsHeight - controlsCount * 8;
			var offsetHeight = 2;
			gridPanelObj.setHeight(gridViewPanelHeight + offsetHeight);
		}
	}

	this.HasAnyCheckBoxChecked = function()
	{
		var gridIdSelector = "#" + _gridConfig.ClientId;
		var whetherHeadCheckBoxChecked = $("#gridHeadCheckBox", gridIdSelector).attr("checked");
		if (whetherHeadCheckBoxChecked)
			return true;

		return $(".gridRowCheckBox:checked", gridIdSelector).length > 0;
	}

	this.OnRowEditButtonClicked = function(entityId)
	{
	    this.ShowDetailPanelWindow(entityId, 'Update');
	}

	this.OnRowViewButtonClicked = function(entityId)
	{
	    this.ShowDetailPanelWindow(entityId, 'View');
	}

	this.OnRowDeleteButtonClicked = function(entityId)
	{
		var confirmCallback = function(id)
		{
			if (id && id.toString().toLowerCase() == "yes")
			{
				var extraParams = [{ Name: "DeleteId", Value: entityId}];
				this.ExecuteQuery(true, extraParams);
			}
		}

		if (!Ext.MessageBox.confirm(_globalizationResources.DeleteConfirmDialogTitle, _globalizationResources.DeleteConfirmMessage, confirmCallback, this)) return;
	}

	function RenderEditButton(value, metadata, record, rowIndex, colIndex, store)
	{
		if (record.data._ShowEditButtonColumn)
		{
			var outputHtml = null;
			if (_gridConfig.ShowEditButton.DisplayAsImage !== false)
			{
				var tooltip = _gridConfig.ShowEditButton.ToolTip ? _gridConfig.ShowEditButton.ToolTip : "";
				outputHtml = '<img src="/Resources/Images/edit.gif" title="' + tooltip + '" />';
			}
			else
				outputHtml = _gridConfig.ShowEditButton.Text;

			var invokeMethod = _variableName + ".OnRowEditButtonClicked(\\\'" + value + "\\\')";
			var invokeCode = "eval('" + invokeMethod + "')";
			var tooltip = _gridConfig.ShowEditButton.ToolTip ? _gridConfig.ShowEditButton.ToolTip : "";
			return '<a href="#" onclick="' + invokeCode + '" title="' + tooltip + '">' + outputHtml + '</a>';
		}
	}

	function RenderViewButton(value, metadata, record, rowIndex, colIndex, store)
	{
		if (record.data._ShowViewButtonColumn)
		{
			var outputHtml = null;
			if (_gridConfig.ShowViewButton.DisplayAsImage !== false)
			{
				var tooltip = _gridConfig.ShowViewButton.ToolTip ? _gridConfig.ShowViewButton.ToolTip : "";
				outputHtml = '<img src="/Resources/Images/view.gif" title="' + tooltip + '"/>';
			}
			else
				outputHtml = _gridConfig.ShowViewButton.Text;

			var invokeMethod = _variableName + ".OnRowViewButtonClicked(\\\'" + value + "\\\')";
			var invokeCode = "eval('" + invokeMethod + "')";
			return '<a href="#" onclick="' + invokeCode + '">' + outputHtml + '</a>';
		}
	}

	function RenderDeleteButton(value, metadata, record, rowIndex, colIndex, store)
	{
		if (record.data._ShowDeleteButtonColumn)
		{
			var outputHtml = null;
			if (_gridConfig.ShowDeleteButton.DisplayAsImage !== false)
			{
				var tooltip = _gridConfig.ShowDeleteButton.ToolTip ? _gridConfig.ShowDeleteButton.ToolTip : "";
				outputHtml = '<img src="/Resources/Images/delete.gif" title="' + tooltip + '" />';
			}
			else
				outputHtml = _gridConfig.ShowDeleteButton.Text;

			var invokeMethod = _variableName + ".OnRowDeleteButtonClicked(\\\'" + value + "\\\')";
			var invokeCode = "eval('" + invokeMethod + "')";
			return '<a href="#" onclick="' + invokeCode + '">' + outputHtml + '</a>';
		}
	}

	function RenderCheckBoxColumn(value, metadata, record, rowIndex, colIndex, store)
	{
		if (record.data._ShowCheckBoxColumn)
		{
			var checkBoxId = "rowCheckBox_" + value;
			return '<input type="checkbox" id="' + checkBoxId + '" name="' + checkBoxId + '" class="gridRowCheckBox" style="padding:0; margin:0" onclick="OnGridRowCheckBoxClicked(this, \'' + extGridPanelId + '\')" />';
		}
	}

	function CreateGridColumnConfig()
	{
		var columns = new Array();

        // render checkbox column
		if (_gridConfig.ShowCheckBoxColumn)
		{
			columns.push({
				header: '<input type="checkbox" id="gridHeadCheckBox" name="gridHeadCheckBox" class="gridHeadCheckBox" onclick="OnGridHeadCheckBoxClicked(this, \'' + extGridPanelId + '\')" style="padding:0; margin:0" />',
				dataIndex: _JsonStoreConfig.IdProperty,
				renderer: RenderCheckBoxColumn,
				width: 25,
				fixed: true,
				sortable: false,
				resizable: false,
				menuDisabled: true
			});
		}

		// render common columns
		if (_gridConfig.Columns != undefined && _gridConfig.Columns != null)
		{
			Ext.each(_gridConfig.Columns, function(columnConfig, index, allItems)
			{
				var column = {
					header: columnConfig.HeaderText,
					dataIndex: columnConfig.FieldName
				};

				if (columnConfig.Renderer)
					column.renderer = function(value, metadata, record, rowIndex, colIndex, store) { eval(columnConfig.Renderer); };

				if (columnConfig.Width)
					column.width = columnConfig.Width;

				if (columnConfig.Sortable != undefined && columnConfig.Sortable != null && !columnConfig.Sortable)
					column.sortable = false;
				else
					column.sortable = true;

				if (columnConfig.Resizable != undefined && columnConfig.Resizable != null && !columnConfig.Resizable)
					column.resizable = false;

				if (columnConfig.Css)
					column.css = columnConfig.Css;

				if (columnConfig.Align)
					column.align = columnConfig.Align.toLowerCase();

				if (columnConfig.Hidden != undefined && columnConfig.Hidden != null && columnConfig.Hidden)
					column.hidden = true;

				columns.push(column);
			});
		}

		// render view button column
		if (_gridConfig.ShowViewButton != undefined && _gridConfig.ShowViewButton != null)
		{
		    columns.push({
		        header: "",
		        dataIndex: _JsonStoreConfig.IdProperty,
		        width: _gridConfig.ShowViewButton.DisplayAsImage !== false ? 26 : 40,
		        fixed: true,
		        sortable: false,
		        resizable: false,
		        hideable: false,
		        menuDisabled: true,
		        align: "center",
		        renderer: RenderViewButton
		    });
		}

		// render edit button column
		if (_gridConfig.ShowEditButton != undefined && _gridConfig.ShowEditButton != null)
		{
		    columns.push({
		        header: "",
		        dataIndex: _JsonStoreConfig.IdProperty,
		        width: _gridConfig.ShowEditButton.DisplayAsImage !== false ? 26 : 40,
		        fixed: true,
		        sortable: false,
		        resizable: false,
		        menuDisabled: true,
		        align: "center",
		        renderer: RenderEditButton
		    });
		}

		// render delete button column
		if (_gridConfig.ShowDeleteButton != undefined && _gridConfig.ShowDeleteButton != null)
		{
		    columns.push({
		        header: "",
		        dataIndex: _JsonStoreConfig.IdProperty,
		        width: _gridConfig.ShowDeleteButton.DisplayAsImage !== false ? 26 : 40,
		        fixed: true,
		        sortable: false,
		        resizable: false,
		        menuDisabled: true,
		        align: "center",
		        renderer: RenderDeleteButton
		    });
		}

		return columns;
	}

	Ext.onReady(function()
	{
		if (!window.GlobalDynamicPageDataServicePostVariables || !window.GlobalDynamicPageDataServicePostVariables.Contains('ObjectId'))
			return;

		Ext.QuickTips.init();

		objectId = window.GlobalDynamicPageDataServicePostVariables.GetItem('ObjectId');

		// destroy existed resources
		if (this.DynamicPageGridPanelJsonStoreObj) this.DynamicPageGridPanelJsonStoreObj.destroy();
		if (this.DynamicPageGridPanelPagingBarObj) this.DynamicPageGridPanelPagingBarObj.destroy();
		if (this.DynamicPageGridPanelObj) this.DynamicPageGridPanelObj.destroy();
		if (this.DynamicPageGridPanelHeadCheckBoxToolTipObj) this.DynamicPageGridPanelHeadCheckBoxToolTipObj.destroy();

		// create the data store
		this.DynamicPageGridPanelJsonStoreObj = new Ext.data.JsonStore(
		{
			root: _JsonStoreConfig.Root,
			totalProperty: _JsonStoreConfig.TotalProperty,
			idProperty: _JsonStoreConfig.IdProperty,
			remoteSort: true,
			fields: _JsonStoreConfig.Fields,
			proxy: new Ext.data.HttpProxy(
			{
				url: dynamicPageDataServiceUrl,
				method: 'GET'
			}),
			paramNames: { sort: 'sortField', dir: 'sortDirection' },
			listeners:
			{
				loadexception: function(thisObject, options, response, error)
				{
					var responseObj = null;
					eval('responseObj = ' + response.responseText + ';');
					if (responseObj.HasError)
						window.RWD.MessageBox.Error('', responseObj.ErrorMessage);
				}
			}
		});

		this.DynamicPageGridPanelPagingBarObj = new Ext.PagingToolbar(
		{
			pageSize: _JsonStoreConfig.PageSize,
			store: this.DynamicPageGridPanelJsonStoreObj,
			displayInfo: true,
			displayMsg: _globalizationResources.PagingDisplayMessageTemplate,
			emptyMsg: _globalizationResources.EmptyGridPanelMessage,
			paramNames: { start: 'start', limit: 'limit' }
		});

		var gridViewConfig = null;
		if (_gridConfig.ViewConfig)
		{
			gridViewConfig = new Object();
			gridViewConfig.forceFit = true;
			gridViewConfig.enableRowBody = true;
			gridViewConfig.showPreview = true;
			gridViewConfig.getRowClass = function(record, rowIndex, p, store)
			{
				if (this.showPreview)
				{
					var displayString = eval('record.data.' + _gridConfig.ViewConfig.FieldName);
					var previewSegmentClass = _gridConfig.ViewConfig.Css ? _gridConfig.ViewConfig.Css : "grid-preview-fragment";
					p.body = '<' + _gridConfig.ViewConfig.TagName + ' class="' + previewSegmentClass + '">' + displayString + '</' + _gridConfig.ViewConfig.TagName + '>';
					return 'x-grid3-row-expanded';
				}

				return 'x-grid3-row-collapsed';
			};
		}

		this.DynamicPageGridPanelObj = new Ext.grid.GridPanel(
		{
			id: extGridPanelId,
			height: _gridConfig.Height,
			title: _gridConfig.HeaderText,
			store: this.DynamicPageGridPanelJsonStoreObj,
			trackMouseOver: false,
			disableSelection: false,
			loadMask: true,
			selModel: new Ext.grid.RowSelectionModel({ singleSelect: false }),

			// grid columns
			columns: CreateGridColumnConfig(),

			// customize view config
			viewConfig: gridViewConfig,

			// paging bar on the bottom
			bbar: this.DynamicPageGridPanelPagingBarObj
		});

		// render it
		this.DynamicPageGridPanelObj.render(_gridConfig.ClientId);

		if (_gridConfig.ViewConfig)
		{
			// render "show preview" button onto PagingToolBar
			this.DynamicPageGridPanelPagingBarObj.addSeparator();
			var showPreviewButton = this.DynamicPageGridPanelPagingBarObj.addButton({
				pressed: true,
				enableToggle: true,
				text: _globalizationResources.PreviewButtonText,
				cls: 'x-btn-text-icon details'
			});

			showPreviewButton.on("toggle", function(btn, pressed)
			{
				var view = this.DynamicPageGridPanelObj.getView();
				view.showPreview = pressed;
				view.refresh();
			}, this);

			this.DynamicPageGridPanelPagingBarObj.syncSize()
		}

		// set default sort
		if (_JsonStoreConfig.DefaultSortField)
		{
			var sortDirection = 'ASC';
			if (_JsonStoreConfig.DefaultSortDirection) sortDirection = _JsonStoreConfig.DefaultSortDirection;
			this.DynamicPageGridPanelJsonStoreObj.setDefaultSort(_JsonStoreConfig.DefaultSortField, sortDirection);
		}

		// create tooltip for the checkbox in checkbox-column header of grid.
		this.DynamicPageGridPanelHeadCheckBoxToolTipObj = new Ext.ToolTip(
		{
			showDelay: 0,
			dismissDelay: 0,
			target: 'gridHeadCheckBox',
			title: _globalizationResources.GridPanelHeadCheckBoxToolTip
		});
	}, this);
}