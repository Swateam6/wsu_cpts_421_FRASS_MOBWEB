<%@ Page Title="Single Tree Analysis" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SingleTreeAnalysis.aspx.cs" Inherits="FRASS.SingleTreeAnalysis" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
        <div class="contentwrapper" style="display: flex;">
            <div class="contentcolumn" style="flex: 1;">
                <div class="roundedDiv">
                    <h2>Single Tree Analysis</h2>
                    <div style="display: flex; align-items: flex-start;">
                        <div style="flex: 1;">
                            <p>Enter the details for a single tree to calculate its merchandized log segments.</p>
                            <table style="width: 500px;">
                                <tr>
                                    <td style="width: 150px;"><asp:Label ID="Label_Species" runat="server" Text="Species:"></asp:Label></td>
                                    <td>
                                        <asp:DropDownList ID="DropDownList_Species" runat="server" Width="200px">
                                            <asp:ListItem Value="PY">PY</asp:ListItem>
                                            <asp:ListItem Value="GF">GF</asp:ListItem>
                                            <asp:ListItem Value="BM">BM</asp:ListItem>
                                            <asp:ListItem Value="WI">WI</asp:ListItem>
                                            <asp:ListItem Value="NF">NF</asp:ListItem>
                                            <asp:ListItem Value="WH">WH</asp:ListItem>
                                            <asp:ListItem Value="RC">RC</asp:ListItem>
                                            <asp:ListItem Value="RA">RA</asp:ListItem>
                                            <asp:ListItem Value="WP">WP</asp:ListItem>
                                            <asp:ListItem Value="DF">DF</asp:ListItem>
                                            <asp:ListItem Value="YC">YC</asp:ListItem>
                                            <asp:ListItem Value="MH">MH</asp:ListItem>
                                            <asp:ListItem Value="CH">CH</asp:ListItem>
                                            <asp:ListItem Value="RF">RF</asp:ListItem>
                                            <asp:ListItem Value="RW">RW</asp:ListItem>
                                            <asp:ListItem Value="OT">OT</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="Label_DBH" runat="server" Text="DBH (inches):"></asp:Label></td>
                                    <td><asp:TextBox ID="TextBox_DBH" runat="server" Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorDBH" runat="server" ControlToValidate="TextBox_DBH" ErrorMessage="DBH is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="CompareValidatorDBH" runat="server" ControlToValidate="TextBox_DBH" Operator="DataTypeCheck" Type="Double" ErrorMessage="DBH must be a number." ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="Label_Height" runat="server" Text="Height (feet):"></asp:Label></td>
                                    <td><asp:TextBox ID="TextBox_Height" runat="server" Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorHeight" runat="server" ControlToValidate="TextBox_Height" ErrorMessage="Height is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="CompareValidatorHeight" runat="server" ControlToValidate="TextBox_Height" Operator="DataTypeCheck" Type="Double" ErrorMessage="Height must be a number." ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="Label_CR" runat="server" Text="Crown Ratio:"></asp:Label></td>
                                    <td><asp:TextBox ID="TextBox_CR" runat="server" Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorCR" runat="server" ControlToValidate="TextBox_CR" ErrorMessage="Crown Ratio is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="CompareValidatorCR" runat="server" ControlToValidate="TextBox_CR" Operator="DataTypeCheck" Type="Double" ErrorMessage="Crown Ratio must be a number." ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="Label_CFV" runat="server" Text="CFV Target:"></asp:Label></td>
                                    <td><asp:TextBox ID="TextBox_CFV" runat="server" Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorCFV" runat="server" ControlToValidate="TextBox_CFV" ErrorMessage="CFV Target is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="CompareValidatorCFV" runat="server" ControlToValidate="TextBox_CFV" Operator="DataTypeCheck" Type="Double" ErrorMessage="CFV Target must be a number." ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="Label_StumpHeight" runat="server" Text="Stump Height (feet):"></asp:Label></td>
                                    <td><asp:TextBox ID="TextBox_StumpHeight" runat="server" Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidatorStumpHeight" runat="server" ControlToValidate="TextBox_StumpHeight" ErrorMessage="Stump Height is required." ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
                                        <asp:CompareValidator ID="CompareValidatorStumpHeight" runat="server" ControlToValidate="TextBox_StumpHeight" Operator="DataTypeCheck" Type="Double" ErrorMessage="Stump Height must be a number." ForeColor="Red" Display="Dynamic"></asp:CompareValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td><asp:Button ID="Button_Calculate" runat="server" Text="Calculate" OnClick="Button_Calculate_Click" />
                                        <asp:Button ID="Button_LoadRandom" runat="server" Text="Load Random Tree" OnClick="Button_LoadRandom_Click" CausesValidation="false" /></td>
                                </tr>
                            </table>
                            <br />
                            <asp:Label ID="Label_Error" runat="server" ForeColor="Red"></asp:Label>
                            <br />
                            <h3>Results</h3>
                            <p>Market as of date: <asp:Label ID="Label_MarketDate" runat="server" Visible="false"></asp:Label></p>
                            <asp:Literal ID="Literal_Results" runat="server"></asp:Literal>
                            <br />
                            <asp:Label ID="Label_FlexTaperBF" runat="server" Font-Bold="true"></asp:Label>
                            <br />
                            <asp:Label ID="Label_FVSBF" runat="server" Font-Bold="true"></asp:Label>
                            <br />
                        </div>
                        <div style="flex: 1; margin-left: 20px;">
                            <h3>Tree Diagram</h3>
                            <asp:Literal ID="Literal_Diagram" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:Label ID="Label_Copyright" runat="server" Visible="true" Text=&copy;<br />
        Forest Resource Analysis System Software<br />
        Western Washington Delivered Log Markets<br />
        https://frass.forest-econometrics.com/</asp:Label>
    </telerik:RadAjaxPanel>
</asp:Content>
