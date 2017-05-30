<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AlertNotifier.ascx.cs" Inherits="WebUI.UserControls.AlertNotifier" ClientIDMode="Static" %>
<script type="text/javascript">
	function showAlerts() {
		openDialog = loadPageInPopupDialog('Alerts', 'Alerts.aspx');
	}
</script>
<div runat="server" id="alertsNotifier" class="smallText" onclick="showAlerts();return false;">
</div>