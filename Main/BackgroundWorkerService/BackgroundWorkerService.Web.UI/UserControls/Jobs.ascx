<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Jobs.ascx.cs" Inherits="WebUI.UserControls.Jobs" %>
<%@ Import Namespace="WebUI.BackgroundWorkerService.Service" %>
<%@ Import Namespace="WebUI.Code" %>


<script type="text/javascript">

	function refreshJobsList<%= ApplicationName  %>() {
		reloadAlertNotifier();
		<%= this.Page.ClientScript.GetPostBackEventReference(btnRefresh, "")  %>
	}

	function pageLoad<%= ApplicationName  %>() {
		$('#SelectStatus<%= ApplicationName  %>').buttonset();
	}

	$(function () {
		Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoad<%= ApplicationName  %>);
	});

</script>

<asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
	<ContentTemplate>
		<table style="width: 100%; padding-bottom: 30px">
			<tr>
				<td></td>
				<td>
					<div id="SelectStatus<%= ApplicationName %>" style="padding-top: 3px">
						<asp:CheckBox ID="Done" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Done.ClientID %>">Done</label>
						<asp:CheckBox ID="Pending" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Pending.ClientID %>">Pending</label>
						<asp:CheckBox ID="Queued" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Queued.ClientID %>">Queued</label>
						<asp:CheckBox ID="Ready" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Ready.ClientID %>">Ready</label>
						<asp:CheckBox ID="Scheduled" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Scheduled.ClientID %>">Scheduled</label>
						<asp:CheckBox ID="Executing" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Executing.ClientID %>">Executing</label>
						<asp:CheckBox ID="Fail" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Fail.ClientID %>">Fail</label>
						<asp:CheckBox ID="FailRetry" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= FailRetry.ClientID %>">FailRetry</label>
						<asp:CheckBox ID="FailAutoRetry" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= FailAutoRetry.ClientID %>">FailAutoRetry</label>
						<asp:CheckBox ID="ShutdownTimeout" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= ShutdownTimeout.ClientID %>">ShutdownTimeout</label>
						<asp:CheckBox ID="ExecutionTimeout" runat="server" Checked="true" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= ExecutionTimeout.ClientID %>">ExecutionTimeout</label>
						<asp:CheckBox ID="Deleted" runat="server" Checked="false" OnCheckedChanged="StatusCheckChanged" AutoPostBack="true" /><label for="<%= Deleted.ClientID %>">Deleted</label>
					</div>
				</td>
				<td>

				<td style="width: 230px">Last Refresh :
					<asp:Literal ID="lblLastRefresh" runat="server"></asp:Literal></td>
				<td style="width: 50px; text-align: right">
					<asp:ImageButton ID="btnRefresh" runat="server" ToolTip="Refresh" ImageUrl="~/content/images/Refresh.png" OnClick="btnRefresh_Click" />
				</td>
				<td style="width: 130px; text-align: right; cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Create Job','CreateJob.aspx');">
					<img id="btnCreateJob" src="content/images/Add.png" />
					Create Job
				</td>
			</tr>
		</table>
		<div runat="server" id="JobGridsContainer">
		</div>
	</ContentTemplate>
</asp:UpdatePanel>

