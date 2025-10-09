using System;
using System.Globalization;
using System.Web.UI;

namespace ProStudCreator.UserControls
{
    public partial class GradingV1Control : UserControl 
    {
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

        public void Initialize()
        {
        }

        public void LoadFormFields(Project _project, string _projectName, bool _underNDA, GradingV1 _grading)
        {
            lblProjectTitle.Text = _projectName;

            if (_project.Advisor2 == null)
                lblAdvisor.Text = _project.Advisor1.Name;
            else
                lblAdvisor.Text = $"{_project.Advisor1.Name}, {_project.Advisor2.Name}";

            lblExpert.Text = _project.Expert?.Name ?? "-";

            chkUnderNDA.Checked = _underNDA;
            chkNotUnderNDA.Checked = !_underNDA;

            txtCriticalAcclaim.Text = _grading.CriticalAcclaim;

            // Block A fields
            txtAStrategyWeight.Text = _grading.AStrategyWeight.ToString("0.0");
            txtAStrategy.Text = _grading.AStrategy.ToString("0.0");
            txtAStrategyComment.Text = _grading.AStrategyComment;

            txtAProjectSummaryContentsWeight.Text = _grading.AProjectSummaryContentsWeight.ToString("0.0");
            txtAProjectSummaryContents.Text = _grading.AProjectSummaryContents.ToString("0.0");
            txtAProjectSummaryContentsComment.Text = _grading.AProjectSummaryContentsComment;

            txtAProjectSummaryPlanningWeight.Text = _grading.AProjectSummaryPlanningWeight.ToString("0.0");
            txtAProjectSummaryPlanning.Text = _grading.AProjectSummaryPlanning.ToString("0.0");
            txtAProjectSummaryPlanningComment.Text = _grading.AProjectSummaryPlanningComment;

            // Block B fields
            txtBTheoreticalWorkWeight.Text = _grading.BTheoreticalWorkWeight.ToString("0.0");
            txtBTheoreticalWork.Text = _grading.BTheoreticalWork.ToString("0.0");
            txtBTheoreticalWorkComment.Text = _grading.BTheoreticalWorkComment;

            txtBPracticalWorkWeight.Text = _grading.BPracticalWorkWeight.ToString("0.0");
            txtBPracticalWork.Text = _grading.BPracticalWork.ToString("0.0");
            txtBPracticalWorkComment.Text = _grading.BPracticalWorkComment;

            txtBEvaluationWeight.Text = _grading.BEvaluationWeight.ToString("0.0");
            txtBEvaluation.Text = _grading.BEvaluation.ToString("0.0");
            txtBEvaluationComment.Text = _grading.BEvaluationComment;

            txtBResultsWeight.Text = _grading.BResultsWeight.ToString("0.0");
            txtBResults.Text = _grading.BResults.ToString("0.0");
            txtBResultsComment.Text = _grading.BResultsComment;

            txtBAutonomyWeight.Text = _grading.BAutonomyWeight.ToString("0.0");
            txtBAutonomy.Text = _grading.BAutonomy.ToString("0.0");
            txtBAutonomyComment.Text = _grading.BAutonomyComment;

            // Block C fields
            txtCDocumentationWeight.Text = _grading.CDocumentationWeight.ToString("0.0");
            txtCDocumentation.Text = _grading.CDocumentation.ToString("0.0");
            txtCDocumentationComment.Text = _grading.CDocumentationComment;

            txtCDefenseWeight.Text = _grading.CDefenseWeight.ToString("0.0");
            txtCDefense.Text = _grading.CDefense.ToString("0.0");
            txtCDefenseComment.Text = _grading.CDefenseComment;

            txtCPresentationsWeight.Text = _grading.CPresentationsWeight.ToString("0.0");
            txtCPresentations.Text = _grading.CPresentations.ToString("0.0");
            txtCPresentationsComment.Text = _grading.CPresentationsComment;

            // Block D fields
            txtDCollaborationInternalWeight.Text = _grading.DCollaborationInternalWeight.ToString("0.0");
            txtDCollaborationInternal.Text = _grading.DCollaborationInternal.ToString("0.0");
            txtDCollaborationInternalComment.Text = _grading.DCollaborationInternalComment;

            txtDCollaborationExternalWeight.Text = _grading.DCollaborationExternalWeight.ToString("0.0");
            txtDCollaborationExternal.Text = _grading.DCollaborationExternal.ToString("0.0");
            txtDCollaborationExternalComment.Text = _grading.DCollaborationExternalComment;

            txtDMotivationWeight.Text = _grading.DMotivationWeight.ToString("0.0");
            txtDMotivation.Text = _grading.DMotivation.ToString("0.0");
            txtDMotivationComment.Text = _grading.DMotivationComment;

            // Bonus fields
            txtENewTopic.Text = _grading.ENewTopic.ToString();
            txtENewTopicComment.Text = _grading.ENewTopicComment;

            txtEDifficulty.Text = _grading.EDifficulty.ToString();
            txtEDifficultyComment.Text = _grading.EDifficultyComment;

            txtEEnvironment.Text = _grading.EEnvironment.ToString();
            txtEEnvironmentComment.Text = _grading.EEnvironmentComment;

            txtEBonus.Text = _grading.EBonus.ToString("0.0");

            UpdateCalculatedGrades();
        }

        protected void OnGradeChanged(object sender, EventArgs e)
        {
            UpdateCalculatedGrades();
        }

        public void SaveFormFields(out bool _underNDA, GradingV1 _grading)
        {
            _underNDA = chkUnderNDA.Checked;

            _grading.CriticalAcclaim = txtCriticalAcclaim.Text;

            // Block A fields
            _grading.AStrategyWeight = double.Parse(txtAStrategyWeight.Text, CultureInfo.InvariantCulture);
            _grading.AStrategy = double.Parse(txtAStrategy.Text, CultureInfo.InvariantCulture);
            _grading.AStrategyComment = txtAStrategyComment.Text;

            _grading.AProjectSummaryContentsWeight = double.Parse(txtAProjectSummaryContentsWeight.Text, CultureInfo.InvariantCulture);
            _grading.AProjectSummaryContents = double.Parse(txtAProjectSummaryContents.Text, CultureInfo.InvariantCulture);
            _grading.AProjectSummaryContentsComment = txtAProjectSummaryContentsComment.Text;

            _grading.AProjectSummaryPlanningWeight = double.Parse(txtAProjectSummaryPlanningWeight.Text, CultureInfo.InvariantCulture);
            _grading.AProjectSummaryPlanning = double.Parse(txtAProjectSummaryPlanning.Text, CultureInfo.InvariantCulture);
            _grading.AProjectSummaryPlanningComment = txtAProjectSummaryPlanningComment.Text;

            // Block B fields
            _grading.BTheoreticalWorkWeight = double.Parse(txtBTheoreticalWorkWeight.Text, CultureInfo.InvariantCulture);
            _grading.BTheoreticalWork = double.Parse(txtBTheoreticalWork.Text, CultureInfo.InvariantCulture);
            _grading.BTheoreticalWorkComment = txtBTheoreticalWorkComment.Text;

            _grading.BPracticalWorkWeight = double.Parse(txtBPracticalWorkWeight.Text, CultureInfo.InvariantCulture);
            _grading.BPracticalWork = double.Parse(txtBPracticalWork.Text, CultureInfo.InvariantCulture);
            _grading.BPracticalWorkComment = txtBPracticalWorkComment.Text;

            _grading.BEvaluationWeight = double.Parse(txtBEvaluationWeight.Text, CultureInfo.InvariantCulture);
            _grading.BEvaluation = double.Parse(txtBEvaluation.Text, CultureInfo.InvariantCulture);
            _grading.BEvaluationComment = txtBEvaluationComment.Text;

            _grading.BResultsWeight = double.Parse(txtBResultsWeight.Text, CultureInfo.InvariantCulture);
            _grading.BResults = double.Parse(txtBResults.Text, CultureInfo.InvariantCulture);
            _grading.BResultsComment = txtBResultsComment.Text;

            _grading.BAutonomyWeight = double.Parse(txtBAutonomyWeight.Text, CultureInfo.InvariantCulture);
            _grading.BAutonomy = double.Parse(txtBAutonomy.Text, CultureInfo.InvariantCulture);
            _grading.BAutonomyComment = txtBAutonomyComment.Text;

            // Block C fields
            _grading.CDocumentationWeight = double.Parse(txtCDocumentationWeight.Text, CultureInfo.InvariantCulture);
            _grading.CDocumentation = double.Parse(txtCDocumentation.Text, CultureInfo.InvariantCulture);
            _grading.CDocumentationComment = txtCDocumentationComment.Text;

            _grading.CDefenseWeight = double.Parse(txtCDefenseWeight.Text, CultureInfo.InvariantCulture);
            _grading.CDefense = double.Parse(txtCDefense.Text, CultureInfo.InvariantCulture);
            _grading.CDefenseComment = txtCDefenseComment.Text;

            _grading.CPresentationsWeight = double.Parse(txtCPresentationsWeight.Text, CultureInfo.InvariantCulture);
            _grading.CPresentations = double.Parse(txtCPresentations.Text, CultureInfo.InvariantCulture);
            _grading.CPresentationsComment = txtCPresentationsComment.Text;

            // Block D fields
            _grading.DCollaborationInternalWeight = double.Parse(txtDCollaborationInternalWeight.Text, CultureInfo.InvariantCulture);
            _grading.DCollaborationInternal = double.Parse(txtDCollaborationInternal.Text, CultureInfo.InvariantCulture);
            _grading.DCollaborationInternalComment = txtDCollaborationInternalComment.Text;

            _grading.DCollaborationExternalWeight = double.Parse(txtDCollaborationExternalWeight.Text, CultureInfo.InvariantCulture);
            _grading.DCollaborationExternal = double.Parse(txtDCollaborationExternal.Text, CultureInfo.InvariantCulture);
            _grading.DCollaborationExternalComment = txtDCollaborationExternalComment.Text;

            _grading.DMotivationWeight = double.Parse(txtDMotivationWeight.Text, CultureInfo.InvariantCulture);
            _grading.DMotivation = double.Parse(txtDMotivation.Text, CultureInfo.InvariantCulture);
            _grading.DMotivationComment = txtDMotivationComment.Text;

            // Bonus fields
            _grading.ENewTopic = int.Parse(txtENewTopic.Text);
            _grading.ENewTopicComment = txtENewTopicComment.Text;

            _grading.EDifficulty = int.Parse(txtEDifficulty.Text);
            _grading.EDifficultyComment = txtEDifficultyComment.Text;

            _grading.EEnvironment = int.Parse(txtEEnvironment.Text);
            _grading.EEnvironmentComment = txtEEnvironmentComment.Text;

            _grading.EBonus = double.Parse(txtEBonus.Text, CultureInfo.InvariantCulture);
        }

        private double SafeParseDouble(string text)
        {
            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
            return 0.0;
        }

        private double ComputeBlockAGrade()
        {
            var aStrategyWeight = SafeParseDouble(txtAStrategyWeight.Text);
            var aStrategy = SafeParseDouble(txtAStrategy.Text);
            var aProjSummaryContentsWeight = SafeParseDouble(txtAProjectSummaryContentsWeight.Text);
            var aProjSummaryContents = SafeParseDouble(txtAProjectSummaryContents.Text);
            var aProjSummaryPlanningWeight = SafeParseDouble(txtAProjectSummaryPlanningWeight.Text);
            var aProjSummaryPlanning = SafeParseDouble(txtAProjectSummaryPlanning.Text);

            var blockAGrade = (aStrategy * aStrategyWeight +
                             aProjSummaryContents * aProjSummaryContentsWeight +
                             aProjSummaryPlanning * aProjSummaryPlanningWeight) /
                            (aStrategyWeight + aProjSummaryContentsWeight + aProjSummaryPlanningWeight);

            return double.IsNaN(blockAGrade) ? 0 : blockAGrade;
        }

        private double ComputeBlockBGrade()
        {
            var bTheoreticalWorkWeight = SafeParseDouble(txtBTheoreticalWorkWeight.Text);
            var bTheoreticalWork = SafeParseDouble(txtBTheoreticalWork.Text);
            var bPracticalWorkWeight = SafeParseDouble(txtBPracticalWorkWeight.Text);
            var bPracticalWork = SafeParseDouble(txtBPracticalWork.Text);
            var bEvaluationWeight = SafeParseDouble(txtBEvaluationWeight.Text);
            var bEvaluation = SafeParseDouble(txtBEvaluation.Text);
            var bResultsWeight = SafeParseDouble(txtBResultsWeight.Text);
            var bResults = SafeParseDouble(txtBResults.Text);
            var bAutonomyWeight = SafeParseDouble(txtBAutonomyWeight.Text);
            var bAutonomy = SafeParseDouble(txtBAutonomy.Text);

            var blockBGrade = (bTheoreticalWork * bTheoreticalWorkWeight +
                             bPracticalWork * bPracticalWorkWeight +
                             bEvaluation * bEvaluationWeight +
                             bResults * bResultsWeight +
                             bAutonomy * bAutonomyWeight) /
                            (bTheoreticalWorkWeight + bPracticalWorkWeight +
                             bEvaluationWeight + bResultsWeight + bAutonomyWeight);

            return double.IsNaN(blockBGrade) ? 0 : blockBGrade;
        }

        private double ComputeBlockCGrade()
        {
            var cDocumentationWeight = SafeParseDouble(txtCDocumentationWeight.Text);
            var cDocumentation = SafeParseDouble(txtCDocumentation.Text);
            var cDefenseWeight = SafeParseDouble(txtCDefenseWeight.Text);
            var cDefense = SafeParseDouble(txtCDefense.Text);
            var cPresentationsWeight = SafeParseDouble(txtCPresentationsWeight.Text);
            var cPresentations = SafeParseDouble(txtCPresentations.Text);

            var blockCGrade = (cDocumentation * cDocumentationWeight +
                             cDefense * cDefenseWeight +
                             cPresentations * cPresentationsWeight) /
                            (cDocumentationWeight + cDefenseWeight + cPresentationsWeight);

            return double.IsNaN(blockCGrade) ? 0 : blockCGrade;
        }

        private double ComputeBlockDGrade()
        {
            var dCollabInternalWeight = SafeParseDouble(txtDCollaborationInternalWeight.Text);
            var dCollabInternal = SafeParseDouble(txtDCollaborationInternal.Text);
            var dCollabExternalWeight = SafeParseDouble(txtDCollaborationExternalWeight.Text);
            var dCollabExternal = SafeParseDouble(txtDCollaborationExternal.Text);
            var dMotivationWeight = SafeParseDouble(txtDMotivationWeight.Text);
            var dMotivation = SafeParseDouble(txtDMotivation.Text);

            var blockDGrade = (dCollabInternal * dCollabInternalWeight +
                             dCollabExternal * dCollabExternalWeight +
                             dMotivation * dMotivationWeight) /
                            (dCollabInternalWeight + dCollabExternalWeight + dMotivationWeight);

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
            var bonus = SafeParseDouble(txtEBonus.Text);
            return Math.Min(6.0, ComputePreBonus() + bonus);
        }

        private void UpdateCalculatedGrades()
        {
            lblGradeA.Text = ComputeBlockAGrade().ToString("0.0", CultureInfo.InvariantCulture);
            lblGradeB.Text = ComputeBlockBGrade().ToString("0.0", CultureInfo.InvariantCulture);
            lblGradeC.Text = ComputeBlockCGrade().ToString("0.0", CultureInfo.InvariantCulture);
            lblGradeD.Text = ComputeBlockDGrade().ToString("0.0", CultureInfo.InvariantCulture);
            lblGradePreBonus.Text = ComputePreBonus().ToString("0.0", CultureInfo.InvariantCulture);
            lblGradeTotal.Text = lblFinalGradeFrontPage.Text = ComputeFinalGrade().ToString("0.0", CultureInfo.InvariantCulture);
        }
    }
}