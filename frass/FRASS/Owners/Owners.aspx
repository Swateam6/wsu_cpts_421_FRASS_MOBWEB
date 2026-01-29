<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Owners.aspx.cs" Inherits="FRASS.Owners.Owners" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<asp:Label ID="Label_Message" runat="server"></asp:Label>
	<telerik:RadGrid ID="RadGrid1" GridLines="None" AutoGenerateColumns="false" PageSize="50"
		AllowPaging="true" AllowSorting="true" runat="server"
		DataSourceID="OwnersDataSource" AllowFilteringByColumn="true"
		OnUpdateCommand="RadGrid1_UpdateCommand"
		OnInsertCommand="RadGrid1_InsertCommand"
		OnItemDataBound="RadGrid1_ItemDataBound"
		ShowStatusBar="true">
		<PagerStyle Position="TopAndBottom" />
		<MasterTableView ShowFooter="false" DataKeyNames="OwnerID" EditMode="InPlace" CommandItemDisplay="TopAndBottom">
			<DetailTables>
				<telerik:GridTableView DataSourceID="ParcelsDataSource" DataKeyNames="OwnerID" Name="OwnerParcels" Width="100%" AllowFilteringByColumn="false">
					<ParentTableRelation>
						<telerik:GridRelationFields DetailKeyField="OwnerID" MasterKeyField="OwnerID" />
					</ParentTableRelation>
					<Columns>
						<telerik:GridHyperLinkColumn DataTextField="ParcelNumber" HeaderText="Parcel Number" DataNavigateURLFormatString="/Parcels/Report.aspx?ParcelID={0}" DataNavigateUrlFields="ParcelID" SortExpression="ParcelNumber" DataType="System.String"></telerik:GridHyperLinkColumn>
						<telerik:GridNumericColumn  DataField="Acres" HeaderText="Acres" NumericType="Number" DataType="System.Decimal" DecimalDigits="1" AllowRounding="true" DataFormatString="{0:N1}"><ItemStyle HorizontalAlign="Right" /></telerik:GridNumericColumn>
							<telerik:GridBoundColumn DataField="Legal" HeaderText="Legal"><ItemStyle HorizontalAlign="Left" />
							</telerik:GridBoundColumn>
						</Columns>
					</telerik:GridTableView>
				</DetailTables>
			<Columns>
				<telerik:GridBoundColumn DataField="Name" HeaderText="Name"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="Address" HeaderText="Address"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="City" HeaderText="City"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridTemplateColumn UniqueName="StateID" HeaderText="State" SortExpression="StateID">
					<ItemTemplate>
						<%# DataBinder.Eval(Container.DataItem, "StateInitial") %>
					</ItemTemplate>
					<EditItemTemplate>
						<telerik:RadComboBox runat="server" ID="RadComboBox_States" DataTextField="StateInitial"
							DataValueField="StateID" DataSourceID="StatesDataSource" SelectedValue='<%#Bind("StateID") %>'>
						</telerik:RadComboBox>
					</EditItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridBoundColumn DataField="Zip" HeaderText="Zip"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="Zip4" HeaderText="Zip 4"><ItemStyle HorizontalAlign="Left" />
				</telerik:GridBoundColumn>
				<telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn"
					HeaderText="Edit" HeaderStyle-Width="100px" UpdateText="Update">
				</telerik:GridEditCommandColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
	</telerik:RadAjaxPanel>
	<asp:LinqDataSource ID="OwnersDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="Owners" Select="new (OwnerID, Name, Address, City, StateID, State.StateInitial,Zip, Zip4)">
		<SelectParameters>
			<asp:Parameter Name="OwnerID" />
		</SelectParameters>
	</asp:LinqDataSource>
	<asp:LinqDataSource ID="StatesDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="States" Select="new (StateID, StateInitial)"></asp:LinqDataSource>
	<asp:LinqDataSource ID="ParcelsDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" OnSelecting="ParcelsDataSource_Selecting" Where="OwnerID == @OwnerID" >
		<WhereParameters>
			<asp:Parameter Name="OwnerID" Type="Int32" />
		</WhereParameters>
	</asp:LinqDataSource>
</asp:Content>