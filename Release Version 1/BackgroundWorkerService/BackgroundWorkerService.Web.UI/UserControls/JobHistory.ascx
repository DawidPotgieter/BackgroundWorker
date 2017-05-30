<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobHistory.ascx.cs" Inherits="WebUI.UserControls.JobHistory" %>

<table class="default" style="padding-bottom: 30px">
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
	<asp:ListView ID="JobHistoryList" runat="server">
		<LayoutTemplate>
			<tr runat="server" id="itemPlaceHolder" />
		</LayoutTemplate>
		<ItemTemplate>
			<tr>
				<td>
					<%# (DateTime)Eval("StartTime") %>
				</td>
				<td>
					<%# (DateTime)Eval("EndTime")%>
				</td>
				<td>
					<%# Eval("Status") %>
				</td>
				<td>
					<%# (string)Eval("ErrorMessage")%>
				</td>
			</tr>
		</ItemTemplate>
	</asp:ListView>
</table>
