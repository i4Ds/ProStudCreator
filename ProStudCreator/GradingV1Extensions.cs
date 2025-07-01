using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProStudCreator
{
    public partial class GradingV1
    {
        public static string AStrategySchema =
            "6: Innovatives Lösungskonzept, übertrifft die Erwartungen klar, effektive kreative Strategie\n" +
            "5: Lösungskonzept und Strategie umfassend, klar, präzise und effektiv\n" +
            "4: Lösungskonzept und Strategie zielführend, Standardvorgehen\n" +
            "3: Lösungskonzept nur teilweise nachvollziehbar, unklare Strategie\n" +
            "2: Lösungskonzept nicht nachvollziehbar, keine Strategie\n" +
            "1: Kein Lösungskonzept vorhanden";

        public static string AProjectSummaryContentsSchema =
            "6: Abgabe termingerecht, überdurchschnittliche/unerwartete Analyse der Aufgabenstellung, Erfassung sämtlicher Einzelfragen im thematischen Zusammenhang, wesentlicher eigener inhaltlicher Beitrag zur Umsetzung\n" +
            "5: Abgabe termingerecht; Vollständige Durchdringung der Aufgabenstellung, gesamtheitlicher Lösungsansatz und eigenständige kreative Umsetzung\n" +
            "4: Abgabe Termingerecht, Aufgabenstellung eins zu eins umgesetzt, Abgrenzung von Teilaufgaben\n" +
            "3: Abgabe termingerecht, Umsetzung der Aufgabenstellung nur teilweise erkennbar, ungenügende Analyse, unpassender Lösungsatz\n" +
            "2: Umsetzung der Aufgabenstellung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin\n" +
            "1: Keine Projektvereinbarung bis 2 Wochen nach Termin";

        public static string AProjectSummaryPlanningSchema =
            "6: Abgabe termingerecht, überdurchschnittliche und detaillierte Projektplanung, Arbeitsumfang realistisch abgeschätzt und abgebildet, genügend sinnvolle und klar messbare Meilensteine\n" +
            "5: Abgabe termingerecht, umfängliche Projektplanung, Arbeitsumfang realitätsnah abgeschätzt und abgebildet, mit messabren Meilensteinen\n" +
            "4: Abgabe termingerecht, Projektplanung enthält die wesentlichen Arbeitsschritte, Arbeitsumfang weitgehend realitätsnah abgeschätzt, teilweise messbare Meilensteine\n" +
            "3: Abgabe termingerecht, Projektplanung enthält nicht alle Arbeitsschritte, Arbeitsumfang wird teilweise deutlich über-/unterschätzt, zu wenig oder nicht messbare Meilensteine\n" +
            "2: Umsetzung der Projektplanung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin\n" +
            "1: Keine Projektvereinbarung bis 2 Wochen nach Termin";

        public static string BTheoreticalWorkSchema =
            "6: Neuartiger Lösungsansatz, der die üblichen theoretischen Grundlagenkenntnisse von Studierenden klar übertrifft, sehr gut und umfassend umgesetzt\n" +
            "5: Problem umfassend in allen Aspekten gelöst\n" +
            "4: Problem mit bekannten Konzepten und Tools in seinen wesentlichen Aspekten gelöst\n" +
            "3: Unzureichender theoretischer Hintergrund, teilweise falsche Argumentation\n" +
            "2: Theoretischer Hintergrund nicht ersichtlich, keine logische Argumentation\n" +
            "1: Keine Bearbeitung eines theoretischen Hintergrundes";

        public static string BPracticalWorkSchema =
            "6: Äusserst umfassender und effizienter Einsatz der verfügbaren Mittel und Verfahren, Entwicklung problemspezifischer neuer Methoden\n" +
            "5: Die in Frage kommenden Verfahren werden in korrekter Gewichtung umfassend und effektiv eingesetzt\n" +
            "4: Ausgewählte Standardverfahren und Vorgehensweisen werden zuverlässig eingesetzt\n" +
            "3: Eingesetzte Verfahren nur teilweise angemessen, Durchführung unzureichend\n" +
            "2: Keine oder falsche Verfahren angewendet, keine oder unbrauchbare Durchführung\n" +
            "1: Mutwillig falscher Einsatz von Verfahren mit resultierenden Schäden an Personal und/oder Geräten";

        public static string BEvaluationSchema =
            "6: Sämtliche Fragestellungen aus der PV beantwortet, Ergebnisse umfassend und kritisch analysiert, klare Schlussfolgerungen gezogen und Vorschläge für Umsetzung und Vertiefung erarbeitet\n" +
            "5: Bis auf wenige Details alle Fragestellungen der PV beantwortet, vollständige Analyse der Ergebnisse, ausgerichtet auf deren Umsetzung\n" +
            "4: Die wichtigsten Fragestellungen der PV beantwortet, Analyse beschränkt sich auf Vergleich mit Aufgabenstellung, keine weiterführenden Aussagen\n" +
            "3: Fragestellungen der PV nur teilweise beantwortet, Ergebnisse unvollständig analysiert, teilweise falsche Schlussfolgerungen\n" +
            "2: Fragestellungen der PV kaum beantwortet, nicht in der Lage, die Ergebnisse einzuordnen und zu bewerten\n" +
            "1: Keine Bewertung der Ergebnisse durchgeführt und dokumentiert";

        public static string BResultsSchema =
            "6: Ziel übertroffen, zusätzlicher  unerwarteter Kundennutzen und Erkenntnisgewinn\n" +
            "5: Ziel vollumfänglich erreicht\n" +
            "4: Ziel im Wesentlichen erreicht, Einzelaspekte ergänzungsbedürftig\n" +
            "3: Weniger als die Hälfte der Ziele erreicht\n" +
            "2: Die meisten Ziele wurden nicht erreicht, Ergebnisse nicht brauchbar\n" +
            "1: Kein Ziel wurde erreicht";

        public static string BAutonomySchema =
            "6: Sehr geringer Betreuungsaufwand. Kritik und Anregungen von Betreuenden und Aussenstehenden werden kritisch hinterfragt, selbstständig weiterentwickelt und bestmöglichst umgesetzt.\n" +
            "5: Geringer Betreuungsaufwand. Kritik und Anregungen von Betreuenden und Aussenstehenden werden umfänglich und gut umgesetzt.\n" +
            "4: Durchschnittlicher Betreuungsaufwand. Kritik und Anregungen von Betreuenden werden weitgehend umgesetzt.\n" +
            "3: Überdurchschnittlicher Betreungsaufwand. Anregungen werden nur teilweise umgesetzt oder es muss daran erinnert werden.\n" +
            "2: Hoher Betreungsaufwand. Auch nach mehrmaliger Erinnerung nur einfachste Anregungen umgesetzt.\n" +
            "1: Trotz sehr hohem Betreuungsaufwand wurden selbst einfache Anregungen nicht umgesetzt.";

        public static string CDocumentationSchema =
            "6: Bericht nachvollziehbar, sehr gute Leseführung, Inhalte logisch strukturiert, sehr umfassen informativ, formal sowie sprachlich und gestalterisch herausragend\n" +
            "5: Bericht inhaltlich vollständig, gut nachvollziehbar, formal korrekt, sprachlich und gestalterisch ansprechend\n" +
            "4: Alle wesentlichen Aspekte dokumentiert, wenig Leseführung, inhaltlich und sprachlich mehrheitlich verständlich\n" +
            "3: Nur ein Teil der wesentlichen Aspekte dokumentiert, anstrengend zu lesen, Darstellung verbesserungswürdig\n" +
            "2: Wesentliche Aspekte nicht dokumentiert, Bericht unstrukturiert, Darstellung mangelhaft, formal ungenügend\n" +
            "1: Keine Dokumentation zum Abgabetermin vorhanden";

        public static string CDefenseSchema =
            "6: Alle Fragen richtig und souverän beantwortet, Frage in Kontext eingeordnet, eine Venetzung der Fachinhalte ist klar ersichtlich\n" +
            "5: Alle Fragen korrekt und auf die Arbeit bezogen beantwortet\n" +
            "4: Fragen zögernd, aber im Wesentlichen korrekt beantwortet, manchmal Probleme bei spezifischen Details\n" +
            "3: Fragen teilweise falsch beantwortet, kein Detailwissen vohanden\n" +
            "2: Überwiegende Mehrzahl der Fragen nicht oder nicht korrekt beantwortet, die übrigen nur mangelhaft\n" +
            "1: Nicht auf Fragen eingegangen, keine richtige Antwort";

        public static string CPresentationsSchema =
            "6: Inhaltlich vollständiger und logisch aufgebauter Vortrag,  grafisch sehr gut gestaltet (unterstützend) und souverän vorgetragen, Fragen korrekt und umfassend beantwortet\n" +
            "5: Vortrag inhaltlich vollständig, Aufbau und Präsentation logisch und ansprechend, Fragen korrekt beantwortet\n" +
            "4: Im Vortrag relevante Inhalte behandelt, Einschränkungen in Aufbau (inkl. Folien) und Präsentationstechnik, Fragen gut beantwortet\n" +
            "3: Vortrag mit inhaltlichen Lücken, Aufbau unklar / unlogisch, Fragen nur teilweise richtig beantwortet\n" +
            "2: Vortrag inhaltlich unzureichend, Präsentation mangelhaft, Fragen nicht oder kaum beantwortet\n" +
            "1: Vortrag mit falschem/viel zu wenig Inhalt, Präsentation sehr schwach, keine Fragen beantwortet";

        public static string DCollaborationInternalSchema =
            "Eine gute Zusammenarbeit und Kommunikation beinhaltet die Einladung zu Sitzung mit Traktandenlisten, das Führen eines Sitzungsprotokolls mit Dokumentation der wichtigen Beschlüsse, das kommentierte Nachbessern von Zeitplänen sowie die Aktivierung von internem Wissen. Der/die Studierende setzte dies in folgendem Masse um:\n" +
            "6: Äusserst selbständig\n" +
            "5: Angemessen\n" +
            "4: Ausreichend\n" +
            "3: Auf Anregung\n" +
            "2: Nach mehrmaligem Nachfragen\n" +
            "1: Nicht/unzureichend";

        public static string DCollaborationExternalSchema =
            "Eine gute Zusammenarbeit und Kommunikation beinhaltet die Einladung zu Sitzung mit Traktandenlisten, das Führen eines Sitzungsprotokolls mit Dokumentation der wichtigen Beschlüsse, das kommentierte Nachbessern von Zeitplänen sowie die Aktivierung von internem Wissen. Der/die Studierende setzte dies in folgendem Masse um:\n" +
            "6: Äusserst selbständig\n" +
            "5: Angemessen\n" +
            "4: Ausreichend\n" +
            "3: Auf Anregung\n" +
            "2: Nach mehrmaligem Nachfragen\n" +
            "1: Nicht/unzureichend";

        public static string DMotivationSchema =
            "6: Persönlicher Einsatz hervorragend, Arbeitsumfang ausserordentlich hoch\n" +
            "5: Hoher persönlicher Einsatz, überdurchschnittlicher Arbeitsumfang\n" +
            "4: Persönlicher Einsatz gerade noch ausreichend, Arbeitsumfang durchschnittlich\n" +
            "3: Unzureichender persönlicher Einsatz, Arbeitsumfang unterdurchschnittlich\n" +
            "2: Nicht motiviert, Einsatz mangelhaft, Arbeitsumfang unzureichend\n" +
            "1: Demotiviert / destruktiv, Einsatz nicht vorhanden, kaum Arbeiten geleistet";

        public static string ENewTopicSchema =
            "10: Mit der Thematik noch nie Kontakt gehabt\n" +
            "5: Thematik bekannt (z.B. durch Unterricht)\n" +
            "0: Mit Thema vertraut (z.B. durch Semesterarbeit)";

        public static string EDifficultySchema =
            "10: Ausserordentlich hoch\n" +
            "5: Überdurchschnittlich\n" +
            "0: Standard";

        public static string EEnvironmentSchema =
            "10: Ausserordentlich schwierig\n" +
            "5: Schwierig\n" +
            "0: Standard";

        public static string EBonusSchema =
            "Absolute Korrektur;\n" +
            "Anwendung z. B. bei mehreren Partnern mit unterschiedlichen Schwerpunkten; komplexer Datenanalyse; neuen, noch nicht erprobten Tools u.Ä.";

        private double ComputeBlockAGrade()
        {
            var blockAGrade = (AStrategy * AStrategyWeight +
                             AProjectSummaryContents * AProjectSummaryContentsWeight +
                             AProjectSummaryPlanning * AProjectSummaryPlanningWeight) /
                            (AStrategyWeight + AProjectSummaryContentsWeight + AProjectSummaryPlanningWeight);

            return double.IsNaN(blockAGrade) ? 0 : blockAGrade;
        }

        private double ComputeBlockBGrade()
        {
            var blockBGrade = (BTheoreticalWork * BTheoreticalWorkWeight +
                             BPracticalWork * BPracticalWorkWeight +
                             BEvaluation * BEvaluationWeight +
                             BResults * BResultsWeight +
                             BAutonomy * BAutonomyWeight) /
                            (BTheoreticalWorkWeight + BPracticalWorkWeight +
                             BEvaluationWeight + BResultsWeight + BAutonomyWeight);

            return double.IsNaN(blockBGrade) ? 0 : blockBGrade;
        }

        private double ComputeBlockCGrade()
        {
            var blockCGrade = (CDocumentation * CDocumentationWeight +
                             CDefense * CDefenseWeight +
                             CPresentations * CPresentationsWeight) /
                            (CDocumentationWeight + CDefenseWeight + CPresentationsWeight);

            return double.IsNaN(blockCGrade) ? 0 : blockCGrade;
        }

        private double ComputeBlockDGrade()
        {
            var blockDGrade = (DCollaborationInternal * DCollaborationInternalWeight +
                             DCollaborationExternal * DCollaborationExternalWeight +
                             DMotivation * DMotivationWeight) /
                            (DCollaborationInternalWeight + DCollaborationExternalWeight + DMotivationWeight);

            return double.IsNaN(blockDGrade) ? 0 : blockDGrade;
        }

        public double ComputePreBonus()
        {
            var blockAGrade = ComputeBlockAGrade();
            var blockBGrade = ComputeBlockBGrade();
            var blockCGrade = ComputeBlockCGrade();
            var blockDGrade = ComputeBlockDGrade();

            return (blockAGrade * 1 + blockBGrade * 4 + blockCGrade * 2 + blockDGrade * 1) / 8;
        }

        public double ComputeFinalGrade()
        {
            return Math.Min(6.0, ComputePreBonus() + EBonus);
        }

        public static GradingV1 CreateDefault(bool _isP6, bool _externalCustomer)
        {
            return new GradingV1
            {
                CriticalAcclaim = "",
                AStrategy = 4,
                AStrategyWeight = 2,
                AStrategyComment = "",
                AProjectSummaryContents = 4,
                AProjectSummaryContentsWeight = 1,
                AProjectSummaryContentsComment = "",
                AProjectSummaryPlanning = 4,
                AProjectSummaryPlanningWeight = 1,
                AProjectSummaryPlanningComment = "",
                BTheoreticalWork = 4,
                BTheoreticalWorkWeight = 2,
                BTheoreticalWorkComment = "",
                BPracticalWork = 4,
                BPracticalWorkWeight = 2,
                BPracticalWorkComment = "",
                BEvaluation = 4,
                BEvaluationWeight = 2,
                BEvaluationComment = "",
                BResults = 4,
                BResultsWeight = 1,
                BResultsComment = "",
                BAutonomy = 4,
                BAutonomyWeight = 1,
                BAutonomyComment = "",
                CDocumentation = 4,
                CDocumentationWeight = 1,
                CDocumentationComment = "",
                CDefense = 4,
                CDefenseWeight = _isP6 ? 1 : 0,
                CDefenseComment = "",
                CPresentations = 4,
                CPresentationsWeight = 1,
                CPresentationsComment = "",
                DCollaborationInternal = 4,
                DCollaborationInternalWeight = 1,
                DCollaborationInternalComment = "",
                DCollaborationExternal = 4,
                DCollaborationExternalWeight = _externalCustomer ? 1 : 0,
                DCollaborationExternalComment = "",
                DMotivation = 4,
                DMotivationWeight = 1,
                DMotivationComment = "",
                ENewTopic = 5,
                ENewTopicComment = "",
                EDifficulty = 0,
                EDifficultyComment = "",
                EEnvironment = 0,
                EEnvironmentComment = "",
                EBonus = 0
            };
        }
    }
}
