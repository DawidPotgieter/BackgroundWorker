<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="JobHistory.aspx.cs" Inherits="WebUI.JobHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<table style="width:100%;table-layout:fixed;word-wrap:break-word;">
		<tr>
			<td style="width:140px">Id :</td>
			<td><asp:Literal ID="lblId" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Name :</td>
			<td><asp:Literal ID="lblName" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Description :</td>
			<td><asp:Literal ID="lblDescription" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Job Id :</td>
			<td><asp:Literal ID="lblJobId" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Job Type :</td>
			<td><asp:Literal ID="lblJobType" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Job Unique Id :</td>
			<td><asp:Literal ID="lblUniqueId" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Success :</td>
			<td><asp:Literal ID="lblSuccess" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Status :</td>
			<td><asp:Label ID="lblStatus" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td>Data :</td>
			<td><asp:Literal ID="lblData" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Metadata :</td>
			<td><asp:Literal ID="lblMetaData" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Start :</td>
			<td><asp:Literal ID="lblStart" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>End :</td>
			<td><asp:Literal ID="lblEnd" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Error Message :</td>
			<td style="color:Red"><asp:Literal ID="lblErrorMessage" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Application :</td>
			<td><asp:Literal ID="lblApplication" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Group :</td>
			<td><asp:Literal ID="lblGroup" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Queue Id :</td>
			<td><asp:Literal ID="lblQueueId" runat="server"></asp:Literal></td>
		</tr>
		<tr>
			<td>Instance :</td>
			<td><asp:Literal ID="lblInstance" runat="server"></asp:Literal></td>
		</tr>
	</table>
</asp:Content>
