function loadPageInPopupDialog(caption, url) {
	var dialog;
	dialog = $("#PopupDialog");
	if (dialog.length == 0) {
		dialog = $('<div id="PopupDialog" style="display:none"></div>');
	}
	document.body.style.overflow = "hidden"; //Hide the main window scrollbar - don't want to have multiple scrollbars
	var dialogOptions =
			{
				title: caption,
				draggable: false,
				resizable: false,
				autoOpen: false,
				modal: true,
				width: 900,
				height: 800,
				open: function () {
					dialog.load(url);
				},
				close: function () {
					document.body.style.overflow = "auto"; //Re-enable scrolling on main window
				}
			};
	dialog.dialog(dialogOptions);
	showLoadingPanel(dialog);
	dialog.parent().css({ position: "fixed" }).end().dialog('open'); //Make sure the dialog can't move and then open it (if autoOpen = false)
	return dialog;
}

function showInformationDialog(message, title, width, height, onOk) {
	if (width == undefined) width = '500';
	if (height == undefined) height = '500';
	$('<div><table class="default" style="width:100%;table-layout:fixed;word-wrap:break-word;"><tr><td>' + message + '</td></tr></table></div>')
	.dialog(
	{
		modal: true,
		width: width,
		height: height,
		draggable: false,
		buttons:
		{
			'Ok': function () {
				$(this).dialog('close');
				if (onOk) { onOk(); }
			}
		},
		resizable: false,
		title: title,
		open: function (event, ui) {
			$(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
		}
	});
}

function showYesNoDialog(message, title, onYes) {
	$('<div></div>')
	.html(message)
	.dialog(
	{
		modal: true,
		draggable: false,
		buttons:
		{
			'Yes': function () {
				$(this).dialog('close');
				if (onYes) { onYes(); }
			},
			'No': function () {
				$(this).dialog('close');
			}
		},
		resizable: false,
		title: title,
		open: function (event, ui) {
			$(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
		}
	});
}

function showLoadingPanel(container, text) {
	if (text == undefined) text = " Loading...";
	var imgSrc = '/Content/images/ajax-loader.gif';
	container.html("<div><img src='" + imgSrc + "' alt='" + text + "'>" + text + "</div>");
}

function callPageMethod(pageName, methodName, params, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: pageName + "/" + methodName,
		data: params,
		beforeSend: function (xhr) {
			xhr.setRequestHeader("Content-type", "application/json; charset=utf-8");
		},
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: successCallback,
		error: errorCallback
	});
}