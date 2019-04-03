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
        <asp:Button ID="ForceTaskCheckNow" runat="server" OnClick="ForceTaskCheckNow_Click" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
        <h3>Tasks: Noten an Ausbildungsadministration übergeben</h3>
        <ul ID="TasksMarks" runat="server"></ul>
        <h3>Tasks: Experten auszahlen</h3>
        <ul ID="TasksExperts" runat="server"></ul>
        <h3>Tasks: Thesis Titel an Ausbildungsadministration senden</h3>
        <ul ID="TasksTitles" runat="server"></ul>
        <h3>Tasks: Projekte abschliessen</h3>
        <ul ID="TasksProjects" runat="server"></ul>
    </div>
</asp:Content>
