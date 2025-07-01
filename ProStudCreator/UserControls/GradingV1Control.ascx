<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradingV1Control.ascx.cs" Inherits="ProStudCreator.UserControls.GradingV1Control" %>

<style type="text/css">
    .gradingV1 td {
        vertical-align: top;
        border-collapse: collapse;
        border: 1px solid #000;
    }

    .gradingV1 .sectionTitleRow td {
        background-color: #D8E4BC;
        font-weight: bold;
        height: 3em;
        vertical-align: middle;
        text-align: center;
    }

    .gradingV1 .name {
        font-style: italic;
    }

    .gradingV1 .sectionSummaryCell {
        font-weight: bold;
        font-style: italic;
        background-color: #E6B8B7;
        height: 3em;
        vertical-align: middle;
    }

    .gradingV1 .bonusCell {
        font-style: italic;
        background-color: #B1A0C7;
        height: 3em;
        vertical-align: middle;
    }

    .gradingV1 .finalCell {
        font-weight: bold;
        font-size: 150%;
        background-color: #ff0000;
        vertical-align: middle;
    }

    .gradingV1 input, textarea {
        background-color: #d9d9d9;
        color: #0000ff;
    }

    .gradingV1 textarea {
        height: initial !important;
        min-height: initial !important;
        max-height: initial !important;
    }

    .gradingV1 input {
        width: 6em;
        margin-left: auto;
        margin-right: auto;
    }

    .gradingV1 input {
        text-align: center;
    }

    .gradingV1 .small {
        font-size: 80%;
    }

    .gradingV1 .centered {
        text-align: center;
    }

    .gradingV1Overview {
        width: 100%;
        font-weight: bold;
        margin-bottom: 1.5em;
    }

        .gradingV1Overview label {
            font-weight: normal;
            padding-left: 0.5em;
        }

        .gradingV1Overview td {
            border: 1px solid #000;
            border-collapse: collapse;
            vertical-align: top;
        }

    .gradingV1OverviewTextarea {
        width: 100%;
        margin-bottom: 1.5em;
    }

    .gradingV1OverviewFinalTable {
        width: 100%;
        border: 1px solid #000;
    }

        .gradingV1OverviewFinalTable td {
            height: 4em;
        }
</style>

<asp:UpdatePanel runat="server" UpdateMode="Conditional" style="width: 100%; height: 100%" ChildrenAsTriggers="false">
    <ContentTemplate>
        <ajaxToolkit:TabContainer runat="server" Width="100%" Height="100%" ScrollBars="Auto" TabStripPlacement="Bottom">
            <ajaxToolkit:TabPanel runat="server" HeaderText="Gesamtbewertung">
                <ContentTemplate>
                    <h4>Beurteilungsbogen für die Bachelor-Thesis</h4>
                    <table class="gradingV1Overview">
                        <tr>
                            <td style="width: 25%;">Student:</td>
                            <td>
                                <asp:Label runat="server" ID="lblStudent" /></td>
                        </tr>
                        <tr>
                            <td>Projekttitel:</td>
                            <td style="height: 3em;">
                                <asp:Label runat="server" ID="lblProjectTitle" /></td>
                        </tr>
                        <tr>
                            <td>Betreuender Dozent:</td>
                            <td>
                                <asp:Label runat="server" ID="lblAdvisor" /></td>
                        </tr>
                        <tr>
                            <td>Experte:</td>
                            <td>
                                <asp:Label runat="server" ID="lblExpert" /></td>
                        </tr>
                    </table>
                    <asp:TextBox runat="server" TextMode="MultiLine" CssClass="gradingV1OverviewTextarea" Rows="20"
                        ID="txtCriticalAcclaim"></asp:TextBox>
                    <table class="gradingV1Overview">
                        <tr>
                            <td>Note, übertragen vom Bewertungsbogen</td>
                            <td style="width: 10em;">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblFinalGradeFrontPage"></asp:Label></td>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>

                    <table class="gradingV1Overview">
                        <tr>
                            <td>Klassifizierung der Arbeit (Zutreffendes ankreuzen):</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton ID="chkNotUnderNDA" runat="server" Text="Grundsätzlich zur Veröffentlichung geeignet (nach Absprache mit dem Auftraggeber)" GroupName="OverviewNDA" /><br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton ID="chkUnderNDA" runat="server" Text="Aus Gründen der Vertraulichkeit nicht zur Veröffentlichung und Einsichtnahme geeignet" GroupName="OverviewNDA" />
                            </td>
                        </tr>
                    </table>

                    <table class="gradingV1OverviewFinalTable">
                        <tr>
                            <td style="width: 25%;">Windisch,</td>
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
                    <p style="color: #ff0000">
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
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ClientIDMode="Static" ID="txtAStrategyWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtAStrategy" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.AStrategySchema.Replace("\n", "<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtAStrategyComment"></asp:TextBox></td>
                        </tr>

                        <tr>
                            <td>1.2</td>
                            <td class="name">
                                <b>Projektvereinbarung: Inhalt</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtAProjectSummaryContentsWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtAProjectSummaryContents" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.AProjectSummaryContentsSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtAProjectSummaryContentsComment"></asp:TextBox></td>
                        </tr>

                        <tr>
                            <td>1.3</td>
                            <td class="name">
                                <b>Projektvereinbarung: Planung des Projektes</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtAProjectSummaryPlanningWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtAProjectSummaryPlanning" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.AProjectSummaryPlanningSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtAProjectSummaryPlanningComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="sectionSummaryCell">Blocknote 1 - Gewicht: 1</td>
                            <td class="sectionSummaryCell centered">1</td>
                            <td class="sectionSummaryCell centered">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblGradeA"></asp:Label></td>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBTheoreticalWorkWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtBTheoreticalWork" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.BTheoreticalWorkSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtBTheoreticalWorkComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>2.2</td>
                            <td class="name">
                                <b>Praktische Arbeit</b><br />
                                Die Gewichtung soll der Ausrichtung des Projekts entsprechend Richtung Theorie oder Praxis verschoben werden.
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBPracticalWorkWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtBPracticalWork" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.BPracticalWorkSchema.Replace("\n","<br/>") %> </td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtBPracticalWorkComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>2.3</td>
                            <td class="name">
                                <b>Analyse von Ergebnissen</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBEvaluationWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtBEvaluation" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.BEvaluationSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtBEvaluationComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>2.4</td>
                            <td class="name">
                                <b>Zielerreichung</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBResultsWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtBResults" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.BResultsSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtBResultsComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>2.5</td>
                            <td class="name">
                                <b>Selbstständigkeit/Betreuungsintensität</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtBAutonomyWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtBAutonomy" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.BAutonomySchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtBAutonomyComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="sectionSummaryCell">Blocknote 2 - Gewicht: 4</td>
                            <td class="sectionSummaryCell centered">4</td>
                            <td class="sectionSummaryCell centered">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblGradeB"></asp:Label></td>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtCDocumentationWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtCDocumentation" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.CDocumentationSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtCDocumentationComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>3.2</td>
                            <td class="name">
                                <b>Verteidigung (P6)</b><br />
                                Bei P5 Gewicht auf 0 setzen
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0" max="2" step="0.5" ID="txtCDefenseWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtCDefense" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.CDefenseSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtCDefenseComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>3.3</td>
                            <td class="name">
                                <b>Präsentationen (Zwischen- und Schlusspräsentation, P5 und P6)</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtCPresentationsWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtCPresentations" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.CPresentationsSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtCPresentationsComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="sectionSummaryCell">Blocknote 3 - Gewicht: 2</td>
                            <td class="sectionSummaryCell centered">2</td>
                            <td class="sectionSummaryCell centered">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblGradeC"></asp:Label></td>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtDCollaborationInternalWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtDCollaborationInternal" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.DCollaborationInternalSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtDCollaborationInternalComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>4.2</td>
                            <td class="name">
                                <b>Zusammenarbeit und Kommunikation extern</b><br />
                                Die Gewichtung soll der Ausrichtung des Projekts entsprechend Richtung Intern oder Extern verschoben werden.
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0" max="2" step="0.5" ID="txtDCollaborationExternalWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtDCollaborationExternal" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.DCollaborationExternalSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtDCollaborationExternalComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>4.3</td>
                            <td class="name">
                                <b>Motivation, pers. Einsatz, Umfang</b>
                            </td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0.5" max="2" step="0.5" ID="txtDMotivationWeight" CssClass="form-control"></asp:TextBox></td>
                            <td>
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="1" max="6" step="0.1" ID="txtDMotivation" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.DMotivationSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtDMotivationComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="sectionSummaryCell">Blocknote 4 - Gewicht: 1</td>
                            <td class="sectionSummaryCell centered">1</td>
                            <td class="sectionSummaryCell centered">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblGradeD"></asp:Label></td>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td style="font-weight: bold;" class="bonusCell">Zwischennote vor Bonus</td>
                            <td style="font-weight: bold;" colspan="2" class="bonusCell centered">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblGradePreBonus"></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
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
                            <td colspan="2">
                                <asp:TextBox runat="server" TextMode="Number" min="0" max="10" step="5" ID="txtENewTopic" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.ENewTopicSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtENewTopicComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="name">
                                <b>Schwierigkeitsgrad</b>
                            </td>
                            <td colspan="2">
                                <asp:TextBox runat="server" TextMode="Number" min="0" max="10" step="5" ID="txtEDifficulty" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.EDifficultySchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtEDifficultyComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="name">
                                <b>Umfeld</b><br />
                                (Projektpartner, Lieferanten, usw.)
                            </td>
                            <td colspan="2">
                                <asp:TextBox runat="server" TextMode="Number" min="0" max="10" step="5" ID="txtEEnvironment" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.EEnvironmentSchema.Replace("\n","<br/>") %></td>
                            <td>
                                <asp:TextBox runat="server" TextMode="MultiLine" Columns="30" Rows="6" ID="txtEEnvironmentComment"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td class="bonusCell">
                                <b>Bonus</b><br />
                                absolute Notenkorrektur<br />
                                festgelegt durch Betreuer aufgrund der Bonuspunkte (Wert wird nicht berechnet)
                            </td>
                            <td class="bonusCell" colspan="2">
                                <asp:TextBox runat="server" AutoPostBack="true" OnTextChanged="OnGradeChanged" TextMode="Number" min="0" max="0.2" step="0.1" ID="txtEBonus" CssClass="form-control"></asp:TextBox></td>
                            <td class="small"><%= ProStudCreator.GradingV1.EBonusSchema.Replace("\n","<br/>") %></td>
                            <td></td>
                        </tr>

                        <tr>
                            <td></td>
                            <td class="finalCell">GESAMTNOTE</td>
                            <td style="text-align: center" colspan="2" class="finalCell">
                                <asp:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="lblGradeTotal"></asp:Label>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
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
    </ContentTemplate>
</asp:UpdatePanel>
