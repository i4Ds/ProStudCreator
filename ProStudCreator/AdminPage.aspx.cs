using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public class ProjectSingleTask
    {
        public string Project { get; set; }
        public string TaskOrganiseExpert { get; set; }
        public string TaskOrganiseRoom { get; set; }
        public string TaskOrganiseDate { get; set; }
        public string TaskPayExpert { get; set; }
    }

    public partial class AdminPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
            SelectedSemester.DataSource = db.Semester.OrderByDescending(s => s.StartDate);
            SelectedSemester.DataBind();
            SelectedSemester.SelectedValue = Semester.CurrentSemester(db).Id.ToString();
            SelectedSemester.Items.Insert(0, new ListItem("Alle Semester", ""));
            SelectedSemester.Items.Insert(1, new ListItem("――――――――――――――――", ".", false));
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShibUser.CanVisitAdminPage())
            {
                Response.Redirect("error/AccessDenied.aspx?url=" + HttpContext.Current.Request.Url.AbsoluteUri);
                Response.End();
            }

            DivAdminProjects.Visible = ShibUser.CanPublishProject();
            DivExcelExport.Visible = ShibUser.CanExportExcel();


            if (!Page.IsPostBack)
            {
                if (Session["SelectedAdminProjects"] == null)
                {
                    radioSelectedProjects.SelectedIndex = 0;
                    Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
                }
                else
                {
                    radioSelectedProjects.SelectedIndex = (int)Session["SelectedAdminProjects"];
                }

                if (Session["AdminProjectCollapsed"] == null)
                    CollapseAdminProjects(false);
                else
                    CollapseAdminProjects((bool)Session["AdminProjectCollapsed"]);

                if (Session["ExcelExportCollapsed"] == null)
                    CollapseExcelExport(false);
                else
                    CollapseExcelExport((bool)Session["ExcelExportCollapsed"]);


                if (Session["AddInfoCollapsed"] == null)
                    CollapseAddInfo(true);
                else
                    CollapseAddInfo((bool)Session["AddInfoCollapsed"]);

            }

            CheckProjects.DataSource = GetSelectedProjects();
            CheckProjects.DataBind();


            gvDates.DataSource = CalculateDates();
            gvDates.DataBind();

            //----------- wozu?
            //GVTasks.DataSource = AllTasks();
            //GVTasks.DataBind();


            Session["LastPage"] = "adminpage";
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
                ProjectNr = i.ProjectNr != 0 ? i.ProjectNr.ToString("D2") : " "
            };
        }

        private IEnumerable<object> CalculateDates()
        {
            for (var year = DateTime.Now.Year; year <= DateTime.Now.Year + 3; year++)
            {
                yield return new { Name = $"{year % 100:D2}FS", StartDate = Semester.StartOfWeek(year, 8).ToString("yyyy-MM-dd"), EndDate = Semester.StartOfWeek(year, 24).AddDays(5).ToString("yyyy-MM-dd"), SubmissionUntil = Semester.StartOfWeek(year - 1, 47).AddDays(2).ToString("yyyy-MM-dd"), ProjectAllocation = $"Ende 01.{year}", SubmissionIP5 = Semester.StartOfWeek(year, 24).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP5Lang = Semester.StartOfWeek(year, 33).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP6 = Semester.StartOfWeek(year, 33).AddDays(4).ToString("dd.MM.yyyy"), DefenseStart = Semester.StartOfWeek(year, 36).ToString("dd.MM.yyyy"), DefenseEnd = Semester.StartOfWeek(year, 37).AddDays(4).ToString("dd.MM.yyyy"), Exhibition = "?", DayBeforeNext = Semester.StartOfWeek(year, 24).AddDays(6).ToString("yyyy-MM-dd") };

                yield return new { Name = $"{year % 100:D2}HS", StartDate = Semester.StartOfWeek(year, 38).ToString("yyyy-MM-dd"), EndDate = Semester.StartOfWeek(year + 1, 3).AddDays(5).ToString("yyyy-MM-dd"), SubmissionUntil = Semester.StartOfWeek(year, 21).AddDays(2).ToString("yyyy-MM-dd"), ProjectAllocation = $"Anfang 07.{year}", SubmissionIP5 = Semester.StartOfWeek(year + 1, 3).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP5Lang = Semester.StartOfWeek(year + 1, 12).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP6 = Semester.StartOfWeek(year + 1, 12).AddDays(4).ToString("dd.MM.yyyy"), DefenseStart = Semester.StartOfWeek(year + 1, 16).ToString("dd.MM.yyyy"), DefenseEnd = Semester.StartOfWeek(year + 1, 17).AddDays(4).ToString("dd.MM.yyyy"), Exhibition = "keine", DayBeforeNext = Semester.StartOfWeek(year + 1, 3).AddDays(6).ToString("yyyy-MM-dd") };
            }
        }

        private IQueryable<ProjectSingleElement> GetSelectedProjects()
        {
            var depId = ShibUser.GetDepartment(db).Id;

            switch(radioSelectedProjects.SelectedValue)
            {
                case "inProgress":
                    return db.Projects.Where(p => p.IsMainVersion
                                               && (p.State == ProjectState.InProgress || p.State == ProjectState.Submitted || p.State == ProjectState.Rejected))
                                      .OrderBy(i => i.Department.DepartmentName)
                                      .ThenBy(i => i.ProjectNr)
                                      .Select(i => GetProjectSingleElement(i));
                case "toPublish":
                    return db.Projects.Where(p => p.State == ProjectState.Submitted
                                               && p.IsMainVersion
                                               && p.DepartmentId == depId)
                                      .OrderBy(i => i.Advisor1.Mail)
                                      .ThenBy(i => i.ProjectNr)
                                      .Select(i => GetProjectSingleElement(i));
                case "allProjects":
                    return db.Projects.Where(p => p.State != ProjectState.Deleted
                                               && p.IsMainVersion)
                                      .OrderBy(i => i.DepartmentId)
                                      .ThenBy(i => i.State)
                                      .ThenBy(i => i.ProjectNr)
                                      .Select(i => GetProjectSingleElement(i));
                default:
                    throw new Exception($"Unexpected radioSelectedProjects.SelectedValue '{radioSelectedProjects.SelectedValue}'");
            }
        }

        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
                return;

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

        private void CollapseAdminProjects(bool collapse)
        {
            Session["AdminProjectCollapsed"] = collapse;
            DivAdminProjectsCollapsable.Visible = divRadioProjects.Visible = !collapse;
            btnAdminProjectsCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseExcelExport(bool collapse)
        {
            Session["ExcelExportCollapsed"] = collapse;
            DivExcelExportCollapsable.Visible = !collapse;
            btnExcelExportCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseAddInfo(bool collapse)
        {
            Session["AddInfoCollapsed"] = collapse;
            DivAddInfoCollapsable.Visible = !collapse;
            btnAddInfoCollapse.Text = collapse ? "◄" : "▼";
        }

        protected void BtnMarketingExport_OnClick(object sender, EventArgs e)
        {
            IEnumerable<Project> projectsToExport = null;
            //if (radioProjectStart.SelectedValue == "StartingProjects") //Projects which start in this Sem.
            if (SelectedSemester.SelectedValue == "") //Alle Semester
            {
                projectsToExport = db.Projects
                    .Where(i => (i.State == ProjectState.Published || i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderBy(i => i.Semester.Name)
                    .ThenBy(i => i.Department.DepartmentName)
                    .ThenBy(i => i.ProjectNr);
            }
            else
            {
                var semesterId = int.Parse(SelectedSemester.SelectedValue);
                projectsToExport = db.Projects
                    .Where(i => i.SemesterId == semesterId
                             && (i.State == ProjectState.Published || i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderBy(i => i.Semester.Name)
                    .ThenBy(i => i.Department.DepartmentName)
                    .ThenBy(i => i.ProjectNr);
            }
            /*else if (radioProjectStart.SelectedValue == "EndingProjects") //Projects which end in this Sem.
                if (SelectedSemester.SelectedValue == "") //Alle Semester
                {
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.IsMainVersion && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != "")
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
                else
                {
                    var selectedSemester = db.Semester.Single(s => s.Id == int.Parse(SelectedSemester.SelectedValue));
                    var previousSemester = db.Semester.OrderByDescending(s => s.StartDate)
                        .FirstOrDefault(s => s.StartDate < selectedSemester.StartDate);
                    projectsToExport = db.Projects
                        .Where(i => i.State == ProjectState.Published && i.IsMainVersion && i.LogStudent1Mail != null &&
                                    i.LogStudent1Mail != ""
                                    && (i.LogProjectDuration == 1 && i.Semester == selectedSemester ||
                                        i.LogProjectDuration == 2 && i.Semester == previousSemester))
                        .OrderBy(i => i.Semester.Name)
                        .ThenBy(i => i.Department.DepartmentName)
                        .ThenBy(i => i.ProjectNr);
                }
            else
                throw new Exception($"Unexpected selection: {radioProjectStart.SelectedValue}");*/

            //Response
            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition",
                $"attachment; filename={SelectedSemester.SelectedItem.Text.Replace(" ", "_")}_IP56_Informatikprojekte.xlsx");


            ExcelCreator.GenerateMarketingList(Response.OutputStream, projectsToExport, db,
            SelectedSemester.SelectedItem.Text);
            Response.End();
        }

        protected void RadioSelectedProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
            CheckProjects.DataSource = GetSelectedProjects();
            CheckProjects.DataBind();

        }

        protected void CheckProjects_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var project = db.Projects.Single(item => item.Id == ((ProjectSingleElement)e.Row.DataItem).id);

            if (!project.UserCanEdit())
            {
                var x = e.Row.Cells[e.Row.Cells.Count - 4].Controls;
                e.Row.Cells[e.Row.Cells.Count - 3].Controls.OfType<DataBoundLiteralControl>().First().Visible = false; //edit
                e.Row.Cells[e.Row.Cells.Count - 2].Controls.OfType<LinkButton>().First().Visible = false; //delete
            }

            //TODO: decide wether to keep this button or not
            e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = false; //submit

            Color col = ColorTranslator.FromHtml(project.StateColor);
            foreach (TableCell cell in e.Row.Cells)
                cell.BackColor = col;
        }

        protected void BtnAdminProjectsCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAdminProjects(!(bool)Session["AdminProjectCollapsed"]);
        }

        protected void BtnExcelExportCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseExcelExport(!(bool)Session["ExcelExportCollapsed"]);
        }

        protected void BtnAddInfoCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAddInfo(!(bool)Session["AddInfoCollapsed"]);
        }

        /*
        protected void EinreichenButton_Click(int id)
        {
            Project project = db.Projects.Single(p => p.Id == id);
            var validationMessage = project.GenerateValidationMessage();

            if (validationMessage != null && validationMessage != "")
            {
                var sb = new StringBuilder();
                sb.Append("alert('");
                sb.Append(validationMessage);
                sb.Append("');");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(),
                    "key", sb.ToString(), true);
            }
            else
            {
                project.Submit(db);
                db.SubmitChanges();
                Response.Redirect("projectlist");
            }

        }
        */

        protected void BtnBillingExport_Click(object sender, EventArgs e)
        {
            IEnumerable<Project> projectsToExport = null;

            if (SelectedSemester.SelectedValue == "") //Alle Semester
            {
                projectsToExport = db.Projects
                    .Where(i => (i.State == ProjectState.Published || i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderByDescending(i => i.BillingStatus.Billable)
                    .ThenBy(i => i.ClientCompany)
                    .ThenBy(i => i.ClientPerson);
            }
            else
            {
                var semesterId = int.Parse(SelectedSemester.SelectedValue);
                projectsToExport = db.Projects
                    .Where(i => i.SemesterId == semesterId
                             && (i.State == ProjectState.Published || i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderByDescending(i => i.BillingStatus.Billable)
                    .ThenBy(i => i.ClientCompany)
                    .ThenBy(i => i.ClientPerson);
            }
            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition",
                $"attachment; filename={SelectedSemester.SelectedItem.Text.Replace(" ", "_")}_Verrechnungs_Excel.xlsx");


            ExcelCreator.GenerateBillingList(Response.OutputStream, projectsToExport, db, SelectedSemester.SelectedItem.Text);
            Response.End();
        }
    }
}


