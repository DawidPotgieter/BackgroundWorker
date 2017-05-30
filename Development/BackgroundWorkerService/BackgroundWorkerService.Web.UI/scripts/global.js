$.ajaxSetup({
	// Disable caching of AJAX responses
	cache: false
});

function loadPageInPopupDialog(caption, url, ignoreScrollbarsOnClose) {
	var dialog;
	dialog = $("#PopupDialog");
	if (dialog.length == 0) {
		dialog = $('<div id="PopupDialog" style="display:none"></div>');
	}
	var dialogOptions =
			{
				title: caption,
				draggable: false,
				resizable: false,
				autoOpen: false,
				modal: true,
				zIndex: $.maxZIndex(),
				width: 900,
				height: 800,
				open: function () {
					showLoadingPanel(dialog);
					dialog.load(url);
					document.body.style.overflow = "hidden"; //Hide the main window scrollbar - don't want to have multiple scrollbars
				},
				close: function () {
					$(this).dialog('destroy');
					if (!ignoreScrollbarsOnClose) {
						document.body.style.overflow = "auto"; //Re-enable scrolling on main window
					}
				}
			};
	dialog.dialog(dialogOptions);
	dialog.parent().css({ position: "absolute" }).end().dialog('open'); //Make sure the dialog can't move and then open it (if autoOpen = false)
	return dialog;
}

function showInformationDialog(message, title, width, height, onOk) {
  var dialogContent = '<table class="default" style="width:100%;table-layout:fixed;word-wrap:break-word;"><tr><td>' + message + '</td></tr></table>';
	dialog = $("#InformationPopupDialog");
	if (dialog.length == 0) {
		dialog = $('<div id="InformationPopupDialog" style="display:none"></div>');
	}
	if (width == undefined) width = '500';
	if (height == undefined) height = '500';
	var dialogOptions =
	{
		modal: true,
		width: width,
		height: height,
		draggable: false,
		zIndex: $.maxZIndex(),
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
			document.body.style.overflow = "hidden"; //Hide the main window scrollbar - don't want to have multiple scrollbars
		},
		close: function () {
			$(this).dialog('destroy');
			document.body.style.overflow = "auto"; //Re-enable scrolling on main window
		}
	};
	dialog.html(dialogContent).dialog(dialogOptions);
	dialog.parent().css({ position: "absolute" }).end().dialog('open'); //Make sure the dialog can't move and then open it (if autoOpen = false)
	return dialog;
}

function showYesNoDialog(message, title, onYes, ignoreScrollbarsOnClose) {
	dialog = $("#YesNoPopupDialog");
	if (dialog.length == 0) {
		dialog = $('<div id="YesNoPopupDialog" style="display:none"></div>');
	}
	var dialogOptions =
	{
		modal: true,
		draggable: false,
		zIndex: $.maxZIndex(),
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
			document.body.style.overflow = "hidden"; //Hide the main window scrollbar - don't want to have multiple scrollbars
		},
		close: function () {
			$(this).dialog('destroy');
			if (!ignoreScrollbarsOnClose) {
				document.body.style.overflow = "auto"; //Re-enable scrolling on main window
			}
		}
	};
	dialog.html(message).dialog(dialogOptions);
	dialog.parent().css({ position: "absolute" }).end().dialog('open'); //Make sure the dialog can't move and then open it (if autoOpen = false)
	return dialog;
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

$.maxZIndex = $.fn.maxZIndex = function (opt) {
	/// <summary>
	/// Returns the max zOrder in the document (no parameter)
	/// Sets max zOrder by passing a non-zero number
	/// which gets added to the highest zOrder.
	/// </summary>    
	/// <param name="opt" type="object">
	/// inc: increment value, 
	/// group: selector for zIndex elements to find max for
	/// </param>
	/// <returns type="jQuery" />
	var def = { inc: 10, group: "*" };
	$.extend(def, opt);
	var zmax = 0;
	$(def.group).each(function () {
		var cur = parseInt($(this).css('z-index'));
		zmax = cur > zmax ? cur : zmax;
	});
	if (!this.jquery)
		return zmax;

	return this.each(function () {
		zmax += def.inc;
		$(this).css("z-index", zmax);
	});
}