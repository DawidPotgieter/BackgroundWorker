<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Alerts.aspx.cs" Inherits="WebUI.Alerts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">
	$(function () {
		$('#btnDeleteAllAlerts').button();
	});

	function refreshAlertsList() {
		reloadAlertNotifier();
		openDialog.load('Alerts.aspx');
	}

	function toggleMessage(img, rowId) {
		$('#' + rowId).toggle();
		if ($(img).attr("src").indexOf("Down_Small") == -1)	{
			$(img).attr("src", "../content/images/Down_Small.png");
		}
		else {
			$(img).attr("src", "../content/images/Up_Small.png");
		}
	}

	function queryDeleteAlert(alertId) {
		showYesNoDialog("Are you sure you want to delete this alert?", "Confirm Delete Alert", function () {
			callPageMethod(
				"Default.aspx",
				"DeleteAlert",
				JSON.stringify({ 'alertId': alertId }),
				function (msg, status) {
					if (msg.d.length > 0) {
						showInformationDialog(msg.d, "Delete Alert Result", '500', '200', refreshAlertsList);
					}
				},
				function (xhr, msg, e) {
					showInformationDialog(xhr.responseText, "Ajax Error");
				});
			});
	}

	function queryDeleteAllAlerts() {
		showYesNoDialog("Are you sure you want to delete all alerts?", "Confirm Delete All Alerts", function () {
			callPageMethod(
				"Default.aspx",
				"DeleteAllAlerts",
				null,
				function (msg, status) {
					if (msg.d.length > 0) {
						showInformationDialog(msg.d, "Delete All Alerts Result", '500', '200', function () { openDialog.dialog('close'); reloadAlertNotifier(); });
					}
				},
				function (xhr, msg, e) {
					showInformationDialog(xhr.responseText, "Ajax Error");
				});
		});
	}
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
		<div>
			<asp:ListView ID="AlertsView" runat="server" onitemdatabound="AlertsView_ItemDataBound">
				<LayoutTemplate>
					<table class="alertstable">
						<tr>
							<th style="width: 60px">
								Id
							</th>
							<th>
								Job Id
							</th>
							<th>
								Job Name
							</th>
							<th>
								History Id
							</th>
							<th style="width:30px;min-width:30px"></th>
							<th style="width:30px;min-width:30px"></th>
						</tr>
						<tr runat="server" id="itemPlaceHolder" />
					</table>
				</LayoutTemplate>
				<ItemTemplate>
					<tr class="alertrow">
						<td><%# (long)Eval("Id") %></td>
						<td><span style="cursor:pointer;text-decoration:underline" onclick="openDialog.dialog('close');openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("JobId") %>'); return false;"><%# (long)Eval("JobId") %></span></td>
						<td><span><%# (string)Eval("JobName")%></span></td>
						<td><span style="cursor:pointer;text-decoration:underline" onclick="openDialog.dialog('close');openDialog = loadPageInPopupDialog('Job History','JobHistory.aspx?Id=<%# (long?)Eval("JobHistoryId") %>'); return false;"><%# (long?)Eval("JobHistoryId") %></span></td>
						<td><img src="../content/images/Down_Small.png" alt="Message" onclick="toggleMessage(this, 'messageRow_<%# (long)Eval("Id") %>'); return false;" style="cursor:pointer" title="Message" /></td>
						<td><asp:Image runat ="server" ID="btnDeleteAlertImage" ImageUrl="~/content/images/Delete_Small.png" style="cursor:pointer" ToolTip="Delete" /></td>
					</tr>
					<tr id='messageRow_<%# (long)Eval("Id") %>' class="alertmessagerow" style="display:none">
						<td colspan="6" style="color:Red"><%# HttpUtility.HtmlEncode((string)Eval("Message"))%></td>
					</tr>
				</ItemTemplate>
				<EmptyDataTemplate>
					<table class="alertstable" style="border-bottom:1px solid white">
						<tr><td>No alerts to display.</td></tr>
					</table>
				</EmptyDataTemplate>
			</asp:ListView>
			<table style="width:100%">
				<tr>
					<td></td>
					<td style="text-align:right;padding-right:0"><input id="btnDeleteAllAlerts" type="button" value="Delete All" onclick="queryDeleteAllAlerts();" /></td>
				</tr>
			</table>				
		</div>
</asp:Content>
