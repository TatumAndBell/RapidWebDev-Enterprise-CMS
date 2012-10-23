// 1) Property Align: string, candidate values are Center, Right and Left, defaults to Right

// 2) Property ClientID: string, The client id of ASP.NET rendered Panel control.

// 3) Property VariableName: string, The variable name of the ButtonPanel instance.

// 4) Property Buttons : array of object
//		Button: object
//		4.1) DisplayAsImage: boolean, defaults to false.
//		4.2) ImageUrl: string, only works if the DisplayAsImage is true.
//		4.3) Text: string, button text if DisplayAsImage equals to false
//		4.4) CommandArgument: string. It displays detail panel only if the argument value is "New" when the button is clicked. Otherwise it shows Aggregate Panel for custom operations to multiple selected records in grid.
//		4.5) ToolTip: string, button tooltip
//		4.6) GridSelectionRequired: boolean, to validate whether the user has selected any data in grid before sending request to web services.
//		4.7) GridSelectionRequiredWarningMessage: string, warning message popup to user if there is no grid data selected when the user clicks the button.
//		4.8) OnClientClick: string, the custom javascript code executed when the user clicks the button
//		4.9) Css: string, class applied to the button

function ButtonPanel(config)
{
	var panel;
	var controlsNeedToDestroy = new Array();
	var objectId;

	Ext.onReady(function()
	{
		if (!window.GlobalDynamicPageDataServicePostVariables || !window.GlobalDynamicPageDataServicePostVariables.Contains('ObjectId'))
			return;

		objectId = window.GlobalDynamicPageDataServicePostVariables.GetItem('ObjectId');
		var align = 'right';
		if (config.Align)
			align = config.Align.toLowerCase();

		panel = new Ext.Panel(
		{
			contentEl: config.ClientID,
			header: false,
			collapsed: false,
			collapsible: false,
			frame: false,
			border: false,
			style: 'margin-top: 6px; margin-bottom: 6px; margin-left: 12px; margin-right: 12px; text-align:' + align,
			stateful: false
		});

		panel.render(Ext.query('#' + config.ClientID)[0].parentNode, config.ClientID);
		eval('window.' + config.VariableName + ' = panel;');

		if (config.Buttons && config.Buttons.length > 0)
		{
			Ext.each(config.Buttons, function(buttonConfig)
			{
				var buttonCss = buttonConfig.Css ? buttonConfig.Css : "";
				if (buttonConfig.DisplayAsImage)
				{
					var imageButtonHtml = '<a href="#" class="' + buttonCss + '" onclick="OnButtonPanelButtonClick(\'' + buttonConfig.OnClientClick + '\', \'' + buttonConfig.CommandArgument + '\', ' + buttonConfig.GridSelectionRequired + ', \'' + buttonConfig.GridSelectionRequiredWarningMessage + '\')"><img src="' + buttonConfig.ImageUrl + '" title="" /></a>';
					var imageButton = Ext.DomHelper.append(panel.getEl(), imageButtonHtml);
					controlsNeedToDestroy.push(new Ext.ToolTip(
		            {
		            	showDelay: 0,
		            	dismissDelay: 0,
		            	target: imageButton,
		            	title: buttonConfig.ToolTip
		            }));
				}
				else
				{
					controlsNeedToDestroy.push(new Ext.Button
					({
						text: buttonConfig.Text,
						type: "button",
						cls: buttonConfig.Css,
						style: 'display:inline',
						renderTo: panel.getId(),
						listeners:
         				{
         					click: function(button, e)
         					{
         						OnButtonPanelButtonClick(buttonConfig.OnClientClick, buttonConfig.CommandArgument, buttonConfig.GridSelectionRequired, buttonConfig.GridSelectionRequiredWarningMessage);
         					}
         				}
					}));
				}

				Ext.DomHelper.append(panel.getEl(), "&nbsp;&nbsp;")
			});
		}
	});

	// Destroy current button panel.
	this.destroy = function()
	{
		if (controlsNeedToDestroy && controlsNeedToDestroy.length > 0)
			Ext.each(controlsNeedToDestroy, function(controlNeedToDestroy) { controlNeedToDestroy.destroy(); });

		if (panel) panel.destroy();
	}
}

function OnButtonPanelButtonClick(onClientClick, commandArgument, gridSelectionRequired, gridSelectionRequiredWarningMessage)
{
	if (!commandArgument || commandArgument.toString().trim().length == 0) return;
	if (!window.RegisteredGridViewPanelObject) return;

	var returnValue = EvalOnClientClick(onClientClick);
	if (returnValue != true) return;

	// "new" command argument indicates to popup detail panel to add a new entity.
	if (commandArgument.toString().toLowerCase() == "new")
		window.RegisteredGridViewPanelObject.ShowDetailPanelWindow(null, "New");
	else if (commandArgument.toString().toLowerCase() == "downloadexcel" && ResolvePostValuesFromQueryPanel != undefined)
	{
		var excelDownloadUrl = "DynamicPageDownloadExcel.svc" + ConstructDynamicPagePrinterUrl();
		window.open(excelDownloadUrl);
	}
	else if (commandArgument.toString().toLowerCase() == "print")
	{
		var excelDownloadUrl = "DynamicPageHtmlPrinter.svc" + ConstructDynamicPagePrinterUrl();
		window.open(excelDownloadUrl);
	}
	else
		window.RegisteredGridViewPanelObject.ShowAggregatePanelWindow(commandArgument, gridSelectionRequired, gridSelectionRequiredWarningMessage);
}

function EvalOnClientClick(onClientClick)
{
	if (onClientClick != undefined && onClientClick != null && onClientClick.trim().length > 0)
	{
		var returnValue = false;
		eval(onClientClick);
		return returnValue;
	}
	else
		return true;
}

function ConstructDynamicPagePrinterUrl()
{
	var downloadUrl = "";
	var postValues = ResolvePostValuesFromQueryPanel();
	if (postValues)
	{
		Ext.each(postValues, function(postValue)
		{
			if (postValue.Value)
			{
				if (downloadUrl.length == 0) downloadUrl += "?";
				else downloadUrl += "&";

				downloadUrl += postValue.Name + "=" + postValue.Value;
			}
		});
	}

	if (window.GlobalDynamicPageDataServicePostVariables)
	{
		Ext.each(window.GlobalDynamicPageDataServicePostVariables, function(globalVariable)
		{
			if (globalVariable.Value)
			{
				if (downloadUrl.length == 0) downloadUrl += "?";
				else downloadUrl += "&";

				downloadUrl += globalVariable.Name + "=" + globalVariable.Value;
			}
		});
	}

	return downloadUrl;
}