<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportStand.aspx.cs" Inherits="FRASS.WebUI.Parcels.ReportStand" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>   
	<telerik:RadAjaxManagerProxy ID="RadAjaxManager1" runat="server"></telerik:RadAjaxManagerProxy>
	<telerik:RadWindowManager ID="RadWindowManager1" runat="server" EnableShadow="true">
		<Windows>
			<telerik:RadWindow ID="RadWindow1" runat="server" Style="z-index: 9999"
				ShowContentDuringLoad="false" Width="848px" Height="654px" Behaviors="Move, Close" Modal="true"
			 VisibleStatusbar="false" AutoSize="true" Skin="Hay"
			 	></telerik:RadWindow>
		</Windows>
	</telerik:RadWindowManager>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<div style="float: left; width: 95px; font-weight: bold;">Parcel:</div>
	<div style="float: left; font-weight: bold;"><asp:Label ID="Label_Parcel" runat="server"></asp:Label></div>
	<div style="clear: both; padding-top: 5px;"><div>
	<div style="float: left; width: 95px; font-weight: bold;">Stand:</div>
	<div style="float: left; font-weight: bold;"><asp:Label ID="Label_Stand" runat="server"></asp:Label></div>
	<div style="clear: both; padding-top: 5px;"><div>
	<div style="float: left; width: 95px; font-weight: bold;">Market Model:</div>
	<div style="float: left; font-weight: bold;"><asp:Label ID="Label_MarketModel" runat="server"></asp:Label></div>
	<div style="clear: both; padding-top: 5px;"><div>
	<div style="float: left; width: 95px; font-weight: bold;">RPA Portfolio:</div>
	<div style="float: left; font-weight: bold;"><asp:Label ID="Label_RPAPortfolio" runat="server"></asp:Label></div>
	<div style="clear: both; padding-top: 5px;"><div>

	<telerik:RadTabStrip ID="RadTabStrip1" runat="server" Skin="Hay" MultiPageID="RadMultiPage1" SelectedIndex="0" Align="Justify" OnTabClick="RadTabStrip1_TabClick"
		ReorderTabsOnSelect="true" Width="600px" style="position: relative; z-index: 1000;">
		<Tabs>
			<telerik:RadTab Text="Rotation 1"></telerik:RadTab>
			<telerik:RadTab Text="Rotation 2"></telerik:RadTab>
			<telerik:RadTab Text="Perpetuity"></telerik:RadTab>
			<telerik:RadTab Text="Optimal Combination"></telerik:RadTab>
		</Tabs>
	</telerik:RadTabStrip>
	<div style="margin-top: -1px;">
		<div style="padding-right: 10px; min-height: 450px;">
		<telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" Width="1100" BorderWidth="1px" BorderColor="#3D556C" BorderStyle="Solid">
			<telerik:RadPageView ID="RadPageView_R1" runat="server">
				<div style="padding: 5px; background-color: #ffffff; min-height: 400px; padding-bottom: 10px">
					<div style="width: 1070px; overflow: auto; min-height: 400px;">
						<div style="width: 5320px;">
							<div style="float: left; width: 150px; font-weight: bold;">Sort</div>
							<div style="float: left; width: 100px; font-weight: bold;">Species Code</div>
							<div style="float: left; width: 100px; font-weight: bold;">Sort Code</div>
							<asp:Repeater ID="Repeater_Years_R1" OnItemDataBound="Repeater_Years_R1_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Year" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<asp:Repeater ID="Repeater_Species_R1" OnItemDataBound="Repeater_Species_R1_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 150px; font-weight: bold;"><asp:Label ID="Label_Species" runat="server"></asp:Label></div>
									<div style="clear: both;"></div>
									<asp:Repeater ID="Repeater_Sorts_R1" runat="server" OnItemDataBound="Repeater_Sorts_R1_ItemDataBound">
										<ItemTemplate>
											<div style="float: left; width: 150px; font-weight: normal;"><asp:Label ID="Label_Market" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: normal;"><asp:Label ID="Label_Abbreviation" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: normal;"><asp:Label ID="Label_SortCode" runat="server"></asp:Label></div>
												<asp:Repeater ID="Repeater_FutureValueTBL_R1" OnItemDataBound="Repeater_FutureValueTBL_R1_ItemDataBound" runat="server">
													<ItemTemplate>
														<div style="float: left; width: 120px; text-align: right;">
															<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
														</div>
													</ItemTemplate>
												</asp:Repeater>
											<div style="clear: both;"></div>
										</ItemTemplate>
									</asp:Repeater>
								</ItemTemplate>
							</asp:Repeater>
							<div style="padding-top: 10px;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Value-Cost</div>
							<asp:Repeater ID="Repeater_ValueCosts_R1" runat="server" OnItemDataBound="Repeater_ValueCosts_R1_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Additional Costs</div>
							<asp:Repeater ID="Repeater_FutureCosts_R1" runat="server" OnItemDataBound="Repeater_FutureCosts_R1_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Periodic NET Revenue</div>
							<asp:Repeater ID="Repeater_PeriodicNetRevenue_R1" runat="server" OnItemDataBound="Repeater_PeriodicNetRevenue_R1_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Acre Adjustment FV</div>
							<asp:Repeater ID="Repeater_AcreFV_R1" runat="server" OnItemDataBound="Repeater_AcreFV_R1_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">NPV</div>
							<asp:Repeater ID="Repeater_NPV_R1" runat="server" OnItemDataBound="Repeater_NPV_R1_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="clear: both; padding-top: 10px;"></div>
							<div style="float: left; font-weight: bold; width: 150px;"><asp:HyperLink ID="HyperLink_R1" runat="server" style="text-decoration: underline; color: Blue; cursor: pointer;">NPV Max</asp:HyperLink></div>
							<div style="float: left; font-weight: bold;"><asp:Label ID="Label_FVMaxR1" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>
							<div style="float: left; font-weight: bold; width: 150px;">Optimal Rotation Year</div>
							<div style="float: left; font-weight: bold;"><asp:Label ID="Label_R1" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>
						</div>
					</div>
				</div>
			</telerik:RadPageView>
			<telerik:RadPageView ID="RadPageView_R2" runat="server">			
				<div style="padding: 5px; background-color: #ffffff; min-height: 400px; padding-bottom: 10px;">
					<asp:DropDownList ID="DropDownList_YearOffset" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_YearOffset_SelectedIndexChanged"></asp:DropDownList>
					<div style="width: 1070px; overflow: auto; min-height: 400px;">
						<div style="width: 5320px;">
							<div style="float: left; width: 150px; font-weight: bold;">Sort</div>
							<div style="float: left; width: 100px; font-weight: bold;">Species Code</div>
							<div style="float: left; width: 100px; font-weight: bold;">Sort Code</div>
							<asp:Repeater ID="Repeater_Years_R2" OnItemDataBound="Repeater_Years_R2_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Year" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<asp:Repeater ID="Repeater_Species_R2" OnItemDataBound="Repeater_Species_R2_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 150px; font-weight: bold;"><asp:Label ID="Label_Species" runat="server"></asp:Label></div>
									<div style="clear: both;"></div>
									<asp:Repeater ID="Repeater_Sorts_R2" runat="server" OnItemDataBound="Repeater_Sorts_R2_ItemDataBound">
										<ItemTemplate>
											<div style="float: left; width: 150px; font-weight: normal;"><asp:Label ID="Label_Market" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: normal;"><asp:Label ID="Label_Abbreviation" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: normal;"><asp:Label ID="Label_SortCode" runat="server"></asp:Label></div>
												<asp:Repeater ID="Repeater_FutureValueTBL_R2" OnItemDataBound="Repeater_FutureValueTBL_R2_ItemDataBound" runat="server">
													<ItemTemplate>
														<div style="float: left; width: 120px; text-align: right;">
															<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
														</div>
													</ItemTemplate>
												</asp:Repeater>
											<div style="clear: both;"></div>
										</ItemTemplate>
									</asp:Repeater>
								</ItemTemplate>
							</asp:Repeater>
							<div style="padding-top: 10px;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Value-Cost</div>
							<asp:Repeater ID="Repeater_ValueCosts_R2" runat="server" OnItemDataBound="Repeater_ValueCosts_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Additional Costs</div>
							<asp:Repeater ID="Repeater_FutureCosts_R2" runat="server" OnItemDataBound="Repeater_FutureCosts_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Acre Adjustment FV</div>
							<asp:Repeater ID="Repeater_AcreFV_R2" runat="server" OnItemDataBound="Repeater_AcreFV_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">NPV</div>
							<asp:Repeater ID="Repeater_NPV_R2" runat="server" OnItemDataBound="Repeater_NPV_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="clear: both; padding-top: 10px;"></div>
							<div style="float: left; font-weight: bold; width: 150px;"><asp:HyperLink ID="HyperLink_R2" runat="server" style="text-decoration: underline; color: Blue; cursor: pointer;">NPV Max</asp:HyperLink></div>
							<div style="float: left; font-weight: bold;"><asp:Label ID="Label_FVMaxR2" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>
							<div style="float: left; font-weight: bold; width: 150px;">Optimal Rotation Length</div>
							<div style="float: left; font-weight: bold;"><asp:Label ID="Label_R2" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>
						</div>
					</div>
				</div>
			</telerik:RadPageView>
			<telerik:RadPageView ID="RadPageView_SEV" runat="server">
				<div style="padding: 5px; background-color: #ffffff; min-height: 400px;">
					<div style="width: 1070px; overflow: auto; min-height: 400px; padding-bottom: 10px;">
						<div style="width: 5320px;">
							<div style="float: left; width: 150px; font-weight: bold;">Sort</div>
							<div style="float: left; width: 100px; font-weight: bold;">Species Code</div>
							<div style="float: left; width: 100px; font-weight: bold;">Sort Code</div>
							<asp:Repeater ID="Repeater_Years" OnItemDataBound="Repeater_Years_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Year" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<asp:Repeater ID="Repeater_Species" OnItemDataBound="Repeater_Species_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 150px; font-weight: bold;"><asp:Label ID="Label_Species" runat="server"></asp:Label></div>
									<div style="clear: both;"></div>
									<asp:Repeater ID="Repeater_Sorts" runat="server" OnItemDataBound="Repeater_Sorts_ItemDataBound">
										<ItemTemplate>
											<div style="float: left; width: 150px; font-weight: normal;"><asp:Label ID="Label_Market" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: normal;"><asp:Label ID="Label_Abbreviation" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: normal;"><asp:Label ID="Label_SortCode" runat="server"></asp:Label></div>
												<asp:Repeater ID="Repeater_FutureValueTBL" OnItemDataBound="Repeater_FutureValueTBL_ItemDataBound" runat="server">
													<ItemTemplate>
														<div style="float: left; width: 120px; text-align: right;">
															<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
														</div>
													</ItemTemplate>
												</asp:Repeater>
											<div style="clear: both;"></div>
										</ItemTemplate>
									</asp:Repeater>
								</ItemTemplate>
							</asp:Repeater>
							<div style="padding-top: 10px;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Total Future Value Stand</div>
								<asp:Repeater ID="Repeater_TotalFutureValue" runat="server" OnItemDataBound="Repeater_TotalFutureValue_ItemDataBound">
									<ItemTemplate>
										<div style="float: left; width: 120px; text-align: right;">
											<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
										</div>
									</ItemTemplate>
								</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 350px; font-weight: bold;">Current SEV Value</div>
							<asp:Repeater ID="Repeater_CurrentSEVValue" runat="server" OnItemDataBound="Repeater_CurrentSEVValue_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px; text-align: right;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both; padding-top: 10px;"></div>
							<div style="float: left; font-weight: bold; width: 150px;"><asp:HyperLink ID="HyperLink_Sev" runat="server" style="text-decoration: underline; color: Blue; cursor: pointer;">SEV Rotation Optimum</asp:HyperLink></div>
							<div style="float: left; font-weight: bold;"><asp:Label ID="Label_SEVRotationOptimum" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>
							<div style="float: left; font-weight: bold; width: 150px;">Optimal Rotation Length</div>
							<div style="float: left; font-weight: bold;"><asp:Label ID="Label_OptimalRotationLength" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>
						</div>
					</div>
				</div>
			</telerik:RadPageView>
			<telerik:RadPageView ID="RadPageView_Redux" runat="server">
				<div style="padding: 5px; background-color: #ffffff; min-height: 400px; padding-bottom: 10px;">
					<div style="width: 1070px; overflow: auto; min-height: 400px;">
						<div style="width: 1000px;">
							<div style="float: left; font-weight: bold; width: 120px;">Total</div>
							<div style="float: left; font-weight: bold; width: 120px;">R1</div>
							<div style="float: left; font-weight: bold; width: 120px;">R2 Year</div>
							<div style="float: left; font-weight: bold; width: 120px;">R2</div>
							<div style="float: left; font-weight: bold; width: 120px;">SEV <asp:Label ID="Label_SEVYears" runat="server"></asp:Label></div>
							<div style="clear: both;"></div>  
							<asp:Repeater ID="Repeater_Grow0" OnItemDataBound="Repeater_Grow0_ItemDataBound" runat="server">
								<ItemTemplate>
									<div><asp:Label ID="Label_Title" runat="server"></asp:Label></div>
									<div style="padding-bottom: 10px;">
										<asp:Repeater ID="Repeater_Values_Redux" OnItemDataBound="Repeater_Values_Redux_ItemDataBound" runat="server">
											<ItemTemplate>
												<div style="float: left; font-weight: bold; width: 120px; text-align: left;">
													<asp:Label ID="Label_Total" runat="server" Font-Bold="true"></asp:Label>
												</div>
												<div style="float: left; font-weight: bold; width: 120px; text-align: left;">
													<asp:Label ID="Label_R1" runat="server" Font-Bold="true"></asp:Label>
												</div>
												<div style="float: left; font-weight: bold; width: 120px; text-align: left;">
													<asp:Label ID="Label_R2Year" runat="server" Font-Bold="true"></asp:Label>
												</div>
												<div style="float: left; font-weight: bold; width: 120px; text-align: left;">
													<asp:Label ID="Label_R2" runat="server" Font-Bold="true"></asp:Label>
												</div>
												<div style="float: left; font-weight: bold; width: 120px; text-align: left;">
													<asp:Label ID="Label_SEV" runat="server" Font-Bold="true"></asp:Label>
												</div>
												<div style="clear: both;"></div>  
											</ItemTemplate>
										</asp:Repeater>
									</div>
								</ItemTemplate>
							</asp:Repeater>
						</div>
					</div>
				</div>
			</telerik:RadPageView>
		</telerik:RadMultiPage>
		</div>
	</div>
	</telerik:RadAjaxPanel>
</asp:Content>
