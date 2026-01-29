<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SiteSettings.aspx.cs" Inherits="FRASS.Admin.SiteSettings" %>
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
        OnItemDataBound="RadGrid1_ItemDataBound"
        DataSourceID="SitesDataSource" 
        ShowStatusBar="true">
        <MasterTableView ShowFooter="false" DataKeyNames="SiteSettingID" EditMode="InPlace">
            <Columns>
                <telerik:GridBoundColumn DataField="Setting" HeaderText="Setting" ReadOnly="true">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Value" HeaderText="Value">
                </telerik:GridBoundColumn>
                <telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn"
                    HeaderText="Edit" HeaderStyle-Width="100px" UpdateText="Update">
                </telerik:GridEditCommandColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    </telerik:RadAjaxPanel>
    <asp:LinqDataSource ID="SitesDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="SiteSettings" Select="new (SiteSettingID, Setting, Value)"></asp:LinqDataSource>
</asp:Content>
