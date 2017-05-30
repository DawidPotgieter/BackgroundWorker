<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditJob.aspx.cs" Inherits="WebUI.Job" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
	<script type="text/javascript">
		$(function () {
			$("#txtAbsoluteTimeout").timepicker({ timeFormat: 'hh:mm:ss', showSecond: true });
			$('#btnUpdate').button();
		});

		function updateJob() {
			callPageMethod(
				"Job.aspx",
				"UpdateJob?Id=<%= JobId %>",
				"{ name : '" + $('#txtName').val() + "'" +
				", description : '" + $('#txtDescription').val() + "'" +
				", data : '" + $('#txtData').val() + "'" +
				", metaData : '" + $('#txtMetaData').val() + "'" +
				", absoluteTimeout : '" + $('#txtAbsoluteTimeout').val() + "'" +
				", queueId : '" + $('#txtQueueId').val() + "'" +
				", application : '" + $('#txtApplication').val() + "'" +
				", group : '" + $('#txtGroup').val() + "'" +
				", suppressHistory : '" + $('#chkSuppressHistory').is(':checked') + "'" +
				", deleteWhenDone : '" + $('#chkDeleteWhenDone').is(':checked') + "'" +
				"}",
				function (msg, status) {
					if (msg.d.length > 0) {
						alert(msg.d);
					}
					else {
						openDialog.dialog('close');
					}
				},
				function (xhr, msg, e) {
					alert(msg.d);
				});
		}
	</script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
	<table class="default" style="width:750px;">
		<tr>
			<td style="width:140px">Id :</td>
			<td><asp:Literal ID="lblId" runat="server"></asp:Literal></td>
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
			<td><asp:Literal ID="lblLastErrorMessage" runat="server"></asp:Literal></td>
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
			<td>Absolute Timeout :</td>
			<td><asp:TextBox ID="txtAbsoluteTimeout" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Queue Id :</td>
			<td><asp:TextBox ID="txtQueueId" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td>Status :</td>
			<td><asp:Literal ID="lblStatus" runat="server"></asp:Literal></td>
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
			<td><asp:CheckBox ID="chkSuppressHistory" runat="server" /></td>
		</tr>
		<tr>
			<td>Delete When Done :</td>
			<td><asp:CheckBox ID="chkDeleteWhenDone" runat="server" /></td>
		</tr>
		<tr>
			<td>
			</td>
			<td style="text-align:right">
				<input id="btnUpdate" type="button" value="Update" onclick="if (confirm('Are you sure you want to update the values for this job?')) { updateJob(); }" />
			</td>
		</tr>
	</table>
</asp:Content>