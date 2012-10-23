if (!window.RWD)
{
	window.RWD = new Object();
	window.RWD.MessageBox = new Object();
	window.RWD.MessageBox.Error = function(dialogTitle, message)
	{
		Ext.MessageBox.show({
			title: dialogTitle,
			msg: message,
			buttons: Ext.MessageBox.OK,
			icon: Ext.MessageBox.ERROR,
			minWidth: 240
		});
	}

	window.RWD.MessageBox.Warn = function(dialogTitle, message)
	{
		Ext.MessageBox.show({
			title: dialogTitle,
			msg: message,
			buttons: Ext.MessageBox.OK,
			icon: Ext.MessageBox.WARNING,
			minWidth: 240
		});
	}

	window.RWD.MessageBox.Info = function(dialogTitle, message)
	{
		Ext.MessageBox.show({
			title: dialogTitle,
			msg: message,
			buttons: Ext.MessageBox.OK,
			icon: Ext.MessageBox.INFO,
			minWidth: 240
		});
	}
}