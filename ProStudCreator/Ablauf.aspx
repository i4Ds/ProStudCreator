<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Ablauf.aspx.cs" Inherits="ProStudCreator.Ablauf" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well usernSettings">
        <h3>Durchführung eines Projektes</h3>
        <ol>
            <li>Passe die grau hinterlegte Gewichtung im <a href="Content/Bewertungsbogen Projekte 5 und 6.xlsx" class="xls">Bewertungsbogen</a> an.
            </li>
            <li>Schreibe bei Semesterbeginn den Studierenden und Verlange die Organisation eines Kickoff-Meetings.</li>
            <li>Informiere die Studierenden über folgende Punkte beim Kickoff-Meeting:
                <ul>
                    <li>Information, dass pro Student/-in eine individuelle Note festgelegt wird.</li>
                    <li>Bitte kommuniziere, wie Du informiert werden möchtest (z.B. jede Woche ein kurzes Treffen).</li>
                    <li>Terminplan besprechen (Abgabetermin, Ausstellung, Projektwoche, Präsentation/Verteidigung, z.B. Inhaltsverzeichnis der Doku nach einem Drittel, ...)</li>
                    <li>Ein Hinweis, dass auf dem Netzwerkshare (\\fsemu18.edu.ds.fhnw.ch\e_18_data11$\E1811_Info\E1811_Info_I\Projektschiene) Unterlagen zu finden sind.</li>
                    <li>Optional: <a href="Content/P5_P6_Guide_20180914.pdf" class="pdf">Guide von Marco Soldati für IP5 und IP6</a>, <a href="Content/Leitfaden_Berichte_4.01.pdf" class="pdf">Offizieller Leitfaden für Berichte</a>, <a href="Content/Vorlage_Bericht_17.dotx" class="doc">Offizielle Vorlage für Berichte</a></li>
                    <li>Besprich den <a href="Content/Bewertungsbogen Projekte 5 und 6.xlsx" class="xls">Bewertungsbogen</a> mit den Studierenden</li>
                    <li>Die Studierenden müssen in den ersten Wochen eine Projektvereinbarung verfassen. Eine Vorlage dafür
                        existiert nicht. Die Projektvereinbarung umfasst 1-2 Seiten und enthält folgende Punkte:
                        <ul>
                            <li>Wiedergabe der Ausgangslage und Aufgabenstellung in eigenen Worten</li>
                            <li>Vorgehen mit grobem Zeitplan und Meilensteinen</li>
                            <li>Umgang mit Projektrisiken, falls nötig</li>
                        </ul>
                    </li>
                </ul>
            </li>
            <li>Falls sich eine ungenügende Note abzeichnet, sind für den Rekursfall <a href="FAQ#rekurs">einige Dinge</a> zu beachten. Melde Dich möglichst früh bei uns.</li>
            <li>Titeländerungen können bis <%: (ProStudCreator.Global.AllowTitleChangesBeforeSubmission.Days/7) %> Wochen vor Abgabe auf der jeweiligen Projekt-Infoseite vorgenommen werden.</li>
            <li>Die Studierenden erstellen das obligatorische Websummary. Auf dem Netzwerkshare gibts dazu einen Leitfaden (\\fsemu18.edu.ds.fhnw.ch\e_18_data11$\E1811_Info\E1811_Info_I\Projektschiene).</li>
            <li>Nur für Projekte, die im Herbst abschliessen: Die Studierenden stellen Ihr IP6 an der Ausstellung mit einem <a href="Content/Poster_TemplateI_150923.pptx" class="ppt">Poster</a> vor. Poster
                können vom Empfang in A0-Grösse gedruckt werden.</li>
            <li><ul>
                    <li>IP5: Lasse die Studierenden ein Schlusspräsentation organisieren (Raum, Termin). Die Präsentation im IP5 wird verlangt, da sie der Vorbereitung auf die Schlusspräsentation und Verteidigung der Bachelorarbeit dient.</li>
                    <li>IP6: Die Schlusspräsentation (Raum, Termin, Experte/-in) wird von uns organisiert. Details sind auf der Projekt-Infoseite ersichtlich. Die Studierenden schicken ihre Arbeit an den Experten, in elektronischer oder Papier-Form, je nach Wunsch des Experten.</li>
                </ul>
            </li>
            <li>Fülle nach der Projektabgabe den <a href="Content/Bewertungsbogen Projekte 5 und 6.xlsx" class="xls">Bewertungsbogen</a> aus.</li>
            <li>Lade Kollegen (_D_A18_47_Mitarbeitende_IIT@adm.mx.ds.fhnw.ch; _D_A18_41_Mitarbeitende_I4DS@adm.mx.ds.fhnw.ch; _D_A18_39_Mitarbeitende_IMVS@adm.mx.ds.fhnw.ch) und andere interessierte Gasthörer zur Schlusspräsentation ein.</li>
            <li>Schlusspräsentation mit ca. 30 Minuten Präsentation, und ca. 30 Minuten Fragen von Betreuern, Experten (IP6) und interessierten Gästen.</li>
            <li>Notenbesprechung mit den Experten (IP6), ggf. Anpassung der Bewertung. Pro Student/-in soll eine <i>individuelle</i> Note festgelegt werden. Noten >5.8 müssen an die SGL gemeldet werden.</li>
            <li>Auf der Projekt-Infoseite die Note, Verrechenbarkeit und Sprache des Projekts eintragen. Dort auch alle Projektartefakte (Doku, Code, ...) hochladen.</li>
            <li><b>NEU: Zum Schluss das Projekt im ProStud abschliessen.</b></li>
            <li>Den Bewertungsbogen den Studierenden schicken, aber ohne Kommentare (Spalte G).</li>
            <li>NEU: Du machst mit den Studierenden eine Schlusssitzung. An der Sitzung soll die Bewertung besprochen und die Arbeit reflektiert werden, um den Lerneffekt zu erhöhen.</li>
        </ol>
    </div>

    <div class="well usernSettings non-selectable">
        <h3>Einreichen eines eigenen Projekts</h3>
        <ol>
            <li>Du erarbeitest einen Projektbeschrieb.</li>
            <li>Du reichst den Projektbeschrieb auf dieser Plattform ein.</li>
            <li>Der Projektbeschrieb wird geprüft.</li>
        </ol>
        <p>
            Für alle Projekte gelten dieselben Anforderungen (siehe <a href="FAQ.aspx">FAQ</a>).
        </p>
        <h3>Projekte von externen Auftraggebern</h3>
        <p>
            Hinweis: Bitte keine Projekte direkt von Studierenden annehmen oder gar Versprechungen machen.
        </p>
        <img src="Content/Prozess.PNG" />
        <ol>
            <li>Ein externer Auftraggeber (nie ein Student selbst) schickt <a class="pdf" href="Content/Externe_Projekteingabe.pdf">dieses Formular</a> an Markus Oehninger.</li>
            <li>Markus verteilt das Projekt ans I4DS, IMVS und IIT.</li>
            <li>Dominik Gruntz (IMVS), André Csillaghy (I4DS) oder Marcel Gygli (IIT) verteilen das Projekt an passende Betreuer.</li>
            <li>Die Betreuer erarbeiten einen Projektbeschrieb, zusammen mit dem externen Auftraggeber.</li>
            <li>Der fertige Projektbeschrieb wird über diese Plattform eingereicht und geprüft.</li>
        </ol>
    </div>
</asp:Content>
