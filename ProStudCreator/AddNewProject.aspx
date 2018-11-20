<%@ Page Title="Projekt bearbeiten" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.AddNewProject" %>

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

        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Ein neues Projekt wurde erstellt. Zu neuem Projekt wechseln?")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
            return true;
        }


        function confirmSaving(message) {
            var ok = confirm(message);
            if (ok) {
                hasUnsavedChanges = false;
            }
            return ok;
        }
        function isContentStud(currentObject) {
            var txtBoxValue = currentObject.value;
            var term = "@students.fhnw.ch";
            var index = txtBoxValue.indexOf(term);
            if (currentObject.value != "") {
                if (index == -1) {
                    alert("Geben Sie eine E-Mail Adresse an, welche mit @students.fhnw.ch endet");
                    currentObject.style.borderColor = 'red';
                } else {
                    currentObject.style.borderColor = 'green';
                }
            } else {
                currentObject.style.borderColor = '#ccc';
            }

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

        $(window).on('beforeunload',
            function () {
                if (hasUnsavedChanges) {
                    return "Änderungen wurden noch nicht gespeichert. Seite wirklich verlassen?";
                }
            });
    </script>
    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="refuseProjectUpdatePanel">
        <ContentTemplate>
            <div id="refusedReason" class="well newProjectSettings" runat="server" visible="false">
                <asp:Label runat="server" ID="refusedReasonTitle" Text="Ablehnungsgrund:" Font-Size="24px" Height="50px"></asp:Label>
                <div class="form-group">
                    <asp:Label runat="server" CssClass="control-label" Text="Weshalb wird dieses Projekt abgelehnt? Der Text wird dem Projektersteller via E-Mail zugestellt."></asp:Label>
                    <asp:TextBox runat="server" ID="refusedReasonText" CssClass="form-control contentRefuseDesign" TextMode="MultiLine"></asp:TextBox>
                </div>
                <asp:Button runat="server" ID="RefuseDefinitiveNewProject" CssClass="btn btn-default refuseProject" Width="125px" Text="Ablehnen" OnClientClick="return confirmSaving('Dieses Projekt wirklich ablehnen?');" OnClick="RefuseDefinitiveNewProject_Click"></asp:Button>
                <asp:Button runat="server" ID="cancelRefusion" CssClass="btn btn-default" Text="Abbrechen" OnClick="CancelRefusion_Click" CausesValidation="false"></asp:Button>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="well newProjectSettings ">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <asp:PlaceHolder ID="AdminView" runat="server" Visible="True">
            <asp:Label runat="server" ID="CreatorID" CssClass="pull-right" Font-Size="24px" Height="50px"></asp:Label>
        </asp:PlaceHolder>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projektname:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ProjectName" CssClass="form-control" MaxLength="80"></asp:TextBox>
                    <asp:Label runat="server" ID="ProjectNameLabel" CssClass="form-control" Visible="false" Style="overflow: auto; width: 75%;"></asp:Label>
                    <asp:RequiredFieldValidator ID="ProjectNameValidator" ForeColor="Red" Display="Dynamic" ControlToValidate="ProjectName" runat="server" SetFocusOnError="true" ErrorMessage="Bitte geben Sie einen Projektnamen an."></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Vorgängerprojekt:"></asp:Label>
                <div class="col-sm-9">
                    <asp:DropDownList runat="server" ID="dropPreviousProject" DataValueField="Id" DataTextField="Name" AutoPostBack="true" CausesValidation="false" CssClass="form-control dropPreviousProject" OnSelectedIndexChanged="DropPreviousProject_SelectedIndexChanged" />
                    <asp:Label runat="server" ID="dropPreviousProjectLabel" CssClass="form-control" Style="width: 75%;" Visible="false"></asp:Label>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Hauptbetreuung:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="dropAdvisor1" DataValueField="Id" DataTextField="Name" CssClass="form-control"></asp:DropDownList>
                    <asp:Label runat="server" ID="dropAdvisor1Label" CssClass="form-control" Visible="false"></asp:Label>
                </div>
                <div class="col-sm-3"></div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Nebenbetreuung:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="dropAdvisor2" CssClass="form-control" DataValueField="Id" DataTextField="Name"></asp:DropDownList>
                    <asp:Label runat="server" ID="dropAdvisor2Label" CssClass="form-control" Visible="false"></asp:Label>
                </div>
                <div class="col-sm-3"></div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Institut:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DepartmentName" ID="Department" CssClass="form-control"></asp:DropDownList>
                    <asp:Label runat="server" ID="DepartmentLabel" CssClass="form-control" Visible="false"></asp:Label>
                </div>
            </div>
            <hr />
            <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="updateClient">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="dropPreviousProject" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="radioClientType" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <div class="form-group" style="text-align: left">
                        <asp:Label runat="server" Text="Kundentyp:" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="col-sm-7 radioButtonSettings">
                            <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" TextAlign="Right" BorderStyle="None" ID="radioClientType" AutoPostBack="true" OnSelectedIndexChanged="RadioClientType_SelectedIndexChanged">
                                <asp:ListItem Text="FHNW intern" Selected="True" Value="Intern" />
                                <asp:ListItem Text="Unternehmen" Value="Company" />
                                <asp:ListItem Text="Privatperson" Value="PrivatePerson" />
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="form-group" runat="server" id="divClientForm" visible="false">
                        <div class="form-group" style="text-align: left" runat="server" id="divClientCompany">
                            <asp:Label runat="server" Text="Unternehmen:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-3">
                                <asp:TextBox runat="server" ID="txtClientCompany" CssClass="form-control col-sm-3"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientCompanyLabel" CssClass="form-control col-sm-3" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Anrede:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-3">
                                <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpClientTitle" AutoPostBack="false" CssClass="form-control">
                                    <asp:ListItem Text="-" Value="0" />
                                    <asp:ListItem Text="Herr" Value="1" />
                                    <asp:ListItem Text="Frau" Value="2" />
                                </asp:DropDownList>
                                <asp:Label runat="server" ID="drpClientTitleLabel" CssClass="form-control" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Vor- und Nachname:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6 col-md-7">
                                <asp:TextBox runat="server" ID="txtClientName" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientNameLabel" CssClass="form-control" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="E-Mail Adresse:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6 col-md-7">
                                <asp:TextBox runat="server" ID="txtClientEmail" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientEmailLabel" CssClass="form-control" Visible="false"></asp:Label>
                                <asp:RegularExpressionValidator ID="ClientEmailFormatValidator" ValidationExpression=".+@.+\..+" ForeColor="Red" Display="Dynamic" ControlToValidate="txtClientEmail" runat="server" SetFocusOnError="true" ErrorMessage="Bitte geben Sie eine E-Mail mit Format 'xxx@yyy.zzz' an."></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left" runat="server" id="divClientDepartment">
                            <asp:Label runat="server" Text="Abteilung:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6 col-md-7">
                                <asp:TextBox runat="server" ID="txtClientDepartment" CssClass="form-control maxWidth" Placeholder="Falls vorhanden" MaxLength="50"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientDepartmentLabel" CssClass="form-control" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Strasse und Nummer:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6 col-md-7">
                                <asp:TextBox runat="server" ID="txtClientStreet" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientStreetLabel" CssClass="form-control" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="PLZ und Ort:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-2">
                                <asp:TextBox runat="server" ID="txtClientPLZ" CssClass="form-control maxWidth" TextMode="Number" MaxLength="10"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientPLZLabel" CssClass="form-control" Visible="false"></asp:Label>
                            </div>
                            <div class="col-sm-4">
                                <asp:TextBox runat="server" ID="txtClientCity" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientCityLabel" CssClass="form-control maxWidth" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Referenz des Kunden:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientReference" CssClass="form-control maxWidth" Placeholder="Falls vorhanden." ToolTip="z.B. Bestellnummer des Auftraggebers." MaxLength="50"></asp:TextBox>
                                <asp:Label runat="server" ID="txtClientReferenceLabel" CssClass="form-control" Visible="false"></asp:Label>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>


            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Geheimhaltung:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-6">
                    <asp:CheckBox runat="server" ID="chkNDA" CssClass="form-control" ToolTip="NDA-Projekte werden nicht in öffentlichen Broschüren aufgeführt" Text=" Projekt/Auftraggeber unter NDA" />
                </div>
            </div>

            <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="updateReservation">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="POneTeamSize" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="PTwoTeamSize" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="dropPreviousProject" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <hr />
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Reserviert für (Admin only):"></asp:Label>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" ID="Reservation1Name" CssClass="col-sm-9 form-control" placeholder="(Vorname Nachname)"></asp:TextBox>
                            <asp:Label runat="server" ID="Reservation1NameLabel" CssClass="col-sm-9 form-control" Visible="false"></asp:Label>
                        </div>
                        <div class="col-sm-6">
                            <asp:TextBox runat="server" onchange="isContentStud(this)" ID="Reservation1Mail" CssClass="col-sm-9 form-control" placeholder="(E-Mail)" TextMode="Email"></asp:TextBox>
                            <asp:Label runat="server" ID="Reservation1MailLabel" CssClass="col-sm-9 form-control" Visible="false"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" ID="Reservation2Name" CssClass="col-sm-9 form-control contentReservation" placeholder="(Vorname Nachname)"></asp:TextBox>
                            <asp:Label runat="server" ID="Reservation2NameLabel" CssClass="col-sm-9 form-control" Visible="false"></asp:Label>
                        </div>
                        <div class="col-sm-6">
                            <asp:TextBox runat="server" onchange="isContentStud(this)" ID="Reservation2Mail" CssClass="col-sm-9 form-control" placeholder="(E-Mail)" TextMode="Email"></asp:TextBox>
                            <asp:Label runat="server" ID="Reservation2MailLabel" CssClass="col-sm-9 form-control" Visible="false"></asp:Label>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Sprache:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="Language" DataValueField="Id" DataTextField="Description" CssClass="form-control">
                        <asp:ListItem Text="Deutsch oder Englisch" />
                        <asp:ListItem Text="Nur Deutsch" />
                        <asp:ListItem Text="Nur Englisch" />
                    </asp:DropDownList>
                </div>
            </div>
            <%-- <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Dauer:"></asp:Label>
                <div class="col-sm-6">
                    <asp:CheckBox ID="DurationOneSemester" CssClass="checkbox" Text="Projekt muss in 1 Semester durchgeführt werden." Checked="true" runat="server" />
                </div>
            </div>--%>
            <asp:UpdatePanel ID="updatePriority" runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="dropPreviousProject" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 1:"></asp:Label>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" ID="POneType" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" />
                            <asp:Label runat="server" ID="POneTypeLabel" CssClass="form-control ellipsisLabel" Width="250px" Visible="false"></asp:Label>
                        </div>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" ID="POneTeamSize" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" OnSelectedIndexChanged="TeamSize_SelectedIndexChanged" AutoPostBack="true" />
                            <asp:Label runat="server" ID="POneTeamSizeLabel" CssClass="form-control ellipsisLabel" Width="250px" Visible="false"></asp:Label>
                        </div>
                        <div class="col-sm-3"></div>
                    </div>
                    <div class="form-group" runat="server" id="divPriorityTwo">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 2:"></asp:Label>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" ID="PTwoType" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" />
                            <asp:Label runat="server" ID="PTwoTypeLabel" CssClass="form-control ellipsisLabel" Width="250px" Visible="false"></asp:Label>
                        </div>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" ID="PTwoTeamSize" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" OnSelectedIndexChanged="TeamSize_SelectedIndexChanged" AutoPostBack="true" />
                            <asp:Label runat="server" ID="PTwoTeamSizeLabel" CssClass="form-control ellipsisLabel" Width="250px" Visible="false"></asp:Label>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <hr />
            <asp:UpdatePanel runat="server" class="form-group" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Themengebiet:"></asp:Label>
                    <div id="projectTypes" class="col-sm-9">
                        <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ToolTip="Design, Usability, User Interfaces, ..." ImageUrl="pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ToolTip="Hardwarenah, IoT, Embedded, Low-level, ..." ImageUrl="pictures/projectTypHWUnchecked.png" OnClick="HW_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ToolTip="Computergrafik, 3D, Bildverarbeitung, ..." ImageUrl="pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="MlAlg" Height="60px" runat="server" ToolTip="Mathematik, Algorithmen, Machine Learning, Data Mining, ..." ImageUrl="pictures/projectTypMlAlgUnchecked.png" OnClick="MlAlg_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ToolTip="Mobile Apps, Webentwicklung, ..." ImageUrl="pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ToolTip="Datenbanken, Big Data, Data Spaces, ..." ImageUrl="pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click" CausesValidation="false" />
                        <% if (ShibUser.IsAdmin() || ShibUser.GetDepartment().IMVS)
                            { %>
                        <asp:ImageButton CssClass="img-rounded" ID="SysSec" Height="60px" runat="server" ToolTip="ITSM, Networks, Security, ..." ImageUrl="pictures/projectTypSysSecUnchecked.png" OnClick="SysSec_Click" CausesValidation="false" />
                        <% } %>
                        <% if (ShibUser.IsAdmin() || ShibUser.GetDepartment().IMVS || ShibUser.GetDepartment().IIT)
                            { %>
                        <asp:ImageButton CssClass="img-rounded" ID="SE" Height="60px" runat="server" ToolTip="Software Engineering, Testing, Tooling, Architectures, Requirements Engineering, ..." ImageUrl="pictures/projectTypSEUnchecked.png" OnClick="SE_Click" CausesValidation="false" />
                        <% } %>
                    </div>
                    <asp:Timer runat="server" Interval="60000" Enabled="true" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="form-group">
                <asp:Label runat="server" ID="AddPictureLabel" CssClass="control-label col-sm-3" Text="Bild hinzufügen:"></asp:Label>
                <div class="col-sm-3">
                    <asp:FileUpload runat="server" ID="AddPicture" accept=".jpeg,.jpg,.png" CssClass="control-label" /><small>(max. 1MB)</small>
                    <br />
                    <a style="color: red">
                        <asp:RegularExpressionValidator ID="regexValidator" runat="server"
                            ControlToValidate="AddPicture"
                            ErrorMessage="Es werden nur JPEGs und PNGs als Bildformat unterstützt."
                            ValidationExpression="(.*\.([Jj][Pp][Gg])|.*\.([Jj][Pp][Ee][Gg])|.*\.([Pp][Nn][Gg])$)">
                        </asp:RegularExpressionValidator>
                    </a>
                </div>
                <div class="col-sm-1">
                    <asp:LinkButton runat="server" ID="DeleteImageButton" OnClick="DeleteImage_Click" OnClientClick="hasUnsavedChanges = false;return confirm('Dieses Bild wirklich entfernen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="ImageLabel" CssClass="control-label col-sm-3" Text=""></asp:Label>
                <div class="col-sm-9">
                    <asp:Image runat="server" ID="ProjectPicture" CssClass="maxImageWidth img-rounded" Visible="true" EnableViewState="False" />
                    <asp:Image runat="server" ID="ProjectPicturePrevious" CssClass="maxImageWidth img-rounded" Visible="false" EnableViewState="False" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass=" col-sm-3" Text=""></asp:Label>
                <div class="col-md-9">
                    <asp:TextBox runat="server" ID="imgdescription" CssClass="form-control" placeholder="Beschreibung des Bildes" TextMode="Search" MaxLength="255"></asp:TextBox>
                    <asp:TextBox runat="server" ID="imgdescriptionLabel" CssClass="form-control" Visible="false"></asp:TextBox>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ausgangslage:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="InitialPositionContent" CssClass="form-control" TextMode="MultiLine"></asp:TextBox>
                    <asp:Label runat="server" ID="InitialPositionContentLabel" CssClass="form-control" Style="overflow: auto; height: 300px;" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ziel der Arbeit:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ObjectivContent" CssClass="form-control" placeholder="Ziel der Arbeit" TextMode="MultiLine"></asp:TextBox>
                    <asp:Label runat="server" ID="ObjectivContentLabel" CssClass="form-control" Style="overflow: auto; height: 300px;" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Problemstellung:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ProblemStatementContent" CssClass="form-control" placeholder="Problemstellung" TextMode="MultiLine"></asp:TextBox>
                    <asp:Label runat="server" ID="ProblemStatementContentLabel" CssClass="form-control" Style="overflow: auto; height: 300px;" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologien:<br/>Schwerpunkte:<br/>Referenzen:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ReferencesContent" CssClass="form-control" placeholder="Technologien/Schwerpunkte/Referenzen" TextMode="MultiLine"></asp:TextBox>
                    <asp:Label runat="server" ID="ReferencesContentLabel" class="form-control" Style="overflow: auto; height: 300px;" Visible="false"></asp:Label>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bemerkungen (optional):"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="RemarksContent" CssClass="form-control" placeholder="Bemerkungen" TextMode="MultiLine"></asp:TextBox>
                    <asp:Label runat="server" ID="RemarksContentLabel" CssClass="form-control" Style="overflow: auto; height: 300px;" Visible="false"></asp:Label>
                </div>
            </div>

            <div class="form-group">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="true" />
            </div>
        </div>
        <div style="text-align: right;">
            <div id="fixedupdatepanel">
                <asp:UpdatePanel ID="PdfupdatePanel" runat="server">
                    <ContentTemplate>
                        <asp:Label ID="Pdfupdatelabel" runat="server" Text=""></asp:Label>
                        <asp:Timer ID="Pdfupdatetimer" runat="server" Interval="3000" OnTick="Pdfupdatetimer_Tick" Enabled="true">
                        </asp:Timer>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
           
                    <asp:Button runat="server" ID="publishProject" Visible="false" CssClass="btn btn-default publishProject" Width="113px" Text="Veröffentlichen" OnClick="PublishProject_Click" OnClientClick="return confirmSaving('Projekt wirklich veröffentlichen?');"></asp:Button>
                    <asp:Button runat="server" ID="refuseProject" Visible="false" Style="margin-right: 0px;" CssClass="btn btn-default refuseProject" Width="113px" Text="Ablehnen" OnClick="RefuseProject_Click"></asp:Button>
                    <asp:Button runat="server" ID="rollbackProject" Visible="false" Style="margin-right: 0px;" CssClass="btn btn-default rollbackMarginRight redButton" Text="Zurückziehen" OnClick="RollbackProject_Click" OnClientClick="return confirmSaving('Projekt wirklich zurückziehen?');"></asp:Button>
                    <asp:Button runat="server" ID="submitProject" Visible="false" Style="margin-right: 0px;" CssClass="btn btn-default greenButton" Text="Einreichen" OnClick="SubmitProject_Click" OnClientClick="return confirmSaving('Dieses Projekt einreichen?');"></asp:Button>
                    <asp:Button runat="server" AutoPostBack="true" ID="duplicateProject" Style="margin-right: 0px;" CssClass="btn btn-default" Text="Duplizieren" OnClick="DuplicatProject_Click" OnClientClick="return Confirm();" />
                    <asp:Button runat="server" ID="saveCloseProject" OnClick="SaveCloseProjectButton" CssClass="btn btn-default" Text="Speichern & Schliessen" OnClientClick="hasUnsavedChanges = false;"></asp:Button>
                    <asp:Button runat="server" ID="saveProject" OnClick="SaveProjectButton" CssClass="btn btn-default" Text="Zwischenspeichern" OnClientClick="hasUnsavedChanges = false;"></asp:Button>
                    <asp:Button runat="server" ID="cancelProject" CssClass="btn btn-default" TabIndex="5" Text="Abbrechen" OnClick="CancelNewProject_Click" CausesValidation="false"></asp:Button>
        </div>
    </div>
    <div runat="server" class="well newProjectSettings" id="divHistory">
        <asp:UpdatePanel runat="server" ID="UpdateHistory">
            <ContentTemplate>
                <asp:Label runat="server" Font-Size="24px" Height="50px" Text="History" CssClass="col-sm-5"></asp:Label>
                <div style="text-align: right;">
                    <asp:Button runat="server" ID="btnHistoryCollapse" CssClass="btn btn-default btnHeight" Text="▼" OnClick="BtnHistoryCollapse_OnClick" />
                </div>
                <br />
                <div runat="server" id="DivHistoryCollapsable">
                    <asp:ListView runat="server" ItemType="ProStudCreator.Project" ID="historyListView" OnItemCommand="ProjectRowClick">
                        <ItemTemplate>
                            <table>
                                <div class="row" <%#"style='background-color:"+Item.StateColor+";'" %>>
                                    <div class="row" id="historyRow">
                                        <dîv class="col-xs-12 col-md-1"></dîv>
                                        <div class="col-xs-12 col-md-3">
                                            <asp:Label runat="server"><%#"<img style='width:35px;' src='http://www.gravatar.com/avatar.php?gravatar_id="+ShibUser.GetGravatar(Item.LastEditedBy)+"'/> " + Item.LastEditedBy %></asp:Label>
                                        </div>
                                        <div class="col-xs-12 col-md-2" style="height: 100%;">
                                            <asp:Label runat="server"><%#DataBinder.Eval(Container.DataItem,"ModificationDate") %></asp:Label>
                                        </div>
                                        <div class="col-xs-12 col-md-2">
                                            <asp:Label runat="server"><%#Item.StateAsString %></asp:Label>
                                        </div>
                                        <div class="col-xs-12 col-md-2" style="width: 11.666%">
                                            <asp:LinkButton runat="server" ID="showChanges" title="Änderungen zeigen" class="btn btn-primary btnHeight" CommandArgument='<%# Item.Id %>' CommandName="showChanges">Vergleichen</asp:LinkButton>
                                        </div>
                                        <div class="col-xs-12 col-md-2">
                                            <asp:LinkButton runat="server" ID="LinkButton1" title="Projekt zurücksetzen" class="btn btn-danger btnHeight" OnClientClick="return confirmSaving('Dieses Projekt zurücksetzen?');" CommandArgument='<%# Item.Id %>' CommandName="revertProject">Wiederherstellen</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <dîv class="col-xs-12 col-md-5"></dîv>
                                        <%# Item.Ablehnungsgrund != null && Item.StateAsString == "Abgelehnt" ? 
                                    "<div style='margin-top:1em;'><div class='col-sm-1'></div><div class='col-sm-2'><b>Ablehnungsgrund</b></div><div class='col-sm-7'></div><div class='col-sm-2'></div></div>" : ""%>
                                        </div>
                                    <div class="row">
                                        <%# Item.Ablehnungsgrund != null  && Item.StateAsString == "Abgelehnt" ?
                                    "<div><div class='col-sm-1'></div><div class='col-sm-11'style='margin-bottom:1em;'>" + Item.Ablehnungsgrund.Replace(Environment.NewLine,"<br />") + "</div></div><hr/>" : ""%>
                                    </div>
                                </div>
                            </table>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
