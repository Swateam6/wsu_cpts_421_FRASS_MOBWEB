<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EventLogs.aspx.cs" Inherits="FRASS.WebUI.Admin.EventLogs" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Hay"></telerik:RadWindowManager>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<asp:Label ID="Label_Message" runat="server"></asp:Label>
		<telerik:RadGrid ID="RadGrid1" GridLines="None" AutoGenerateColumns="false" PageSize="50"
			AllowPaging="true" AllowSorting="true" runat="server"
			DataSourceID="LogsDataSource" 
			AllowFilteringByColumn="true"
			ShowStatusBar="true">
			<MasterTableView ShowFooter="false" DataKeyNames="LogID" CommandItemDisplay="None">
				<Columns>
					<telerik:GridBoundColumn FilterControlWidth="100" DataField="LogID" HeaderText="Event ID">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn  FilterControlWidth="100" DataField="LogType.LogType1" HeaderText="Type">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="DateCreated" HeaderText="Error Time" DataType="System.DateTime">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Description" HeaderText="Error">
					</telerik:GridBoundColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</telerik:RadAjaxPanel>
	<asp:LinqDataSource ID="LogsDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="Logs" OrderBy="LogID Descending"></asp:LinqDataSource>
</asp:Content>
