using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{

    public partial class WebAdminPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShibUser.GetEmail() != Global.WebAdmin)
            {
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
            }

            if (!Page.IsPostBack)
            {
            }
            var lastSem = Semester.LastSemester(db);
            var currSem = Semester.CurrentSemester(db);

            LabelLastSem.Text = lastSem.Name;
            LabelCurrSem.Text = currSem.Name;

            TasksNextTaskCheck.InnerHtml = $"{TaskHandler.GetNextTaskCheck()}";
            TasksMarks.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 11).OrderBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)}</li>").ToArray());
            TasksExperts.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 9).OrderBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)}</li>").ToArray());
            TasksTitles.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 18).OrderBy(t => t.DueDate).Select(t => $"<li>Datum: {t.DueDate}</li>").ToArray());

            LastSemIP5Normal.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == lastSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a></li><br/>").ToArray());
            LastSemIP5Long.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == lastSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 2).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a></li><br/>").ToArray());
            LastSemIP6.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == lastSem.Id && t.Project.LogProjectType.P6 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a></li><br/>").ToArray());

            CurrSemIP5Normal.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == currSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a></li><br/>").ToArray());
            CurrSemIP5Long.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == currSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 2).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a></li><br/>").ToArray());
            CurrSemIP6.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == currSem.Id && t.Project.LogProjectType.P6 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a></li><br/>").ToArray());


            ForceTaskCheckNow.Text = "Force Task Check Now!";

            Session["LastPage"] = "webadminpage";
        }

        protected void ForceTaskCheckNow_Click(object sender, EventArgs e)
        {
            TaskHandler.ForceCheckAllTasks();
            ForceTaskCheckNow.Enabled = true;
            Response.Redirect("WebAdminPage.aspx");
        }
    }
}


