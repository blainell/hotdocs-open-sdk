/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

(function($)
{
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