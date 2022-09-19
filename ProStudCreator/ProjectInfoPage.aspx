<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" EnableEventValidation="false" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectTopicImageControl" Src="~/UserControls/ProjectTopicImageControl.ascx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="ProjectInofpageContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function isContentStud(currentObject) {
            var txtBoxValue = currentObject.value;
            var term = "@students.fhnw.ch";
            var term2 = "@fhnw.ch";
            var index = txtBoxValue.indexOf(term);
            var index2 = txtBoxValue.indexOf(term2);
            if (currentObject.value != "") {
                if (index == -1 && index2 == -1) {
                    alert("Geben Sie eine E-Mail Adresse an, welche mit @students.fhnw.ch oder @fhnw.ch endet");
                    currentObject.style.borderColor = 'red';
                } else {
                    currentObject.style.borderColor = 'green';
                }
            } else {
                currentObject.style.borderColor = '#ccc';
            }
        }

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

    </script>
    <div class="well newProjectSettings">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px" Text="Projektinformationen"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">

            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Projektname:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="ProjectTitle" CssClass="col-md-6 maxWidth alignbottom" Font-Bold="true"></asp:Label>
                <div runat="server" id="DivProjectTitleAdmin" class="col-md-6">
                    <asp:TextBox runat="server" ID="ProjectTitleAdmin" CssClass="form-control maxWidth" MaxLength="120"></asp:TextBox>
                </div>
                <div class="col-md-3">
                    <UserControl:ProjectTopicImageControl runat="server" ID="ProjectTopicImage1" Margin="4"/>
                    <UserControl:ProjectTopicImageControl runat="server" ID="ProjectTopicImage2" Margin="4"/>
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" ID="ChangeTitleDate" CssClass="col-md-6 col-md-offset-3"></asp:Label>
            </div>

            <div class="form-group">
                <asp:Label runat="server" Text="Projektstatus:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelState" CssClass="col-md-6 alignbottom"></asp:Label>
            </div>
            <div class="form-group">
                <asp:Label runat="server" Text="Projektnummer:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelProjectNr" CssClass="col-md-6 alignbottom"></asp:Label>
            </div>
            <hr />

            <div class="form-group">
                <asp:Label runat="server" Text="Betreuung:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="Advisor1Name" CssClass="col-md-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Advisor2Name" CssClass="col-md-3 alignbottom"></asp:Label>
            </div>


            <div runat="server" id="DivStudents" class="form-group">
                <asp:Label runat="server" Text="Studierende:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="Student1Name" CssClass="col-md-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Student2Name" CssClass="col-md-3 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivStudentsAdmin">
                <div class="form-group">
                    <asp:Label runat="server" Text="Studierende:" CssClass="control-label col-md-3"></asp:Label>
                    <div class="col-md-1 no-padding-right">
                        <asp:TextBox runat="server" ID="Student1EventoAdmin" CssClass="form-control" placeholder="(Evento Id)"></asp:TextBox>
                    </div>
                    <div class="col-md-1 no-padding-right">
                        <asp:TextBox runat="server" ID="Student1FirstNameAdmin" CssClass="form-control" placeholder="(Vorname)"></asp:TextBox>
                    </div>
                    <div class="col-md-1 no-padding-right">
                        <asp:TextBox runat="server" ID="Student1LastNameAdmin" CssClass="form-control" placeholder="(Nachname)"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="Student1MailAdmin" CssClass="form-control"  placeholder="(E-Mail)" TextMode="Email" onchange="isContentStud(this)"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-1 col-md-offset-3 no-padding-right">
                        <asp:TextBox runat="server" ID="Student2EventoAdmin" CssClass="form-control" placeholder="(Evento Id)"></asp:TextBox>
                    </div>
                    <div class="col-md-1 no-padding-right">
                        <asp:TextBox runat="server" ID="Student2FirstNameAdmin" CssClass="form-control" placeholder="(Vorname)"></asp:TextBox>
                    </div>
                    <div class="col-md-1 no-padding-right">
                        <asp:TextBox runat="server" ID="Student2LastNameAdmin" CssClass="form-control" placeholder="(Nachname)"></asp:TextBox>
                    </div>
                    <div class="col-md-3">
                        <asp:TextBox runat="server" ID="Student2MailAdmin" CssClass="form-control" placeholder="(E-Mail)" TextMode="Email" onchange="isContentStud(this)"></asp:TextBox>
                    </div>
                </div>
            </div>

                
            <div runat="server" id="DivType" class="form-group">
                <asp:Label runat="server" Text="Art des Projektes:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelProjectType" CssClass="col-md-3 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivTypeAdmin" class="form-group">
                <asp:Label runat="server" Text="Art des Projektes:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-3 no-padding-right">
                    <asp:DropDownList runat="server" ID="DropType" DataValueField="Id" DataTextField="Description" CssClass="form-control"/>
                </div>
                <div class="col-md-3">
                    <asp:DropDownList runat="server" ID="DropDuration" DataValueField="Id" DataTextField="Description" CssClass="form-control">
                        <asp:ListItem Text="(Bitte Auswählen)" Value="dropDurationImpossibleValue" />
                        <asp:ListItem Text="Normal" Value="1"/>
                        <asp:ListItem Text="Lang" Value="2"/>
                    </asp:DropDownList>
                </div>
            </div>

            <div runat="server" id="DivStudyCourse" class="form-group">
                <asp:Label runat="server" Text="Studiengang:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelStudyCourse" CssClass="col-md-3 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivStudyCourseAdmin" class="form-group">
                <asp:Label runat="server" Text="Studiengang:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:DropDownList runat="server" ID="DropStudyCourse" DataValueField="Id" DataTextField="Description" CssClass="form-control">
                        <asp:ListItem Text="(Bitte Auswählen)" Value="dropStudyCourseImpossibleValue" />
                        <asp:ListItem Text="Informatik" Value="1"/>
                        <asp:ListItem Text="Data Science" Value="2"/>
                    </asp:DropDownList>
                </div>
            </div>
            <hr />

            <div runat="server" id="DivDelivery" class="form-group">
                <asp:Label runat="server" Text="Abgabe:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelProjectDelivery" CssClass="col-md-6 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivPresentation" class="form-group">
                <asp:Label runat="server" Text="Schlusspräsentation:" CssClass="control-label col-md-3" ID="LabelProjectEndPresentation"></asp:Label>
                <asp:Label runat="server" ID="LabelPresentation" CssClass="col-md-6 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivPresentationAdmin">
                <div class="form-group">
                    <asp:Label runat="server" Text="Schlusspräsentation:" CssClass="control-label col-md-3" ID="LabelProjectEndPresentationAdmin"></asp:Label>
                    <div class="col-md-6">
                        <asp:TextBox runat="server" ID="TextBoxLabelPresentationDate" CssClass="form-control" placeholder="Datum (dd.MM.yyyy)"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="PresentationDateFormatValidator" ValidationExpression="([0-2][0-9]|(3)[0-1])(\.)(((0)[0-9])|((1)[0-2]))(\.)\d{4}" ForeColor="Red" Display="Dynamic" ControlToValidate="TextBoxLabelPresentationDate" runat="server" SetFocusOnError="true" ErrorMessage="Bitte geben Sie das Datum im Format 'dd.MM.yyyy' an."></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-6 col-md-offset-3">
                        <asp:TextBox runat="server" ID="TextBoxLabelPresentationTime" CssClass="form-control" placeholder="Zeit (HH:mm)"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ValidationExpression="([[0][0-9]|[1][0-9]|[2][0-3]):[0-5][0-9]" ForeColor="Red" Display="Dynamic" ControlToValidate="TextBoxLabelPresentationTime" runat="server" SetFocusOnError="true" ErrorMessage="Bitte geben Sie die Zeit im Format 'HH:mm' an."></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-6 col-md-offset-3">
                        <asp:TextBox runat="server" ID="TextBoxLabelPresentationRoom" CssClass="form-control" placeholder="Raum" MaxLength="500"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div runat="server" id="DivBachelor" class="form-group">
                <asp:Label runat="server" Text="Ausstellung Bachelorthesis:" ID="lblAussstellungBachelorthese" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="ProjectExhibition" CssClass="col-md-6 alignbottom"></asp:Label>
                <br />
                <br />
            </div>

            <div runat="server" id="DivExpert" class="form-group">
                <asp:Label runat="server" Text="Experte:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelExpertName" CssClass="col-md-6 alignbottom"></asp:Label>
                <br />
                <br />
            </div>
            <div runat="server" id="DivExpertAdmin" class="form-group">
                <asp:Label runat="server" Text="Experte:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:DropDownList runat="server" ID="DropExpert" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="DrpExpert_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <asp:UpdatePanel ID="UpdateExpertMail" UpdateMode="Conditional" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="DropExpert" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Label runat="server" ID="LabelExpertMail" CssClass="col-md-3 alignbottom"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <hr />

            <div runat="server" id="DivLanguage" class="form-group">
                <asp:Label runat="server" Text="Durchführungssprache:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelLanguage" CssClass="col-md-6 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivLanguageAdmin" class="form-group">
                <asp:Label runat="server" Text="Durchführungssprache:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-6">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="DropLanguage" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="DropLanguage_SelectedIndexChanged">
                        <asp:ListItem Text="(Bitte Auswählen)" Value="dropLanguageImpossibleValue" />
                        <asp:ListItem Text="Englisch" Value="1" />
                        <asp:ListItem Text="Deutsch" Value="2" />
                    </asp:DropDownList>
                </div>
            </div>

            <div runat="server" id="DivBillingStatus" class="form-group">
                <asp:Label runat="server" Text="Verrechnungsstatus:" CssClass="control-label col-md-3"></asp:Label>
                <asp:Label runat="server" ID="LabelBillingStatus" CssClass="col-md-6 alignbottom"></asp:Label>
            </div>
            <div runat="server" id="DivBillingStatusAdmin">
                <div class="form-group">
                    <asp:Label runat="server" Text="Verrechnungsstatus:" CssClass="control-label col-md-3"></asp:Label>
                    <div class="col-md-6">
                        <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="DropBillingStatus" AutoPostBack="true" OnSelectedIndexChanged="DropBillingStatusChanged" CssClass="form-control"></asp:DropDownList>
                    </div>
                </div>
            </div>

            <div runat="server" id="DivWebSummary" class="form-group">
                <asp:Label runat="server" Text="Websummary:" CssClass="control-label col-md-3"></asp:Label>
                <div class="col-md-4">
                    <asp:CheckBox runat="server" ID="cbxWebSummaryChecked" Text="Websummary kontrolliert" CssClass="form-control" AutoPostBack="true" OnCheckedChanged="cbxWebSummaryChecked_CheckedChanged" />
                </div>
                <asp:Label runat="server" ID="LabelWebsummaryLink" CssClass="col-md-2 alignbottom">Link</asp:Label>
            </div>

            <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="UpdateGradeFields">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DropLanguage" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbxWebSummaryChecked" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="DropBillingStatus" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <div runat="server" id="DivGradeStudent1" class="form-group">
                        <asp:Label runat="server" Text="Note:" CssClass="control-label col-md-3" ID="LabelGradeStudent1"></asp:Label>
                        <asp:Label runat="server" ID="NumGradeStudent1" CssClass="col-md-6 alignbottom"></asp:Label>
                        <div class="col-md-6">
                            <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="NumGradeStudent1Admin" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div runat="server" id="DivGradeStudent2" class="form-group">
                        <asp:Label runat="server" Text="Note:" CssClass="control-label col-md-3" ID="LabelGradeStudent2"></asp:Label>
                        <asp:Label runat="server" ID="NumGradeStudent2" CssClass="col-md-6 alignbottom"></asp:Label>
                        <div class="col-md-6">
                            <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="NumGradeStudent2Admin" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group" style="text-align: left" runat="server" visible="false" id="DivGradeWarning">
                        <asp:Label runat="server" ID="Label1" CssClass="col-md-6 col-md-offset-3" Text="Hinweis: Bitte das Projekt nach dem Eintragen der Noten abschliessen, damit die Noten an die Administration gesendet werden können."></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <hr />

            <asp:PlaceHolder runat="server" ID="BillAddressPlaceholder">
                <h3>Kundeninformationen</h3>
                <asp:UpdatePanel runat="server" ID="updateClientCompany" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="radioClientType" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Kundentyp:" CssClass="control-label col-md-3"></asp:Label>
                            <div class="col-md-6 radioButtonSettings">
                                <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" TextAlign="Right" BorderStyle="None" ID="radioClientType" AutoPostBack="true" OnSelectedIndexChanged="RadioClientType_SelectedIndexChanged">
                                    <asp:ListItem Text=" FHNW intern" Value="Intern" />
                                    <asp:ListItem Text=" Unternehmen" Value="Company" />
                                    <asp:ListItem Text=" Privatperson" Value="PrivatePerson" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div class="form-group" runat="server" id="divClientCompany">
                            <asp:Label runat="server" ID="LabelCompany" Text="Unternehmen*:" CssClass="control-label col-md-3"></asp:Label>
                            <asp:Label runat="server" ID="LabelClientCompany" CssClass="col-md-6 alignbottom"></asp:Label>
                            <div class="col-md-6">
                                <asp:TextBox runat="server" ID="txtClientCompanyAdmin" CssClass="form-control maxWidth" MaxLength="255"></asp:TextBox>
                            </div>
                        </div>
                        <div runat="server" id="divClientForm">
                            <div class="form-group">
                                <asp:Label runat="server" Text="Anrede*:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientTitle" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpClientTitleAdmin" AutoPostBack="false" CssClass="form-control">
                                        <asp:ListItem Text="Herr" Value="1" />
                                        <asp:ListItem Text="Frau" Value="2" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Vor- und Nachname*:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientName" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientNameAdmin" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="E-Mail Adresse*:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientEmail" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientEmailAdmin" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Telefonnummer:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientPhoneNumber" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientPhoneNumberAdmin" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left" runat="server" id="divClientDepartment">
                                <asp:Label runat="server" Text="Abteilung:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientDepartment" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientDepartmentAdmin" CssClass="form-control maxWidth" Placeholder="Falls vorhanden" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Strasse und Nummer:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientStreet" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientStreetAdmin" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="PLZ:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientPLZ" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientPLZAdmin" CssClass="form-control maxWidth" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Ort:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientCity" CssClass="col-md-9 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientCityAdmin" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Verrechnungs- / Auftragsnummer:" CssClass="control-label col-md-3"></asp:Label>
                                <asp:Label runat="server" ID="LabelClientReference" CssClass="col-md-6 alignbottom"></asp:Label>
                                <div class="col-md-6">
                                    <asp:TextBox runat="server" ID="txtClientReferenceAdmin" CssClass="form-control maxWidth" Placeholder="Falls vorhanden." ToolTip="z.B. Bestellnummer des Auftraggebers." MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="form-group" style="text-align: left">
                    <asp:Label runat="server" Text="Geheimhaltung:" CssClass="control-label col-md-3"></asp:Label>
                    <div class="col-md-6">
                        <asp:CheckBox runat="server" ID="chkNDA" CssClass="form-control" ToolTip="NDA-Projekte werden nicht in öffentlichen Broschüren aufgeführt" Text=" Projekt/Auftraggeber unter NDA" />
                    </div>
                </div>
                <h6>Mit * markierte Felder sind Pflichtfelder.</h6>
            </asp:PlaceHolder>
        </div>
        <div style="clear: both"></div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="BtnFinishProject" OnClick="BtnFinishProject_OnClick" CssClass="btn btn-default" Text="Projekt abschliessen" OnClientClick="return confirmSaving('Projekt wirklich abschliessen?');"></asp:Button>
            <asp:Button runat="server" ID="BtnCancelProject" OnClick="BtnCancelProject_OnClick" CssClass="btn btn-default" Text="Projekt abbrechen" OnClientClick="return confirmSaving('Projekt wirklich abbrechen?');"></asp:Button>
            <asp:Button runat="server" ID="BtnKickoffProject" OnClick="BtnKickoffProject_OnClick" CssClass="btn btn-default" Text="Projekt starten" OnClientClick="return confirmSaving('Projekt wirklich starten?');"></asp:Button>
            <asp:Button runat="server" ID="BtnDuplicateProject" OnClick="BtnDuplicateProject_OnClick" CssClass="btn btn-default" Text="Duplizieren"  OnClientClick="return Confirm();"  AutoPostBack="true" Style="margin-right: 0px;" />
            <asp:Button runat="server" ID="BtnSaveChanges" OnClick="BtnSaveChanges_OnClick" CssClass="btn btn-default" Text="Speichern & Schliessen"></asp:Button>
            <asp:Button runat="server" ID="BtnSaveBetween" OnClick="BtnSaveBetween_OnClick" CssClass="btn btn-default" Text="Zwischenspeichern" />
            <asp:Button runat="server" ID="BtnCancel" OnClick="BtnCancel_OnClick" CssClass="btn btn-default" Text="Abbrechen"></asp:Button>
        </div>
    </div>

    <div class="well newProjectSettings" id="DivAttachments" runat="server">
        <asp:Label runat="server" ID="lblProjectAttachements" Font-Size="24px" Height="50px" Text="Projekt Artefakte"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <asp:UpdatePanel runat="server" ID="updateProjectAttachements" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label runat="server" Text="Projekt Artefakte (Dokumentation, Präsentation, Code):" CssClass="control-label col-md-3"></asp:Label>
                    <div class="form-group col-md-9">
                        <asp:GridView runat="server" Width="100%" ID="gridProjectAttachs" EmptyDataText="Noch keine Dokumente hochgeladen." ItemType="ProStudCreator.ProjectSingleAttachment" EnableModelValidation="False" ValidateRequestMode="Disabled" OnSelectedIndexChanged="GridProjectAttachs_OnSelectedIndexChanged" CellPadding="4" EnableViewState="False" GridLines="None" AutoGenerateColumns="False" ForeColor="#333333" AllowSorting="False" OnRowCommand="GridProjectAttachs_OnRowCommand" OnRowDataBound="GridProjectAttachs_OnRowDataBound" DataKeyNames="Guid">
                            <Columns>
                                <asp:ImageField ItemStyle-Width="20px" DataImageUrlField="FileType" ControlStyle-Height="30px" />
                                <asp:BoundField DataField="Name" HeaderText="Dokumentname" ItemStyle-Wrap="False" />
                                <asp:BoundField DataField="Size" HeaderText="Dateigrösse" />
                                <asp:TemplateField ItemStyle-Wrap="False">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="deleteProjectAttachButton" ToolTip="Datei löschen" CommandName="deleteProjectAttach" OnClientClick="return confirm('Wollen Sie diese Datei wirklich löschen?');" CommandArgument="<%# Item.Guid %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#a5c7ff" Font-Bold="True" ForeColor="Black" />
                            <RowStyle BackColor="#FFFFFF" ForeColor="0000000"></RowStyle>
                        </asp:GridView>
                    </div>
                    <div style="clear: both"></div>
                    <div runat="server" id="divFileUpload">
                        <hr />
                        <asp:Label runat="server" Text="Upload Projekt Artefakte:" CssClass="control-label col-md-3"></asp:Label>
                        <div class="form-group">
                            <ajax:AjaxFileUpload runat="server" MaxFileSize="-1" OnUploadComplete="OnUploadComplete" ClearFileListAfterUpload="True" AutoStartUpload="True" ID="fileUpProjectAttach" AllowedFileTypes="7z,aac,avi,bz2,csv,doc,docx,gif,gz,htm,html,jpeg,jpg,md,mp3,mp4,ods,odt,ogg,pdf,png,ppt,pptx,svg,tar,tgz,txt,xls,xlsx,xml,zip" OnClientUploadCompleteAll="doPostBack" MaximumNumberOfFiles="-1"  />
                            <small class="col-md-offset-4">Dokumente mit gleichem Namen werden überschriben.</small>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel ID="updateDownloadButton" runat="server" UpdateMode="Conditional">
            <Triggers>
                <%--<asp:AsyncPostBackTrigger runat="server" ControlID="gridProjectAttachs" EventName="DataBound"/>--%>
                <asp:PostBackTrigger ControlID="BtnDownloadAllFiles"/>
            </Triggers>
            <ContentTemplate>
                <div runat="server" ID="divDownloadBtn" style="text-align: right;">
                <button runat="server" ID="btnDownloadAllFiles" class="btn btn-default" OnServerClick="DownloadFiles_OnClick"><img src="Content/zip.png" style="height: 30px;" alt="download"/>  Download ZIP </button>
            </div>
          </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <script type="text/javascript">
        function doPostBack() {
            var updatePanel1 = '<%=updateProjectAttachements.ClientID%>';
            __doPostBack(updatePanel1, '');
        }
    </script>
</asp:Content>

