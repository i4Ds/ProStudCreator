<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" EnableEventValidation="false" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectTopicImageControl" Src="~/UserControls/ProjectTopicImageControl.ascx" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="ProjectInofpageContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function isContentStud(currentObject) {
            var txtBoxValue = currentObject.value;
            var term = "@students.fhnw.ch";
            //var term2 = "@fhnw.ch";
            var index = txtBoxValue.indexOf(term);
            //var index2 = txtBoxValue.indexOf(term2);
            if (currentObject.value != "") {
                if (index == -1 /*&& index2 == -1*/) {
                    alert("Geben Sie eine E-Mail Adresse an, welche mit @students.fhnw.ch endet");
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
                        <div class="col-md-2">
                            <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="NumGradeStudent1Admin" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4 alignbottom">
                            <asp:Button runat="server" OnClick="Student1OpenGradingPopup_Click" Text="Bewertungsformular öffnen" />
                            <a runat="server" onclick=""></a>
                        </div>
                    </div>
                    <div runat="server" id="DivGradeStudent2" class="form-group">
                        <asp:Label runat="server" Text="Note:" CssClass="control-label col-md-3" ID="LabelGradeStudent2"></asp:Label>
                        <asp:Label runat="server" ID="NumGradeStudent2" CssClass="col-md-6 alignbottom"></asp:Label>
                        <div class="col-md-2">
                            <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="NumGradeStudent2Admin" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-4 alignbottom">
                            <asp:Button runat="server" OnClick="Student2OpenGradingPopup_Click" Text="Bewertungsformular öffnen" />
                        </div>
                    </div>
                    <div class="form-group" style="text-align: left" runat="server" visible="false" id="DivGradeWarning">
                        <asp:Label runat="server" ID="Label1" CssClass="col-md-6 col-md-offset-3"><small>Hinweis: Bitte das Projekt nach dem Eintragen der Noten abschliessen, damit die Noten an die Administration gesendet werden können.</small></asp:Label>
                    </div>


                    <div id="GradingV1Popup" style="display: none; height:80%; width:80%; padding: 16px; background-color: #f5f5f5;">
                        <div style="position:relative;height:100%;width:100%;left:0px;top:0px;padding-bottom:96px;">
                            <h3><asp:Label runat="server" ID="PopupTitle">Bewertungsformular</asp:Label></h3>
                            <ajaxToolkit:TabContainer runat="server" Width="100%" Height="100%" ScrollBars="Auto" TabStripPlacement="Bottom">
                                <ajaxToolkit:TabPanel runat="server" HeaderText="Gesamtbewertung">
                                    <ContentTemplate>
                                        <h4>Beurteilungsbogen für die Bachelor-Thesis</h4>
                                        <table class="gradingV1Overview">
                                            <tr>
                                                <td style="width:25%;">Student:</td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>Projekttitel:</td>
                                                <td style="height:3em;"></td>
                                            </tr>
                                            <tr>
                                                <td>Betreuender Dozent:</td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>Experte:</td>
                                                <td></td>
                                            </tr>
                                        </table>
                                        <asp:TextBox runat="server" TextMode="MultiLine" CssClass="gradingV1OverviewTextarea" Rows="20"></asp:TextBox>
                                        <table class="gradingV1Overview">
                                            <tr>
                                                <td>Note, übertragen vom Bewertungsbogen</td>
                                                <td style="width:10em;"></td>
                                            </tr>
                                        </table>

                                        <table class="gradingV1Overview">
                                            <tr>
                                                <td>Klassifizierung der Arbeit (Zutreffendes ankreuzen):</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RadioButton runat="server" Text="Grundsätzlich zur Veröffentlichung geeignet (nach Absprache mit dem Auftraggeber)" GroupName="OverviewNDA" /><br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:RadioButton runat="server" Text="Aus Gründen der Vertraulichkeit nicht zur Veröffentlichung und Einsichtnahme geeignet" GroupName="OverviewNDA" />
                                                </td>
                                            </tr>
                                        </table>

                                        <table class="gradingV1OverviewFinalTable">
                                            <tr>
                                                <td style="width:25%;">Windisch,</td>
                                                <td><%=DateTime.Now.ToString("dd.MM.yyyy") %></td>
                                            </tr>
                                            <tr>
                                                <td>Betreuender Dozent:</td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td>Experte:</td>
                                                <td></td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" HeaderText="Detailbewertung">
                                    <ContentTemplate>
                                        <h4>Bewertungsbogen: Projektarbeit 5 und Bachelorthesis (Projektarbeit 6)</h4>
                                        <p>
                                            <b>Bemerkungen:</b> Dieser Bewertungsbogen wird von der betreunden Person ausgefüllt. Bei zwei betreuenden Personen wird er von beiden unabhängig ausgefüllt und danach abgeglichen. Wo möglich und sinnvoll wird ein Kommentar zu jeder Bewertung verfasst. Die Studierenden erhalten in jedem Fall die Würdigung in Papierform. Falls erwünscht, wird auch der Bewertungsbogen in PDF-From abgegeben. Nach der Projektarbeit 5 muss dieser Bewertungsbogen zwingen mit den Studierenden besprochen und auf mögliches Verbesserungspotential für die kommende Projektarbeit 6 hingewiesen werden. Nach Abschluss der Projektarbeit 6 wird der Bewertungsbogen auf Wunsch der Studierenden mit diesen besprochen.					
                                        </p>
                                        <p style="color:#ff0000">
                                            Grundsatz: Die Note 5.0 ist zu erteilen, wenn für das jeweilige Kriterium die Leistung in vollem Umfang die Anforderungen an einen in der Industrie tätigen Ingenieur erfüllt.  					
                                        </p>
                                        <table class="gradingV1">
                                            <tr>
                                                <td></td>
                                                <td>Name:</td>
                                                <td>Gewichtung</td>
                                                <td>Note</td>
                                                <td>Beschreibung</td>
                                                <td>Kommentar</td>
                                            </tr>
                                            <tr class="sectionTitleRow">
                                                <td>1</td>
                                                <td colspan="5">ORGANISATION, PLANUNG, METHODIK</td>
                                            </tr>
                                            <tr>
                                                <td>1.1</td>
                                                <td class="name">
                                                    <b>Lösungskonzept/Strategie</b><br />
                                                    Gewichtung aufgrund der Komplexität des Projektes festlegen.
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtAStrategyWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtAStrategy" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Innovatives Lösungskonzept, übertrifft die Erwartungen klar, effektive kreative Strategie<br />
                                                    5: Lösungskonzept und Strategie umfassend, klar, präzise und effektiv<br />
                                                    4: Lösungskonzept und Strategie zielführend, Standardvorgehen<br />
                                                    3: Lösungskonzept nur teilweise nachvollziehbar, unklare Strategie<br />
                                                    2: Lösungskonzept nicht nachvollziehbar, keine Strategie<br />
                                                    1: Kein Lösungskonzept vorhanden
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtAStrategyComment"></asp:TextBox></td>
                                            </tr>

                                            <tr>
                                                <td>1.2</td>
                                                <td class="name">
                                                    <b>Projektvereinbarung: Inhalt</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtAProjectSummaryContentsWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtAProjectSummaryContents" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Abgabe termingerecht, überdurchschnittliche/unerwartete Analyse der Aufgabenstellung, Erfassung sämtlicher Einzelfragen im thematischen Zusammenhang, wesentlicher eigener inhaltlicher Beitrag zur Umsetzung<br />
                                                    5: Abgabe termingerecht; Vollständige Durchdringung der Aufgabenstellung, gesamtheitlicher Lösungsansatz und eigenständige kreative Umsetzung<br />
                                                    4: Abgabe Termingerecht, Aufgabenstellung eins zu eins umgesetzt, Abgrenzung von Teilaufgaben<br />
                                                    3: Abgabe termingerecht, Umsetzung der Aufgabenstellung nur teilweise erkennbar, ungenügende Analyse, unpassender Lösungsatz<br />
                                                    2: Umsetzung der Aufgabenstellung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin<br />
                                                    1: Keine Projektvereinbarung bis 2 Wochen nach Termin
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtAProjectSummaryContentsComment"></asp:TextBox></td>
                                            </tr>

                                            <tr>
                                                <td>1.3</td>
                                                <td class="name">
                                                    <b>Projektvereinbarung: Planung des Projektes</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtAProjectSummaryPlanningWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtAProjectSummaryPlanning" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Abgabe termingerecht, überdurchschnittliche und detaillierte Projektplanung, Arbeitsumfang realistisch abgeschätzt und abgebildet, genügend sinnvolle und klar messbare Meilensteine<br />
                                                    5: Abgabe termingerecht, umfängliche Projektplanung, Arbeitsumfang realitätsnah abgeschätzt und abgebildet, mit messabren Meilensteinen<br />
                                                    4: Abgabe termingerecht, Projektplanung enthält die wesentlichen Arbeitsschritte, Arbeitsumfang weitgehend realitätsnah abgeschätzt, teilweise messbare Meilensteine<br />
                                                    3: Abgabe termingerecht, Projektplanung enthält nicht alle Arbeitsschritte, Arbeitsumfang wird teilweise deutlich über-/unterschätzt, zu wenig oder nicht messbare Meilensteine<br />
                                                    2: Umsetzung der Projektplanung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin<br />
                                                    1: Keine Projektvereinbarung bis 2 Wochen nach Termin
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtAProjectSummaryPlanningComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="sectionSummaryCell">Blocknote 1 - Gewicht: 1</td>
                                                <td class="sectionSummaryCell centered">1</td>
                                                <td class="sectionSummaryCell centered"><asp:Label runat="server" ID="Grade1"></asp:Label></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>










                                            <tr class="sectionTitleRow">
                                                <td>2</td>
                                                <td colspan="5">FACHLICHES, ANWENDUNG VON WISSEN, SELBSTSTÄNDIGKEIT</td>
                                            </tr>
                                            <tr>
                                                <td>2.1</td>
                                                <td class="name">
                                                    <b>Theoretische Arbeit</b><br />
                                                    Die Gewichtung soll der Ausrichtung des Projekts entsprechend Richtung Theorie oder Praxis verschoben werden.
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBTheoreticalWorkWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtBTheoreticalWork" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Neuartiger Lösungsansatz, der die üblichen theoretischen Grundlagenkenntnisse von Studierenden klar übertrifft, sehr gut und umfassend umgesetzt<br />
                                                    5: Problem umfassend in allen Aspekten gelöst<br />
                                                    4: Problem mit bekannten Konzepten und Tools in seinen wesentlichen Aspekten gelöst<br />
                                                    3: Unzureichender theoretischer Hintergrund, teilweise falsche Argumentation<br />
                                                    2: Theoretischer Hintergrund nicht ersichtlich, keine logische Argumentation<br />
                                                    1: Keine Bearbeitung eines theoretischen Hintergrundes
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtBTheoreticalWorkComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>2.2</td>
                                                <td class="name">
                                                    <b>Praktische Arbeit</b><br />
                                                    Die Gewichtung soll der Ausrichtung des Projekts entsprechend Richtung Theorie oder Praxis verschoben werden.
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBPracticalWorkWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtBPracticalWork" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Äusserst umfassender und effizienter Einsatz der verfügbaren Mittel und Verfahren, Entwicklung problemspezifischer neuer Methoden<br />
                                                    5: Die in Frage kommenden Verfahren werden in korrekter Gewichtung umfassend und effektiv eingesetzt<br />
                                                    4: Ausgewählte Standardverfahren und Vorgehensweisen werden zuverlässig eingesetzt<br />
                                                    3: Eingesetzte Verfahren nur teilweise angemessen, Durchführung unzureichend<br />
                                                    2: Keine oder falsche Verfahren angewendet, keine oder unbrauchbare Durchführung<br />
                                                    1: Mutwillig falscher Einsatz von Verfahren mit resultierenden Schäden an Personal und/oder Geräten
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtBPracticalWorkComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>2.3</td>
                                                <td class="name">
                                                    <b>Analyse von Ergebnissen</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBEvaluationWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtBEvaluation" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Sämtliche Fragestellungen aus der PV beantwortet, Ergebnisse umfassend und kritisch analysiert, klare Schlussfolgerungen gezogen und Vorschläge für Umsetzung und Vertiefung erarbeitet<br />
                                                    5: Bis auf wenige Details alle Fragestellungen der PV beantwortet, vollständige Analyse der Ergebnisse, ausgerichtet auf deren Umsetzung<br />
                                                    4: Die wichtigsten Fragestellungen der PV beantwortet, Analyse beschränkt sich auf Vergleich mit Aufgabenstellung, keine weiterführenden Aussagen<br />
                                                    3: Fragestellungen der PV nur teilweise beantwortet, Ergebnisse unvollständig analysiert, teilweise falsche Schlussfolgerungen<br />
                                                    2: Fragestellungen der PV kaum beantwortet, nicht in der Lage, die Ergebnisse einzuordnen und zu bewerten<br />
                                                    1: Keine Bewertung der Ergebnisse durchgeführt und dokumentiert
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtBEvaluationComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>2.4</td>
                                                <td class="name">
                                                    <b>Zielerreichung</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtResultsWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtResults" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Ziel übertroffen, zusätzlicher  unerwarteter Kundennutzen und Erkenntnisgewinn<br />
                                                    5: Ziel vollumfänglich erreicht<br />
                                                    4: Ziel im Wesentlichen erreicht, Einzelaspekte ergänzungsbedürftig<br />
                                                    3: Weniger als die Hälfte der Ziele erreicht<br />
                                                    2: Die meisten Ziele wurden nicht erreicht, Ergebnisse nicht brauchbar<br />
                                                    1: Kein Ziel wurde erreicht
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtResultsComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>2.5</td>
                                                <td class="name">
                                                    <b>Selbstständigkeit/Betreuungsintensität</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBAutonomyWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtBAutonomy" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Sehr geringer Betreuungsaufwand. Kritik und Anregungen von Betreuenden und Aussenstehenden werden kritisch hinterfragt, selbstständig weiterentwickelt und bestmöglichst umgesetzt.<br />
                                                    5: Geringer Betreuungsaufwand. Kritik und Anregungen von Betreuenden und Aussenstehenden werden umfänglich und gut umgesetzt.<br />
                                                    4: Durchschnittlicher Betreuungsaufwand. Kritik und Anregungen von Betreuenden werden weitgehend umgesetzt.<br />
                                                    3: Überdurchschnittlicher Betreungsaufwand. Anregungen werden nur teilweise umgesetzt oder es muss daran erinnert werden.<br />
                                                    2: Hoher Betreungsaufwand. Auch nach mehrmaliger Erinnerung nur einfachste Anregungen umgesetzt.<br />
                                                    1: Trotz sehr hohem Betreuungsaufwand wurden selbst einfache Anregungen nicht umgesetzt.
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtBAutonomyComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="sectionSummaryCell">Blocknote 2 - Gewicht: 4</td>
                                                <td class="sectionSummaryCell centered">4</td>
                                                <td class="sectionSummaryCell centered"><asp:Label runat="server" ID="Grade2"></asp:Label></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>













                                            <tr class="sectionTitleRow">
                                                <td>3</td>
                                                <td colspan="5">DOKUMENTATION, WISSENSTRANSFER</td>
                                            </tr>
                                            <tr>
                                                <td>3.1</td>
                                                <td class="name">
                                                    <b>Bericht/Dokumentation</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtCDocumentationWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtCDocumentation" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Bericht nachvollziehbar, sehr gute Leseführung, Inhalte logisch strukturiert, sehr umfassen informativ, formal sowie sprachlich und gestalterisch herausragend<br />
                                                    5: Bericht inhaltlich vollständig, gut nachvollziehbar, formal korrekt, sprachlich und gestalterisch ansprechend<br />
                                                    4: Alle wesentlichen Aspekte dokumentiert, wenig Leseführung, inhaltlich und sprachlich mehrheitlich verständlich<br />
                                                    3: Nur ein Teil der wesentlichen Aspekte dokumentiert, anstrengend zu lesen, Darstellung verbesserungswürdig<br />
                                                    2: Wesentliche Aspekte nicht dokumentiert, Bericht unstrukturiert, Darstellung mangelhaft, formal ungenügend<br />
                                                    1: Keine Dokumentation zum Abgabetermin vorhanden
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtCDocumentationComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>3.2</td>
                                                <td class="name">
                                                    <b>Verteidigung (P6)</b><br />
                                                    Bei P5 Gewicht auf 0 setzen
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtCDefenseWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtCDefense" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Alle Fragen richtig und souverän beantwortet, Frage in Kontext eingeordnet, eine Venetzung der Fachinhalte ist klar ersichtlich
                                                    5: Alle Fragen korrekt und auf die Arbeit bezogen beantwortet
                                                    4: Fragen zögernd, aber im Wesentlichen korrekt beantwortet, manchmal Probleme bei spezifischen Details
                                                    3: Fragen teilweise falsch beantwortet, kein Detailwissen vohanden
                                                    2: Überwiegende Mehrzahl der Fragen nicht oder nicht korrekt beantwortet, die übrigen nur mangelhaft
                                                    1: Nicht auf Fragen eingegangen, keine richtige Antwort
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtCDefenseComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>3.3</td>
                                                <td class="name">
                                                    <b>Präsentationen (Zwischen- und Schlusspräsentation, P5 und P6)</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtCPresentationsWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtCPresentations" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Inhaltlich vollständiger und logisch aufgebauter Vortrag,  grafisch sehr gut gestaltet (unterstützend) und souverän vorgetragen, Fragen korrekt und umfassend beantwortet<br />
                                                    5: Vortrag inhaltlich vollständig, Aufbau und Präsentation logisch und ansprechend, Fragen korrekt beantwortet<br />
                                                    4: Im Vortrag relevante Inhalte behandelt, Einschränkungen in Aufbau (inkl. Folien) und Präsentationstechnik, Fragen gut beantwortet<br />
                                                    3: Vortrag mit inhaltlichen Lücken, Aufbau unklar / unlogisch, Fragen nur teilweise richtig beantwortet<br />
                                                    2: Vortrag inhaltlich unzureichend, Präsentation mangelhaft, Fragen nicht oder kaum beantwortet<br />
                                                    1: Vortrag mit falschem/viel zu wenig Inhalt, Präsentation sehr schwach, keine Fragen beantwortet
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtCPresentationsComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="sectionSummaryCell">Blocknote 3 - Gewicht: 2</td>
                                                <td class="sectionSummaryCell centered">2</td>
                                                <td class="sectionSummaryCell centered"><asp:Label runat="server" ID="Grade3"></asp:Label></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>










                                            <tr class="sectionTitleRow">
                                                <td>4</td>
                                                <td colspan="5">KOMMUNIKATION, MOTIVATION</td>
                                            </tr>
                                            <tr>
                                                <td>4.1</td>
                                                <td class="name">
                                                    <b>Zusammenarbeit und Kommunikation intern</b><br />
                                                    Die Gewichtung soll der Ausrichtung des Projekts entsprechend Richtung Intern oder Extern verschoben werden.
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtDCollaborationInternalWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtDCollaborationInternal" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    Eine gute Zusammenarbeit und Kommunikation beinhaltet die Einladung zu Sitzung mit Traktandenlisten, das Führen eines Sitzungsprotokolls mit Dokumentation der wichtigen Beschlüsse, das kommentierte Nachbessern von Zeitplänen sowie die Aktivierung von internem Wissen. Der/die Studierende setzte dies in folgendem Masse um:<br />
                                                    6: Äusserst selbständig<br />
                                                    5: Angemessen<br />
                                                    4: Ausreichend<br />
                                                    3: Auf Anregung<br />
                                                    2: Nach mehrmaligem Nachfragen<br />
                                                    1: Nicht/unzureichend
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtDCollaborationInternalComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>4.2</td>
                                                <td class="name">
                                                    <b>Zusammenarbeit und Kommunikation extern</b><br />
                                                    Die Gewichtung soll der Ausrichtung des Projekts entsprechend Richtung Intern oder Extern verschoben werden.
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtDCollaborationExternalWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtDCollaborationExternal" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    Eine gute Zusammenarbeit und Kommunikation beinhaltet die Einladung zu Sitzung mit Traktandenlisten, das Führen eines Sitzungsprotokolls mit Dokumentation der wichtigen Beschlüsse, das kommentierte Nachbessern von Zeitplänen sowie die Aktivierung von internem Wissen. Der/die Studierende setzte dies in folgendem Masse um:<br />
                                                    6: Äusserts selbständig<br />
                                                    5: Angemessen<br />
                                                    4: Ausreichend<br />
                                                    3: Auf Anregung<br />
                                                    2: Nach mehrmaligem Nachfragen<br />
                                                    1: Nicht/unzureichend
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtDCollaborationExternalComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td>4.3</td>
                                                <td class="name">
                                                    <b>Motivation, pers. Einsatz, Umfang</b>
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtDMotivationWeight" CssClass="form-control"></asp:TextBox></td>
                                                <td><asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="txtDMotivation" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    6: Persönlicher Einsatz hervorragend, Arbeitsumfang ausserordentlich hoch<br />
                                                    5: Hoher persönlicher Einsatz, überdurchschnittlicher Arbeitsumfang<br />
                                                    4: Persönlicher Einsatz gerade noch ausreichend, Arbeitsumfang durchschnittlich<br />
                                                    3: Unzureichender persönlicher Einsatz, Arbeitsumfang unterdurchschnittlich<br />
                                                    2: Nicht motiviert, Einsatz mangelhaft, Arbeitsumfang unzureichend<br />
                                                    1: Demotiviert / destruktiv, Einsatz nicht vorhanden, kaum Arbeiten geleistet
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtDMotivationComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="sectionSummaryCell">Blocknote 4 - Gewicht: 1</td>
                                                <td class="sectionSummaryCell centered">1</td>
                                                <td class="sectionSummaryCell centered"><asp:Label runat="server" ID="Grade4"></asp:Label></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td style="font-weight:bold;" class="bonusCell">Zwischennote vor Bonus</td>
                                                <td style="font-weight:bold;" colspan="2" class="bonusCell centered"><asp:Label runat="server" ID="GradePreBonus"></asp:Label></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>






                                            <tr class="sectionTitleRow">
                                                <td>5</td>
                                                <td colspan="5">BONUS</td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="name">
                                                    <b>Neue Thematik</b>
                                                </td>
                                                <td colspan="2"><asp:TextBox runat="server" TextMode="Number" min="0" max="10" step="5" ID="txtENewTopic" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    10: Mit der Thematik noch nie Kontakt gehabt<br />
                                                    5: Thematik bekannt (z.B. durch Unterricht)<br />
                                                    0: Mit Thema vertraut (z.B. durch Semesterarbeit)
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtENewTopicComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="name">
                                                    <b>Schwierigkeitsgrad</b>
                                                </td>
                                                <td colspan="2"><asp:TextBox runat="server" TextMode="Number" min="0" max="10" step="5" ID="txtEDifficulty" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    10: Ausserordentlich hoch<br />
                                                    5: Überdurchschnittlich<br />
                                                    0: Standard
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtEDifficultyComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="name">
                                                    <b>Umfeld</b><br />
                                                    (Projektpartner, Lieferanten, usw.)
                                                </td>
                                                <td colspan="2"><asp:TextBox runat="server" TextMode="Number" min="0" max="10" step="5" ID="txtEEnvironment" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    10: Ausserordentlich schwierig<br />
                                                    5: Schwierig<br />
                                                    0: Standard
                                                </td>
                                                <td><asp:TextBox runat="server" TextMode="MultiLine" Columns="50" Rows="6" ID="txtEEnvironmentComment"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td class="bonusCell">
                                                    <b>Bonus</b><br />
                                                    absolute Notenkorrektur<br />
                                                    festgelegt durch Betreuer aufgrund der
                                                    Bonuspunkte (Wert wird nicht berechnet)
                                                </td>
                                                <td class="bonusCell" colspan="2"><asp:TextBox runat="server" TextMode="Number" min="0" max="0.2" step="0.1" ID="txtEBonus" CssClass="form-control"></asp:TextBox></td>
                                                <td class="small">
                                                    Absolute Korrektur;<br />
                                                    Anwendung z. B. bei mehreren Partnern mit unterschiedlichen Schwerpunkten; komplexer Datenanalyse; neuen, noch nicht erprobten Tools u.Ä.
                                                </td>
                                                <td></td>
                                            </tr>

                                            <tr>
                                                <td></td>
                                                <td class="finalCell">GESAMTNOTE</td>
                                                <td style="text-align:center" colspan="2" class="finalCell"><asp:Label runat="server" ID="GradeTotal"></asp:Label></td>
                                                <td></td>
                                                <td></td>
                                            </tr>

                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" HeaderText="Anleitung">
                                    <ContentTemplate>
                                        <h4>Anleitung zum Ausfüllen der Detailbewertung</h4>
                                        <p>
                                            Nur die grau unterlegten Felder ausfüllen.
                                        </p>
                                        <p>
                                            Gewichtungsfaktoren (Spalte C) in der Anphangsphase des Projektes festlegen.
                                        </p>
                                        <p>
                                            Die Gewichtungen der Blocknoten (Zellen C10, C17, C22 und C27) sind vorgegeben.
                                        </p>
                                        <p>
                                            Bonuspunkte: nur für grössere bzw. unvorhergesehene Herausforderungen in 3 Bereichen.
                                            Diese Punkte gehen nicht in eine Rechnung ein, sie sind nur ein Hilfsmittel für den Betreuer.<br />
                                            Achtung: Bonus nicht doppelt vergeben, d.h. grössere Herausforderungen entweder bei der
                                            Benotung im oberen Teil berücksichtigen oder mit dem Bonus.<br />
                                            Mit dem Bonus keine Notenkosmetik betreiben.
                                        </p>
                                        <p>
                                            Bonusnote (Feld C33):  Zusatznotenpunkte, gerundet auf 1/10.<br />
                                            Sparsam einsetzen, max. 0.2 Notenpunkte!
                                        </p>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel runat="server" HeaderText="Kommunikation der Bewertung">
                                    <ContentTemplate>
                                        <h4>Ziel</h4>
                                        <p>
                                            Lerneffekt durch die Bewertung und die Rückmeldungen.
                                        </p>
                                        <h4>Grundsatz</h4>
                                        <p>
                                            Eine detaillierte Kommunikation der Bewertung ist bei den P5 obligatorisch (kann auch erst zu Beginn des Folgesemesters stattfinden) und bei den P6 wünschenswert (manchmal sind die Studierenden gar nicht mehr verfügbar). Ist die Thesis im ersten Anlauf nicht bestanden, ist ein Feedback obligatorisch.
                                        </p>
                                        <h4>Einzelheiten</h4>
                                        <p>
                                            Die Studierenden kennen den Bewertungsbogen ab Beginn P5, P6.
                                        </p>
                                        <p>
                                            Die Gewichtungsfaktoren im Blatt "2. Detailbewertung" werden in der Anfangsphase des Projektes festgelegt (der Aufgabenstellung angepasst) und den Studierenden abgegeben.
                                        </p>
                                        <p>
                                            Bei der Abschlussbesprechung der Arbeit wird das Blatt "1. Gesamtbewertung" an die Studierenden abgegeben.
                                        </p>
                                        <p>
                                            Das Blatt "2. Detailbewertung" wird ebenfalls an die Studierenden abgegeben, ggf. aber ohne die Kommentare (Spalte G). Die Teilnoten (Spalte D) werden abgegeben.
                                        </p>
                                        <p>
                                            Die Kommentare (Spalte G) sind ein Hilfsmittel für die Projektbetreuer und u.U. nicht geeignet, weitergegeben zu werden. Sie dienen darum primär als Grundlage für die Notengebung und die Schlussbesprechung, aber nicht direkt für die Kommunikation der Bewertung.
                                        </p>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>
                            <div style="clear:both;text-align:center;padding-top:12px;">
                                <asp:Button OnClick="GradingPopupCloseOk_Click" runat="server" Text="Ok" />
                                <asp:Button OnClick="GradingPopupCloseCancel_Click" runat="server" Text="Abbrechen" />
                            </div>
                        </div>
                    </div>
                    <asp:Button style="display:none;" runat="server" id="DummyPopupTarget" />
                    <ajaxToolkit:ModalPopupExtender runat="server" TargetControlID="DummyPopupTarget" BackgroundCssClass="PopupBackground" PopupDragHandleControlID="PopupTitle" PopupControlID="GradingV1Popup" RepositionMode="RepositionOnWindowResizeAndScroll" ID="PopupExtender" />

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
                <small>Mit * markierte Felder sind Pflichtfelder.</small>
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
                    <asp:Label runat="server" Text="Projekt Artefakte (Doku, Code, Excel-Bewertungsformular, ...):" CssClass="control-label col-md-3"></asp:Label>
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
                        <div class="col-md-offset-3">
                            <ajax:AjaxFileUpload runat="server" MaxFileSize="-1" OnUploadComplete="OnUploadComplete" ClearFileListAfterUpload="True" AutoStartUpload="True" ID="fileUpProjectAttach" AllowedFileTypes="7z,aac,avi,bz2,csv,doc,docx,gif,gz,htm,html,jpeg,jpg,md,mp3,mp4,ods,odt,ogg,pdf,png,ppt,pptx,svg,tar,tgz,txt,xls,xlsx,xml,zip" OnClientUploadCompleteAll="doPostBack" MaximumNumberOfFiles="-1"  />
                        </div>
                        <small class="col-md-offset-3">Dokumente mit gleichem Namen werden überschriben.</small>
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

