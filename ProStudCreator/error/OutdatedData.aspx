<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutdatedData.aspx.cs" Inherits="ProStudCreator.error.OutdatedData" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>IP5/6 Projekte - Veraltete Informationen</title>

    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
    <webopt:BundleReference runat="server" Path="~/Content/css"/>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon"/>
</head>
<body>
    <form id="form1" runat="server">
<div class="navbar navbar-inverse navbar-fixed-top non-selectable" style="background-color: #f5f5f5">
    <div class="container">
        <div class="navbar-header" style="width: 100%">
            <div class="navbar-brand" style="width: 100%">
                <img runat="server" src="~/pictures/Logo.png"/>
                <a href="../projectlist" style="margin-left: 5%;">
                    <h2 style="color: #000; display: inline; text-decoration: none; vertical-align: middle;">IP5/IP6 Projekte</h2>
                </a>
            </div>
        </div>
    </div>
</div>
<div class="container body-content" style="margin-top: 50px;">
    <div>
        <h2>Neue Projekt Version verfügbar</h2>
        <div class="alert alert-danger">Die Informationen auf dieser Seite sind nicht mehr aktuell.</div>
    </div>
    <hr/>
    <footer>
        <div class="pull-left">
            &copy;<%: DateTime.Now.Year %> - Fachhochschule Nordwestschweiz
        </div>
    </footer>
</div>
    </form>
</body>
</html>