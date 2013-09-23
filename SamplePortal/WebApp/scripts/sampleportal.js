/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

(function($)
{
	jQuery(document).ready(function()
	{
		return jQuery('#txtSearch')
		.on('keydown', function(e)
		{
			var bOk = true;
			switch (e.which)
			{
				case 13: // Key.Enter
					// Execute the search by clicking the search button.
					var searchButton = jQuery('#btnSearch');
					if (searchButton.length > 0)
						searchButton[0].click();
					e.preventDefault();
					break;
				case 27: // Key.Escape
					// Clear the search by clicking the clear button.
					var clearButton = jQuery('#btnSearchClear');
					if (clearButton.length > 0)
						clearButton[0].click();
					e.preventDefault();
					break;
				case 188: // "," or ">" character
					if (e.shiftKey)
						bOk = false;
				    break;
			}
			return bOk;
		});

		return jQuery('#txtSearch')
		.on('keypress', function (e) {
			var bOk = true;
			switch (e.which) {
				case 188: // "," or ">" character
					if (e.shiftKey)
						bOk = false;
					break;
                default:
                    break;
			}
			return bOk;
		});
	});

	// This function is used on the "Select Answers" page to enable/disable 
	// the file control depending on the user's selection.
	$.OnSelChange = function()
	{
		var fileCtrl = document.getElementById("fileUpload");
		if (fileCtrl)
		{
			var uploadBtn = document.getElementById("rbUpload");
			if (uploadBtn)
				fileCtrl.disabled = !uploadBtn.checked;
		}
	}

	window.HDSamplePortal = $;

}(typeof HDSamplePortal === "undefined" ? {} : HDSamplePortal));
