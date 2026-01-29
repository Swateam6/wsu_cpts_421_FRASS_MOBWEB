<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="R1Chart.aspx.cs" Inherits="FRASS.WebUI.Parcels.Charts.R1Chart" %>
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
		<div style="color: #ffffff; font-weight: bold;">Start at Year: <asp:DropDownList ID="DropDownList_Years" runat="server" OnSelectedIndexChanged="DropDownList_Years_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></div>
		<telerik:RadChart ID="RadChart1" runat="server" Skin="Hay" DefaultType="Line">
		</telerik:RadChart>
	</div>
	</form>
</body>
</html>
