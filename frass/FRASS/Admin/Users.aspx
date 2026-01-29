<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="FRASS.Admin.Users" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
	<div style="padding-bottom: 5px;"><asp:Button runat="server" ID="Button_SendEmail" Text="Send Message To Users"  /></div>
	<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
	<asp:Label ID="Label_Message" runat="server"></asp:Label>
	<telerik:RadGrid ID="RadGrid1" GridLines="None" AutoGenerateColumns="false" PageSize="50"
		AllowPaging="true" AllowSorting="true" runat="server"
		OnUpdateCommand="RadGrid1_UpdateCommand"
		OnInsertCommand="RadGrid1_InsertCommand"
		OnItemDataBound="RadGrid1_ItemDataBound"
		OnDeleteCommand="RadGrid1_DeleteCommand"
		ShowStatusBar="true">
		<MasterTableView ShowFooter="false" DataKeyNames="UserID, UserTypeID" EditMode="InPlace" CommandItemDisplay="TopAndBottom">
			<Columns>
				<telerik:GridBoundColumn DataField="FirstName" HeaderText="First Name">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="LastName" HeaderText="Last Name">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="Email" HeaderText="Email">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="Company" HeaderText="Company" UniqueName="Company">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="PhoneNumber" HeaderText="Phone" UniqueName="PhoneNumber">
				</telerik:GridBoundColumn>
				<telerik:GridTemplateColumn UniqueName="UserTypeID" HeaderText="User Type" SortExpression="UserType1">
					<ItemTemplate>
						<%# DataBinder.Eval(Container.DataItem, "UserType1") %>
					</ItemTemplate>
					<EditItemTemplate>
						<telerik:RadComboBox runat="server" ID="RadComboBox_UserTypeID" DataTextField="UserType1"
							DataValueField="UserTypeID" DataSourceID="UserTypeDataSource" SelectedValue='<%#Bind("UserTypeID") %>'>
						</telerik:RadComboBox>
					</EditItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridTemplateColumn UniqueName="UserStatusID" HeaderText="Status" SortExpression="UserStatus">
					<ItemTemplate>
						<%# DataBinder.Eval(Container.DataItem, "Status") %>
					</ItemTemplate>
					<EditItemTemplate>
						<telerik:RadComboBox runat="server" ID="RadComboBox_UserStatusID" DataTextField="Status"
							DataValueField="UserStatusID" DataSourceID="UserStatusDataSource" SelectedValue='<%#Bind("UserStatusID") %>'>
						</telerik:RadComboBox>
					</EditItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridEditCommandColumn FooterText="EditCommand footer" UniqueName="EditCommandColumn"
					HeaderText="Edit" HeaderStyle-Width="100px" UpdateText="Update">
				</telerik:GridEditCommandColumn>
				<telerik:GridTemplateColumn>
					<EditItemTemplate>
						<asp:Button ID="Button_PasswordReset" runat="server" Text="Reset Password" OnCommand="Button_PasswordReset_Click"></asp:Button>
					</EditItemTemplate>
				</telerik:GridTemplateColumn>
				<telerik:GridButtonColumn FooterText="DeleteCommand footer" UniqueName="DeleteCommandColumn"
					HeaderText="Delete" HeaderStyle-Width="100px" ButtonType="LinkButton" Text="Delete" CommandName="Delete" ConfirmText="Are you sure you want to delete this user?">
				</telerik:GridButtonColumn>
			</Columns>
		</MasterTableView>
	</telerik:RadGrid>
	</telerik:RadAjaxPanel>
	<asp:LinqDataSource ID="UsersDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="Users" Select="new (UserID, FirstName, LastName, Email, UserTypeID, UserType.UserType1, UserStatusID, UserStatuse.Status, Company, PhoneNumber)"></asp:LinqDataSource>
	<asp:LinqDataSource ID="UsersDataSourceActive" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" Where="UserStatusID !=5 " TableName="Users" Select="new (UserID, FirstName, LastName, Email, UserTypeID, UserType.UserType1, UserStatusID, UserStatuse.Status, Company, PhoneNumber)"></asp:LinqDataSource>
	<asp:LinqDataSource ID="UserTypeDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="UserTypes" Select="new (UserTypeID, UserType1)"></asp:LinqDataSource>
	<asp:LinqDataSource ID="UserStatusDataSource" runat="server" ContextTypeName="FRASS.DAL.Context.FRASSDataContext" TableName="UserStatuses" Select="new (UserStatusID, Status)"></asp:LinqDataSource>
	<telerik:RadWindowManager ID="RadWindowManager1" Behaviors="Close" runat="server" Width="410px" Height="420px" ReloadOnShow="true"
		ShowContentDuringLoad="false" AutoSize="false" Modal="true" Skin="Hay" VisibleStatusbar="false" DestroyOnClose="false"
		>
		<Windows>
			<telerik:RadWindow ID="SendMessage" runat="server" Title="Send Users A Message" NavigateUrl="/Popups/SendEmail.aspx"></telerik:RadWindow>
		</Windows>
	</telerik:RadWindowManager>
</asp:Content>
