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
            InfoStartProject = 4,
            InfoFinishProject = 5,
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
            SendThesisTitles = 18,
            FinishProject = 19,
            ThesisTitleHint2Weeks = 20,
            ThesisTitleHint2Days = 21,
            GradeDeadlineIP5 = 22,
            GradeDeadlineIP6 = 23
        }

        private static readonly object TaskCheckLock = new object();
        public const int CheckHour = 23;

        public static DateTime GetNextTaskCheck()
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                var lastRun = db.TaskRuns.Where(r => !r.Forced).OrderByDescending(r => r.Date).FirstOrDefault();
                if (lastRun == null) return DateTime.Now.Date.AddHours(CheckHour);

                return lastRun.Date.Date.AddDays(1).AddHours(CheckHour);
            }
        }

        public static void CheckAllTasks()
        {
            //protect against reentrancy-problems. this lock is enough as long as the method runs in less than 24h
            lock (TaskCheckLock)
            {
                if (!ShouldTaskCheckBeRun()) return;

                RunAllTasks(false);
            }
        }

        private static bool ShouldTaskCheckBeRun()
        {
            var now = DateTime.Now;
            using (var db = new ProStudentCreatorDBDataContext())
            {
                var lastRun = db.TaskRuns.Where(r => !r.Forced).OrderByDescending(r => r.Date).FirstOrDefault();
                if (lastRun == null) return true;

                return now.Date > lastRun.Date.Date && now.Hour >= CheckHour;
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
            try
            {
                using (var db = new ProStudentCreatorDBDataContext())
                {
                    CheckFinishProject(db);
                    CheckArchiveProject(db);
                    // CheckGradesRegistered(db);
                    // CheckWebsummaryChecked(db);
                    // CheckBillingStatus(db);
                    // CheckLanguageSet(db);
                    // CheckUploadResults(db);

                    // InfoStartProject(db);
                    // InfoFinishProject(db);


                    InfoInsertNewSemesters(db);
                    // EnterAssignedStudents(db);

                    SendGradeDeadlineHints(db);

                    SendThesisTitleHints(db);
                    SendThesisTitlesToAdmin(db);
                    // SendGradesToAdmin(db);
                    // SendPayExperts(db);
                    var _ = new_SendPayExperts(db);

                    // vvvvvvvvvvvvv NOT YET IMPLEMENTED
                    // SendInvoiceCustomers(db); //<-- not yet implemented

                    SendDoubleCheckMarKomBrochureData(db);
                    SendMarKomBrochure(db);

                    SendMailsToResponsibleUsers(db);
                    SendTaskCheckMailToWebAdmin(forced);

                    db.TaskRuns.InsertOnSubmit(new TaskRun
                    {
                        Date = DateTime.Now,
                        Forced = forced
                    });
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                mail.To.Add(new MailAddress(Global.WebAdmin));
                mail.Subject = "TaskCheck has thrown an Exception";
                mail.IsBodyHtml = true;

                var mailMessage = new StringBuilder();
                mailMessage.Append("<div style=\"font-family: Arial\">");
                mailMessage.Append(
                    $"<p>Time: {DateTime.Now}<p>" +
                    $"<p>{e.Message}<p>");
                mail.Body = mailMessage.ToString();

                SendMail(mail);
            }
        }

        private static List<int> noFinishReminderMailsForAdvisors = new List<int>() { 7 };
        private static List<int> noFinishReminderMailsForProjects = new List<int>() { 10342 };

        private static void CheckFinishProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks

            var finishTasks = db.Tasks.Where(t => t.TaskType.Id == (int)Type.FinishProject);
            var projectIdsWithTask = finishTasks.Select(t => t.ProjectId).ToList();
            var allOngoingProjects = db.Projects.Where(p => p.State == ProjectState.Ongoing && p.IsMainVersion).ToList();

            foreach (var project in allOngoingProjects)
            {
                if (noFinishReminderMailsForAdvisors.Contains(project?.Advisor1?.Id ?? -1)) continue; // no reminder mails for these advisors
                if (noFinishReminderMailsForProjects.Contains(project?.Id ?? -1)) continue; // no reminder mails for these projects

                if (!projectIdsWithTask.Contains(project.Id))
                {
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        Supervisor = db.UserDepartmentMap.Single(u => u.IsDepartmentManager && u.Department == project.Department),
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.FinishProject),
                        DueDate = project.GetGradeDeliveryDate(db)
                    });
                }
                else
                {
                    var task = db.Tasks.Single(t => t.ProjectId == project.Id && t.TaskTypeId == (int)Type.FinishProject);
                    if (!task.Done)
                    {
                        task.DueDate = project.GetGradeDeliveryDate(db);
                    }
                }
            }
            db.SubmitChanges();

            //check tasks
            var activeFinishTasks = db.Tasks.Where(t => !t.Done && t.TaskTypeId == (int)Type.FinishProject).ToList();
            foreach (var task in activeFinishTasks)
            {
                if (task.Project.State > ProjectState.Ongoing || !task.Project.IsMainVersion)
                {
                    task.Done = true;
                }
            }

            db.SubmitChanges();
        }

        private static void CheckArchiveProject(ProStudentCreatorDBDataContext db)
        {
            var allFinishedOrCanceledProjects = db.Projects.Where(p => p.State == ProjectState.Finished || p.State == ProjectState.Canceled);

            foreach(var p in allFinishedOrCanceledProjects)
            {
                if (p.CheckTransitionArchive())
                {
                    p.Archive(db);
                }
            }

            db.SubmitChanges();
        }

        /*
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

        private static void CheckLanguageSet(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allActiveLanguageTasks = db.Tasks.Where(t => !t.Done && t.TaskType.Id == (int)Type.SetProjectLanguage).Select(i => i.ProjectId).ToList();
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion && (p.LogLanguageEnglish == null || p.LogLanguageGerman == null) && p.BillingStatus.RequiresProjectResults).ToList();
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
                if (openTask.Project.State != ProjectState.Published || !openTask.Project.IsMainVersion || openTask.Project.LogLanguageGerman == true || openTask.Project.LogLanguageEnglish == true || openTask.Project.BillingStatus?.RequiresProjectResults != true)
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


        private static void InfoStartProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.Semester.StartDate <= DateTime.Now.AddDays(7) && p.Semester.StartDate >= new DateTime(2018, 5, 1)
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.InfoStartProject)).ToList();

            foreach (var project in allPublishedProjects)
                db.Tasks.InsertOnSubmit(new Task
                {
                    ProjectId = project.Id,
                    ResponsibleUser = project.Advisor1,
                    FirstReminded = DateTime.Now,
                    TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.InfoStartProject),
                    Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                });

            db.SubmitChanges();
        }

        private static void InfoFinishProject(ProStudentCreatorDBDataContext db)
        {
            //add new tasks for projects
            var allPublishedProjects = db.Projects.Where(p => p.State == ProjectState.Published && p.IsMainVersion
                && p.Semester.StartDate <= DateTime.Now.AddDays(7) && p.Semester.StartDate >= new DateTime(2018, 5, 1)
                && !db.Tasks.Any(t => t.Project == p && t.TaskType.Id == (int)Type.InfoFinishProject)).ToList();

            foreach (var project in allPublishedProjects)
            {
                var deliveryDate = project.GetDeliveryDate();

                if (deliveryDate.HasValue && deliveryDate.Value.AddDays(4 * 7) <= DateTime.Now)
                    db.Tasks.InsertOnSubmit(new Task
                    {
                        ProjectId = project.Id,
                        ResponsibleUser = project.Advisor1,
                        FirstReminded = DateTime.Now,
                        TaskType = db.TaskTypes.Single(t => t.Id == (int)Type.InfoFinishProject),
                        Supervisor = db.UserDepartmentMap.Single(i => i.IsDepartmentManager && i.Department == project.Department)
                    });
            }

            db.SubmitChanges();
        }
        */

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

        public static void SendThesisTitleHints(ProStudentCreatorDBDataContext db)
        {
            var currentSemester = Semester.CurrentSemester(db);
            var type2Weeks = db.TaskTypes.Single(t => t.Id == (int)Type.ThesisTitleHint2Weeks);
            var activeTask2Weeks = db.Tasks.SingleOrDefault(t => t.TaskType == type2Weeks && t.Semester == currentSemester);

            if (activeTask2Weeks == null)
            {
                var deliveryDateCurrentSemester = DateTime.TryParseExact(currentSemester.SubmissionIP6Normal.Trim(), "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dbDate)
                    ? dbDate : (DateTime?)null;

                var dueDate = deliveryDateCurrentSemester - Global.AllowTitleChangesBeforeSubmission - TimeSpan.FromDays(7 * 2);

                activeTask2Weeks = new Task()
                {
                    TaskType = type2Weeks,
                    Semester = currentSemester,
                    DueDate = dueDate
                };
                db.Tasks.InsertOnSubmit(activeTask2Weeks);
                db.SubmitChanges();
            }

            if (!activeTask2Weeks.Done && DateTime.Now > activeTask2Weeks.DueDate)
            {
                activeTask2Weeks.Done = true;

                var thesisProjects = db.Projects.Where(p => p.IsMainVersion
                    && (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.Canceled || p.State == ProjectState.ArchivedFinished || p.State == ProjectState.ArchivedCanceled)
                    && p.Semester == currentSemester
                    && p.LogProjectType.P6
                ).OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);

                if (thesisProjects.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };

                    foreach(var p in thesisProjects)
                    {
                        var ma1 = new MailAddress(p.Advisor1.Mail);
                        if (!mail.To.Contains(ma1))
                        {
                            mail.To.Add(ma1);
                        }
                        if (!string.IsNullOrWhiteSpace(p.Advisor2?.Mail))
                        {
                            var ma2 = new MailAddress(p.Advisor2.Mail);
                            if (!mail.To.Contains(ma2))
                            {
                                mail.To.Add(ma2);
                            }
                        }
                    }

                    mail.Subject = "Informatikprojekte P6: Thesis-Titel Erinnerung";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Betreuerinnen und Betreuer<p>" +
                        $"<p>Die Titel der Bachelorthesen für das {currentSemester.Name} werden in 2 Wochen, so wie sie im ProStud eingetragen sind, an die Ausbildungsadministration gesendet.</p>" +
                        "<p>Ich möchte Sie bitten, die Titel zu überprüfen und Änderungen gleich im ProStud vorzunehmen.</p>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>");
                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                }
            }

            var type2Days = db.TaskTypes.Single(t => t.Id == (int)Type.ThesisTitleHint2Days);
            var activeTask2Days = db.Tasks.SingleOrDefault(t => t.TaskType == type2Days && t.Semester == currentSemester);

            if (activeTask2Days == null)
            {
                var deliveryDateCurrentSemester = DateTime.TryParseExact(currentSemester.SubmissionIP6Normal.Trim(), "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dbDate)
                    ? dbDate : (DateTime?)null;

                var dueDate = deliveryDateCurrentSemester - Global.AllowTitleChangesBeforeSubmission - TimeSpan.FromDays(2);

                activeTask2Days = new Task()
                {
                    TaskType = type2Days,
                    Semester = currentSemester,
                    DueDate = dueDate
                };
                db.Tasks.InsertOnSubmit(activeTask2Days);
                db.SubmitChanges();
            }

            if (!activeTask2Days.Done && DateTime.Now > activeTask2Days.DueDate)
            {
                activeTask2Days.Done = true;

                var thesisProjects = db.Projects.Where(p => p.IsMainVersion
                    && (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.Canceled || p.State == ProjectState.ArchivedFinished || p.State == ProjectState.ArchivedCanceled)
                    && p.Semester == currentSemester
                    && p.LogProjectType.P6
                ).OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);

                if (thesisProjects.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    foreach(var p in thesisProjects)
                    {
                        var ma1 = new MailAddress(p.Advisor1.Mail);
                        if (!mail.To.Contains(ma1))
                        {
                            mail.To.Add(ma1);
                        }
                        if (!string.IsNullOrWhiteSpace(p.Advisor2?.Mail))
                        {
                            var ma2 = new MailAddress(p.Advisor2.Mail);
                            if (!mail.To.Contains(ma2))
                            {
                                mail.To.Add(ma2);
                            }
                        }
                    }

                    mail.Subject = "Informatikprojekte P6: Thesis-Titel Erinnerung";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Betreuerinnen und Betreuer<p>" +
                        $"<p>Die Titel der Bachelorthesen für das {currentSemester.Name} werden in 2 Tagen, so wie sie im ProStud eingetragen sind, an die Ausbildungsadministration gesendet.</p>" +
                        "<p>Ich möchte Sie bitten, die Titel zu überprüfen und Änderungen gleich im ProStud vorzunehmen.</p>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>");
                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                }
            }

            db.SubmitChanges();
        }

        public static void SendGradeDeadlineHints(ProStudentCreatorDBDataContext db)
        {
            var currentSemester = Semester.CurrentSemester(db);
            var lastSemester = Semester.LastSemester(db);
            var typeIP5Deadline = db.TaskTypes.Single(t => t.Id == (int)Type.GradeDeadlineIP5);
            var typeIP6Deadline = db.TaskTypes.Single(t => t.Id == (int)Type.GradeDeadlineIP6);

            var currentSemIP5Task = db.Tasks.SingleOrDefault(t => t.TaskType == typeIP5Deadline && t.Semester == currentSemester);
            var currentSemIP6Task = db.Tasks.SingleOrDefault(t => t.TaskType == typeIP6Deadline && t.Semester == currentSemester);

            if (currentSemIP5Task == null)
            {
                var currentSemIP5Deadline = currentSemester.GradeIP5Deadline;
                var dueDate = currentSemIP5Deadline - TimeSpan.FromDays(7);

                currentSemIP5Task = new Task()
                {
                    TaskType = typeIP5Deadline,
                    Semester = currentSemester,
                    DueDate = dueDate
                };
                db.Tasks.InsertOnSubmit(currentSemIP5Task);
                db.SubmitChanges();
            }
            if (currentSemIP6Task == null)
            {
                var currentSemIP6Deadline = currentSemester.GradeIP6Deadline;
                var dueDate = currentSemIP6Deadline - TimeSpan.FromDays(7);

                currentSemIP6Task = new Task()
                {
                    TaskType = typeIP6Deadline,
                    Semester = currentSemester,
                    DueDate = dueDate
                };
                db.Tasks.InsertOnSubmit(currentSemIP6Task);
                db.SubmitChanges();
            }

            var activeIP5Tasks = db.Tasks.Where(t => t.TaskType == typeIP5Deadline && !t.Done);
            var activeIP6Tasks = db.Tasks.Where(t => t.TaskType == typeIP6Deadline && !t.Done);

            foreach (var task in activeIP5Tasks)
            {
                if (DateTime.Now >= task.DueDate)
                {

                    var projectsIP5 = db.Projects.Where(p => p.IsMainVersion
                        && p.Semester == task.Semester
                        && p.LogProjectTypeID == 1
                        && p.LogProjectDuration == 1
                        && p.State == ProjectState.Ongoing);

                    var adv1 = projectsIP5.Select(p => p.Advisor1);
                    var adv2 = projectsIP5.Where(p => p.Advisor2 != null).Select(p => p.Advisor2);

                    var advisors = adv1.Concat(adv2).Distinct();

                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };

                    foreach (var a in advisors)
                    {
                        var ma = new MailAddress(a.Mail);
                        mail.To.Add(ma);
                    }

                    mail.Subject = "Informatikprojekte IP5: Notenabgabe Erinnerung";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Betreuerinnen und Betreuer<p>" +
                        $"<p>Die Deadline für die Notenabgabe der IP5 (kurz) für das Semester {task.Semester.Name} ist am {task.Semester.GradeIP5Deadline.ToString("dd.MM.yyyy HH:mm")}.</p>" +
                        "<p>Ich möchte Sie bitten, bis zur Deadline die Noten einzutragen und die Projekte abzuschliessen.</p>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>");
                    mail.Body = mailMessage.ToString();

                    SendMail(mail);

                    task.FirstReminded = task.LastReminded = DateTime.Now;
                    task.Done = true;
                }
            }

            foreach (var task in activeIP6Tasks)
            {
                if (DateTime.Now >= task.DueDate)
                {

                    var projectsIP6 = db.Projects.Where(p => p.IsMainVersion
                        && p.Semester == task.Semester
                        && (p.LogProjectTypeID == 2 || p.LogProjectTypeID == 1 && p.LogProjectDuration == 2)
                        && p.State == ProjectState.Ongoing);

                    var adv1 = projectsIP6.Select(p => p.Advisor1);
                    var adv2 = projectsIP6.Where(p => p.Advisor2 != null).Select(p => p.Advisor2);

                    var advisors = adv1.Concat(adv2).Distinct();

                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };

                    foreach (var a in advisors)
                    {
                        var ma = new MailAddress(a.Mail);
                        mail.To.Add(ma);
                    }

                    mail.Subject = "Informatikprojekte IP6: Notenabgabe Erinnerung";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe Betreuerinnen und Betreuer<p>" +
                        $"<p>Die Deadline für die Notenabgabe der IP5 (lang) und IP6 für das Semester {task.Semester.Name} ist am {task.Semester.GradeIP6Deadline.ToString("dd.MM.yyyy HH:mm")}.</p>" +
                        "<p>Ich möchte Sie bitten, bis zur Deadline die Noten einzutragen und die Projekte abzuschliessen.</p>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>");
                    mail.Body = mailMessage.ToString();

                    SendMail(mail);

                    task.FirstReminded = task.LastReminded = DateTime.Now;
                    task.Done = true;
                }
            }

            db.SubmitChanges();
        }

        public static void SendThesisTitlesToAdmin(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.SendThesisTitles);
            var currentSemester = Semester.CurrentSemester(db);
            var activeTask = db.Tasks.SingleOrDefault(t => t.TaskType == type && t.Semester == currentSemester);

            if (activeTask == null)
            {
                var deliveryDateCurrentSemester = DateTime.TryParseExact(currentSemester.SubmissionIP6Normal.Trim(), "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dbDate)
                    ? dbDate : (DateTime?)null;

                //case when semester data in db could not be parsed/read -> send mail to WebAdmin
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
                        $"<p>Das Abgabedatum für die IP6 (SubmissionIP6Normal) für Semester {currentSemester.Name} konnte nicht geladen werden.</p>" +
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

                var dueDate = deliveryDateCurrentSemester - Global.AllowTitleChangesBeforeSubmission + TimeSpan.FromDays(1);

                activeTask = new Task()
                {
                    TaskType = type,
                    Semester = currentSemester,
                    DueDate = dueDate
                };
                db.Tasks.InsertOnSubmit(activeTask);
                db.SubmitChanges();
            }

            //check for next Semester
            var nextSemester = Semester.NextSemester(db);
            var nextSemesterTask = db.Tasks.SingleOrDefault(t => t.TaskType == type && t.Semester == nextSemester);

            if (nextSemesterTask == null)
            {
                var deliveryDateNextSemester = DateTime.TryParseExact(nextSemester.SubmissionIP6Normal.Trim(), "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dbDateNext)
                    ? dbDateNext : (DateTime?)null;

                if (deliveryDateNextSemester == null)
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.WebAdmin));
                    mail.Subject = "ProStud Semesterdaten fehlen!";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe/r WebAdmin<p>" +
                        $"<p>Das Abgabedatum für die IP6 (SubmissionIP6Normal) für Semester {nextSemester.Name} konnte nicht geladen werden.</p>" +
                        "<p>Bitte überprüfe ob die Daten korrekt in der Datenbank eingetragen sind.</p>" +
                        "<br/>" +
                        "<p>Herzliche Grüsse,<br/>" +
                        "ProStud-Team</p>" +
                        $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                        "</div>"
                        );

                    mail.Body = mailMessage.ToString();
                    SendMail(mail);
                }
                else
                {
                    var dueDateNext = deliveryDateNextSemester - Global.AllowTitleChangesBeforeSubmission + TimeSpan.FromDays(1);

                    nextSemesterTask = new Task()
                    {
                        TaskType = type,
                        Semester = nextSemester,
                        DueDate = dueDateNext
                    };
                    db.Tasks.InsertOnSubmit(nextSemesterTask);
                    db.SubmitChanges();
                }
            }

            if (!activeTask.Done && DateTime.Now > activeTask.DueDate)
            {
                activeTask.Done = true;

                var thesisProjects = db.Projects.Where(p => p.IsMainVersion 
                    && (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.Canceled || p.State == ProjectState.ArchivedFinished || p.State == ProjectState.ArchivedCanceled)
                    && p.Semester == currentSemester 
                    && p.LogProjectType.P6
                ).OrderBy(p => p.Department.DepartmentName).ThenBy(p => p.ProjectNr);

                if (thesisProjects.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.WebAdmin)); // TODO: change to Global.GradeAdmin
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
                else
                {
                    var mail = new MailMessage { From = new MailAddress("simon.beck@fhnw.ch") };
                    mail.To.Add(new MailAddress(Global.WebAdmin));
                    mail.Subject = "Keine Thesis Titel an Admin geschickt";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append(
                        "<div style=\"font-family: Arial\">" +
                        "<p>Liebe/r WebAdmin<p>" +
                        $"<p>Es wurden keine Thesen gefunden, deren Titel an die Admin gesendet werden konnten.</p>" +
                        "<p>Bitte überprüfe ob dies stimmt.</p>" +
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

        /*
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
                    mail.To.Add(new MailAddress(Global.WebAdmin)); // Change back to Global.GradeAdmin
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

                    foreach (var row in rowsList.OrderBy(r => r.Item1))
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
        */

        public static (Dictionary<Expert, List<Project>>, Dictionary<Expert, List<Project>>, string) new_SendPayExperts(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.PayExperts);
            var currSemester = Semester.CurrentSemester(db);
            var currSemesterTask = db.Tasks.SingleOrDefault(t => t.TaskType == type && t.Semester == currSemester);
            if (currSemesterTask == null)
            {
                currSemesterTask = new Task()
                {
                    TaskType = type,
                    Semester = currSemester,
                    FirstReminded = DateTime.Now,
                    LastReminded = DateTime.Now
                };
                db.Tasks.InsertOnSubmit(currSemesterTask);
                db.SubmitChanges();
            }

            var expertsToBePaid = new Dictionary<Expert, List<Project>>();
            var expertsNotToBePaid = new Dictionary<Expert, List<Project>>();

            var activeTasks = db.Tasks.Where(t => !t.Done && t.TaskType == type && t.Semester != null);

            foreach (var task in activeTasks)
            {
                var taskSemester = task.Semester;

                if (DateTime.Now < taskSemester.GradeIP6Deadline) continue;

                var thesisProjects = db.Projects.Where(p => p.IsMainVersion
                    && p.Semester == taskSemester
                    && (p.LogProjectType.P6 && !p.LogProjectType.P5)
                    && p.State != ProjectState.Deleted
                    && (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished)
                    && !p.LogExpertPaid
                    && p.Expert != null
                    && p.Expert.AutomaticPayout
                );

                var allExperts = thesisProjects.Select(p => p.Expert).Distinct();

                foreach (Expert e in allExperts)
                {
                    if (thesisProjects.Where(p => p.Expert == e).Any(p => p.State == ProjectState.Ongoing))
                    {
                        var expertProjects = thesisProjects.Where(p => p.Expert == e);
                        expertsNotToBePaid.Add(e, expertProjects.ToList());
                    }
                    else
                    {
                        var expertProjects = thesisProjects.Where(p => p.Expert == e);
                        expertsToBePaid.Add(e, expertProjects.ToList());
                    }
                }

                if (task.LastReminded == null || (DateTime.Now - task.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
                {
                    task.LastReminded = DateTime.Now;

                    if (!thesisProjects.Any())
                    {
                        task.Done = true;
                    }

                    db.SubmitChanges();
                }
            }

            var mailMessage = new StringBuilder();
            if (expertsToBePaid.Any())
            {
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

                foreach (var p in expertsToBePaid.SelectMany(kvpair => kvpair.Value)
                    .OrderBy(p => p.Expert.Name)
                    .ThenBy(p => p.Semester.StartDate)
                    .ThenBy(p => p.Department.DepartmentName)
                    .ThenBy(p => p.ProjectNr))
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

                var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                mail.To.Add(new MailAddress(Global.WebAdmin));
                //mail.To.Add(new MailAddress(Global.PayExpertAdmin));
                //mail.CC.Add(new MailAddress("hanna.troxler@fhnw.ch"));
                mail.Subject = "Informatikprojekte P5/P6: Experten-Honorare auszahlen";
                mail.IsBodyHtml = true;
                mail.Body = mailMessage.ToString();
                SendMail(mail);

                db.SubmitChanges();
            }

            return (expertsToBePaid, expertsNotToBePaid, mailMessage.ToString());
        }

        public static void SendPayExperts(ProStudentCreatorDBDataContext db)
        {
            var type = db.TaskTypes.Single(t => t.Id == (int)Type.PayExperts);
            var activeTask = db.Tasks.SingleOrDefault(t => !t.Done && t.TaskType == type && t.Semester == null);
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

            var lastSemester = Semester.LastSemester(db);

            if (activeTask.LastReminded == null || (DateTime.Now - activeTask.LastReminded.Value).TotalDays > type.DaysBetweenReminds)
            {
                activeTask.LastReminded = DateTime.Now;

                var thesisProjects = db.Projects.Where(p => p.IsMainVersion
                    && p.Semester == lastSemester
                    && (p.LogProjectType.P6 && !p.LogProjectType.P5)
                    && p.State != ProjectState.Deleted
                    && p.State > ProjectState.Published
                );
                
                if (thesisProjects.Any() && thesisProjects.All(p => p.State > ProjectState.Ongoing))
                {
                    var unpaidExperts = thesisProjects.Where(p => p.State == ProjectState.Finished
                            && !p.LogExpertPaid 
                            && p.Expert.AutomaticPayout
                       ).OrderBy(p => p.Expert.Name)
                        .ThenBy(p => p.Semester.StartDate)
                        .ThenBy(p => p.Department.DepartmentName)
                        .ThenBy(p => p.ProjectNr).ToList();
                    
                    if (unpaidExperts.Any())
                    {
                        var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                        mail.To.Add(new MailAddress(Global.WebAdmin));
                        //mail.To.Add(new MailAddress(Global.PayExpertAdmin));
                        //mail.CC.Add(new MailAddress("hanna.troxler@fhnw.ch"));
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
            }

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


        private static void SendDoubleCheckMarKomBrochureData(ProStudentCreatorDBDataContext db)
        {
            var lastCheckDate = new DateTime(DateTime.Now.Year, 5, 1);
            if (lastCheckDate > DateTime.Now)
                lastCheckDate = lastCheckDate.AddYears(-1);

            if (lastCheckDate.Year <= 2018)
                return;

            //add new tasks for projects
            var allExportProjects = db.Projects.Where(p => p.State >= ProjectState.Published && p.State < ProjectState.Deleted && p.IsMainVersion
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
            mail.To.Add(new MailAddress(Global.WebAdmin)); // TODO: change to Global.MarKomAdmin
            //mail.CC.Add(new MailAddress("sibylle.peter@fhnw.ch"));
            mail.Subject = "Informatikprojekte P6: Projektliste für Broschüre";
            mail.IsBodyHtml = true;

            var activeSem = Semester.ActiveSemester(lastSendDate, db);

            var mailMessage = new StringBuilder();
            mailMessage.Append(
                "<div style=\"font-family: Arial\">" +
                "<p>Liebes MarKom<p>" +
                $"<p>Hier die Liste aller Informatik-Bachelorarbeiten im { activeSem.Name }</p>" +
                "<br/>" +
                "<p>Herzliche Grüsse,<br/>" +
                "ProStud-Team</p>" +
                $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>" +
                "</div>"
            );

            //FIXME: should consider project enddate, not startdate of semester
            var projs = db.Projects.Where(p => p.IsMainVersion && p.LogProjectType.P6 && p.Semester.Id == activeSem.Id && p.State == (int)ProjectState.Ongoing && !p.UnderNDA).ToArray();

            var ms = new System.IO.MemoryStream();
            ExcelCreator.GenerateMarKomExcel(ms, projs, db, activeSem.Name);
            byte[] byteArr = ms.ToArray();
            var ms2 = new System.IO.MemoryStream(byteArr, true);
            ms2.Write(byteArr, 0, byteArr.Length);
            ms2.Position = 0;
            var attach = new System.Net.Mail.Attachment(ms2, $"{activeSem.Name}_IP6_Ausstellung.xlsx", "application/vnd.ms-excel");
            mail.Attachments.Add(attach);

            mail.Body = mailMessage.ToString();
            SendMail(mail);

            ms.Close();
            ms2.Close();

            activeTask.Done = true;
            db.SubmitChanges();
        }


        private static void SendMailsToResponsibleUsers(ProStudentCreatorDBDataContext db)
        {
            var usersToMail = db.Tasks.Where(t => !t.Done 
                && t.ResponsibleUser != null
            ).Select(t => t.ResponsibleUser).Distinct();

            foreach (var user in usersToMail)
            {

                var tasks = db.Tasks.Where(t => t.ResponsibleUser == user 
                    && !t.Done
                    && (t.DueDate == null || t.DueDate < DateTime.Now)
                    && (t.LastReminded == null || t.LastReminded <= DateTime.Now.AddDays(-t.TaskType.DaysBetweenReminds))
                ).OrderBy(t => t.Project.ProjectNr).ToArray();

                if (tasks.Any())
                {
                    var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
                    mail.To.Add(new MailAddress(user.Mail));
                    mail.Subject = "Erinnerung von ProStud";
                    mail.IsBodyHtml = true;

                    var mailMessage = new StringBuilder();
                    mailMessage.Append("<div style=\"font-family: Arial\">");
                    mailMessage.Append($"<p style=\"font-size: 110%\">Hallo {HttpUtility.HtmlEncode(user.Name.Split(' ')[0])}<p>"
                                      + "<p>Es stehen folgende Aufgaben an:</p><ul>");

                    foreach (var task in tasks)
                    {
                        if (task.FirstReminded.HasValue 
                            && task.Supervisor != null 
                            && task.TaskType.DaysBetweenReminds > 0 
                            && (DateTime.Now - task.FirstReminded).Value.Days / task.TaskType.DaysBetweenReminds > 2 
                            && task.TaskTypeId == (int)Type.FinishProject)
                        {
                            var ma = new MailAddress(task.Supervisor.Mail);
                            mail.From = ma;
                            if (!mail.CC.Contains(ma))
                            {
                                mail.CC.Add(ma);
                            }
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
                        if (task.FirstReminded == null)
                            task.FirstReminded = DateTime.Now;
                        task.LastReminded = DateTime.Now;
                        if (task.TaskType.DaysBetweenReminds == 0)
                            task.Done = true;
                    }
                }
            }

            db.SubmitChanges();
        }

        private static void SendTaskCheckMailToWebAdmin(bool forced)
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

        public static void SendDebugMail(string body, string subject = "ProStud")
        {
            var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
            mail.To.Add(new MailAddress(Global.WebAdmin));
            mail.Subject = $"DEBUG: {subject}";
            mail.Body = body;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(mail);
            }
        }

        public static void SendMail(MailMessage mail, bool webAdminCC = true)
        {
#if !DEBUG
            using (var smtpClient = new SmtpClient())
            {
                if (webAdminCC)
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