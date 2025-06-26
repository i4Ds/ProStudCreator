using Microsoft.Ajax.Utilities;
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
    public static class GradingV1Extensions
    {

        private static double ComputeBlockAGrade(this GradingV1 _g)
        {
            var blockAGrade = (_g.AStrategy * _g.AStrategyWeight +
                             _g.AProjectSummaryContents * _g.AProjectSummaryContentsWeight +
                             _g.AProjectSummaryPlanning * _g.AProjectSummaryPlanningWeight) /
                            (_g.AStrategyWeight + _g.AProjectSummaryContentsWeight + _g.AProjectSummaryPlanningWeight);

            return double.IsNaN(blockAGrade) ? 0 : blockAGrade;
        }

        private static double ComputeBlockBGrade(this GradingV1 _g)
        {
            var blockBGrade = (_g.BTheoreticalWork * _g.BTheoreticalWorkWeight +
                             _g.BPracticalWork * _g.BPracticalWorkWeight +
                             _g.BEvaluation * _g.BEvaluationWeight +
                             _g.BResults * _g.BResultsWeight +
                             _g.BAutonomy * _g.BAutonomyWeight) /
                            (_g.BTheoreticalWorkWeight + _g.BPracticalWorkWeight +
                             _g.BEvaluationWeight + _g.BResultsWeight + _g.BAutonomyWeight);

            return double.IsNaN(blockBGrade) ? 0 : blockBGrade;
        }

        private static double ComputeBlockCGrade(this GradingV1 _g)
        {
            var blockCGrade = (_g.CDocumentation * _g.CDocumentationWeight +
                             _g.CDefense * _g.CDefenseWeight +
                             _g.CPresentations * _g.CPresentationsWeight) /
                            (_g.CDocumentationWeight + _g.CDefenseWeight + _g.CPresentationsWeight);

            return double.IsNaN(blockCGrade) ? 0 : blockCGrade;
        }

        private static double ComputeBlockDGrade(this GradingV1 _g)
        {
            var blockDGrade = (_g.DCollaborationInternal * _g.DCollaborationInternalWeight +
                             _g.DCollaborationExternal * _g.DCollaborationExternalWeight +
                             _g.DMotivation * _g.DMotivationWeight) /
                            (_g.DCollaborationInternalWeight + _g.DCollaborationExternalWeight + _g.DMotivationWeight);

            return double.IsNaN(blockDGrade) ? 0 : blockDGrade;
        }

        public static double ComputePreBonus(this GradingV1 _g)
        {
            var blockAGrade = _g.ComputeBlockAGrade();
            var blockBGrade = _g.ComputeBlockBGrade();
            var blockCGrade = _g.ComputeBlockCGrade();
            var blockDGrade = _g.ComputeBlockDGrade();

            return (blockAGrade * 1 + blockBGrade * 4 + blockCGrade * 2 + blockDGrade * 1) / 8;
        }

        public static double ComputeFinalGrade(this GradingV1 _g)
        {
            return Math.Min(6.0, _g.ComputePreBonus() + _g.EBonus);
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
