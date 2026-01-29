<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Parcels.aspx.cs" Inherits="FRASS.Parcels.Parcels" %>
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
	<div style="padding-bottom: 5px;">
		<asp:Button ID="Button_ExportExcel" Text="Export to Excel" runat="server" OnClick="Button_ExportExcel_Click" />
		<asp:Button ID="Button_ExportWord" Text="Export to Word" runat="server" OnClick="Button_ExportWord_Click" />
	</div>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<telerik:RadGrid ID="RadGrid1" GridLines="None" AutoGenerateColumns="false" PageSize="50"
		AllowPaging="true" AllowSorting="true" runat="server"
		AllowFilteringByColumn="true"
		OnNeedDataSource="RadGrid1_NeedDataSource"
		ShowStatusBar="true">
		<PagerStyle Position="TopAndBottom" />
		<MasterTableView ShowFooter="false" DataKeyNames="ParcelID" CommandItemDisplay="None">
			<Columns>
				<telerik:GridHyperLinkColumn FilterControlWidth="100px" DataTextField="ParcelNumber" HeaderText="Parcel Number" DataNavigateURLFormatString="/Parcels/Report.aspx?ParcelID={0}" DataNavigateUrlFields="ParcelID" SortExpression="ParcelNumber" DataType="System.String">
					</telerik:GridHyperLinkColumn>
				<telerik:GridNumericColumn  FilterControlWidth="75px" DataField="Acre" HeaderText="Acres" NumericType="Number" DataType="System.Decimal" DecimalDigits="1" AllowRounding="true" DataFormatString="{0:N1}"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridNumericColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="Township" HeaderText="Township"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="Range" HeaderText="Range"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="Section" HeaderText="Section"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn FilterControlWidth="75px" DataField="County" HeaderText="County"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
	</telerik:RadAjaxPanel>
</asp:Content>
