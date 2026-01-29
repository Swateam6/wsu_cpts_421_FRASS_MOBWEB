<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricLogPrices.aspx.cs" Inherits="FRASS.WebUI.MarketModel.HistoricLogPrices" %>
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
		    OnPreRender="RadGrid1_PreRender"
			DataSourceID="HistoricLogPricesDataSource" 
			OnDeleteCommand="RadGrid1_DeleteCommand"
			AllowFilteringByColumn="false"
			ShowStatusBar="true">
			<MasterTableView ShowFooter="false" DataKeyNames="Year, Month, LogMarketReportSpeciesID" EditMode="InPlace" CommandItemDisplay="TopAndBottom">
				<Columns>
					<telerik:GridBoundColumn DataField="Year" HeaderText="Year"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Month" HeaderText="Month"></telerik:GridBoundColumn>
					<telerik:GridTemplateColumn HeaderText="Species" DataField="LogMarketSpeciesAbbreviations">
						<ItemTemplate>
							 <%# DataBinder.Eval(Container.DataItem, "LogMarketSpeciesAbbreviations")%>
						</ItemTemplate>
						<EditItemTemplate>
							<telerik:RadComboBox runat="server" ID="RadComboBox_LogMarketSpeciesAbbreviations" DataTextField="LogMarketSpeciesAbbreviations"
								DataValueField="LogMarketReportSpeciesID" DataSourceID="LogMarketSpeciesAbbreviationsDataSource" SelectedValue='<%#Bind("LogMarketReportSpeciesID") %>'>
							</telerik:RadComboBox>
						</EditItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridBoundColumn DataField="SMPrice" HeaderText="SM and Better" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Saw2Price" HeaderText="2 Saw" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Saw3Price" HeaderText="3 Saw" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Saw4Price" HeaderText="4 Saw" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Saw4CNPrice" HeaderText="4 Saw/CNS" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="PulpPrice" HeaderText="Pulp" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="CamprunPrice" HeaderText="Camprun" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Export12Price" HeaderText="Export 12''" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Export8Price" HeaderText="Export 8-12''" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="ChipPrice" HeaderText="Chip-N-Saw" DataFormatString="{0:C}"></telerik:GridBoundColumn>
					<telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn"
						HeaderText="" HeaderStyle-Width="100px" UpdateText="Update">
					</telerik:GridEditCommandColumn>
					<telerik:GridButtonColumn ConfirmText="Delete This Price?" ConfirmDialogType="RadWindow" ConfirmTitle="Confirm Deletion" ButtonType="LinkButton" CommandName="Delete" Text="Delete" UniqueName="DeleteColumn"></telerik:GridButtonColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</telerik:RadAjaxPanel>
	<asp:LinqDataSource ID="HistoricLogPricesDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="v_HistoricLogPrices" Select="new (Year, Month, LogMarketReportSpeciesID, LogMarketSpeciesAbbreviations,
	 SMPrice, Saw2Price, Saw3Price, Saw4Price, Saw4CNPrice, PulpPrice, CamprunPrice, Export12Price, Export8Price, ChipPrice)"></asp:LinqDataSource>
	 <asp:LinqDataSource ID="LogMarketSpeciesAbbreviationsDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="LogMarketReportSpecies" Select="new (LogMarketReportSpeciesID, LogMarketSpeciesAbbreviations)"></asp:LinqDataSource>
</asp:Content>