/* Copyright (c) 2013, HotDocs Limited
   Use, modification and redistribution of this source is subject
   to the New BSD License as set out in LICENSE.TXT. */

(function($)
{
	var snapshotCookieName = 'spEmbeddedSnapshot';

	jQuery(document).ready(function()
	{
		var snapshot = GetCookieValue(snapshotCookieName);
		jQuery('#SnapshotField').val(snapshot);
		jQuery('#ResumeSessionButton')[snapshot == null ? 'hide' : 'show']();

		SetSessionID();
		LoadEmbeddedInterview(jQuery('#tplGrid').length == 0 && jQuery('#ansGrid').length == 0);

		jQuery('#ResumeSessionButton').on('click', function()
		{
			DeleteCookie(snapshotCookieName);
			jQuery('#ResumeSessionButton').hide();
			jQuery('#tplGrid').hide();
			jQuery('#ansGrid').hide();
		});

	});

	window.HDInterviewOptions = {
		LeaveWarning: false,
		OnInit: function()
		{
			HD$.AddCustomToolbarButton(TakeSnapshot, null, null, null, "Take a snapshot of the interview at the current position.", null, null, null, null, 0, "Save Snapshot", null);
		}
	}

	function GetCookieValue(name)
	{
		var cookies = document.cookie.split(';');
		for (var i = 0; i < cookies.length; i++)
		{
			var cookie = cookies[i].trim();
			var cookieName = cookie.substr(0, cookie.indexOf('='));

			if (cookieName == name)
				return cookie.substr(cookieName.length + 1);
		}
		return null;
	}

	function DeleteCookie(name)
	{
		document.cookie = name + '=;expires=Thu, 01-Jan-1970 00:00:01 GMT';
	}

	function TakeSnapshot()
	{
		HD$.GetSnapshot(function(s)
		{
			document.cookie = snapshotCookieName + '=' + s + '; path=/';

			// Since we have now successfully saved the snapshot, we can show the resume button in the UI.
			jQuery('#ResumeSessionButton').show();
		});
	}

	function LoadEmbeddedInterview(bShow)
	{
		if (bShow)
		{
			jQuery('#TemplateInterview').show();

			if (CloudServiceAddress != "")
				HD$.CloudServicesAddress = CloudServiceAddress;

			HD$.CreateInterviewFrame('TemplateInterview', CloudSessionID, function()
			{
				// This is called when the frame is ready.
			});
		}
	}

}(typeof HDSamplePortal === "undefined" ? {} : HDSamplePortal));

