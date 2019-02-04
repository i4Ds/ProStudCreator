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


            TasksNextTaskCheck.InnerHtml = $"{TaskHandler.GetNextTaskCheck()}";
            TasksMarks.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 11).OrderBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)}</li>").ToArray());
            TasksExperts.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 9).OrderBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)}</li>").ToArray());
            TasksTitles.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 18).OrderBy(t => t.DueDate).Select(t => $"<li>Datum: {t.DueDate}</li>").ToArray());
            TasksProjects.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && (t.TaskTypeId == 1 || t.TaskTypeId == 2 || t.TaskTypeId == 6 || t.TaskTypeId == 8 || t.TaskTypeId == 16 || t.TaskTypeId == 17)).OrderBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Verantwortlich: {t.ResponsibleUser.Name} Projekt: {t.Project.Name} Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)} Task: {t.TaskType.Description}</li>").ToArray());

            ForceTaskCheckNow.Text = "Force Task Check Now!";

            Session["LastPage"] = "webadminpage";
        }

        protected void ForceTaskCheckNow_Click(object sender, EventArgs e)
        {
            TaskHandler.ForceCheckAllTasks();
            ForceTaskCheckNow.Enabled = true;
        }
    }
}


