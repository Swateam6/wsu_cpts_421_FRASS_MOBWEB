<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="FRASS._default" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
		<div style="width: 100%;">
			<div class="contentwrapper">
				<div class="contentcolumn">
					<div class="roundedDiv" style="padding: 8px;">
						<h2>Welcome to the Forest Resource Analysis System Software (FRASS)</h2>
						<p>
							Developed by <b>Forest Econometrics</b>, FRASS is a professional decision-support and valuation system for forestland managers, investors, and agencies. It combines biometric precision with market economics to deliver <b>defensible</b>, <b>data-driven appraisals</b> and <b>economically optimal management schedules</b> for every timber stand and parcel.
						</p>
						<h3>FRASS integrates:</h3>
						<ul>
							<li>Tree-level data on species, size, density, and growth response</li>
							<li>Site factors such as soils, riparian buffers, and wildlife habitat</li>
							<li>Zoning and regulatory constraints</li>
							<li>Monthly updated delivered-log market data</li>
						</ul>
						<p>
							Together, these inputs produce transparent valuations, harvest schedules, and forecasts consistent with <b>highest-and-best-use economics</b>.
						</p>
						<p>
							The system is organized around <b>land parcels</b>—each composed of timber stands, roads, rivers, and other physical assets. Users may represent private timber companies, public agencies, investment trusts, or Tribal nations. FRASS analyzes how biological productivity and regional markets interact to define each parcel’s intrinsic and market value, offering a reliable foundation for long-term stewardship and financial planning.
						</p>
					</div>
				</div>
			</div>
			<div class="rightcolumn">
				<div>
					<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
						<asp:Panel ID="Panel_SiteLogin" runat="server" DefaultButton="Button_SiteCredentials">
							<table class="rfdLoginControl" cellspacing="0" cellpadding="1" style="border-style:Solid;border-collapse:collapse;">
								<tr>
									<td>
										<table cellpadding="0">
											<tr>
												<td align="center" colspan="2">Enter Site Credentials</td>
											</tr>
											<tr>
												<td align="right" valign="top" style="padding-top: 3px;">
													Site Password:
												</td>
												<td>
													<asp:TextBox ID="TextBox_SitePassword" runat="server" TextMode="Password"></asp:TextBox>
													<br /><asp:Label ID="Label_SitePassword_Error" runat="server" ForeColor="Red"></asp:Label>
												</td>
											</tr>
											<tr>
												<td align="right" colspan="2">
													<asp:Button ID="Button_SiteCredentials" runat="server" Text="Submit" OnClick="Button_SiteCredentials_Click" />
												</td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</asp:Panel>
						<asp:Panel ID="Panel_Login" runat="server" DefaultButton="Button_Login" Visible="false">
							<table class="rfdLoginControl" cellspacing="0" cellpadding="1" style="border-style:Solid;border-collapse:collapse;">
								<tr>
									<td>
										<table cellpadding="0">
											<tr>
												<td align="center" colspan="2">Enter User Credentials</td>
											</tr>
											<tr>
												<td align="right" valign="top" style="padding-top: 3px;">
													Email Address:
												</td>
												<td>
													<asp:TextBox ID="TextBox_Email" runat="server"></asp:TextBox>
												</td>
											</tr>
											<tr>
												<td align="right" valign="top" style="padding-top: 3px;">
													Password:
												</td>
												<td>
													<asp:TextBox ID="TextBox_Password" runat="server" TextMode="Password"></asp:TextBox>
													<br /><asp:Label ID="Label_Error" runat="server" ForeColor="Red"></asp:Label>
												</td>
											</tr>
											<tr>
												<td align="right" colspan="2">
													<asp:Button ID="Button_Login" runat="server" Text="Submit" OnClick="Button_Login_Click" />
												</td>
											</tr>
											<tr>
												<td align="left" valign="top" style="padding-top: 2px;">
													<asp:HyperLink ID="HyperLink_ResetPassword" runat="server" Text="Forgot Password" NavigateUrl="/PasswordReset.aspx"></asp:HyperLink>
												</td>
												<td style="padding-left: 2px; padding-top: 2px;">
													<asp:HyperLink ID="HyperLink_RequestAccount" runat="server" Text="Request Account" NavigateUrl="/RequestAccount.aspx"></asp:HyperLink>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								</table>
						</asp:Panel>
					</telerik:RadAjaxPanel>
				</div>
			</div>
			<div style="clear: both;"></div>
		</div>
</asp:Content>