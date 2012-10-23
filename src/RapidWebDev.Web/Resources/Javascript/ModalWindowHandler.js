// * properties of argument "options"
// draggable, modal, resizable, height, width, windowCloseCallback

window.ModalWindowHandler = new Object();
window.ModalWindowHandler.Show = function(headerText, pageUrl, options)
{
	if (Ext == undefined || !Ext.isReady) return;

	var iframeElement = window.frameElement ? window.frameElement : document.documentElement;
	var width = iframeElement ? iframeElement.clientWidth * 0.92 : 960;
	var height = iframeElement ? iframeElement.clientHeight * 0.92 : 560;

	width = options && options.width ? options.width : width;
	height = options && options.height ? options.height : height;

	var draggable = options && options.draggable;
	var modal = options && options.modal;
	var resizable = options && options.resizable;
	var windowCloseCallback = options && options.windowCloseCallback;

	var iframeId = "ModalWindowHandler" + (new Date()).format('YmdHisu');
	var modalDialog = new Ext.Window(
	{
		draggable: draggable,
		modal: modal,
		resizable: resizable,
		closeAction: 'close',
		autoScroll: false,
		plain: true,
		title: headerText,
		html: '<iframe id="' + iframeId + '" src="' + pageUrl + '" frameborder="0" width="100%"></iframe>',
		stateful: false,
		listeners:
		{
			resize: function(component, adjWidth, adjHeight, rawWidth, rawHeight)
			{
				if (component == undefined) return;
				if (rawHeight != undefined)
					$("#" + iframeId).height(rawHeight - 34);
				else
					$("#" + iframeId).height(component.getHeight() - 34);

				$("#" + iframeId).width("100%");
			},
			beforeclose: function(panel)
			{
				var iframeElement = Ext.getDom(iframeId);
				if (iframeElement)
				{
					if (Ext.isIE) iframeElement.src = "javascript:false";
					Ext.removeNode(iframeElement);
					Ext.destroy(iframeElement);
				}
			},
			close: function(panel)
			{
				if (windowCloseCallback && windowCloseCallback.trim().length > 0)
					eval(windowCloseCallback);
			}
		}
	});

	modalDialog.render(document.body);

	if (height > 0)
		modalDialog.setHeight(height);

	if (width > 0)
		modalDialog.setWidth(width);

	modalDialog.center();
	modalDialog.show();
}