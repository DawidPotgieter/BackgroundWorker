<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobHistory.ascx.cs" Inherits="WebUI.UserControls.JobHistory" %>

<div style="border-bottom:1px solid white">
		<asp:ListView ID="JobHistoryList" runat="server">
			<LayoutTemplate>
				<table class="jobhistorytable">
					<tr>
						<th style="width: 20%">
							Start
						</th>
						<th style="width: 20%">
							End
						</th>
						<th style="width: 10%">
							Status
						</th>
						<th style="width: 50%">
							Error Message
						</th>
					</tr>
					<tr runat="server" id="itemPlaceHolder" />
				</table>
			</LayoutTemplate>
			<ItemTemplate>
				<tr class="jobhistoryrow" style="cursor:pointer" onclick="openDialog = loadPageInPopupDialog('Job History','JobHistory.aspx?Id=<%# (long)Eval("Id") %>'); return false;">
					<td style="vertical-align:top">
						<%# (DateTime)Eval("StartTime") %>
					</td>
					<td style="vertical-align:top">
						<%# (DateTime)Eval("EndTime")%>
					</td>
					<td style="vertical-align:top">
						<%# Eval("Status") %>
					</td>
					<td style="color:red;vertical-align:top">
						<%# HttpUtility.HtmlEncode((string)Eval("ErrorMessage")) %>
					</td>
				</tr>
			</ItemTemplate>
			<EmptyDataTemplate>
				<table class="jobhistorytable">
					<tr><td>No history records available.</td></tr>
				</table>
			</EmptyDataTemplate>
		</asp:ListView>
</div>
