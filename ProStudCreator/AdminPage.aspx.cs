using Microsoft.Ajax.Utilities;
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

            ProjectGrid.db = db;
            UserList.db = db;
            ExpertList.db = db;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShibUser.CanVisitAdminPage())
            {
                Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.PathAndQuery}");
                Response.End();
            }

            DivAdminProjects.Visible = ShibUser.CanPublishProject() || ShibUser.CanSeeAllProjectsInProgress();
            DivAdminUsers.Visible = ShibUser.IsWebAdmin() || ShibUser.IsDepartmentManager();
            DivAdminExperts.Visible = ShibUser.IsWebAdmin() || ShibUser.IsDepartmentManager();
            DivExcelExport.Visible = ShibUser.CanExportExcel();
            btnGradeExport.Visible = ShibUser.CanExportExcel();


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

                if (Session["AdminUsersCollapsed"] == null)
                    CollapseAdminUsers(true);
                else
                    CollapseAdminUsers((bool)Session["AdminUsersCollapsed"]);

                if (Session["AdminExpertsCollapsed"] == null)
                    CollapseAdminExperts(true);
                else
                    CollapseAdminExperts((bool)Session["AdminExpertsCollapsed"]);

                if (Session["AddInfoCollapsed"] == null)
                    CollapseAddInfo(true);
                else
                    CollapseAddInfo((bool)Session["AddInfoCollapsed"]);

            }

            ProjectGrid.SetProjects(GetSelectedProjects());

            if (ShibUser.IsWebAdmin())
            {
                UserList.SetUsers(db.UserDepartmentMap.Where(u => true));
            }
            else if (ShibUser.IsDepartmentManager())
            {
                UserList.SetUsers(db.UserDepartmentMap.Where(u => u.Department == ShibUser.GetDepartment()));
            }

            if (ShibUser.IsWebAdmin() || ShibUser.IsDepartmentManager())
            {
                ExpertList.SetExperts(db.Experts.Where(ex => true));
            }

            gvDates.DataSource = CalculateDates();
            gvDates.DataBind();

            //----------- wozu?
            //GVTasks.DataSource = AllTasks();
            //GVTasks.DataBind();


            Session["LastPage"] = "adminpage";
        }

        private IEnumerable<object> CalculateDates()
        {
            for (var year = DateTime.Now.Year; year <= DateTime.Now.Year + 3; year++)
            {
                yield return new { Name = $"{year % 100:D2}FS", StartDate = Semester.StartOfWeek(year, 8).ToString("yyyy-MM-dd"), EndDate = Semester.StartOfWeek(year, 24).AddDays(5).ToString("yyyy-MM-dd"), SubmissionUntil = Semester.StartOfWeek(year - 1, 47).AddDays(2).ToString("yyyy-MM-dd"), ProjectAllocation = $"Ende 01.{year}", SubmissionIP5 = Semester.StartOfWeek(year, 24).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP5Lang = Semester.StartOfWeek(year, 33).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP6 = Semester.StartOfWeek(year, 33).AddDays(4).ToString("dd.MM.yyyy"), DefenseStart = Semester.StartOfWeek(year, 36).ToString("dd.MM.yyyy"), DefenseEnd = Semester.StartOfWeek(year, 37).AddDays(4).ToString("dd.MM.yyyy"), Exhibition = "?", DayBeforeNext = Semester.StartOfWeek(year, 24).AddDays(6).ToString("yyyy-MM-dd") };

                yield return new { Name = $"{year % 100:D2}HS", StartDate = Semester.StartOfWeek(year, 38).ToString("yyyy-MM-dd"), EndDate = Semester.StartOfWeek(year + 1, 3).AddDays(5).ToString("yyyy-MM-dd"), SubmissionUntil = Semester.StartOfWeek(year, 21).AddDays(2).ToString("yyyy-MM-dd"), ProjectAllocation = $"Anfang 07.{year}", SubmissionIP5 = Semester.StartOfWeek(year + 1, 3).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP5Lang = Semester.StartOfWeek(year + 1, 12).AddDays(4).ToString("dd.MM.yyyy"), SubmissionIP6 = Semester.StartOfWeek(year + 1, 12).AddDays(4).ToString("dd.MM.yyyy"), DefenseStart = Semester.StartOfWeek(year + 1, 16).ToString("dd.MM.yyyy"), DefenseEnd = Semester.StartOfWeek(year + 1, 17).AddDays(4).ToString("dd.MM.yyyy"), Exhibition = "keine", DayBeforeNext = Semester.StartOfWeek(year + 1, 3).AddDays(6).ToString("yyyy-MM-dd") };
            }
        }

        private IQueryable<Project> GetSelectedProjects()
        {
            var depId = ShibUser.GetDepartment(db).Id;

            switch(radioSelectedProjects.SelectedValue)
            {
                case "inProgress":
                    return db.Projects.Where(p => p.IsMainVersion
                                               && (p.State == ProjectState.InProgress || p.State == ProjectState.Submitted || p.State == ProjectState.Rejected))
                                      .OrderByDescending(i => i.ModificationDate);
                case "toPublish":
                    return db.Projects.Where(p => p.State == ProjectState.Submitted
                                               && p.IsMainVersion
                                               && p.DepartmentId == depId)
                                      .OrderBy(i => i.Advisor1.Mail)
                                      .ThenBy(i => i.ProjectNr);
                case "allProjects":
                    return db.Projects.Where(p => p.State != ProjectState.Deleted
                                               && p.IsMainVersion)
                                      .OrderBy(i => i.DepartmentId)
                                      .ThenBy(i => i.State)
                                      .ThenBy(i => i.ProjectNr);
                default:
                    throw new Exception($"Unexpected radioSelectedProjects.SelectedValue '{radioSelectedProjects.SelectedValue}'");
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

        private void CollapseAdminUsers(bool collapse)
        {
            Session["AdminUsersCollapsed"] = collapse;
            DivAdminUsersCollapsable.Visible = !collapse;
            BtnAdminUsersCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseAdminExperts(bool collapse)
        {
            Session["AdminExpertsCollapsed"] = collapse;
            DivAdminExpertsCollapsable.Visible = !collapse;
            BtnAdminExpertsCollapse.Text = collapse ? "◄" : "▼";
        }

        private void CollapseAddInfo(bool collapse)
        {
            Session["AddInfoCollapsed"] = collapse;
            DivAddInfoCollapsable.Visible = !collapse;
            btnAddInfoCollapse.Text = collapse ? "◄" : "▼";
        }

        protected void RadioSelectedProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedAdminProjects"] = radioSelectedProjects.SelectedIndex;
            ProjectGrid.SetProjects(GetSelectedProjects());
        }

        protected void BtnAdminProjectsCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAdminProjects(!(bool)Session["AdminProjectCollapsed"]);
        }

        protected void BtnAdminUsersCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAdminUsers(!(bool)Session["AdminUsersCollapsed"]);
        }

        protected void BtnAdminExpertsCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseAdminExperts(!(bool)Session["AdminExpertsCollapsed"]);
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

        protected void NewUser_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserEditPage");
        }

        protected void NewExpert_Click(object sender, EventArgs e)
        {
            Response.Redirect("ExpertEditPage");
        }

        protected IEnumerable<Project> GetSelectedExcelExportProjects()
        {
            IEnumerable<Project> projectsForExcelExport = null;

            // Semester
            if (SelectedSemester.SelectedValue == "") // Alle Semester
            {
                projectsForExcelExport = db.Projects.Where(p => p.IsMainVersion);
            }
            else
            {
                var semesterId = int.Parse(SelectedSemester.SelectedValue);
                projectsForExcelExport = db.Projects.Where(p => p.IsMainVersion && p.SemesterId == semesterId);
            }

            // Study Course
            if (SelectedStudyCourse.SelectedValue == "all")
            {
                projectsForExcelExport = projectsForExcelExport.Where(p => p.LogStudyCourse != null);
            }
            else if (SelectedStudyCourse.SelectedValue == "cs")
            {
                projectsForExcelExport = projectsForExcelExport.Where(p => p.LogStudyCourse == 1);
            }
            else if (SelectedStudyCourse.SelectedValue == "ds")
            {
                projectsForExcelExport = projectsForExcelExport.Where(p => p.LogStudyCourse == 2);
            }
            else
            {
                throw new Exception("Unknown study course");
            }

            return projectsForExcelExport;
        }

        protected void BtnGradeExport_Click(object sender, EventArgs e)
        {

            var projectsToExport = GetSelectedExcelExportProjects()
                    .Where(p => (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.Canceled || p.State == ProjectState.ArchivedFinished || p.State == ProjectState.ArchivedCanceled))
                    .OrderByDescending(p => p.Department)
                    .ThenBy(p => p.ProjectNr);
            
            /*
            IEnumerable<Project> projectsToExport = null;

            if (SelectedSemester.SelectedValue == "") //Alle Semester
            {
                projectsToExport = db.Projects
                    .Where(i => (i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderByDescending(i => i.Department)
                    .ThenBy(i => i.ProjectNr);
            }
            else
            {
                var semesterId = int.Parse(SelectedSemester.SelectedValue);
                projectsToExport = db.Projects
                    .Where(i => i.SemesterId == semesterId
                             && (i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderByDescending(i => i.Department.Id)
                    .ThenBy(i => i.ProjectNr);
            }
            */
            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition",
                $"attachment; filename={SelectedSemester.SelectedItem.Text.Replace(" ", "_")}_Noten_Excel.xlsx");

            ExcelCreator.GenerateGradeExcel(Response.OutputStream, projectsToExport, db);
            Response.End();
        }

        protected void BtnBillingExport_Click(object sender, EventArgs e)
        {
            IEnumerable<Project> projectsToExport = null;

            if (SelectedSemester.SelectedValue == "") //Alle Semester
            {
                projectsToExport = db.Projects
                    .Where(i => (i.State == ProjectState.Published || i.State == ProjectState.Ongoing || i.State == ProjectState.Finished || i.State == ProjectState.Canceled || i.State == ProjectState.ArchivedFinished || i.State == ProjectState.ArchivedCanceled)
                             && i.IsMainVersion)
                    .OrderByDescending(i => i.State > ProjectState.Ongoing)
                    .ThenByDescending(i => i.BillingStatus.Billable)
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
                    .OrderByDescending(i => i.State > ProjectState.Ongoing)
                    .ThenByDescending(i => i.BillingStatus.Billable)
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
            Response.Clear();
            Response.ContentType = "application/Excel";
            Response.AddHeader("content-disposition",
                $"attachment; filename={SelectedSemester.SelectedItem.Text.Replace(" ", "_")}_IP56_Informatikprojekte.xlsx");


            ExcelCreator.GenerateMarketingList(Response.OutputStream, projectsToExport, db,
            SelectedSemester.SelectedItem.Text);
            Response.End();
        }
    }
}


