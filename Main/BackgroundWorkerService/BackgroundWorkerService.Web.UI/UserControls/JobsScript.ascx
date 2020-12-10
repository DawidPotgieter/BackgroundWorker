<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobsScript.ascx.cs" Inherits="WebUI.UserControls.JobsScript" %>

<script type="text/javascript">
	function toggleHistory(img, rowId) {
		$('#' + rowId).toggle();
		if ($(img).attr("src").indexOf("Down_Small") == -1) {
			$(img).attr("src", "../content/images/Down_Small.png");
		}
		else {
			$(img).attr("src", "../content/images/Up_Small.png");
		}
	}

	function queryRunJob(jobId, applicationName) {
		showYesNoDialog("Are you sure you want to run this job now?", "Confirm Run Job", function () {
			callPageMethod(
				"Default.aspx",
				"RunJob",
				JSON.stringify({ 'jobId': jobId }),
				function (msg, status) {
					if (msg.d.length > 0) {
						showInformationDialog(msg.d, "Run Job Result", '500', '200', window["refreshJobsList" + applicationName]);
					}
				},
				function (xhr, msg, e) {
					showInformationDialog(xhr.responseText, "Ajax Error");
				});
		});
	}

	function queryDeleteJob(jobId, applicationName) {
		showYesNoDialog("Are you sure you want to delete this job?", "Confirm Delete Job", function () {
			callPageMethod(
				"Default.aspx",
				"DeleteJob",
				JSON.stringify({ 'jobId': jobId }),
				function (msg, status) {
					if (msg.d.length > 0) {
						showInformationDialog(msg.d, "Delete Job Result", '500', '200', window["refreshJobsList" + applicationName]);
					}
				},
				function (xhr, msg, e) {
					showInformationDialog(xhr.responseText, "Ajax Error");
				});
		});
	}
</script>
