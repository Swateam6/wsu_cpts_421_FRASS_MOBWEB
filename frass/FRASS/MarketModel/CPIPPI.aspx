<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CPIPPI.aspx.cs" Inherits="FRASS.MarketModel.CPIPPI" %>
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
			OnUpdateCommand="RadGrid1_UpdateCommand"
			OnInsertCommand="RadGrid1_InsertCommand"
			OnItemDataBound="RadGrid1_ItemDataBound"
			OnDeleteCommand="RadGrid1_DeleteCommand"
			DataSourceID="MarketModelDataSource" 
			 AllowFilteringByColumn="true"
			ShowStatusBar="true">
			<MasterTableView ShowFooter="false" DataKeyNames="MarketModelDataID" EditMode="InPlace" CommandItemDisplay="TopAndBottom">
				<Columns>
					<telerik:GridTemplateColumn HeaderText="Data Type" DataField="MarketModelType.MarketModelType1">
						<ItemTemplate>
							 <%# DataBinder.Eval(Container.DataItem, "MarketModelType1")%>
						</ItemTemplate>
						<EditItemTemplate>
							<telerik:RadComboBox runat="server" ID="RadComboBox_MarketModelTypes" DataTextField="MarketModelType1"
								DataValueField="MarketModelTypeID" DataSourceID="MarketModelTypesDataSource" SelectedValue='<%#Bind("MarketModelTypeID") %>'>
							</telerik:RadComboBox>
						</EditItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridBoundColumn DataField="SeriesID" HeaderText="Series">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Year" HeaderText="Year">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Period" HeaderText="Period">
					</telerik:GridBoundColumn>
					<telerik:GridBoundColumn DataField="Value" HeaderText="Value">
					</telerik:GridBoundColumn>
					<telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn" 
						HeaderText="Edit" HeaderStyle-Width="100px" UpdateText="Update">
					</telerik:GridEditCommandColumn>
					<telerik:GridButtonColumn ConfirmText="Are you sure you want to delete this CPI/PPI Item?" ConfirmDialogType="RadWindow" ConfirmTitle="Confirm Deletion" ButtonType="LinkButton" CommandName="Delete" Text="Delete" UniqueName="DeleteColumn"></telerik:GridButtonColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</telerik:RadAjaxPanel>
	<asp:LinqDataSource ID="MarketModelDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="MarketModelDatas" Select="new (MarketModelDataID, SeriesID, Year, Period, Value, MarketModelTypeID, MarketModelType.MarketModelType1)"></asp:LinqDataSource>
	<asp:LinqDataSource ID="MarketModelTypesDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="MarketModelTypes" Select="new (MarketModelTypeID, MarketModelType1)"></asp:LinqDataSource>
</asp:Content>
