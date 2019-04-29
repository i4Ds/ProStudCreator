<%@ Page Title="Admin Bereich" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="WebAdminPage.aspx.cs" Inherits="ProStudCreator.WebAdminPage" %>

<asp:Content ID="WebAdminPageContent" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    function confirmSaving(message) {
        return confirm(message);
    }
</script>
    <div class="well usernSettings">
        <h3>Next Task Check</h3>
        <ul ID="TasksNextTaskCheck" runat="server"></ul>
        <h5>Last 10 Task Checks:</h5>
        <ul id="LastTaskRuns" runat="server"></ul>
        <asp:Button ID="ForceTaskCheckNow" runat="server" OnClick="ForceTaskCheckNow_Click" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
        <h3>Tasks: Noten an Ausbildungsadministration übergeben</h3>
        <ul ID="TasksMarks" runat="server"></ul>
        <h3>Tasks: Experten auszahlen</h3>
        <ul ID="TasksExperts" runat="server"></ul>
        <h3>Tasks: Thesis Titel an Ausbildungsadministration senden</h3>
        <ul ID="TasksTitles" runat="server"></ul>
        <h3>Tasks: Projekte abschliessen</h3>
        <h4>Last Semester</h4>
        <asp:Label runat="server" ID="LabelLastSem"></asp:Label>
        <h5>IP5 Normal</h5>
        <ul ID="LastSemIP5Normal" runat="server"></ul>
        <h5>IP5 Lang</h5>
        <ul ID="LastSemIP5Long" runat="server"></ul>
        <h5>IP6</h5>
        <ul ID="LastSemIP6" runat="server"></ul>
        <h4>Current Semester</h4>
        <asp:Label runat="server" ID="LabelCurrSem"></asp:Label>
        <h5>IP5 Normal</h5>
        <ul ID="CurrSemIP5Normal" runat="server"></ul>
        <h5>IP5 Lang</h5>
        <ul ID="CurrSemIP5Long" runat="server"></ul>
        <h5>IP6</h5>
        <ul ID="CurrSemIP6" runat="server"></ul>
        <h5>HttpContext</h5>
        <ul id="HttpContextContent" runat="server"></ul>
    </div>
</asp:Content>
