// * Parameter - regions
// regions.HeaderPanelId: string - head region id.
// regions.FooterPanelId: string - foot region id.
// regions.NavigationItems: Array - array of navigation item object. The item object has 3 properties, Id:string, Title:string and IconClass:string.
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

		var navigationItems = new Array();
		if (regions.NavigationItems && regions.NavigationItems.length > 0)
		{
			for (var i = 0; i < regions.NavigationItems.length; i++)
			{
				var navigationItem = regions.NavigationItems[i];
				navigationItems.push({
					animCollapse: false,
					bufferResize: true,
					contentEl: navigationItem.Id,
					title: navigationItem.Title,
					border: false,
					iconCls: navigationItem.IconClass
				});
			}
		}

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
				{
					animCollapse: false,
					bufferResize: true,
					region: 'west',
					id: 'west-panel',
					title: regions.NavigationBarTitle,
					split: true,
					width: 200,
					minSize: 175,
					maxSize: 300,
					collapsible: true,
					collapsed: false,
					margins: '0 0 0 5',
					layout: 'accordion',
					layoutConfig: { animate: false },
					items: navigationItems
				},
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
}

function Redirect(path)
{
    window.location.href = path;
}