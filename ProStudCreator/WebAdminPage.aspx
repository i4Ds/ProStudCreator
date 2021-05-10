<%@ Page Title="Admin Bereich" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="WebAdminPage.aspx.cs" Inherits="ProStudCreator.WebAdminPage" %>
<%@ Register TagPrefix="UserControl" TagName="UserList" Src="~/UserControls/UserListControl.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectTopicControl" Src="~/UserControls/ProjectTopicControl.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectTopicImageControl" Src="~/UserControls/ProjectTopicImageControl.ascx" %>

<asp:Content ID="WebAdminPageContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function confirmSaving(message) {
            return confirm(message);
        }
    </script>
    
    <div class="well usernSettings" style="overflow:hidden;" >
    </div>
    <div class="well usernSettings">
        <h3>Next Task Check</h3>
        <ul ID="TasksNextTaskCheck" runat="server"></ul>
        <h5>Last 10 Task Checks:</h5>
        <ul id="LastTaskRuns" runat="server"></ul>
        <asp:Button ID="ForceTaskCheckNow" runat="server" OnClick="ForceTaskCheckNow_Click" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />
        <h3>Upcoming presentations</h3>
        <ul id="UpcomingPres" runat="server"></ul>
        <h3>Tasks: Noten an Ausbildungsadministration übergeben</h3>
        <ul ID="TasksMarks" runat="server"></ul>
        <h3>Tasks: Experten auszahlen</h3>
        <ul ID="TasksExperts" runat="server"></ul>
        <h4>Theses with no Expert</h4>
        <ul id="NoExpertTheses" runat="server"></ul>
        <h4>Tasks: NEW Experts to be paid</h4>
        <ul id="ExpertsToBePaid" runat="server"></ul>
        <h4>Tasks: NEW Experts not to be paid</h4>
        <ul id="ExpertsNotToBePaid" runat="server"></ul>
        <h4>Tasks: NEW ExpertPaid Mail (ALL EXPERTS)</h4>
        <div id="ExpertMailMessage" runat="server"></div>
        <asp:Button ID="SendExpertMailToWebAdmin" runat="server" OnClick="SendExpertMailToWebAdmin_Click" />
        <h3>Tasks: Thesis Titel an Ausbildungsadministration senden</h3>
        <ul ID="TasksTitles" runat="server"></ul>
        <h4>Thesis Titel Erinnerung 2 Wochen</h4>
        <ul ID="TitleReminder2Weeks" runat="server"></ul>
        <h4>Thesis Titel Erinnerung 2 Tage</h4>
        <ul ID="TitleReminder2Days" runat="server"></ul>
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
        <!--
        <h5>P5 Noten nach Hauptbetreuer</h5>
        <ul ID="P5Grades" runat="server"></ul>
        <h5>P6 Noten nach Hauptbetreuer</h5>
        <ul ID="P6Grades" runat="server"></ul>
        -->
    </div>
</asp:Content>
