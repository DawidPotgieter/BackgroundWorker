<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarSchedule.ascx.cs" Inherits="WebUI.UserControls.CalendarSchedule" ClientIDMode="Static" %>
<script type="text/javascript">
	$(function () {
		$('#DaysOfTheWeek').buttonset();
		$('#StartDateTime').datetimepicker({ dateFormat: 'dd-mm-yy', timeFormat: 'hh:mm:ss', showSecond: true });
		$('#StartDailyAt').timepicker({ timeFormat: 'hh:mm:ss', showSecond: true });
		$('#RepeatInterval').timepicker({ timeFormat: 'hh:mm:ss', showSecond: true });
	});
</script>

<div id="DaysOfTheWeek" style="padding-bottom:10px">
	<asp:CheckBox ID="Monday" runat="server" /><label for="Monday">Monday</label>
	<asp:CheckBox ID="Tuesday" runat="server" /><label for="Tuesday">Tuesday</label>
	<asp:CheckBox ID="Wednesday" runat="server" /><label for="Wednesday">Wednesday</label>
	<asp:CheckBox ID="Thursday" runat="server" /><label for="Thursday">Thursday</label>
	<asp:CheckBox ID="Friday" runat="server" /><label for="Friday">Friday</label>
	<asp:CheckBox ID="Saturday" runat="server" /><label for="Saturday">Saturday</label>
	<asp:CheckBox ID="Sunday" runat="server" /><label for="Sunday">Sunday</label>
</div>

<table>
	<tr>
		<td style="width:120px"><label for="StartDateTime">Schedule Start :</label></td>
		<td><asp:TextBox ID="StartDateTime" runat="server" style="width:150px"></asp:TextBox></td>
	</tr>
	<tr>
		<td style="width:120px"><label for="StartDailyAt">Start Time :</label></td>
		<td><asp:TextBox ID="StartDailyAt" runat="server" style="width:80px" value="00:00:00"></asp:TextBox></td>
	</tr>
	<tr>
		<td style="width:120px"><label for="RepeatInterval">Repeat Every :</label></td>
		<td><asp:TextBox ID="RepeatInterval" runat="server" style="width:80px"></asp:TextBox></td>
	</tr>
</table>
