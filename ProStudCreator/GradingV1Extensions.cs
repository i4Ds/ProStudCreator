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
