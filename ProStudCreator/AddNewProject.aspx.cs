using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using HtmlDiff;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ProStudCreator
{
    public partial class AddNewProject : Page
    {
        public readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;
        private ProjectType projectPriority = new ProjectType();
        private bool[] projectType = new bool[8];
        private DateTime today = DateTime.Now;
        private bool imageChanged = false;
        private Binary picture;
        
        #region Timer tick

        protected void Pdfupdatetimer_Tick(object sender, EventArgs e) //function for better workflow with long texts
        {
            if (project != null)
            {
                var pdfc = new PdfCreator();

                UpdateProjectFromFormData(project);

                if (pdfc.CalcNumberOfPages(project) > 1)
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

        #region Application logic

        protected void Page_Load(object sender, EventArgs e)
        {
            AdminView.Visible = ShibUser.CanSeeCreationDetails();
            updateReservation.Visible = ShibUser.CanReserveProjects();

            btnHistoryCollapse.CausesValidation = false;
            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                project = db.Projects.Single(p => (int?)p.Id == id);

                // if loaded page is not main version -> redirect outdated error
                if (!project.IsMainVersion && Request.QueryString["showChanges"] == null)
                {
                    //var mainProject = db.Projects.Single(p => p.BaseVersionId == project.BaseVersionId && p.IsMainVersion);
                    //Response.Redirect("~/AddNewProject.aspx?id=" + mainProject.Id);
                    Response.Redirect("error/OutdatedData.aspx");
                }

                if (!project.UserCanEdit())
                {
                    Response.Redirect("error/AccessDenied.aspx");
                    Response.End();
                }
                var history = db.Projects.Where(p => p.BaseVersionId == project.BaseVersionId && !p.IsMainVersion);
                if (history.ToList().Count > 0)
                {
                    historyListView.DataSource = history;
                    historyListView.DataBind();
                }
                else
                {
                    DivHistoryCollapsable.InnerText = "Keine früheren Versionen vorhanden.";
                }

            }
            else
            {
                divHistory.Visible = false;
                if (Request.QueryString["showChanges"] == null)
                    duplicateProject.Visible = false;
            }

            // Project picture
            if (project != null && project.Picture != null)
            {
                ProjectPicture.Visible = true;
                ProjectPicture.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(project.Picture.ToArray());
                DeleteImageButton.Visible = true;
            }
            else
            {
                ImageLabel.Visible = false;
                ProjectPicture.Visible = false;

            }

            if (IsPostBack)
            {
                projectType = (bool[])ViewState["Types"];
            }
            else
            {
                if (Session["AddInfoCollapsed"] == null)
                    CollapseHistory(true);
                else
                    CollapseHistory((bool)Session["AddInfoCollapsed"]);

                // Fix for unsupported maxlength on multi-line in ASP.NET
                // Note: Recursive search of all controls is possible but not worthwhile (yet)
                //InitialPositionContent.Attributes.Add("maxlength", InitialPositionContent.MaxLength.ToString());
                //ObjectivContent.Attributes.Add("maxlength", ObjectivContent.MaxLength.ToString());
                //ProblemStatementContent.Attributes.Add("maxlength", ProblemStatementContent.MaxLength.ToString());
                //ReferencesContent.Attributes.Add("maxlength", ReferencesContent.MaxLength.ToString());
                //RemarksContent.Attributes.Add("maxlength", RemarksContent.MaxLength.ToString());

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

                POneType.DataSource = db.ProjectTypes;
                POneTeamSize.DataSource = db.ProjectTeamSizes;
                PTwoType.DataSource = Enumerable.Repeat(new ProjectType
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(db.ProjectTypes);
                PTwoTeamSize.DataSource = Enumerable.Repeat(new ProjectTeamSize
                {
                    Description = "-",
                    Id = -1
                }, 1).Concat(db.ProjectTeamSizes);

                Department.DataSource = db.Departments;
                var dep = ShibUser.GetDepartment(db);
                if (dep != null)
                    Department.SelectedValue = dep.Id.ToString();

                DataBind();

                POneTeamSize.SelectedIndex = 1;
                PTwoTeamSize.SelectedIndex = 0;

                ViewState["Types"] = projectType;
                AddPictureLabel.Text = "Bild hinzufügen:";

                if (id.HasValue)
                {
                    Page.Title = "Projekt bearbeiten";
                    SiteTitle.Text = "Projekt bearbeiten";
                    if (!project.IsMainVersion && Request.QueryString["showChanges"] == null)
                        PopulateHistoryGUI(project.Id);
                    else
                        UpdateUIFromProjectObject();
                    PrepareClientForm(project);
                }
                else
                {
                    Page.Title = "Neues Projekt";
                    SiteTitle.Text = "Neues Projekt anlegen";

                    FillDropPreviousProject(Semester.CurrentSemester(db));
                    dropPreviousProject.SelectedIndex = 0;
                    radioClientType.SelectedIndex = (int)ClientType.Internal;
                    divClientForm.Visible = false;
                    FillDropAdvisors();
                }
                if (Request.QueryString["showChanges"] == null)
                    ToggleReservationTwoVisible();
            }
        }
        private void FillDropAdvisors()
        {
            dropAdvisor1.DataSource = db.UserDepartmentMap.Where(i => i.CanBeAdvisor1 && i.IsActive).OrderBy(a => a.Name);
            dropAdvisor1.DataBind();
            dropAdvisor1.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor1.SelectedIndex = 0;
            dropAdvisor2.DataSource = db.UserDepartmentMap.Where(i => i.IsActive).OrderBy(a => a.Name);
            dropAdvisor2.DataBind();
            dropAdvisor2.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor2.SelectedValue = db.UserDepartmentMap.Single(i => i.Mail == ShibUser.GetEmail()).Id.ToString();
        }

        private void FillDropPreviousProject(Semester projectSemester)
        {
            var lastSem = Semester.LastSemester(projectSemester, db);
            //var beforeLastSem = Semester.LastSemester(lastSem, db);
            dropPreviousProject.DataSource = db.Projects.Where(p =>
                p.LogProjectType.P5 && !p.LogProjectType.P6
                && p.State == ProjectState.Published
                && (p.SemesterId == projectSemester.Id || p.SemesterId == lastSem.Id && p.LogProjectDuration == 2));
            dropPreviousProject.DataBind();
            dropPreviousProject.Items.Insert(0, new ListItem("-", "dropPreviousProjectImpossibleValue"));
        }

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

            CreatorID.Text = project.Creator + "/" + project.CreateDate.ToString("yyyy-MM-dd");
            AddPictureLabel.Text = "Bild ändern:";
            ProjectNameLabel.Text = CreateSimpleDiffString(project.Name, currentProject.Name);
            dropAdvisor1Label.Text = CreateSimpleDiffString(project.Advisor1?.Name ?? "", currentProject.Advisor1?.Name ?? "");
            dropAdvisor2Label.Text = CreateSimpleDiffString(project.Advisor2?.Name ?? "", currentProject.Advisor2?.Name ?? "");

            if (currentProject.TypeDesignUX)
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectType[0] = true;

                if (!project.TypeDesignUX)
                {
                    DesignUX.BorderStyle = BorderStyle.Solid;
                    DesignUX.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeDesignUX)
                {
                    DesignUX.BorderStyle = BorderStyle.Solid;
                    DesignUX.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeHW)
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectType[1] = true;
                if (!project.TypeHW)
                {
                    HW.BorderStyle = BorderStyle.Solid;
                    HW.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeHW)
                {
                    HW.BorderStyle = BorderStyle.Solid;
                    HW.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeCGIP)
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectType[2] = true;
                if (!project.TypeCGIP)
                {
                    CGIP.BorderStyle = BorderStyle.Solid;
                    CGIP.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeCGIP)
                {
                    CGIP.BorderStyle = BorderStyle.Solid;
                    CGIP.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeMlAlg)
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlg.png";
                projectType[3] = true;
                if (!project.TypeMlAlg)
                {
                    MlAlg.BorderStyle = BorderStyle.Solid;
                    MlAlg.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeMlAlg)
                {
                    MlAlg.BorderStyle = BorderStyle.Solid;
                    MlAlg.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeAppWeb)
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectType[4] = true;
                if (!project.TypeAppWeb)
                {
                    AppWeb.BorderStyle = BorderStyle.Solid;
                    AppWeb.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeAppWeb)
                {
                    AppWeb.BorderStyle = BorderStyle.Solid;
                    AppWeb.BorderColor = Color.Red;
                }
            }
            if (currentProject.TypeDBBigData)
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectType[5] = true;
                if (!project.TypeDBBigData)
                {
                    DBBigData.BorderStyle = BorderStyle.Solid;
                    DBBigData.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeDBBigData)
                {
                    DBBigData.BorderStyle = BorderStyle.Solid;
                    DBBigData.BorderColor = Color.Red;
                }
            }
            if (currentProject.TypeSysSec)
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectType[6] = true;
                if (!project.TypeSysSec)
                {
                    SysSec.BorderStyle = BorderStyle.Solid;
                    SysSec.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeSysSec)
                {
                    SysSec.BorderStyle = BorderStyle.Solid;
                    SysSec.BorderColor = Color.Red;
                }
            }

            if (currentProject.TypeSE)
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectType[7] = true;
                if (!project.TypeSE)
                {
                    SE.BorderStyle = BorderStyle.Solid;
                    SE.BorderColor = Color.Green;
                }
            }
            else
            {
                if (project.TypeSE)
                {
                    SE.BorderStyle = BorderStyle.Solid;
                    SE.BorderColor = Color.Red;
                }
            }

            POneType.SelectedValue = project.POneType.Id.ToString();
            PTwoType.SelectedValue = project.PTwoType?.Id.ToString();
            POneTeamSize.SelectedValue = project.POneTeamSize.Id.ToString();
            PTwoTeamSize.SelectedValue = project.PTwoTeamSize?.Id.ToString();

            if (project.LanguageEnglish && !project.LanguageGerman)
                Language.SelectedIndex = 2;
            else if (!project.LanguageEnglish && project.LanguageGerman)
                Language.SelectedIndex = 1;
            else
                Language.SelectedIndex = 0;

            //DurationOneSemester.Checked = project.DurationOneSemester;
            InitialPositionContentLabel.Text = CreateDiffString(project.InitialPosition, currentProject.InitialPosition);

            ObjectivContentLabel.Text = CreateDiffString(project.Objective, currentProject.Objective);

            ProblemStatementContentLabel.Text = CreateDiffString(project.ProblemStatement, currentProject.ProblemStatement);

            ReferencesContentLabel.Text = CreateDiffString(project.References, currentProject.References);

            RemarksContentLabel.Text = CreateDiffString(project.Remarks, currentProject.Remarks);


            Reservation1NameLabel.Text = CreateSimpleDiffString(project.Reservation1Name, currentProject.Reservation1Name);

            Reservation1MailLabel.Text = CreateSimpleDiffString(project.Reservation1Mail, currentProject.Reservation1Mail);

            Reservation2NameLabel.Text = CreateSimpleDiffString(project.Reservation2Name, currentProject.Reservation2Name);

            Reservation2MailLabel.Text = CreateSimpleDiffString(project.Reservation2Mail, currentProject.Reservation2Mail);

            DepartmentLabel.Text = CreateSimpleDiffString(project.Department.DepartmentName, currentProject.Department.DepartmentName);

            // Button visibility
            saveProject.Visible = false;
            submitProject.Visible = false;
            publishProject.Visible = false;
            refuseProject.Visible = false;
            rollbackProject.Visible = false;
            saveCloseProject.Visible = false;
            duplicateProject.Visible = false;

            POneTypeLabel.Text = CreateSimpleDiffString(project.POneType.Description, currentProject.POneType.Description);
            POneTeamSize.SelectedValue = CreateSimpleDiffString(project.POneTeamSize.Description, currentProject.POneTeamSize.Description);

            Reservation1MailLabel.Text = CreateSimpleDiffString(project?.Reservation1Mail, currentProject?.Reservation1Mail);
            Reservation1NameLabel.Text = CreateSimpleDiffString(project.Reservation1Name, currentProject?.Reservation1Name);
            Reservation2MailLabel.Text = CreateSimpleDiffString(project.Reservation2Mail, currentProject?.Reservation2Mail);
            Reservation2NameLabel.Text = CreateSimpleDiffString(project.Reservation2Name, currentProject?.Reservation2Name);

            PopulateClientWithDiffStrings(project, currentProject);

            PrepareClientForm(project);

            FillDropPreviousProject(project.Semester);

            if (project.PreviousProjectID == null)
            {
                dropPreviousProject.SelectedValue = project.PreviousProjectID.ToString();
                PrepareForm(false);
            }
            else
            {
                dropPreviousProject.SelectedValue = project.PreviousProjectID?.ToString() ??
                                                    "dropPreviousProjectImpossibleValue";
                PrepareForm(true);
            }
        }
        private void UpdateUIFromProjectObject()
        {
            if (Request.QueryString["showChanges"] != null)
            {
                //disable pdf update for showChanges view
                Pdfupdatetimer.Enabled = false;

                PopulateHistoryGUI();
                return;
            }
            var activeStartdate = Semester.ActiveSemester(DateTime.Now, db).StartDate;
            CreatorID.Text = project.Creator + "/" + project.CreateDate.ToString("yyyy-MM-dd");
            AddPictureLabel.Text = "Bild ändern:";

            ProjectName.Text = project.Name;

            dropAdvisor1.DataSource = db.UserDepartmentMap.Where(i => i.CanBeAdvisor1 && (i.IsActive || i.Id == project.Advisor1Id)).OrderBy(a => a.Name);
            dropAdvisor1.DataBind();
            dropAdvisor1.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor1.SelectedValue = project.Advisor1?.Id.ToString() ?? "ImpossibleValue";
            dropAdvisor2.DataSource = db.UserDepartmentMap.Where(i => i.IsActive || i.Id == project.Advisor2Id).OrderBy(a => a.Name);
            dropAdvisor2.DataBind();
            dropAdvisor2.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            dropAdvisor2.SelectedValue = project.Advisor2?.Id.ToString() ?? "ImpossibleValue";

            if (project.TypeDesignUX)
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectType[0] = true;
            }
            if (project.TypeHW)
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectType[1] = true;
            }
            if (project.TypeCGIP)
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectType[2] = true;
            }
            if (project.TypeMlAlg)
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlg.png";
                projectType[3] = true;
            }
            if (project.TypeAppWeb)
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectType[4] = true;
            }
            if (project.TypeDBBigData)
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectType[5] = true;
            }
            if (project.TypeSysSec)
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectType[6] = true;
            }
            if (project.TypeSE)
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectType[7] = true;
            }

            POneType.SelectedValue = project.POneType.Id.ToString();
            PTwoType.SelectedValue = project.PTwoType?.Id.ToString();
            POneTeamSize.SelectedValue = project.POneTeamSize.Id.ToString();
            PTwoTeamSize.SelectedValue = project.PTwoTeamSize?.Id.ToString();

            if (project.LanguageEnglish && !project.LanguageGerman)
                Language.SelectedIndex = 2;
            else if (!project.LanguageEnglish && project.LanguageGerman)
                Language.SelectedIndex = 1;
            else
                Language.SelectedIndex = 0;

            //DurationOneSemester.Checked = project.DurationOneSemester;

            chkNDA.Checked = project.UnderNDA;

            InitialPositionContent.Text = project.InitialPosition;
            ProjectPicture.Visible = true;
            if (project.Picture != null)
            {
                ProjectPicture.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(project.Picture.ToArray());
                DeleteImageButton.Visible = true;
                imgdescription.Text = project.ImgDescription;
            }
            else
            {
                ImageLabel.Visible = false;
                ProjectPicture.Visible = false;
            }

            ObjectivContent.Text = project.Objective;
            ProblemStatementContent.Text = project.ProblemStatement;
            ReferencesContent.Text = project.References;
            RemarksContent.Text = project.Remarks;

            Reservation1Name.Text = project.Reservation1Name;
            Reservation1Mail.Text = project.Reservation1Mail;
            Reservation2Name.Text = project.Reservation2Name;
            Reservation2Mail.Text = project.Reservation2Mail;

            Department.SelectedValue = project.Department.Id.ToString();

            // Button visibility
            saveProject.Visible = true;
            submitProject.Visible = id.HasValue && project.UserCanSubmit();
            publishProject.Visible = project.UserCanPublish();
            refuseProject.Visible = project.UserCanReject();
            rollbackProject.Visible = project.UserCanUnpublish() || project.UserCanUnsubmit();

            POneType.SelectedValue = project?.POneType.Id.ToString();
            POneTeamSize.SelectedValue = project.POneTeamSize.Id.ToString();

            Reservation1Mail.Text = project.Reservation1Mail;
            Reservation1Name.Text = project.Reservation1Name;
            Reservation2Mail.Text = project.Reservation2Mail;
            Reservation2Name.Text = project.Reservation2Name;

            DisplayClient(project);



            FillDropPreviousProject(project.Semester);

            if (project.PreviousProjectID == null)
            {
                dropPreviousProject.SelectedValue = project.PreviousProjectID.ToString();
                PrepareForm(false);
            }
            else
            {
                dropPreviousProject.SelectedValue = project.PreviousProjectID?.ToString() ??
                                                    "dropPreviousProjectImpossibleValue";
                PrepareForm(true);
            }
        }


        /// <summary>
        ///     Saves changes to the project in the database.
        /// </summary>
        private void SaveProject()
        {
            if (project == null) // New project
            {
                project = ProjectExtensions.CreateNewProject(db);
                project.IsMainVersion = true;
                UpdatePDFPageCount(project);
            }
            else if (HasProjectChanged())
            {

                SaveProjectFromEditView();

            }
        }

        private void UpdatePDFPageCount(Project project)
        {
            UpdateProjectFromFormData(project);
            db.SubmitChanges(); // the next few lines depend on this submit
            project.BaseVersionId = project.Id;
            project.OverOnePage = new PdfCreator().CalcNumberOfPages(project) > 1;
            db.SubmitChanges();
        }

        private void SaveProjectFromEditView()
        {
            if (!project.UserCanEdit())
                throw new UnauthorizedAccessException();

            Project oldProject = project;
            UpdateProjectFromFormData(project, oldProject);
            project = oldProject.SaveAsNewVersion(db);
            if (imageChanged)
            {
                project.Picture = picture;
            }
            db.SubmitChanges(); // the next few lines depend on this submit    
            if (oldProject.Picture != null && project.Picture == null)
                project.Picture = oldProject.Picture;
            project.OverOnePage = new PdfCreator().CalcNumberOfPages(project) > 1;
            db.SubmitChanges();
        }


        private bool HasProjectChanged()
        {
            var comparisonProject = new Project();
            UpdateProjectFromFormData(comparisonProject);
            comparisonProject.Id = -1;


            ///////////////////////////////////////
            if (comparisonProject.Picture != null)
            {
                picture = comparisonProject.Picture; //Hack because the picture data will be lost after the first read 
            }                                        //so this will save it for later use
            ///////////////////////////////////////

            ///////////////////////////////////////////////
            db.Projects.InsertOnSubmit(comparisonProject); //Hack so that the project is not submitted
            db.Projects.DeleteOnSubmit(comparisonProject);
            ///////////////////////////////////////////////

            if (!comparisonProject.IsModified(project) && !imageChanged)
                return false;

            db.SubmitChanges();
            return true;
        }



        private void ToggleReservationTwoVisible()
        {
            var showResTwo = POneTeamSize.SelectedIndex != 0 ||
                             PTwoTeamSize.SelectedIndex != 0 && PTwoTeamSize.SelectedIndex != 1;
            Reservation2Name.Visible = showResTwo;
            Reservation2Mail.Visible = showResTwo;
        }

        #endregion

        #region Click handlers: Project categories

        protected void DesignUX_Click(object sender, ImageClickEventArgs e)
        {
            if (DesignUX.ImageUrl == "pictures/projectTypDesignUXUnchecked.png")
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUX.png";
                projectType[0] = true;
            }
            else
            {
                DesignUX.ImageUrl = "pictures/projectTypDesignUXUnchecked.png";
                projectType[0] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void HW_Click(object sender, ImageClickEventArgs e)
        {
            if (HW.ImageUrl == "pictures/projectTypHWUnchecked.png")
            {
                HW.ImageUrl = "pictures/projectTypHW.png";
                projectType[1] = true;
            }
            else
            {
                HW.ImageUrl = "pictures/projectTypHWUnchecked.png";
                projectType[1] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void CGIP_Click(object sender, ImageClickEventArgs e)
        {
            if (CGIP.ImageUrl == "pictures/projectTypCGIPUnchecked.png")
            {
                CGIP.ImageUrl = "pictures/projectTypCGIP.png";
                projectType[2] = true;
            }
            else
            {
                CGIP.ImageUrl = "pictures/projectTypCGIPUnchecked.png";
                projectType[2] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void MlAlg_Click(object sender, ImageClickEventArgs e)
        {
            if (MlAlg.ImageUrl == "pictures/projectTypMlAlgUnchecked.png")
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlg.png";
                projectType[3] = true;
            }
            else
            {
                MlAlg.ImageUrl = "pictures/projectTypMlAlgUnchecked.png";
                projectType[3] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void AppWeb_Click(object sender, ImageClickEventArgs e)
        {
            if (AppWeb.ImageUrl == "pictures/projectTypAppWebUnchecked.png")
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWeb.png";
                projectType[4] = true;
            }
            else
            {
                AppWeb.ImageUrl = "pictures/projectTypAppWebUnchecked.png";
                projectType[4] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void DBBigData_Click(object sender, ImageClickEventArgs e)
        {
            if (DBBigData.ImageUrl == "pictures/projectTypDBBigDataUnchecked.png")
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigData.png";
                projectType[5] = true;
            }
            else
            {
                DBBigData.ImageUrl = "pictures/projectTypDBBigDataUnchecked.png";
                projectType[5] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void SysSec_Click(object sender, ImageClickEventArgs e)
        {
            if (SysSec.ImageUrl == "pictures/projectTypSysSecUnchecked.png")
            {
                SysSec.ImageUrl = "pictures/projectTypSysSec.png";
                projectType[6] = true;
            }
            else
            {
                SysSec.ImageUrl = "pictures/projectTypSysSecUnchecked.png";
                projectType[6] = false;
            }
            ViewState["Types"] = projectType;
        }

        protected void SE_Click(object sender, ImageClickEventArgs e)
        {
            if (SE.ImageUrl == "pictures/projectTypSEUnchecked.png")
            {
                SE.ImageUrl = "pictures/projectTypSE.png";
                projectType[7] = true;
            }
            else
            {
                SE.ImageUrl = "pictures/projectTypSEUnchecked.png";
                projectType[7] = false;
            }
            ViewState["Types"] = projectType;
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
                    var currentProject = db.Projects.Single(p => p.BaseVersionId == project.BaseVersionId && p.IsMainVersion && p.State != ProjectState.Deleted);
                    currentProject.IsMainVersion = false;
                    db.SubmitChanges();
                    var revertedProject = db.Projects.Single(p => p.Id == pid);
                    revertedProject.IsMainVersion = true;
                    db.SubmitChanges();
                    Response.Redirect("~/AddNewProject.aspx?id=" + pid);
                    break;
                case "showChanges":
                    var mainProject = db.Projects.Single(p => p.BaseVersionId == project.BaseVersionId && p.IsMainVersion && p.State != ProjectState.Deleted).Id;
                    Response.Redirect("~/AddNewProject.aspx?id=" + pid + "&showChanges=" + mainProject);
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
            SaveProject();
            Response.Redirect("AddNewProject?id=" + project.Id);
        }

        /// <summary>
        ///     Save the current state of the form and return to project list.
        /// </summary>
        protected void SaveCloseProjectButton(object sender, EventArgs e)
        {
            SaveProject();
            Response.Redirect("projectlist");
        }


        protected void CancelNewProject_Click(object sender, EventArgs e)
        {
            Response.Redirect("projectlist");
        }

        protected void SubmitProject_Click(object sender, EventArgs e)
        {
            UpdateProjectFromFormData(project);
            var validationMessage = project.GenerateValidationMessage(projectType);
            // Generate JavaScript alert with error message
            if (validationMessage != "")
            {
                var sb = new StringBuilder();
                sb.Append("<script type = 'text/javascript'>");
                sb.Append("alert('");
                sb.Append(validationMessage);
                sb.Append("');");
                sb.Append("</script>");
                ScriptManager.RegisterStartupScript(Page, Page.GetType()
                                                      , "alert", sb.ToString(), false);
            }
            else
            {
                project.Submit(db);
                project.SaveAsNewVersion(db);
                Response.Redirect("projectlist");
            }
        }

        #endregion

        #region Click handlers: Buttons (admin only)

        protected void PublishProject_Click(object sender, EventArgs e)
        {
            project.Publish(db);
            SaveProject();
#if !DEBUG // Notification e-mail
            var mailMessage = new MailMessage();
            mailMessage.To.Add(project.Creator);
            if (project.Advisor1?.Mail != null && project.Advisor1.Mail.IsValidEmail() && project.Advisor1.Mail != project.Creator)
                mailMessage.To.Add(project.Advisor1.Mail);
            if (project.Advisor2?.Mail != null && project.Advisor2.Mail.IsValidEmail() && project.Advisor2.Mail != project.Creator)
                mailMessage.To.Add(project.Advisor2.Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.Subject = $"Projekt '{project.Name}' veröffentlicht";
            mailMessage.Body = $"Dein Projekt '{project.Name}' wurde von {ShibUser.GetFirstName()} veröffentlicht.\n"
                + "\n"
                + "----------------------\n"
                + "Automatische Nachricht von ProStudCreator\n"
                + "https://www.cs.technik.fhnw.ch/prostud/";

            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
#endif

            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        protected void RefuseProject_Click(object sender, EventArgs e)
        {
            refusedReason.Visible = true;
            refuseProject.Visible = false;
            publishProject.Visible = false;
            duplicateProject.Visible = false;
            saveProject.Visible = false;

            refusedReasonText.Text = $"Dein Projekt '{project.Name}' wurde leider nicht gewählt. Bitte informiere allfällige externe Auftraggeber!\n"
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
            project.Reject();
            db.Projects.Single(p => p.Id == id).Ablehnungsgrund = refusedReasonText.Text;
            SaveProject();
#if !DEBUG
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(project.Creator);
            if (project.Advisor1?.Mail != null && project.Advisor1.Mail.IsValidEmail() && project.Advisor1.Mail != project.Creator)
                mailMessage.To.Add(project.Advisor1.Mail);
            if (project.Advisor2?.Mail != null && project.Advisor2.Mail.IsValidEmail() && project.Creator != project.Advisor2.Mail)
                mailMessage.To.Add(project.Advisor2.Mail);
            mailMessage.From = new MailAddress(ShibUser.GetEmail());
            mailMessage.CC.Add(ShibUser.GetEmail());
            mailMessage.Subject = $"Projekt '{project.Name}' abgelehnt";
            mailMessage.Body =
refusedReasonText.Text + "\n\n----------------------\nAutomatische Nachricht von ProStudCreator\nhttps://www.cs.technik.fhnw.ch/prostud/";
            var smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
#endif
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
            if (project.UserCanUnpublish())
                project.Unpublish();
            if (project.UserCanUnsubmit())
                project.Unsubmit();
            db.SubmitChanges();
            Response.Redirect(Session["LastPage"] == null ? "projectlist" : (string)Session["LastPage"]);
        }

        #endregion

        #region Other view event handlers

        protected void DeleteImage_Click(object sender, EventArgs e)
        {
            SaveProject();
            project.Picture = null;
            db.SubmitChanges();
            Response.Redirect("~/AddNewProject?id=" + project.Id);
        }

        protected void TeamSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleReservationTwoVisible();
        }

        protected void DropPreviousProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dropPreviousProject.SelectedValue == "dropPreviousProjectImpossibleValue")
            {
                DisplayClient(null);
                DisplayPriority(null);
                DisplayReservations(null);
            }
            else
            {
                var previousProject = db.Projects.Single(p => p.Id == int.Parse(dropPreviousProject.SelectedValue));
                DisplayClient(previousProject);
                DisplayReservations(previousProject);
                DisplayPriority(previousProject);
            }
            updateReservation.Update();
            updateClient.Update();
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

        #endregion

        private void UpdateProjectFromFormData(Project project, Project oldProject = null)
        {
            project.Name = ProjectName.Text.FixupParagraph();

            if (dropAdvisor1.SelectedValue == "ImpossibleValue")
                project.Advisor1 = null;
            else
                project.Advisor1 = db.UserDepartmentMap.Single(user => user.Id == int.Parse(dropAdvisor1.SelectedValue));

            if (dropAdvisor2.SelectedValue == "ImpossibleValue")
                project.Advisor2 = null;
            else
                project.Advisor2 = db.UserDepartmentMap.Single(user => user.Id == int.Parse(dropAdvisor2.SelectedValue));

            if (radioClientType.SelectedIndex != (int)ClientType.Internal)
            {
                if (radioClientType.SelectedIndex == (int)ClientType.Company)
                {
                    project.ClientCompany = txtClientCompany.Text.FixupParagraph();
                    project.ClientType = (int)ClientType.Company;
                }
                else
                {
                    project.ClientCompany = "";
                    project.ClientType = (int)ClientType.PrivatePerson;
                }
                project.ClientAddressTitle = drpClientTitle.SelectedItem.Text;
                project.ClientPerson = txtClientName.Text.FixupParagraph();
                project.ClientAddressDepartment = txtClientDepartment.Text.FixupParagraph();
                project.ClientAddressStreet = txtClientStreet.Text.FixupParagraph();
                project.ClientAddressPostcode = txtClientPLZ.Text.FixupParagraph();
                project.ClientAddressCity = txtClientCity.Text.FixupParagraph();
                project.ClientReferenceNumber = txtClientReference.Text.FixupParagraph();
                project.ClientMail = txtClientEmail.Text.Trim().ToLowerInvariant();
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
                                            project.ClientMail = "";
            }

            // Project categories
            project.TypeDesignUX = projectType[0];
            project.TypeHW = projectType[1];
            project.TypeCGIP = projectType[2];
            project.TypeMlAlg = projectType[3];
            project.TypeAppWeb = projectType[4];
            project.TypeDBBigData = projectType[5];
            project.TypeSysSec = projectType[6];
            project.TypeSE = projectType[7];

            // Languages
            if (Language.SelectedIndex == 0)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = true;
            }
            else if (Language.SelectedIndex == 1)
            {
                project.LanguageGerman = true;
                project.LanguageEnglish = false;
            }
            else if (Language.SelectedIndex == 2)
            {
                project.LanguageGerman = false;
                project.LanguageEnglish = true;
            }
            else
                throw new Exception($"Unexpected language selection {Language.SelectedIndex}");

            // Duration
            //project.DurationOneSemester = DurationOneSemester.Checked;

            // Team size
            project.P1TypeId = int.Parse(POneType.SelectedValue);
            project.P1TeamSizeId = int.Parse(POneTeamSize.SelectedValue);
            if (PTwoType.SelectedIndex == 0 || PTwoTeamSize.SelectedIndex == 0)
            {
                project.P2TypeId = null;
                project.P2TeamSizeId = null;
            }
            else
            {
                project.P2TypeId = int.Parse(PTwoType.SelectedValue);
                project.P2TeamSizeId = int.Parse(PTwoTeamSize.SelectedValue);
            }
            if (project.P1TeamSizeId == project.P2TeamSizeId && project.P1TypeId == project.P2TypeId)
            {
                project.P2TeamSizeId = null;
                project.P2TypeId = null;
            }

            // Long texts (description etc.)
            project.InitialPosition = InitialPositionContent.Text.FixupParagraph();
            project.Objective = ObjectivContent.Text.FixupParagraph();
            project.ProblemStatement = ProblemStatementContent.Text.FixupParagraph();
            project.References = ReferencesContent.Text.FixupParagraph();
            project.Remarks = RemarksContent.Text.FixupParagraph();

            project.UnderNDA = chkNDA.Checked;

            // Student reservations
            project.Reservation1Name = Reservation1Name.Text.FixupParagraph();
            project.Reservation1Mail = Reservation1Mail.Text.Trim().ToLowerInvariant();

            if (Reservation2Name.Visible
            ) // TODO Check team size instead of visibility (just because it makes more sense)
            {
                project.Reservation2Name = Reservation2Name.Text.FixupParagraph();
                project.Reservation2Mail = Reservation2Mail.Text.Trim().ToLowerInvariant();
            }
            else
            {
                project.Reservation2Name = "";
                project.Reservation2Mail = "";
            }

            // Move reservation 2 to 1 if 1 isn't specified
            if (project.Reservation1Name == "" && project.Reservation2Name != "")
            {
                project.Reservation1Name = project.Reservation2Name;
                project.Reservation2Name = "";
            }

            int oldDepartmentId = oldProject != null ? oldProject.DepartmentId : -1;
            project.Department = db.Departments.Single(d => d.Id == int.Parse(Department.SelectedValue));

            // If project changed departments & already has a ProjectNr, generate a new one
            if (project.DepartmentId != oldDepartmentId && project.ProjectNr > 0)
            {
                project.ProjectNr = 0; // 'Remove' project number to allow finding a new one.
                project.AssignUniqueProjectNr(db);
            }

            if (project.Semester == null)
                if (db.Projects.Where(p => p.BaseVersionId == project.BaseVersionId).Count() == 0)
                    project.Semester = Semester.NextSemester(db);

            //picture description
            project.ImgDescription = imgdescription.Text.FixupParagraph();

            if (AddPicture.HasFile)
            {
                var fileExtension = AddPicture.FileName.Split('.').Last().ToLower();
                if (fileExtension != "jpg" && fileExtension != "jpeg" && fileExtension != "png")
                {
                    return;
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

            //Previous Project

            if (dropPreviousProject.SelectedValue == "dropPreviousProjectImpossibleValue")
            {
                project.PreviousProjectID = null;
            }
            else
            {
                var PreviousProjectID = int.Parse(dropPreviousProject.SelectedValue);
                project.PreviousProject = db.Projects.Single(p => p.Id == PreviousProjectID);
            }
        }

        private void DisplayReservations(Project previousProject)
        {
            if (previousProject == null)
            {
                Reservation1Name.Enabled = Reservation1Mail.Enabled = Reservation2Mail.Enabled = Reservation2Name.Enabled = true;
                Reservation1Mail.Text = Reservation1Name.Text = Reservation2Mail.Text = Reservation2Name.Text = "";
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

        private void DisplayClient(Project project)
        {
            if (project == null)
            {
                drpClientTitle.SelectedIndex = 0; //Default

                txtClientCompany.Text =
                    txtClientName.Text =
                        txtClientDepartment.Text =
                            txtClientStreet.Text =
                                txtClientPLZ.Text =
                                    txtClientCity.Text =
                                        txtClientReference.Text =
                                            txtClientEmail.Text = "";

                divClientForm.Visible = false;
                radioClientType.SelectedIndex = (int)ClientType.Internal;
            }
            else
            {
                txtClientCompany.Text = project?.ClientCompany;
                if (project?.ClientAddressTitle != null)
                {
                    drpClientTitle.SelectedIndex = project?.ClientAddressTitle == "Herr" ? 1 : 2;
                }
                else
                {
                    drpClientTitle.SelectedIndex = 0;
                }
                txtClientName.Text = project?.ClientPerson;
                txtClientDepartment.Text = project?.ClientAddressDepartment;
                txtClientStreet.Text = project?.ClientAddressStreet;
                txtClientPLZ.Text = project?.ClientAddressPostcode;
                txtClientCity.Text = project?.ClientAddressCity;
                txtClientReference.Text = project?.ClientReferenceNumber;
                txtClientEmail.Text = project?.ClientMail;

                PrepareClientForm(project);
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
        }

        private void DisplayPriority(Project project)
        {
            if (project == null)
            {
                POneTeamSize.Enabled = POneType.Enabled = PTwoTeamSize.Enabled = PTwoType.Enabled = true;
                POneType.SelectedValue = db.ProjectTypes.Single(p => p.P5 && p.P6).Id.ToString();
                POneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => p.Size1 && p.Size2).Id.ToString();
                divPriorityTwo.Visible = true;
            }
            else
            {
                divPriorityTwo.Visible = false;
                POneTeamSize.Enabled = POneType.Enabled = PTwoTeamSize.Enabled = PTwoType.Enabled = false;
                POneType.SelectedValue = db.ProjectTypes.Single(p => !p.P5 && p.P6).Id.ToString();
                if (string.IsNullOrEmpty(project.LogStudent2Mail))
                    POneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => p.Size1 && !p.Size2).Id.ToString();
                else
                    POneTeamSize.SelectedValue = db.ProjectTeamSizes.Single(p => !p.Size1 && p.Size2).Id.ToString();
            }
        }


        private void PrepareForm(bool hasPreviousProj)
        {
            //Priority
            divPriorityTwo.Visible = POneTeamSize.Enabled = POneType.Enabled = PTwoTeamSize.Enabled = PTwoType.Enabled = !hasPreviousProj;
            //Reservations
            Reservation1Name.Enabled = Reservation1Mail.Enabled = Reservation2Mail.Enabled = Reservation2Name.Enabled = !hasPreviousProj;
        }

        private void PrepareClientForm(Project project)
        {
            switch (project.ClientType)
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

        private void CollapseHistory(bool collapse)
        {
            Session["AddInfoCollapsed"] = collapse;
            DivHistoryCollapsable.Visible = !collapse;
            btnHistoryCollapse.Text = collapse ? "◄" : "▼";
        }

        protected void DuplicatProject_Click(object sender, EventArgs e)
        {
            var duplicate = project.DuplicateAndUseAsTemplate(db);

            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                Response.Redirect("~/AddNewProject.aspx?id=" + duplicate.Id);
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You clicked NO!')", true);
            }
        }
    }
}
