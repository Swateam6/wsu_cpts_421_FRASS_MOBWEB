<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="FRASS.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentwrapper">
        <div class="contentcolumn">
            <div class="roundedDiv">
                <h2>Error</h2>
                <asp:Label ID="Label_ErrorMessage" runat="server" ForeColor="Red"></asp:Label>
                <br /><br />
                <asp:HyperLink ID="HyperLink_Back" runat="server" NavigateUrl="~/default.aspx">Return to Home</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
