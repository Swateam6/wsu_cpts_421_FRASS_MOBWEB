<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StumpageApplied.aspx.cs" Inherits="FRASS.WebUI.Parcels.StumpageApplied" %>
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
			<telerik:RadWindow ID="RadWindow1" runat="server" Style="z-index: 9999"
				ShowContentDuringLoad="false" Width="848px" Height="654px" Behaviors="Move, Close" Modal="true"
			 VisibleStatusbar="false" AutoSize="true" Skin="Hay"
				></telerik:RadWindow>
			<telerik:RadWindow runat="server" ID="RadWindow_AlertReportStarted" Behaviors="Move, Close" VisibleStatusbar="false" Skin="Hay" Width="280" Height="150">
				<ContentTemplate>
					<p>
						This report generation has begun.  You will be notified via email when it is ready.
					</p>
				</ContentTemplate>
			</telerik:RadWindow>
		</Windows>
	</telerik:RadWindowManager>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<div style="float: left;">
		<asp:Button ID="Button_ViewMarketValueReport" runat="server" Text="View Market Value Report" OnClick="Button_ViewMarketValueReport_Click" />
		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		<asp:DropDownList ID="DropDownList_MarketModels" runat="server" Visible="false"></asp:DropDownList>
		<asp:Button ID="Button_ApplyMarketModel" runat="server" Text="Apply Market Model" OnClick="Button_ApplyMarketModel_Click" Visible="false"/>
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
			<asp:Button ID="Button_ExportFullReport" runat="server" Text="Generate Full Parcel Report" OnClick="Button_ExportFullReportToPDF_Click"  OnClientClick="radopen('','RadWindow_AlertReportStarted');" />
		</div>
		<div style="clear: both;"></div>
	</div>
	<div style="margin-top: 10px; border: 1px solid #000000; padding: 10px; background-color: #ffffff; width: 1080px;">
		<table cellspacing="0" cellpadding="0" style="border-collapse: collapse; border: 1px solid #000000; background-color: #ffffff;">
			<colgroup>
				<col width="140px" />
				<col width="140px" />
				<col width="140px" />
				<col width="140px" />
				<col width="130px" />
				<col width="120px" />
				<col width="140px" />
				<col width="130px" />
			</colgroup>
			<tr>
				<td class="topHeader">Market Model Name</td>
				<td class="topHeader">Rate of Inflation</td>
				<td class="topHeader">Landowner Discount Rate</td>
				<td class="topHeader">Reforestation Cost</td>
				<td class="topHeader">Access Fee (Timber)</td>
				<td class="topHeader">Maintenance Fee</td>
				<td class="topHeader">New Logging Road Construction</td>
				<td class="topHeader">Parcel Number</td>
			</tr>
			<tr>
				<td class="details"><asp:Label ID="Label_MarketModelName" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_RateOfInflation" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_LandownerDiscountRate" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_ReforestationCost" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_AccessFee" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_MaintenanceFee" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_NewLoggingRoadConstruction" runat="server"></asp:Label></td>
				<td class="details"><asp:Label ID="Label_ParcelNumber" runat="server"></asp:Label></td>
			</tr>
		</table>
		<div style="padding-top: 5px;">&nbsp;</div>
		<table cellspacing="0" cellpadding="0" style="border-collapse: collapse; border: 1px solid #000000; background-color: #ffffff; width: 1080px;">
			<colgroup>
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="100px" />
				<col width="90px" />
				<col width="90px" />
			</colgroup>
			<tr>
				<td class="topHeader">Species</td>
				<td class="topHeader">Quality Code</td>
				<td class="topHeader">Haul Zone 3<br />RPA</td>
				<td class="topHeader">Haul Zone 4<br />RPA</td>
				<td class="topHeader">Haul Zone 5<br />RPA</td>
				<td class="topHeader">Longeveity</td>
				<td class="topHeader">Haul Zone 3<br />Price</td>
				<td class="topHeader">Haul Zone 4<br />Price</td>
				<td class="topHeader">Haul Zone 5<br />Price</td>
				<td class="topHeader">O & A</td>
				<td class="topHeader">P&R</td>
			</tr>
			<asp:Repeater ID="Repeater_Species" runat="server" OnItemDataBound="Repeater_Species_ItemDataBound">
			<ItemTemplate>
				<tr>
					<td colspan="11" class="details"><asp:Label ID="Label_Species" runat="server" style="font-weight: bold; cursor: pointer;"></asp:Label></td>
				</tr>
				<asp:Repeater ID="Repeater_Vals" runat="server" OnItemDataBound="Repeater_Vals_ItemDataBound">
					<ItemTemplate>
						<tr>
							<td class="values">&nbsp;</td>
							<td class="values"><asp:Label ID="Label_QualityCode" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulZone3_RPA" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulZone4_RPA" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulZone5_RPA" runat="server"></asp:Label></td>						
							<td class="values"><asp:Label ID="Label_Longevity" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulZone3_Price" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulZone4_Price" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_HaulZone5_Price" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_ONA" runat="server"></asp:Label></td>
							<td class="values"><asp:Label ID="Label_PR" runat="server"></asp:Label></td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
			</asp:Repeater>
		</table>
	</div>
	</telerik:RadAjaxPanel>
</asp:Content>
