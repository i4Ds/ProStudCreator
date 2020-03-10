<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projectlist.aspx.cs" Inherits="ProStudCreator.Projectlist" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectList" Src="~/UserControls/ProjectListControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function confirmSaving(message) {
            return confirm(message);
        }
    </script>
    <div class="well usernSettings">
        <div class="col-sm-3">
            <asp:DropDownList runat="server" DataValueField="Id" DataTextField="Name" ID="dropSemester" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="col-sm-1"></div>
        <div class="radioButtonSettings marginProject">
            <asp:RadioButtonList ID="whichOwner" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" CssClass="col-sm-5" TextAlign="Right">
                <asp:ListItem Value="AllProjects">Veröffentlichte Projekte</asp:ListItem>
                <asp:ListItem Value="OwnProjects" Selected="True">Nur eigene Projekte</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="col-sm-3 input-group">
            <asp:TextBox placeholder="Student..." runat="server" class="form-control" ClientIDMode="Static" id="filterText" name="filterText"></asp:TextBox>
            <span class="input-group-btn">
            <asp:Button runat="server" OnClick="FilterButton_Click" class="btn" id="filterBtn" Text="Suchen"/>
            </span>
        </div>
        <br/>
        <br/>
        <hr/>
        <div runat="server" id="DivProjectStatistics">
            <div class="col-md-3">
                <asp:Label runat="server" ID="LabelNumSubmittedProjects"></asp:Label>
            </div>
            <div class="col-md-2">
                <asp:Label runat="server" ID="LabelNumSubmittedProjectsIP5"></asp:Label>
            </div>
            <div class="col-md-2">
                <asp:Label runat="server" ID="LabelNumSubmittedProjectsIP6"></asp:Label>
            </div>
            <div class="col-md-1">
                <asp:Label runat="server" ID="LabelNumSubmittedProjectsI4DS"></asp:Label>
            </div>
            <div class="col-md-1">
                <asp:Label runat="server" ID="LabelNumSubmittedProjectsIMVS"></asp:Label>
            </div>
            <div class="col-md-1">
                <asp:Label runat="server" ID="LabelNumSubmittedProjectsIIT"></asp:Label>
            </div>
            <br />
            <br />
            <div class="col-sm-3">
                <asp:Label runat="server" ID="LabelNumPublishedProjects"></asp:Label>
            </div>
            <div class="col-sm-2">
                <asp:Label runat="server" ID="LabelNumPublishedProjectsIP5"></asp:Label>
            </div>
            <div class="col-sm-2">
                <asp:Label runat="server" ID="LabelNumPublishedProjectsIP6"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumPublishedProjectsI4DS"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumPublishedProjectsIMVS"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumPublishedProjectsIIT"></asp:Label>
            </div>
            <br />
            <br />
            <div class="col-sm-3">
                <asp:Label runat="server" ID="LabelNumRunningProjects"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumRunningProjectsIP5N"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumRunningProjectsIP5L"></asp:Label>
            </div>
            <div class="col-sm-2">
                <asp:Label runat="server" ID="LabelNumRunningProjectsIP6"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumRunningProjectsI4DS"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumRunningProjectsIMVS"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumRunningProjectsIIT"></asp:Label>
            </div>
            <br/>
            <br/>
            <div class="col-sm-3">
                <asp:Label runat="server" ID="LabelNumFinishedProjects"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumFinishedProjectsIP5N"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumFinishedProjectsIP5L"></asp:Label>
            </div>
            <div class="col-sm-2">
                <asp:Label runat="server" ID="LabelNumFinishedProjectsIP6"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumFinishedProjectsI4DS"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumFinishedProjectsIMVS"></asp:Label>
            </div>
            <div class="col-sm-1">
                <asp:Label runat="server" ID="LabelNumFinishedProjectsIIT"></asp:Label>
            </div>
            <br/>
            <hr/>
        </div>
        <div class="col-sm-10">
            <asp:Button runat="server" ID="NewProject" CssClass="btn btn-default buttonFont" Text="Neues Projekt" OnClick="NewProject_Click"/>
            <asp:Button runat="server" ID="AllProjectsAsPDF" CssClass="btn btn-default buttonFont pdf" Text="Projekte als PDF" OnClick="AllProjectsAsPDF_Click"/>
            <asp:Button runat="server" ID="AllProjectsAsExcel" CssClass="btn btn-default buttonFont" Text="Projektliste in Excel" OnClick="AllProjectsAsExcel_Click"/>
        </div>
        <br />
        <br />
        <br />
        <div class="col-sm-10" style="font-size: 70%">
            <asp:Button id="colExInProgress" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#CEECF5"/>
            = In Bearbeitung&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button id="colExSubmitted" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#ffcc99"/>
            = Eingereicht&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button id="colExRejected" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#F5A9A9"/>
            = Abgelehnt&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button id="colExPublished" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#A9F5A9"/>
            = Veröffentlicht&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button id="colExOngoing" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#64ed64"/>
            = In Durchführung&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button id="colExFinished" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#1adb1a"/>
            = Abgeschlossen&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button id="colExCanceled" runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#e8463b"/>
            = Abgebrochen&nbsp;&nbsp;&nbsp;&nbsp;
        </div>
        <br />
        <br />
        <hr/>
        <div class="well" style="background-color: #ffffff; margin-top: 10px; margin-bottom: 0px;">
            <UserControl:ProjectList ID="ProjectGrid" runat="server" ShowModificationDate="false" />
        </div>
    </div>
</asp:Content>