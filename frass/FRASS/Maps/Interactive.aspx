<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Interactive.aspx.cs" Inherits="FRASS.WebUI.Maps.Interactive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<iframe width="940" height="600" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src="https://www.arcgis.com/home/webmap/templates/OnePane/basicviewer/embed.html?webmap=8ea69d81517f4a43b8f7b83e4b477417&amp;gcsextent=-122.3591,46.8098,-122.2154,46.8604&amp;displayslider=true&amp;displayscalebar=true&amp;displaybasemaps=true"></iframe>
	<br />
	<small><a href="https://www.arcgis.com/home/webmap/viewer.html?webmap=8ea69d81517f4a43b8f7b83e4b477417&amp;extent=-122.3591,46.8098,-122.2154,46.8604" style="color: #0000FF; text-align: left" target="_blank">View Larger Map</a></small>
</asp:Content>
