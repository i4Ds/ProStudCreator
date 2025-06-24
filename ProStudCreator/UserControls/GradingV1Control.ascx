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
                            <td class="small">6: Innovatives Lösungskonzept, übertrifft die Erwartungen klar, effektive kreative Strategie<br />
                                5: Lösungskonzept und Strategie umfassend, klar, präzise und effektiv<br />
                                4: Lösungskonzept und Strategie zielführend, Standardvorgehen<br />
                                3: Lösungskonzept nur teilweise nachvollziehbar, unklare Strategie<br />
                                2: Lösungskonzept nicht nachvollziehbar, keine Strategie<br />
                                1: Kein Lösungskonzept vorhanden
                            </td>
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
                            <td class="small">6: Abgabe termingerecht, überdurchschnittliche/unerwartete Analyse der Aufgabenstellung, Erfassung sämtlicher Einzelfragen im thematischen Zusammenhang, wesentlicher eigener inhaltlicher Beitrag zur Umsetzung<br />
                                5: Abgabe termingerecht; Vollständige Durchdringung der Aufgabenstellung, gesamtheitlicher Lösungsansatz und eigenständige kreative Umsetzung<br />
                                4: Abgabe Termingerecht, Aufgabenstellung eins zu eins umgesetzt, Abgrenzung von Teilaufgaben<br />
                                3: Abgabe termingerecht, Umsetzung der Aufgabenstellung nur teilweise erkennbar, ungenügende Analyse, unpassender Lösungsatz<br />
                                2: Umsetzung der Aufgabenstellung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin<br />
                                1: Keine Projektvereinbarung bis 2 Wochen nach Termin
                            </td>
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
                            <td class="small">6: Abgabe termingerecht, überdurchschnittliche und detaillierte Projektplanung, Arbeitsumfang realistisch abgeschätzt und abgebildet, genügend sinnvolle und klar messbare Meilensteine<br />
                                5: Abgabe termingerecht, umfängliche Projektplanung, Arbeitsumfang realitätsnah abgeschätzt und abgebildet, mit messabren Meilensteinen<br />
                                4: Abgabe termingerecht, Projektplanung enthält die wesentlichen Arbeitsschritte, Arbeitsumfang weitgehend realitätsnah abgeschätzt, teilweise messbare Meilensteine<br />
                                3: Abgabe termingerecht, Projektplanung enthält nicht alle Arbeitsschritte, Arbeitsumfang wird teilweise deutlich über-/unterschätzt, zu wenig oder nicht messbare Meilensteine<br />
                                2: Umsetzung der Projektplanung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin<br />
                                1: Keine Projektvereinbarung bis 2 Wochen nach Termin
                            </td>
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
                            <td class="small">6: Neuartiger Lösungsansatz, der die üblichen theoretischen Grundlagenkenntnisse von Studierenden klar übertrifft, sehr gut und umfassend umgesetzt<br />
                                5: Problem umfassend in allen Aspekten gelöst<br />
                                4: Problem mit bekannten Konzepten und Tools in seinen wesentlichen Aspekten gelöst<br />
                                3: Unzureichender theoretischer Hintergrund, teilweise falsche Argumentation<br />
                                2: Theoretischer Hintergrund nicht ersichtlich, keine logische Argumentation<br />
                                1: Keine Bearbeitung eines theoretischen Hintergrundes
                            </td>
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
                            <td class="small">6: Äusserst umfassender und effizienter Einsatz der verfügbaren Mittel und Verfahren, Entwicklung problemspezifischer neuer Methoden<br />
                                5: Die in Frage kommenden Verfahren werden in korrekter Gewichtung umfassend und effektiv eingesetzt<br />
                                4: Ausgewählte Standardverfahren und Vorgehensweisen werden zuverlässig eingesetzt<br />
                                3: Eingesetzte Verfahren nur teilweise angemessen, Durchführung unzureichend<br />
                                2: Keine oder falsche Verfahren angewendet, keine oder unbrauchbare Durchführung<br />
                                1: Mutwillig falscher Einsatz von Verfahren mit resultierenden Schäden an Personal und/oder Geräten
                            </td>
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
                            <td class="small">6: Sämtliche Fragestellungen aus der PV beantwortet, Ergebnisse umfassend und kritisch analysiert, klare Schlussfolgerungen gezogen und Vorschläge für Umsetzung und Vertiefung erarbeitet<br />
                                5: Bis auf wenige Details alle Fragestellungen der PV beantwortet, vollständige Analyse der Ergebnisse, ausgerichtet auf deren Umsetzung<br />
                                4: Die wichtigsten Fragestellungen der PV beantwortet, Analyse beschränkt sich auf Vergleich mit Aufgabenstellung, keine weiterführenden Aussagen<br />
                                3: Fragestellungen der PV nur teilweise beantwortet, Ergebnisse unvollständig analysiert, teilweise falsche Schlussfolgerungen<br />
                                2: Fragestellungen der PV kaum beantwortet, nicht in der Lage, die Ergebnisse einzuordnen und zu bewerten<br />
                                1: Keine Bewertung der Ergebnisse durchgeführt und dokumentiert
                            </td>
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
                            <td class="small">6: Ziel übertroffen, zusätzlicher  unerwarteter Kundennutzen und Erkenntnisgewinn<br />
                                5: Ziel vollumfänglich erreicht<br />
                                4: Ziel im Wesentlichen erreicht, Einzelaspekte ergänzungsbedürftig<br />
                                3: Weniger als die Hälfte der Ziele erreicht<br />
                                2: Die meisten Ziele wurden nicht erreicht, Ergebnisse nicht brauchbar<br />
                                1: Kein Ziel wurde erreicht
                            </td>
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
                            <td class="small">6: Sehr geringer Betreuungsaufwand. Kritik und Anregungen von Betreuenden und Aussenstehenden werden kritisch hinterfragt, selbstständig weiterentwickelt und bestmöglichst umgesetzt.<br />
                                5: Geringer Betreuungsaufwand. Kritik und Anregungen von Betreuenden und Aussenstehenden werden umfänglich und gut umgesetzt.<br />
                                4: Durchschnittlicher Betreuungsaufwand. Kritik und Anregungen von Betreuenden werden weitgehend umgesetzt.<br />
                                3: Überdurchschnittlicher Betreungsaufwand. Anregungen werden nur teilweise umgesetzt oder es muss daran erinnert werden.<br />
                                2: Hoher Betreungsaufwand. Auch nach mehrmaliger Erinnerung nur einfachste Anregungen umgesetzt.<br />
                                1: Trotz sehr hohem Betreuungsaufwand wurden selbst einfache Anregungen nicht umgesetzt.
                            </td>
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
                            <td class="small">6: Bericht nachvollziehbar, sehr gute Leseführung, Inhalte logisch strukturiert, sehr umfassen informativ, formal sowie sprachlich und gestalterisch herausragend<br />
                                5: Bericht inhaltlich vollständig, gut nachvollziehbar, formal korrekt, sprachlich und gestalterisch ansprechend<br />
                                4: Alle wesentlichen Aspekte dokumentiert, wenig Leseführung, inhaltlich und sprachlich mehrheitlich verständlich<br />
                                3: Nur ein Teil der wesentlichen Aspekte dokumentiert, anstrengend zu lesen, Darstellung verbesserungswürdig<br />
                                2: Wesentliche Aspekte nicht dokumentiert, Bericht unstrukturiert, Darstellung mangelhaft, formal ungenügend<br />
                                1: Keine Dokumentation zum Abgabetermin vorhanden
                            </td>
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
                            <td class="small">6: Alle Fragen richtig und souverän beantwortet, Frage in Kontext eingeordnet, eine Venetzung der Fachinhalte ist klar ersichtlich
                                                    5: Alle Fragen korrekt und auf die Arbeit bezogen beantwortet
                                                    4: Fragen zögernd, aber im Wesentlichen korrekt beantwortet, manchmal Probleme bei spezifischen Details
                                                    3: Fragen teilweise falsch beantwortet, kein Detailwissen vohanden
                                                    2: Überwiegende Mehrzahl der Fragen nicht oder nicht korrekt beantwortet, die übrigen nur mangelhaft
                                                    1: Nicht auf Fragen eingegangen, keine richtige Antwort
                            </td>
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
                            <td class="small">6: Inhaltlich vollständiger und logisch aufgebauter Vortrag,  grafisch sehr gut gestaltet (unterstützend) und souverän vorgetragen, Fragen korrekt und umfassend beantwortet<br />
                                5: Vortrag inhaltlich vollständig, Aufbau und Präsentation logisch und ansprechend, Fragen korrekt beantwortet<br />
                                4: Im Vortrag relevante Inhalte behandelt, Einschränkungen in Aufbau (inkl. Folien) und Präsentationstechnik, Fragen gut beantwortet<br />
                                3: Vortrag mit inhaltlichen Lücken, Aufbau unklar / unlogisch, Fragen nur teilweise richtig beantwortet<br />
                                2: Vortrag inhaltlich unzureichend, Präsentation mangelhaft, Fragen nicht oder kaum beantwortet<br />
                                1: Vortrag mit falschem/viel zu wenig Inhalt, Präsentation sehr schwach, keine Fragen beantwortet
                            </td>
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
                            <td class="small">Eine gute Zusammenarbeit und Kommunikation beinhaltet die Einladung zu Sitzung mit Traktandenlisten, das Führen eines Sitzungsprotokolls mit Dokumentation der wichtigen Beschlüsse, das kommentierte Nachbessern von Zeitplänen sowie die Aktivierung von internem Wissen. Der/die Studierende setzte dies in folgendem Masse um:<br />
                                6: Äusserst selbständig<br />
                                5: Angemessen<br />
                                4: Ausreichend<br />
                                3: Auf Anregung<br />
                                2: Nach mehrmaligem Nachfragen<br />
                                1: Nicht/unzureichend
                            </td>
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
                            <td class="small">Eine gute Zusammenarbeit und Kommunikation beinhaltet die Einladung zu Sitzung mit Traktandenlisten, das Führen eines Sitzungsprotokolls mit Dokumentation der wichtigen Beschlüsse, das kommentierte Nachbessern von Zeitplänen sowie die Aktivierung von internem Wissen. Der/die Studierende setzte dies in folgendem Masse um:<br />
                                6: Äusserst selbständig<br />
                                5: Angemessen<br />
                                4: Ausreichend<br />
                                3: Auf Anregung<br />
                                2: Nach mehrmaligem Nachfragen<br />
                                1: Nicht/unzureichend
                            </td>
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
                            <td class="small">6: Persönlicher Einsatz hervorragend, Arbeitsumfang ausserordentlich hoch<br />
                                5: Hoher persönlicher Einsatz, überdurchschnittlicher Arbeitsumfang<br />
                                4: Persönlicher Einsatz gerade noch ausreichend, Arbeitsumfang durchschnittlich<br />
                                3: Unzureichender persönlicher Einsatz, Arbeitsumfang unterdurchschnittlich<br />
                                2: Nicht motiviert, Einsatz mangelhaft, Arbeitsumfang unzureichend<br />
                                1: Demotiviert / destruktiv, Einsatz nicht vorhanden, kaum Arbeiten geleistet
                            </td>
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
                            <td class="small">10: Mit der Thematik noch nie Kontakt gehabt<br />
                                5: Thematik bekannt (z.B. durch Unterricht)<br />
                                0: Mit Thema vertraut (z.B. durch Semesterarbeit)
                            </td>
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
                            <td class="small">10: Ausserordentlich hoch<br />
                                5: Überdurchschnittlich<br />
                                0: Standard
                            </td>
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
                            <td class="small">10: Ausserordentlich schwierig<br />
                                5: Schwierig<br />
                                0: Standard
                            </td>
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
                            <td class="small">Absolute Korrektur;<br />
                                Anwendung z. B. bei mehreren Partnern mit unterschiedlichen Schwerpunkten; komplexer Datenanalyse; neuen, noch nicht erprobten Tools u.Ä.
                            </td>
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
