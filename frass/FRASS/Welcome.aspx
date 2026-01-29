<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="FRASS.Welcome" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server"></telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
		<div class="contentwrapper">
			<div class="contentcolumn2">
				<div class="roundedDiv" style="padding-left:6px;">
					<h2>Welcome to the Forest Resource Analysis System Software (FRASS)</h2>
					<p>
						FRASS is a professional decision-support and valuation system built by <b>Forest Econometrics</b>. It unites biometric precision with market economics to produce <b>defensible appraisals</b> and <b>economically optimal management schedules</b> at the stand and parcel level.
					</p>
					<h3>What FRASS integrates</h3>
					<ul>
						<li>Tree metrics: species, size, density, growth response</li>
						<li>Site factors: soils productivity, riparian buffers, habitat considerations</li>
						<li>Regulatory context: zoning and protections</li>
						<li><b>Monthly updated</b> delivered-log market data</li>
					</ul>
					<p>
						These inputs work together to forecast value, prescribe management, and estimate market price consistent with <b>highest-and-best-use</b> principles.
					</p>
					<h3>Who uses FRASS</h3>
					<p>
						Commercial timber companies, public agencies, forestry consultants, investment trusts, and Tribal nations—anyone needing transparent, auditable decisions about forestland value and management.
					</p>
				</div>
			</div>
		</div>
		<div class="leftcolumn">
			<div class="roundedDiv" style="text-align: center;">
				<asp:Image ID="Image3" ImageUrl="/images/FRASS-logo-cutbg.png" BorderStyle="None" Style="width:100%;" runat="server" AlternateText="Forest Econometrics" /><br />
				<asp:HyperLink ID="HyperLink_ForesetEconometrics" runat="server" Target="_blank" NavigateUrl="http://forest-econometrics.com" ForeColor="Blue">http://forest-Econometrics.com</asp:HyperLink>
			</div>
		</div>
		<div style="clear: both;"></div>
	</telerik:RadAjaxPanel>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>

</asp:Content>