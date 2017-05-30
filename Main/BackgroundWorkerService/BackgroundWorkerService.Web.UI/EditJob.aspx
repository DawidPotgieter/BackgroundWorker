<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditJob.aspx.cs" Inherits="WebUI.Job" MasterPageFile="~/Site.master" ClientIDMode="Static" %>
<%@ Register src="UserControls/CalendarSchedule.ascx" tagname="CalendarSchedule" tagprefix="BWS" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<script type="text/javascript">
		var scheduleSelected = '<%= ScheduleGroupToSelect %>';

		$(function () {
			$("#txtAbsoluteTimeout").timepicker({ timeFormat: 'hh:mm:ss', showSecond: true });
			$('#btnUpdate').button();
			$('#cmbSuppressHistory').selectmenu();
			$('#cmbDeleteWhenDone').selectmenu();
			$('#cmbQueueId').selectmenu();
			$('#cmbStatus').selectmenu();
			$('#scheduleSelect').buttonset();
		});

		function queryUpdateJob() {
			showYesNoDialog("Are you sure you want to update this job now?", "Confirm Update", function () {
				var sc = null;
				if (scheduleSelected == 'runSchedule') {
					var days = new Array();
					$('#DaysOfTheWeek>input').each(function (index) {
						if ($(this).attr("checked") == "checked")	{
							days.push($(this).attr("id"));
						}
					});

					sc =
					{
						DaysOfWeek: days,
						StartDailyAt: $('#StartDailyAt').val(),
						RepeatInterval: $('#RepeatInterval').val(),
						StartDateTime: $('#StartDateTime').val()
					};
				}
				var params =
				{
					jobId: <%= JobId %>,
					uniqueId: $('#txtUniqueId').val(),
					name: $('#txtName').val(),
					description: $('#txtDescription').val(),
					data: $('#txtData').val(),
					metaData: $('#txtMetaData').val(),
					statusId: $('#cmbStatus').val(),
					absoluteTimeout: $('#txtAbsoluteTimeout').val(),
					queueId: $('#cmbQueueId').val(),
					application: $('#txtApplication').val(),
					group: $('#txtGroup').val(),
					suppressHistory: $('#cmbSuppressHistory').val(),
					deleteWhenDone: $('#cmbDeleteWhenDone').val(),
					schedule: sc
				};
				callPageMethod(
					"EditJob.aspx",
					"UpdateJob",
					JSON.stringify(params),
					function (msg, status) {
						if (msg.d.length > 0) {
							showInformationDialog(msg.d, "Update Job Error", '500', '500');
						}
						else {
							openDialog.dialog('close');
							setTimeout("refreshJobsList()", 1000);
						}
					},
					function (xhr, msg, e) {
						showInformationDialog(xhr.responseText, "Ajax Error");
					});
			}, true);
		}

		function scheduleGroupClick(radioButton) {
			scheduleSelected = $(radioButton).attr("id");
			$("#scheduleContainer").hide();
			if ($(radioButton).attr("id") == "runSchedule") {
				$("#scheduleContainer").show();
			}
		}
	</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<table style="width:100%;table-layout:fixed;word-wrap:break-word;">
		<tr>
			<td style="width:140px">Id :</td>
			<td><asp:Literal ID="lblId" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Unique Id :</td>
			<td><asp:TextBox ID="txtUniqueId" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Name :</td>
			<td><asp:TextBox ID="txtName" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Description :</td>
			<td><asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="5" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Data :</td>
			<td><asp:TextBox ID="txtData" runat="server" TextMode="MultiLine" Rows="5" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>MetaData :</td>
			<td><asp:TextBox ID="txtMetaData" runat="server" TextMode="MultiLine" Rows="5" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Last Error :</td>
			<td style="color:Red"><asp:Literal ID="lblLastErrorMessage" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Worker Instance :</td>
			<td><asp:Literal ID="lblInstance" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Created :</td>
			<td><asp:Literal ID="lblCreatedDate" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Last Start :</td>
			<td><asp:Literal ID="lblLastStartTime" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Last End :</td>
			<td><asp:Literal ID="lblLastEndTime" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Next Start :</td>
			<td><asp:Literal ID="lblNextStart" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Absolute Timeout :</td>
			<td><asp:TextBox ID="txtAbsoluteTimeout" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Queue :</td>
			<td>
				<asp:DropDownList ID="cmbQueueId" runat="server" Width="100%">
					<asp:ListItem Value="0" Text="Thread (Default - No Job Timeout)"></asp:ListItem>
					<asp:ListItem Value="1" Text="ThreadPool (No Job Timeout, No Shutdown Timeout)"></asp:ListItem>
					<asp:ListItem Value="2" Text="Timed Thread (Supports Job Timeout & Shutdown Timeout)"></asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td>Status :</td>
			<td>
				<table>
					<tr>
						<td><asp:Literal ID="lblStatus" runat="server"></asp:Literal></td>
						<td>>>>></td>
						<td>
							<asp:DropDownList ID="cmbStatus" runat="server" Width="150px">
								<asp:ListItem Value="" Text="No Change"></asp:ListItem>
								<asp:ListItem Value="0" Text="Ready"></asp:ListItem>
								<asp:ListItem Value="4" Text="Scheduled"></asp:ListItem>
								<asp:ListItem Value="32" Text="Pending"></asp:ListItem>
								<asp:ListItem Value="-32" Text="Deleted"></asp:ListItem>
							</asp:DropDownList>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>Job Type :</td>
			<td><asp:Literal ID="lblJobType" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Application :</td>
			<td><asp:TextBox ID="txtApplication" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Group :</td>
			<td><asp:TextBox ID="txtGroup" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Suppress History :</td>
			<td>
				<asp:DropDownList ID="cmbSuppressHistory" runat="server" Width="100%">
					<asp:ListItem Value="False" Text="No"></asp:ListItem>
					<asp:ListItem Value="True" Text="Yes"></asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td>Delete When Done :</td>
			<td>
				<asp:DropDownList ID="cmbDeleteWhenDone" runat="server" Width="100%">
					<asp:ListItem Value="False" Text="No"></asp:ListItem>
					<asp:ListItem Value="True" Text="Yes"></asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
	</table>
	<div style="width:100%;padding-top : 15px">
		<div id="scheduleSelect">
			<asp:RadioButton ID="runOnce" runat="server" GroupName="scheduleGroup" onclick="scheduleGroupClick(this);return false;" /><label for="runOnce">Run Once</label>
			<asp:RadioButton ID="runSchedule" runat="server" GroupName="scheduleGroup" onclick="scheduleGroupClick(this);return false;" /><label for="runSchedule">Schedule</label>
		</div>
	</div>
	<div style="width:100%" runat="server" id="scheduleContainer">
		<p style="width:100%" class="heading">Schedule</p>
		<BWS:CalendarSchedule ID="CalendarScheduleControl" runat="server" />
	</div>
	<div style="width:100%;padding-top:15px;padding-bottom:15px;text-align:right">
		<input id="btnUpdate" type="button" value="Update" onclick="queryUpdateJob();" />
	</div>
</asp:Content>