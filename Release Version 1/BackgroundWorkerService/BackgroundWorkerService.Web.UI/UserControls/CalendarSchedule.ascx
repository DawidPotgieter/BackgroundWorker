<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarSchedule.ascx.cs" Inherits="WebUI.UserControls.CalendarSchedule" %>
<script type="text/javascript">
	$(function () {
		$('#DaysOfTheWeek').buttonset();
		$('#StartDailyAt').timepicker({ showSecond: true });
		$('#RepeatInterval').timepicker({ showSecond: true });
	});
</script>

<div id="DaysOfTheWeek" style="padding-bottom:10px">
	<input id="Monday" type="checkbox" /><label for="Monday">Monday</label>
	<input id="Tuesday" type="checkbox" /><label for="Tuesday">Tuesday</label>
	<input id="Wednesday" type="checkbox" /><label for="Wednesday">Wednesday</label>
	<input id="Thursday" type="checkbox" /><label for="Thursday">Thursday</label>
	<input id="Friday" type="checkbox" /><label for="Friday">Friday</label>
	<input id="Saturday" type="checkbox" /><label for="Saturday">Saturday</label>
	<input id="Sunday" type="checkbox" /><label for="Sunday">Sunday</label>
</div>

<div id="startTimeContainer" style="padding-bottom:10px">
	<table class="default">
		<tr>
			<td style="width:120px"><label for="StartDailyAt">Start Time :</label></td>
			<td><input type="text" id="StartDailyAt" style="width:80px" value="00:00" /></td>
		</tr>
	</table>
</div>

<div id="repeatEvery" style="padding-bottom:10px">
	<table class="default">
		<tr>
			<td style="width:120px"><label for="RepeatInterval">Repeat Every :</label></td>
			<td><input type="text" id="RepeatInterval" style="width:80px" /></td>
		</tr>
	</table>
</div>
