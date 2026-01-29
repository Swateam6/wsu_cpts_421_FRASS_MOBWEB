<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RequestAccount.aspx.cs" Inherits="FRASS.RequestAccount" %>
<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<telerik:RadAjaxManagerProxy ID="RadAjaxManagerProxy1" runat="server" ></telerik:RadAjaxManagerProxy>
	<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Hay"></telerik:RadAjaxLoadingPanel>
		<div style="width: 100%;">
			<div class="roundedDiv">
				<asp:Panel ID="Panel_Form" runat="server">
					<div style="margin-bottom: 20px;">
						<h2>Request Access to the FRASS Demonstration Site</h2>
						<p>
							Complete this form to request temporary access to the <b>Forest Resource Analysis System Software (FRASS) demonstration environment</b>.
						</p>
						<p>
							Access is granted by <b>Forest Econometrics</b> solely for evaluation and educational use. Each approved user will receive a unique login 
							<br />valid for one month, renewable upon request.
						</p>
						<p>
							Before submitting this form, please review the <b>FRASS Demonstration Access Agreement</b> linked below. By submitting your information, 
							<br />you acknowledge that you have read and agree to its terms, including the confidentiality and non-disclosure provisions that protect 
							<br />proprietary FRASS content.
						</p>
					</div>
					<h3>Required Information</h3>
					<div style="padding-top: 10px;">
						<div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">First Name</div>                     
						<div style="float: left;"><asp:TextBox ID="TextBox_FirstName" runat="server"></asp:TextBox></div>                     
						<div style="clear: both; padding-top: 5px;"></div>
						<div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">Last Name</div>                     
						<div style="float: left;"><asp:TextBox ID="TextBox_LastName" runat="server"></asp:TextBox></div>                     
						<div style="clear: both; padding-top: 5px;"></div>
						<div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">Company</div>                     
						<div style="float: left;"><asp:TextBox ID="TextBox_Company" runat="server" MaxLength="100"></asp:TextBox></div>                     
						<div style="clear: both; padding-top: 5px;"></div>
						<div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">Email Address</div>                     
						<div style="float: left;"><asp:TextBox ID="TextBox_Email" runat="server"></asp:TextBox></div>                     
						<div style="clear: both; padding-top: 5px;"></div>
						<div style="float: left; width: 100px; padding-top: 3px; font-weight: normal;">Phone Number</div>                     
						<div style="float: left;"><asp:TextBox ID="TextBox_PhoneNumber" runat="server" MaxLength="20"></asp:TextBox></div>                     
						<div style="clear: both; padding-top: 5px;"></div>
						<div style="margin-top: 20px; margin-bottom: 10px;">
							By clicking <b>Submit Request</b>, you agree to hold all login credentials in confidence and to refrain from sharing account access with others.
						</div>
						<div style="margin-bottom: 20px;">
							📄 <a href="/Documents/User_Guide/FRASS_Data_Rights_and_Access_Policy-20251025B.pdf" target="_blank" style="text-decoration: underline; font-size: .9rem;">View the FRASS Demonstration Access Agreement (PDF)</a>
						</div> 
						
						<div style="padding-top: 5px;">
							<asp:RadioButton ID="RadioButton_Agree" runat="server" Text="I Agree" GroupName="Agree" />
							<asp:RadioButton ID="RadioButton_DoNotAgree" runat="server" Text="I Do Not Agree" GroupName="Agree" />
						</div>

						<div style="clear: both; padding-top: 10px;"></div>                  
						<div style="float: left;"><asp:Button ID="Button_Submit" runat="server" Text="Request Account" OnClick="Button_Submit_Click"/></div>                     
						<div style="float: left;"><asp:Button ID="Button_Cancel" runat="server" Text="Cancel" OnClick="Button_Cancel_Click" /></div>                     
						<div style="clear: both; padding-top: 5px;"></div>
					</div>
				</asp:Panel>
				<asp:Label ID="Label_Message" runat="server"></asp:Label>
			</div>
		</div>
</asp:Content>
