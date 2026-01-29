<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportApplied.aspx.cs" Inherits="FRASS.WebUI.Parcels.ReportApplied" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<style type="text/css">
		.topHeader {padding: 2px 2px 2px 4px; font-weight: bold; background-color: #EEEEEE; }
		.details { padding: 2px 2px 2px 4px; background-color: #DDD9C3; }
		.values { padding: 2px 2px 2px 4px; background-color: #FFFFFF; }
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true">
		<Windows>
			<telerik:RadWindow ID="RadWindow1" runat="server" Style="z-index: 9999;"
				ShowContentDuringLoad="false" Width="848px" Height="654px" Behaviors="Move, Close" Modal="true"
			 VisibleStatusbar="false" AutoSize="true" Skin="Hay"
			 	></telerik:RadWindow>
			<telerik:RadWindow runat="server" ID="RadWindow_AlertReportStarted" Behaviors="Move, Close" VisibleStatusbar="false" Skin="Hay" Width="280" Height="150">
				<ContentTemplate>
					<p style="padding: 10px;">
						This report generation has begun.  You will be notified via email when it is ready.
					</p>
				</ContentTemplate>
			</telerik:RadWindow>
		</Windows>
	</telerik:RadWindowManager>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay" ZIndex="700"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<div style="float: left;">
		<asp:Button ID="Button_ViewMarketValueReport" runat="server" Text="View Market Value Report" OnClick="Button_ViewMarketValueReport_Click" />
		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		<asp:DropDownList ID="DropDownList_MarketModels" runat="server" Visible="false"></asp:DropDownList>
		<asp:DropDownList ID="DropDownList_RPA" runat="server"></asp:DropDownList>
		<asp:Button ID="Button_ApplyMarketModel" runat="server" Text="Apply Market Model with RPA Portfolio" OnClick="Button_ApplyMarketModel_Click"  Visible="false"/>
		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		<asp:DropDownList ID="DropDownList_StumpageModels" runat="server" Visible="false"></asp:DropDownList>
		<asp:Button ID="Button_ApplyStumpageModels" runat="server" Text="Apply Stumpage Model" OnClick="Button_ApplyStumpageModel_Click" Visible="false"/>
	</div>
	<div style="clear: both;"></div>
	<div style="padding-top: 5px;">
		<div style="float: left; padding-right: 2px;">
			<asp:Button ID="Button_ViewStandReport" runat="server" Text="View Stand Report" OnClick="Button_ViewStandReport_Click" />
		</div>
		<div style="float: left;"><telerik:RadComboBox ID="RadComboBox_Stands" runat="server" Skin="Hay"></telerik:RadComboBox></div>
		<div style="clear: both;"></div>
		<div style="float: left; padding-top: 5px; padding-right: 2px;">
			<asp:Button ID="Button_ExportPDF" runat="server" Text="Export to PDF" OnClick="Button_ExportToPDF_Click" />
		</div>
		<div style="float: left; padding-top: 5px; padding-right: 2px;">
			<asp:Button ID="Button_ExportFullReport" runat="server" Text="Generate Full Parcel Report" OnClick="Button_ExportFullReportToPDF_Click" OnClientClick="radopen('','RadWindow_AlertReportStarted');" />
		</div>
		<div style="clear: both;"></div>
	</div>
	<div style="margin-top: 10px; border: 1px solid #000000; padding: 10px; background-color: #ffffff; width: 1080px;">
		<table cellspacing="0" cellpadding="0" style="border-collapse: collapse; border: 1px solid #000000; background-color: #ffffff;">
			<colgroup>
				<col width="150px" />
				<col width="150px" />
				<col width="110px" />
				<col width="140px" />
				<col width="120px" />
				<col width="130px" />
				<col width="120px" />
				<col width="160px" />
			</colgroup>
			<tr>
				<td class="topHeader">Market Model Name</td>
				<td class="topHeader">RPA Portfolio Name</td>
				<td class="topHeader">Rate of Inflation</td>
				<td class="topHeader">Landowner Discount Rate</td>
				<td class="topHeader">Reforestation Cost</td>
				<td class="topHeader">Access Fee (Timber)</td>
				<td class="topHeader">Maintenance Fee</td>
				<td class="topHeader">New Logging Road Construction</td>
			</tr>
			<tr>
				<td class="details"><asp:Label ID="Label_MarketModelName" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_RPAPortfolioName" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_RateOfInflation" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_LandownerDiscountRate" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_ReforestationCost" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_AccessFee" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_MaintenanceFee" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_NewLoggingRoadConstruction" runat="server"></asp:Label></td>
			</tr>
		</table>
		<div style="padding-top: 5px;">&nbsp;</div>
		<table cellspacing="0" cellpadding="0" style="border-collapse: collapse; border: 1px solid #000000; background-color: #ffffff;">
			<colgroup>
				<col width="110px" />
				<col width="110px" />
				<col width="110px" />
				<col width="110px" />
				<col width="110px" />
				<col width="110px" />
				<col width="110px" />
				<col width="110px" />
				<col width="40px" />
				<col width="40px" />
				<col width="40px" />
				<col width="40px" />
				<col width="40px" />
			</colgroup>
			<tr>
				<td rowspan="2" class="topHeader">Sort</td>
				<td rowspan="2" class="topHeader">Market Value</td>
				<td rowspan="2" class="topHeader">RPA</td>
				<td rowspan="2" class="topHeader">Longevity Term</td>
				<td rowspan="2" class="topHeader">Profit & Risk</td>
				<td rowspan="2" class="topHeader">Overhead & Administration</td>
				<td rowspan="2" class="topHeader">Logging Cost</td>
				<td rowspan="2" class="topHeader">Hauling Cost</td>
				<td colspan="5" class="topHeader">Projected Delivered Log Value</td>
			</tr>
			<tr>
				<td class="topHeader"><asp:Label runat="server" ID="Label_2011"></asp:Label></td>
				<td class="topHeader"><asp:Label runat="server" ID="Label_2020"></asp:Label></td>
				<td class="topHeader"><asp:Label runat="server" ID="Label_2040"></asp:Label></td>
				<td class="topHeader"><asp:Label runat="server" ID="Label_2060"></asp:Label></td>
				<td class="topHeader"><asp:Label runat="server" ID="Label_2080"></asp:Label></td>
			</tr>
			<asp:Repeater ID="Repeater_Sorts" runat="server" OnItemDataBound="Repeater_Sorts_ItemDataBound">
			<ItemTemplate>
				<tr>
					<td colspan="13" class="details"><asp:HyperLink ID="HyperLink_Sort" runat="server" style="text-decoration: underline; color: Blue; cursor: pointer;"></asp:HyperLink></td>
				</tr>
				<asp:Repeater ID="Repeater_Vals" runat="server" OnItemDataBound="Repeater_Vals_ItemDataBound">
					<ItemTemplate>
						<tr>
							<td class="values"><asp:Label ID="Label_Sort" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_MarketValue" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_RPA" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_Longevity" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_PR" runat="server"></asp:Label></td>						
							<td class="values"><asp:Label ID="Label_ONA" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_LoggingCosts" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulingCosts" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_2011Future" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_2020Future" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_2040Future" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_2060Future" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_2080Future" runat="server"></asp:Label></td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
			</asp:Repeater>

			
		</table>
	</div>
	</telerik:RadAjaxPanel>
</asp:Content>
