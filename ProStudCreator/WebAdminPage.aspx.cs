﻿using System;
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
            if (!(ShibUser.IsWebAdmin() || ShibUser.IsProjectOwner()))
            {
                Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.PathAndQuery}");
                Response.End();
            }

            if (Page.IsPostBack)
            {
                return;
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

            var (expertsToBePaid, expertsNotToBePaid) = TaskHandler.getAllPayExperts(db);

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<Expert, List<Project>> entry in expertsToBePaid)
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
            foreach (KeyValuePair<Expert, List<Project>> entry in expertsNotToBePaid)
            {
                sb.Append($"<li>{entry.Key.Name}<ul>");
                foreach (Project p in entry.Value)
                {
                    sb.Append($"<li><a href=\"ProjectInfoPage?id={p.Id}\">{p.GetProjectLabel()}</a> : {p.StateAsString}</li>");
                }
                sb.Append("</ul></li>");
            }
            ExpertsNotToBePaid.InnerHtml = sb.ToString();

            var mailMessage = TaskHandler.createPayExpertsString(db, expertsToBePaid, expertsNotToBePaid, true, false);
            ExpertMailMessage.InnerHtml = mailMessage;

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

            var sbSemDates = new StringBuilder();
            var currYear = int.Parse("20" + currSem.Name.Substring(0, 2));
            for (int y = currYear; y < currYear + 5; y++)
            {
                sbSemDates.Append($"<li>{y}<ul>");
                // FS
                sbSemDates.Append("<li>FS<ul>");

                // Semester Start
                var fsSemStart = Semester.StartOfWeek(y, 8);
                sbSemDates.Append($"<li>Semester Start: {fsSemStart:d}</li>");

                // Semester End
                var fsSemEnd = Semester.StartOfWeek(y, 24).AddDays(5);
                sbSemDates.Append($"<li>Semester End: {fsSemEnd:d}</li>");

                // Projekt Einreichung (extern)
                var fsProjSubExt = Semester.StartOfWeek(y - 1, 41).AddDays(2);
                sbSemDates.Append($"<li>Projekt Einreichung (extern): {fsProjSubExt:d}</li>");

                // Projekt Einreichung (intern)
                var fsProjSubInt = Semester.StartOfWeek(y - 1, 47).AddDays(2);
                sbSemDates.Append($"<li>Projekt Einreichung (intern): {fsProjSubInt:d}</li>");

                // Infoveranstaltung / Start ProApp
                var fsInfoEvent = Semester.StartOfWeek(y - 1, 50);
                sbSemDates.Append($"<li>Infoveranstaltung / Start ProApp: {fsInfoEvent:d} 11:15</li>");

                // Anmeldeschluss ProApp
                var fsProAppApp = Semester.StartOfWeek(y, 2).AddDays(4);
                sbSemDates.Append($"<li>Anmeldeschluss ProApp: {fsProAppApp:d}</li>");

                // Projekt Zuteilung
                var fsProjAlloc = $"Ende 01.{y}";
                sbSemDates.Append($"<li>Projekt Zuteilung: {fsProjAlloc}</li>");

                // Abgabe IP5
                var fsSubIP5Norm = Semester.StartOfWeek(y, 24).AddDays(4);
                sbSemDates.Append($"<li>Abgabe IP5: {fsSubIP5Norm:d}</li>");

                // Notenabgabe IP5
                var fsGradeIP5 = Semester.StartOfWeek(y, 30).AddDays(4);
                sbSemDates.Append($"<li>Notenabgabe IP5: {fsGradeIP5:d}</li>");

                // Abgabe IP5 (lang) / IP6
                var fsSubIP6Norm = Semester.StartOfWeek(y, 33).AddDays(4);
                sbSemDates.Append($"<li>Abgabe IP5 (long) / IP6: {fsSubIP6Norm:d}</li>");

                // Ausstellung Thesis
                var fsExhibThesis = Semester.StartOfWeek(y, 33).AddDays(4);
                sbSemDates.Append($"<li>Ausstellung Thesis: {fsExhibThesis:d}</li>");

                // Verteidigung Start
                var fsDefenseStart = Semester.StartOfWeek(y, 36);
                sbSemDates.Append($"<li>Verteidigung Start: {fsDefenseStart:d}</li>");

                // Verteidigung End
                var fsDefenseEnd = Semester.StartOfWeek(y, 37).AddDays(4);
                sbSemDates.Append($"<li>Verteidigung End: {fsDefenseEnd:d}</li>");

                // Notenabgabe IP5 (lang) / IP6
                var fsGradeIP6 = Semester.StartOfWeek(y, 38);
                sbSemDates.Append($"<li>Notenabgabe IP5 (lang) / IP6: {fsGradeIP6:d}</li>");

                sbSemDates.Append("</ul></li>");

                // HS
                sbSemDates.Append("<li>HS<ul>");

                // Semester Start
                var hsSemStart = Semester.StartOfWeek(y, 38);
                sbSemDates.Append($"<li>Semester Start: {hsSemStart:d}</li>");

                // Semester End
                var hsSemEnd = Semester.StartOfWeek(y + 1, 3).AddDays(5);
                sbSemDates.Append($"<li>Semester End: {hsSemEnd:d}</li>");

                // Projekt Einreichung (extern)
                var hsProjSubExt = Semester.StartOfWeek(y, 15).AddDays(2);
                sbSemDates.Append($"<li>Projekt Einreichung (extern): {hsProjSubExt:d}</li>");

                // Projekt Einreichung (intern)
                var hsProjSubInt = Semester.StartOfWeek(y, 21).AddDays(2);
                sbSemDates.Append($"<li>Projekt Einreichung (intern): {hsProjSubInt:d}</li>");

                // Infoveranstaltung / Start ProApp
                var hsInfoEvent = Semester.StartOfWeek(y, 24);
                sbSemDates.Append($"<li>Infoveranstaltung / Start ProApp: {hsInfoEvent:d} 11:15</li>");

                // Anmeldeschluss ProApp
                var hsProAppApp = Semester.StartOfWeek(y, 26).AddDays(4);
                sbSemDates.Append($"<li>Anmeldeschluss ProApp: {hsProAppApp:d}</li>");

                // Projekt Zuteilung
                var hsProjAlloc = $"Anfang 07.{y}";
                sbSemDates.Append($"<li>Projekt Zuteilung: {hsProjAlloc}</li>");

                // Abgabe IP5
                var hsSubIP5Norm = Semester.StartOfWeek(y + 1, 3).AddDays(4);
                sbSemDates.Append($"<li>Abgabe IP5: {hsSubIP5Norm:d}</li>");

                // Notenabgabe IP5
                var hsGradeIP5 = Semester.StartOfWeek(y + 1, 7).AddDays(4);
                sbSemDates.Append($"<li>Notenabgabe IP5: {hsGradeIP5:d}</li>");

                // Abgabe IP5 (lang) / IP6
                var hsSubIP6Norm = Semester.StartOfWeek(y + 1, 12).AddDays(4);
                sbSemDates.Append($"<li>Abgabe IP5 (long) / IP6: {hsSubIP6Norm:d}</li>");

                // Ausstellung Thesis
                var hsExhibThesis = "keine";
                sbSemDates.Append($"<li>Ausstellung Thesis: {hsExhibThesis:d}</li>");

                // Verteidigung Start
                var hsDefenseStart = Semester.StartOfWeek(y + 1, 16);
                sbSemDates.Append($"<li>Verteidigung Start: {hsDefenseStart:d}</li>");

                // Verteidigung End
                var hsDefenseEnd = Semester.StartOfWeek(y + 1, 17).AddDays(4);
                sbSemDates.Append($"<li>Verteidigung End: {hsDefenseEnd:d}</li>");

                // Notenabgabe IP5 (lang) / IP6
                var hsGradeIP6 = Semester.StartOfWeek(y + 1, 18);
                sbSemDates.Append($"<li>Notenabgabe IP5 (lang) / IP6: {hsGradeIP6:d}</li>");

                sbSemDates.Append("</ul></li>");

                sbSemDates.Append("</ul></li>");
            }
            SemesterData.InnerHtml = sbSemDates.ToString();

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
            var (expertsToBePaid, expertsNotToBePaid) = TaskHandler.getAllPayExperts(db);
            var mailMessage = TaskHandler.createPayExpertsString(db, expertsToBePaid, expertsNotToBePaid, true, false);

            var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
            mail.To.Add(new MailAddress(Global.WebAdmin));
            mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
            mail.IsBodyHtml = true;
            mail.Body = mailMessage;
            TaskHandler.SendMail(mail);
        }
    }
}
