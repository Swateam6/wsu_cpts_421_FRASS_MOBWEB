<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Allottees.aspx.cs" Inherits="FRASS.WebUI.Owners.Allottees" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style>
		html.RadForm.rfdTextbox .rgFilterRow .RadInput input[type="text"],
			html.RadForm.rfdTextbox .rgFilterRow .RadInput input[type="password"],
			html.RadForm.rfdTextbox .rgFilterRow .RadInput input[type="search"],
			html.RadForm.rfdTextbox .rgFilterRow .RadInput input[type="url"],
			html.RadForm.rfdTextbox .rgFilterRow .RadInput input[type="tel"],
			html.RadForm.rfdTextbox .rgFilterRow .RadInput input[type="email"]
			{
			  width: 100%;
			}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<telerik:RadGrid ID="RadGrid1" GridLines="None" AutoGenerateColumns="false" PageSize="50"
		AllowPaging="true" AllowSorting="true" runat="server"
		AllowFilteringByColumn="true"
		OnNeedDataSource="RadGrid1_NeedDataSource"
		ShowStatusBar="true">
		<PagerStyle Position="TopAndBottom" />
		<MasterTableView ShowFooter="false" CommandItemDisplay="None">
			<Columns>
				<telerik:GridHyperLinkColumn FilterControlWidth="100px" DataTextField="FirstName" HeaderText="First Name" DataNavigateURLFormatString="/Parcels/Report.aspx?ParcelID={0}" DataNavigateUrlFields="ParcelID" SortExpression="FirstName" DataType="System.String">
				</telerik:GridHyperLinkColumn>
				<telerik:GridHyperLinkColumn FilterControlWidth="75px" DataTextField="LastName" HeaderText="Last Name" DataNavigateURLFormatString="/Parcels/Report.aspx?ParcelID={0}" DataNavigateUrlFields="ParcelID"  SortExpression="LastName" DataType="System.String">
				</telerik:GridHyperLinkColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="AllotteeNumber" HeaderText="Allottee Number">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="ParcelNumber" HeaderText="Parcel Number">
				</telerik:GridBoundColumn>
				<telerik:GridNumericColumn  FilterControlWidth="75px" DataField="Share" HeaderText="Undivided Interest Share" NumericType="Number" DataType="System.Decimal" DecimalDigits="5" AllowRounding="true" DataFormatString="{0:N5}"><ItemStyle HorizontalAlign="Right" />
				</telerik:GridNumericColumn>
				<telerik:GridNumericColumn FilterControlWidth="75px" DataField="Acres" HeaderText="Acres" NumericType="Number" DataType="System.Decimal" DecimalDigits="1" AllowRounding="true" DataFormatString="{0:N1}"><ItemStyle HorizontalAlign="Right" />
				</telerik:GridNumericColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="AllotmentNumber" HeaderText="Allotment Number">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="Township" HeaderText="Township">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="Range" HeaderText="Range">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="Section" HeaderText="Section">
				</telerik:GridBoundColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
	</telerik:RadAjaxPanel>
</asp:Content>
