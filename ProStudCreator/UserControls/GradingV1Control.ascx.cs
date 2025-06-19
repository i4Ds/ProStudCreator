using System;
using System.Globalization;
using System.Web.UI;

namespace ProStudCreator.UserControls
{
    public partial class GradingV1Control : UserControl 
    {
        private GradingV1 _grading;

        public GradingV1 Grading
        {
            get => _grading;
            set
            {
                _grading = value;
                UpdateFormFields();
            }
        }

        private string _studentName;
        public string StudentName
        {
            get => _studentName;
            set
            {
                _studentName = value;
                lblStudent.Text = value;
            }
        }

        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                lblProjectTitle.Text = _project.Name;

                if(_project.Advisor2 == null)
                    lblAdvisor.Text = _project.Advisor1.Name;
                else
                    lblAdvisor.Text = $"{_project.Advisor1.Name}, {_project.Advisor2.Name}";

                lblExpert.Text = _project.Expert?.Name ?? "?";
            }
        }

        public void Initialize()
        {
        }

        private void UpdateFormFields()
        {
            txtCriticalAcclaim.Text = Grading.CriticalAcclaim;

            // Block A fields
            txtAStrategyWeight.Text = Grading.AStrategyWeight.ToString("0.0");
            txtAStrategy.Text = Grading.AStrategy.ToString("0.0");
            txtAStrategyComment.Text = Grading.AStrategyComment;

            txtAProjectSummaryContentsWeight.Text = Grading.AProjectSummaryContentsWeight.ToString("0.0");
            txtAProjectSummaryContents.Text = Grading.AProjectSummaryContents.ToString("0.0");
            txtAProjectSummaryContentsComment.Text = Grading.AProjectSummaryContentsComment;

            txtAProjectSummaryPlanningWeight.Text = Grading.AProjectSummaryPlanningWeight.ToString("0.0");
            txtAProjectSummaryPlanning.Text = Grading.AProjectSummaryPlanning.ToString("0.0");
            txtAProjectSummaryPlanningComment.Text = Grading.AProjectSummaryPlanningComment;

            // Block B fields
            txtBTheoreticalWorkWeight.Text = Grading.BTheoreticalWorkWeight.ToString("0.0");
            txtBTheoreticalWork.Text = Grading.BTheoreticalWork.ToString("0.0");
            txtBTheoreticalWorkComment.Text = Grading.BTheoreticalWorkComment;

            txtBPracticalWorkWeight.Text = Grading.BPracticalWorkWeight.ToString("0.0");
            txtBPracticalWork.Text = Grading.BPracticalWork.ToString("0.0");
            txtBPracticalWorkComment.Text = Grading.BPracticalWorkComment;

            txtBEvaluationWeight.Text = Grading.BEvaluationWeight.ToString("0.0");
            txtBEvaluation.Text = Grading.BEvaluation.ToString("0.0");
            txtBEvaluationComment.Text = Grading.BEvaluationComment;

            txtBResultsWeight.Text = Grading.BResultsWeight.ToString("0.0");
            txtBResults.Text = Grading.BResults.ToString("0.0");
            txtBResultsComment.Text = Grading.BResultsComment;

            txtBAutonomyWeight.Text = Grading.BAutonomyWeight.ToString("0.0");
            txtBAutonomy.Text = Grading.BAutonomy.ToString("0.0");
            txtBAutonomyComment.Text = Grading.BAutonomyComment;

            // Block C fields
            txtCDocumentationWeight.Text = Grading.CDocumentationWeight.ToString("0.0");
            txtCDocumentation.Text = Grading.CDocumentation.ToString("0.0");
            txtCDocumentationComment.Text = Grading.CDocumentationComment;

            txtCDefenseWeight.Text = Grading.CDefenseWeight.ToString("0.0");
            txtCDefense.Text = Grading.CDefense.ToString("0.0");
            txtCDefenseComment.Text = Grading.CDefenseComment;

            txtCPresentationsWeight.Text = Grading.CPresentationsWeight.ToString("0.0");
            txtCPresentations.Text = Grading.CPresentations.ToString("0.0");
            txtCPresentationsComment.Text = Grading.CPresentationsComment;

            // Block D fields
            txtDCollaborationInternalWeight.Text = Grading.DCollaborationInternalWeight.ToString("0.0");
            txtDCollaborationInternal.Text = Grading.DCollaborationInternal.ToString("0.0");
            txtDCollaborationInternalComment.Text = Grading.DCollaborationInternalComment;

            txtDCollaborationExternalWeight.Text = Grading.DCollaborationExternalWeight.ToString("0.0");
            txtDCollaborationExternal.Text = Grading.DCollaborationExternal.ToString("0.0");
            txtDCollaborationExternalComment.Text = Grading.DCollaborationExternalComment;

            txtDMotivationWeight.Text = Grading.DMotivationWeight.ToString("0.0");
            txtDMotivation.Text = Grading.DMotivation.ToString("0.0");
            txtDMotivationComment.Text = Grading.DMotivationComment;

            // Bonus fields
            txtENewTopic.Text = Grading.ENewTopic.ToString();
            txtENewTopicComment.Text = Grading.ENewTopicComment;

            txtEDifficulty.Text = Grading.EDifficulty.ToString();
            txtEDifficultyComment.Text = Grading.EDifficultyComment;

            txtEEnvironment.Text = Grading.EEnvironment.ToString();
            txtEEnvironmentComment.Text = Grading.EEnvironmentComment;

            txtEBonus.Text = Grading.EBonus.ToString("0.0");

            // Update grade displays
            UpdateCalculatedGrades();
        }

        protected void OnGradeChanged(object sender, EventArgs e)
        {
            UpdateCalculatedGrades();
        }

        public void SaveFormFields()
        {
            Grading.CriticalAcclaim = txtCriticalAcclaim.Text;

            // Block A fields
            Grading.AStrategyWeight = double.Parse(txtAStrategyWeight.Text, CultureInfo.InvariantCulture);
            Grading.AStrategy = double.Parse(txtAStrategy.Text, CultureInfo.InvariantCulture);
            Grading.AStrategyComment = txtAStrategyComment.Text;

            Grading.AProjectSummaryContentsWeight = double.Parse(txtAProjectSummaryContentsWeight.Text, CultureInfo.InvariantCulture);
            Grading.AProjectSummaryContents = double.Parse(txtAProjectSummaryContents.Text, CultureInfo.InvariantCulture);
            Grading.AProjectSummaryContentsComment = txtAProjectSummaryContentsComment.Text;

            Grading.AProjectSummaryPlanningWeight = double.Parse(txtAProjectSummaryPlanningWeight.Text, CultureInfo.InvariantCulture);
            Grading.AProjectSummaryPlanning = double.Parse(txtAProjectSummaryPlanning.Text, CultureInfo.InvariantCulture);
            Grading.AProjectSummaryPlanningComment = txtAProjectSummaryPlanningComment.Text;

            // Block B fields
            Grading.BTheoreticalWorkWeight = double.Parse(txtBTheoreticalWorkWeight.Text, CultureInfo.InvariantCulture);
            Grading.BTheoreticalWork = double.Parse(txtBTheoreticalWork.Text, CultureInfo.InvariantCulture);
            Grading.BTheoreticalWorkComment = txtBTheoreticalWorkComment.Text;

            Grading.BPracticalWorkWeight = double.Parse(txtBPracticalWorkWeight.Text, CultureInfo.InvariantCulture);
            Grading.BPracticalWork = double.Parse(txtBPracticalWork.Text, CultureInfo.InvariantCulture);
            Grading.BPracticalWorkComment = txtBPracticalWorkComment.Text;

            Grading.BEvaluationWeight = double.Parse(txtBEvaluationWeight.Text, CultureInfo.InvariantCulture);
            Grading.BEvaluation = double.Parse(txtBEvaluation.Text, CultureInfo.InvariantCulture);
            Grading.BEvaluationComment = txtBEvaluationComment.Text;

            Grading.BResultsWeight = double.Parse(txtBResultsWeight.Text, CultureInfo.InvariantCulture);
            Grading.BResults = double.Parse(txtBResults.Text, CultureInfo.InvariantCulture);
            Grading.BResultsComment = txtBResultsComment.Text;

            Grading.BAutonomyWeight = double.Parse(txtBAutonomyWeight.Text, CultureInfo.InvariantCulture);
            Grading.BAutonomy = double.Parse(txtBAutonomy.Text, CultureInfo.InvariantCulture);
            Grading.BAutonomyComment = txtBAutonomyComment.Text;

            // Block C fields
            Grading.CDocumentationWeight = double.Parse(txtCDocumentationWeight.Text, CultureInfo.InvariantCulture);
            Grading.CDocumentation = double.Parse(txtCDocumentation.Text, CultureInfo.InvariantCulture);
            Grading.CDocumentationComment = txtCDocumentationComment.Text;

            Grading.CDefenseWeight = double.Parse(txtCDefenseWeight.Text, CultureInfo.InvariantCulture);
            Grading.CDefense = double.Parse(txtCDefense.Text, CultureInfo.InvariantCulture);
            Grading.CDefenseComment = txtCDefenseComment.Text;

            Grading.CPresentationsWeight = double.Parse(txtCPresentationsWeight.Text, CultureInfo.InvariantCulture);
            Grading.CPresentations = double.Parse(txtCPresentations.Text, CultureInfo.InvariantCulture);
            Grading.CPresentationsComment = txtCPresentationsComment.Text;

            // Block D fields
            Grading.DCollaborationInternalWeight = double.Parse(txtDCollaborationInternalWeight.Text, CultureInfo.InvariantCulture);
            Grading.DCollaborationInternal = double.Parse(txtDCollaborationInternal.Text, CultureInfo.InvariantCulture);
            Grading.DCollaborationInternalComment = txtDCollaborationInternalComment.Text;

            Grading.DCollaborationExternalWeight = double.Parse(txtDCollaborationExternalWeight.Text, CultureInfo.InvariantCulture);
            Grading.DCollaborationExternal = double.Parse(txtDCollaborationExternal.Text, CultureInfo.InvariantCulture);
            Grading.DCollaborationExternalComment = txtDCollaborationExternalComment.Text;

            Grading.DMotivationWeight = double.Parse(txtDMotivationWeight.Text, CultureInfo.InvariantCulture);
            Grading.DMotivation = double.Parse(txtDMotivation.Text, CultureInfo.InvariantCulture);
            Grading.DMotivationComment = txtDMotivationComment.Text;

            // Bonus fields
            Grading.ENewTopic = int.Parse(txtENewTopic.Text);
            Grading.ENewTopicComment = txtENewTopicComment.Text;

            Grading.EDifficulty = int.Parse(txtEDifficulty.Text);
            Grading.EDifficultyComment = txtEDifficultyComment.Text;

            Grading.EEnvironment = int.Parse(txtEEnvironment.Text);
            Grading.EEnvironmentComment = txtEEnvironmentComment.Text;

            Grading.EBonus = double.Parse(txtEBonus.Text, CultureInfo.InvariantCulture);
        }

        private void UpdateCalculatedGrades()
        {
            // Block A - Weight 1
            var blockAGrade = (Grading.AStrategy * Grading.AStrategyWeight +
                             Grading.AProjectSummaryContents * Grading.AProjectSummaryContentsWeight +
                             Grading.AProjectSummaryPlanning * Grading.AProjectSummaryPlanningWeight) /
                            (Grading.AStrategyWeight + Grading.AProjectSummaryContentsWeight + Grading.AProjectSummaryPlanningWeight);

            // Block B - Weight 4
            var blockBGrade = (Grading.BTheoreticalWork * Grading.BTheoreticalWorkWeight +
                             Grading.BPracticalWork * Grading.BPracticalWorkWeight +
                             Grading.BEvaluation * Grading.BEvaluationWeight +
                             Grading.BResults * Grading.BResultsWeight +
                             Grading.BAutonomy * Grading.BAutonomyWeight) /
                            (Grading.BTheoreticalWorkWeight + Grading.BPracticalWorkWeight + 
                             Grading.BEvaluationWeight + Grading.BResultsWeight + Grading.BAutonomyWeight);

            // Block C - Weight 2
            var blockCGrade = (Grading.CDocumentation * Grading.CDocumentationWeight +
                             Grading.CDefense * Grading.CDefenseWeight +
                             Grading.CPresentations * Grading.CPresentationsWeight) /
                            (Grading.CDocumentationWeight + Grading.CDefenseWeight + Grading.CPresentationsWeight);

            // Block D - Weight 1
            var blockDGrade = (Grading.DCollaborationInternal * Grading.DCollaborationInternalWeight +
                             Grading.DCollaborationExternal * Grading.DCollaborationExternalWeight +
                             Grading.DMotivation * Grading.DMotivationWeight) /
                            (Grading.DCollaborationInternalWeight + Grading.DCollaborationExternalWeight + Grading.DMotivationWeight);

            // Update block grade displays
            lblGrade1.Text = blockAGrade.ToString("0.0", CultureInfo.InvariantCulture);
            lblGrade2.Text = blockBGrade.ToString("0.0", CultureInfo.InvariantCulture);
            lblGrade3.Text = blockCGrade.ToString("0.0", CultureInfo.InvariantCulture);
            lblGrade4.Text = blockDGrade.ToString("0.0", CultureInfo.InvariantCulture);

            // Calculate pre-bonus grade
            var preBonus = (blockAGrade * 1 + blockBGrade * 4 + blockCGrade * 2 + blockDGrade * 1) / 8;
            lblGradePreBonus.Text = preBonus.ToString("0.0", CultureInfo.InvariantCulture);

            // Calculate final grade with bonus
            var finalGrade = Math.Min(6.0, preBonus + Grading.EBonus);
            lblGradeTotal.Text = finalGrade.ToString("0.0", CultureInfo.InvariantCulture);
        }
    }
}