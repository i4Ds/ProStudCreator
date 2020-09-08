<%@ Page Title="User bearbeiten" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="UserEditPage.aspx.cs" Inherits="ProStudCreator.UserEditPage" %>

<%@ Import Namespace="ProStudCreator" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var hasUnsavedChanges = false;

        /**
        * Asks user to confirm an action, which then saves the project.
        * Supresses the "Unsaved changes" dialog.
        **/
        function confirmSaving(message) {
            var ok = confirm(message);
            if (ok) {
                hasUnsavedChanges = false;
            }
            return ok;
        }

        // Attach event handlers once page is loaded
        $(document).ready(function () {
            $(":input").not(document.getElementsByTagName("select")).change(function () {
                hasUnsavedChanges = true;
            });
            $("#projectTypes :input").not(document.getElementsByTagName("select")).click(function () {
                hasUnsavedChanges = true;
            });

        });

        $(document).ready(function () {
            var count = 0; // 
            var focusCount = 0;

            $('input[type=textbox]').focus(function () {

                focusCount = $(this).val().length;
                count = 0;
                // get total text length
                $('input[type=textbox]').each(function () {
                    count += $(this).val().length;
                });
                // remove text length of focused input from total count
                count -= focusCount;
            });

            $('input[type=textbox]').keyup(function () {
                focusCount = $(this).val().length;
                $('#charNum').text(2600 - (count + focusCount));
            });
        });

        // should resolve the jumping issue during scrolling because of the pdf UpdatePanel
        // https://forums.asp.net/post/1904273.aspx
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_beginRequest(beginRequest);
        function beginRequest()
        {
            prm._scrollPosition = null;
        }

        $(window).on('beforeunload',
            function () {
                if (hasUnsavedChanges) {
                    return "Änderungen wurden noch nicht gespeichert. Seite wirklich verlassen?";
                }
            }
        );
    </script>
    <div class="well newProjectSettings ">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-md-3" Text="Name:"></asp:Label>
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>
            </div>
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-md-3" Text="Mail:"></asp:Label>
                <div class="col-md-6">
                    <asp:TextBox runat="server" ID="UserMail" CssClass="form-control" MaxLength="50"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-md-3" Text="Institut:"></asp:Label>
                <div class="col-md-6">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DepartmentName" ID="dropDepartment" CssClass="form-control"></asp:DropDownList>
                    <asp:Label runat="server" ID="DepartmentLabel" CssClass="form-control" Visible="false"></asp:Label>
                </div>
            </div>
            <hr />
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf Hauptbetreuer sein:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanBeAdvisor1" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Ist Aktiv:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserIsActive" CssClass="form-control" />
                </div>
            </div>
            <hr />
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf Excel exportieren:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanExportExcel" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf Projekte veröffentlichen:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanPublishProjects" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf Admin Seite besuchen:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanVisitAdminPage" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf alle Projekte sehen:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanSeeAllProjectsInProgress" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf alle Projekte bearbeiten:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanEditAllProjects" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf alle Projekte einreichen:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanSubmitAllProjects" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Darf Projekte reservieren:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:CheckBox runat="server" ID="UserCanReserveProjects" CssClass="form-control" />
                </div>
            </div>

        </div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="saveUser" Text="Speichern" OnClick="SaveCloseUserButton" CssClass="btn btn-default"  OnClientClick="this.disabled = 'true'; hasUnsavedChanges = false;" UseSubmitBehavior="false"></asp:Button>
            <asp:Button runat="server" ID="cancelUser" Text="Abbrechen" OnClick="CancelUserButton" CssClass="btn btn-default" CausesValidation="false"></asp:Button>
            <asp:Button runat="server" ID="deleteUser" Text="Löschen" OnClick="DeleteUserButton" CssClass="btn btn-default" OnClientClick="return confirmSaving('Diesen User löschen?');" Visible="false"></asp:Button>
        </div>
    </div>
</asp:Content>
