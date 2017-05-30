<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateJob.aspx.cs" Inherits="WebUI.CreateJob" MasterPageFile="~/Site.master" %>

<%@ Register src="UserControls/CalendarSchedule.ascx" tagname="CalendarSchedule" tagprefix="BWS" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<script type="text/javascript">
		var scheduleSelected = 'runOnce';

		$(function () {
			$('#jobType').selectmenu();
			$('#queueId').selectmenu();
			$('#deleteWhenDone').selectmenu();
			$('#createHistory').selectmenu();
			$('#contractType').selectmenu();
			$('#securityMode').selectmenu();
			$('#credentialType').selectmenu();
			$('#ignoreCertificateErrors').selectmenu();
			$('#scheduleSelect').buttonset();
			$('#createButton').button();
			showJobDataSections('');
		});

		function showJobDataSections(selectedValue) {
			$('#RemoteCallbackJobData').hide();
			$('#SendMailJobData').hide();
			$('#GenericJobData').hide();
			$('#JobDataContainer').hide();
			$('#JobMetaDataContainer').hide();
			switch (selectedValue) {
				case "BackgroundWorkerService.Jobs.BasicHttpSoapCallbackJob, BackgroundWorkerService.Jobs":
					$('#RemoteCallbackJobData').show();
					$('#JobDataContainer').show();
					$('#JobMetaDataContainer').show();
					break;
				case "BackgroundWorkerService.Jobs.SendMailJob, BackgroundWorkerService.Jobs":
					$('#JobDataContainer').show();
					$('#SendMailJobData').show();
					break;
				case "":
					$('#JobDataContainer').show();
					$('#JobMetaDataContainer').show();
					$('#GenericJobData').show();
				default:
					break;
			}
		}

		function showHideMethodName(selectedValue) {
			switch (selectedValue) {
				case "Basic":
					$('#methodNameRow').hide();
					break;
				case "Composite":
					$('#methodNameRow').show();
					break;
			}
		}

		function scheduleGroupClick(radioButton) {
			scheduleSelected = $(radioButton).attr("id");
			$("#scheduleContainer").hide();
			if ($(radioButton).attr("id") == "runSchedule") {
				$("#scheduleContainer").show();
			}
		}

		function createJob() {
			pd = new Array();
			pd.push(
			{
				Key: 'contractType',
				Value: $('#contractType').val()
			});
			pd.push(
			{
				Key: 'methodName',
				Value: $('#methodName').val()
			});
			pd.push(
			{
				Key: 'callbackUrl',
				Value: $('#callbackUrl').val()
			});
			pd.push(
			{
				Key: 'securityMode',
				Value: $('#securityMode').val()
			});
			pd.push(
			{
				Key: 'credentialType',
				Value: $('#credentialType').val()
			});
			pd.push(
			{
				Key: 'domain',
				Value: $('#domain').val()
			});
			pd.push(
			{
				Key: 'username',
				Value: $('#username').val()
			});
			pd.push(
			{
				Key: 'password',
				Value: $('#password').val()
			});
			pd.push(
			{
				Key: 'ignoreCertificateErrors',
				Value: $('#ignoreCertificateErrors').val()
			});

			pd.push(
			{
				Key: 'sendTo',
				Value: $('#sendTo').val()
			});
			pd.push(
			{
				Key: 'sendFrom',
				Value: $('#sendFrom').val()
			});
			pd.push(
			{
				Key: 'sendSubject',
				Value: $('#sendSubject').val()
			});
			pd.push(
			{
				Key: 'emailBody',
				Value: $('#emailBody').val()
			});

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
					RepeatInterval: $('#RepeatInterval').val()
				};
			}

			var jobType = $('#jobType').val();
			if (jobType == '') {
				jobType = $('#jobTypeName').val();
			}

			var p =
			{
				name : $('#jobName').val(),
				description: $('#description').val(),
				data: $('#data').val(),
				metaData:  $('#metaData').val(),
				jobType: jobType,
				absoluteTimeout: $('#absoluteTimeout').val(),
				queueId: $('#queueId').val(),
				application: $('#application').val(),
				group: $('#group').val(),
				suppressHistory: false,
				deleteWhenDone: false,
				additionalData: pd,
				schedule: sc
			};
			callPageMethod(
				"CreateJob.aspx",
				"Create",
				JSON.stringify(p),
				function (msg, status) {
					if (msg.d.length > 0) {
						showInformationDialog(msg.d, "Validation Error");
					}
					else {
						openDialog.dialog('close');
						refreshJobsList();
					}
				},
				function (xhr, msg, e) {
					showInformationDialog(msg.d, "Ajax Error");
				});
		}
	</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<table class="default" style="width:100%;">
		<tr>
			<td class="heading" style="vertical-align:middle;width: 170px; min-width:170px; max-width:170px">
				Job Type :
			</td>
			<td>
				<select id="jobType" onchange="showJobDataSections(this.options[this.selectedIndex].value)">
					<option value="">Generic</option>
					<option value="BackgroundWorkerService.Jobs.BasicHttpSoapCallbackJob, BackgroundWorkerService.Jobs">Remote Callback</option>
					<option value="BackgroundWorkerService.Jobs.SendMailJob, BackgroundWorkerService.Jobs">Send Email</option>
				</select>
			</td>
		</tr>
		<tr>
			<td class="heading">Job Name :</td>
			<td><div class="textBoxContainer"><input type="text" id="jobName" /></div></td>
		</tr>
		<tr>
			<td class="heading">Description :</td>
			<td><div class="textBoxContainer"><textarea id="description" rows="4" cols="100" /></div></td>
		</tr>
		<tr>
			<td class="heading" style="vertical-align:middle">Queue :</td>
			<td>
				<select id="queueId">
					<option value="0">Thread (Default - No Job Timeout)</option>
					<option value="1">ThreadPool (No Job Timeout, No Shutdown Timeout)</option>
					<option value="2">Timed Thread (Supports Job Timeout & Shutdown Timeout)</option>
				</select>
			</td>
		</tr>
		<tr>
			<td class="heading" style="vertical-align:middle">
				Delete When Done :
			</td>
			<td>
				<select id="deleteWhenDone">
					<option value="False">No</option>
					<option value="True">Yes</option>
				</select>
			</td>
		</tr>
		<tr>
			<td class="heading" style="vertical-align:middle">
				Create History :
			</td>
			<td>
				<select id="createHistory">
					<option value="True">Yes</option>
					<option value="False">No</option>
				</select>
			</td>
		</tr>
		<tr id="JobDataContainer">
			<td class="heading">Data :</td>
			<td><div class="textBoxContainer"><textarea id="data" rows="5" cols="50"></textarea></div></td>
		</tr>
		<tr id="JobMetaDataContainer">
			<td class="heading">MetaData :</td>
			<td><div class="textBoxContainer"><textarea id="metaData" rows="5" cols="50"></textarea></div></td>
		</tr>
		<tr>
			<td class="heading">Application :</td>
			<td><div class="textBoxContainer"><input type="text" id="application" /></div></td>
		</tr>
		<tr>
			<td class="heading">Group :</td>
			<td><div class="textBoxContainer"><input type="text" id="group" /></div></td>
		</tr>
		<tr>
			<td class="heading">Absolute Timeout :</td>
			<td><div class="textBoxContainer"><input type="text" id="absoluteTimeout" /></div></td>
		</tr>
	</table>
	<table id="GenericJobData" class="default" style="width:100%;display:none">
		<tr>
			<td class="heading" style="width: 170px; min-width:170px; max-width:170px">Type (AssemblyQualified) :</td>
			<td><div class="textBoxContainer"><input type="text" id="jobTypeName" /></div></td>
		</tr>
	</table>
	<table id="SendMailJobData" class="default" style="width:100%;display:none">
		<tr>
			<td class="heading" style="width: 150px; min-width:150px; max-width:150px">To :</td>
			<td><div class="textBoxContainer"><input type="text" id="sendTo" /></div></td>
		</tr>
		<tr>
			<td class="heading">From :</td>
			<td><div class="textBoxContainer"><input type="text" id="sendFrom" /></div></td>
		</tr>
		<tr>
			<td class="heading">Subject :</td>
			<td><div class="textBoxContainer"><input type="text" id="sendSubject" /></div></td>
		</tr>
		<tr>
			<td class="heading">Body :</td>
			<td><div class="textBoxContainer"><textarea id="emailBody" rows="5" cols="50"></textarea></div></td>
		</tr>
	</table>		
	<table id="RemoteCallbackJobData" class="default" style="width:100%">
		<tr>
			<td class="heading" style="vertical-align:middle;width: 170px; min-width:170px; max-width:170px">
				Contract Type :
			</td>
			<td>
				<select id="contractType" onchange="showHideMethodName(this.options[this.selectedIndex].value)">
					<option value="Basic">Basic</option>
					<option value="Composite">Composite</option>
				</select>
			</td>
		</tr>
		<tr id="methodNameRow" style="display:none">
			<td class="heading">Method Name :</td>
			<td><div class="textBoxContainer"><input type="text" id="methodName" /></div></td>
		</tr>
		<tr>
			<td class="heading">Callback Url :</td>
			<td><div class="textBoxContainer"><input type="text" id="callbackUrl" /></div></td>
		</tr>
		<tr>
			<td class="heading" style="vertical-align:middle">
				Security Mode :
			</td>
			<td>
				<select id="securityMode">
					<option value="None">None</option>
					<option value="Transport">Transport</option>
					<option value="Message">Message</option>
					<option value="TransportWithMessageCredential">TransportWithMessageCredential</option>
					<option value="TransportCredentialOnly">TransportCredentialOnly</option>
				</select>
			</td>
		</tr>
		<tr>
			<td class="heading" style="vertical-align:middle">
				Credential Type :
			</td>
			<td>
				<select id="credentialType">
					<option value="None">None</option>
					<option value="Basic">Basic</option>
					<option value="Digest">Digest</option>
					<option value="Ntlm">Ntlm</option>
					<option value="Windows">Windows</option>
					<option value="Certificate">Certificate</option>
				</select>
			</td>
		</tr>
		<tr>
			<td class="heading">Domain :</td>
			<td><div class="textBoxContainer"><input type="text" id="domain" /></div></td>
		</tr>
		<tr>
			<td class="heading">Username :</td>
			<td><div class="textBoxContainer"><input type="text" id="username" /></div></td>
		</tr>
		<tr>
			<td class="heading">Password :</td>
			<td><div class="textBoxContainer"><input type="text" id="password" /></div></td>
		</tr>
		<tr>
			<td class="heading" style="vertical-align:middle">
				Ignore Certificate Errors :
			</td>
			<td>
				<select id="ignoreCertificateErrors">
					<option value="False">No</option>
					<option value="True">Yes</option>
				</select>
			</td>
		</tr>
	</table>
	<div style="width:100%;padding-top : 15px">
		<div id="scheduleSelect">
			<input id="runOnce" type="radio" name="scheduleGroup" checked="checked" onclick="scheduleGroupClick(this);" /><label for="runOnce">Run Once</label>
			<input id="runSchedule" type="radio" name="scheduleGroup" onclick="scheduleGroupClick(this);" /><label for="runSchedule">Schedule</label>
		</div>
	</div>
	<div style="width:100%;display:none" id="scheduleContainer">
		<p style="width:100%" class="heading">Schedule</p>
		<BWS:CalendarSchedule ID="CalendarScheduleControl" runat="server" />
	</div>
	<div style="width:100%;padding-top:30px;text-align:right"><input id="createButton" type="button" value="Create" onclick="createJob();" /></div>
</asp:Content>