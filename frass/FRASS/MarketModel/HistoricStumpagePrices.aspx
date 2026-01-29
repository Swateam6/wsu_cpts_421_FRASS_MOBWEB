<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricStumpagePrices.aspx.cs" Inherits="FRASS.MarketModel.HistoricStumpagePrices" %>
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
			OnInsertCommand="RadGrid1_InsertCommand"
			OnItemDataBound="RadGrid1_ItemDataBound"
			OnUpdateCommand="RadGrid1_UpdateCommand"
			OnNeedDataSource="RadGrid1_NeedDataSource"
			OnDeleteCommand="RadGrid1_DeleteCommand"
			AllowFilteringByColumn="false"
			ShowStatusBar="true">
			<MasterTableView ShowFooter="false" DataKeyNames="StumpagePricesID" EditMode="InPlace" CommandItemDisplay="TopAndBottom">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Species" DataField="Specy.Abbreviation">
						<ItemTemplate>
							 <%# DataBinder.Eval(Container.DataItem, "Specy.Abbreviation")%>
						</ItemTemplate>
						<EditItemTemplate>
							<telerik:RadComboBox runat="server" ID="RadComboBox_SpeciesAbbreviations" DataTextField="Abbreviation"
								DataValueField="SpeciesID" DataSourceID="SpeciesAbbreviationsDataSource" SelectedValue='<%#Bind("SpeciesID") %>'>
							</telerik:RadComboBox>
						</EditItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridBoundColumn DataType="System.DateTime" DataField="MarketDate" HeaderText="MarketDate" DataFormatString="{0:M/d/yyyy}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="TimberQualityCode" HeaderText="Timber Quality Code"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="HaulingZone1" HeaderText="Hauling Zone 1" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="HaulingZone2" HeaderText="Hauling Zone 2" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="HaulingZone3" HeaderText="Hauling Zone 3" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="HaulingZone4" HeaderText="Hauling Zone 4" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="HaulingZone5" HeaderText="Hauling Zone 5" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn"
						 HeaderStyle-Width="100px" UpdateText="Update">
					</telerik:GridEditCommandColumn>
					<telerik:GridButtonColumn ConfirmText="Delete This Price?" ConfirmDialogType="RadWindow" ConfirmTitle="Confirm Deletion" ButtonType="LinkButton" CommandName="Delete" Text="Delete" UniqueName="DeleteColumn"></telerik:GridButtonColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</telerik:RadAjaxPanel>
	<asp:LinqDataSource ID="SpeciesAbbreviationsDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="Species"></asp:LinqDataSource>
</asp:Content>