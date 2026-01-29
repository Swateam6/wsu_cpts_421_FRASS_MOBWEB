<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimberMarketFutureChart.aspx.cs" Inherits="FRASS.WebUI.Parcels.Charts.TimberMarketFutureChart" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<link href="/Styles/Site.css" rel="Stylesheet" type="text/css" />
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
	<telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" Skin="Hay" />
	<div>
		<telerik:RadChart ID="RadChart1" runat="server" Skin="Hay" DefaultType="Line">
		</telerik:RadChart>
	</div>
	</form>
</body>
</html>