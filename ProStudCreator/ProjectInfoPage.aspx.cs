using System;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ICSharpCode.SharpZipLib.Zip;

namespace ProStudCreator
{
    public partial class ProjectInfoPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project pageProject;
        
        private readonly string dropTypeImpossibleValue = "dropTypeImpossibleValue";
        private readonly string dropDurationImpossibleValue = "dropDurationImpossibleValue";
        private readonly string dropExpertImpossibleValue = "dropExpertImpossibleValue";
        private readonly string dropBillingStatusImpossibleValue = "dropBillingStatusImpossibleValue";
        private readonly string dropLanguageImpossibleValue = "dropLanguageImpossibleValue";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                pageProject = db.Projects.Single(p => p.Id == id);
                if (!pageProject.IsMainVersion)
                {
                    pageProject = db.Projects.Single(p => p.BaseVersionId == pageProject.BaseVersionId && p.IsMainVersion);
                    Response.Redirect(@"~/ProjectInfoPage?id=" + pageProject.Id);
                    return;
                }
                divDownloadBtn.Visible = false;
                updateDownloadButton.Update();
            }
            else
            {
                Response.Redirect("Projectlist.aspx");
                Response.End();
            }

            gridProjectAttachs.DataSource = db.Attachements.Where(item => item.ProjectId == pageProject.Id && !item.Deleted)
                .Select(i => GetProjectSingleAttachment(i));
            gridProjectAttachs.DataBind();

            if (Page.IsPostBack)
            {
                id = int.Parse(Request.QueryString["id"]);
                pageProject = db.Projects.Single(p => (int?)p.Id == id);
                return;
            }
            else
            {
                UpdateUIFromProjectObject();
            }
        }

        #region Form

        private void UpdateUIFromProjectObject()
        {
            //Name
            DisplayName();

            //State
            LabelState.Text = pageProject.StateAsString;

            //Semester
            LabelProjectNr.Text = pageProject.GetProjectLabel();

            //Topics
            DisplayProjectTopics();

            //Advisor
            Advisor1Name.Text = pageProject.Advisor1 != null
                ? $"<a href=\"mailto:{pageProject.Advisor1.Mail}\">{Server.HtmlEncode(pageProject.Advisor1.Name).Replace(" ", "&nbsp;")}</a>"
                : "?";
            Advisor2Name.Text = pageProject.Advisor2 != null
                ? $"<a href=\"mailto:{pageProject.Advisor2.Mail}\">{Server.HtmlEncode(pageProject.Advisor2.Name).Replace(" ", "&nbsp;")}</a>"
                : "";

            //Students
            DisplayStudents();

            //Type and duration
            DisplayTypeAndDuration();

            //Delivery
            LabelProjectDelivery.Text = pageProject.GetDeliveryDate()?.ToString("dd.MM.yyyy") ?? "?";

            //Presentation
            DisplayPresentation();

            //Expert
            DisplayExpert();

            //Language
            DisplayLanguage();

            //WebSummary
            cbxWebSummaryChecked.Checked = pageProject.WebSummaryChecked;
            cbxWebSummaryChecked.Enabled = pageProject.State == ProjectState.Ongoing && pageProject.UserHasAdvisor2Rights();

            string webSumLink = pageProject.GetWebSummaryLink();
            LabelWebsummaryLink.Text = webSumLink is null ? "" : $"<a target=\"_blank\" href=\"{webSumLink}\">Link</a>";

            /*
            LabelWebsummaryLink.Text = !string.IsNullOrEmpty(pageProject?.Semester?.Name ?? null)
                ? pageProject.LogProjectTypeID != null
                    ? "<a target=\"_blank\" href=\"https://web.fhnw.ch/technik/projekte/i/" + (pageProject.LogProjectTypeID == 1
                        ? "ip5" + pageProject.Semester.Name.Substring(0, 2)
                        : "bachelor" + pageProject.Semester.Name.Substring(0, 2))
                    + "\">Link</a>"
                    : ""
                : "";
            */

            //BillingStatus
            DisplayBillingStatus();

            //Grades
            DisplayGrades();

            //Client
            DisplayClient();

            //Buttons
            DisplayButtons();

            //Attachments
            DisplayAttachments();
        }

        private void DisplayName()
        {
            ProjectTitle.Visible = DivProjectTitleAdmin.Visible = false;
            ProjectTitle.Text = ProjectTitleAdmin.Text = pageProject.Name;

            var deliveryDate = pageProject.GetDeliveryDate();

            if (deliveryDate.HasValue)
            {
                if (pageProject.CanEditTitle())
                {
                    if (pageProject.LogProjectType?.P5 == true)
                        ChangeTitleDate.Text = "";
                    else
                        ChangeTitleDate.Text = $"Titeländerung noch bis {(deliveryDate.Value - Global.AllowTitleChangesBeforeSubmission).ToString("dd.MM.yyyy")} möglich!";
                }
                else
                    ChangeTitleDate.Text = $"Titeländerung war nur bis {(deliveryDate.Value - Global.AllowTitleChangesBeforeSubmission).ToString("dd.MM.yyyy")} möglich!";
            }

            switch (pageProject.State)
            {
                case ProjectState.Published:
                    if (pageProject.UserHasDepartmentManagerRights() && pageProject.CanEditTitle())
                    {
                        DivProjectTitleAdmin.Visible = true;
                    }
                    else
                    {
                        ProjectTitle.Visible = true;
                    }
                    break;
                case ProjectState.Ongoing:
                    if (pageProject.UserHasAdvisor2Rights() && pageProject.CanEditTitle())
                    {
                        DivProjectTitleAdmin.Visible = true;
                    }
                    else
                    {
                        ProjectTitle.Visible = true;
                    }
                    break;
                default:
                    ProjectTitle.Visible = true;
                    break;
            }
        }

        private void DisplayProjectTopics()
        {
            var (topic1, topic2) = pageProject.GetTopicStrings();
            ProjectTopicImage1.Name = topic1;
            ProjectTopicImage2.Name = topic2;

            /*
            Topic1.ImageUrl = "pictures/projectTyp" + (pageProject.TypeDesignUX
                   ? "DesignUX"
                   : (pageProject.TypeHW
                       ? "HW"
                       : (pageProject.TypeCGIP
                           ? "CGIP"
                           : (pageProject.TypeMlAlg
                               ? "MlAlg"
                               : (pageProject.TypeAppWeb
                                   ? "AppWeb"
                                   : (pageProject.TypeDBBigData
                                       ? "DBBigData"
                                       : (pageProject.TypeSysSec
                                           ? "SysSec"
                                           : (pageProject.TypeSE ? "SE" : "Transparent")))))))) + ".png";
            Topic2.ImageUrl = "pictures/projectTyp" + (pageProject.TypeHW && pageProject.TypeDesignUX
                               ? "HW"
                               : (pageProject.TypeCGIP && (pageProject.TypeDesignUX || pageProject.TypeHW)
                                   ? "CGIP"
                                   : (pageProject.TypeMlAlg && (pageProject.TypeDesignUX || pageProject.TypeHW || pageProject.TypeCGIP)
                                       ? "MlAlg"
                                       : (pageProject.TypeAppWeb &&
                                          (pageProject.TypeDesignUX || pageProject.TypeHW || pageProject.TypeCGIP || pageProject.TypeMlAlg)
                                           ? "AppWeb"
                                           : (pageProject.TypeDBBigData &&
                                              (pageProject.TypeDesignUX || pageProject.TypeHW || pageProject.TypeCGIP || pageProject.TypeMlAlg ||
                                               pageProject.TypeAppWeb)
                                               ? "DBBigData"
                                               : (pageProject.TypeSysSec &&
                                                  (pageProject.TypeDesignUX || pageProject.TypeHW || pageProject.TypeCGIP || pageProject.TypeMlAlg ||
                                                   pageProject.TypeAppWeb || pageProject.TypeDBBigData)
                                                   ? "SysSec"
                                                   : (pageProject.TypeSE && (pageProject.TypeDesignUX || pageProject.TypeHW || pageProject.TypeCGIP ||
                                                                   pageProject.TypeMlAlg || pageProject.TypeAppWeb ||
                                                                   pageProject.TypeDBBigData || pageProject.TypeSysSec)
                                                       ? "SE"
                                                       : "Transparent"))))))) + ".png";
            */
        }

        private void DisplayStudents()
        {
            DivStudents.Visible = DivStudentsAdmin.Visible = false;

            switch (pageProject.State)
            {
                case ProjectState.Published:
                    if (pageProject.UserHasDepartmentManagerRights())
                    {
                        //show textboxes to input students
                        Student1EventoAdmin.Text = pageProject.LogStudent1Evento ?? "";
                        Student1FirstNameAdmin.Text = pageProject.LogStudent1FirstName ?? "";
                        Student1LastNameAdmin.Text = pageProject.LogStudent1LastName ?? "";
                        Student1MailAdmin.Text = pageProject.LogStudent1Mail ?? "";
                        Student2EventoAdmin.Text = pageProject.LogStudent2Evento ?? "";
                        Student2FirstNameAdmin.Text = pageProject.LogStudent2FirstName ?? "";
                        Student2LastNameAdmin.Text = pageProject.LogStudent2LastName ?? "";
                        Student2MailAdmin.Text = pageProject.LogStudent2Mail ?? "";
                        DivStudentsAdmin.Visible = true;
                    } else
                    {
                        //show ?
                        Student1Name.Text = "?";
                        Student2Name.Text = "?";
                        DivStudents.Visible = true;
                    }
                    break;
                case ProjectState.Ongoing:
                    if (pageProject.UserHasDepartmentManagerRights())
                    {
                        //show textboxes to input students
                        Student1EventoAdmin.Text = pageProject.LogStudent1Evento ?? "";
                        Student1FirstNameAdmin.Text = pageProject.LogStudent1FirstName ?? "";
                        Student1LastNameAdmin.Text = pageProject.LogStudent1LastName ?? "";
                        Student1MailAdmin.Text = pageProject.LogStudent1Mail ?? "";
                        Student2EventoAdmin.Text = pageProject.LogStudent2Evento ?? "";
                        Student2FirstNameAdmin.Text = pageProject.LogStudent2FirstName ?? "";
                        Student2LastNameAdmin.Text = pageProject.LogStudent2LastName ?? "";
                        Student2MailAdmin.Text = pageProject.LogStudent2Mail ?? "";
                        DivStudentsAdmin.Visible = true;
                    }
                    else
                    {
                        //show student labels
                        Student1Name.Text = $"<a href=\"mailto:{pageProject.LogStudent1Mail}\">{Server.HtmlEncode(pageProject.GetStudent1FullName()).Replace(" ", "&nbsp;")}</a>";
                        Student2Name.Text = !string.IsNullOrEmpty(pageProject.LogStudent2Mail)
                            ? $"<a href=\"mailto:{pageProject.LogStudent2Mail}\">{Server.HtmlEncode(pageProject.GetStudent2FullName()).Replace(" ", "&nbsp;")}</a>"
                            : "";
                        DivStudents.Visible = true;
                        break;
                    }
                    break;
                case ProjectState.Finished:
                case ProjectState.Canceled:
                case ProjectState.ArchivedFinished:
                case ProjectState.ArchivedCanceled:
                    //show student labels
                    Student1Name.Text = $"<a href=\"mailto:{pageProject.LogStudent1Mail}\">{Server.HtmlEncode(pageProject.GetStudent1FullName()).Replace(" ", "&nbsp;")}</a>";
                    Student2Name.Text = !string.IsNullOrEmpty(pageProject.LogStudent2Mail)
                        ? $"<a href=\"mailto:{pageProject.LogStudent2Mail}\">{Server.HtmlEncode(pageProject.GetStudent2FullName()).Replace(" ", "&nbsp;")}</a>"
                        : "";
                    DivStudents.Visible = true;
                    break;
                default:
                    //show ?
                    Student1Name.Text = "?";
                    Student2Name.Text = "?";
                    DivStudents.Visible = true;
                    break;
            }
        }

        private void DisplayTypeAndDuration()
        {
            DivType.Visible = DivTypeAdmin.Visible = false;

            switch (pageProject.State)
            {
                case ProjectState.Published:
                    if (pageProject.UserHasDepartmentManagerRights())
                    {
                        //show dropdowns
                        FillDropType(true);
                        DropDuration.SelectedValue = pageProject.LogProjectDuration?.ToString() ?? dropDurationImpossibleValue;
                        DivTypeAdmin.Visible = true;

                        //TODO: Set DropDuration.visible depending on DropType
                    }
                    else
                    {
                        //show ?
                        LabelProjectType.Text = "?";
                        DivType.Visible = true;
                    }
                    break;
                case ProjectState.Ongoing:
                    if (pageProject.UserHasDepartmentManagerRights())
                    {
                        //show dropdowns
                        FillDropType(false);
                        DropDuration.Items.Remove(DropDuration.Items.FindByValue(dropDurationImpossibleValue));
                        DropDuration.SelectedValue = pageProject.LogProjectDuration.ToString();
                        DivTypeAdmin.Visible = true;

                        //TODO: Set DropDuration.visible depending on DropType
                    }
                    else
                    {
                        //show type with duration
                        if (pageProject.LogProjectType.P5 && !pageProject.LogProjectType.P6)
                        {
                            LabelProjectType.Text = "IP5" + (pageProject.LogProjectDuration == 2 ? " (Lang)" : "");
                        }
                        else if (!pageProject.LogProjectType.P5 && pageProject.LogProjectType.P6)
                        {
                            LabelProjectType.Text = "IP6"/* + (pageProject.LogProjectDuration == 2 ? " (Lang)" : "")*/;
                        }
                        else
                        {
                            LabelProjectType.Text = "?";
                        }
                        DivType.Visible = true;
                    }
                    break;
                case ProjectState.Finished:
                case ProjectState.Canceled:
                case ProjectState.ArchivedFinished:
                case ProjectState.ArchivedCanceled:
                    //show type with duration
                    if (pageProject.LogProjectType is null)
                    {
                        LabelProjectType.Text = "?";
                    }
                    else if (pageProject.LogProjectType.P5 && !pageProject.LogProjectType.P6)
                    {
                        LabelProjectType.Text = "IP5" + (pageProject.LogProjectDuration == 2 ? " (Lang)" : "");
                    }
                    else if (!pageProject.LogProjectType.P5 && pageProject.LogProjectType.P6)
                    {
                        LabelProjectType.Text = "IP6"/* + (pageProject.LogProjectDuration == 2 ? " (Lang)" : "")*/;
                    }
                    else
                    {
                        LabelProjectType.Text = "?";
                    }
                    DivType.Visible = true;
                    break;
                default:
                    //show ?
                    LabelProjectType.Text = "?";
                    DivType.Visible = true;
                    break;
            }
        }

        private void FillDropType(bool withImpossibleValue)
        {
            DropType.DataSource = db.ProjectTypes.Where(t => t.P5 != t.P6).OrderBy(t => t.Id);
            DropType.DataBind();
            if (withImpossibleValue)
            {
                DropType.Items.Insert(0, new ListItem("(Bitte Auswählen)", dropTypeImpossibleValue));
                DropType.SelectedValue = pageProject.LogProjectType?.Id.ToString() ?? dropTypeImpossibleValue;
            }
            else
            {
                DropType.SelectedValue = pageProject.LogProjectType.Id.ToString();
            }
        }

        private void DisplayPresentation()
        {
            DivPresentation.Visible = DivPresentationAdmin.Visible = DivBachelor.Visible = false;

            if (pageProject.LogProjectType?.P6 ?? false && !(pageProject.LogProjectType?.P5 ?? false))
            {
                //P6
                LabelProjectEndPresentation.Text = LabelProjectEndPresentationAdmin.Text = "Verteidigung:";
                ProjectExhibition.Text = pageProject.ExhibitionBachelorThesis(db);
                DivBachelor.Visible = !string.IsNullOrWhiteSpace(ProjectExhibition.Text);
            }
            else
            {
                //P5
                LabelProjectEndPresentation.Text = LabelProjectEndPresentationAdmin.Text = "Schlusspräsentation:";
            }

            switch (pageProject.State)
            {
                case ProjectState.Ongoing:
                    if (pageProject.UserHasAdvisor2Rights())
                    {
                        if (pageProject.LogDefenceDate.HasValue)
                        {
                            var defenceDate = pageProject.LogDefenceDate.Value;
                            TextBoxLabelPresentationDate.Text = defenceDate.ToString("dd.MM.yyyy");
                            TextBoxLabelPresentationTime.Text = defenceDate.ToString("HH:mm");
                            TextBoxLabelPresentationRoom.Text = pageProject.LogDefenceRoom;
                        }
                        DivPresentationAdmin.Visible = true;
                    }
                    else
                    {
                        if (pageProject.LogDefenceDate.HasValue)
                        {
                            var defenceDate = pageProject.LogDefenceDate.Value;
                            LabelPresentation.Text = $"{defenceDate.ToString("dd.MM.yyyy HH:mm")}{(pageProject.LogDefenceRoom != null ? ", Raum: " + pageProject.LogDefenceRoom : "")}";
                        }
                        else
                        {
                            LabelPresentation.Text = "?";
                        }
                        DivPresentation.Visible = true;
                    }
                    break;
                case ProjectState.Finished:
                case ProjectState.Canceled:
                case ProjectState.ArchivedFinished:
                case ProjectState.ArchivedCanceled:
                default:
                    if (pageProject.LogDefenceDate.HasValue)
                    {
                        var defenceDate = pageProject.LogDefenceDate.Value;
                        LabelPresentation.Text = $"{defenceDate.ToString("dd.MM.yyyy HH:mm")}{(pageProject.LogDefenceRoom != null ? ", Raum: " + pageProject.LogDefenceRoom : "")}";
                    }
                    else
                    {
                        LabelPresentation.Text = "?";
                    }
                    DivPresentation.Visible = true;
                    break;
            }
        }

        private void DisplayExpert()
        {
            DivExpert.Visible = DivExpertAdmin.Visible = false;

            if (pageProject.LogProjectType?.P6 ?? false && !(pageProject.LogProjectType?.P5 ?? false))
            {
                switch (pageProject.State)
                {
                    case ProjectState.Ongoing:
                        if (pageProject.UserHasDepartmentManagerRights())
                        {
                            DropExpert.DataSource = db.Experts.Where(i => i.Active).OrderBy(a => a.Name).Select(x =>
                                new {
                                    Id = x.id,
                                    Mail = x.Mail,
                                    DropDownString = string.Format("{0} | {1}", x.Name, x.Knowhow).Replace(" ", HttpUtility.HtmlDecode("&nbsp;"))
                                }
                            );

                            DropExpert.DataValueField = "Id";
                            DropExpert.DataTextField = "DropDownString";
                            DropExpert.DataBind();
                            DropExpert.Items.Insert(0, new ListItem("-", dropExpertImpossibleValue));
                            DropExpert.SelectedValue = pageProject.Expert?.id.ToString() ?? dropExpertImpossibleValue;
                            
                            SetExpertMail();

                            DivExpertAdmin.Visible = true;
                        }
                        else
                        {
                            LabelExpertName.Text = !string.IsNullOrEmpty(pageProject.LogExpertID.ToString())
                                ? $"<a href=\"mailto:{pageProject.Expert.Mail}\">{Server.HtmlEncode(pageProject.Expert.Name).Replace(" ", "&nbsp;")}</a>"
                                : "Noch nicht entschieden";
                            DivExpert.Visible = true;
                        }
                        break;
                    default:
                        LabelExpertName.Text = !string.IsNullOrWhiteSpace(pageProject.LogExpertID.ToString())
                            ? $"<a href=\"mailto:{pageProject.Expert.Mail}\">{Server.HtmlEncode(pageProject.Expert.Name).Replace(" ", "&nbsp;")}</a>"
                            : "?";
                        DivExpert.Visible = true;
                        break;
                }


            }
        }

        private void SetExpertMail()
        {
            LabelExpertMail.Text = DropExpert.SelectedIndex != 0
                ? "<a href=\"mailto:" + db.Experts.SingleOrDefault(ex => ex.id == int.Parse(DropExpert.SelectedValue))?.Mail + "\">Mail</a>"
                : "";
        }

        private void DisplayLanguage()
        {
            DivLanguage.Visible = DivLanguageAdmin.Visible = false;

            switch (pageProject.State)
            {
                case ProjectState.Ongoing:
                    if (pageProject.UserHasAdvisor2Rights())
                    {
                        if (pageProject.LogLanguageEnglish != null && pageProject.LogLanguageGerman != null)
                            if (pageProject.LogLanguageEnglish.Value && !pageProject.LogLanguageGerman.Value)
                                DropLanguage.SelectedValue = "1";
                            else if (!pageProject.LogLanguageEnglish.Value && pageProject.LogLanguageGerman.Value)
                                DropLanguage.SelectedValue = "2";
                            else
                                DropLanguage.SelectedValue = "0";
                        else
                            DropLanguage.SelectedValue = "0";
                        DivLanguageAdmin.Visible = true;
                    }
                    else
                    {
                        if (pageProject.LogLanguageEnglish != null && pageProject.LogLanguageGerman != null)
                            if (pageProject.LogLanguageEnglish.Value && !pageProject.LogLanguageGerman.Value)
                                LabelLanguage.Text = "Englisch";
                            else if (!pageProject.LogLanguageEnglish.Value && pageProject.LogLanguageGerman.Value)
                                DropLanguage.SelectedValue = "Deutsch";
                            else
                                DropLanguage.SelectedValue = "Noch nicht festgelegt";
                        else
                            DropLanguage.SelectedValue = "Noch nicht festgelegt";
                        DivLanguage.Visible = true;
                    }
                    break;
                default:
                    if (pageProject.LogLanguageEnglish != null && pageProject.LogLanguageGerman != null)
                        if (pageProject.LogLanguageEnglish.Value && !pageProject.LogLanguageGerman.Value)
                            LabelLanguage.Text = "Englisch";
                        else if (!pageProject.LogLanguageEnglish.Value && pageProject.LogLanguageGerman.Value)
                            LabelLanguage.Text = "Deutsch";
                        else
                            LabelLanguage.Text = "Noch nicht festgelegt";
                    else
                        DropLanguage.SelectedValue = "Noch nicht festgelegt";
                    DivLanguage.Visible = true;
                    break;
            }
        }

        private void DisplayBillingStatus()
        {
            DivBillingStatus.Visible = DivBillingStatusAdmin.Visible = false;
            LabelBillingStatus.ForeColor = System.Drawing.Color.Empty;

            switch (pageProject.State)
            {
                case ProjectState.Ongoing:
                    if (pageProject.UserHasAdvisor1Rights())
                    {
                        DropBillingStatus.DataSource = db.BillingStatus.Where(bs => bs.RequiresProjectResults).OrderBy(i => i.DisplayName);
                        DropBillingStatus.DataBind();
                        DropBillingStatus.Items.Insert(0, new ListItem("(Bitte Auswählen)", dropBillingStatusImpossibleValue));
                        DropBillingStatus.SelectedValue = pageProject.BillingStatusID?.ToString() ?? dropBillingStatusImpossibleValue;

                        DivBillingStatusAdmin.Visible = true;
                    }
                    else
                    {
                        if (pageProject.UserIsAdvisor2() && pageProject.BillingStatus == null)
                        {
                            LabelBillingStatus.Text = "Der Verrechnungsstatus kann nur vom Hauptbetreuer gesetzt werden.";
                            LabelBillingStatus.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            LabelBillingStatus.Text = pageProject.BillingStatus?.DisplayName ?? "?";
                        }
                        DivBillingStatus.Visible = true;
                    }
                    break;
                default:
                    LabelBillingStatus.Text = pageProject.BillingStatus?.DisplayName ?? "?";
                    DivBillingStatus.Visible = true;
                    break;
            }
        }

        private void DisplayGrades()
        {
            NumGradeStudent1.Visible = NumGradeStudent2.Visible = NumGradeStudent1Admin.Visible = NumGradeStudent2Admin.Visible = DivGradeWarning.Visible = false;
            NumGradeStudent1.ForeColor = System.Drawing.Color.Empty;
            NumGradeStudent2.ForeColor = System.Drawing.Color.Empty;

            //set the Labels to the Grades
            LabelGradeStudent1.Text = $"Note von {pageProject.GetStudent1FullName()}:";
            LabelGradeStudent2.Text = $"Note von {pageProject.GetStudent2FullName()}:";

            switch (pageProject.State)
            {
                case ProjectState.Ongoing:
                    if (pageProject.UserHasAdvisor1Rights())
                    {
                        NumGradeStudent1Admin.Text = pageProject.LogGradeStudent1?.ToString("N1", CultureInfo.InvariantCulture) ?? "";
                        NumGradeStudent2Admin.Text = pageProject.LogGradeStudent2?.ToString("N1", CultureInfo.InvariantCulture) ?? "";

                        SetGradeFieldStatus();
                        UpdateGradeFields.Update();
                    }
                    else
                    {
                        NumGradeStudent1.Text = pageProject.LogGradeStudent1?.ToString("N1", CultureInfo.InvariantCulture) ?? "?";
                        NumGradeStudent2.Text = pageProject.LogGradeStudent2?.ToString("N1", CultureInfo.InvariantCulture) ?? "?";

                        if (pageProject.UserIsAdvisor2() && pageProject.LogGradeStudent1 == null)
                        {
                            NumGradeStudent1.Text = "Noten können nur vom Hauptbetreuer gesetzt werden.";
                            NumGradeStudent1.ForeColor = System.Drawing.Color.Red;
                        }
                        if (pageProject.UserIsAdvisor2() && pageProject.LogGradeStudent2 == null)
                        {
                            NumGradeStudent2.Text = "Noten können nur vom Hauptbetreuer gesetzt werden.";
                            NumGradeStudent2.ForeColor = System.Drawing.Color.Red;
                        }
                        
                        NumGradeStudent1.Visible = NumGradeStudent2.Visible = true;
                    }
                    break;
                default:
                    NumGradeStudent1.Text = pageProject.LogGradeStudent1?.ToString("N1", CultureInfo.InvariantCulture) ?? "?";
                    NumGradeStudent2.Text = pageProject.LogGradeStudent2?.ToString("N1", CultureInfo.InvariantCulture) ?? "?";
                    NumGradeStudent1.Visible = NumGradeStudent2.Visible = true;
                    break;
            }

            DivGradeStudent1.Visible = true;
            DivGradeStudent2.Visible = !string.IsNullOrEmpty(pageProject.LogStudent2Mail);

        }

        private void DisplayClient()
        {
            UpdateClientInfo();
            UpdateClientVisibility();
        }

        private void DisplayButtons()
        {
            BtnSaveBetween.Visible = BtnSaveChanges.Visible = true;
            BtnFinishProject.Visible = BtnCancelProject.Visible = BtnKickoffProject.Visible = false;
            BtnDuplicateProject.Visible = true;

            switch (pageProject.State)
            {
                case ProjectState.Published:
                    BtnSaveBetween.Enabled = BtnSaveChanges.Enabled = pageProject.UserHasDepartmentManagerRights();
                    BtnKickoffProject.Visible = true;
                    BtnKickoffProject.Enabled = pageProject.UserHasDepartmentManagerRights();
                    break;
                case ProjectState.Ongoing:
                    BtnSaveBetween.Enabled = BtnSaveChanges.Enabled = pageProject.UserHasAdvisor2Rights();
                    BtnFinishProject.Visible = true;
                    BtnFinishProject.Enabled = pageProject.UserHasAdvisor2Rights();
                    BtnCancelProject.Visible = true;
                    BtnCancelProject.Enabled = pageProject.UserHasAdvisor2Rights();
                    break;
                default:
                    BtnSaveBetween.Enabled = BtnSaveChanges.Enabled = BtnFinishProject.Enabled = BtnCancelProject.Enabled = BtnKickoffProject.Enabled = false;
                    BtnDuplicateProject.Enabled = true;
                    break;
            }

            BtnFinishProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Finished));
            BtnCancelProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Canceled));
            BtnKickoffProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Ongoing));
        }

        private void DisplayAttachments()
        {
            if (pageProject.UnderNDA && !pageProject.UserHasAdvisor2Rights())
            {
                DivAttachments.Visible = false;
            }

            divFileUpload.Visible = pageProject.UserHasAdvisor2Rights();
        }

        #endregion

        #region Events

        public void DrpExpert_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetExpertMail();
        }

        protected void BtnFinishProject_OnClick(object sender, EventArgs e)
        {
            var validationMessageSave = GenerateValidationMessageForSave();
            if (validationMessageSave == null)
            {
                var validationMessageFinish = GenerateValidationMessageForFinish();
                if (validationMessageFinish == null)
                {
                    SaveChanges();
                    pageProject.ModificationDate = DateTime.Now;
                    pageProject.LastEditedBy = ShibUser.GetEmail();
                    pageProject.Finish(db);
                    Response.Redirect("Projectlist");
                }
                else
                {
                    ReturnAlert(validationMessageFinish);
                }
            }
            else
            {
                ReturnAlert(validationMessageSave);
            }
        }

        protected void BtnCancelProject_OnClick(object sender, EventArgs e)
        {
            var validationMessageCancel = GenerateValidationMessageForCancel();
            if (validationMessageCancel == null)
            {
                pageProject.ModificationDate = DateTime.Now;
                pageProject.LastEditedBy = ShibUser.GetEmail();
                pageProject.LogGradeStudent1 = 1;
                if (pageProject.LogStudent2Mail != null) pageProject.LogGradeStudent2 = 1;
                pageProject.BillingStatus = db.BillingStatus.Single(bs => bs.DisplayName == "Abgebrochen");
                pageProject.Cancel(db);
                Response.Redirect("Projectlist");
            }
            else
            {
                ReturnAlert(validationMessageCancel);
            }
        }

        protected void BtnKickoffProject_OnClick(object sender, EventArgs e)
        {
            var validationMessageSave = GenerateValidationMessageForSave();
            if (validationMessageSave == null)
            {
                var validationMessageKickoff = GenerateValidationMessageForKickoff();
                if (validationMessageKickoff == null)
                {
                    SaveChanges();
                    pageProject.ModificationDate = DateTime.Now;
                    pageProject.LastEditedBy = ShibUser.GetEmail();
                    pageProject.Kickoff(db);
                    Response.Redirect("Projectlist");
                }
                else
                {
                    ReturnAlert(validationMessageKickoff);
                }
            }
            else
            {
                ReturnAlert(validationMessageSave);
            }
        }

        protected void BtnDuplicateProject_OnClick(object sender, EventArgs e)
        {
            var duplicate = pageProject.DuplicateProject(db);

            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                Response.Redirect("~/ProjectEditPage.aspx?id=" + duplicate.Id);
            }
        }

        protected void BtnSaveChanges_OnClick(object sender, EventArgs e)
        {
            var validationMessage = GenerateValidationMessageForSave();

            if (validationMessage == null)
            {
                SaveChanges();
                pageProject.ModificationDate = DateTime.Now;
                pageProject.LastEditedBy = ShibUser.GetEmail();
                db.SubmitChanges();
                Response.Redirect("Projectlist");
            }
            else
            {
                ReturnAlert(validationMessage);
            }
        }

        protected void BtnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("Projectlist");
        }

        protected void DropBillingStatusChanged(object sender, EventArgs e)
        {
            SetGradeFieldStatus();
            UpdateGradeFields.Update();

            var billingStatus = DropBillingStatus.Visible
                ? DropBillingStatus.SelectedValue != dropBillingStatusImpossibleValue
                    ? db.BillingStatus.Single(bs => bs.Id == int.Parse(DropBillingStatus.SelectedValue))
                    : null
                : pageProject.BillingStatus;

            if (billingStatus == null) return;

            if (billingStatus.ShowAddressOnInfoPage && radioClientType.SelectedIndex == (int)ClientType.Internal)
            {
                radioClientType.SelectedIndex = (int)ClientType.Company;
                UpdateClientVisibility();
            }
        }

        protected void DropLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetGradeFieldStatus();
            UpdateGradeFields.Update();
        }

        protected void cbxWebSummaryChecked_CheckedChanged(object sender, EventArgs e)
        {
            SetGradeFieldStatus();
            UpdateGradeFields.Update();
        }

        private bool CheckGradeFieldStatus()
        {
            return cbxWebSummaryChecked.Checked
                && DropBillingStatus.SelectedValue != dropBillingStatusImpossibleValue
                && DropLanguage.SelectedIndex != 0;
        }

        private void SetGradeFieldStatus()
        {
            if (pageProject.State == ProjectState.Ongoing && pageProject.UserHasAdvisor1Rights())
            {
                if (CheckGradeFieldStatus())
                {
                    NumGradeStudent1.Visible = NumGradeStudent2.Visible = false;
                    NumGradeStudent1Admin.Visible = NumGradeStudent2Admin.Visible = true;
                }
                else
                {
                    NumGradeStudent1Admin.Visible = NumGradeStudent2Admin.Visible = false;
                    NumGradeStudent1.Visible = NumGradeStudent2.Visible = true;
                    NumGradeStudent1.Text = "Die Noten können erst eingetragen werden, wenn die Felder: Durchführungssprache, Websummary und Verrechnungsstatus ausgefüllt sind.";
                    NumGradeStudent2.Text = "Die Noten können erst eingetragen werden, wenn die Felder: Durchführungssprache, Websummary und Verrechnungsstatus ausgefüllt sind.";
                    NumGradeStudent1.ForeColor = System.Drawing.Color.Red;
                    NumGradeStudent2.ForeColor = System.Drawing.Color.Red;
                }

                DivGradeWarning.Visible = true;
            }
            else
            {
                DivGradeWarning.Visible = false;
            }
        }

        protected void BtnSaveBetween_OnClick(object sender, EventArgs e)
        {
            var validationMessage = GenerateValidationMessageForSave();

            if (validationMessage == null)
            {
                SaveChanges();
                pageProject.ModificationDate = DateTime.Now;
                pageProject.LastEditedBy = ShibUser.GetEmail();
                db.SubmitChanges();
                Response.Redirect("ProjectInfoPage?id=" + pageProject.Id);
            }
            else
            {
                ReturnAlert(validationMessage);
            }
        }

        protected void RadioClientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateClientVisibility();
        }

        private void UpdateClientInfo()
        {
            radioClientType.SelectedIndex = pageProject.ClientType;
            LabelClientCompany.Text = txtClientCompanyAdmin.Text = pageProject.ClientCompany ?? "";
            LabelClientTitle.Text = pageProject.ClientAddressTitle ?? "";
            drpClientTitleAdmin.SelectedValue = pageProject.ClientAddressTitle == "Herr" ? "1" : "2";
            LabelClientName.Text = txtClientNameAdmin.Text = pageProject.ClientPerson ?? "";
            LabelClientEmail.Text = !string.IsNullOrWhiteSpace(pageProject.ClientMail)
                ? $"<a href=\"mailto:{pageProject.ClientMail}\">{Server.HtmlEncode(pageProject.ClientMail).Replace(" ", "&nbsp;")}</a>"
                : "";
            txtClientEmailAdmin.Text = pageProject.ClientMail ?? "";
            LabelClientPhoneNumber.Text = txtClientPhoneNumberAdmin.Text = pageProject.ClientPhoneNumber ?? "";
            LabelClientDepartment.Text = txtClientDepartmentAdmin.Text = pageProject.ClientAddressDepartment ?? "";
            LabelClientStreet.Text = txtClientStreetAdmin.Text = pageProject.ClientAddressStreet ?? "";
            LabelClientPLZ.Text = txtClientPLZAdmin.Text = pageProject.ClientAddressPostcode ?? "";
            LabelClientCity.Text = txtClientCityAdmin.Text = pageProject.ClientAddressCity ?? "";
            LabelClientReference.Text = txtClientReferenceAdmin.Text = pageProject.ClientReferenceNumber ?? "";
            chkNDA.Checked = pageProject.UnderNDA;
        }

        private void UpdateClientVisibility()
        {
            LabelClientCompany.Visible = txtClientCompanyAdmin.Visible =
                LabelClientTitle.Visible = drpClientTitleAdmin.Visible =
                    LabelClientName.Visible = txtClientNameAdmin.Visible =
                        LabelClientEmail.Visible = txtClientEmailAdmin.Visible =
                            LabelClientPhoneNumber.Visible = txtClientPhoneNumberAdmin.Visible = 
                                LabelClientDepartment.Visible = txtClientDepartmentAdmin.Visible =
                                    LabelClientStreet.Visible = txtClientStreetAdmin.Visible =
                                        LabelClientPLZ.Visible = txtClientPLZAdmin.Visible =
                                            LabelClientCity.Visible = txtClientCityAdmin.Visible =
                                                LabelClientReference.Visible = txtClientReferenceAdmin.Visible = false;

            switch (pageProject.State)
            {
                case ProjectState.Ongoing:
                    if (pageProject.UserHasAdvisor2Rights())
                    {
                        txtClientCompanyAdmin.Visible =
                            drpClientTitleAdmin.Visible =
                                txtClientNameAdmin.Visible =
                                    txtClientEmailAdmin.Visible =
                                        txtClientPhoneNumberAdmin.Visible = 
                                            txtClientDepartmentAdmin.Visible =
                                                txtClientStreetAdmin.Visible =
                                                    txtClientPLZAdmin.Visible =
                                                        txtClientCityAdmin.Visible =
                                                            txtClientReferenceAdmin.Visible = true;
                        radioClientType.Enabled = true;
                        chkNDA.Enabled = true;
                    }
                    else
                    {
                        LabelClientCompany.Visible =
                            LabelClientTitle.Visible =
                                LabelClientName.Visible =
                                    LabelClientEmail.Visible =
                                        LabelClientPhoneNumber.Visible = 
                                            LabelClientDepartment.Visible =
                                                LabelClientStreet.Visible =
                                                    LabelClientPLZ.Visible =
                                                        LabelClientCity.Visible =
                                                            LabelClientReference.Visible = true;
                        radioClientType.Enabled = false;
                        chkNDA.Enabled = false;
                    }
                    break;
                default:
                    LabelClientCompany.Visible =
                        LabelClientTitle.Visible =
                            LabelClientName.Visible =
                                LabelClientEmail.Visible =
                                    LabelClientPhoneNumber.Visible = 
                                        LabelClientDepartment.Visible =
                                            LabelClientStreet.Visible =
                                                LabelClientPLZ.Visible =
                                                    LabelClientCity.Visible =
                                                        LabelClientReference.Visible = true;
                    radioClientType.Enabled = false;
                    chkNDA.Enabled = false;
                    break;
            }

            switch (radioClientType.SelectedValue)
            {
                case "Intern":
                    divClientForm.Visible = false;
                    break;
                case "Company":
                    divClientForm.Visible = true;
                    divClientCompany.Visible = true;
                    divClientDepartment.Visible = true;
                    break;
                case "PrivatePerson":
                    divClientForm.Visible = true;
                    divClientCompany.Visible = false;
                    divClientDepartment.Visible = false;
                    break;
                default:
                    throw new Exception($"Unexpected radioClientType {radioClientType.SelectedValue}");
            }
        }

        #endregion

        #region Save

        private void SaveChanges()
        {
            switch(pageProject.State)
            {
                case ProjectState.Published:

                    if (pageProject.UserHasDepartmentManagerRights())
                    {
                        //Name
                        pageProject.Name = ProjectTitleAdmin.Text.FixupParagraph();

                        //Students
                        pageProject.LogStudent1Evento = string.IsNullOrWhiteSpace(Student1EventoAdmin.Text)
                            ? null
                            : Student1EventoAdmin.Text;
                        pageProject.LogStudent1Mail = string.IsNullOrWhiteSpace(Student1MailAdmin.Text)
                            ? null
                            : Student1MailAdmin.Text;
                        pageProject.LogStudent1FirstName = string.IsNullOrWhiteSpace(Student1FirstNameAdmin.Text)
                            ? null
                            : Student1FirstNameAdmin.Text;
                        pageProject.LogStudent1LastName = string.IsNullOrWhiteSpace(Student1LastNameAdmin.Text)
                            ? null
                            : Student1LastNameAdmin.Text;
                        pageProject.LogStudent2Evento = string.IsNullOrWhiteSpace(Student2EventoAdmin.Text)
                            ? null
                            : Student2EventoAdmin.Text;
                        pageProject.LogStudent2Mail = string.IsNullOrWhiteSpace(Student2MailAdmin.Text)
                            ? null
                            : Student2MailAdmin.Text;
                        pageProject.LogStudent2FirstName = string.IsNullOrWhiteSpace(Student2FirstNameAdmin.Text)
                            ? null
                            : Student2FirstNameAdmin.Text;
                        pageProject.LogStudent2LastName = string.IsNullOrWhiteSpace(Student2LastNameAdmin.Text)
                            ? null
                            : Student2LastNameAdmin.Text;

                        //Type and duration
                        pageProject.LogProjectType = DropType.SelectedValue == dropTypeImpossibleValue
                            ? null
                            : db.ProjectTypes.Single(t => t.Id == int.Parse(DropType.SelectedValue));

                        pageProject.LogProjectDuration = DropDuration.SelectedValue == dropDurationImpossibleValue
                            ? (byte?)null
                            : byte.Parse(DropDuration.SelectedValue);

                    }
                    break;
                case ProjectState.Ongoing:
                    
                    if (pageProject.UserHasAdvisor2Rights())
                    {
                        //Name
                        if (pageProject.CanEditTitle())
                        {
                            pageProject.Name = ProjectTitleAdmin.Text.FixupParagraph();
                        }

                        //Presentation
                        if (string.IsNullOrWhiteSpace(TextBoxLabelPresentationDate.Text) && string.IsNullOrWhiteSpace(TextBoxLabelPresentationTime.Text))
                        {
                            pageProject.LogDefenceDate = (DateTime?)null;
                        }
                        else
                        {
                            pageProject.LogDefenceDate = DateTime.ParseExact($"{TextBoxLabelPresentationDate.Text} {TextBoxLabelPresentationTime.Text}", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                        }

                        pageProject.LogDefenceRoom = string.IsNullOrWhiteSpace(TextBoxLabelPresentationRoom.Text)
                            ? null
                            : TextBoxLabelPresentationRoom.Text;
                        

                        //Language
                        switch (DropLanguage.SelectedValue)
                        {
                            case "1":
                                pageProject.LogLanguageEnglish = true;
                                pageProject.LogLanguageGerman = false;
                                break;
                            case "2":
                                pageProject.LogLanguageEnglish = false;
                                pageProject.LogLanguageGerman = true;
                                break;
                            default:
                                pageProject.LogLanguageGerman = null;
                                pageProject.LogLanguageEnglish = null;
                                break;
                        }

                        //WebSummary
                        pageProject.WebSummaryChecked = cbxWebSummaryChecked.Checked;

                        //Client
                        pageProject.ClientType = radioClientType.SelectedIndex;
                        pageProject.ClientCompany = txtClientCompanyAdmin.Text;
                        pageProject.ClientAddressTitle = drpClientTitleAdmin.SelectedValue == "1" ? "Herr" : "Frau";
                        pageProject.ClientPerson = txtClientNameAdmin.Text;
                        pageProject.ClientMail = txtClientEmailAdmin.Text;
                        pageProject.ClientPhoneNumber = txtClientPhoneNumberAdmin.Text;
                        pageProject.ClientAddressDepartment = txtClientDepartmentAdmin.Text == "" ? null : txtClientDepartmentAdmin.Text;
                        pageProject.ClientAddressStreet = txtClientStreetAdmin.Text == "" ? null : txtClientStreetAdmin.Text;
                        pageProject.ClientAddressPostcode = txtClientPLZAdmin.Text == "" ? null : txtClientPLZAdmin.Text;
                        pageProject.ClientAddressCity = txtClientCityAdmin.Text == "" ? null : txtClientCityAdmin.Text;
                        pageProject.ClientReferenceNumber = txtClientReferenceAdmin.Text == "" ? null : txtClientReferenceAdmin.Text;

                        pageProject.UnderNDA = chkNDA.Checked;

                    }

                    if (pageProject.UserHasAdvisor1Rights())
                    {
                        //BillingStatus
                        pageProject.BillingStatus = DropBillingStatus.SelectedIndex == 0
                            ? null
                            : db.BillingStatus.Single(b => b.Id == int.Parse(DropBillingStatus.SelectedValue));

                        //Grades
                        if (!string.IsNullOrWhiteSpace(NumGradeStudent1Admin.Text) && CheckGradeFieldStatus())
                        {
                            pageProject.LogGradeStudent1 = float.Parse(NumGradeStudent1Admin.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                        }

                        if (!string.IsNullOrWhiteSpace(NumGradeStudent2Admin.Text) && CheckGradeFieldStatus())
                        {
                            pageProject.LogGradeStudent2 = float.Parse(NumGradeStudent2Admin.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                        }
                    }

                    if (pageProject.UserHasDepartmentManagerRights())
                    {
                        //Students
                        pageProject.LogStudent1Evento = string.IsNullOrWhiteSpace(Student1EventoAdmin.Text)
                            ? null
                            : Student1EventoAdmin.Text;
                        pageProject.LogStudent1Mail = string.IsNullOrWhiteSpace(Student1MailAdmin.Text)
                            ? null
                            : Student1MailAdmin.Text;
                        pageProject.LogStudent1FirstName = string.IsNullOrWhiteSpace(Student1FirstNameAdmin.Text)
                            ? null
                            : Student1FirstNameAdmin.Text;
                        pageProject.LogStudent1LastName = string.IsNullOrWhiteSpace(Student1LastNameAdmin.Text)
                            ? null
                            : Student1LastNameAdmin.Text;
                        pageProject.LogStudent2Evento = string.IsNullOrWhiteSpace(Student2EventoAdmin.Text)
                            ? null
                            : Student2EventoAdmin.Text;
                        pageProject.LogStudent2Mail = string.IsNullOrWhiteSpace(Student2MailAdmin.Text)
                            ? null
                            : Student2MailAdmin.Text;
                        pageProject.LogStudent2FirstName = string.IsNullOrWhiteSpace(Student2FirstNameAdmin.Text)
                            ? null
                            : Student2FirstNameAdmin.Text;
                        pageProject.LogStudent2LastName = string.IsNullOrWhiteSpace(Student2LastNameAdmin.Text)
                            ? null
                            : Student2LastNameAdmin.Text;

                        //Type and duration
                        pageProject.LogProjectType = db.ProjectTypes.Single(t => t.Id == int.Parse(DropType.SelectedValue));
                        pageProject.LogProjectDuration = byte.Parse(DropDuration.SelectedValue);

                        //Expert
                        if (pageProject.LogProjectType.P6 && DropExpert.Visible)
                        {
                            pageProject.Expert = DropExpert.SelectedIndex == 0
                                ? null
                                : db.Experts.First(ex => ex.id == int.Parse(DropExpert.SelectedValue));
                        }
                    }
                    break;
                default:
                    throw new UnauthorizedAccessException();
            }
        }

        private string GenerateValidationMessageForSave()
        {
            switch(pageProject.State)
            {
                case ProjectState.Published:
                    return null;
                    
                case ProjectState.Ongoing:
                    //Presentation
                    if (string.IsNullOrWhiteSpace(TextBoxLabelPresentationDate.Text) ^ string.IsNullOrWhiteSpace(TextBoxLabelPresentationTime.Text))
                    {
                        return "Bitte füllen Sie das Präsentationsdatum und die Präsentationszeit aus.";
                    }

                    if (!string.IsNullOrWhiteSpace(TextBoxLabelPresentationDate.Text) && !string.IsNullOrWhiteSpace(TextBoxLabelPresentationTime.Text))
                    {
                        if(!DateTime.TryParseExact($"{TextBoxLabelPresentationDate.Text} {TextBoxLabelPresentationTime.Text}", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date))
                            return "Bitte geben Sie ein valides Präsentationsdatum an.";

                        if (date < pageProject.Semester?.StartDate || date > pageProject.Semester?.StartDate + TimeSpan.FromDays(360))
                            return "Bitte geben Sie ein valides Präsentationsdatum an.";
                    }

                    //Client
                    if (radioClientType.SelectedIndex != (int)ClientType.Internal && (txtClientCompanyAdmin.Text + txtClientNameAdmin.Text == "" || txtClientStreetAdmin.Text == "" || txtClientPLZAdmin.Text == "" || txtClientCityAdmin.Text == "" || txtClientEmailAdmin.Text == ""))
                    {
                        return "Bitte füllen Sie alle Pflichtfelder aus.";
                    }

                    //BillingStatus
                    if (DivBillingStatusAdmin.Visible)
                    {
                        var billingStatus = DropBillingStatus.SelectedIndex == 0
                            ? null
                            : db.BillingStatus.Single(b => b.Id == int.Parse(DropBillingStatus.SelectedValue));

                        if (billingStatus?.ShowAddressOnInfoPage == true && radioClientType.SelectedIndex == (int)ClientType.Internal)
                        {
                            return "Dieser Verrechnungsstatus ist nur bei externen Auftraggebern verfügbar.";
                        }
                    }

                    //Grades
                    if (!CheckGradeFieldStatus())
                    {
                        if (pageProject.LogGradeStudent1 != null && !string.IsNullOrWhiteSpace(NumGradeStudent1Admin.Text))
                        {
                            return "Bitte füllen Sie die Felder Websummary, Durchführungssprache und Verrechnungsstatus aus, um die Noten einzutragen.";
                        }

                        if (pageProject.LogGradeStudent2 != null && !string.IsNullOrWhiteSpace(NumGradeStudent2Admin.Text))
                        {
                            return "Bitte füllen Sie die Felder Websummary, Durchführungssprache und Verrechnungsstatus aus, um die Noten einzutragen.";
                        }
                    }
                    break;
                default:
                    return "In diesem Projektstatus sind keine Änderungen erlaubt.";
            }

            return null;
        }

        private string GenerateValidationMessageForKickoff()
        {
            //Type
            if (DropType.SelectedValue == dropTypeImpossibleValue)
                return "Bitte wählen Sie eine Projektart aus.";

            //Duration
            if (DropDuration.SelectedValue == dropDurationImpossibleValue)
                return "Bitte wählen Sie eine Projektdauer aus.";

            //Long IP6 ar no longer possible
            if (db.ProjectTypes.Single(t => t.Id == int.Parse(DropType.SelectedValue)).P6 && int.Parse(DropDuration.SelectedValue) == 2)
                return "Ein langes IP6 wird nicht mehr angeboten. Bitte wählen Sie \"Normal\"";

            //Students
            var stud1Evento = Student1EventoAdmin.Text;
            var stud1Mail = Student1MailAdmin.Text;
            var stud1FirstName = Student1FirstNameAdmin.Text;
            var stud1LastName = Student1LastNameAdmin.Text;
            var stud2Evento = Student2EventoAdmin.Text;
            var stud2Mail = Student2MailAdmin.Text;
            var stud2FirstName = Student2FirstNameAdmin.Text;
            var stud2LastName = Student2LastNameAdmin.Text;

            Regex studentMailRegex1 = new Regex(@".*\..*@students\.fhnw\.ch");
            Regex studentMailRegex2 = new Regex(@".*\..*@fhnw\.ch");


            //Student1
            if (string.IsNullOrWhiteSpace(stud1Evento))
                return "Bitte geben Sie die EventoID der ersten Person an.";

            if (string.IsNullOrWhiteSpace(stud1Mail))
                return "Bitte geben Sie die E-Mail-Adresse der ersten Person an.";

            System.Text.RegularExpressions.Match stud1match1 = studentMailRegex1.Match(stud1Mail);
            System.Text.RegularExpressions.Match stud1match2 = studentMailRegex2.Match(stud1Mail);
            if (!stud1match1.Success && !stud1match2.Success)
                return "Bitte geben Sie eine gültige E-Mail-Adresse der ersten Person an. (vorname.nachname@students.fhnw.ch oder vorname.nachname@fhnw.ch)";

            if (string.IsNullOrWhiteSpace(stud1FirstName))
                return "Bitte geben Sie den Vornamen der ersten Person an.";

            if (string.IsNullOrWhiteSpace(stud1LastName))
                return "Bitte geben Sie den Nachnamen der ersten Person an.";

            //Student2
            if (!string.IsNullOrWhiteSpace(stud2Evento)
                || !string.IsNullOrWhiteSpace(stud2Mail)
                || !string.IsNullOrWhiteSpace(stud2FirstName)
                || !string.IsNullOrWhiteSpace(stud2LastName))
            {
                if (string.IsNullOrWhiteSpace(stud2Evento))
                    return "Bitte geben Sie die EventoID der zweiten Person an.";

                if (string.IsNullOrWhiteSpace(stud2Mail))
                    return "Bitte geben Sie die E-Mail-Adresse der zweiten Person an.";

                System.Text.RegularExpressions.Match stud2match1 = studentMailRegex1.Match(stud2Mail);
                System.Text.RegularExpressions.Match stud2match2 = studentMailRegex2.Match(stud2Mail);
                if (!stud2match1.Success && !stud2match2.Success)
                    return "Bitte geben Sie eine gültige E-Mail-Adresse der zweiten Person an. (vorname.nachname@students.fhnw.ch oder vorname.nachname@fhnw.ch)";

                if (string.IsNullOrWhiteSpace(stud2FirstName))
                    return "Bitte geben Sie den Vornamen der zweiten Person an.";

                if (string.IsNullOrWhiteSpace(stud2LastName))
                    return "Bitte geben Sie den Nachnamen der zweiten Person an.";
            }

            return null;
        }

        private string GenerateValidationMessageForFinish()
        {
            //Permission
            if (!pageProject.UserHasAdvisor1Rights())
            {
                return "Nur Hauptbetreuer können das Projekt abschliessen.";
            }

            //Expert
            if (pageProject.LogProjectType.P6)
            {
                if (pageProject.UserHasDepartmentManagerRights() && DropExpert.SelectedValue == dropExpertImpossibleValue)
                {
                    return "Bitte wählen Sie einen Experten aus.";
                }

                if (!pageProject.UserHasDepartmentManagerRights() && pageProject.Expert == null)
                {
                    return "Um das Projekt abzuschliessen muss der Experte gesetzt sein.";
                }
            }

            //Language
            if (DropLanguage.SelectedValue == dropLanguageImpossibleValue)
                return "Bitte geben Sie die Durchführungssprache an.";

            //WebSummary
            if (!cbxWebSummaryChecked.Checked)
                return "Bitte prüfen Sie das WebSummary";

            //BillingStatus
            if (DropBillingStatus.SelectedValue == dropBillingStatusImpossibleValue)
                return "Bitte geben Sie den Verrechnungsstatus an.";

            //Grades
            if (!string.IsNullOrWhiteSpace(pageProject.LogStudent1Mail))
            {
                if (string.IsNullOrWhiteSpace(NumGradeStudent1Admin.Text))
                    return $"Bitte geben Sie die Note von {pageProject.GetStudent1FullName()} an.";

                if (float.TryParse(NumGradeStudent1Admin.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out float grade))
                {
                    if (grade < 1 || grade > 6)
                        return "Bitte geben Sie eine Note im Bereich 1-6 an.";
                }
                else
                {
                    return "Bitte geben Sie die Note korrekt an (z.B. 4 oder 4.5)";
                }
            }
            else
            {
                throw new InvalidOperationException("No Student1 in State Ongoing is not allowed");
            }

            if (!string.IsNullOrWhiteSpace(pageProject.LogStudent2Mail))
            {
                if (string.IsNullOrWhiteSpace(NumGradeStudent2Admin.Text))
                    return $"Bitte geben Sie die Note von {pageProject.GetStudent2FullName()} an.";

                if (float.TryParse(NumGradeStudent2Admin.Text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out float grade))
                {
                    if (grade < 1 || grade > 6)
                        return "Bitte geben Sie eine Note im Bereich 1-6 an.";
                }
                else
                {
                    return "Bitte geben Sie die Note korrekt an (z.B. 4 oder 4.5)";
                }
            }
            
            //Client
            if (radioClientType.SelectedIndex != (int)ClientType.Internal)
            {
                if (string.IsNullOrWhiteSpace(txtClientEmailAdmin.Text))
                    return "Bitte geben Sie die E-Mail des Kunden an.";

                if (radioClientType.SelectedIndex == (int)ClientType.Company && string.IsNullOrWhiteSpace(txtClientCompanyAdmin.Text))
                    return "Bitte geben Sie den Namen des Unternehmen an.";
            }

            //
            //Department specific
            //

            if (pageProject.Department.i4DS)
            {
                //Attachment
                if (!pageProject.Attachements.Any(a => !a.Deleted))
                {
                    return "Bitte laden Sie die Dokumentation des Projektes hoch.";
                }
            }
            else if (pageProject.Department.IIT)
            {
                //Attachment
                if (!pageProject.Attachements.Any(a => !a.Deleted))
                {
                    return "Bitte laden Sie die Dokumentation des Projektes hoch.";
                }
            }
            else if (pageProject.Department.IMVS)
            {
                //None
            }
            else
            {
                return $"Das Projekt hat kein Institut. Bitte kontaktiere {Global.WebAdmin}";
            }
            
            return null;
        }

        private string GenerateValidationMessageForCancel()
        {
            //Permission
            if (!pageProject.UserHasAdvisor1Rights())
            {
                return "Nur Hauptbetreuer können das Projekt abbrechen.";
            }

            return null;
        }

        private void ReturnAlert(string message)
        {
            var sb = new StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("window.onload=function(){");
            sb.Append("alert('");
            sb.Append(message);
            sb.Append("')};");
            sb.Append("</script>");
            ClientScript.RegisterClientScriptBlock(GetType(), "alert", sb.ToString());
        }

        #endregion

        #region Attachments

        private ProjectSingleAttachment GetProjectSingleAttachment(Attachements attach)
        {
            return new ProjectSingleAttachment
            {
                Guid = attach.ROWGUID,
                BaseVersionId = attach.ProjectId,
                Name = attach.FileName,
                Size = FixupSize((long)(attach.UploadSize ?? 0)),
                FileType = GetFileTypeImgPath(attach.FileName)
            };
        }

        private string GetFileTypeImgPath(string filename)
        {
            switch (filename.Split('.').Last())
            {
                case "pdf":
                    return "Content/pdf.png";
                case "pptx":
                    return "Content/ppt.png";
                case "docx":
                    return "Content/doc.png";
                case "xlsx":
                    return "Content/xls.png";
                case "zip":
                case "rar":
                case "7z":
                    return "Content/zip.png";
                case "png":
                case "jpg":
                case "jpeg":
                    return "Content/jpg.png";
                default:
                    return "Content/file.png";
            }
        }

        private string FixupSize(long size)
        {
            if (size < 1024) //bytes
                return size + " B";
            if (size / 1024 < 1024) //kilobytes
                return size / 1024 + " KB";
            if (size / (1024 * 1024) < 1024)
                return size / (1024 * 1024) + " MB";

            return Math.Round(size / ((float)1024 * 1024 * 1024), 2) + " GB";
        }

        protected void OnUploadComplete(object sender, AjaxFileUploadEventArgs e)
        {
            if (pageProject is null)
            {
                id = int.Parse(Request.QueryString["id"]);
                pageProject = db.Projects.Single(p => p.Id == id);
            }

            using (var s = e.GetStreamContents())
            if (db.Attachements.Any(a => a.ProjectId == pageProject.Id && a.FileName == e.FileName && !a.Deleted))
            {
                SaveFileInDb(db.Attachements.Single(a => a.ProjectId == pageProject.Id && a.FileName == e.FileName && !a.Deleted), s);
            }
            else
            {
                var attachement = CreateNewAttach(e.FileSize, e.FileName);
                SaveFileInDb(attachement, s);
            }

            divDownloadBtn.Visible = true;

            var di = new DirectoryInfo(Path.GetTempPath() + "_AjaxFileUpload");

            foreach (var dir in di.GetDirectories())
                try
                {
                    dir.Delete(true);
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        private Attachements CreateNewAttach(long fileSize, string fileName)
        {
            var attach = new Attachements
            {
                ProjectId = pageProject.Id,
                UploadUserMail = ShibUser.GetEmail(),
                UploadDate = DateTime.Now,
                UploadSize = fileSize,
                ProjectAttachement = new Binary(new byte[0]),
                FileName = fileName
            };
            db.Attachements.InsertOnSubmit(attach);
            db.SubmitChanges();

            return attach;
        }

        private void SaveFileInDb(Attachements attach, Stream file)
        {
            using (var connection = new SqlConnection(db.Connection.ConnectionString))
            {
                connection.Open();

                var command =
                    new SqlCommand(
                        "SELECT TOP(1) ProjectAttachement.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Attachements WHERE ROWGUID = @ROWGUID",
                        connection);
                command.Parameters.AddWithValue("@ROWGUID", attach.ROWGUID.ToString());

                using (var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    command.Transaction = tran;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the pointer for file
                            var pathfile = reader.GetString(0);
                            var transactionContext = reader.GetSqlBytes(1).Buffer;

                            using (
                                Stream fileStream = new SqlFileStream(pathfile, transactionContext, FileAccess.Write,
                                    FileOptions.SequentialScan, 0))
                            {
                                file.CopyTo(fileStream, 65536);
                            }
                        }
                    }
                    tran.Commit();
                }
            }
        }

        protected void GridProjectAttachs_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            // var project = db.Projects.Single(item => item.Id == ((ProjectSingleAttachment)e.Row.DataItem).BaseVersionId);

            if (!pageProject.UserHasAdvisor2Rights())
                e.Row.Cells[e.Row.Cells.Count - 1].Visible = false;

            try
            {
                e.Row.Attributes.Add("onmouseover",
                    "this.style.backgroundColor='#cecece'; this.style.color='Black'; this.style.cursor='pointer'");
                e.Row.Attributes.Add("onmouseout", "this.style.color='Black';this.style.backgroundColor='#FFFFFF';");
                e.Row.Attributes.Add("onclick",
                    Page.ClientScript.GetPostBackEventReference(gridProjectAttachs, "Select$" + e.Row.RowIndex));
            }
            catch
            {
                // ignored
            }

            divDownloadBtn.Visible = true;
            updateDownloadButton.Update();
        }

        protected void GridProjectAttachs_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "deleteProjectAttach") return;
            var guid = new Guid(e.CommandArgument.ToString());
            var attach = db.Attachements.Single(a => a.ROWGUID == guid);
            attach.DeletedDate = DateTime.Now;
            attach.Deleted = true;
            attach.DeletedUser = ShibUser.GetEmail();
            db.SubmitChanges();


            gridProjectAttachs.DataSource = db.Attachements.Where(item => item.ProjectId == pageProject.Id && !item.Deleted)
                .Select(i => GetProjectSingleAttachment(i));
            gridProjectAttachs.DataBind();

            updateProjectAttachements.Update();
            divDownloadBtn.Visible = gridProjectAttachs.Rows.Count > 0;
            updateDownloadButton.Update();
        }

        protected void GridProjectAttachs_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect("ProjectFilesDownload?guid=" + (Guid)gridProjectAttachs.SelectedValue);
        }

        protected void DownloadFiles_OnClick(object sender, EventArgs e)
        {
            var attachments = db.Attachements.Where(item => item.ProjectId == pageProject.Id && !item.Deleted).ToList();

            if (!ShibUser.IsAuthenticated(db))
            {
                Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.PathAndQuery}");
                Response.End();
                return;
            }
            using (var memoryStream = new MemoryStream())
            {
                var zip = new ZipFile(memoryStream);
                decimal? totalSize = 0;
                using (var connection = new SqlConnection(db.Connection.ConnectionString))
                {
                    connection.Open();
                    foreach (var attachment in attachments)
                    {
                        totalSize += attachment.UploadSize;
                        var command =
                            new SqlCommand(
                                $"SELECT TOP(1) ProjectAttachement.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Attachements WHERE ROWGUID = @ROWGUID;",
                                connection);
                        command.Parameters.AddWithValue("@ROWGUID", attachment.ROWGUID.ToString());
                        using (var tran = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            command.Transaction = tran;

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Get the pointer for the file  
                                    var path = reader.GetString(0);
                                    var transactionContext = reader.GetSqlBytes(1).Buffer;

                                    //used to save a stream to a zipfile
                                    var streamToZipDataSource = new StreamToZipDataSource();

                                    // Create the SqlFileStream  
                                    using (
                                        Stream fileStream = new SqlFileStream(path, transactionContext,
                                            FileAccess.Read,
                                            FileOptions.SequentialScan, 0))
                                    {
                                        //update zipfile
                                        zip.BeginUpdate();
                                        streamToZipDataSource.SetStream(fileStream);
                                        zip.Add(streamToZipDataSource, attachment.FileName);
                                        zip.CommitUpdate();
                                        zip.IsStreamOwner = false;
                                    }
                                }
                            }
                            tran.Commit();
                        }
                    }
                    if (totalSize == 0)
                    {
                        return;
                    }
                }
                zip.Close();
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + pageProject.Name + ".zip" + "\"");
                Response.AddHeader("Content-Length", totalSize.ToString());
                Response.ContentType = "text/plain";
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        #endregion
    }
    public class StreamToZipDataSource : IStaticDataSource
    {
        private Stream _stream;

        public Stream GetSource()
        {
            return _stream;
        }


        public void SetStream(Stream inputStream)
        {
            _stream = inputStream;
            _stream.Position = 0;
        }
    }

    public class ProjectSingleAttachment
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int BaseVersionId { get; set; }
        public string FileType { get; set; }
    }
}