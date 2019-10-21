using System;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace ProStudCreator
{
    public partial class ProjectEditPage : Page
    {
        public readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project pageProject;
        private bool[] projectTopics = new bool[8];
        private bool imageChanged = false;
        private bool imageDeleted = false;
        //private Binary picture;

        private readonly string dropSemesterImpossibleValue = "dropSemesterImpossibleValue";
        private readonly string dropPreviousProjectImpossibleValue = "dropPreviousProjectImpossibleValue";
        private readonly string dropAdvisor1ImpossibleValue = "dropAdvisor1ImpossibleValue";
        private readonly string dropAdvisor2ImpossibleValue = "dropAdvisor2ImpossibleValue";
        private readonly string dropPTwoTypeImpossibleValue = "dropPTwoTypeImpossibleValue";
        private readonly string dropPTwoTeamSizeImpossibleValue = "dropPTwoTeamSizeImpossibleValue";

        #region Timer tick

        protected void Pdfupdatetimer_Tick(object sender, EventArgs e) //function for better workflow with long texts
        {
            if (pageProject != null)
            {
                var pdfc = new PdfCreator();

                //TODO: change it so this isn't needed (don't change database object every tick)
                UpdateProjectFromFormData(pageProject);

                if (pdfc.CalcNumberOfPages(pageProject) > 1)
                {
                    Pdfupdatelabel.Text = "Länge: Das PDF ist länger als eine Seite!";
                    Pdfupdatelabel.ForeColor = Color.Red;
                }
                else
                {
                    Pdfupdatelabel.Text = "Länge: OK (1 Seite)";
                    Pdfupdatelabel.ForeColor = Color.Green;
                }
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            AdminView.Visible = ShibUser.CanSeeCreationDetails();
            updateReservation.Visible = ShibUser.CanReserveProjects();

            //Button color
            submitProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Submitted));
            rollbackProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.InProgress));
            refuseProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Rejected));
            publishProject.BackColor = ColorTranslator.FromHtml(Project.GetStateColor(ProjectState.Published));

            btnHistoryCollapse.CausesValidation = false;
            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                pageProject = db.Projects.Single(p => (int?)p.Id == id);

                // if loaded page is not main version -> redirect outdated error
                if (!pageProject.IsMainVersion && Request.QueryString["showChanges"] == null)
                {
                    var main = db.Projects.Single(p => p.BaseVersionId == pageProject.BaseVersionId && p.IsMainVersion);
                    Response.Redirect("~/ProjectEditPage.aspx?id=" + main.Id);
                    Response.End();
                }

                if (!pageProject.UserCanEdit())
                {
                    Response.Redirect("error/AccessDenied.aspx?url=" + HttpContext.Current.Request.Url.AbsoluteUri);
                    Response.End();
                }

                /* //TODO
                var history = db.Projects.Where(p => p.BaseVersionId == pageProject.BaseVersionId && !p.IsMainVersion);
                if (history.ToList().Count > 0)
                {
                    historyListView.DataSource = history;
                    historyListView.DataBind();
                }
                else
                {
                    DivHistoryCollapsable.InnerText = "Keine früheren Versionen vorhanden.";
                }
                */

            }
            else
            {
                divHistory.Visible = false;
                if (Request.QueryString["showChanges"] == null)
                    duplicateProject.Visible = false;
            }



            if (IsPostBack)
            {
                projectTopics = (bool[])ViewState["Topics"];
            }
            else
            {
                if (Session["AddInfoCollapsed"] == null)
                    CollapseHistory(true);
                else
                    CollapseHistory((bool)Session["AddInfoCollapsed"]);

                if (id.HasValue)
                {
                    //existing project
                    Page.Title = "Projekt bearbeiten";
                    SiteTitle.Text = "Projekt bearbeiten";

                    if (!pageProject.IsMainVersion && Request.QueryString["showChanges"] == null)
                    {
                        //TODO
                        //PopulateHistoryGUI(pageProject.Id);
                    }
                    else
                    {
                        UpdateUIFromProjectObject(false);
                    }
                }
                else
                {
                    //new project
                    Page.Title = "Neues Projekt";
                    SiteTitle.Text = "Neues Projekt anlegen";

                    UpdateUIFromProjectObject(true);
                }
            }
        }

        #region Form

        private void UpdateUIFromProjectObject(bool isNewProject)
        {
            if (Request.QueryString["showChanges"] != null)
            {
                //disable pdf update for showChanges view
                Pdfupdatetimer.Enabled = false;

                PopulateHistoryGUI();
                return;
            }

            CreatorID.Text = pageProject?.Creator + "/" + pageProject?.CreateDate.ToString("yyyy-MM-dd") ?? "";

            //Name
            ProjectName.Text = pageProject?.Name ?? "";

            //Semester
            FillDropSemester(isNewProject);

            //Previous project
            FillDropPreviousProject(isNewProject);

            //Advisor
            FillDropAdvisors(isNewProject);

            //Department
            FillDropDepartment(isNewProject);

            //Language
            FillDropLanguage(isNewProject);

            //Priority
            FillDropPriority(isNewProject);
            DisplayPriority();

            //Client
            DisplayClient(isNewProject);

            //NDA
            chkNDA.Checked = pageProject?.UnderNDA ?? false;

            //Reservation
            DisplayReservations();
            ToggleReservationTwoVisible();

            //ProjectTopics
            DisplayTopics(isNewProject);

            //Picture
            DisplayPicture(isNewProject);

            //Textboxes
            InitialPositionContent.Attributes.Add("placeholder",
                "Beispiel: Die Open-Source WebGL-Library three.js stellt Benutzern einen einfachen Editor zum Erstellen von 3D-Szenen zur Verfügung. Eine Grundversion dieses Editors ist unter http://threejs.org/editor abrufbar. Dieser Editor wird als Basis für die hochschulübergreifende strategische Initiative „Playful Media Practices“ verwendet, wo er zum Design audiovisueller Szenen verwendet wird. Diesem Editor fehlt jedoch eine Undo-/Redo-Funktion, welche in diesem Projekt hinzuzufügen ist.");
            ObjectivContent.Attributes.Add("placeholder",
                "Beispiel: Das Ziel dieser Arbeit ist die Erarbeitung eines Undo-/Redo-Konzepts für den three.js-Editor sowie dessen Implementation. Da three.js eine Library für die cutting-edge-Technologie WebGL ist, nutzt auch der three.js-Editor modernste Browsermittel wie LocalStorage oder FileAPI. Deshalb gilt es nicht, die Implementation kompatibel zu alten Browsern zu halten, sondern das Maximum aus aktuellen Browsern zu holen.");
            ProblemStatementContent.Attributes.Add("placeholder",
                "Beispiel: Der three.js-Editor hat mittlerweile eine beachtliche Komplexität erreicht, entsprechend muss für verschiedene Bereiche anders mit Undo&Redo umgegangen werden. Wenn beispielsweise jemand neue Texturen hochlädt, müssen die vorherigen Texturen im Speicher behalten werden.");
            ReferencesContent.Attributes.Add("placeholder",
                "Beispiel:\n- JavaScript\n- Komplexe Datenstrukturen\n- Three.js/WebGL");
            RemarksContent.Attributes.Add("placeholder",
                "Beispiel: Ein Pullrequest der Implementation wird diese Erweiterung einem weltweiten Publikum öffentlich zugänglich machen. Sie leisten damit einen entscheidenden Beitrag für die Open-Source Community von three.js!");
            NotesContent.Attributes.Add("placeholder",
                "Notizen (werden nicht auf dem PDF angezeigt)");

            InitialPositionContent.Text = pageProject?.InitialPosition ?? "";
            ObjectivContent.Text = pageProject?.Objective ?? "";
            ProblemStatementContent.Text = pageProject?.ProblemStatement ?? "";
            ReferencesContent.Text = pageProject?.References ?? "";
            RemarksContent.Text = pageProject?.Remarks ?? "";
            NotesContent.Text = pageProject?.Notes ?? "";

            //Button visibility
            saveProject.Visible = true;
            if (id.HasValue)
            {
                submitProject.Visible = pageProject.UserCanSubmit();
                if ((pageProject.State == ProjectState.InProgress || pageProject.State == ProjectState.Rejected)
                    && (pageProject.UserIsAdvisor2() || pageProject.UserIsCreator())
                    && !pageProject.UserHasAdvisor1Rights())
                {
                    submitProject.Visible = true;
                    submitProject.Enabled = false;
                    submitProject.Text = "Nur Hauptbetreuer können Projekte einreichen";
                }
            }
            //submitProject.Visible = id.HasValue && pageProject.UserCanSubmit();
            publishProject.Visible = pageProject?.UserCanPublish() ?? false;
            refuseProject.Visible = pageProject?.UserCanReject() ?? false;
            rollbackProject.Visible = pageProject?.UserCanUnsubmit() ?? false;
        }

        private void FillDropSemester(bool isNewProject)
        {
            if (ShibUser.GetEmail() == Global.WebAdmin && (pageProject?.State ?? 0) <= ProjectState.Published)
            {
                dropSemester.DataSource = db.Semester.OrderBy(s => s.StartDate);
                dropSemester.DataBind();
                dropSemester.Items.Insert(0, new ListItem("-", dropSemesterImpossibleValue));

                if (isNewProject)
                {
                    dropSemester.SelectedValue = Semester.NextSemester(db).Id.ToString();
                }
                else
                {
                    dropSemester.SelectedValue = pageProject?.Semester?.Id.ToString() ?? dropSemesterImpossibleValue;
                }
            }
            else
            {
                DivSemester.Visible = false;
            }
        }

        private void FillDropPreviousProject(bool isNewProject)
        {
            Semester projectSemester;

            if (DivSemester.Visible && dropSemester.SelectedValue != dropSemesterImpossibleValue)
            {
                projectSemester = db.Semester.Single(s => s.Id.ToString() == dropSemester.SelectedValue);
            }
            else
            {
                projectSemester = Semester.NextSemester(db);
            }

            var lastSem = Semester.LastSemester(projectSemester, db);
            var lastlastSem = Semester.LastSemester(lastSem, db);

            if (lastSem != null)
            {
                if (lastlastSem != null)
                {
                    dropPreviousProject.DataSource = db.Projects.Where(p =>
                            p.IsMainVersion
                        && p.LogProjectType.P5
                        && !p.LogProjectType.P6
                        && (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.ArchivedFinished)
                        && (p.SemesterId == lastSem.Id || (p.LogProjectDuration == 2 && lastlastSem != null && p.SemesterId == lastlastSem.Id)))
                        .OrderBy(p => p.Name);
                }
                else
                {
                    dropPreviousProject.DataSource = db.Projects.Where(p =>
                            p.IsMainVersion
                        &&  p.LogProjectType.P5
                        && !p.LogProjectType.P6
                        && (p.State == ProjectState.Ongoing || p.State == ProjectState.Finished || p.State == ProjectState.ArchivedFinished)
                        && (p.SemesterId == lastSem.Id))
                        .OrderBy(p => p.Name);
                }
            }
            else
            {
                dropPreviousProject.DataSource = new List<Semester>();
            }
            dropPreviousProject.DataBind();
            dropPreviousProject.Items.Insert(0, new ListItem("-", dropPreviousProjectImpossibleValue));

            if (pageProject?.PreviousProject != null && dropPreviousProject.Items.FindByValue(pageProject.PreviousProject.Id.ToString()) == null)
            {
                dropPreviousProject.Items.Add(new ListItem(pageProject.PreviousProject.Name, pageProject.PreviousProject.Id.ToString()));
            }

            if (isNewProject || lastSem == null)
            {
                dropPreviousProject.SelectedValue = dropPreviousProjectImpossibleValue;
            }
            else
            {
                var val = pageProject?.PreviousProject?.Id.ToString() ?? dropPreviousProjectImpossibleValue;
                dropPreviousProject.SelectedValue = dropPreviousProject.Items.FindByValue(val)?.Value ?? dropPreviousProjectImpossibleValue;
            }

            bool hasPreviousProject = dropPreviousProject.SelectedValue != dropPreviousProjectImpossibleValue;
            PrepareForm(hasPreviousProject);
        }

        private void FillDropAdvisors(bool isNewProject)
        {
            if (isNewProject)
            {
                dropAdvisor1.DataSource = db.UserDepartmentMap.Where(i => i.CanBeAdvisor1 && i.IsActive).OrderBy(a => a.Name);
                dropAdvisor2.DataSource = db.UserDepartmentMap.Where(i => i.IsActive).OrderBy(a => a.Name);
            }
            else
            {
                dropAdvisor1.DataSource = db.UserDepartmentMap.Where(i => i.CanBeAdvisor1 && (i.IsActive || i.Id == pageProject.Advisor1Id)).OrderBy(a => a.Name);
                dropAdvisor2.DataSource = db.UserDepartmentMap.Where(i => i.IsActive || i.Id == pageProject.Advisor2Id).OrderBy(a => a.Name);
            }
            dropAdvisor1.DataBind();
            dropAdvisor1.Items.Insert(0, new ListItem("-", dropAdvisor1ImpossibleValue));
            dropAdvisor1.SelectedValue = pageProject?.Advisor1?.Id.ToString() ?? dropAdvisor1ImpossibleValue;

            dropAdvisor2.DataBind();
            dropAdvisor2.Items.Insert(0, new ListItem("-", dropAdvisor2ImpossibleValue));
            if (isNewProject)
            {
                dropAdvisor2.SelectedValue = db.UserDepartmentMap.First(u => u.Mail == ShibUser.GetEmail())?.Id.ToString() ?? dropAdvisor2ImpossibleValue;
            }
            else
            {
                dropAdvisor2.SelectedValue = pageProject?.Advisor2?.Id.ToString() ?? dropAdvisor2ImpossibleValue;
            }
        }

        private void FillDropDepartment(bool isNewProject)
        {
            dropDepartment.DataSource = db.Departments;
            dropDepartment.DataBind();
            if (isNewProject)
            {
                dropDepartment.SelectedValue = ShibUser.GetDepartment()?.Id.ToString() ?? 0.ToString();
            }
            else
            {
                dropDepartment.SelectedValue = pageProject.Department.Id.ToString();
            }
        }

        private void FillDropLanguage(bool isNewProject)
        {
            if (isNewProject)
            {
                dropLanguage.SelectedIndex = 0;
            }
            else
            {
                if (pageProject.LanguageEnglish && !pageProject.LanguageGerman)
                {
                    dropLanguage.SelectedIndex = 2;
                }
                else if (!pageProject.LanguageEnglish && pageProject.LanguageGerman)
                {
                    dropLanguage.SelectedIndex = 1;
                }
                else
                {
                    dropLanguage.SelectedIndex = 0;
                }
            }
        }

        private void FillDropPriority(bool isNewProject)
        {
            dropPOneType.DataSource = db.ProjectTypes;
            dropPOneType.DataBind();

            dropPOneTeamSize.DataSource = db.ProjectTeamSizes;
            dropPOneTeamSize.DataBind();

            dropPTwoType.DataSource = db.ProjectTypes;
            dropPTwoType.DataBind();
            dropPTwoType.Items.Insert(0, new ListItem("-", dropPTwoTypeImpossibleValue));

            dropPTwoTeamSize.DataSource = db.ProjectTeamSizes;
            dropPTwoTeamSize.DataBind();
            dropPTwoTeamSize.Items.Insert(0, new ListItem("-", dropPTwoTeamSizeImpossibleValue));

            if (isNewProject)
            {
                dropPOneType.SelectedValue = db.ProjectTypes.Single(t => t.P5 && t.P6).Id.ToString();
                dropPOneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(s => s.Size1 && s.Size2).Id.ToString();
                dropPTwoType.SelectedValue = dropPTwoTypeImpossibleValue;
                dropPTwoTeamSize.SelectedValue = dropPTwoTeamSizeImpossibleValue;
            }
            else
            {
                dropPOneType.SelectedValue = pageProject.POneType.Id.ToString();
                dropPOneTeamSize.SelectedValue = pageProject.POneTeamSize.Id.ToString();
                dropPTwoType.SelectedValue = pageProject.PTwoType?.Id.ToString() ?? dropPTwoTypeImpossibleValue;
                dropPTwoTeamSize.SelectedValue = pageProject.PTwoTeamSize?.Id.ToString() ?? dropPTwoTeamSizeImpossibleValue;
            }
        }

        private void DisplayPicture(bool isNewProject, bool imageDeleted = false)
        {
            if (!isNewProject && pageProject.Picture != null && !imageDeleted)
            {
                AddPictureLabel.Text = "Bild ändern:";
                ProjectPicture.Visible = true;
                ProjectPicture.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(pageProject.Picture.ToArray());
                DeleteImageButton.Visible = true;
                imgdescription.Text = pageProject.ImgDescription;
            }
            else
            {
                AddPictureLabel.Text = "Bild hinzufügen:";
                ImageLabel.Visible = false;
                ProjectPicture.Visible = false;
                DeleteImageButton.Visible = false;
            }
        }

        private void DisplayReservations()
        {
            var previousProject = db.Projects.SingleOrDefault(p => p.Id.ToString() == dropPreviousProject.SelectedValue);

            if (previousProject == null)
            {
                Reservation1Name.Enabled = Reservation1Mail.Enabled = Reservation2Mail.Enabled = Reservation2Name.Enabled = true;
                Reservation1Name.Text = pageProject?.Reservation1Name ?? "";
                Reservation1Mail.Text = pageProject?.Reservation1Mail ?? "";
                Reservation2Name.Text = pageProject?.Reservation2Name ?? "";
                Reservation2Mail.Text = pageProject?.Reservation2Mail ?? "";
            }
            else
            {
                Reservation1Name.Enabled = Reservation1Mail.Enabled = Reservation2Mail.Enabled = Reservation2Name.Enabled = false;
                Reservation1Mail.Text = previousProject.LogStudent1Mail;
                Reservation2Mail.Text = previousProject.LogStudent2Mail;
                Reservation1Name.Text = previousProject.LogStudent1Name;
                Reservation2Name.Text = previousProject.LogStudent2Name;
            }
        }

        private void DisplayClient(bool isNewProject)
        {
            if (isNewProject)
            {
                drpClientTitle.SelectedIndex = 0; //Default

                txtClientCompany.Text =
                    txtClientName.Text =
                        txtClientDepartment.Text =
                            txtClientStreet.Text =
                                txtClientPLZ.Text =
                                    txtClientCity.Text =
                                        txtClientReference.Text =
                                            txtClientEmail.Text = 
                                                txtClientPhoneNumber.Text = "";

                divClientForm.Visible = false;
                radioClientType.SelectedIndex = (int)ClientType.Internal;
            }
            else
            {
                txtClientCompany.Text = pageProject?.ClientCompany;
                if (pageProject?.ClientAddressTitle != null)
                {
                    drpClientTitle.SelectedIndex = pageProject?.ClientAddressTitle == "Herr" ? 1 : 2;
                }
                else
                {
                    drpClientTitle.SelectedIndex = 0;
                }
                txtClientName.Text = pageProject?.ClientPerson;
                txtClientDepartment.Text = pageProject?.ClientAddressDepartment;
                txtClientStreet.Text = pageProject?.ClientAddressStreet;
                txtClientPLZ.Text = pageProject?.ClientAddressPostcode;
                txtClientCity.Text = pageProject?.ClientAddressCity;
                txtClientReference.Text = pageProject?.ClientReferenceNumber;
                txtClientEmail.Text = pageProject?.ClientMail;
                txtClientPhoneNumber.Text = pageProject?.ClientPhoneNumber;

                PrepareClientForm();
            }
        }


        private void DisplayPriority()
        {
            var previousProject = db.Projects.SingleOrDefault(p => p.Id.ToString() == dropPreviousProject.SelectedValue);

            if (previousProject == null)
            {
                dropPOneTeamSize.Enabled = dropPOneType.Enabled = dropPTwoTeamSize.Enabled = dropPTwoType.Enabled = true;
                divPriorityTwo.Visible = true;
            }
            else
            {
                divPriorityTwo.Visible = false;
                dropPOneTeamSize.Enabled = dropPOneType.Enabled = dropPTwoTeamSize.Enabled = dropPTwoType.Enabled = false;
                dropPOneType.SelectedValue = db.ProjectTypes.Single(p => !p.P5 && p.P6).Id.ToString();
                if (string.IsNullOrEmpty(previousProject?.LogStudent2Mail))
                    dropPOneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => p.Size1 && !p.Size2).Id.ToString();
                else
                    dropPOneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => !p.Size1 && p.Size2).Id.ToString();
            }
        }

        private void ToggleReservationTwoVisible()
        {
            var showResTwo = dropPOneTeamSize.SelectedIndex != 0
                         || (dropPTwoTeamSize.SelectedIndex != 0 && dropPTwoTeamSize.SelectedIndex != 1);

            Reservation2Name.Visible = Reservation2Mail.Visible = showResTwo;
        }

        private void PrepareForm(bool hasPreviousProj)
        {
            //Priority
            divPriorityTwo.Visible = dropPOneTeamSize.Enabled = dropPOneType.Enabled = dropPTwoTeamSize.Enabled = dropPTwoType.Enabled = !hasPreviousProj;
            //Reservations
            Reservation1Name.Enabled = Reservation1Mail.Enabled = Reservation2Name.Enabled = Reservation2Mail.Enabled = !hasPreviousProj;
        }

        private void PrepareClientForm()
        {
            switch (pageProject.ClientType)
            {
                case (int)ClientType.Company:
                    divClientCompany.Visible = divClientForm.Visible = true;
                    radioClientType.SelectedIndex = (int)ClientType.Company;
                    break;

                case (int)ClientType.PrivatePerson:
                    divClientCompany.Visible = false;
                    divClientForm.Visible = true;
                    radioClientType.SelectedIndex = (int)ClientType.PrivatePerson;
                    break;

                case (int)ClientType.Internal:
                    divClientCompany.Visible = divClientForm.Visible = false;
                    radioClientType.SelectedIndex = (int)ClientType.Internal;
                    break;
            }
        }

        private void DisplayTopics(bool isNewProject)
        {
            if (!isNewProject)
            {
                if (pageProject.TypeDesignUX)
                {
                    DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                    projectTopics[0] = true;
                }
                if (pageProject.TypeHW)
                {
                    HW.ImageUrl = "pictures/projectTypHW.png";
                    projectTopics[1] = true;
                }
                if (pageProject.TypeCGIP)
                {
                    CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                    projectTopics[2] = true;
                }
                if (pageProject.TypeMlAlg)
                {
                    MlAlg.ImageUrl = "pictures/projectTypMlAlg.png";
                    projectTopics[3] = true;
                }
                if (pageProject.TypeAppWeb)
                {
                    AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                    projectTopics[4] = true;
                }
                if (pageProject.TypeDBBigData)
                {
                    DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                    projectTopics[5] = true;
                }
                if (pageProject.TypeSysSec)
                {
                    SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                    projectTopics[6] = true;
                }
                if (pageProject.TypeSE)
                {
                    SE.ImageUrl = "pictures/projectTypSE.png";
                    projectTopics[7] = true;
                }
            }

            ViewState["Topics"] = projectTopics;
        }

        #endregion

        #region Save

        private void UpdateProjectFromFormData(Project project)
        {
            //Name
            project.Name = ProjectName.Text.FixupParagraph();

            //Semester
            if (DivSemester.Visible)
            {
                if (dropSemester.SelectedValue == dropSemesterImpossibleValue)
                {
                    project.Semester = null;
                }
                else
                {
                    project.Semester = db.Semester.Single(s => s.Id == int.Parse(dropSemester.SelectedValue));
                }
            }

            //Previous Project
            if (dropPreviousProject.SelectedValue == dropPreviousProjectImpossibleValue)
            {
                project.PreviousProject = null;
            }
            else
            {
                project.PreviousProject = db.Projects.Single(p => p.Id == int.Parse(dropPreviousProject.SelectedValue));
            }

            //Advisor
            if (dropAdvisor1.SelectedValue == dropAdvisor1ImpossibleValue)
                project.Advisor1 = null;
            else
                project.Advisor1 = db.UserDepartmentMap.Single(user => user.Id == int.Parse(dropAdvisor1.SelectedValue));

            if (dropAdvisor2.SelectedValue == dropAdvisor2ImpossibleValue)
                project.Advisor2 = null;
            else
                project.Advisor2 = db.UserDepartmentMap.Single(user => user.Id == int.Parse(dropAdvisor2.SelectedValue));

            //Department
            int oldDepartmentId = project?.Department?.Id ?? -1;
            int newDepartmentId = int.Parse(dropDepartment.SelectedValue);
            project.Department = db.Departments.Single(d => d.Id == newDepartmentId);

            // If project changed departments & already has a ProjectNr, generate a new one
            if (newDepartmentId != oldDepartmentId && project.ProjectNr > 0)
            {
                project.ProjectNr = 0; // 'Remove' project number to allow finding a new one.
                project.AssignUniqueProjectNr(db);
            }

            // Languages
            if (dropLanguage.SelectedIndex == 0)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = true;
            }
            else if (dropLanguage.SelectedIndex == 1)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = false;
            }
            else if (dropLanguage.SelectedIndex == 2)
            {
                project.LanguageGerman = false;
                project.LanguageEnglish = true;
            }
            else throw new Exception($"Unexpected language selection {dropLanguage.SelectedIndex}");

            //Priority
            project.POneType = db.ProjectTypes.Single(t => t.Id == int.Parse(dropPOneType.SelectedValue));
            project.POneTeamSize = db.ProjectTeamSizes.Single(s => s.Id == int.Parse(dropPOneTeamSize.SelectedValue));

            if (dropPTwoType.SelectedValue == dropPTwoTypeImpossibleValue || dropPTwoTeamSize.SelectedValue == dropPTwoTeamSizeImpossibleValue)
            {
                project.PTwoType = null;
                project.PTwoTeamSize = null;
            }
            else
            {
                project.PTwoType = db.ProjectTypes.Single(t => t.Id == int.Parse(dropPTwoType.SelectedValue));
                project.PTwoTeamSize = db.ProjectTeamSizes.Single(s => s.Id == int.Parse(dropPTwoTeamSize.SelectedValue));
            }

            if (project.P1TeamSizeId == project.P2TeamSizeId && project.P1TypeId == project.P2TypeId)
            {
                project.PTwoType = null;
                project.PTwoTeamSize = null;
            }

            //Client
            if (radioClientType.SelectedIndex != (int)ClientType.Internal)
            {
                if (radioClientType.SelectedIndex == (int)ClientType.Company)
                {
                    project.ClientType = (int)ClientType.Company;
                    project.ClientCompany = txtClientCompany.Text.FixupParagraph();
                }
                else
                {
                    project.ClientType = (int)ClientType.PrivatePerson;
                    project.ClientCompany = "";
                }
                project.ClientAddressTitle = drpClientTitle.SelectedItem.Text;
                project.ClientPerson = txtClientName.Text.FixupParagraph();
                project.ClientAddressDepartment = txtClientDepartment.Text.FixupParagraph();
                project.ClientAddressStreet = txtClientStreet.Text.FixupParagraph();
                project.ClientAddressPostcode = txtClientPLZ.Text.FixupParagraph();
                project.ClientAddressCity = txtClientCity.Text.FixupParagraph();
                project.ClientReferenceNumber = txtClientReference.Text.FixupParagraph();
                project.ClientMail = txtClientEmail.Text.Trim().ToLowerInvariant();
                project.ClientPhoneNumber = txtClientPhoneNumber.Text.Trim();
            }
            else
            {
                project.ClientType = (int)ClientType.Internal;
                project.ClientAddressTitle = "Herr";

                project.ClientCompany =
                    project.ClientPerson =
                        project.ClientAddressDepartment =
                            project.ClientAddressStreet =
                                project.ClientAddressPostcode =
                                    project.ClientAddressCity =
                                        project.ClientReferenceNumber =
                                            project.ClientMail = 
                                                project.ClientPhoneNumber = "";
            }

            //NDA
            project.UnderNDA = chkNDA.Checked;

            //Reservation
            project.Reservation1Name = Reservation1Name.Text.FixupParagraph();
            project.Reservation1Mail = Reservation1Mail.Text.Trim().ToLowerInvariant();
            if (project.POneTeamSize.Size2)
            {
                project.Reservation2Name = Reservation2Name.Text.FixupParagraph();
                project.Reservation2Mail = Reservation2Mail.Text.Trim().ToLowerInvariant();
            }
            else
            {
                project.Reservation2Name = "";
                project.Reservation2Mail = "";
            }

            // Project categories
            project.TypeDesignUX = projectTopics[0];
            project.TypeHW = projectTopics[1];
            project.TypeCGIP = projectTopics[2];
            project.TypeMlAlg = projectTopics[3];
            project.TypeAppWeb = projectTopics[4];
            project.TypeDBBigData = projectTopics[5];
            project.TypeSysSec = projectTopics[6];
            project.TypeSE = projectTopics[7];

            //Picture description
            project.ImgDescription = imgdescription.Text.FixupParagraph();

            // Long texts (description etc.)
            project.InitialPosition = InitialPositionContent.Text.FixupParagraph();
            project.Objective = ObjectivContent.Text.FixupParagraph();
            project.ProblemStatement = ProblemStatementContent.Text.FixupParagraph();
            project.References = ReferencesContent.Text.FixupParagraph();
            project.Remarks = RemarksContent.Text.FixupParagraph();
            project.Notes = NotesContent.Text.FixupParagraph();
        }

        private void UpdateNonDBProjectFromFormData(NonDBProject project)
        {
            //Name
            project.Name = ProjectName.Text.FixupParagraph();

            //Semester
            if (DivSemester.Visible)
            {
                if (dropSemester.SelectedValue == dropSemesterImpossibleValue)
                {
                    project.SemesterId = null;
                }
                else
                {
                    project.SemesterId = db.Semester.Single(s => s.Id == int.Parse(dropSemester.SelectedValue)).Id;
                }
            }

            //Previous Project
            if (dropPreviousProject.SelectedValue == dropPreviousProjectImpossibleValue)
            {
                project.PreviousProjectId = null;
            }
            else
            {
                project.PreviousProjectId = db.Projects.Single(p => p.Id == int.Parse(dropPreviousProject.SelectedValue)).Id;
            }

            //Advisor
            if (dropAdvisor1.SelectedValue == dropAdvisor1ImpossibleValue)
                project.Advisor1Id = null;
            else
                project.Advisor1Id = db.UserDepartmentMap.Single(user => user.Id == int.Parse(dropAdvisor1.SelectedValue)).Id;

            if (dropAdvisor2.SelectedValue == dropAdvisor2ImpossibleValue)
                project.Advisor2Id = null;
            else
                project.Advisor2Id = db.UserDepartmentMap.Single(user => user.Id == int.Parse(dropAdvisor2.SelectedValue)).Id;

            //Department
            int newDepartmentId = int.Parse(dropDepartment.SelectedValue);
            project.DepartmentId = db.Departments.Single(d => d.Id == newDepartmentId).Id;

            // Languages
            if (dropLanguage.SelectedIndex == 0)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = true;
            }
            else if (dropLanguage.SelectedIndex == 1)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = false;
            }
            else if (dropLanguage.SelectedIndex == 2)
            {
                project.LanguageGerman = false;
                project.LanguageEnglish = true;
            }
            else throw new Exception($"Unexpected language selection {dropLanguage.SelectedIndex}");

            //Priority
            project.P1TypeId = db.ProjectTypes.Single(t => t.Id == int.Parse(dropPOneType.SelectedValue)).Id;
            project.P1TeamSizeId = db.ProjectTeamSizes.Single(s => s.Id == int.Parse(dropPOneTeamSize.SelectedValue)).Id;

            if (dropPTwoType.SelectedValue == dropPTwoTypeImpossibleValue || dropPTwoTeamSize.SelectedValue == dropPTwoTeamSizeImpossibleValue)
            {
                project.P2TypeId = null;
                project.P2TeamSizeId = null;
            }
            else
            {
                project.P2TypeId = db.ProjectTypes.Single(t => t.Id == int.Parse(dropPTwoType.SelectedValue)).Id;
                project.P2TeamSizeId = db.ProjectTeamSizes.Single(s => s.Id == int.Parse(dropPTwoTeamSize.SelectedValue)).Id;
            }

            if (project.P1TeamSizeId == project.P2TeamSizeId && project.P1TypeId == project.P2TypeId)
            {
                project.P2TypeId = null;
                project.P2TeamSizeId = null;
            }

            //Client
            if (radioClientType.SelectedIndex != (int)ClientType.Internal)
            {
                if (radioClientType.SelectedIndex == (int)ClientType.Company)
                {
                    project.ClientType = (int)ClientType.Company;
                    project.ClientCompany = txtClientCompany.Text.FixupParagraph();
                }
                else
                {
                    project.ClientType = (int)ClientType.PrivatePerson;
                    project.ClientCompany = "";
                }
                project.ClientAddressTitle = drpClientTitle.SelectedItem.Text;
                project.ClientPerson = txtClientName.Text.FixupParagraph();
                project.ClientAddressDepartment = txtClientDepartment.Text.FixupParagraph();
                project.ClientAddressStreet = txtClientStreet.Text.FixupParagraph();
                project.ClientAddressPostcode = txtClientPLZ.Text.FixupParagraph();
                project.ClientAddressCity = txtClientCity.Text.FixupParagraph();
                project.ClientReferenceNumber = txtClientReference.Text.FixupParagraph();
                project.ClientMail = txtClientEmail.Text.Trim().ToLowerInvariant();
                project.ClientPhoneNumber = txtClientPhoneNumber.Text.Trim();
            }
            else
            {
                project.ClientType = (int)ClientType.Internal;
                project.ClientAddressTitle = "Herr";

                project.ClientCompany =
                    project.ClientPerson =
                        project.ClientAddressDepartment =
                            project.ClientAddressStreet =
                                project.ClientAddressPostcode =
                                    project.ClientAddressCity =
                                        project.ClientReferenceNumber =
                                            project.ClientMail = 
                                                project.ClientPhoneNumber = "";
            }

            //NDA
            project.UnderNDA = chkNDA.Checked;

            //Reservation
            project.Reservation1Name = Reservation1Name.Text.FixupParagraph();
            project.Reservation1Mail = Reservation1Mail.Text.Trim().ToLowerInvariant();
            if (db.ProjectTeamSizes.Single(s => s.Id == project.P1TeamSizeId).Size2)
            {
                project.Reservation2Name = Reservation2Name.Text.FixupParagraph();
                project.Reservation2Mail = Reservation2Mail.Text.Trim().ToLowerInvariant();
            }
            else
            {
                project.Reservation2Name = "";
                project.Reservation2Mail = "";
            }

            // Project categories
            project.TypeDesignUX = projectTopics[0];
            project.TypeHW = projectTopics[1];
            project.TypeCGIP = projectTopics[2];
            project.TypeMlAlg = projectTopics[3];
            project.TypeAppWeb = projectTopics[4];
            project.TypeDBBigData = projectTopics[5];
            project.TypeSysSec = projectTopics[6];
            project.TypeSE = projectTopics[7];

            // Picture changed
            if (AddPicture.HasFile) imageChanged = true;
            // Picture deleted
            if (db.Projects.Single(p => p.Id == pageProject.Id).Picture != null && !DeleteImageButton.Visible) imageDeleted = true;
            // Picture description
            project.ImgDescription = imgdescription.Text.FixupParagraph();

            // Long texts (description etc.)
            project.InitialPosition = InitialPositionContent.Text.FixupParagraph();
            project.Objective = ObjectivContent.Text.FixupParagraph();
            project.ProblemStatement = ProblemStatementContent.Text.FixupParagraph();
            project.References = ReferencesContent.Text.FixupParagraph();
            project.Remarks = RemarksContent.Text.FixupParagraph();
            project.Notes = NotesContent.Text.FixupParagraph();
        }

        private void SavePicture(Project project)
        {
            //Picture
            if (imageDeleted)
            {
                project.Picture = null;
            }

            if (AddPicture.HasFile)
            {
                var fileExtension = AddPicture.FileName.Split('.').Last().ToLower();
                if (fileExtension != "jpg" && fileExtension != "jpeg" && fileExtension != "png")
                {
                    throw new Exception("Only jpg, jpeg or png are supported");
                }
                imageChanged = true;
                using (var input = AddPicture.PostedFile.InputStream)
                {
                    var data = new byte[AddPicture.PostedFile.ContentLength];
                    var offset = 0;
                    for (; ; )
                    {
                        var read = input.Read(data, offset, data.Length - offset);
                        if (read == 0)
                            break;

                        offset += read;
                    }
                    project.Picture = new Binary(data);
                }
            }

            db.SubmitChanges();
        }


        /// <summary>
        ///     Saves changes to the project in the database.
        /// </summary>
        private void SaveProject(bool copy)
        {
            if (pageProject == null) // New project
            {
                pageProject = ProjectExtensions.CreateNewProject(db);
                pageProject.IsMainVersion = true;
                SaveNewProject();
            }
            else if (!copy)
            {
                SaveChangedProject();
            }
            else if (NEW_HasProjectChanged())
            {
                SaveChangedProjectAsNewVersion();
            }
        }

        private void SaveNewProject()
        {
            UpdateProjectFromFormData(pageProject);
            SavePicture(pageProject);
            db.SubmitChanges(); // the next few lines depend on this submit
            pageProject.BaseVersionId = pageProject.Id;
            pageProject.OverOnePage = new PdfCreator().CalcNumberOfPages(pageProject) > 1;
            db.SubmitChanges();
            pageProject = pageProject.CopyAndUseCopyAsMainVersion(db);
        }

        private void SaveChangedProject()
        {
            if (!pageProject.UserCanEdit())
                throw new UnauthorizedAccessException();
            UpdateProjectFromFormData(pageProject);
            SavePicture(pageProject);
            pageProject.OverOnePage = new PdfCreator().CalcNumberOfPages(pageProject) > 1;
            pageProject.SaveProjectAsMainVersion(db);
        }

        private void SaveChangedProjectAsNewVersion()
        {
            if (!pageProject.UserCanEdit())
                throw new UnauthorizedAccessException();

            UpdateProjectFromFormData(pageProject);
            SavePicture(pageProject);
            pageProject.OverOnePage = new PdfCreator().CalcNumberOfPages(pageProject) > 1;
            pageProject = pageProject.CopyAndUseCopyAsMainVersion(db);
        }

        private bool HasProjectChanged()
        {
            var comparisonProject = pageProject.CopyProject();
            UpdateProjectFromFormData(comparisonProject);
            comparisonProject.Id = -1;

            ///////////////////////////////////////////////
            db.Projects.InsertOnSubmit(comparisonProject); //Hack so that the project is not submitted
            db.Projects.DeleteOnSubmit(comparisonProject);
            ///////////////////////////////////////////////

            bool isChanged = comparisonProject.IsModified(pageProject) || imageChanged || imageDeleted;
            db.SubmitChanges();
            return isChanged;
        }

        private bool NEW_HasProjectChanged()
        {
            var comparisonProject = new NonDBProject();
            UpdateNonDBProjectFromFormData(comparisonProject);
            return pageProject.IsModified(comparisonProject, true, false) || imageChanged || imageDeleted;
        }

        #endregion

        #region history and comparison

        private string CreateSimpleDiffString(string oldText, string newText)
        {
            if (oldText != newText)
                return "<del class='diffmod'>" + oldText + "</del> " + "<ins class='diffmod'>" + newText + "</ins>";
            else
                return newText;
        }

        private string CreateDiffString(string oldText, string newText)
        {
            string diffString;
            try
            {
                HtmlDiff.HtmlDiff h = new HtmlDiff.HtmlDiff(oldText, newText);
                diffString = h.Build();
            }
            catch
            {
                return " ";
            }
            return diffString;
        }

        private void GetControlList<T>(ControlCollection controlCollection, List<Control> resultCollection) where T : Control
        {
            foreach (Control control in controlCollection)
            {
                if (control is T)
                    resultCollection.Add(control);

                if (control.HasControls())
                    GetControlList<T>(control.Controls, resultCollection);
            }
        }
        private void ShowAllLabelsForComparison()
        {
            List<Control> labelList = new List<Control>();
            GetControlList<Label>(Controls, labelList);
            foreach (Label l in labelList)
                l.Visible = true;

            //additional controls 
            ProjectPicturePrevious.Visible = true;
        }
        private void HideTextboxesAndDropdownsForComparison()
        {
            List<Control> controlList = new List<Control>();
            GetControlList<DropDownList>(Controls, controlList);
            GetControlList<TextBox>(Controls, controlList);
            foreach (var control in controlList)
                control.Visible = false;
            ProjectPicture.Visible = false;
        }
        private void PopulateHistoryGUI(int BaseVersionId = 0)
        {
            var pid = 0;

            if (BaseVersionId == 0)
                pid = int.Parse(Request.QueryString["showChanges"]);
            else
                pid = BaseVersionId;

            var currentProject = db.Projects.Single(p => p.Id == pid);

            ShowAllLabelsForComparison();
            HideTextboxesAndDropdownsForComparison();

            CreatorID.Text = pageProject.Creator + "/" + pageProject.CreateDate.ToString("yyyy-MM-dd");
            AddPictureLabel.Text = "Bild ändern:";
            ProjectNameLabel.Text = CreateSimpleDiffString(pageProject.Name, currentProject.Name);
            dropAdvisor1Label.Text = CreateSimpleDiffString(pageProject.Advisor1?.Name ?? "", currentProject.Advisor1?.Name ?? "");
            dropAdvisor2Label.Text = CreateSimpleDiffString(pageProject.Advisor2?.Name ?? "", currentProject.Advisor2?.Name ?? "");

            if (currentProject.TypeDesignUX)
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectTopics[0] = true;

                if (!pageProject.TypeDesignUX)
                {
                    DesignUX.BorderStyle = BorderStyle.Solid;
                    DesignUX.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeDesignUX)
                {
                    DesignUX.BorderStyle = BorderStyle.Solid;
                    DesignUX.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeHW)
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectTopics[1] = true;
                if (!pageProject.TypeHW)
                {
                    HW.BorderStyle = BorderStyle.Solid;
                    HW.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeHW)
                {
                    HW.BorderStyle = BorderStyle.Solid;
                    HW.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeCGIP)
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectTopics[2] = true;
                if (!pageProject.TypeCGIP)
                {
                    CGIP.BorderStyle = BorderStyle.Solid;
                    CGIP.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeCGIP)
                {
                    CGIP.BorderStyle = BorderStyle.Solid;
                    CGIP.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeMlAlg)
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlg.png";
                projectTopics[3] = true;
                if (!pageProject.TypeMlAlg)
                {
                    MlAlg.BorderStyle = BorderStyle.Solid;
                    MlAlg.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeMlAlg)
                {
                    MlAlg.BorderStyle = BorderStyle.Solid;
                    MlAlg.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeAppWeb)
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectTopics[4] = true;
                if (!pageProject.TypeAppWeb)
                {
                    AppWeb.BorderStyle = BorderStyle.Solid;
                    AppWeb.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeAppWeb)
                {
                    AppWeb.BorderStyle = BorderStyle.Solid;
                    AppWeb.BorderColor = Color.Red;
                }
            }
            if (currentProject.TypeDBBigData)
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectTopics[5] = true;
                if (!pageProject.TypeDBBigData)
                {
                    DBBigData.BorderStyle = BorderStyle.Solid;
                    DBBigData.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeDBBigData)
                {
                    DBBigData.BorderStyle = BorderStyle.Solid;
                    DBBigData.BorderColor = Color.Red;
                }
            }
            if (currentProject.TypeSysSec)
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectTopics[6] = true;
                if (!pageProject.TypeSysSec)
                {
                    SysSec.BorderStyle = BorderStyle.Solid;
                    SysSec.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeSysSec)
                {
                    SysSec.BorderStyle = BorderStyle.Solid;
                    SysSec.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeSE)
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectTopics[7] = true;
                if (!pageProject.TypeSE)
                {
                    SE.BorderStyle = BorderStyle.Solid;
                    SE.BorderColor = Color.Green;
                }
            }
            else
            {
                if (pageProject.TypeSE)
                {
                    SE.BorderStyle = BorderStyle.Solid;
                    SE.BorderColor = Color.Red;
                }
            }

            dropPOneType.SelectedValue = pageProject.POneType.Id.ToString();
            dropPTwoType.SelectedValue = pageProject.PTwoType?.Id.ToString();
            dropPOneTeamSize.SelectedValue = pageProject.POneTeamSize.Id.ToString();
            dropPTwoTeamSize.SelectedValue = pageProject.PTwoTeamSize?.Id.ToString();

            if (pageProject.LanguageEnglish && !pageProject.LanguageGerman)
                dropLanguage.SelectedIndex = 2;
            else if (!pageProject.LanguageEnglish && pageProject.LanguageGerman)
                dropLanguage.SelectedIndex = 1;
            else
                dropLanguage.SelectedIndex = 0;

            //DurationOneSemester.Checked = project.DurationOneSemester;
            InitialPositionContentLabel.Text = CreateDiffString(pageProject.InitialPosition, currentProject.InitialPosition);

            ObjectivContentLabel.Text = CreateDiffString(pageProject.Objective, currentProject.Objective);

            ProblemStatementContentLabel.Text = CreateDiffString(pageProject.ProblemStatement, currentProject.ProblemStatement);

            ReferencesContentLabel.Text = CreateDiffString(pageProject.References, currentProject.References);

            RemarksContentLabel.Text = CreateDiffString(pageProject.Remarks, currentProject.Remarks);


            Reservation1NameLabel.Text = CreateSimpleDiffString(pageProject.Reservation1Name, currentProject.Reservation1Name);

            Reservation1MailLabel.Text = CreateSimpleDiffString(pageProject.Reservation1Mail, currentProject.Reservation1Mail);

            Reservation2NameLabel.Text = CreateSimpleDiffString(pageProject.Reservation2Name, currentProject.Reservation2Name);

            Reservation2MailLabel.Text = CreateSimpleDiffString(pageProject.Reservation2Mail, currentProject.Reservation2Mail);

            DepartmentLabel.Text = CreateSimpleDiffString(pageProject.Department.DepartmentName, currentProject.Department.DepartmentName);

            // Button visibility
            saveProject.Visible = false;
            submitProject.Visible = false;
            publishProject.Visible = false;
            refuseProject.Visible = false;
            rollbackProject.Visible = false;
            saveCloseProject.Visible = false;
            duplicateProject.Visible = false;

            POneTypeLabel.Text = CreateSimpleDiffString(pageProject.POneType.Description, currentProject.POneType.Description);
            dropPOneTeamSize.SelectedValue = CreateSimpleDiffString(pageProject.POneTeamSize.Description, currentProject.POneTeamSize.Description);

            Reservation1MailLabel.Text = CreateSimpleDiffString(pageProject?.Reservation1Mail, currentProject?.Reservation1Mail);
            Reservation1NameLabel.Text = CreateSimpleDiffString(pageProject.Reservation1Name, currentProject?.Reservation1Name);
            Reservation2MailLabel.Text = CreateSimpleDiffString(pageProject.Reservation2Mail, currentProject?.Reservation2Mail);
            Reservation2NameLabel.Text = CreateSimpleDiffString(pageProject.Reservation2Name, currentProject?.Reservation2Name);

            PopulateClientWithDiffStrings(pageProject, currentProject);

            PrepareClientForm();

            FillDropPreviousProject(false);

            if (pageProject.PreviousProjectID == null)
            {
                dropPreviousProject.SelectedValue = pageProject.PreviousProjectID.ToString();
                PrepareForm(false);
            }
            else
            {
                dropPreviousProject.SelectedValue = pageProject.PreviousProjectID?.ToString() ?? dropPreviousProjectImpossibleValue;
                PrepareForm(true);
            }
        }





        private void PopulateClientWithDiffStrings(Project project, Project currentProject)
        {
            txtClientCompanyLabel.Text = CreateDiffString(project?.ClientCompany, currentProject?.ClientCompany);
            drpClientTitleLabel.Text = CreateDiffString(project?.ClientAddressTitle, currentProject?.ClientAddressTitle);
            txtClientNameLabel.Text = CreateDiffString(project?.ClientPerson, currentProject?.ClientPerson);
            txtClientDepartmentLabel.Text = CreateDiffString(project?.ClientAddressDepartment, currentProject?.ClientAddressDepartment);
            txtClientStreetLabel.Text = CreateDiffString(project?.ClientAddressStreet, currentProject?.ClientAddressStreet);
            txtClientPLZLabel.Text = CreateDiffString(project?.ClientAddressPostcode, currentProject?.ClientAddressPostcode);
            txtClientCityLabel.Text = CreateDiffString(project?.ClientAddressCity, currentProject?.ClientAddressCity);
            txtClientReferenceLabel.Text = CreateDiffString(project?.ClientReferenceNumber, currentProject?.ClientReferenceNumber);
            txtClientEmailLabel.Text = CreateDiffString(project?.ClientMail, currentProject?.ClientMail);
            txtClientPhoneNumberLabel.Text = CreateDiffString(project?.ClientPhoneNumber, currentProject?.ClientPhoneNumber);
        }

        private void CollapseHistory(bool collapse)
        {
            Session["AddInfoCollapsed"] = collapse;
            DivHistoryCollapsable.Visible = !collapse;
            btnHistoryCollapse.Text = collapse ? "◄" : "▼";
        }


        #endregion

        #region Click handlers: Project topics

        protected void DesignUX_Click(object sender, ImageClickEventArgs e)
        {
            if (DesignUX.ImageUrl == "pictures/projectTypDesignUXUnchecked.png")
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectTopics[0] = true;
            }
            else
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUXUnchecked.png";
                projectTopics[0] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void HW_Click(object sender, ImageClickEventArgs e)
        {
            if (HW.ImageUrl == "pictures/projectTypHWUnchecked.png")
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectTopics[1] = true;
            }
            else
            {
                HW.ImageUrl = "pictures/projectTypHWUnchecked.png";
                projectTopics[1] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void CGIP_Click(object sender, ImageClickEventArgs e)
        {
            if (CGIP.ImageUrl == "pictures/projectTypCGIPUnchecked.png")
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectTopics[2] = true;
            }
            else
            {
                CGIP.ImageUrl = "pictures/projectTypCGIPUnchecked.png";
                projectTopics[2] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void MlAlg_Click(object sender, ImageClickEventArgs e)
        {
            if (MlAlg.ImageUrl == "pictures/projectTypMlAlgUnchecked.png")
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlg.png";
                projectTopics[3] = true;
            }
            else
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlgUnchecked.png";
                projectTopics[3] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void AppWeb_Click(object sender, ImageClickEventArgs e)
        {
            if (AppWeb.ImageUrl == "pictures/projectTypAppWebUnchecked.png")
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectTopics[4] = true;
            }
            else
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWebUnchecked.png";
                projectTopics[4] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void DBBigData_Click(object sender, ImageClickEventArgs e)
        {
            if (DBBigData.ImageUrl == "pictures/projectTypDBBigDataUnchecked.png")
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectTopics[5] = true;
            }
            else
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigDataUnchecked.png";
                projectTopics[5] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void SysSec_Click(object sender, ImageClickEventArgs e)
        {
            if (SysSec.ImageUrl == "pictures/projectTypSysSecUnchecked.png")
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectTopics[6] = true;
            }
            else
            {
                SysSec.ImageUrl = "pictures/projectTypSysSecUnchecked.png";
                projectTopics[6] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        protected void SE_Click(object sender, ImageClickEventArgs e)
        {
            if (SE.ImageUrl == "pictures/projectTypSEUnchecked.png")
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectTopics[7] = true;
            }
            else
            {
                SE.ImageUrl = "pictures/projectTypSEUnchecked.png";
                projectTopics[7] = false;
            }
            ViewState["Topics"] = projectTopics;
        }

        public bool CheckVisibility(int id)
        {
            var paramId = Request.QueryString.Get("id");
            return paramId == id.ToString();
        }

        #endregion

        #region Click handlers: Buttons (user)

        protected void ProjectRowClick(object sender, ListViewCommandEventArgs e)
        {


            var pid = Convert.ToInt32(e.CommandArgument);
            switch (e.CommandName)
            {
                case "revertProject":
                    var currentProject = db.Projects.Single(p => p.BaseVersionId == pageProject.BaseVersionId && p.IsMainVersion && p.State != ProjectState.Deleted);
                    currentProject.IsMainVersion = false;
                    db.SubmitChanges();
                    var revertedProject = db.Projects.Single(p => p.Id == pid);
                    revertedProject.IsMainVersion = true;
                    db.SubmitChanges();
                    Response.Redirect("~/ProjectEditPage.aspx?id=" + pid);
                    break;
                case "showChanges":
                    var mainProject = db.Projects.Single(p => p.BaseVersionId == pageProject.BaseVersionId && p.IsMainVersion && p.State != ProjectState.Deleted).Id;
                    Response.Redirect("~/ProjectEditPage.aspx?id=" + pid + "&showChanges=" + mainProject);
                    break;
                default:
                    throw new Exception("Unknown command " + e.CommandName);
            }
        }

        /// <summary>
        ///     Saves the current state of the form and continue editing.
        /// </summary>
        protected void SaveProjectButton(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveProject(pageProject != null ? pageProject.State <= ProjectState.Published : true);
                Response.Redirect("ProjectEditPage?id=" + pageProject.Id);
            }
            saveProject.Enabled = true;
        }

        /// <summary>
        ///     Save the current state of the form and return to project list.
        /// </summary>
        protected void SaveCloseProjectButton(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveProject(pageProject != null ? pageProject.State <= ProjectState.Published : true);
                Response.Redirect("projectlist");
            }
            saveCloseProject.Enabled = true;
        }


        protected void CancelNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("projectlist");
        }

        protected void SubmitProject_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveProject(false);
                var validationMessage = GenerateValidationMessageForSubmitAndPublish();

                if (string.IsNullOrWhiteSpace(validationMessage))
                {
                    pageProject.Submit(db);
                    db.SubmitChanges();

                    var mail = new MailMessage();
                    var departmentMngr = db.UserDepartmentMap.FirstOrDefault(u => u.IsDepartmentManager && u.Department == pageProject.Department);
                    if (departmentMngr is null)
                    {
                        mail.To.Add(Global.WebAdmin);
                        mail.From = new MailAddress("noreply@fhnw.ch");
                        mail.Subject = $"ERROR";
                        mail.Body = $"No DepartmentManager found for Department ID: {pageProject.DepartmentId}";
                        
                    }
                    else
                    {
                        mail.To.Add(departmentMngr.Mail);
                        mail.From = new MailAddress("noreply@fhnw.ch");
                        mail.Subject = $"Neues Projekt eingereicht";
                        mail.IsBodyHtml = true;

                        var mailMessage = new StringBuilder();
                        mailMessage.Append("<div style=\"font-family: Arial\">");
                        mailMessage.Append($"<p style=\"font-size: 110%\">Hallo {HttpUtility.HtmlEncode(departmentMngr.Name.Split(' ')[0])}</p>"
                            + "<p>Ein neues Projekt wurde eingereicht:</p>"
                            + $"<p><a href=\"https://www.cs.technik.fhnw.ch/prostud/ProjectEditPage?id={pageProject.Id}\">{HttpUtility.HtmlEncode(pageProject.Name)}</a></p>"
                            + "<br/>"
                            + "<p>Freundliche Grüsse</p>"
                            + "<p>ProStud-Team</p>"
                            + $"<p>Feedback an {HttpUtility.HtmlEncode(Global.WebAdmin)}</p>"
                            + "</div>"
                            );

                        mail.Body = mailMessage.ToString();
                    }

                    TaskHandler.SendMail(mail);

                    Response.Redirect("projectlist");
                    return;
                }
                // Generate JavaScript alert with error message
                var sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("alert('");
                sb.Append(validationMessage);
                sb.Append("');");
                sb.Append("</script>");
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alert", sb.ToString(), false);
            }
        }

        private string GenerateValidationMessageForSubmitAndPublish()
        {
            //Advisor1
            if (dropAdvisor1.SelectedValue == dropAdvisor1ImpossibleValue)
                return "Bitte wählen Sie einen Hauptbetreuer aus.";

            //Semester
            if (DivSemester.Visible && dropSemester.SelectedValue != dropSemesterImpossibleValue && dropSemester.SelectedValue != Semester.CurrentSemester(db).Id.ToString() && dropSemester.SelectedValue != Semester.NextSemester(db).Id.ToString())
                return "Nur Projekte für das aktuelle oder das nächste Semester können veröffentlicht werden.";

            //1-2 selected ProjectTopics
            var numAssignedTypes = 0;
            numAssignedTypes = projectTopics.Count(a => a);

            if (numAssignedTypes != 1 && numAssignedTypes != 2)
                return "Bitte wählen Sie genau 1-2 passende Themengebiete aus.";

            //Client Information
            if (radioClientType.SelectedIndex != (int)ClientType.Internal)
            {
                if (string.IsNullOrWhiteSpace(txtClientName.Text))
                    return "Bitte geben Sie den Namen des Kundenkontakts an (Vorname Nachname).";

                if (!txtClientName.Text.IsValidName())
                    return "Bitte geben Sie den Namen des Kundenkontakts im Format (Vorname Nachname) an.";

                if (string.IsNullOrWhiteSpace(txtClientEmail.Text))
                    return "Bitte geben Sie die E-Mail-Adresse des Kundenkontakts an.";

                if (!txtClientEmail.Text.IsValidEmail())
                    return "Bitte geben Sie die E-Mail-Adresse des Kundenkontakts im Format (xxx@yyy.zzz) an.";

                if (radioClientType.SelectedIndex == (int)ClientType.Company)
                {
                    if (string.IsNullOrWhiteSpace(txtClientCompany.Text))
                        return "Bitte geben Sie den Namen des Unternehmens an.";
                }
            }

            //Reservation
            var res1Name = Reservation1Name.Text;
            var res1Mail = Reservation1Mail.Text;
            var res2Name = Reservation2Name.Text;
            var res2Mail = Reservation2Mail.Text;

            if (res1Mail.Trim().Length != 0 && res1Name.Trim().Length == 0)
                return "Bitte geben Sie den Namen der ersten Person an, für die das Projekt reserviert ist (Vorname Nachname).";

            if (res1Name.Trim().Length != 0 && res1Mail.Trim().Length == 0)
                return "Bitte geben Sie die E-Mail-Adresse der Person an, für die das Projekt reserviert ist.";

            if (res1Mail.Trim().Length != 0 && res1Name.Trim().Length != 0)
            {
                Regex regex = new Regex(@".*\..*@students\.fhnw\.ch");
                System.Text.RegularExpressions.Match match = regex.Match(res1Mail);
                if (!match.Success)
                    return "Bitte geben Sie eine gültige E-Mail-Adresse der Person an, für die das Projekt reserviert ist. (vorname.nachname@students.fhnw.ch)";
            }


            if (res2Mail.Trim().Length != 0 && res2Name.Trim().Length == 0)
                return "Bitte geben Sie den Namen der zweiten Person an, für die das Projekt reserviert ist (Vorname Nachname).";

            if (res2Name.Trim().Length != 0 && res2Mail.Trim().Length == 0)
                return "Bitte geben Sie die E-Mail-Adresse der zweiten Person an, für die das Projekt reserviert ist.";

            if (res2Mail.Trim().Length != 0 && res2Name.Trim().Length != 0)
            {
                Regex regex = new Regex(@".*\..*@students\.fhnw\.ch");
                System.Text.RegularExpressions.Match match = regex.Match(res2Mail);
                if (!match.Success)
                    return "Bitte geben Sie eine gültige E-Mail-Adresse der zweiten Person an, für die das Projekt reserviert ist.(vorname.nachname@students.fhnw.ch)";
            }

            if (pageProject.OverOnePage)
                return "Der Projektbeschrieb passt nicht auf eine A4-Seite.Bitte kürzen Sie die Beschreibung.";

            return null;
        }

        #endregion

        #region Click handlers: Buttons (admin only)

        protected void PublishProject_Click(object sender, EventArgs e)
        {
            SaveProject(false);
            var validationMessage = GenerateValidationMessageForSubmitAndPublish();

            if (string.IsNullOrWhiteSpace(validationMessage))
            {
                pageProject.Publish(db);
                db.SubmitChanges();

                var mailMessage = new MailMessage();
                mailMessage.To.Add(pageProject.Creator);
                if (pageProject.Advisor1.Mail != pageProject.Creator)
                    mailMessage.To.Add(pageProject.Advisor1.Mail);
                if (pageProject.Advisor2?.Mail != null && pageProject.Advisor2.Mail != pageProject.Creator)
                    mailMessage.To.Add(pageProject.Advisor2.Mail);
                mailMessage.From = new MailAddress(ShibUser.GetEmail());
                mailMessage.Subject = $"Projekt '{pageProject.Name}' veröffentlicht";
                mailMessage.Body = $"Dein Projekt '{pageProject.Name}' wurde von {ShibUser.GetFirstName()} veröffentlicht.\n"
                    + "\n"
                    + "----------------------\n"
                    + "Automatische Nachricht von ProStud\n"
                    + "https://www.cs.technik.fhnw.ch/prostud/";

                TaskHandler.SendMail(mailMessage);

                Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
                return;
            }
            // Generate JavaScript alert with error message
            var sb = new StringBuilder();
            sb.Append("<script type = 'text/javascript'>");
            sb.Append("alert('");
            sb.Append(validationMessage);
            sb.Append("');");
            sb.Append("</script>");
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alert", sb.ToString(), false);
        }

        protected void RefuseProject_Click(object sender, EventArgs e)
        {
            refusedReason.Visible = true;
            refuseProject.Visible = false;
            publishProject.Visible = false;
            duplicateProject.Visible = false;
            saveProject.Visible = false;

            refusedReasonText.Text = $"Dein Projekt '{pageProject.Name}' wurde leider nicht gewählt. Bitte informiere allfällige externe Auftraggeber!\n"
                                     + "\n"
                                     + "Hier kannst Du das Projekt für das nächste Semester wieder einreichen:\n"
                                     + "https://www.cs.technik.fhnw.ch/prostud/\n"
                                     + "\n"
                                     + "\n"
                                     + "Freundliche Grüsse\n"
                                     + ShibUser.GetFirstName();

            /*
            refusedReasonText.Text = $"Dein Projekt '{project.Name}' wurde von {ShibUser.GetFirstName()} abgelehnt.\n"
                                     + "\n"
                                     + "Dies sind die Gründe dafür:\n"
                                     + "\n"
                                     + "\n"
                                     + "\n"
                                     + "Freundliche Grüsse\n"
                                     + ShibUser.GetFirstName();
                                     */
            refuseProjectUpdatePanel.Update();
            SetFocus(refusedReason);
        }

        protected void RefuseDefinitiveNewProject_Click(object sender, EventArgs e)
        {
            SaveProject(true);
            pageProject.Ablehnungsgrund = refusedReasonText.Text;
            pageProject.Reject(db);
            db.SubmitChanges();

            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(pageProject.Creator);
            if (pageProject.Advisor1.Mail != pageProject.Creator)
                mailMessage.To.Add(pageProject.Advisor1.Mail);
            if (pageProject.Advisor2?.Mail != null && pageProject.Creator != pageProject.Advisor2.Mail)
                mailMessage.To.Add(pageProject.Advisor2.Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.CC.Add(ShibUser.GetEmail());
            mailMessage.Subject = $"Projekt '{pageProject.Name}' abgelehnt";
            mailMessage.Body =
            refusedReasonText.Text + "\n\n----------------------\nAutomatische Nachricht von ProStud\nhttps://www.cs.technik.fhnw.ch/prostud/";

            TaskHandler.SendMail(mailMessage);

            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        protected void CancelRefusion_Click(object sender, EventArgs e)
        {
            saveProject.Visible = true;
            refusedReason.Visible = false;
            refuseProject.Visible = true;
            publishProject.Visible = true;
            duplicateProject.Visible = true;
        }

        protected void RollbackProject_Click(object sender, EventArgs e)
        {
            if (!pageProject.UserCanUnsubmit())
            {
                throw new InvalidOperationException("Unsubmit not permitted");
            }

            SaveProject(true);
            pageProject.Unsubmit(db);
            db.SubmitChanges();
            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        #endregion

        #region Other view event handlers

        protected void DeleteImage_Click(object sender, EventArgs e)
        {
            if (pageProject != null)
            {
                DisplayPicture(false, imageDeleted=true);
            }
            else
            {
                DisplayPicture(true, imageDeleted=true);
            }
        }

        protected void TeamSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleReservationTwoVisible();
        }

        protected void DropPreviousProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropPreviousProject.SelectedValue == dropPreviousProjectImpossibleValue)
            {
                DisplayClient(pageProject == null);
                DisplayPriority();
                DisplayReservations();
            }
            else
            {
                var previousProject = db.Projects.Single(p => p.Id == int.Parse(dropPreviousProject.SelectedValue));
                DisplayPriority();
                DisplayReservations();
                DisplayClient(pageProject == null);
            }
            ToggleReservationTwoVisible();
            updateReservation.Update();
            updateClient.Update();
        }

        protected void DropSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selValue = dropPreviousProject.SelectedValue;

            FillDropPreviousProject(false);

            if (selValue != dropPreviousProjectImpossibleValue)
            {
                dropPreviousProject.SelectedValue = dropPreviousProjectImpossibleValue;

                DisplayPriority();
                updatePriority.Update();
                DisplayReservations();
                updateReservation.Update();
                DisplayClient(pageProject==null);
                updateClient.Update();
            }
            
            updatePreviousProject.Update();
        }

        protected void RadioClientType_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        protected void BtnHistoryCollapse_OnClick(object sender, EventArgs e)
        {
            CollapseHistory(!(bool)Session["AddInfoCollapsed"]);
        }

        protected void DuplicateProject_Click(object sender, EventArgs e)
        {
            SaveProject(false);
            var duplicate = pageProject.DuplicateProject(db);

            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                Response.Redirect("~/ProjectEditPage.aspx?id=" + duplicate.Id);
            }
        }

        #endregion

    }
}
