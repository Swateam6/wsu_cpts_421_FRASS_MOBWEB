<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SharePortfolio.aspx.cs" Inherits="FRASS.WebUI.Popups.SharePortfolio" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #loadingOverlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(238, 238, 238, 0.75);
            z-index: 9999;
            align-items: center;
            justify-content: center;
            font-family: Segoe UI, Arial, sans-serif;
        }
        .spinner {
            width: 40px;
            height: 40px;
            border: 4px solid #ccc;
            border-top-color: #4b8f29;
            border-radius: 50%;
            animation: spin 1s linear infinite;
            margin-bottom: 10px;
        }
        @keyframes spin {
            from { transform: rotate(0deg); }
            to { transform: rotate(360deg); }
        }
    </style>
    <telerik:RadCodeBlock ID="RadCodeBlockScripts" runat="server">
    <script type="text/javascript">
        // Suppress Telerik/ASP.NET AJAX cleanup errors - must be set first
        window.onerror = function (msg, url, line, col, error) {
            if (msg && (
                msg.toString().indexOf('dispose') > -1 ||
                msg.toString().indexOf('PageRequestManager') > -1 ||
                msg.toString().indexOf('undefined') > -1 ||
                msg.toString().indexOf('Cannot read properties') > -1
            )) {
                return true; // Suppress these errors
            }
            return false;
        };

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow)
                oWindow = window.radWindow;
            else if (window.frameElement && window.frameElement.radWindow)
                oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function cancelClick() {
            var oWindow = GetRadWindow();
            if (oWindow) {
                oWindow.argument = null;
                oWindow.close();
            }
        }

        function sanitizeRadListBoxState() {
            // RadListBox expects integer scroll positions; sanitize client state before postback
            var listBox = $find('<%= RadListBox_Shared.ClientID %>');
            if (!listBox || typeof listBox.get_clientStateFieldID !== 'function' || !window.JSON || !JSON.parse) {
                return true;
            }

            var stateFieldId = listBox.get_clientStateFieldID();
            if (!stateFieldId) {
                return true;
            }

            var stateField = document.getElementById(stateFieldId);
            if (!stateField || !stateField.value) {
                return true;
            }

            try {
                var state = JSON.parse(stateField.value);
                if (state && state.scrollPosition != null) {
                    state.scrollPosition = Math.round(state.scrollPosition || 0);
                }
                stateField.value = JSON.stringify(state);
            } catch (err) {
                // Ignore malformed state payloads and let the server handle defaults
            }

            return true;
        }

        function attachRadListBoxSanitizer() {
            var form = document.getElementById('<%= form1.ClientID %>');
            if (!form || form._radListBoxSanitizerAttached) {
                return;
            }

            var handler = function () { sanitizeRadListBoxState(); };

            if (form.addEventListener) {
                form.addEventListener('submit', handler, true);
            } else if (form.attachEvent) {
                form.attachEvent('onsubmit', handler);
            } else {
                var originalSubmit = form.onsubmit;
                form.onsubmit = function (evt) {
                    sanitizeRadListBoxState();
                    if (typeof originalSubmit === 'function') {
                        return originalSubmit.call(form, evt);
                    }
                    return true;
                };
            }

            form._radListBoxSanitizerAttached = true;
        }

        if (document.addEventListener) {
            document.addEventListener('DOMContentLoaded', attachRadListBoxSanitizer);
        } else {
            window.attachEvent('onload', attachRadListBoxSanitizer);
        }

        function showLoading() {
            sanitizeRadListBoxState();
            var overlay = document.getElementById('loadingOverlay');
            if (overlay) {
                overlay.style.display = 'flex';
            }
            return true; // allow postback to continue
        }
    </script>
    </telerik:RadCodeBlock>
</head>
<body style="margin-right: 5px; background-color: #EEEEEE;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
        <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server" Skin="Hay" />
        <div>
            <div style="font-size: 12px; font-family: Segoe UI; font-weight: bold;">
                Select who should get a copy of this Portfolio
            </div>
            <div style="padding-top: 5px;">
                <telerik:RadListBox ID="RadListBox_Shared" runat="server" CheckBoxes="true" Width="400px" Height="300px">
                </telerik:RadListBox>
            </div>
            <div style="padding-top: 5px;">
                <asp:Button ID="Button_Save" runat="server" Text="Share" OnClick="Button_Save_Click" UseSubmitBehavior="true" OnClientClick="return showLoading();" />
                <asp:Button ID="Button_Cancel" runat="server" Text="Close" OnClick="Button_Cancel_Click" UseSubmitBehavior="true" />
            </div>
            <div style="padding-top: 5px;">
                <asp:Label ID="Label_Message" runat="server" Font-Size="12px" Font-Names="Segoe UI" ForeColor="Green" Font-Bold="true"></asp:Label>
            </div>
            <div id="loadingOverlay">
                <div style="display: flex; flex-direction: column; align-items: center;">
                    <div class="spinner"></div>
                    <div style="font-size: 14px; color: #4b4b4b;">Sharing portfolio, please wait...</div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>