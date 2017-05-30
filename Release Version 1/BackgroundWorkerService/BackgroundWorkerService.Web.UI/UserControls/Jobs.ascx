<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Jobs.ascx.cs" Inherits="WebUI.UserControls.Jobs" %>
<%@ Register src="JobHistory.ascx" tagname="JobHistory" tagprefix="BWS" %>

<script type="text/javascript">
	function refreshJobsList() {
		<%= this.Page.GetPostBackEventReference(btnRefresh)  %>
	}

	function toggleHistory(img, rowId) {
		$('#' + rowId).toggle();
		if ($(img).attr("src").indexOf("Down_Small") == -1)	{
			$(img).attr("src", "../content/images/Down_Small.png");
		}
		else {
			$(img).attr("src", "../content/images/Up_Small.png");
		}
	}
</script>


<asp:UpdatePanel ID="uPanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
	<ContentTemplate>
		<table class="default" style="padding-bottom: 30px">
			<tr>
				<td></td>
				<td style="width:100px;text-align:right">
					<asp:ImageButton ID="btnRefresh" runat="server" ToolTip="Refresh"
						ImageUrl="~/content/images/Refresh.png" onclick="btnRefresh_Click" />
				</td>
				<td style="width:130px;text-align:right;cursor:pointer" onclick="openDialog = loadPageInPopupDialog('Create Job','CreateJob.aspx');">
					<img id="btnCreateJob" src="content/images/Add.png" />
					Create Job
				</td>
			</tr>
		</table>
		<div>
			<asp:ListView ID="JobsList" runat="server" onitemcommand="JobsList_ItemCommand" 
				onitemdatabound="JobsList_ItemDataBound" 
				onpagepropertieschanged="JobsList_PagePropertiesChanged">
				<LayoutTemplate>
					<table class="default" style="padding-bottom: 30px">
						<tr>
							<th style="width:20px"></th>
							<th style="width: 5%">
								Id
							</th>
							<th style="width: 20%">
								Name
							</th>
							<th>
								Description
							</th>
							<th style="width:20px"></th>
							<th style="width:20px"></th>
							<th style="width:20px"></th>
						</tr>
						<tr runat="server" id="itemPlaceHolder" />
					</table>
				</LayoutTemplate>
				<ItemTemplate>
					<tr>
						<td><img src="../content/images/Job.png" alt="Job" /></td>
						<td>
							<%# (long)Eval("Id") %>
						</td>
						<td style="cursor:pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>');">
							<%# (string)Eval("Name") %>
						</td>
						<td style="cursor:pointer" onclick="openDialog = loadPageInPopupDialog('Edit Job','EditJob.aspx?Id=<%# (long)Eval("Id") %>');">
							<%# (string)Eval("Description") %>
						</td>
						<td><img src="../content/images/Down_Small.png" alt="Job" onclick="toggleHistory(this, 'historyRow_<%# (long)Eval("Id") %>'); return false;" style="cursor:pointer" /></td>
						<td><asp:ImageButton runat="server" ID="btnRunJob" ImageUrl="~/content/images/Run_Small.png" CommandName="Run" CommandArgument='<%# (long)Eval("Id") %>' OnClientClick="return confirm('Are you sure you want to run this job now?');" /></td>
						<td><asp:ImageButton runat="server" ID="btnDeleteJob" ImageUrl="~/content/images/Delete_Small.png" CommandName="Delete" CommandArgument='<%# (long)Eval("Id") %>' OnClientClick="return confirm('Are you sure you want to permanently DELETE this job?');" /></td>
					</tr>
					<tr id='historyRow_<%# (long)Eval("Id") %>' style="display:none">
						<td colspan="4"><BWS:JobHistory ID="jobHistoryControl" runat="server" /></td>
					</tr>
				</ItemTemplate>
				<EmptyDataTemplate>
					No jobs to display.
				</EmptyDataTemplate>
			</asp:ListView>
			<div style="vertical-align:middle">
				<asp:DataPager runat="server" ID="JobsListPager" PageSize="10" PagedControlID="JobsList" OnPreRender="JobsListPager_PreRender">
					<Fields>
						<asp:NextPreviousPagerField ButtonType="Image" ShowFirstPageButton="false" ShowNextPageButton="False" ShowPreviousPageButton="true" PreviousPageImageUrl="~/content/images/Prev_Small.png" />
						<asp:NumericPagerField ButtonCount="5" NumericButtonCssClass="pagerNumber" CurrentPageLabelCssClass="pagerCurrent" />
						<asp:NextPreviousPagerField ButtonType="Image" ShowLastPageButton="false" ShowNextPageButton="True" ShowPreviousPageButton="false" NextPageImageUrl="~/content/images/Next_Small.png" />
					</Fields>
				</asp:DataPager>
			</div>
		</div>
	</ContentTemplate>
</asp:UpdatePanel>

