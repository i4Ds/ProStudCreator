using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{

    public partial class WebAdminPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
            pl.db = db;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShibUser.IsWebAdmin())
            {
                Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.AbsolutePath}");
                Response.End();
            }
            
            pl.SetProjects(db.Projects.Where(p => p.IsMainVersion));

            if (!Page.IsPostBack)
            {
            }
            var lastSem = Semester.LastSemester(db);
            var currSem = Semester.CurrentSemester(db);

            LabelLastSem.Text = lastSem.Name;
            LabelCurrSem.Text = currSem.Name;

            TasksNextTaskCheck.InnerHtml = $"{TaskHandler.GetNextTaskCheck()}";
            LastTaskRuns.InnerHtml = String.Join("", db.TaskRuns.OrderByDescending(r => r.Date).Select(r => $"Date: {r.Date} | Forced: {r.Forced.ToString()}<br>").Take(10).ToArray());

            UpcomingPres.InnerHtml = String.Join("", db.Projects.Where(p => p.State == ProjectState.Ongoing && p.LogDefenceDate != null && p.LogDefenceDate > DateTime.Now).OrderBy(p => p.LogDefenceDate).Select(p => $"<li>{p.LogDefenceDate} - {p.LogDefenceRoom} - <a href=\"ProjectInfoPage?id={p.Id}\">{p.Name}</a></li>").ToArray());

            TasksMarks.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 11).OrderBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)}</li>").ToArray());
            TasksExperts.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 9).OrderBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Datum: {t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0)}</li>").ToArray());

            NoExpertTheses.InnerHtml = String.Join("", db.Projects.Where(p => p.LogProjectType.P6 && !p.LogProjectType.P5 && p.IsMainVersion && p.State >= ProjectState.Ongoing && p.State <= ProjectState.Finished && p.Expert == null).Select(p => $"<li><a href=\"ProjectInfoPage?id={p.Id}\">{p.GetProjectLabel()}</a> : {p.StateAsString}</li>").ToArray());

            (var etbp, var entbp, var msg) = TaskHandler.new_SendPayExperts(db);
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<Expert, List<Project>> entry in etbp)
            {
                sb.Append($"<li>{entry.Key.Name}<ul>");
                foreach (Project p in entry.Value)
                {
                    sb.Append($"<li><a href=\"ProjectInfoPage?id={p.Id}\">{p.GetProjectLabel()}</a> : {p.StateAsString}</li>");
                }
                sb.Append("</ul></li>");
            }
            ExpertsToBePaid.InnerHtml = sb.ToString();
            sb = new StringBuilder();
            foreach (KeyValuePair<Expert, List<Project>> entry in entbp)
            {
                sb.Append($"<li>{entry.Key.Name}<ul>");
                foreach (Project p in entry.Value)
                {
                    sb.Append($"<li><a href=\"ProjectInfoPage?id={p.Id}\">{p.GetProjectLabel()}</a> : {p.StateAsString}</li>");
                }
                sb.Append("</ul></li>");
            }
            ExpertsNotToBePaid.InnerHtml = sb.ToString();

            ExpertMailMessage.InnerHtml = msg;

            TasksTitles.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 18).OrderBy(t => t.DueDate).Select(t => $"<li>Datum: {t.DueDate}</li>").ToArray());
            TitleReminder2Weeks.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 20).OrderBy(t => t.DueDate).Select(t => $"<li>Datum: {t.DueDate}</li>").ToArray());
            TitleReminder2Days.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 21).OrderBy(t => t.DueDate).Select(t => $"<li>Datum: {t.DueDate}</li>").ToArray());

            LastSemIP5Normal.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == lastSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Status: {Project.GetStateAsString(t.Project.State)}<br/>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a><br/>Präsentation: {t.Project.LogDefenceDate}</li><br/>").ToArray());
            LastSemIP5Long.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == lastSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 2).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Status: {Project.GetStateAsString(t.Project.State)}<br/>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a><br/>Präsentation: {t.Project.LogDefenceDate}</li><br/>").ToArray());
            LastSemIP6.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == lastSem.Id && t.Project.LogProjectType.P6 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Status: {Project.GetStateAsString(t.Project.State)}<br/>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a><br/>Präsentation: {t.Project.LogDefenceDate}</li><br/>").ToArray());

            CurrSemIP5Normal.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == currSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Status: {Project.GetStateAsString(t.Project.State)}<br/>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a><br/>Präsentation: {t.Project.LogDefenceDate}</li><br/>").ToArray());
            CurrSemIP5Long.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == currSem.Id && t.Project.LogProjectType.P5 && t.Project.LogProjectDuration == 2).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Status: {Project.GetStateAsString(t.Project.State)}<br/>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a><br/>Präsentation: {t.Project.LogDefenceDate}</li><br/>").ToArray());
            CurrSemIP6.InnerHtml = String.Join("", db.Tasks.Where(t => t.Done == false && t.TaskTypeId == 19 && t.Project.Semester.Id == currSem.Id && t.Project.LogProjectType.P6 && t.Project.LogProjectDuration == 1).OrderBy(t => t.DueDate).ThenBy(t => t.ResponsibleUser.Name).ThenBy(t => t.Project.Name).ThenBy(t => (t.LastReminded + new TimeSpan(t.TaskType.DaysBetweenReminds, 0, 0, 0))).Select(t => $"<li>Status: {Project.GetStateAsString(t.Project.State)}<br/>DueDate: {t.DueDate}<br/>LastReminded: {t.LastReminded}<br/>Verantwortlich: {t.ResponsibleUser.Name}<br/>Projekt: <a href=\"ProjectInfoPage?id={t.Project.Id}\">{t.Project.Name}</a><br/>Präsentation: {t.Project.LogDefenceDate}</li><br/>").ToArray());

            // HttpContextContent.InnerHtml = System.Uri.UnescapeDataString(HttpContext.Current.Request.Headers.ToString()).Replace("&","<br><br>");

            ForceTaskCheckNow.Text = "Force Task Check Now!";
            SendExpertMailToWebAdmin.Text = "Send Mail To Webadmin";

            Session["LastPage"] = "webadminpage";
        }

        protected void ForceTaskCheckNow_Click(object sender, EventArgs e)
        {
            TaskHandler.ForceCheckAllTasks();
            ForceTaskCheckNow.Enabled = true;
            Response.Redirect("WebAdminPage.aspx");
        }

        protected void SendExpertMailToWebAdmin_Click(object sender, EventArgs e)
        {
            (_, _, var msg) = TaskHandler.new_SendPayExperts(db);

            var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
            mail.To.Add(new MailAddress(Global.WebAdmin));
            mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
            mail.IsBodyHtml = true;
            mail.Body = msg;
            TaskHandler.SendMail(mail);
        }
    }
}
