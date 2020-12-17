<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobsGroup.ascx.cs" Inherits="WebUI.UserControls.JobsGroup" %>
<%@ Register Src="JobHistory.ascx" TagName="JobHistory" TagPrefix="BWS" %>
<%@ Import Namespace="WebUI.BackgroundWorkerService.Service" %>
<%@ Import Namespace="WebUI.Code" %>

<h2 style="padding-bottom:0px"><asp:Literal ID="GroupNameHeader" runat="server"></asp:Literal></h2>

<asp:ListView ID="JobsList" runat="server" OnItemDataBound="JobsList_ItemDataBound" OnPagePropertiesChanged="JobsList_PagePropertiesChanged">
	<LayoutTemplate>
		<table class="jobtable">
			<tr>
				<th style="width: 20px"></th>
				<th style="width: 60px">Id</th>
				<th style="width:400px">Name</th>
				<th>Description</th>
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
				<asp:Image runat="server" ID="StatusIcon" Style="cursor: pointer" />
			</td>
			<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
				<%# (long)Eval("Id") %>
			</td>
			<td style="cursor: pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
				<%# (string)Eval("Name") %>
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
	<asp:DataPager runat="server" ID="JobsListPager" PageSize="40" PagedControlID="JobsList">
		<Fields>
			<asp:NextPreviousPagerField ButtonType="Image" ShowFirstPageButton="true" ShowNextPageButton="false" ShowPreviousPageButton="true" PreviousPageImageUrl="~/content/images/Prev_Small.png" FirstPageImageUrl="~/content/images/First_Small.png" />
			<asp:NumericPagerField ButtonCount="25" NumericButtonCssClass="pagerNumber" CurrentPageLabelCssClass="pagerCurrent" ButtonType="Image" NextPageImageUrl="~/content/images/NextPage_Small.png" PreviousPageImageUrl="~/content/images/PrevPage_Small.png" />
			<asp:NextPreviousPagerField ButtonType="Image" ShowLastPageButton="true" ShowNextPageButton="True" ShowPreviousPageButton="false" NextPageImageUrl="~/content/images/Next_Small.png" LastPageImageUrl="~/content/images/Last_Small.png" />
		</Fields>
	</asp:DataPager>
</div>