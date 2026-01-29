<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Species.aspx.cs" Inherits="FRASS.MarketModel.Species" %>
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
            OnUpdateCommand="RadGrid1_UpdateCommand"
            OnInsertCommand="RadGrid1_InsertCommand"
            OnItemDataBound="RadGrid1_ItemDataBound"
            DataSourceID="SpeciesDataSource" 
            ShowStatusBar="true">
            <MasterTableView ShowFooter="false" DataKeyNames="SpeciesID" EditMode="InPlace" CommandItemDisplay="TopAndBottom">
                <Columns>
                    <telerik:GridBoundColumn DataField="CommonName" HeaderText="Common Name">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Abbreviation" HeaderText="Abbreviation">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Latin" HeaderText="Latin" ItemStyle-Font-Italic="true">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LatinAbbreviation" HeaderText="Latin Abbreviation" ItemStyle-Font-Italic="true">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Taxa" HeaderText="Taxa">
                    </telerik:GridBoundColumn>
                    <telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn"
                        HeaderText="Edit" HeaderStyle-Width="100px" UpdateText="Update">
                    </telerik:GridEditCommandColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
    <asp:LinqDataSource ID="SpeciesDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="Species" Select="new (SpeciesID, CommonName, Abbreviation, Latin, LatinAbbreviation, Taxa)">
    </asp:LinqDataSource>
</asp:Content>