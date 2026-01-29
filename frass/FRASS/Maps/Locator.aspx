<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Locator.aspx.cs" Inherits="FRASS.WebUI.Maps.IndexGuide" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        $(".group1").colorbox({ rel: 'group1', transition: "fade", maxWidth: "95%", maxHeight: "95%" });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<div style="font-size: 1.2em; text-align: left;">
		<div style="display: flex; flex-direction: row;">
			<div style="width: 38%; margin-right: 2%;">
				<div class="roundedDiv" style="padding: 5px 10px 20px 15px; ">
					<h2 style="padding-bottom:20px">FRASS Demonstration Environment — Pack Forest, Washington</h2>
					<h3>Exploring the interface between forest biology and economics</h3>
					<p>
						The FRASS Demonstration Environment allows users to explore how forestland value is created at the intersection of biology, management, and markets.
					</p>
					<p>
						Each user type views this map differently — foresters see stands and site productivity, appraisers see market zones and comparable sales, and landowners see the long-term earning potential of their properties.
					</p>
					<p>
						Use the map to visualize place, context, and function. In production, your workspace will host secure, client-specific data for your parcels, timber stands, roads, and hydrology layers.
					</p>
					<h3 style="padding-top: 30px;">Templates</h3>
					<p>
						<b>1) Forestland Owner – "Where are my parcels?"</b><br />
						Use this view to orient stakeholders to your property's geography. Turn on Parcels and Stands; label key features (gates, landings, stream buffers).<br />
						<i>Tip: Export a high-contrast map for board packets and lender reviews.</i>
					</p>
					<p>
						<b>2) Licensed Appraiser – "Where I provide appraisals"</b><br />
						Shade your service region and pin recent comparable sale areas. Add the Delivered-Log Market overlay to show market context.<br />
						<i>Tip: Save as "Appraisal Service Area – [Your Firm]" and reuse.</i>
					</p>
					<p>
						<b>3) Forestry Consultant – "The forestlands I serve"</b><br />
						Outline your operational footprint (counties, ownerships, timber types). Display access roads and known haul corridors to signal practical capability.<br />
						<i>Tip: Pair with a short note on how FRASS uses monthly market data for value forecasts.</i>
					</p>
				</div>
			</div>
			<div style="width: 56%; position: relative;">
				<div style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; display: flex; flex-direction: column; align-items: center;">
					<a class="group1" href="/Images/Index_Maps/FRASS_Overview.png" title="FRASS Demonstration Environment" style="flex: 1; display: flex; align-items: center; justify-content: center; overflow: hidden; width: 100%;">
						<asp:Image ID="Image1" runat="server" ImageUrl="/Images/Index_Maps/FRASS_Overview.png" style="max-height: 100%; max-width: 100%; width: auto; height: auto;" />
					</a>
					<div style="text-align: center; font-style: italic; margin-top: 5px;">FRASS Demonstration Environment (Click to enlarge)</div>
				</div>
			</div>
		</div>
		<div class="roundedDiv" style="padding-left: 15px; margin-top: 20px; width: 65%">
			<h3>Demo note</h3>
			<p>
				This is a Demonstration site with curated content. Your production workspace will display your secure layers and saved regions.
			</p>
			<h3>License & Display Terms</h3>
			<p>
				By using this page within your business outreach, you agree:
			</p>
			<ul>
				<li>You may display data that you own or are licensed to show to your clients.</li>
				<li>Client-facing use is permitted within your FRASS license; public redistribution (posting maps/data outside client deliverables) requires written permission from Forest Econometrics.</li>
				<li>Screenshots/exports must retain the FRASS and Forest Econometrics attribution in the footer.</li>
			</ul>
			<p>
				If you need broader display rights, contact support to amend your license.
			</p>

			<hr style="margin: 25px 0;" />

			<div class="frass-about">
    			<h3><b>About the FRASS Demonstration Environment</b></h3>

				<p>
        			The FRASS Demonstration Environment represents the public-facing entry point to a larger analytical system developed over more than three decades at the intersection of forest biology, management decisions, and market economics.
    			</p>

				<p>
        			FRASS was conceived, designed, and is owned by <b>D&D Larix, LLC</b>, with core system architecture, economic modeling, delivered log market analytics, and valuation logic developed under the direction of <b>William E. Schlosser, PhD</b>, Founder and Principal Economist. Early system design and implementation were made possible through close collaboration with <b>John Doroshenk</b>, whose programming and systems architecture work established much of the durable foundation on which FRASS continues to evolve. Geospatial integration and applied spatial analysis were further strengthened through contributions by <b>Joe Mierzwinski</b> and other collaborators working within the Larix framework.
    			</p>

				<p>
        			In recent years, FRASS development has expanded through collaboration with <b>faculty-guided Capstone teams from the School of Electrical Engineering and Computer Science at Washington State University</b>. These teams have contributed to deployed system components now operating within FRASS, including the Flex Taper integration and enhancements to core analytical and database infrastructure. This collaboration supports continued system integration and refinement while preserving the integrity of FRASS's underlying biological and economic foundations.
				</p>

				<p>
        			The <b>current WSU Engineering Capstone team (2025–26)</b> is actively contributing to multiple areas of the FRASS platform. Their work reflects a high level of technical engagement and is helping advance both the analytical capabilities of FRASS and the professional development of the students involved. This demonstration site contains curated data and illustrative workflows; production environments operate on secure, client-specific datasets under licensed use.
    			</p>

		</div>
	</div>
</asp:Content>
