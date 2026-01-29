<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RPAPortfolio.aspx.cs" Inherits="FRASS.WebUI.MarketModel.RPAPortfolioPage" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		var StartTextBox;
		var EndTextBox;
		var RPALabel;
		var LongevityLabel;

		function GetPriceStart(startDateDropDown, endingDateDropDown, startTextBox, endTextBox, rpaLabel, longevityLabel, timberMarketID, logMarketReportSpeciesID) {
			StartTextBox = startTextBox;
			EndTextBox = endTextBox;
			RPALabel = rpaLabel;
			LongevityLabel = longevityLabel;
			$.ajax({
				type: "POST",
				url: "/MarketModel/RPAValue.ashx?WithPrice=0",
				data: {
					startDate: $('#' + startDateDropDown).val(),
					endingDate: $('#' + endingDateDropDown).val(),
					timberMarketID: timberMarketID,
					logMarketReportSpeciesID: logMarketReportSpeciesID
				},
				dataType: "json",
				traditional: true
			})
			.done(function (data) {
				$('#' + StartTextBox).val(data.BeginningRealValue);
				$('#' + RPALabel).html(data.RPA);
				$('#' + LongevityLabel).html(data.Longevity);
			});
		}

		function GetPriceEnd(startDateDropDown, endingDateDropDown, startTextBox, endTextBox, rpaLabel, longevityLabel, timberMarketID, logMarketReportSpeciesID) {
			StartTextBox = startTextBox;
			EndTextBox = endTextBox;
			RPALabel = rpaLabel;
			LongevityLabel = longevityLabel;
			$.ajax({
				type: "POST",
				url: "/MarketModel/RPAValue.ashx?WithPrice=0",
				data: {
					startDate: $('#' + startDateDropDown).val(),
					endingDate: $('#' + endingDateDropDown).val(),
					timberMarketID: timberMarketID,
					logMarketReportSpeciesID: logMarketReportSpeciesID
				},
				dataType: "json",
				traditional: true
			})
			.done(function (data) {
				$('#' + EndTextBox).val(data.EndingRealValue);
				$('#' + RPALabel).html(data.RPA);
				$('#' + LongevityLabel).html(data.Longevity);
			});
		}

		function GetRPA(startDateDropDown, endingDateDropDown, startTextBox, endTextBox, rpaLabel, longevityLabel, timberMarketID, logMarketReportSpeciesID) {
			StartTextBox = startTextBox;
			EndTextBox = endTextBox;
			RPALabel = rpaLabel;
			LongevityLabel = longevityLabel;
			$.ajax({
				type: "POST",
				url: "/MarketModel/RPAValue.ashx?WithPrice=1",
				data: {
					startDate: $('#' + startDateDropDown).val(),
					endingDate: $('#' + endingDateDropDown).val(),
					startingPrice: $('#' + StartTextBox).val(),
					endingPrice: $('#' + EndTextBox).val(),
					timberMarketID: timberMarketID,
					logMarketReportSpeciesID: logMarketReportSpeciesID
				},
				dataType: "json",
				traditional: true
			})
			.done(function (data) {
				$('#' + RPALabel).html(data.RPA);
				$('#' + LongevityLabel).html(data.Longevity);
			});
		}

		function LaunchChart(startDateDropDown, endingDateDropDown, startTextBox, endTextBox, rpaLabel, longevityLabel, timberMarketID, logMarketReportSpeciesID) {
			var startDate = $('#' + startDateDropDown).val();
			var endingDate = $('#' + endingDateDropDown).val();
			var rpa = $('#' + rpaLabel).html();
			var longevity = $('#' + longevityLabel).html();
			var startPrice = $('#' + startTextBox).val();
			var endPrice = $('#' + endTextBox).val();
			if ((eval(startPrice)) == 0 || eval(endPrice) == 0) {
				alert("Prices cannot be zero");
			}
			else {
				radopen("/Parcels/Charts/TimberMarketFutureChart.aspx?TimberMarketID=" + timberMarketID + "&StartDate=" + startDate + "&EndDate=" + endingDate + "&rpa=" + rpa + "&longevity=" + longevity + "&LogMarketReportSpeciesID=" + logMarketReportSpeciesID + "&StartPrice=" + startPrice + "&EndPrice=" + endPrice, "ChartWindow");
			}
			
		}

		function LaunchTimberMarketRPAChart(startDateDropDown, endingDateDropDown, startTextBox, endTextBox, rpaLabel, longevityLabel, timberMarketID, logMarketReportSpeciesID) {
			var startDate = $('#' + startDateDropDown).val();
			var endingDate = $('#' + endingDateDropDown).val();
			var rpa = $('#' + rpaLabel).html();
			var longevity = $('#' + longevityLabel).html();
			var startPrice = $('#' + startTextBox).val();
			var endPrice = $('#' + endTextBox).val();
			radopen("/Parcels/Charts/TimberMarketRPAChart.aspx?TimberMarketID=" + timberMarketID + "&StartDate=" + startDate + "&EndDate=" + endingDate + "&rpa=" + rpa + "&longevity=" + longevity + "&LogMarketReportSpeciesID=" + logMarketReportSpeciesID, "ChartWindow");
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server">
	</telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay">
	</telerik:RadAjaxLoadingPanel>
	<telerik:RadWindowManager runat="server" ID="RadWindowManager1" Behaviors="Close" Width="435px" Height="450px" ReloadOnShow="true"
		ShowContentDuringLoad="false" AutoSize="false" Modal="true" Skin="Hay" VisibleStatusbar="false" DestroyOnClose="false">
		<Windows>
			<telerik:RadWindow ID="ChartWindow" runat="server" Style="z-index: 9999;"
				ShowContentDuringLoad="false" Width="848px" Height="654px" Behaviors="Move, Close" Modal="true"
				VisibleStatusbar="false" AutoSize="true" Skin="Hay">
			</telerik:RadWindow>
			<telerik:RadWindow ID="ShareModel" runat="server" NavigateUrl="/Popups/SharePortfolio.aspx" Width="435px" Height="450px"></telerik:RadWindow>
		</Windows>
	</telerik:RadWindowManager>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
		<div style="padding-top: 10px;">
			<div style="width: 100%; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
				<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
					Delivered Log Market Real Price Appreciation Calculations
				</div>
				<div style="color: #000000;">
					<div style="padding: 5px;">
						<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
							<div style="width: 100%;">
								<div style="text-align: left; padding: 5px; background-color: #ffffff; border: 1px solid #000000; border-radius: 4px;">
									<div style="float: left; width: 100%;">
										<div style="float: left; padding-top: 4px; font-weight: bold;">Model Name:</div>
										<div style="float: left; padding-left: 10px;">
											<asp:TextBox ID="TextBox_RPAModelName" runat="server"></asp:TextBox>
										</div>
										<div style="float: left; padding-left: 5px; padding-top: 1px;">
											<asp:Button ID="Button_SaveEditRPAModel" runat="server" Text="Save Edits To Model" OnClick="Button_SaveEditRPAModel_Click" />
											<asp:Button ID="Button_SaveNewRPAModel" runat="server" Text="Save New Model" OnClick="Button_SaveNewRPAModel_Click" />
											<asp:Button ID="Button_Cancel" runat="server" Text="Cancel" OnClick="Button_Cancel_Click" />
										</div>
										<div style="clear: both;"></div>
									</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
		<div style="padding-top: 5px;">
			<div style="width: 100%; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
				<div style="padding: 5px;">
					<div style="border: 1px solid #000000; padding: 5px; background-color: #ffffff; border-radius: 4px;">
						<div style="background-color: #cccccc; padding: 2px;">
							<div style="float: left; width: 125px; font-weight: bold;">
								Species
							</div>
							<div style="float: left; width: 150px; font-weight: bold;">
								Sort/Grade
							</div>
							<div style="float: left; width: 200px; font-weight: bold;">
								Beginning Date
							</div>
							<div style="float: left; width: 150px; font-weight: bold;">
								Real Value <asp:Label ID="Label_RealValueDate1" runat="server"></asp:Label>
							</div>
							<div style="float: left; width: 200px; font-weight: bold;">
								Turning Point Date
							</div>
							<div style="float: left; width: 150px; font-weight: bold;">
								Real Value  <asp:Label ID="Label_RealValueDate2" runat="server"></asp:Label>
							</div>
							<div style="float: left; width: 125px; font-weight: bold;">
								Real Price Appreciation
							</div>
							<div style="float: left; width: 100px; font-weight: bold;">
								Longevity
							</div>
							<div style="float: left; width: 100px; font-weight: bold;">
								&nbsp;
							</div>
							<div style="clear: both; padding-top: 5px;"></div>
						</div>
						<asp:Repeater ID="Repeater_RPAMarketCalculations" runat="server" OnItemDataBound="Repeater_RPAMarketCalculations_ItemDataBound" EnableViewState="true">
							<AlternatingItemTemplate>
								<div style="background-color: #eeeeee;">
									<div style="float: left; width: 125px; font-weight: bold; padding-top: 2px;">
										<asp:HiddenField ID="HiddenField_LogMarketReportSpeciesID" runat="server" Value="0" />
										<asp:Label ID="Label_LogMarketSpecies" runat="server"></asp:Label>
									</div>
									<div style="float: left;">
										<asp:Repeater ID="Repeater_SortsRPA" runat="server" OnItemDataBound="Repeater_SortsRPA_ItemDataBound">
											<ItemTemplate>
												<div style="float: left; width: 150px;">
													<asp:HiddenField ID="HiddenField_TimberMarketID" runat="server" Value="0" />
													<asp:LinkButton ID="LinkButton_Sort" runat="server" ForeColor="Blue" Font-Underline="true" style="cursor: pointer;"></asp:LinkButton>
												</div>
												<div style="float: left; width: 200px;">
													<asp:DropDownList ID="DropDownList_BeginningDate" runat="server"></asp:DropDownList>
												</div>
												<div style="float: left; width: 150px;">
													<div style="float: left; font-weight: bold; padding-top: 5px;">
														$</div>
													<div style="float: left; padding-left: 1px; padding-right: 1px;">
														<asp:TextBox ID="TextBox_BeginningRealValue" runat="server" Width="75" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
													</div>
													<div style="clear: both;"></div>
												</div>
												<div style="float: left; width: 200px;">
													<asp:DropDownList ID="DropDownList_EndindingDate" runat="server"></asp:DropDownList>
												</div>
												<div style="float: left; width: 150px;">
													<div style="float: left; font-weight: bold; padding-top: 5px;">
														$</div>
													<div style="float: left; padding-left: 1px; padding-right: 1px;">
														<asp:TextBox ID="TextBox_EndingRealValue" runat="server" Width="75" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
													</div>
													<div style="clear: both;"></div>
												</div>
												<div style="float: left; width: 125px;">
													<asp:Label ID="Label_RPA" runat="server"></asp:Label>
												</div>
												<div style="float: left; width: 100px;">
													<asp:Label ID="Label_Longevity" runat="server"></asp:Label>
												</div>
												<div style="float: left; width: 100px;">
													<asp:LinkButton ID="LinkButton_ViewChart" runat="server" Text="View Chart"></asp:LinkButton>
												</div>
												<div style="clear: both; padding-top: 10px;">
												</div>
											</ItemTemplate>
										</asp:Repeater>
									</div>
									<div style="clear: both; padding-top: 10px;"></div>
								</div>
							</AlternatingItemTemplate>
							<ItemTemplate>
								<div style="background-color: #ffffff;">
									<div style="float: left; width: 125px; font-weight: bold; padding-top: 2px;">
										<asp:HiddenField ID="HiddenField_LogMarketReportSpeciesID" runat="server" Value="0" />
										<asp:Label ID="Label_LogMarketSpecies" runat="server"></asp:Label>
									</div>
									<div style="float: left;">
										<asp:Repeater ID="Repeater_SortsRPA" runat="server" OnItemDataBound="Repeater_SortsRPA_ItemDataBound" EnableViewState="true">
											<ItemTemplate>
												<div style="float: left; width: 150px;">
													<asp:HiddenField ID="HiddenField_TimberMarketID" runat="server" Value="0" />
													<asp:LinkButton ID="LinkButton_Sort" runat="server" ForeColor="Blue" Font-Underline="true" style="cursor: pointer;"></asp:LinkButton>
												</div>
												<div style="float: left; width: 200px;">
													<asp:DropDownList ID="DropDownList_BeginningDate" runat="server"></asp:DropDownList>
												</div>
												<div style="float: left; width: 150px;">
													<div style="float: left; font-weight: bold; padding-top: 5px;">
														$</div>
													<div style="float: left; padding-left: 1px; padding-right: 1px;">
														<asp:TextBox ID="TextBox_BeginningRealValue" runat="server" Width="75" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
													</div>
													<div style="clear: both;"></div>
												</div>
												<div style="float: left; width: 200px;">
													<asp:DropDownList ID="DropDownList_EndindingDate" runat="server"></asp:DropDownList>
												</div>
												<div style="float: left; width: 150px;">
													<div style="float: left; font-weight: bold; padding-top: 5px;">
														$</div>
													<div style="float: left; padding-left: 1px; padding-right: 1px;">
														<asp:TextBox ID="TextBox_EndingRealValue" runat="server" Width="75" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
													</div>
													<div style="clear: both;"></div>
												</div>
												<div style="float: left; width: 125px;">
													<asp:Label ID="Label_RPA" runat="server"></asp:Label>
												</div>
												<div style="float: left; width: 100px;">
													<asp:Label ID="Label_Longevity" runat="server"></asp:Label>
												</div>
												<div style="float: left; width: 100px;">
													<asp:LinkButton ID="LinkButton_ViewChart" runat="server" Text="View Chart"></asp:LinkButton>
												</div>
												<div style="clear: both; padding-top: 10px;">
												</div>
											</ItemTemplate>
										</asp:Repeater>
									</div>
									<div style="clear: both; padding-top: 10px;"></div>
								</div>
							</ItemTemplate>
						</asp:Repeater>
					</div>
				</div>
			</div>
		</div>
	</telerik:RadAjaxPanel>
</asp:Content>
