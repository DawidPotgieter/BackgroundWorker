<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Jobs.ascx.cs" Inherits="WebUI.UserControls.Jobs" %>
<%@ Register Src="JobHistory.ascx" TagName="JobHistory" TagPrefix="BWS" %>
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
					<asp:ImageButton ID="btnRefresh" runat="server" ToolTip="Refresh" ImageUrl="~/content/images/Refresh.png" OnClick="btnRefresh_Click" OnClientClick="refreshJobsList<%= ApplicationName  %>();return false;" />
				</td>
				<td style="width: 130px; text-align: right; cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Create Job','CreateJob.aspx');">
					<img id="btnCreateJob" src="content/images/Add.png" />
					Create Job
				</td>
			</tr>
		</table>
		<div>
			<asp:ListView ID="JobsList" runat="server" OnItemDataBound="JobsList_ItemDataBound" OnPagePropertiesChanged="JobsList_PagePropertiesChanged">
				<LayoutTemplate>
					<table class="jobtable">
						<tr>
							<th style="width: 20px"></th>
							<th style="width: 60px">Id
							</th>
							<th>Name
							</th>
							<th>Group
							</th>
							<th>Description
							</th>
							<th style="width: 80px">Status</th>
							<th style="width: 140px">Last Start</th>
							<th style="width: 140px">Last End</th>
							<th style="width: 140px">Next Start</th>
							<th style="width: 20px"></th>
							<th style="width: 20px"></th>
							<th style="width: 20px"></th>
						</tr>
						<tr runat="server" id="itemPlaceHolder" />
					</table>
				</LayoutTemplate>
				<ItemTemplate>
					<tr class="jobrow">
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<img src="../content/images/Job.png" alt="Job" /></td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# (long)Eval("Id") %>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# (string)Eval("Name") %>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# (string)Eval("Group") %>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# (string)Eval("Description") %>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<span style='color: <%# Utility.GetStatusColor(((JobStatus)Eval("Status"))) %>'>
								<%# ((JobStatus)Eval("Status")).ToString() %>
							</span>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# Eval("LastStartTime") != null ? ((DateTime)Eval("LastStartTime")).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty %>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# Eval("LastEndTime") != null ? ((DateTime)Eval("LastEndTime")).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty%>
						</td>
						<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
							<%# Eval("NextStartTime") != null ? ((DateTime)Eval("NextStartTime")).ToString("dd-MM-yyyy HH:mm:ss") : string.Empty%>
						</td>
						<td>
							<img src="../content/images/Down_Small.png" alt="Job" onclick="toggleHistory(this, 'historyRow_<%# (long)Eval("Id") %>'); return false;" style="cursor: pointer" title="History" /></td>
						<td>
							<asp:Image runat="server" ID="btnRunJobImage" ImageUrl="~/content/images/Run_Small.png" Style="cursor: pointer" ToolTip="Run" />
						<td>
							<asp:Image runat="server" ID="btnDeleteJobImage" ImageUrl="~/content/images/Delete_Small.png" Style="cursor: pointer" ToolTip="Delete" />
						</td>
					</tr>
					<tr id='historyRow_<%# (long)Eval("Id") %>' style="display: none">
						<td colspan="12" style="padding: 0 0 0 0">
							<BWS:JobHistory ID="jobHistoryControl" runat="server" />
						</td>
					</tr>
				</ItemTemplate>
				<EmptyDataTemplate>
					<table class="jobtable" style="border-bottom: 1px solid white">
						<tr>
							<td>No jobs to display.</td>
						</tr>
					</table>
				</EmptyDataTemplate>
			</asp:ListView>
			<div style="vertical-align: middle">
				<asp:DataPager runat="server" ID="JobsListPager" PageSize="40" PagedControlID="JobsList" OnPreRender="JobsListPager_PreRender">
					<Fields>
						<asp:NextPreviousPagerField ButtonType="Image" ShowFirstPageButton="true" ShowNextPageButton="false" ShowPreviousPageButton="true" PreviousPageImageUrl="~/content/images/Prev_Small.png" FirstPageImageUrl="~/content/images/First_Small.png" />
						<asp:NumericPagerField ButtonCount="25" NumericButtonCssClass="pagerNumber" CurrentPageLabelCssClass="pagerCurrent" ButtonType="Image" NextPageImageUrl="~/content/images/NextPage_Small.png" PreviousPageImageUrl="~/content/images/PrevPage_Small.png" />
						<asp:NextPreviousPagerField ButtonType="Image" ShowLastPageButton="true" ShowNextPageButton="True" ShowPreviousPageButton="false" NextPageImageUrl="~/content/images/Next_Small.png" LastPageImageUrl="~/content/images/Last_Small.png" />
					</Fields>
				</asp:DataPager>
			</div>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>

