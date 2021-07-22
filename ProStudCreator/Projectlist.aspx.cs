using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace ProStudCreator
{
    public partial class Projectlist : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        
        // SR test
        private IQueryable<Project> projects;
        //~SR test

        protected void Page_Init(object sender, EventArgs e)
        {
            dropSemester.DataSource = db.Semester.OrderByDescending(s => s.StartDate);
            dropSemester.DataBind();
            dropSemester.Items.Insert(0, new ListItem("Alle Semester", "allSemester"));
            dropSemester.Items.Insert(1, new ListItem("――――――――――――――――", "."));
            dropSemester.SelectedValue = Semester.NextSemester(db).Id.ToString();

            ProjectGrid.db = db;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            projects = db.Projects.Where(i => i.IsMainVersion);

            if (IsPostBack || Session["SelectedOwner"] == null || Session["SelectedSemester"] == null)
            {
                Session["SelectedOwner"] = whichOwner.SelectedValue;
                Session["SelectedSemester"] = dropSemester.SelectedValue;
            }
            else
            {
                whichOwner.SelectedValue = (string)Session["SelectedOwner"];
                dropSemester.SelectedValue = (string)Session["SelectedSemester"];
            }

            ProjectGrid.SetProjects(UpdateGridView());

            //Disabling the "-----" element in the Dropdownlist. So the item "Alle Semester" is separated from the rest
            dropSemester.Items.FindByValue(".").Attributes.Add("disabled", "disabled");

            colExInProgress.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.InProgress));
            colExSubmitted.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Submitted));
            colExRejected.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Rejected));
            colExPublished.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Published));
            colExOngoing.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Ongoing));
            colExFinished.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Finished));
            colExCanceled.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Canceled));

            //Statistics
            if ((ShibUser.IsWebAdmin() || ShibUser.CanVisitAdminPage()) && whichOwner.SelectedValue == "AllProjects")
            {
                var refProj = FilterRelevantProjects(projects);

                var submittedProj = projects.Where(p => p.State == ProjectState.Submitted && p.IsMainVersion);
                var submittedProjIP5 = submittedProj.Where(p => p.POneType.P5 || (p.PTwoType != null && p.PTwoType.P5));
                var submittedProjIP6 = submittedProj.Where(p => p.POneType.P6 || (p.PTwoType != null && p.PTwoType.P6));
                var submittedProjI4DS = submittedProj.Where(p => p.DepartmentId == 0);
                var submittedProjIMVS = submittedProj.Where(p => p.DepartmentId == 1);
                var submittedProjIIT = submittedProj.Where(p => p.DepartmentId == 2);

                var publishedProj = refProj.Where(p => p.State == ProjectState.Published);
                var publishedProjIP5 = publishedProj.Where(p => p.POneType.P5 || (p.PTwoType != null && p.PTwoType.P5));
                var publishedProjIP6 = publishedProj.Where(p => p.POneType.P6 || (p.PTwoType != null && p.PTwoType.P6));
                var publishedProjI4DS = publishedProj.Where(p => p.DepartmentId == 0);
                var publishedProjIMVS = publishedProj.Where(p => p.DepartmentId == 1);
                var publishedProjIIT = publishedProj.Where(p => p.DepartmentId == 2);

                var runningProj = refProj.Where(p => p.State == ProjectState.Ongoing);
                var runningProjIP5N = runningProj.Where(p => p.LogProjectType.P5 && p.LogProjectDuration == 1);
                var runningProjIP5L = runningProj.Where(p => p.LogProjectType.P5 && p.LogProjectDuration == 2);
                var runningProjIP6 = runningProj.Where(p => p.LogProjectType.P6 && p.LogProjectDuration == 1);
                var runningProjI4DS = runningProj.Where(p => p.DepartmentId == 0);
                var runningProjIMVS = runningProj.Where(p => p.DepartmentId == 1);
                var runningProjIIT = runningProj.Where(p => p.DepartmentId == 2);

                var finishedProj = refProj.Where(p => p.State > ProjectState.Ongoing && p.State < ProjectState.Deleted);
                var finishedProjIP5N = finishedProj.Where(p => p.LogProjectType.P5 && p.LogProjectDuration == 1);
                var finishedProjIP5L = finishedProj.Where(p => p.LogProjectType.P5 && p.LogProjectDuration == 2);
                var finishedProjIP6 = finishedProj.Where(p => p.LogProjectType.P6 && p.LogProjectDuration == 1);
                var finishedProjI4DS = finishedProj.Where(p => p.DepartmentId == 0);
                var finishedProjIMVS = finishedProj.Where(p => p.DepartmentId == 1);
                var finishedProjIIT = finishedProj.Where(p => p.DepartmentId == 2);

                LabelNumSubmittedProjects.Text = $"Anzahl eingereichter Projekte: {submittedProj.Count()}";
                LabelNumSubmittedProjectsIP5.Text = $"IP5: {submittedProjIP5.Count()}";
                LabelNumSubmittedProjectsIP6.Text = $"IP6: {submittedProjIP6.Count()}";
                LabelNumSubmittedProjectsI4DS.Text = $"I4DS: {submittedProjI4DS.Count()}";
                LabelNumSubmittedProjectsIMVS.Text = $"IMVS: {submittedProjIMVS.Count()}";
                LabelNumSubmittedProjectsIIT.Text = $"IIT: {submittedProjIIT.Count()}";

                LabelNumPublishedProjects.Text = $"Anzahl veröffentlichter Projekte: {publishedProj.Count()}";
                LabelNumPublishedProjectsIP5.Text = $"IP5: {publishedProjIP5.Count()}";
                LabelNumPublishedProjectsIP6.Text = $"IP6: {publishedProjIP6.Count()}";
                LabelNumPublishedProjectsI4DS.Text = $"I4DS: {publishedProjI4DS.Count()}";
                LabelNumPublishedProjectsIMVS.Text = $"IMVS: {publishedProjIMVS.Count()}";
                LabelNumPublishedProjectsIIT.Text = $"IIT: {publishedProjIIT.Count()}";

                LabelNumRunningProjects.Text = $"Anzahl laufender Projekte: {runningProj.Count()}";
                LabelNumRunningProjectsIP5N.Text = $"IP5N: {runningProjIP5N.Count()}";
                LabelNumRunningProjectsIP5L.Text = $"IP5L: {runningProjIP5L.Count()}";
                LabelNumRunningProjectsIP6.Text = $"IP6: {runningProjIP6.Count()}";
                LabelNumRunningProjectsI4DS.Text = $"I4DS: {runningProjI4DS.Count()}";
                LabelNumRunningProjectsIMVS.Text = $"IMVS: {runningProjIMVS.Count()}";
                LabelNumRunningProjectsIIT.Text = $"IIT: {runningProjIIT.Count()}";

                LabelNumFinishedProjects.Text = $"Anzahl abgeschlossener Projekte: {finishedProj.Count()}";
                LabelNumFinishedProjectsIP5N.Text = $"IP5N: {finishedProjIP5N.Count()}";
                LabelNumFinishedProjectsIP5L.Text = $"IP5L: {finishedProjIP5L.Count()}";
                LabelNumFinishedProjectsIP6.Text = $"IP6: {finishedProjIP6.Count()}";
                LabelNumFinishedProjectsI4DS.Text = $"I4DS: {finishedProjI4DS.Count()}";
                LabelNumFinishedProjectsIMVS.Text = $"IMVS: {finishedProjIMVS.Count()}";
                LabelNumFinishedProjectsIIT.Text = $"IIT: {finishedProjIIT.Count()}";

                DivProjectStatistics.Visible = true;
            }
            else
            {
                DivProjectStatistics.Visible = false;
            }

            Session["LastPage"] = "projectlist";
        }

        private IQueryable<Project> UpdateGridView()
        {
            if (string.IsNullOrWhiteSpace(filterText.Text)) return FilterRelevantProjects(projects);

            // TODO: Improve search
            var searchString = filterText.Text;
            return FilterRelevantProjects(
                projects.Where(p => (p.Reservation1Name.Contains(searchString) || p.Reservation2Name.Contains(searchString) || p.LogStudent1FirstName.Contains(searchString) || p.LogStudent1LastName.Contains(searchString) || p.LogStudent2FirstName.Contains(searchString) || p.LogStudent2LastName.Contains(searchString))
                                  && p.IsMainVersion))
                        .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
        }

        private IQueryable<Project> FilterRelevantProjects(IQueryable<Project> allProjects)
        {
            var projects = allProjects;
            switch (Session["SelectedOwner"])
            {
                case "OwnProjects":
                    if (dropSemester.SelectedValue == "allSemester")
                    {
                        projects = projects.Where(p => (p.Creator == ShibUser.GetEmail() || p.Advisor1.Mail == ShibUser.GetEmail() || p.Advisor2.Mail == ShibUser.GetEmail()) 
                                                     && p.State != ProjectState.Deleted
                                                     && p.IsMainVersion)
                                           .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.State).ThenBy(p => p.ProjectNr);
                    }

                    else
                    {
                        projects = projects.Where(p => (p.Creator == ShibUser.GetEmail() || p.Advisor1.Mail == ShibUser.GetEmail() || p.Advisor2.Mail == ShibUser.GetEmail())
                                                     && p.State != ProjectState.Deleted
                                                     && p.IsMainVersion
                                                     && (p.Semester.Id == int.Parse(dropSemester.SelectedValue)
                                                        || p.State < ProjectState.Published))
                                           .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.State).ThenBy(p => p.ProjectNr);
                    }
                    break;
                case "AllProjects":
                    if (dropSemester.SelectedValue == "allSemester")
                        projects = projects.Where(p => (p.State == ProjectState.Published || p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.Canceled || p.State == ProjectState.ArchivedFinished || p.State == ProjectState.ArchivedCanceled)
                                                     && p.IsMainVersion)
                                           .OrderBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.State).ThenBy(p => p.ProjectNr);
                    else
                        projects = projects.Where(p => (p.State == ProjectState.Published || p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.Canceled || p.State == ProjectState.ArchivedFinished || p.State == ProjectState.ArchivedCanceled)
                                                     && p.IsMainVersion
                                                     && p.Semester.Id == int.Parse(dropSemester.SelectedValue))
                                           .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.State).ThenBy(p => p.ProjectNr);
                    break;
            }
            return projects;
        }

        protected void NewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProjectEditPage");
        }

        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            if (ProjectGrid.Grid.Rows.Count != 0)
            {
                Response.Clear();
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", "attachment; filename=AllProjects.pdf");
                Response.Buffer = false;

                var output = Response.OutputStream;
                var document = PdfCreator.CreateDocument();
                try
                {
                    var pdfCreator = new PdfCreator();
                    pdfCreator.AppendToPDF(document, output,
                        ((IEnumerable<ProjectRowElement>) ProjectGrid.Grid.DataSource)
                        .Select(p => db.Projects.Single(pr => pr.Id == p.id))
                        .OrderBy(p => p.Reservation1Name != "")
                        .ThenBy(p => p.Department.DepartmentName)
                        .ThenBy(p => p.ProjectNr)
                    );
                    document.Dispose();
                }
                catch (DocumentException documentException) when (documentException.Message.Contains("0x800704CD"))
                {
                    try
                    {
                        document.Dispose();
                    }
                    catch { }
                }
                catch (Exception)
                {
                    try
                    {
                        document.Dispose();
                    }
                    catch { }
                    throw;
                }
                Response.End();
            }
            else
            {
                var message = "In dieser Kategorie sind keine Projekte vorhanden!";
                var sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(message);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", sb.ToString());
            }
        }

        protected void AllProjectsAsExcel_Click(object sender, EventArgs e)
        {
            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                ExcelCreator.GenerateProjectList(output, ((IEnumerable<ProjectRowElement>) ProjectGrid.Grid.DataSource)
                    .Select(p => db.Projects.Single(pr => pr.Id == p.id && pr.IsMainVersion))
                    .Where(p => p.SubmitToStudyCourseCS)
                    .OrderBy(p => p.Reservation1Name != "")
                    .ThenBy(p => p.Department.DepartmentName)
                    .ThenBy(p => p.ProjectNr));
                bytesInStream = output.ToArray();
            }

            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition", "attachment; filename=Informatikprojekte.xlsx");
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }

        protected void FilterButton_Click(object sender, EventArgs e)
        {
            ProjectGrid.SetProjects(UpdateGridView());
        }
    }
}