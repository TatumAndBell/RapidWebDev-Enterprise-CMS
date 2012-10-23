// * argument "clientId": string - the html container which used to locate the grid.

// * argument "uniqueName": string - http request post value of the control.

// * argument "datasource" : Array of objects with following properties
// datasource[indexer].Text: string
// datasource[indexer].Value: string
// datasource[indexer].Selected: bool

// * argument "options"
// options.RowEditorSaveText: string
// options.RowEditorCancelText: string
// options.SelectionItemTextColumnHeader: string
// options.SelectionItemValueColumnHeader: string
// options.SelectionItemSelectedColumnHeader: string
// options.AddItem: string
// options.RemoveItemText: string
// options.YesText: string
// options.NoText: string
// options.Width: integer
// options.Height: integer
// options.Disabled: boolean

var SelectionFieldMetadataControl = function(clientId, uniqueName, datasource, options)
{
	if (!options) alert("The argument \"options\" cannot be null.");
	var hiddenFieldId = clientId + '_hidden';
	var hiddenFieldSelector = '#' + clientId + '_hidden';

	var store, rowEditor, grid;

	this.destroy = function()
	{
		if (grid) grid.destroy();
		if (rowEditor) rowEditor.destroy();
		if (store) store.destroy();
	}

	function updateDataSource(store, operateRecord, isDelete)
	{
		var selected = operateRecord.data.Selected;
		var cachedDataSource = new Array();
		var needToReload = false;
		for (var i = 0; i < store.getCount(); i++)
		{
			var record = store.getAt(i);
			if (selected == true && isDelete == false && operateRecord != record)
			{
				record.data.Selected = false;
				needToReload = true;
			}

			cachedDataSource.push({ Text: record.data.Text, Value: record.data.Value, Selected: record.data.Selected });
		}

		$(hiddenFieldSelector).val(Ext.encode(cachedDataSource));
		if (needToReload) grid.getView().refresh();
	}

	function initializeControl()
	{
		var html = '<input id="' + hiddenFieldId + '" name="' + uniqueName + '" type="hidden" />';
		Ext.DomHelper.insertHtml("beforeBegin", Ext.DomQuery.selectNode("#" + clientId), html);
		$(hiddenFieldSelector).val(Ext.encode(datasource));

		var SelectionItem = Ext.data.Record.create(
		[
			{ name: 'Text', type: 'string' },
			{ name: 'Value', type: 'string' },
			{ name: 'Selected', type: 'bool' }
		]);

		store = new Ext.data.Store(
		{
			reader: new Ext.data.JsonReader({ fields: SelectionItem }),
			data: datasource,
			sortInfo: { field: 'Text', direction: 'ASC' },
			listeners:
			{
				add: function(store, records, index)
				{
					updateDataSource(store, records[0], false);
				},
				update: function(store, record, operation)
				{
					updateDataSource(store, record, false);
				},
				remove: function(store, record, index)
				{
					updateDataSource(store, record, true);
				}
			}
		});



		var titleBar = null;
		if (!options.Disabled)
		{
			rowEditor = new Ext.ux.grid.RowEditor({ saveText: options.RowEditorSaveText, cancelText: options.RowEditorCancelText, disabled: options.Disabled });
			titleBar =
                [
				    {
				    	iconCls: 'selectionFieldMetadataControl-icon-add',
				    	text: options.AddItem,
				    	handler: function()
				    	{
				    		var selectionItem = new SelectionItem({ Text: '', Selected: false });
				    		rowEditor.stopEditing();
				    		store.insert(0, selectionItem);
				    		grid.getView().refresh();
				    		grid.getSelectionModel().selectRow(0);
				    		rowEditor.startEditing(0);
				    	}
				    },
				    {
				    	ref: '../removeBtn',
				    	iconCls: 'selectionFieldMetadataControl-icon-delete',
				    	text: options.RemoveItemText,
				    	disabled: true,
				    	handler: function()
				    	{
				    		rowEditor.stopEditing();
				    		var s = grid.getSelectionModel().getSelections();
				    		for (var i = 0, r; r = s[i]; i++)
				    			store.remove(r);
				    	}
				    }
			    ];
		}

		grid = new Ext.grid.GridPanel(
		{
			store: store,
			width: options.Width,
			height: options.Height,
			plugins: rowEditor,
			tbar: titleBar,
			columns:
			[
				{
					id: 'SelectionItemText',
					header: options.SelectionItemTextColumnHeader,
					dataIndex: 'Text',
					width: 200,
					sortable: true,
					editor: { xtype: 'textfield' }
				},
				{
					id: 'SelectionItemValue',
					header: options.SelectionItemValueColumnHeader,
					dataIndex: 'Value',
					width: 140,
					sortable: true,
					editor: { xtype: 'textfield' }
				},
				{
					id: 'SelectionItemSelected',
					xtype: 'booleancolumn',
					header: options.SelectionItemSelectedColumnHeader,
					dataIndex: 'Selected',
					align: 'center',
					width: 80,
					trueText: options.YesText,
					falseText: options.NoText,
					editor: { xtype: 'checkbox' }
				}
			]
		});

		if (!options.Disabled)
			grid.getSelectionModel().on('selectionchange', function(sm) { grid.removeBtn.setDisabled(sm.getCount() < 1); });

		grid.render(clientId);
	}

	if (Ext.isReady) initializeControl();
	else Ext.onReady(function() { initializeControl(); });
}