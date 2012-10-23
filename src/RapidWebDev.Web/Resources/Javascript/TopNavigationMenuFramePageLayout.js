// * Parameter - regions
// regions.HeaderPanelId: string - head region id.
// regions.MenuPanelId: string - menu region id.
// regions.FooterPanelId: string - foot region id.
// regions.NavigationItems: Array - array of navigation item object. The item object has properties as following,
// @id:string
// @text:string
// @iconCls:string
// @menu: Array
// @url: string
// @clientSideCommand: string 
// regions.NavigationBarTitle: string - navigation bar title

// * Parameter - options
// options.EnableMultipleTabs: boolean - whether allows user to open multiple business page in frame page.
// options.MaximumTabs: integer - Maximum tabs of content page in the frame page.
// options.DefaultTab: object - the default tab displays when user logs onto system.
// options.DefaultTab.Url: string - the page displayed in the default tab when user logs onto system.
// options.DefaultTab.Title: string - the default tab title.
// options.DefaultTab.Id: string - the default tab id.

function FramePageClass(regions, options)
{
	if (!options) alert("The argument 'options' of FramePageClass cannot be null.");
	if (!options.DefaultTab) alert("The argument 'options.DefaultTab' of FramePageClass cannot be null.");
	
	var openedNavigationItemIdArray = new Array();

	Ext.onReady(function()
	{
		// initialize ExtJs cookie state and quick tips
		Ext.state.Manager.setProvider(new Ext.state.CookieProvider({ path: '/FramePage/' }));
		Ext.QuickTips.init();

		if (!regions.NavigationItems || regions.NavigationItems == null)
			regions.NavigationItems = new Array();

		setClickEventHandler(regions.NavigationItems);
		regions.NavigationItems = insertSeparatorBetweenMenuItems(regions.NavigationItems);

		if (window.LayoutViewPort)
			window.LayoutViewPort.destroy();

		window.LayoutViewPort = new Ext.Viewport(
		{
			id: 'LayoutViewPort',
			layout: 'border',
			items:
			[
				new Ext.BoxComponent(
				{
					region: 'north',
					el: regions.HeaderPanelId
				}),
				new Ext.Toolbar(
				{
					renderTo: regions.HeaderPanelId,
					hideBorders: true,
					hideMode: "display",
					items: regions.NavigationItems
				}),
				new Ext.TabPanel(
				{
					id: 'openingTabPanel',
					region: 'center',
					deferredRender: true,
					activeTab: 0,
					margins: '0 0 0 0',
					stateful: false
				})
			]
		});

		this.AddTab(options.DefaultTab);

		// force to sync size after added the default tab for fixing a style issue.
		window.LayoutViewPort.syncSize();

		// add the border wraps to the site header
		$("#" + regions.HeaderPanelId + " table.headtemplate").css("border", "1px solid #a9bfd3");

		// unload all iframes inner of tabs when the window is unloaded.
		$(window).unload(function()
		{
			var iframeSearcher = $("iframe.tabInnerIFrame");
			iframeSearcher.each(function(index, iframe)
			{
				if (Ext.isIE) iframe.src = "javascript:false";
				Ext.removeNode(iframe);
				Ext.destroy(iframe);
			});
		});
	}, this);

	this.AddTab = function(navigationItem)
	{
		var tabId = "navigationTab_" + navigationItem.Id;
		var iframeId = "iFrame_" + navigationItem.Id;
		var tabTitle = navigationItem.Title;
		var openingTabPanel = Ext.getCmp("openingTabPanel");
		if (!options.EnableMultipleTabs && openingTabPanel.items.getCount() > 0)
		{
			for (var i = openingTabPanel.items.getCount() - 1; i >= 0; i--)
			{
				var tabComponent = openingTabPanel.items.get(i);
				var tabIdToRemove = tabComponent.getId();
				if (tabIdToRemove != tabId)
				{
					openedNavigationItemIdArray.remove(tabIdToRemove);
					openingTabPanel.remove(tabComponent, true);
				}
			}
		}

		var doesAddedTabExist = openedNavigationItemIdArray.indexOf(tabId) > -1;
		if (options.EnableMultipleTabs
			&& openingTabPanel.items.getCount() >= options.MaximumTabs
			&& openingTabPanel.items.getCount() > 0
			&& !doesAddedTabExist)
		{
			var tabComponent = openingTabPanel.items.get(0);
			var tabIdToRemove = tabComponent.getId();
			if (tabIdToRemove != tabId)
			{
				openedNavigationItemIdArray.remove(tabIdToRemove);
				openingTabPanel.remove(tabComponent, true);
			}
		}

		// add a stamp parameter with random value to avoid the page is cached locally.
		var iFramePageUrl = navigationItem.Url + (navigationItem.Url.indexOf('?') > -1 ? '&' : '?') + 'stamp=' + (new Date()).format('YmdHisu');
		if (openedNavigationItemIdArray.indexOf(tabId) == -1)
		{
			var tabPanelHtml = '<iframe id="' + iframeId + '" tabid="' + tabId + '" src="' + iFramePageUrl + '" frameborder="0" width="100%" class="tabInnerIFrame" scrolling="no"></iframe>';
			openedNavigationItemIdArray.push(tabId);
			var createdComponent = openingTabPanel.add(
			{
				id: tabId,
				title: tabTitle,
				closable: options.EnableMultipleTabs,
				autoScroll: false,
				header: false,
				html: tabPanelHtml,
				listeners:
				{
					resize: function(component, adjWidth, adjHeight, rawWidth, rawHeight)
					{
						if (component == undefined) return;

						if (rawHeight != undefined)
						{
							$("#" + iframeId).height(rawHeight);
						}
						else
						{
							var height = component.getHeight();
							$("#" + iframeId).height(height);
						}

						$("#" + iframeId).width("100%");
					},
					beforedestroy: function(component)
					{
						var iframeElement = Ext.getDom(iframeId);
						if (iframeElement)
						{
							var tabIdToRemove = component.getId();
							openedNavigationItemIdArray = openedNavigationItemIdArray.remove(tabIdToRemove);
							if (Ext.isIE) iframeElement.src = "javascript:false";
							Ext.removeNode(iframeElement);
							Ext.destroy(iframeElement);
						}

						return true;
					}
				}
			});

			createdComponent.fireEvent("resize");
		}

		openingTabPanel.activate(tabId);
	}

	function setClickEventHandler(menuItems)
	{
		if(menuItems == null || menuItems.length == 0) return;
		for(var i=0; i<menuItems.length; i++)
		{
			var menuItem = menuItems[i];
			
			var hasUrl = false;
			if(menuItem.url != null && menuItem.url.length > 0)
				hasUrl = true;
			
			var hasClientSideCommand = false;
			if(menuItem.clientSideCommand != null && menuItem.clientSideCommand.length > 0)
				hasClientSideCommand = true;
			
			if(hasUrl || hasClientSideCommand)
			{
				menuItem.listeners =
				{
					click: function(thisMenuItem, e)
					{
						if (thisMenuItem.initialConfig.clientSideCommand != null)
							eval(thisMenuItem.initialConfig.clientSideCommand);
						if (thisMenuItem.initialConfig.url != null)
							window.FramePageObj.AddTab({ Id: thisMenuItem.initialConfig.id, Url: thisMenuItem.initialConfig.url, Title: thisMenuItem.initialConfig.text });
					}
				};
			}
			
			if(menuItem.menu != null)
				setClickEventHandler(menuItem.menu);
		}
	}

	function insertSeparatorBetweenMenuItems(menuItems)
	{
		var results = new Array();
		for (var i = 0; i < menuItems.length; i++)
		{
			var menuItem = menuItems[i];
			results.push(menuItem);
			if (menuItem != "-" && i < menuItems.length - 1 && menuItems[i + 1] != "-")
				results.push("-");
		}

		return results;
	}
}

function Redirect(path)
{
    window.location.href = path;
}