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
            "5: Abgabe termingerecht, umfängliche Projektplanung, Arbeitsumfang realitätsnah abgeschätzt und abgebildet, mit messabren Meilensteinen\n"+
            "4: Abgabe termingerecht, Projektplanung enthält die wesentlichen Arbeitsschritte, Arbeitsumfang weitgehend realitätsnah abgeschätzt, teilweise messbare Meilensteine\n"+
            "3: Abgabe termingerecht, Projektplanung enthält nicht alle Arbeitsschritte, Arbeitsumfang wird teilweise deutlich über-/unterschätzt, zu wenig oder nicht messbare Meilensteine\n"+
            "2: Umsetzung der Projektplanung nicht erkennbar und/oder Abgabe der Projektvereinbarung 0-2 Wochen nach Termin\n"+
            "1: Keine Projektvereinbarung bis 2 Wochen nach Termin";

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
