using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProStudCreator
{
    public static class ProjectExtensions
    {
        #region Actions

        public static void InitNew(this Project _p)
        {
            _p.Creator = ShibUser.GetEmail();
            _p.CreateDate = DateTime.Now;
            _p.PublishedDate = DateTime.Now;
            _p.State = ProjectState.InProgress;
            _p.IsMainVersion = true;
        }

        public static string GetFullNr(this Project _p) => $"{_p.Semester.Name}_{_p.Department.DepartmentName}{_p.ProjectNr:D2}";

        public static string GetFullTitle(this Project _p) => $"{_p.Semester.Name}_{_p.Department.DepartmentName}{_p.ProjectNr:D2}: {_p.Name}";

        public static Project SaveAsNewVersion(this Project _p, ProStudentCreatorDBDataContext db)
        {
            Project project = new Project
            {
                ModificationDate = DateTime.Now,
                LastEditedBy = ShibUser.GetEmail(),
                IsMainVersion = true
            };
            _p.IsMainVersion = false;
            _p.MapProject(project);

            db.Projects.InsertOnSubmit(project);
            db.SubmitChanges();

            return project;

        }
        public static Project DuplicateAndUseAsTemplate(this Project _p, ProStudentCreatorDBDataContext db)
        {
            var duplicate = _p.Duplicate(db);
            duplicate.BaseVersionId = duplicate.Id;
            duplicate.Reservation1Mail = "";
            duplicate.Reservation2Mail = "";
            duplicate.Reservation1Name = "";
            duplicate.Reservation2Name = "";
            duplicate.State = ProjectState.InProgress;
            _p.ClearLog(db);
            duplicate.IsMainVersion = true;
            db.SubmitChanges();
            return duplicate;
        }
        public static void ClearLog(this Project _p, ProStudentCreatorDBDataContext db)
        {
            _p.LogDefenceDate = null;
            _p.LogDefenceRoom = null;
            _p.LogExpertID = null;
            _p.LogExpertPaid = false;
            _p.LogGradeStudent1 = null;
            _p.LogGradeStudent2 = null;
            _p.LogLanguageEnglish = null;
            _p.LogLanguageGerman = null;
            _p.LogProjectDuration = null;
            _p.LogProjectType = null;
            _p.LogProjectTypeID = null;
            _p.LogStudent1Mail = null;
            _p.LogStudent1Name = null;
            _p.LogStudent2Mail = null;
            _p.LogStudent2Name = null;
            db.SubmitChanges();
        }
        public static bool IsModified(this Project p1, Project p2)
        {

            //Folgende Eîgenschaftene werden ignoriert, da sie entweder vom Benutzer nicht geändert werden können 
            //  oder nur etwas enthalten, wenn das Objekt aus der Datanbank stammt (Relationen) 
            List<string> exclusionList = new List<string>();
            var projectType = typeof(Project);
            var pid = nameof(Project.Id);
            var modDate = nameof(Project.ModificationDate);
            var pubDate = nameof(Project.PublishedDate);
            var lastEditedBy = nameof(Project.LastEditedBy);
            var projectNr = nameof(Project.ProjectNr);
            var isMainVers = nameof(Project.IsMainVersion);
            var projId = nameof(Project.BaseVersionId);
            var credate = nameof(Project.CreateDate);
            var prs = nameof(Project.Projects);
            var attch = nameof(Project.Attachements);
            var tsk = nameof(Project.Tasks);
            var stateColor = nameof(Project.StateColor);
            var stateAsString = nameof(Project.StateAsString);
            var creator = nameof(Project.Creator);
            var semester = nameof(Project.Semester);
            var semesterid = nameof(Project.SemesterId);
            var pOneType = nameof(Project.POneType);
            var pTwoType = nameof(Project.PTwoType);
            var pOneTypeTeamSize = nameof(Project.POneTeamSize);
            var pTwoTypeTeamSize = nameof(Project.PTwoTeamSize);
            var advisor1 = nameof(Project.Advisor1);
            var advisor2 = nameof(Project.Advisor2);


            exclusionList.Add(pid);
            exclusionList.Add(modDate);
            exclusionList.Add(pubDate);
            exclusionList.Add(lastEditedBy);
            exclusionList.Add(projId);
            exclusionList.Add(projectNr);
            exclusionList.Add(isMainVers);
            exclusionList.Add(credate);
            exclusionList.Add(prs);
            exclusionList.Add(attch);
            exclusionList.Add(tsk);
            exclusionList.Add(stateColor);
            exclusionList.Add(stateAsString);
            exclusionList.Add(creator);
            exclusionList.Add(semester);
            exclusionList.Add(semesterid);
            exclusionList.Add(pOneType);
            exclusionList.Add(pTwoType);
            exclusionList.Add(pOneTypeTeamSize);
            exclusionList.Add(pTwoTypeTeamSize);
            exclusionList.Add(advisor1);
            exclusionList.Add(advisor2);

            foreach (PropertyInfo pi in typeof(Project).GetProperties())
            {
                if (!exclusionList.Contains(pi.Name))
                {

                    var value1 = pi.GetValue(p1);
                    var value2 = pi.GetValue(p2);
                    if (value1 != null && value2 != null)
                    {
                        if (!value1.Equals(value2))
                            return true;
                    }
                    else if (value1 != null && value2 == null)
                    {
                        return true;
                    }
                    else if (value1 == null && value2 != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Project Duplicate(this Project _p, ProStudentCreatorDBDataContext db)
        {
            Project duplicatedProject = new Project
            {
                ModificationDate = DateTime.Now,
                LastEditedBy = ShibUser.GetEmail()
            };
            _p.MapProject(duplicatedProject);
            db.Projects.InsertOnSubmit(duplicatedProject);
            db.SubmitChanges();
            return duplicatedProject;
        }

        /// <summary>
        ///     Validates the user's input and generates an error message for invalid input.
        ///     One message is returned at a time, processed top to bottom.
        /// </summary>
        /// <returns>First applicable error message from the validation.</returns>
        public static string GenerateValidationMessage(this Project _p, bool[] projectTypes = null)
        {
            string validationMessage = "";
            if (_p.Advisor1 == null)
                validationMessage = "Bitte wählen Sie einen Hauptbetreuer aus.";

            if (_p.ClientPerson.Trim().Length != 0 && !_p.ClientPerson.IsValidName())
                validationMessage = "Bitte geben Sie den Namen des Kundenkontakts an (Vorname Nachname).";

            if (_p.ClientMail.Trim().Length != 0 && !_p.ClientMail.IsValidEmail())
                validationMessage = "Bitte geben Sie die E-Mail-Adresse des Kundenkontakts an.";

            if ((!_p.Advisor1?.Name.IsValidName()) ?? true)
                validationMessage = "Bitte wählen Sie einen Hauptbetreuer aus.";

            var numAssignedTypes = 0;
            if (projectTypes == null)
            {
                projectTypes = _p.GetProjectTypeBools();
            }

            numAssignedTypes = projectTypes.Count(a => a);


            if (numAssignedTypes != 1 && numAssignedTypes != 2)
                validationMessage = "Bitte wählen Sie genau 1-2 passende Themengebiete aus.";

            if (_p.OverOnePage)
                validationMessage = "Der Projektbeschrieb passt nicht auf eine A4-Seite. Bitte kürzen Sie die Beschreibung.";

            if (!ShibUser.CanSubmitAllProjects() && ShibUser.GetEmail() != _p.Advisor1?.Mail)
                validationMessage = "Nur Hauptbetreuer können Projekte einreichen.";

            if (_p.Reservation1Mail.Trim().Length != 0 && _p.Reservation1Name.Trim().Length == 0)
                validationMessage = "Bitte geben Sie den Namen der ersten Person an, für die das Projekt reserviert ist (Vorname Nachname).";

            if (_p.Reservation1Mail.Trim().Length != 0 && _p.Reservation1Name.Trim().Length != 0)
            {
                Regex regex = new Regex(@".*\..*@students\.fhnw\.ch");
                System.Text.RegularExpressions.Match match = regex.Match(_p.Reservation1Mail);
                if (!match.Success)
                    validationMessage = "Bitte geben Sie eine gültige E-Mail-Adresse der Person an, für die das Projekt reserviert ist. (vorname.nachname@students.fhnw.ch)";
            }

            if (_p.Reservation2Mail.Trim().Length != 0 && _p.Reservation2Name.Trim().Length == 0)
                validationMessage =
                    "Bitte geben Sie den Namen der zweiten Person an, für die das Projekt reserviert ist (Vorname Nachname).";

            if (_p.Reservation2Mail.Trim().Length != 0 && _p.Reservation2Name.Trim().Length != 0)
            {
                Regex regex = new Regex(@".*\..*@students\.fhnw\.ch");
                System.Text.RegularExpressions.Match match = regex.Match(_p.Reservation1Mail);
                match = regex.Match(_p.Reservation2Mail);
                if (!match.Success)
                    validationMessage = "Bitte geben Sie eine gültige E-Mail-Adresse der zweiten Person an, für die das Projekt reserviert ist.(vorname.nachname@students.fhnw.ch)";
            }

            if (_p.Reservation1Name.Trim().Length != 0 && _p.Reservation1Mail.Trim().Length == 0)
                validationMessage = "Bitte geben Sie die E-Mail-Adresse der Person an, für die das Projekt reserviert ist.";

            if (_p.Reservation2Name.Trim().Length != 0 && _p.Reservation2Mail.Trim().Length == 0)
                validationMessage = "Bitte geben Sie die E-Mail-Adresse der zweiten Person an, für die das Projekt reserviert ist.";

            return validationMessage;
        }

        private static bool[] GetProjectTypeBools(this Project _p)
        {
            bool[] projectType = new bool[8];
            if (_p.TypeDesignUX)
                projectType[0] = true;
            if (_p.TypeHW)
                projectType[1] = true;
            if (_p.TypeCGIP)
                projectType[2] = true;
            if (_p.TypeMlAlg)
                projectType[3] = true;
            if (_p.TypeAppWeb)
                projectType[4] = true;
            if (_p.TypeDBBigData)
                projectType[5] = true;
            if (_p.TypeSysSec)
                projectType[6] = true;
            if (_p.TypeSE)
                projectType[7] = true;
            return projectType;
        }


        /***
         * 
         * MapProject creates a duplicate of a project. This can't be done with reflection as it has some issues when fields are null.
         * Doing it this way, the problem is if you change the Project you will have to update this method or the field won't get copied.
         * This is currently solved with a constant which has to be manually updated after the method has been updated
         * 
         */
        public static void MapProject(this Project _p, Project target)
        {
            int EXPECTEDPROPCOUNT = 90; // has to be updated after the project class has changed and the method has been updated

            var actualPropCount = typeof(Project).GetProperties().Count();

            if (actualPropCount != EXPECTEDPROPCOUNT)
                throw new Exception("The Save-Method is outdated. You have mostlikely edited the DBML. Please update ProjectExtension.cs AND the constant 'EXPECTEDPROPCOUNT'. PropertyCount: " + actualPropCount);

            target.Ablehnungsgrund = _p.Ablehnungsgrund;
            target.Advisor1 = _p.Advisor1;
            target.Advisor2 = _p.Advisor2;
            target.Attachements = _p.Attachements;
            target.BaseVersionId = _p.BaseVersionId;
            target.BillingStatus = _p.BillingStatus;
            target.ClientAddressCity = _p.ClientAddressCity;
            target.ClientAddressDepartment = _p.ClientAddressDepartment;
            target.ClientAddressPostcode = _p.ClientAddressPostcode;
            target.ClientAddressStreet = _p.ClientAddressStreet;
            target.ClientAddressTitle = _p.ClientAddressTitle;
            target.ClientCompany = _p.ClientCompany;
            target.ClientMail = _p.ClientMail;
            target.ClientPerson = _p.ClientPerson;
            target.ClientReferenceNumber = _p.ClientReferenceNumber;
            target.ClientType = _p.ClientType;
            target.CreateDate = _p.CreateDate;
            target.Creator = _p.Creator;
            target.Department = _p.Department;
            target.DurationOneSemester = target.DurationOneSemester;
            target.Expert = _p.Expert;
            target.ImgDescription = _p.ImgDescription;
            target.Important = _p.Important;
            target.InitialPosition = _p.InitialPosition;
            //target.IsContinuation = _p.IsContinuation;
            //target.IsMainVersion = _p.IsMainVersion;
            target.LanguageEnglish = _p.LanguageEnglish;
            target.LanguageGerman = _p.LanguageGerman;
            target.LogDefenceDate = _p.LogDefenceDate;
            target.LogDefenceRoom = _p.LogDefenceRoom;
            target.LogExpertID = _p.LogExpertID;
            target.LogExpertPaid = _p.LogExpertPaid;
            target.LogGradeStudent1 = _p.LogGradeStudent1;
            target.LogGradeStudent2 = _p.LogGradeStudent2;
            target.LogLanguageEnglish = _p.LogLanguageEnglish;
            target.LogLanguageGerman = _p.LogLanguageGerman;
            target.LogProjectDuration = _p.LogProjectDuration;
            target.LogProjectType = _p.LogProjectType;
            target.LogProjectTypeID = _p.LogProjectTypeID;
            target.LogStudent1Mail = _p.LogStudent1Mail;
            target.LogStudent1Name = _p.LogStudent1Name;
            target.LogStudent2Mail = _p.LogStudent2Mail;
            target.LogStudent2Name = _p.LogStudent2Name;
            target.Name = _p.Name;
            target.Objective = _p.Objective;
            target.OverOnePage = _p.OverOnePage;
            target.Picture = _p.Picture;
            target.POneTeamSize = _p.POneTeamSize;
            target.POneType = _p.POneType;
            target.PreviousProjectID = _p.PreviousProjectID;
            target.ProblemStatement = _p.ProblemStatement;
            target.PreviousProject = _p.PreviousProject;
            target.ProjectNr = _p.ProjectNr;
            target.Projects = target.Projects;
            target.PTwoTeamSize = _p.PTwoTeamSize;
            target.PTwoType = _p.PTwoType;
            target.PublishedDate = _p.PublishedDate;
            target.References = _p.References;
            target.Remarks = _p.Remarks;
            target.Reservation1Mail = _p.Reservation1Mail;
            target.Reservation1Name = _p.Reservation1Name;
            target.Reservation2Mail = _p.Reservation2Mail;
            target.Reservation2Name = _p.Reservation2Name;
            target.Semester = _p.Semester;
            target.State = _p.State;
            target.Tasks = _p.Tasks;
            target.TypeAppWeb = _p.TypeAppWeb;
            target.TypeCGIP = _p.TypeCGIP;
            target.TypeDBBigData = _p.TypeDBBigData;
            target.TypeDesignUX = _p.TypeDesignUX;
            target.TypeHW = _p.TypeHW;
            target.TypeMlAlg = _p.TypeMlAlg;
            target.TypeSE = _p.TypeSE;
            target.TypeSysSec = _p.TypeSysSec;
            target.UnderNDA = _p.UnderNDA;
            target.WebSummaryChecked = _p.WebSummaryChecked;
            target.GradeSentToAdmin = _p.GradeSentToAdmin;
        }

        public static Project CreateNewProject(ProStudentCreatorDBDataContext db)
        {
            Project project = new Project();
            project.InitNew();
            db.Projects.InsertOnSubmit(project);
            project.ModificationDate = DateTime.Now;
            project.LastEditedBy = ShibUser.GetEmail();
            return project;
        }

        /// <summary>
        ///     Submits user's project for approval by an admin.
        /// </summary>
        /// <param name="_p"></param>
        public static void Submit(this Project _p, ProStudentCreatorDBDataContext _dbx)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            AssignUniqueProjectNr(_p, _dbx);
            _p.State = ProjectState.Submitted;
        }

        /// <summary>
        ///     Publishes a project after submission. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Publish(this Project _p, ProStudentCreatorDBDataContext _dbx)
        {
            _p.PublishedDate = DateTime.Now;
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Published;
            _p.Semester = _dbx.Semester.OrderBy(x => x.StartDate).First(x => x.StartDate > DateTime.Now);
            _dbx.SubmitChanges();
        }

        /// <summary>
        ///     Rejects a project after submission. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Reject(this Project _p)
        {
            _p.State = ProjectState.Rejected;
        }

        /// <summary>
        ///     Generates a new project number that is unique per semester and institute.
        ///     Applies to projects after submission.
        /// </summary>
        /// <param name="_p"></param>
        public static void AssignUniqueProjectNr(this Project _p, ProStudentCreatorDBDataContext dbx)
        {
            var semesterStart = Semester.ActiveSemester(_p.PublishedDate, dbx).StartDate;
            var semesterEnd = Semester.ActiveSemester(_p.PublishedDate, dbx).DayBeforeNextSemester;

            // Get project numbers from this semester & same department
            var nrs = (
                from p in dbx.Projects
                where p.PublishedDate >= semesterStart && p.PublishedDate <= semesterEnd
                      && p.Id != _p.Id
                      && (p.State == ProjectState.Published || p.State == ProjectState.Submitted)
                      && p.Department == _p.Department
                select p.ProjectNr).ToArray();
            if (_p.ProjectNr >= 100 || nrs.Contains(_p.ProjectNr) || _p.ProjectNr < 1)
            {
                _p.ProjectNr = 1;
                while (nrs.Contains(_p.ProjectNr))
                    _p.ProjectNr++;
            }

        }

        /// <summary>
        ///     Sets project state to deleted so it's no longer listed. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Delete(this Project _p)
        {
            _p.ModificationDate = DateTime.Now;
            _p.State = ProjectState.Deleted;
        }

        /// <summary>
        ///     Rolls back project into editable state.
        /// </summary>
        /// <param name="_p"></param>
        public static void Unsubmit(this Project _p)
        {
            _p.State = ProjectState.InProgress;
        }

        /// <summary>
        ///     Revokes project state from 'published' to 'submitted'. Admin only.
        /// </summary>
        /// <param name="_p"></param>
        public static void Unpublish(this Project _p)
        {
            _p.State = ProjectState.Submitted;
        }

        #endregion

        #region Getters

        public static int GetProjectTeamSize(this Project _p)
        {
            if (_p.POneTeamSize.Size2 ||
                _p.PTwoTeamSize != null && _p.PTwoTeamSize.Size2)
                return 2;
            return 1;
        }

        public static DateTime? GetDeliveryDate(this Project _p)
        {
            using (var db = new ProStudentCreatorDBDataContext())
            {
                DateTime dbDate;

                if (_p.LogProjectDuration == 1 && (_p.LogProjectType?.P5 ?? false)) //IP5 Projekt Voll/TeilZeit
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP5FullPartTime, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate : (DateTime?)null;
                if (_p.LogProjectDuration == 2 && (_p.LogProjectType?.P5 ?? false)) //IP5 Berufsbegleitend
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP5Accompanying, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate : (DateTime?)null;
                if (_p.LogProjectDuration == 1 && (_p.LogProjectType?.P6 ?? false)) //IP6 Variante 1 Semester
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP6Normal, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate : (DateTime?)null;
                if (_p.LogProjectDuration == 2 && (_p.LogProjectType?.P6 ?? false)) //IP6 Variante 2 Semester
                    return DateTime.TryParseExact(_p.Semester.SubmissionIP6Variant2, "dd.MM.yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                        ? dbDate : (DateTime?)null;

                return null;
            }
        }

        public static string Student1FirstName(this Project _p) => _p.LogStudent1Name?.Split(' ')?[0];
        public static string Student2FirstName(this Project _p) => _p.LogStudent2Name?.Split(' ')?[0];

        public static string Student1LastName(this Project _p) => _p.LogStudent1Name == null ? null : string.Join(" ", _p.LogStudent1Name.Split(' ').Skip(1).ToArray());
        public static string Student2LastName(this Project _p) => _p.LogStudent2Name == null ? null : string.Join(" ", _p.LogStudent2Name.Split(' ').Skip(1).ToArray());



        public static bool WasDefenseHeld(this Project _p)
        {
            var def = _p.GetDefenseDate();
            if (def == null)
                return false;

            return _p.BillingStatus != null && _p.WebSummaryChecked && _p.IsMainVersion && _p.LogGradeStudent1 != null && def.Value.AddDays(1) < DateTime.Now;
        }

        public static DateTime? GetDefenseDate(this Project _p)
        {
            if (_p.LogDefenceDate != null)
                return _p.LogDefenceDate;

            DateTime dbDate;
            if (_p.LogProjectDuration == 1 && (_p?.LogProjectType?.P5 ?? false)) //IP5 Projekt Voll/TeilZeit
                return DateTime.TryParseExact(_p.Semester.SubmissionIP5FullPartTime, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                    ? dbDate + Global.ExpectFinalPresentationAfterSubmissionForIP5 : (DateTime?)null;
            if (_p.LogProjectDuration == 2 && (_p?.LogProjectType?.P5 ?? false)) //IP5 Berufsbegleitend
                return DateTime.TryParseExact(_p.Semester.SubmissionIP5Accompanying, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dbDate)
                    ? dbDate + Global.ExpectFinalPresentationAfterSubmissionForIP5 : (DateTime?)null;

            return null;
        }


        public static bool CanEditTitle(this Project _p)
        {
            return (_p.LogProjectType?.P5 == true) || (DateTime.Now < _p.GetDeliveryDate() - Global.AllowTitleChangesBeforeSubmission);
        }


        public static Semester GetEndSemester(this Project _p, ProStudentCreatorDBDataContext db)
        {
            return _p.LogProjectDuration == 2 && _p.LogProjectType.P6 ? Semester.NextSemester(_p.Semester, db) : _p.Semester;
        }

        public static string ExhibitionBachelorThesis(this Project _p, ProStudentCreatorDBDataContext db)
        {
            if (_p.LogProjectType?.P5 == true
                || (_p.LogProjectType?.P6 == true && _p.LogProjectDuration == 1 && !_p.Semester.IsSpringSemester())
                || (_p.LogProjectType?.P6 == true && _p.LogProjectDuration == 2 && _p.Semester.IsSpringSemester()))
            {
                return "";
            }
            return _p.GetEndSemester(db).ExhibitionBachelorThesis;
        }


        public static bool ShouldBeGradedByNow(this Project p, ProStudentCreatorDBDataContext db, DateTime _cutoffDate)
        {
            var date = p.GetDefenseDate();
            if (!date.HasValue)
                return false;

            var gradeDate = date.Value + Global.GradingDuration;
            if (gradeDate <= _cutoffDate)
                return false;

            return gradeDate <= DateTime.Now;
        }

        public static string StateColor(this Project p)
        {
            switch (p.State)
            {
                case ProjectState.InProgress:
                    return "#EFF3FB";

                case ProjectState.Published:
                    return "#A9F5A9";

                case ProjectState.Rejected:
                    return "#F5A9A9";

                case ProjectState.Submitted:
                    return "#ffcc99";

                default:
                    return "";
            }

        }

        #endregion

        #region Permissions

        public static bool UserCanEdit(this Project _p)
        {
            return ShibUser.CanEditAllProjects() || _p.UserIsOwner() &&
                   (_p.State == ProjectState.InProgress || _p.State == ProjectState.Rejected ||
                    _p.State == ProjectState.Submitted);
        }

        public static bool UserIsOwner(this Project _p)
        {
            return _p.Creator == ShibUser.GetEmail() || _p.ClientMail == ShibUser.GetEmail() ||
                   _p.Advisor1?.Mail == ShibUser.GetEmail() || _p.Advisor2?.Mail == ShibUser.GetEmail();
        }

        public static bool UserCanEditAfterStart(this Project _p)
        {
            return ShibUser.CanEditAllProjects() || _p.UserIsOwner();
        }

        public static bool UserCanPublish(this Project _p)
        {
            return _p.State == ProjectState.Submitted && ShibUser.CanPublishProject();
        }

        public static bool UserCanUnpublish(this Project _p)
        {
            return _p.State == ProjectState.Published && ShibUser.CanPublishProject();
        }

        public static bool UserCanReject(this Project _p)
        {
            return (_p.State == ProjectState.Submitted || _p.State == ProjectState.Published)
                   && ShibUser.CanPublishProject();
        }

        public static bool UserCanSubmit(this Project _p)
        {
            return _p.UserCanEdit() && (_p.State == ProjectState.InProgress || _p.State == ProjectState.Rejected);
        }

        public static bool UserCanUnsubmit(this Project _p)
        {
            return _p.UserCanEdit() && _p.State == ProjectState.Submitted;
        }

        #endregion
    }
    public partial class Project
    {
        public string StateColor
        {
            get
            {
                switch (State)
                {
                    case ProjectState.InProgress:
                        return "#EFF3FB";

                    case ProjectState.Published:
                        return "#A9F5A9";


                    case ProjectState.Rejected:
                        return "#F5A9A9";


                    case ProjectState.Submitted:
                        return "#ffcc99";


                    case ProjectState.Deleted:
                        return "#F5A9A9";

                    default:
                        throw new Exception();

                }
            }
        }


        public string StateAsString
        {
            get
            {
                switch (State)
                {
                    case ProjectState.InProgress:
                        return "In Bearbeitung";

                    case ProjectState.Published:
                        return "Veröffentlicht";

                    case ProjectState.Rejected:
                        return "Abgelehnt";

                    case ProjectState.Submitted:
                        return "Eingereicht";

                    case ProjectState.Deleted:
                        return "Gelöscht";

                    default:
                        throw new Exception();

                }
            }
        }
    }
}