<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyReports.aspx.cs" Inherits="FRASS.WebUI.Reports.MyReports" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style>
		.doc-links {
			display: flex;
			flex-wrap: wrap;
			gap: 12px;
			margin-bottom: 16px;
		}

		.doc-link {
			flex: 1 1 180px;
			padding: 10px 14px;
			border: 1px solid #ccc;
			border-radius: 4px;
			background-color: #f9f9f9;
			text-align: center;
		}

		.doc-link a {
			display: block;
			color: #2a4b8d;
			font-weight: 600;
			text-decoration: none;
		}

		.doc-link a:hover {
			text-decoration: underline;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" Skin="Hay"></telerik:RadWindowManager>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
		<div class="doc-links">
			<div class="doc-link"><a href="/Documents/User_Guide/FRASS_User_Manual.pdf" target="_blank">📘 FRASS User Guide</a></div>
			<div class="doc-link"><a href="/Documents/User_Guide/FRASS_Package_Summary.pdf" target="_blank">📄 FRASS Package Summary</a></div>
			<div class="doc-link"><a href="/Documents/User_Guide/FRASS_Data_Rights_and_Access_Policy-20251025B.pdf" target="_blank">🧾 Data Rights &amp; Access Policy</a></div>
			<div class="doc-link"><a href="/Documents/User_Guide/FRASS_Demo_Access_Agreement_2025.pdf" target="_blank">🔒 Terms of Demonstration Use</a></div>
			<div class="doc-link"><a href="/Documents/User_Guide/FRASS_Display_Attribution_Summary_2025.pdf" target="_blank">🖼️ Display &amp; Attribution Summary</a></div>
		</div>
	<asp:Label ID="Label_Message" runat="server"></asp:Label>
		<telerik:RadGrid ID="RadGrid1" GridLines="None" AutoGenerateColumns="false" PageSize="500"
			AllowPaging="false" AllowSorting="false" runat="server"
			AllowFilteringByColumn="false"
			OnItemDataBound="RadGrid1_ItemDataBound"
			OnDeleteCommand="RadGrid1_DeleteCommand"
			ShowStatusBar="true">
			<MasterTableView ShowFooter="false" DataKeyNames="ReportID" CommandItemDisplay="None">
				<Columns>
					<telerik:GridHyperLinkColumn UniqueName="HyperLink_Title" DataTextField="Title" DataNavigateUrlFields="ReportID" DataNavigateUrlFormatString="/PDFs/ParcelReportFull.aspx?ReportID={0}"></telerik:GridHyperLinkColumn>
					<telerik:GridBoundColumn DataField="ParcelNumber" HeaderText="Parcel Number">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Type" HeaderText="Report Type">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="DateCreated" HeaderText="Report Date" DataType="System.DateTime" DataFormatString="{0:M/d/yyyy}">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Completed" HeaderText="Ready For Download">
					</telerik:GridBoundColumn>
					<telerik:GridButtonColumn ConfirmText="Are you sure you want to delete this Report?" ConfirmDialogType="RadWindow" ConfirmTitle="Confirm Deletion" ButtonType="LinkButton" CommandName="Delete" Text="Delete" UniqueName="DeleteColumn"></telerik:GridButtonColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</telerik:RadAjaxPanel>
</asp:Content>
