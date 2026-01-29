<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Portfolios.aspx.cs" Inherits="FRASS.MarketModel.Portfolios" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script type="text/javascript">
		function setCurrentNetValue(dlp, lc, hc, lha, cnv) {
			var dlp_tb = document.getElementById(dlp);
			var lc_tb = document.getElementById(lc);
			var hc_tb = document.getElementById(hc);
			var lha_tb = document.getElementById(lha);
			var cnv_tb = document.getElementById(cnv);
			var dlpValue = 0;
			if (isNumber(dlp_tb.value)) {
				dlpValue = eval(dlp_tb.value);
			}
			var lcValue = 0;
			if (isNumber(lc_tb.value)) {
				lcValue = eval(lc_tb.value);
			}
			var hcValue = 0;
			if (isNumber(hc_tb.value)) {
				hcValue = eval(hc_tb.value);
			}
			var lhaValue = 0;
			if (isNumber(lha_tb.value)) {
				lhaValue = eval(lha_tb.value);
			}

			cnv_tb.value = parseFloat(dlpValue - lcValue - hcValue - lhaValue, 2).toFixed(2);
		}

		function allocateQualityCodes() {
			var multipliers = <%= Multipliers %>;
			for(var m = 0; m < multipliers.length; m++) {
				var multi = multipliers[m];
				var haul3 = $(".Allocate_" + multi.StumpageGroupQualityCodeID + "_3");
				var haul4 = $(".Allocate_" + multi.StumpageGroupQualityCodeID + "_4");
				var haul5 = $(".Allocate_" + multi.StumpageGroupQualityCodeID + "_5");
				var val3 = parseFloat(haul3.val() * multi.HaulZone3,2).toFixed(2);
				var val4 = parseFloat(haul4.val() * multi.HaulZone4,2).toFixed(2);
				var val5 = parseFloat(haul5.val() * multi.HaulZone5,2).toFixed(2);
				haul3.val(val3);
				haul3.keyup();
				haul4.val(val4);
				haul4.keyup();
				haul5.val(val5);
				haul5.keyup();
			}
		}

		function setStumpageNetValue(haul, ona, cnv) {
			var haul_tb = document.getElementById(haul);
			var ona_tb = document.getElementById(ona);
			var cnv_tb = document.getElementById(cnv);

			var haulValue = 0;
			if (isNumber(haul_tb.value)) {
				haulValue = eval(haul_tb.value);
			}
			var onaValue = 0;
			if (isNumber(ona_tb.value)) {
				onaValue = eval(ona_tb.value);
			}
			cnv_tb.value = parseFloat(haulValue - onaValue,2).toFixed(2);
		}

		function setAllStumpageNetValues(ona, haul3, cnv3, haul4, cnv4, haul5, cnv5) {
			setStumpageNetValue(haul3, ona, cnv3);
			setStumpageNetValue(haul4, ona, cnv4);
			setStumpageNetValue(haul5, ona, cnv5);
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
			 VisibleStatusbar="false" AutoSize="true" Skin="Hay"
			 	></telerik:RadWindow>
			<telerik:RadWindow ID="ShareModel" runat="server" NavigateUrl="/Popups/SharePortfolio.aspx" Width="435px" Height="450px"></telerik:RadWindow>
		</Windows>
	</telerik:RadWindowManager>
	<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
		<script type="text/javascript">
			function confirmDeleteModel(button) {
				function aspUpdatePanelCallbackFn(arg) {
					if (arg) {
						__doPostBack(button.name, "");
					}
				}
				radconfirm("Are you sure you want to delete this Portfolio?", aspUpdatePanelCallbackFn, 330, 110, null, "Confirm");
			}
			
		</script>
	</telerik:RadScriptBlock>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
		<div>
			<asp:HiddenField ID="HiddenField_ModelType" runat="server" Value="0" />
			<asp:HiddenField ID="HiddenField_PortfolioID" runat="server" Value="0" />
			<asp:Panel ID="Panel_Header" runat="server">
				<div style="font-weight: bold;">
					Set Global Parameters</div>
				<div style="padding-top: 10px;">
					These will be used to drive the projections of your data forward in time. These
					control the time value of money.</div>
			</asp:Panel>
			<asp:Panel ID="Panel_FrontPage" runat="server" Visible="false">
				<div style="padding-top: 10px;">
					<telerik:RadDockLayout runat="server" ID="RadDockLayout1" Skin="Hay">
						<table>
							<tr>
								<td>
									<telerik:RadDockZone runat="server" ID="RadDockZone1" MinHeight="300" Width="1000" BorderStyle="None">
										<telerik:RadDock runat="server" ID="RadDock3" Title="Delivered Log Market Portfolio" EnableRoundedCorners="true" Pinned="true">
											<Commands>
												<telerik:DockExpandCollapseCommand />
											</Commands>
											<ContentTemplate>
												<div style="padding-top: 5px; padding-bottom: 5px;">
													<asp:Button ID="Button_StartNewDeliveredLogModel" runat="server" Text="Start New Delivered Log  Market Portfolio" OnClick="Button_StartNewDeliveredLogModel_Click"/>
												</div>
												<telerik:RadGrid ID="RadGrid_DeliveredLogModels" runat="server" 
													GridLines="None" AutoGenerateColumns="false"
													AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false"
													OnNeedDataSource="RadGrid_DeliveredLogModels_NeedDataSource" ShowStatusBar="false"
													OnItemDataBound="RadGrid_DeliveredLogModels_ItemDataBound"										>
													<PagerStyle Visible="false" />
													<MasterTableView ShowFooter="false" DataKeyNames="ModelID" CommandItemDisplay="None">
														<Columns>
															<telerik:GridTemplateColumn HeaderText="Model Name" ItemStyle-Width="300px">
																<ItemTemplate>
																	<asp:LinkButton ID="LinkButton_MarketModel" runat="server" OnCommand="LinkButton_MarketModel_Click" />
																</ItemTemplate>
															</telerik:GridTemplateColumn>
															<telerik:GridBoundColumn DataField="LastEdited" HeaderText="Created/Updated" DataFormatString="{0:MM/dd/yyyy}" ><ItemStyle HorizontalAlign="Left" Width="150"/></telerik:GridBoundColumn>
															<telerik:GridBoundColumn DataField="Creator" HeaderText="Creator"><ItemStyle HorizontalAlign="Left" Width="200"/></telerik:GridBoundColumn>
															<telerik:GridBoundColumn DataField="Editor" HeaderText="Editor"><ItemStyle HorizontalAlign="Left" Width="200" /></telerik:GridBoundColumn>
															<telerik:GridTemplateColumn HeaderText="Share It" ItemStyle-Width="75px">
																<ItemTemplate>
																	<asp:Button ID="Button_ShareMarketModel" runat="server" Text="Share" />
																</ItemTemplate>
															</telerik:GridTemplateColumn>
															<telerik:GridTemplateColumn HeaderText="Delete It" ItemStyle-Width="75px">
																<ItemTemplate>
																	<asp:Button ID="Button_DeleteMarketModel" runat="server" Text="Delete" OnClientClick="confirmDeleteModel(this); return false;" OnCommand="Button_DeleteMarketModel_Click"/>
																</ItemTemplate>
															</telerik:GridTemplateColumn>
														</Columns>
													</MasterTableView>
												</telerik:RadGrid>
											</ContentTemplate>
										</telerik:RadDock>
										<telerik:RadDock runat="server" ID="RadDock1" Title="RPA Portfolio" EnableRoundedCorners="true" Pinned="true">
											<Commands>
												<telerik:DockExpandCollapseCommand />
											</Commands>
											<ContentTemplate>
												<div style="padding-top: 5px; padding-bottom: 5px;">
													<asp:Button ID="Button_StartNewRPAPortfolio" runat="server" Text="Start New RPA  Market Portfolio" OnClick="Button_StartNewRPAPortfolio_Click"/>
												</div>
												<telerik:RadGrid ID="RadGrid_RPAPortfolios" runat="server" 
													GridLines="None" AutoGenerateColumns="false"
													AllowPaging="false" AllowSorting="false" AllowFilteringByColumn="false"
													OnNeedDataSource="RadGrid_RPAPortfolios_NeedDataSource" ShowStatusBar="false"
													OnItemDataBound="RadGrid_RPAPortfolios_ItemDataBound"								>
													<PagerStyle Visible="false" />
													<MasterTableView ShowFooter="false" DataKeyNames="ModelID" CommandItemDisplay="None">
														<Columns>
															<telerik:GridTemplateColumn HeaderText="Model Name" ItemStyle-Width="300px">
																<ItemTemplate>
																	<asp:LinkButton ID="LinkButton_RPAPortfolio" runat="server" OnCommand="LinkButton_RPAPortfolio_Click" />
																</ItemTemplate>
															</telerik:GridTemplateColumn>
															<telerik:GridBoundColumn DataField="LastEdited" HeaderText="Created/Updated" DataFormatString="{0:MM/dd/yyyy}" ><ItemStyle HorizontalAlign="Left" Width="150"/></telerik:GridBoundColumn>
															<telerik:GridBoundColumn DataField="Creator" HeaderText="Creator"><ItemStyle HorizontalAlign="Left" Width="200"/></telerik:GridBoundColumn>
															<telerik:GridBoundColumn DataField="Editor" HeaderText="Editor"><ItemStyle HorizontalAlign="Left" Width="200" /></telerik:GridBoundColumn>
															<telerik:GridTemplateColumn HeaderText="Share It" ItemStyle-Width="75px">
																<ItemTemplate>
																	<asp:Button ID="Button_ShareRPAPortfolio" runat="server" Text="Share" />
																</ItemTemplate>
															</telerik:GridTemplateColumn>
															<telerik:GridTemplateColumn HeaderText="Delete It" ItemStyle-Width="75px">
																<ItemTemplate>
																	<asp:Button ID="Button_DeleteRPAPortfolio" runat="server" Text="Delete" OnClientClick="confirmDeleteModel(this); return false;" OnCommand="Button_DeleteRPAPortfolio_Click"/>
																</ItemTemplate>
															</telerik:GridTemplateColumn>
														</Columns>
													</MasterTableView>
												</telerik:RadGrid>
											</ContentTemplate>
										</telerik:RadDock>
										<telerik:RadDock runat="server" ID="RadDock2" Title="Stumpage Market Portfolio" EnableRoundedCorners="true" Pinned="true" Visible="false">
											<Commands>
												<telerik:DockExpandCollapseCommand />
											</Commands>
											<ContentTemplate>
												<div style="padding-top: 5px; padding-bottom: 5px;">
													<asp:Button ID="Button_StartNewStumpageModel" runat="server" Text="Start New Stumpage Portfolio" OnClick="Button_StartNewStumpageModel_Click"/>
												</div>
												<asp:Repeater ID="Repeater_SavedStumpageLogModels" runat="server" OnItemDataBound="Repeater_SavedStumpageLogModels_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 150px; padding-top: 4px; padding-left: 10px;">
															<asp:LinkButton ID="LinkButton_StumpageModelName" runat="server" OnCommand="LinkButton_StumpageModelName_Click"></asp:LinkButton>
														</div>
														<div style="float: left;">
															<asp:Button ID="Button_ShareStumpageModel" runat="server" Text="Share" />
														</div>
														<div style="float: left;">
															<asp:Button ID="Button_DeleteStumpageModel" runat="server" Text="Delete"  OnClientClick="confirmDeleteModel(this); return false;" OnCommand="Button_DeleteStumpageModel_Click"/>
														</div>
														<div style="clear: both;"></div>
													</ItemTemplate>
												</asp:Repeater>
											</ContentTemplate>
										</telerik:RadDock>
									</telerik:RadDockZone>
								</td>
							</tr>
						</table>
					</telerik:RadDockLayout>
				</div>
			</asp:Panel>
			<asp:Panel ID="Panel_Inflation" runat="server" Visible="false">
				<div style="padding-top: 10px;">
					<div style="width: 600px; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
						<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
							Inflation Calculator</div>
						<div style="text-align: center; color: #ffffff;">
							Select the Beginning Year and the Ending year from the pull down lists below</div>
						<div style="padding: 5px;">
							<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
								<div style="float: left; width: 49%; text-align: center;">
									<div style="font-weight: bold;">
										Beginning Year</div>
									<div style="padding-top: 5px;">
										<asp:DropDownList ID="DropDownList_BeginningYear" runat="server" OnSelectedIndexChanged="DropDownList_Year_Changed"
											AutoPostBack="true">
										</asp:DropDownList>
									</div>
								</div>
								<div style="float: left; width: 49%; text-align: center;">
									<div style="font-weight: bold;">
										Ending Year</div>
									<div style="padding-top: 5px;">
										<asp:DropDownList ID="DropDownList_EndingYear" runat="server" OnSelectedIndexChanged="DropDownList_Year_Changed"
											AutoPostBack="true">
										</asp:DropDownList>
									</div>
								</div>
								<div style="clear: both;">
								</div>
							</div>
						</div>
						<div style="padding: 5px; color: #ffffff; text-align: center;">
							<div>
								<div style="float: left; padding-top: 5px; padding-left: 150px;">
									CPI
								</div>
								<div style="float: left; padding-left: 4px;">
									<asp:TextBox ID="TextBox_CPI" runat="server" MaxLength="5" Width="50px"></asp:TextBox>
								</div>
								<div style="float: left; padding-top: 5px; padding-left: 2px;">
									% Discount rate per year
								</div>
								<div style="clear: both;">
								</div>
								<div style="float: left; padding-top: 5px; padding-left: 150px;">
									PPI
								</div>
								<div style="float: left; padding-left: 4px;">
									<asp:TextBox ID="TextBox_PPI" runat="server" MaxLength="5" Width="50px"></asp:TextBox>
								</div>
								<div style="float: left; padding-top: 5px; padding-left: 2px;">
									% Inflation per year
								</div>
								<div style="clear: both;"></div>
							</div>
						</div>
					</div>
				</div>
				<div style="padding-top: 5px;">
					<div style="width: 600px; border: 1px solid #000000; padding: 5px; background-color: #B0BDAA; border-radius: 4px;">
						<div style="padding-left: 155px; font-weight: bold; padding-bottom: 5px;">
							Set global economic parameters
						</div>
						<div style="float: left; width: 275px; text-align: right; font-weight: bold; padding-top: 4px;">
							REAL Landowner Discount Rate (inflation neutral):
						</div>
						<div style="float: left; padding-left: 5px;">
							<asp:TextBox runat="server" ID="TextBox_CPIRate" Width="75px" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
						</div>
						<div style="clear: both;"></div>
						<div style="float: left; width: 275px; text-align: right; font-weight: bold; padding-top: 4px;">
							Long-term Rate of Inflation:
						</div>
						<div style="float: left; padding-left: 5px;">
							<asp:TextBox runat="server" ID="TextBox_PPIRate" Width="75px" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
						</div>
						<div style="clear: both; padding-top: 5px;">
						</div>
					</div>
				</div>
			</asp:Panel>
			<asp:Panel ID="Panel_ReforestationCosts" runat="server" Visible="false">
				<div style="padding-top: 10px;">
					<div style="width: 600px; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
						<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
							Reforestation Costs
						</div>
						<div style="color: #ffffff;">
							Current cost of reforestation per acre including site preparation, seedlings, and tree 
                            planting labor to bring each timber stand to free-to-grow status after reforestation initiates. 
                            This is the cost encumbered at of the beginning of each reforestation event.
						</div>
						<div style="padding: 5px;">
							<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
								<div>
									<div style="float: left; font-weight: bold; padding-top: 5px;">
										$</div>
									<div style="float: left; padding-left: 1px; padding-right: 1px;">
										<asp:TextBox ID="TextBox_ReforestationCosts" runat="server" Text="375" Width="35" onkeypress="return blockNonNumbers(this,event,false,false);"></asp:TextBox>
									</div>
									<div style="float: left; font-weight: bold; padding-top: 5px;">
										/ Acre</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</asp:Panel>
			<asp:Panel ID="Panel_RoadUseConstruction" runat="server" Visible="false">
				<div style="padding-top: 10px;">
					<div style="width: 600px; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
						<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
							Road Use and Construction Costs	
						</div>
						<div style="color: #ffffff;">
							<span style="font-weight: bold;">Access Fee</span><br />
							User Fees are paid to adjacent landowners for timber haul on forest roads for non-deeded access.
						</div>
						<div style="padding: 5px;">
							<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
								<div>
									<div style="float: left; text-align: right; font-weight: bold; padding-top: 5px; width: 80px; display: none;">
										Rock $
									</div>
									<div style="float: left; padding-left: 1px; padding-right: 1px;display: none;">
										<asp:TextBox ID="TextBox_AccessFeeRock" runat="server" Text="0.16" Width="35" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
									</div>
									<div style="float: left; font-weight: bold; padding-top: 5px;display: none;">
										/ CY / Mile ($ / Cubic Yard / Mile)</div>
									<div style="clear: both;"></div>
									<div style="float: left;  text-align: right; font-weight: bold; padding-top: 5px; width: 80px;">
										Timber $
									</div>
									<div style="float: left; padding-left: 1px; padding-right: 1px;">
										<asp:TextBox ID="TextBox_AccessFeeTimber" runat="server" Text="0.50" Width="35" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
									</div>
									<div style="float: left; font-weight: bold; padding-top: 5px;">
										/ MBF / Mile  ($ / MBF Timber Harvested / Mile of surface or main haul road)</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
						<div style="color: #ffffff;">
							<span style="font-weight: bold;">Road Maintenance Fee</span><br />
							Road Maintenance Fees are used to fund periodic road maintenance activities for access to properties for timber harvest activities. The following fees apply:
						</div>
						<div style="padding: 5px;">
							<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
								<div>
									<div style="float: left;  text-align: right; font-weight: bold; padding-top: 5px; width: 80px;display: none;">
										Rock Haul $
									</div>
									<div style="float: left; padding-left: 1px; padding-right: 1px;display: none;">
										<asp:TextBox ID="TextBox_MaintenanceFeeRock" runat="server" Text="0.56" Width="35" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
									</div>
									<div style="float: left; font-weight: bold; padding-top: 5px;display: none;">
										/ CY / Mile
									</div>
									<div style="clear: both;"></div>
									<div style="float: left; text-align: right; font-weight: bold; padding-top: 5px; width: 80px;">
										Timber Haul $
									</div>
									<div style="float: left; padding-left: 1px; padding-right: 1px;">
										<asp:TextBox ID="TextBox_MaintenanceFeeTimber" runat="server" Text="1.33" Width="35" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
									</div>
									<div style="float: left; font-weight: bold; padding-top: 5px;">
										/ MBF / Mile
									</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
						<div style="color: #ffffff;">
							<span style="font-weight: bold;">Construction Costs</span><br />
							Road construction costs will be applied to parcels without identified road access to surface roads or one of the Main Haul Roads. Road needs are identified only to provide access from the parcel to the closest road to the parcel (surface, Main Haul, or Paved Road). This management measure is intended for application on lands where silvicultural or forestry operations are planned or conducted. It is intended to apply to road construction/reconstruction operations for silvicultural purposes, including:
							<ol>
								<li>The clearing phase: clearing to remove trees and woody vegetation from the road right-of-way</li>
								<li>The pioneering phase: excavating and filling the slope to establish the road centerline and approximate grade</li>
								<li>The construction phase: final grade and road prism construction and bridge, culvert, and road drainage installation</li>
								<li>The surfacing phase: placement and compaction of the roadbed, road fill compaction, and surface placement and compaction (if applicable).</li>
							</ol>
							The road distances identified on the Parcel Reports of the FRASS program identify the straight-line distance from the center of the parcel to the nearest road. The actual road placement may not be a straight line, nor is the road's placement necessarily from the center of the parcel, nor is it necessarily to the road identified. Use these estimates for reference purposes only.
						</div>
						<div style="padding: 5px;">
							<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
								<div>
									<div style="float: left; text-align: right; font-weight: bold; padding-top: 5px;">
										Logging Road Construction Cost $
									</div>
									<div style="float: left; padding-left: 1px; padding-right: 1px;">
										<asp:TextBox ID="TextBox_LoggingRoadConstructionCost" runat="server" Text="19500" Width="55" onkeypress="return blockNonNumbers(this,event,false,false);"></asp:TextBox>
									</div>
									<div style="float: left; font-weight: bold; padding-top: 5px;">
										/ Mile
									</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</asp:Panel>
			<asp:Panel ID="Panel_DeliveredLogModel" runat="server" Visible="false">
				<div style="padding-top: 10px; margin-right: 10px;">
					<div style="width: 100%; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
						<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
							Delivered Log Model
						</div>
						<div style="color: #000000;">
							<div style="padding: 5px;">
								<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
									<div style="width: 100%;">
										<div style="text-align: left; padding: 5px; background-color: #ffffff; border: 1px solid #000000; border-radius: 4px;">
											<div style="float: left; width: 50%;">
												<div style="float: left;padding-top: 4px; font-weight: bold;">Model Name:</div>
												<div style="float: left; padding-left: 10px;"><asp:TextBox ID="TextBox_DeliveredLogModelName" runat="server"></asp:TextBox></div>
												<div style="float: left; padding-left: 5px; padding-top: 1px;">
													<asp:Button ID="Button_SaveEditsToModel" runat="server" Text="Save Edits To Model" OnClick="Button_SaveEditsToModel_Click" />
													<asp:Button ID="Button_SaveDeliveredLogModel" runat="server" Text="Save New Model" OnClick="Button_SaveMarketModel_Click" />
												</div>
												<div style="clear: both;"></div>
											</div>
											<div style="clear: both;"></div>
										</div>
									</div>
								</div>
							</div>
						</div>
						<div style="padding: 5px;">
							<div style="border: 1px solid #000000; padding: 5px; background-color: #ffffff; border-radius: 4px;">
								<div style="background-color: #cccccc; padding: 2px;">
									<div style="float: left; width: 125px; font-weight: bold;">
										Species
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Sort
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										Delivered Log Prices
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										Logging Costs
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										Hauling Costs
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										Overhead & Admin
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										Current Net Value
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										Profit & Risk
									</div>
									<div style="float: left; font-weight: bold;">
										Notes
									</div>
									<div style="clear: both; padding-top: 5px;"></div>
									<div style="float: left; width: 125px; font-weight: bold;">
										&nbsp;
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										&nbsp;
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										&nbsp;
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										<asp:TextBox ID="TextBox_LoggingCostsApply" runat="server" Width="55" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
										<asp:Button ID="Button_LoggingCostsApply" runat="server" Text="Apply" OnClick="Button_LoggingCostsApply_Click" />
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										<asp:TextBox ID="TextBox_HaulingCostsApply" runat="server" Width="55" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
										<asp:Button ID="Button_HaulingCostsApply" runat="server" Text="Apply" OnClick="Button_HaulingCostsApply_Click" />
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										<asp:TextBox ID="TextBox_OverheadAndAdminApply" runat="server" Width="55" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
										<asp:Button ID="Button_OverheadAndAdminApply" runat="server" Text="Apply" OnClick="Button_OverheadAndAdminApply_click" />
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										&nbsp;
									</div>
									<div style="float: left; width: 135px; font-weight: bold;">
										<asp:TextBox ID="TextBox_ProfitAndRiskApply" runat="server" Width="55" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
										<asp:Button ID="Button_ProfitAndRiskApply" runat="server" Text="Apply" OnClick="Button_ProfitAndRiskApply_Click" />
									</div>
									<div style="float: left; font-weight: bold;">
										<asp:TextBox ID="TextBox_NotesApply" runat="server" Width="125"></asp:TextBox>
										<asp:Button ID="Button_NotesApply" runat="server" Text="Apply" OnClick="Button_NotesApply_Click" />
									</div>
									<div style="clear: both; padding-top: 5px;"></div>
								</div>
								<asp:Repeater ID="Repeater_DeliveredLogModelSpecies" runat="server" OnItemDataBound="Repeater_DeliveredLogModelSpecies_ItemDataBound">
									<AlternatingItemTemplate>
										<div style="background-color: #eeeeee;">
											<div style="float: left; width: 125px; font-weight: bold; padding-top: 2px;">
												<asp:HiddenField ID="HiddenField_LogMarketReportSpeciesID" runat="server" Value="0" />
												<asp:Label ID="Label_LogMarketSpecies" runat="server"></asp:Label>
											</div>
											<div style="float: left;">
												<asp:Repeater ID="Repeater_DeliveredLogModelSpecies_Sorts" runat="server" OnItemDataBound="Repeater_DeliveredLogModelSpecies_Sorts_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 100px;">
															<asp:HiddenField ID="HiddenField_TimberMarketID" runat="server" Value="0" />
															<asp:Label ID="Label_TimberMarket" runat="server"></asp:Label>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_DeliveredLogPrices" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_LoggingCosts" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>															
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulingCosts" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>															
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_OverheadAndAdmin" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_CurrentNetValue" Width="55" EnableViewState="true" ReadOnly="true"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_ProfitAndRisk" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> %</div>
														</div>
														<div style="float: left; ">
															<asp:TextBox runat="server" ID="TextBox_Notes" Width="125" EnableViewState="true"></asp:TextBox>
														</div>
														<div style="clear: both; padding-top: 10px;"></div>
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
												<asp:Repeater ID="Repeater_DeliveredLogModelSpecies_Sorts" runat="server" OnItemDataBound="Repeater_DeliveredLogModelSpecies_Sorts_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 100px;">
															<asp:HiddenField ID="HiddenField_TimberMarketID" runat="server" Value="0" />
															<asp:Label ID="Label_TimberMarket" runat="server"></asp:Label>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_DeliveredLogPrices" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_LoggingCosts" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>															
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulingCosts" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>															
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_OverheadAndAdmin" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_CurrentNetValue" Width="55" EnableViewState="true" ReadOnly="true"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> / MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 135px;">
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_ProfitAndRisk" Width="55" EnableViewState="true" CssClass="numonly" onkeypress="return blockNonNumbers(this,event,true,false);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;"> %</div>
														</div>
														<div style="float: left; ">
															<asp:TextBox runat="server" ID="TextBox_Notes" Width="125" EnableViewState="true"></asp:TextBox>
														</div>
														<div style="clear: both; padding-top: 10px;"></div>
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
			</asp:Panel>



			<asp:Panel ID="Panel_StumpageModel" runat="server" Visible="false">
				<div style="padding-top: 10px; margin-right: 10px;">
					<div style="width: 100%; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
						<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
							Stumpage Model
						</div>
						<div style="color: #000000;">
							<div style="padding: 5px;">
								<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
									<div style="float: left; width: 525px; text-align: center;">
										<div style="text-align: left; padding: 5px; background-color: #ffffff; border: 1px solid #000000; border-radius: 4px;">
											Real Price Appreciation and Longevity Settings
										</div>
									</div>
									<div style="float: left; padding-left: 30px;">
										<div style="float: left; padding-left: 20px;  text-align: center;">
											<div style="font-weight: bold;">
												Select Beginning Year</div>
											<div style="padding-top: 5px;">
												<asp:DropDownList ID="DropDownList_StumpageModelStartingYear" runat="server" OnSelectedIndexChanged="DropDownList_StumpageModel_Changed"
													AutoPostBack="true">
												</asp:DropDownList>
											</div>
										</div>
										<div style="float: left; padding-left: 20px; text-align: center;">
											<div style="font-weight: bold;">
												Select Ending Year</div>
											<div style="padding-top: 5px;">
												<asp:DropDownList ID="DropDownList_StumpageModelEndingYear" runat="server" OnSelectedIndexChanged="DropDownList_StumpageModel_Changed"
													AutoPostBack="true">
												</asp:DropDownList>
											</div>
										</div>
										<div style="float: left; padding-left: 20px; text-align: center;">
											<div style="font-weight: bold;">&nbsp;
											</div>
											<div style="padding-top: 5px;">
												<asp:Button ID="Button_ApplyStumpageModel" runat="server" Text="Apply Suggested Values" OnClick="Button_ApplyStumpageModel_Click" />
											</div>
										</div>
										<div style="clear: both;"></div>
									</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
						<div style="padding: 5px;">
							<div style="border: 1px solid #000000; padding: 5px; background-color: #ffffff; border-radius: 4px;">
								<div style="background-color: #cccccc; padding: 2px;">
									<div style="float: left; width: 125px; font-weight: bold;">
										Species
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Quality Code
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul Zone 3
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul Zone 4
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul Zone 5
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Longevity of <br />RPA or RPD
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										Suggested Zone 3
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										Suggested Zone 4
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										Suggested Zone 5
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Suggested Longevity
									</div>
									<div style="clear: both; padding-top: 5px;"></div>
								</div>
								<asp:Repeater ID="Repeater_StumpageGroups" runat="server" OnItemDataBound="Repeater_StumpageGroups_ItemDataBound">
									<AlternatingItemTemplate>
										<div style="background-color: #eeeeee;">
											<div style="float: left; width: 125px; font-weight: bold; padding-top: 2px;">
												<asp:HiddenField ID="HiddenField_StumpageGroupID" runat="server" Value="0" />
												<asp:Label ID="Label_StumpageGroupName" runat="server"></asp:Label>
												<br />
												<asp:Label ID="Label_StumpageGroupSpeciesAbbreviations" runat="server" Font-Bold="false"></asp:Label>
											</div>
											<div style="float: left;">
												<asp:Repeater ID="Repeater_QualityCodes" runat="server" OnItemDataBound="Repeater_QualityCodes_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 100px;">
															<asp:HiddenField ID="QualityCodeID" runat="server" Value="0" />
															<asp:Label ID="Label_QualityCodeNumber" runat="server" Font-Bold="true">&nbsp;</asp:Label>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_HaulZone3" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_HaulZone4" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_HaulZone5" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_NewLongevityOfRPAOrRPD" Width="55" runat="server"></asp:TextBox>
														</div>
														<div style="float: left; width: 120px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedZone3" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 120px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedZone4" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 120px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedZone5" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedLongevity" Width="55" runat="server"></asp:TextBox>
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
												<asp:HiddenField ID="HiddenField_StumpageGroupID" runat="server" Value="0" />
												<asp:Label ID="Label_StumpageGroupName" runat="server"></asp:Label>
												<br />
												<asp:Label ID="Label_StumpageGroupSpeciesAbbreviations" runat="server" Font-Bold="false"></asp:Label>
											</div>
											<div style="float: left;">
												<asp:Repeater ID="Repeater_QualityCodes" runat="server" OnItemDataBound="Repeater_QualityCodes_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 100px;">
															<asp:HiddenField ID="QualityCodeID" runat="server" Value="0" />
															<asp:Label ID="Label_QualityCodeNumber" runat="server" Font-Bold="true">&nbsp;</asp:Label>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_HaulZone3" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_HaulZone4" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_HaulZone5" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_NewLongevityOfRPAOrRPD" Width="55" runat="server"></asp:TextBox>
														</div>
														<div style="float: left; width: 120px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedZone3" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 120px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedZone4" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 120px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedZone5" Width="55" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
														</div>
														<div style="float: left; width: 100px;">
															<asp:TextBox runat="server" ID="TextBox_SuggestedLongevity" Width="55" runat="server"></asp:TextBox>
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
			</asp:Panel>
			<asp:Panel ID="Panel_StumpageModelPage2" runat="server" Visible="false">
				<div style="padding-top: 10px; margin-right: 10px;">
					<div style="width: 100%; border: 1px solid #000000; padding: 5px; background-color: #2E501C; border-radius: 4px;">
						<div style="text-align: center; font-size: 16px; font-weight: bold; color: #ffffff;">
							Stumpage Model
						</div>
						<div style="color: #000000;">
							<div style="padding: 5px;">
								<div style="background-color: #B0BDAA; padding: 5px; border-radius: 4px;">
									<div style="float: left; width: 100%; text-align: center;">
										<div style="text-align: left; padding: 5px; background-color: #ffffff; border: 1px solid #000000; border-radius: 4px;">
											<div style="float: left; width: 50%;">
												<div style="float: left;padding-top: 4px; font-weight: bold;">Model Name:</div>
												<div style="float: left; padding-left: 10px;"><asp:TextBox ID="TextBox_StumpageModelName" runat="server"></asp:TextBox></div>
												<div style="float: left; padding-left: 5px; padding-top: 1px;">
													<asp:Button ID="Button_SaveEditsToStumpageModel" runat="server" Text="Save Edits To Model" OnClick="Button_SaveEditsToStumpageModel_Click" />
													<asp:Button ID="Button_SaveMarketStumpageModel" runat="server" Text="Save New Model" OnClick="Button_SaveMarketStumpageModel_Click" />
												</div>
												<div style="clear: both;"></div>
											</div>
											<div style="clear: both;"></div>
										</div>
									</div>
									<div style="clear: both;"></div>
								</div>
							</div>
						</div>
						<div style="padding: 5px;">
							<div style="border: 1px solid #000000; padding: 5px; background-color: #ffffff; border-radius: 4px;">
								<div style="background-color: #cccccc; padding: 2px;">
									<div style="float: left; width: 95px; font-weight: bold;">
										Species
									</div>
									<div style="float: left; width: 60px; font-weight: bold;">
										Quality
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul 3
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul 4
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul 5
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										Overhead & Admin
									</div>
									<div style="float: left; width: 300px; font-weight: bold; text-align: center;">
										Current Net Value
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										Profit & Risk
									</div>
									<div style="float: left; font-weight: bold;">
										Notes
									</div>
									<div style="clear: both; padding-top: 5px;"></div>
									<div style="float: left; width: 95px; font-weight: bold;">
										&nbsp;
									</div>
									<div style="float: left; width: 60px; font-weight: bold;">
										Code
									</div>
									<div style="float: left; width: 300px; font-weight: bold; text-align: center;">
										<input type="button" value="Allocate to Quality Codes" onclick="allocateQualityCodes()" />
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										<asp:TextBox ID="TextBox_ONAApply" runat="server" Width="45" CssClass="numonly"></asp:TextBox>
										<asp:Button ID="Button_ONAApply" runat="server" Text="Apply" OnClick="Button_ONAApply_Click" />
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul 3
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul 4
									</div>
									<div style="float: left; width: 100px; font-weight: bold;">
										Haul 5
									</div>
									<div style="float: left; width: 120px; font-weight: bold;">
										<asp:TextBox ID="TextBox_PNRApply" runat="server" Width="45" CssClass="numonly"></asp:TextBox>
										<asp:Button ID="Button_PNRApply" runat="server" Text="Apply" OnClick="Button_PNRApply_Click" />
									</div>
									<div style="float: left; font-weight: bold;">
										<asp:TextBox ID="TextBox_StumpageNotesApply" runat="server" Width="85"></asp:TextBox>
										<asp:Button ID="Button_StumpageNotesApply" runat="server" Text="Apply" OnClick="Button_StumpageNotesApply_Click" />
									</div>
									<div style="clear: both; padding-top: 5px;"></div>
								</div>
								<asp:Repeater ID="Repeater_StumpageGroups2" runat="server" OnItemDataBound="Repeater_StumpageGroups2_ItemDataBound">
									<AlternatingItemTemplate>
										<div style="background-color: #eeeeee;">
											<div style="float: left; width: 95px; font-weight: bold; padding-top: 2px;">
												<asp:HiddenField ID="HiddenField_StumpageGroupID" runat="server" Value="0" />
												<asp:Label ID="Label_StumpageGroupName" runat="server"></asp:Label>
												<br />
												<asp:Label ID="Label_StumpageGroupSpeciesAbbreviations" runat="server" Font-Bold="false"></asp:Label>
											</div>
											<div style="float: left;">
												<asp:Repeater ID="Repeater_QualityCodes2" runat="server" OnItemDataBound="Repeater_QualityCodes2_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 60px;">
															<asp:HiddenField ID="QualityCodeID" runat="server" Value="0" />
															<asp:Label ID="Label_QualityCodeNumber" runat="server" Font-Bold="true">&nbsp;</asp:Label>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone3" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone4" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone5" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 120px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_ONA" Width="50" runat="server"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone3CNV" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone4CNV" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone5CNV" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 120px;">
															<div style="float: left;padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_PR" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">%</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left;">
															<asp:TextBox runat="server" ID="TextBox_Notes" Width="125" EnableViewState="true"></asp:TextBox>
														</div>
														<div style="clear: both; padding-top: 10px;"></div>
													</ItemTemplate>
												</asp:Repeater>
											</div>
											<div style="clear: both; padding-top: 10px;"></div>
										</div>
									</AlternatingItemTemplate>
									<ItemTemplate>
										<div style="background-color: #eeeeee;">
											<div style="float: left; width: 95px; font-weight: bold; padding-top: 2px;">
												<asp:HiddenField ID="HiddenField_StumpageGroupID" runat="server" Value="0" />
												<asp:Label ID="Label_StumpageGroupName" runat="server"></asp:Label>
												<br />
												<asp:Label ID="Label_StumpageGroupSpeciesAbbreviations" runat="server" Font-Bold="false"></asp:Label>
											</div>
											<div style="float: left;">
												<asp:Repeater ID="Repeater_QualityCodes2" runat="server" OnItemDataBound="Repeater_QualityCodes2_ItemDataBound">
													<ItemTemplate>
														<div style="float: left; width: 60px;">
															<asp:HiddenField ID="QualityCodeID" runat="server" Value="0" />
															<asp:Label ID="Label_QualityCodeNumber" runat="server" Font-Bold="true">&nbsp;</asp:Label>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone3" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone4" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone5" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 120px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_ONA" Width="50" runat="server"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone3CNV" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone4CNV" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 100px;">
															<div style="float: left; font-weight: normal; padding-top: 5px;">$</div>
															<div style="float: left; padding-left: 1px; padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_HaulZone5CNV" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">/MBF</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left; width: 120px;">
															<div style="float: left;padding-right: 1px;">
																<asp:TextBox runat="server" ID="TextBox_PR" Width="50" EnableViewState="true" onkeypress="return blockNonNumbers(this,event,true,true);"></asp:TextBox>
															</div>
															<div style="float: left; font-weight: normal; padding-top: 5px;">%</div>
															<div style="clear: both;"></div>
														</div>
														<div style="float: left;">
															<asp:TextBox runat="server" ID="TextBox_Notes" Width="125" EnableViewState="true"></asp:TextBox>
														</div>
														<div style="clear: both; padding-top: 10px;"></div>
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
			</asp:Panel>
			<div style="padding-top: 5px;">
				<asp:Button ID="Button_Prev" runat="server" Text="Prev Page" OnClick="Button_Prev_Click" />
				<asp:Button ID="Button_Next" runat="server" Text="Next Page" OnClick="Button_Next_Click" />
				<asp:Button ID="Button_Cancel" runat="server" Text="Cancel" OnClick="Button_Cancel_Click" />
			</div>
		</div>
	</telerik:RadAjaxPanel>
</asp:Content>
