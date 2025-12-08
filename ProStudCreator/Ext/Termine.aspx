<%@ Page Title="IP5/IP6 Projekte" Language="C#" AutoEventWireup="True" CodeBehind="Termine.aspx.cs" Inherits="ProStudCreator.Ext.Termine" %>

<!DOCTYPE html>
<html lang="de">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <meta http-equiv="x-ua-compatible" content="IE=edge">
    <title><%: Page.Title %></title>
    <asp:PlaceHolder runat="server">
        <%: Scripts.Render("~/bundles/modernizr") %>
    </asp:PlaceHolder>
    <webopt:BundleReference runat="server" Path="~/Content/css"/>
    <link href="~/favicon.ico" rel="shortcut icon" type="image/x-icon"/>
</head>
<body>
<form runat="server">
    <asp:ScriptManager runat="server">
        <Scripts>
            <%--Weitere Informationen zum Bundling von Skripts in ScriptManager finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=301884". --%>
            <%--Framework-Skripts--%>
            <asp:ScriptReference Name="jquery"/>
            <asp:ScriptReference Name="bootstrap"/>
            <asp:ScriptReference Name="respond"/>
            <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js"/>
            <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js"/>
            <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js"/>
            <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js"/>
            <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js"/>
            <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js"/>
            <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js"/>
            <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js"/>
            <asp:ScriptReference Name="WebFormsBundle"/>
            <%--Websiteskripts--%>
        </Scripts>
    </asp:ScriptManager>

    <div class="navbar navbar-inverse navbar-fixed-top non-selectable" style="background-color: #f5f5f5">
        <div class="container">
            <div class="navbar-header" style="width: 100%">
                <div class="navbar-brand" style="width: 100%">
                    <img runat="server" src="~/pictures/Logo.png"/>
                    <a href="Termine" style="margin-left: 5%;">
                        <h2 style="color: #000; display: inline; text-decoration: none; vertical-align: middle;">Informatik-Projekte 5/6</h2>
                    </a>
                </div>
            </div>
        </div>
    </div>
    <div class="container body-content">
        <div class="well usernSettings">
            <h3>Termine</h3>
            <div class="well" style="background-color: #ffffff">
                <asp:GridView ID="AllEvents" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="True" AlternatingRowStyle-BackColor="#e7e7e7" Width="100%" OnDataBound="AllEvents_DataBinding">
                    <%--<AlternatingRowStyle BackColor="White" />--%>
                </asp:GridView>
                <hr/>
                <p>Titeländerungen von Informatikprojekten sind bis <%: (ProStudCreator.Global.AllowTitleChangesBeforeSubmission.Days/7) %> Wochen vor Abgabe möglich!</p>
                <br/>
                <p>
                    <a href="../Content/Projekteingabe_Firma.pdf" target="_blank" class="pdf">Formular für Projekte von externen Auftraggebern.</a>
                </p>
                <p>
                    Handbuch und Vorlagen unter <%: ConfigurationManager.AppSettings["infoMaterialsUrl"] %>
                </p>
            </div>
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