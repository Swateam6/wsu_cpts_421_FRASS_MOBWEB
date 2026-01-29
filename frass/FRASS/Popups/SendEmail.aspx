<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendEmail.aspx.cs" Inherits="FRASS.WebUI.Popups.SendEmail" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	 <script type="text/javascript">
		function GetRadWindow() {
			var oWindow = null;
			if (window.radWindow)
				oWindow = window.radWindow;
			else if (window.frameElement.radWindow)
				oWindow = window.frameElement.radWindow;
			return oWindow;
		}

		function cancelClick() {
			var oWindow = GetRadWindow();
			oWindow.argument = null;
			oWindow.close();
		}

	</script>
	<link href="/Styles/Site.css" rel="Stylesheet" type="text/css" />
</head>
<body style="margin-right: 5px; background-color: #EEEEEE;">
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
	<telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" Skin="Hay" />
	<div>
		<asp:TextBox ID="TextBox_Subject" runat="server" TextMode="SingleLine" Width="365px" Placeholder="Subject" ></asp:TextBox>
		<div style="padding-top: 5px;">
			<asp:TextBox ID="TextBox_Message" runat="server" TextMode="MultiLine" Width="365px" Height="290px"></asp:TextBox>
		</div>
		<div style="padding-top: 5px;">
			<asp:Button ID="Button_Send" runat="server" Text="Send Message" OnClick="Button_Send_Click" />
			<input type="button" value="Cancel" onclick="cancelClick();" />
		</div>
	</div>
	</form>
</body>
</html>
