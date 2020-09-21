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
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShibUser.IsWebAdmin())
            {
                Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.AbsolutePath}");
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

            /*
            var semesterIDS = new List<int>() { 2,6,7,10,17,18,23,28 };
            var sbp5 = new StringBuilder();
            var sbp6 = new StringBuilder();

            foreach (var user in db.UserDepartmentMap.Where(u => u.CanBeAdvisor1).OrderBy(u => u.DepartmentId))
            {
                sbp5.Append($"<li>{user.Name},");
                sbp6.Append($"<li>{user.Name},");

                foreach (var sem in semesterIDS)
                {
                    var p5grades = new List<float>();
                    var p6grades = new List<float>();

                    foreach (var proj in db.Projects.Where(p => 
                        p.IsMainVersion 
                        && (p.State == ProjectState.Finished || p.State == ProjectState.ArchivedFinished) 
                        && p.SemesterId == sem 
                        && p.Advisor1Id == user.Id
                    ))
                    {
                        if (proj.LogGradeStudent1.HasValue)
                        {
                            if (proj.LogProjectTypeID == 1)
                                p5grades.Add(proj.LogGradeStudent1.Value);
                            else if (proj.LogProjectTypeID == 2)
                                p6grades.Add(proj.LogGradeStudent1.Value);
                        }
                        if (proj.LogGradeStudent2.HasValue)
                        {
                            if (proj.LogProjectTypeID == 1)
                                p5grades.Add(proj.LogGradeStudent2.Value);
                            else if (proj.LogProjectTypeID == 2)
                                p6grades.Add(proj.LogGradeStudent2.Value);
                        }
                    }

                    if (p5grades.Any())
                        sbp5.Append($"{p5grades.Average()}".Replace(',','.'));
                    if (p6grades.Any())
                        sbp6.Append($"{p6grades.Average()}".Replace(',', '.'));

                    sbp5.Append(",");
                    sbp6.Append(",");
                }

                sbp5.Append("</li>");
                sbp6.Append("</li>");
            }

            P5Grades.InnerHtml = sbp5.ToString();
            P6Grades.InnerHtml = sbp6.ToString();
            */

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
