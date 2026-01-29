<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StumpageReportStand.aspx.cs" Inherits="FRASS.WebUI.Parcels.StumpageReportStand" %>
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
							<div style="float: left; width: 175px; font-weight: bold;">Species</div>
							<div style="float: left; width: 100px; font-weight: bold;">Quality Code</div>
							<asp:Repeater ID="Repeater_Years_R1" OnItemDataBound="Repeater_Years_R1_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Year" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<asp:Repeater ID="Repeater_Species_R1" OnItemDataBound="Repeater_Species_R1_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 175px; font-weight: bold;"><asp:Label ID="Label_Species" runat="server"></asp:Label></div>
									<div style="clear: both;"></div>
									<asp:Repeater ID="Repeater_QualityCodes_R1" runat="server" OnItemDataBound="Repeater_QualityCodes_R1_ItemDataBound">
										<ItemTemplate>
											<div style="float: left; width: 175px; font-weight: normal;"><asp:Label ID="Label_CommonNames" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: bold;"><asp:Label ID="Label_QualityCodeNumber" runat="server"></asp:Label></div>
												<asp:Repeater ID="Repeater_FutureValueTBL_R1" OnItemDataBound="Repeater_FutureValueTBL_R1_ItemDataBound" runat="server">
													<ItemTemplate>
														<div style="float: left; width: 120px;">
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
							<div style="float: left; width: 275px; font-weight: bold;">Stand</div>
							<asp:Repeater ID="Repeater_Stand" runat="server" OnItemDataBound="Repeater_Stand_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 275px; font-weight: bold;">NET FV</div>
							<asp:Repeater ID="Repeater_NetFV" runat="server" OnItemDataBound="Repeater_NetFV_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 275px; font-weight: bold;">NPV</div>
							<asp:Repeater ID="Repeater_NPV" runat="server" OnItemDataBound="Repeater_NPV_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 175px; font-weight: bold;">Max NPV</div>
							<div style="float: left; width: 175px; font-weight: bold;">
								<asp:Label ID="Label_MaxNPV" runat="server"></asp:Label>
							</div>
						</div>
					</div>
				</div>
			</telerik:RadPageView>
			<telerik:RadPageView ID="RadPageView_R2" runat="server">			
				<div style="padding: 5px; background-color: #ffffff; min-height: 400px; padding-bottom: 10px;">
					<asp:DropDownList ID="DropDownList_YearOffset" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_YearOffset_SelectedIndexChanged"></asp:DropDownList>
					<div style="width: 1070px; overflow: auto; min-height: 400px;">
						<div style="width: 5320px;">
							<div style="float: left; width: 175px; font-weight: bold;">Species</div>
							<div style="float: left; width: 100px; font-weight: bold;">Quality Code</div>
							<asp:Repeater ID="Repeater_Years_R2" OnItemDataBound="Repeater_Years_R2_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Year" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<asp:Repeater ID="Repeater_Species_R2" OnItemDataBound="Repeater_Species_R2_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 150px; font-weight: bold;"><asp:Label ID="Label_Species" runat="server"></asp:Label></div>
									<div style="clear: both;"></div>
									<asp:Repeater ID="Repeater_QualityCodes_R2" runat="server" OnItemDataBound="Repeater_QualityCodes_R2_ItemDataBound">
										<ItemTemplate>
											<div style="float: left; width: 175px; font-weight: normal;"><asp:Label ID="Label_CommonNames" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: bold;"><asp:Label ID="Label_QualityCodeNumber" runat="server"></asp:Label></div>
												<asp:Repeater ID="Repeater_FutureValueTBL_R2" OnItemDataBound="Repeater_FutureValueTBL_R2_ItemDataBound" runat="server">
													<ItemTemplate>
														<div style="float: left; width: 120px;">
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
							<div style="float: left; width: 275px; font-weight: bold;">Stand</div>
							<asp:Repeater ID="Repeater_Stand_R2" runat="server" OnItemDataBound="Repeater_Stand_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 275px; font-weight: bold;">NET FV</div>
							<asp:Repeater ID="Repeater_NetFV_R2" runat="server" OnItemDataBound="Repeater_NetFV_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 275px; font-weight: bold;">NPV</div>
							<asp:Repeater ID="Repeater_NPV_R2" runat="server" OnItemDataBound="Repeater_NPV_R2_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="clear: both;"></div>
							<div style="float: left; width: 175px; font-weight: bold;">Max NPV</div>
							<div style="float: left; width: 175px; font-weight: bold;">
								<asp:Label ID="Label_MaxNPV_R2" runat="server"></asp:Label>
							</div>
						</div>
					</div>
				</div>
			</telerik:RadPageView>
			<telerik:RadPageView ID="RadPageView_SEV" runat="server">
				<div style="padding: 5px; background-color: #ffffff; min-height: 400px;">
				<div style="width: 1070px; overflow: auto; min-height: 400px;">
						<div style="width: 5320px;">
							<div style="float: left; width: 175px; font-weight: bold;">Species</div>
							<div style="float: left; width: 100px; font-weight: bold;">Quality Code</div>
							<asp:Repeater ID="Repeater_Years_SEV" OnItemDataBound="Repeater_Years_SEV_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Year" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<asp:Repeater ID="Repeater_Species_SEV" OnItemDataBound="Repeater_Species_SEV_ItemDataBound" runat="server">
								<ItemTemplate>
									<div style="float: left; width: 150px; font-weight: bold;"><asp:Label ID="Label_Species" runat="server"></asp:Label></div>
									<div style="clear: both;"></div>
									<asp:Repeater ID="Repeater_QualityCodes_SEV" runat="server" OnItemDataBound="Repeater_QualityCodes_SEV_ItemDataBound">
										<ItemTemplate>
											<div style="float: left; width: 175px; font-weight: normal;"><asp:Label ID="Label_CommonNames" runat="server"></asp:Label></div>
											<div style="float: left; width: 100px; font-weight: bold;"><asp:Label ID="Label_QualityCodeNumber" runat="server"></asp:Label></div>
												<asp:Repeater ID="Repeater_FutureValueTBL_SEV" OnItemDataBound="Repeater_FutureValueTBL_SEV_ItemDataBound" runat="server">
													<ItemTemplate>
														<div style="float: left; width: 120px;">
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
							<div style="float: left; width: 275px; font-weight: bold;">Stand</div>
							<asp:Repeater ID="Repeater_Stand_SEV" runat="server" OnItemDataBound="Repeater_Stand_SEV_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 275px; font-weight: bold;">NET FV</div>
							<asp:Repeater ID="Repeater_NetFV_SEV" runat="server" OnItemDataBound="Repeater_NetFV_SEV_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="float: left; width: 275px; font-weight: bold;">SEV</div>
							<asp:Repeater ID="Repeater_SEV" runat="server" OnItemDataBound="Repeater_SEV_ItemDataBound">
								<ItemTemplate>
									<div style="float: left; width: 120px;">
										<asp:Label ID="Label_Value" runat="server" Font-Bold="true"></asp:Label>
									</div>
								</ItemTemplate>
							</asp:Repeater>
							<div style="clear: both;"></div>
							<div style="clear: both;"></div>
							<div style="float: left; width: 175px; font-weight: bold;">Max NPV</div>
							<div style="float: left; width: 175px; font-weight: bold;">
								<asp:Label ID="Label_MaxSEV" runat="server"></asp:Label>
							</div>
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
						<asp:Repeater ID="Repeater_Redux" OnItemDataBound="Repeater_Redux_ItemDataBound" runat="server">
							<ItemTemplate>
								<div><asp:Label ID="Label_Title" runat="server"></asp:Label></div>
								<div style="padding-bottom: 10px;">
									<asp:Repeater ID="Repeater_Values_Redux" OnItemDataBound="Repeater_Values_Redux_ItemDataBound" runat="server">
										<ItemTemplate>
											<div style="float: left; font-weight: bold; width: 120px;">
												<asp:Label ID="Label_Total" runat="server" Font-Bold="true"></asp:Label>
											</div>
											<div style="float: left; font-weight: bold; width: 120px;">
												<asp:Label ID="Label_R1" runat="server" Font-Bold="true"></asp:Label>
											</div>
											<div style="float: left; font-weight: bold; width: 120px;">
												<asp:Label ID="Label_R2Year" runat="server" Font-Bold="true"></asp:Label>
											</div>
											<div style="float: left; font-weight: bold; width: 120px;">
												<asp:Label ID="Label_R2" runat="server" Font-Bold="true"></asp:Label>
											</div>
											<div style="float: left; font-weight: bold; width: 120px;">
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
