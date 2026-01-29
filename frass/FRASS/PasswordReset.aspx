<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PasswordReset.aspx.cs" Inherits="FRASS.PasswordReset" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
            <div style="width: 100%;">
                <div class="roundedDiv">
                    <asp:Panel ID="Panel_Form" runat="server" Visible="false">
                        <div>
                            Complete this form to reset your password.
                        </div>
                        <div style="padding-top: 10px;">
                            <div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">First Name</div>                     
                            <div style="float: left;"><asp:TextBox ID="TextBox_FirstName" runat="server"></asp:TextBox></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                            <div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">Last Name</div>                     
                            <div style="float: left;"><asp:TextBox ID="TextBox_LastName" runat="server"></asp:TextBox></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                            <div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">Email Address</div>                     
                            <div style="float: left;"><asp:TextBox ID="TextBox_Email" runat="server"></asp:TextBox></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                            <div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">&nbsp;</div>                     
                            <div style="float: left;"><asp:Button ID="Button_Submit" runat="server" Text="Reset Password" OnClick="Button_Submit_Click" /></div>                     
                            <div style="float: left;"><asp:Button ID="Button_Cancel" runat="server" Text="Cancel" OnClick="Button_Cancel_Click" /></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="Panel_Reset" runat="server" Visible="false">
                        <div>
                            Enter in your new Password.
                        </div>
                        <div style="padding-top: 10px;">
                            <div style="float: left; width: 130px; padding-top: 3px; font-weight: normal;">New Password</div>                     
                            <div style="float: left;"><asp:TextBox ID="TextBox_Password1" runat="server" TextMode="Password"></asp:TextBox></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                            <div style="float: left; width: 130px; padding-top: 3px; font-weight: normal;">Confirm New Password</div>                     
                            <div style="float: left;"><asp:TextBox ID="TextBox_Password2" runat="server" TextMode="Password"></asp:TextBox></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                            <div style="float: left; width: 130px; padding-top: 3px; font-weight: normal;">&nbsp;</div>                     
                            <div style="float: left;"><asp:Button ID="Button_SaveNewPassword" runat="server" Text="Save New Password" OnClick="Button_SaveNewPassword_Click" /></div>                     
                            <div style="float: left;"><asp:Button ID="Button_Cancel2" runat="server" Text="Cancel" OnClick="Button_Cancel_Click" /></div>                     
                            <div style="clear: both; padding-top: 5px;"></div>
                        </div>
                    </asp:Panel>
                    <asp:Label ID="Label_Message" runat="server"></asp:Label>
                </div>
            </div>
        </telerik:RadAjaxPanel>
</asp:Content>

