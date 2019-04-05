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
    public class ProjectSingleElement
    {
        public int id { get; set; }
        public string Institute { get; set; }
        public string ProjectNr { get; set; }
        public string advisorName { get; set; }
        public string projectName { get; set; }
        public string projectType1 { get; set; }
        public string projectType2 { get; set; }
        public bool p5 { get; set; }
        public bool p6 { get; set; }
        public bool lng { get; set; }
    }

    public partial class Projectlist : Page
    {
        protected PlaceHolder AdminView;
        protected GridView AllProjects;
        protected GridView CheckProjects;
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        protected Button NewProject;
        
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

            AllProjects.DataSource = UpdateGridView().Select(i => GetProjectSingleElement(i));
            AllProjects.DataBind();

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
            if (ShibUser.GetEmail() == Global.WebAdmin && whichOwner.SelectedValue == "AllProjects")
            {
                var refProj = FilterRelevantProjects(projects);
                LabelNumProjects.Text = $"Anzahl Projekte: {refProj.Count()}";
                var runningProj = refProj.Where(p => p.State >= 4 && p.State < 9);
                LabelNumRunningProjects.Text = $"Anzahl Laufender Projekte: {runningProj.Count()}";
                var ip5n = runningProj.Where(p => p.LogProjectType.P5 && p.LogProjectDuration == 1);
                LabelIP5Normal.Text = $"IP5: {ip5n.Where(p => p.State >= 5).Count()}/{ip5n.Count()}";
                var ip5l = runningProj.Where(p => p.LogProjectType.P5 && p.LogProjectDuration == 2);
                LabelIP5Long.Text = $"IP5 Lang: {ip5l.Where(p => p.State >= 5).Count()}/{ip5l.Count()}";
                var ip6 = runningProj.Where(p => p.LogProjectType.P6 && p.LogProjectDuration == 1);
                LabelIP6.Text = $"IP6: {ip6.Where(p => p.State >= 5).Count()}/{ip6.Count()}";

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
            if (filterText.Text == "") return FilterRelevantProjects(projects);

            var searchString = filterText.Text;
            var filteredProjects = FilterRelevantProjects(
                projects.Where(p => (p.Reservation1Name.Contains(searchString) || p.Reservation2Name.Contains(searchString) || p.LogStudent1Name.Contains(searchString) || p.LogStudent2Name.Contains(searchString))
                                  && p.IsMainVersion))
                        .OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);
            return filteredProjects;
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

        private ProjectSingleElement GetProjectSingleElement(Project i)
        {
            return new ProjectSingleElement
            {
                id = i.Id,
                advisorName = string.Concat(new[]
                {
                    i.Advisor1 != null
                        ? "<a href=\"mailto:" + i.Advisor1.Mail + "\">" +
                          Server.HtmlEncode(i.Advisor1.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "?",
                    i.Advisor2 != null
                        ? "<br /><a href=\"mailto:" + i.Advisor2.Mail + "\">" +
                          Server.HtmlEncode(i.Advisor2.Name).Replace(" ", "&nbsp;") + "</a>"
                        : ""
                }),
                projectName = i.Name,
                Institute = i.Department.DepartmentName,
                p5 = i.LogProjectType?.P5 ?? (i.POneType.P5 || (i.PTwoType?.P5 ?? false)),
                p6 = i.LogProjectType?.P6 ?? (i.POneType.P6 || (i.PTwoType?.P6 ?? false)),
                lng = i.LogProjectDuration == (byte)2,
                projectType1 = "pictures/projectTyp" + (i.TypeDesignUX
                                   ? "DesignUX"
                                   : (i.TypeHW
                                       ? "HW"
                                       : (i.TypeCGIP
                                           ? "CGIP"
                                           : (i.TypeMlAlg
                                               ? "MlAlg"
                                               : (i.TypeAppWeb
                                                   ? "AppWeb"
                                                   : (i.TypeDBBigData
                                                       ? "DBBigData"
                                                       : (i.TypeSysSec
                                                           ? "SysSec"
                                                           : (i.TypeSE ? "SE" : "Transparent")))))))) + ".png",
                projectType2 = "pictures/projectTyp" + (i.TypeHW && i.TypeDesignUX
                                   ? "HW"
                                   : (i.TypeCGIP && (i.TypeDesignUX || i.TypeHW)
                                       ? "CGIP"
                                       : (i.TypeMlAlg && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP)
                                           ? "MlAlg"
                                           : (i.TypeAppWeb &&
                                              (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMlAlg)
                                               ? "AppWeb"
                                               : (i.TypeDBBigData &&
                                                  (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMlAlg ||
                                                   i.TypeAppWeb)
                                                   ? "DBBigData"
                                                   : (i.TypeSysSec &&
                                                      (i.TypeDesignUX || i.TypeHW || i.TypeCGIP || i.TypeMlAlg ||
                                                       i.TypeAppWeb || i.TypeDBBigData)
                                                       ? "SysSec"
                                                       : (i.TypeSE && (i.TypeDesignUX || i.TypeHW || i.TypeCGIP ||
                                                                       i.TypeMlAlg || i.TypeAppWeb ||
                                                                       i.TypeDBBigData || i.TypeSysSec)
                                                           ? "SE"
                                                           : "Transparent"))))))) + ".png",
                ProjectNr = i.ProjectNr != 0 ? i.ProjectNr.ToString("D2"):" "
            };
        }

        protected void AllProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = db.Projects.Single(item => item.Id == ((ProjectSingleElement) e.Row.DataItem).id);

                if (!project.UserCanEdit())
                {
                    var x = e.Row.Cells[e.Row.Cells.Count - 4].Controls;
                    e.Row.Cells[e.Row.Cells.Count - 3].Controls.OfType<DataBoundLiteralControl>().First().Visible = false; //edit
                }
                
                e.Row.Cells[e.Row.Cells.Count - 2].Controls.OfType<LinkButton>().First().Visible = project.UserCanDelete(); //delete

                //TODO: decide wether to keep this button or not
                e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = false; //submit

                Color col = ColorTranslator.FromHtml(project.StateColor);
                foreach (TableCell cell in e.Row.Cells)
                    cell.BackColor = col;
            }
        }

        protected void NewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProjectEditPage");
        }

        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
            {
                List<ProjectSingleElement> sortedProjects = UpdateGridView().Select(i => GetProjectSingleElement(i)).ToList();

                switch (e.CommandArgument)
                {
                    case "Advisor":
                        AllProjects.DataSource = sortedProjects.OrderBy(p => p.advisorName.Contains("?")).ThenBy(p => p.advisorName);
                        break;
                    case "Institute":
                        AllProjects.DataSource = sortedProjects.OrderBy(p => p.Institute);
                        break;
                    case "projectName":
                        AllProjects.DataSource = sortedProjects.OrderBy(p => p.projectName);
                        break;
                    case "P5":
                        AllProjects.DataSource = sortedProjects.OrderByDescending(p => p.p5).ThenBy(p => p.lng);
                        break;
                    case "P6":
                        AllProjects.DataSource = sortedProjects.OrderByDescending(p => p.p6);
                        break;
                    case "Long":
                        AllProjects.DataSource = sortedProjects.OrderByDescending(p => p.lng);
                        break;
                }
            }
            else
            {
                var id = Convert.ToInt32(e.CommandArgument);
                switch (e.CommandName)
                {
                    case "revokeSubmission":
                        var projectr = db.Projects.Single(i => i.Id == id);
                        projectr.State = ProjectState.InProgress;
                        db.SubmitChanges();
                        Response.Redirect(Request.RawUrl);
                        break;
                    case "deleteProject":
                        var project = db.Projects.Single(i => i.Id == id);
                        project.Delete(db);
                        db.SubmitChanges();
                        Response.Redirect(Request.RawUrl);
                        break;
                    case "editProject":
                        Response.Redirect("ProjectEditPage?id=" + id);
                        break;
                    case "submitProject":
                        //EinreichenButton_Click(id);
                        break;
                    default:
                        throw new Exception("Unknown command " + e.CommandName);
                }
            }
        }

        protected void AllProjectsAsPDF_Click(object sender, EventArgs e)
        {
            if (AllProjects.Rows.Count != 0)
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
                        ((IEnumerable<ProjectSingleElement>) AllProjects.DataSource)
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
                ExcelCreator.GenerateProjectList(output, ((IEnumerable<ProjectSingleElement>) AllProjects.DataSource)
                    .Select(p => db.Projects.Single(pr => pr.Id == p.id && pr.IsMainVersion))
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
            projects = UpdateGridView();
            AllProjects.DataSource = projects
               .Select(i => GetProjectSingleElement(i));
            AllProjects.DataBind();
        }

        /*
        protected void EinreichenButton_Click(int id)
        {
            Project project = db.Projects.Single(p => p.Id == id);
            var validationMessage = project.GenerateValidationMessage();
            if (validationMessage != "")
            {
                var sb = new StringBuilder();
                sb.Append("<script type='text/javascript'>");
                sb.Append("window.onload=function(){");
                sb.Append("alert('");
                sb.Append(validationMessage);
                sb.Append("')};");
                sb.Append("</script>");
                ClientScript.RegisterClientScriptBlock(GetType(), "alert", sb.ToString());
            }
            else
            {
                throw new Exception("has changed");
                project.Submit(db);
                db.SubmitChanges();
                project.CopyAndUseCopyAsMainVersion(db);
                Response.Redirect("projectlist");
            }
        }
        */

        protected void AllProjects_Sorting(object sender, GridViewSortEventArgs e)
        {
            AllProjects.DataBind();
        }

    }
}