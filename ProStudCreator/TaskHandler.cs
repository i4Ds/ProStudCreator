using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web;
using NPOI.Util;

namespace ProStudCreator
{
    public class TaskHandler
    {
        enum Type : int
        {
            RegisterGrades = 1,
            CheckWebsummary = 2,
            StartProject = 4,
            FinishProject = 5,
            UploadResults = 6,
            PlanDefenses = 7, //TODO
            UpdateDefenseDates = 8, //TODO
            PayExperts = 9,
            InsertNewSemesters = 10,
            SendGrades = 11,
            SendMarKomBrochure = 12,
            InvoiceCustomers = 13, //TODO
            EnterAssignedStudents = 14,
            DoubleCheckMarKomBrochureData = 15,
            CheckBillingStatus = 16,
            SetProjectLanguage = 17,
            SendThesisTitles = 18
        }

        private static readonly object TaskCheckLock = new object();
#if !DEBUG
        private static DateTime NextTaskCheck = DateTime.Today.AddHours(19);
#else
        private static DateTime NextTaskCheck = DateTime.Today.AddHours(-25);
#endif

        public static DateTime GetNextTaskCheck()
        {
            DateTime tmp = NextTaskCheck;
            return tmp;
        }

        public static void CheckAllTasks()
        {
            //protect against reentrancy-problems. this lock is enough as long as the method runs in less than 24h
            lock (TaskCheckLock)
            {
                var now = DateTime.Now;
                if (now < NextTaskCheck)
                    return;

                NextTaskCheck = now.Date.AddHours(24 + 19);

                RunAllTasks(false);
            }
        }

        public static void ForceCheckAllTasks()
        {
            if (Monitor.TryEnter(TaskCheckLock))
            {
                try
                {
                    RunAllTasks(true);
                }
                finally
                {
                    Monitor.Exit(TaskCheckLock);
                }
            }
        }

        private static void RunAllTasks(bool forced) //register all Methods which check for tasks here.
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                CheckGradesRegistered(db);
                CheckWebsummaryChecked(db);
                CheckBillingStatus(db);
                //CheckUploadResults(db);
                CheckLanguageSet(db);

                InfoStartProject(db);
                InfoFinishProject(db);

                InfoInsertNewSemesters(db);

                EnterAssignedStudents(db);

                SendThesisTitlesToAdmin(db);
                SendGradesToAdmin(db);
                SendPayExperts(db);

                //vvvvvvvvvvvvv NOT YET IMPLEMENTED
                //SendInvoiceCustomers(db); //<-- not yet implemented

                SendDoubleCheckMarKomBrochureData(db);
                SendMarKomBrochure(db);

                SendMailsToResponsibleUsers(db);
                SendMailToWebAdmin(forced);
            }
        }




        private static void EnterAssignedStudents(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.EnterAssignedStudents);

            var cutoffDate = new DateTime(2018, 1, 1);

            //add new tasks for projects
            var allActiveAssignStudentsTasks = db.Tasks.Where(t => !t.Done && t.TaskType == type).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && (p.LogStudent1Mail == null || p.LogStudent1Name == null || p.LogProjectDuration == null || p.LogProjectType == null) && p.Semester.StartDate <= DateTime.Now && p.Semester.StartDate >= cutoffDate).ToList();

            foreach (var project in allPublishedProjects)
                if (!allActiveAssignStudentsTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department),
                        FirstReminded = DateTime.Now,
                        TaskType = type,
                        Supervisor = db.UserDepartmentMap.Single(i => i.Mail == Global.WebAdmin)
                    });

            //mark registered tasks as done
            var openAssignStudentsTasks = db.Tasks.Where(i => !i.Done && i.TaskType == type).ToList();
            foreach (var openTask in openAssignStudentsTasks)
                if ((!string.IsNullOrEmpty(openTask.Project.LogStudent1Mail) && !string.IsNullOrEmpty(openTask.Project.LogStudent1Name) && openTask.Project.LogProjectDuration != null && openTask.Project.LogProjectType != null) || openTask.Project.State != ProjectState.Published)
                    openTask.Done = true;

            db.SubmitChanges();
        }





        //public static void SendInvoiceCustomers(ProStudentCreatorDBDataContext db)
        //{
        //    var type = db.TaskTypes.Single(t => t.Type == (int)Type.InvoiceCustomers);

        //    var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && && t.TaskType == type);
        //    if (activeTask == null)
        //    {
        //        activeTask = new Task()
        //        {
        //            TaskType = type,
        //        };
        //        db.Tasks.InsertOnSubmit(activeTask);
        //        db.SubmitChanges();
        //    }

        //    if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).Ticks > type.TicksBetweenReminds)
        //    {
        //        activeTask.LastReminded = DateTime.Now;

        //        var unpaidExperts = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && p.WebSummaryChecked && !p.LogExpertPaid && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) && p.BillingStatus != null && p.Expert != null).OrderBy(p => p.Expert.Name).ThenBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr).ToList();

        //        unpaidExperts = unpaidExperts.Where(p => p.WasDefenseHeld()).ToList();
        //        if (unpaidExperts.Any())
        //            using (var smtpClient = new SmtpClient())
        //            {
        //                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
        //                mail.To.Add(new MailAddress(Global.PayExpertAdmin));
        //                mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
        //                mail.IsBodyHtml = true;

        //                var mailMessage = new StringBuilder();
        //                mailMessage.Append(
        //                    "<div style=\"font-family: Arial\">" +
        //                    "<p>Liebe Administration<p>" +
        //                    "<p>Bitte die Auszahlung von den folgenden Expertenhonoraren veranlassen:</p>" +
        //                    "<table>" +
        //                    "<tr>" +
        //                        "<th>Experte</th>" +
        //                        "<th>Semester</th>" +
        //                        "<th>Studierende</th>" +
        //                        "<th>Betreuer</th>" +
        //                        "<th>Projekttitel</th>" +
        //                    "</tr>");

        //                foreach (var p in unpaidExperts)
        //                {
        //                    p.LogExpertPaid = true;

        //                    mailMessage.Append(
        //                    "<tr>" +
        //                        $"<td>{p.Expert.Name}</td>" +
        //                        $"<td>{p.Semester.Name}</td>" +
        //                        $"<td>{p.LogStudent1Mail + (p.LogStudent2Mail != null ? ", " + p.LogStudent2Mail : "")}</td>" +
        //                        $"<td>{p.Advisor1.Mail}</td>" +
        //                        $"<td>{p.GetFullTitle()}</td>" +
        //                    "</tr>"
        //                    );
        //                }

        //                mailMessage.Append(
        //                    "</table>" +
        //                    "<br/>" +
        //                    "<p>Herzliche Grüsse,<br/>" +
        //                    "ProStud-Team</p>" +
        //                    $"<p>Feedback an {Global.WebAdmin}</p>" +
        //                    "</div>"
        //                    );

        //                mail.Body = mailMessage.ToString();
        //                smtpClient.Send(mail);
        //            }
        //    }

        //    db.SubmitChanges();
        //}




        public static void SendMarKomBrochure(ProStudentCreatorDBDataContext db)
        {
            var lastSendDate = new DateTime(DateTime.Now.Year, 6, 1);
            if (DateTime.Now < lastSendDate)
                lastSendDate = lastSendDate.AddYears(-1);

            var type = db.TaskTypes.Single(t => t.Id == (int)Type.SendMarKomBrochure);
            if (db.Tasks.Any(t => t.Done && t.TaskType == type && t.LastReminded >= lastSendDate))
                return;


            var activeTask = new Task()
            {
                TaskType = type,
                LastReminded = DateTime.Now,
                FirstReminded = DateTime.Now,
            };
            db.Tasks.InsertOnSubmit(activeTask);
            db.SubmitChanges();

            var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
            mail.To.Add(new MailAddress(Global.MarKomAdmin));
            mail.Subject = "Informatikprojekte P6: Projektliste für Broschüre";
            mail.IsBodyHtml = true;

            var mailMessage = new StringBuilder();
            mailMessage.Append(
                "<div style=\"font-family: Arial\">" +
                "<p>Liebes MarKom<p>" +
                $"<p>Hier die Liste aller Informatik-Bachelorarbeiten im { Semester.ActiveSemester(lastSendDate, db).Name }:</p>" +
                "<table>" +
                "<tr>" +
                    "<th>Nachname</th>" +
                    "<th>Vorname</th>" +
                    "<th>Titel</th>" +
                    "<th>Unternehmen</th>" +
                    "<th>Unternehmensort</th>" +
                    "<th>Nummer</th>" +
                "</tr>");

            //FIXME: should consider project enddate, not startdate of semester
            var projs = db.Projects.Where(p => p.IsMainVersion && p.LogProjectType.P6 && p.Semester.StartDate <= lastSendDate && p.Semester.EndDate >= lastSendDate && p.State == (int)ProjectState.Published && !p.UnderNDA).ToArray();

            foreach (var p in projs.OrderBy(p => p.Student1LastName()))
                mailMessage.Append(
                "<tr>" +
                    $"<td>{HttpUtility.HtmlEncode(p.Student1LastName()) + (p.LogStudent2Name != null ? "<br/>" + HttpUtility.HtmlEncode(p.Student2LastName()) : "")}</td>" +
                    $"<td>{HttpUtility.HtmlEncode(p.Student1FirstName()) + (p.LogStudent2Name != null ? "<br/>" + HttpUtility.HtmlEncode(p.Student2FirstName()) : "")}</td>" +
                    $"<td>{HttpUtility.HtmlEncode(p.Name)}</td>" +
                    $"<td>{(p.ClientType == (int)ClientType.Company ? HttpUtility.HtmlEncode(p.ClientCompany) : "")}</td>" +
                    $"<td>{(p.ClientType == (int)ClientType.Company ? HttpUtility.HtmlEncode(p.ClientAddressCity) : "")}</td>" +
                    $"<td>{HttpUtility.HtmlEncode(p.GetFullNr())}</td>" +
                "</tr>"
                );

            mailMessage.Append(
                "</table>" +
                "<br/>" +
                "<p>Herzliche Grüsse,<br/>" +
                "ProStud-Team</p>" +
                $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                "</div>"
                );

            mail.Body = mailMessage.ToString();
            SendMail(mail);

            activeTask.Done = true;
            db.SubmitChanges();
        }








        public static void SendPayExperts(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.PayExperts);
            var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && t.TaskType == type);
            if (activeTask == null)
            {
                activeTask = new Task()
                {
                    TaskType = type,
                    FirstReminded = DateTime.Now
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
            {
                activeTask.LastReminded = DateTime.Now;

                var unpaidExperts = db.Projects.Where(p => p.IsMainVersion 
                    && p.State == (int)ProjectState.Published 
                    && !p.LogExpertPaid 
                    && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) 
                    && p.BillingStatus != null 
                    && p.Expert != null 
                    && p.BillingStatus.RequiresProjectResults
                ).OrderBy(p => p.Expert.Name).ThenBy(p => p.Semester.StartDate).ThenBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr).ToList();

                unpaidExperts = unpaidExperts.Where(p => p.WasDefenseHeld()).ToList();
                if (unpaidExperts.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.PayExpertAdmin));
                    mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Administration<p>" +
                        "<p>Bitte die Auszahlung von den folgenden Expertenhonoraren veranlassen:</p>" +
                        "<table>" +
                        "<tr>" +
                            "<th>Experte</th>" +
                            "<th>Semester</th>" +
                            "<th>Studierende</th>" +
                            "<th>Betreuer</th>" +
                            "<th>Projekttitel</th>" +
                        "</tr>");

                    foreach (var p in unpaidExperts)
                    {
                        p.LogExpertPaid = true;

                        mailMessage.Append(
                        "<tr>" +
                            $"<td>{HttpUtility.HtmlEncode(p.Expert.Name)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(p.Semester.Name)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(p.LogStudent1Mail + (p.LogStudent2Mail != null ? ", " + p.LogStudent2Mail : ""))}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(p.Advisor1.Mail)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(p.GetFullTitle())}</td>" +
                        "</tr>"
                        );
                    }

                    mailMessage.Append(
                        "</table>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>"
                        );

                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                }
            }

            db.SubmitChanges();
        }

        public static void SendGradesToAdmin(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.SendGrades);
            var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && t.TaskType == type);
            if (activeTask == null)
            {
                activeTask = new Task()
                {
                    TaskType = type,
                    FirstReminded = DateTime.Now
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
            {
                activeTask.LastReminded = DateTime.Now;

                var updatedProjects = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && !p.GradeSentToAdmin && (p.LogGradeStudent1 != null || p.LogGradeStudent2 != null) && p.BillingStatus != null && (p.LogLanguageEnglish == true || p.LogLanguageGerman == true));

                if (updatedProjects.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.GradeAdmin));
                    mail.Subject = "Informatikprojekte P5/P6: Neue Noten";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Ausbildungsadministration<p>" +
                        "<p>Es wurden neue Noten für die Informatikprojekte erfasst:</p>" +
                        "<table>" +
                        "<tr>" +
                            "<th>Mail</th>" +
                            "<th>Note</th>" +
                            "<th>Art</th>" +
                            "<th>Sprache</th>" +
                            "<th>Betreuer</th>" +
                            "<th>Projekttitel</th>" +
                        "</tr>");

                    var rowsList = new List<Tuple<string, string, string, string, string, string>>();

                    foreach (var p in updatedProjects)
                    {
                        p.GradeSentToAdmin = true;

                        if (p.LogStudent1Mail != null && p.LogGradeStudent1 != null)
                        {
                            rowsList.Add(new Tuple<string, string, string, string, string, string>(
                                p.LogStudent1Mail,
                                p.LogGradeStudent1.Value.ToString("F1").Replace(',', '.'),
                                p.LogProjectType.ExportValue,
                                (p.LogLanguageGerman.Value ? "Deutsch" : "Englisch"),
                                p.Advisor1.Mail,
                                p.GetFullTitle()
                            ));
                        }

                        if (p.LogStudent2Mail != null && p.LogGradeStudent2 != null)
                        {
                            rowsList.Add(new Tuple<string, string, string, string, string, string>(
                                p.LogStudent2Mail,
                                p.LogGradeStudent2.Value.ToString("F1").Replace(',', '.'),
                                p.LogProjectType.ExportValue,
                                (p.LogLanguageGerman.Value ? "Deutsch" : "Englisch"),
                                p.Advisor1.Mail,
                                p.GetFullTitle()
                            ));
                        }
                    }

                    foreach(var row in rowsList.OrderBy(r => r.Item1))
                    {
                        mailMessage.Append(
                        "<tr>" +
                            $"<td>{HttpUtility.HtmlEncode(row.Item1)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(row.Item2)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(row.Item3)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(row.Item4)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(row.Item5)}</td>" +
                            $"<td>{HttpUtility.HtmlEncode(row.Item6)}</td>" +
                        "</tr>"
                        );
                    }

                    mailMessage.Append(
                        "</table>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>"
                        );

                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                }
            }

            db.SubmitChanges();
        }

        public static void SendThesisTitlesToAdmin(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.SendThesisTitles);
            var currentSemester = Semester.CurrentSemester(db);
            var activeTask = db.Tasks.SingleOrDefault(t => t.TaskType == type && t.Semester == currentSemester);
            var nextSemester = Semester.NextSemester(db);
            var nextSemesterTask = db.Tasks.SingleOrDefault(t => t.TaskType == type && t.Semester == nextSemester);

            if (activeTask == null)
            {
                var deliveryDateCurrentSemester = DateTime.TryParseExact(currentSemester.SubmissionIP6Normal, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dbDate)
                    ? dbDate : (DateTime?)null;

                if (deliveryDateCurrentSemester == null)
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.WebAdmin));
                    mail.Subject = "ProStud Semesterdaten fehlen!";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe/r WebAdmin<p>" +
                        $"<p>Das Abgabedatum für die IP6 für Semester {currentSemester.Name} konnte nicht geladen werden.</p>" +
                        "<p>Bitte überprüfe ob die Daten korrekt in der Datenbank eingetragen sind.</p>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>"
                        );

                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                    return;
                }

                var dueDate = deliveryDateCurrentSemester - Global.AllowTitleChangesBeforeSubmission;

                activeTask = new Task()
                {
                    TaskType = type,
                    Semester = currentSemester,
                    DueDate = dueDate
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            if (nextSemesterTask == null)
            {
                var deliveryDateNextSemester = DateTime.TryParseExact(nextSemester.SubmissionIP6Normal, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dbDateNext)
                    ? dbDateNext : (DateTime?)null;

                var dueDateNext = (deliveryDateNextSemester == null) ? DateTime.MaxValue : deliveryDateNextSemester - Global.AllowTitleChangesBeforeSubmission;

                nextSemesterTask = new Task()
                {
                    TaskType = type,
                    Semester = nextSemester,
                    DueDate = dueDateNext
                };
                db.Tasks.InsertOnSubmit(nextSemesterTask);
                db.SubmitChanges();
            }

            if (!activeTask.Done && DateTime.Now > activeTask.DueDate)
            {
                activeTask.Done = true;

                var thesisProjects = db.Projects.Where(p => p.IsMainVersion && p.State == (int)ProjectState.Published && p.Semester == currentSemester && p.LogProjectType.P6).OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);

                if (thesisProjects.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.GradeAdmin));
                    mail.Subject = "Informatikprojekte P6: Thesis-Titel";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Ausbildungsadministration<p>" +
                        $"<p>Die Thesis-Titel für das Semester {currentSemester.Name}</p>" +
                        "<table>" +
                        "<tr>" +
                            "<th>Projekttitel</th>" +
                            "<th>Betreuer</th>" +
                            "<th>Mail1</th>" +
                            "<th>Mail2</th>" +
                        "</tr>");

                    foreach (var p in thesisProjects)
                    {
                        if (p.LogStudent1Mail != null)
                        {
                            mailMessage.Append(
                            "<tr>" +
                                $"<td>{HttpUtility.HtmlEncode(p.GetFullTitle())}</td>" +
                                $"<td>{HttpUtility.HtmlEncode(p.Advisor1.Mail)}</td>" +
                                $"<td>{HttpUtility.HtmlEncode(p.LogStudent1Mail)}</td>" +
                                $"<td>{((p.LogStudent2Mail != null) ? HttpUtility.HtmlEncode(p.LogStudent2Mail) : "-")}</td>" +
                            "</tr>"
                            );
                        }
                    }

                    mailMessage.Append(
                        "</table>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>"
                        );

                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                }
            }

            db.SubmitChanges();
        }

        private static void InfoInsertNewSemesters(ProStudentCreatorDBDataContext db)
        {
            var targetDate = DateTime.Now.Date.AddMonths(18);

            var activeTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.InsertNewSemesters);
            var semesterMissing = !db.Semester.Any(s => targetDate >= s.StartDate && targetDate < s.DayBeforeNextSemester);

            //add new tasks for projects
            if (semesterMissing && !activeTasks.Any())
                db.Tasks.InsertOnSubmit(new Task
                {
                    ResponsibleUser = db.UserDepartmentMap.Single(i => i.Mail == Global.WebAdmin),
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.InsertNewSemesters),
                    FirstReminded = DateTime.Now
                });

            if (!semesterMissing)
                foreach (var t in activeTasks)
                    t.Done = true;

            db.SubmitChanges();
        }





        private static void SendDoubleCheckMarKomBrochureData(ProStudentCreatorDBDataContext db)
        {
            var lastCheckDate = new DateTime(DateTime.Now.Year, 5, 1);
            if (lastCheckDate > DateTime.Now)
                lastCheckDate = lastCheckDate.AddYears(-1);

            if (lastCheckDate.Year <= 2018)
                return;

            //add new tasks for projects
            var allExportProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.LogProjectType.P6 && p.Semester.StartDate >= lastCheckDate && p.Semester.EndDate <= lastCheckDate
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.DoubleCheckMarKomBrochureData)).ToList();

            foreach (var project in allExportProjects)
                db.Tasks.InsertOnSubmit(new Task
                {
                    ProjectId = project.Id,
                    ResponsibleUser = project.Advisor1,
                    FirstReminded = DateTime.Now,
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.DoubleCheckMarKomBrochureData),
                    Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                });

            db.SubmitChanges();
        }




        private static void InfoStartProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.Semester.StartDate <= DateTime.Now.AddDays(7) && p.Semester.StartDate >= new DateTime(2018, 5, 1)
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.StartProject)).ToList();

            foreach (var project in allPublishedProjects)
                db.Tasks.InsertOnSubmit(new Task
                {
                    ProjectId = project.Id,
                    ResponsibleUser = project.Advisor1,
                    FirstReminded = DateTime.Now,
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.StartProject),
                    Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                });

            db.SubmitChanges();
        }

        private static void InfoFinishProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.Semester.StartDate <= DateTime.Now.AddDays(7) && p.Semester.StartDate >= new DateTime(2018, 5, 1)
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.FinishProject)).ToList();

            foreach (var project in allPublishedProjects)
            {
                var deliveryDate = project.GetDeliveryDate();

                if (deliveryDate.HasValue && deliveryDate.Value.AddDays(4 * 7) <= DateTime.Now)
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.FinishProject),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                    });
            }

            db.SubmitChanges();
        }

        private static void CheckGradesRegistered(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveGradeTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.RegisterGrades).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 5, 1))))
                if ((project.LogGradeStudent1 == null && !string.IsNullOrEmpty(project.LogStudent1Mail)) || (!string.IsNullOrEmpty(project.LogStudent2Mail) && project.LogGradeStudent2 == null))
                    if (!allActiveGradeTasks.Contains(project.Id))
                        db.Tasks.InsertOnSubmit(new Task
                        {
                            ProjectId = project.Id,
                            ResponsibleUser = project.Advisor1,
                            FirstReminded = DateTime.Now,
                            TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.RegisterGrades),
                            Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                        });

            //mark registered tasks as done
            var openGradeTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.RegisterGrades).ToList();
            foreach (var openTask in openGradeTasks)
                if (openTask.Project.State != ProjectState.Published || !openTask.Project.IsMainVersion || (openTask.Project.LogGradeStudent1.HasValue && (openTask.Project.LogGradeStudent2.HasValue || string.IsNullOrEmpty(openTask.Project.LogStudent2Mail))))
                    openTask.Done = true;

            db.SubmitChanges();
        }

        private static void CheckLanguageSet(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveLanguageTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.SetProjectLanguage).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && (p.LogLanguageEnglish==null || p.LogLanguageGerman==null) && p.BillingStatus.RequiresProjectResults).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2018, 5, 1))))
                if (!allActiveLanguageTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.SetProjectLanguage),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openLanguageTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.SetProjectLanguage).ToList();
            foreach (var openTask in openLanguageTasks)
                if (openTask.Project.State != ProjectState.Published || !openTask.Project.IsMainVersion || openTask.Project.LogLanguageGerman==true || openTask.Project.LogLanguageEnglish==true || openTask.Project.BillingStatus?.RequiresProjectResults!=true)
                    openTask.Done = true;

            db.SubmitChanges();
        }

        private static void CheckWebsummaryChecked(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveWebsummaryTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.CheckWebsummary).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && !p.WebSummaryChecked && p.BillingStatus.RequiresProjectResults).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 4, 1))))
                if (!allActiveWebsummaryTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.CheckWebsummary),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openGradeTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.CheckWebsummary && (i.Project.WebSummaryChecked || i.Project.State != ProjectState.Published || !i.Project.BillingStatus.RequiresProjectResults || !i.Project.IsMainVersion)).ToList();
            foreach (var openTask in openGradeTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }

        private static void CheckBillingStatus(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveBillingTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.CheckBillingStatus).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && p.BillingStatus == null).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2017, 4, 1))))
                if (!allActiveBillingTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.CheckBillingStatus),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var completedBillingTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.CheckBillingStatus && (i.Project.BillingStatus != null || i.Project.State != ProjectState.Published || !i.Project.IsMainVersion)).ToList();
            foreach (var openTask in completedBillingTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }


        private static void CheckUploadResults(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var activeUploadTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.UploadResults).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && !p.Attachements.Any(a => !a.Deleted) && p.BillingStatus.RequiresProjectResults).ToList();
            foreach (var project in allPublishedProjects.Where(p => p.ShouldBeGradedByNow(db, new DateTime(2018, 4, 1))))
                if (!activeUploadTasks.Contains(project.Id))
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.UploadResults),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                    });

            //mark registered tasks as done
            var openUploadTasks = db.Tasks.Where(i => !i.Done && i.TaskType.Id == (int)Type.UploadResults &&
            (i.Project.Attachements.Any(a => !a.Deleted) || !i.Project.BillingStatus.RequiresProjectResults || i.Project.State != ProjectState.Published || !i.Project.IsMainVersion)).ToList();
            foreach (var openTask in openUploadTasks)
                openTask.Done = true;

            db.SubmitChanges();
        }


        private static void SendMailsToResponsibleUsers(ProStudentCreatorDBDataContext db)
        {
            var usersToMail = db.Tasks.Where(t => !t.Done && t.ResponsibleUser != null && (t.LastReminded == null || t.LastReminded <= DateTime.Now.AddDays(-t.TaskType.DaysBetweenReminds))).Select(t => t.ResponsibleUser).Distinct();

            foreach (var user in usersToMail)
            {
                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                mail.To.Add(new MailAddress(user.Mail));
                mail.Subject = "Erinnerung von ProStud";
                mail.IsBodyHtml = true;

                var mailMessage = new StringBuilder();
                mailMessage.Append("<div style=\"font-family: Arial\">");
                mailMessage.Append($"<p style=\"font-size: 110%\">Hallo {HttpUtility.HtmlEncode(user.Name.Split(' ')[0])}<p>"
                                    + "<p>Es stehen folgende Aufgaben an:</p><ul>");

                var tasks = db.Tasks.Where(t => t.ResponsibleUser == user && !t.Done).OrderBy(t => t.Project.ProjectNr).ToArray();
                foreach (var task in tasks)
                {
                    if (task.DueDate != null && DateTime.Now.AddDays(3) > task.DueDate && task.Supervisor != null)
                    {
                        mail.CC.Add(task.Supervisor.Mail);
                    }
                    else if (task.FirstReminded != null && task.Supervisor != null && task.TaskType.DaysBetweenReminds > 0 && (DateTime.Now - task.FirstReminded).Value.Days / task.TaskType.DaysBetweenReminds > 3 && task.TaskTypeId == 1)
                    {
                        mail.CC.Add(task.Supervisor.Mail);
                    }


                    mailMessage.Append(task.Project != null ? "<li>" + $"{HttpUtility.HtmlEncode(task.TaskType.Description)} beim Projekt <a href=\"https://www.cs.technik.fhnw.ch/prostud/ProjectInfoPage?id={task.ProjectId}\">{HttpUtility.HtmlEncode(task.Project.Name)}</a></li>" : $"<li><a href=\"https://www.cs.technik.fhnw.ch/prostud/ \">{HttpUtility.HtmlEncode(task.TaskType.Description)}</a></li>");
                }

                mailMessage.Append("</ul>"
                    + "<br/>"
                    + "<p>Freundliche Grüsse</p>"
                    + "ProStud-Team</p>"
                    + $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>"
                    + "</div>"
                    );

                mail.Body = mailMessage.ToString();

                SendMail(mail);

                foreach (var task in tasks)
                {
                    task.LastReminded = DateTime.Now;
                    if (task.TaskType.DaysBetweenReminds == 0)
                        task.Done = true;

                    db.SubmitChanges();
                }
            }
        }

        private static void SendMailToWebAdmin(bool forced)
        {
            var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
            mail.To.Add(new MailAddress(Global.WebAdmin));
            mail.Subject = "TaskCheck has been run";
            mail.IsBodyHtml = true;

            var mailMessage = new StringBuilder();
            mailMessage.Append("<div style=\"font-family: Arial\">");
            mailMessage.Append(
                $"<p>Time: {DateTime.Now}<p>" +
                $"<p>Forced: {forced}<p>" +
                $"<p>NextTaskCheck: {GetNextTaskCheck()}<p>");
            mail.Body = mailMessage.ToString();

            SendMail(mail);
        }

        public static void SendMail(MailMessage mail)
        {
#if !DEBUG
            using (var smtpClient = new SmtpClient())
            {
                mail.CC.Add(new MailAddress(Global.WebAdmin));
                smtpClient.Send(mail);
            }
#else
            using (var smtpClient = new SmtpClient())
            {
                mail.To.Clear();
                mail.CC.Clear();
                mail.Bcc.Clear();
                mail.Subject = "DEBUG: " + mail.Subject;
                mail.To.Add(Global.WebAdmin);
                smtpClient.Send(mail);
            }
#endif
        }
    }
}