using System;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using ICSharpCode.SharpZipLib.Zip;

namespace ProStudCreator
{
    public partial class ProjectInfoPage : Page
    {
        public static bool OverRide = false;
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Project project;

        private ProjectTypes type = ProjectTypes.NotDefined;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Retrieve the project from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                project = db.Projects.Single(p => (int?)p.Id == id);
                if (!project.IsMainVersion)
                {
                    project = db.Projects.Single(p => p.BaseVersionId == project.BaseVersionId && p.IsMainVersion);
                    Response.Redirect(@"~/ProjectInfoPage?id=" + project.Id);
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

            gridProjectAttachs.DataSource = db.Attachements.Where(item => item.ProjectId == project.Id && !item.Deleted)
                .Select(i => GetProjectSingleAttachment(i));
            gridProjectAttachs.DataBind();

            if (Page.IsPostBack)
            {
                id = int.Parse(Request.QueryString["id"]);
                project = db.Projects.Single(p => (int?)p.Id == id);
                return;
            }


            //set the Semester if it isn't set already
            project.Semester = project.Semester ?? Semester.NextSemester(db);

            //Project title
            ProjectTitle.Text = project.Name;

            //Topic pictures
            Topic1.ImageUrl = "pictures/projectTyp" + (project.TypeDesignUX
                   ? "DesignUX"
                   : (project.TypeHW
                       ? "HW"
                       : (project.TypeCGIP
                           ? "CGIP"
                           : (project.TypeMlAlg
                               ? "MlAlg"
                               : (project.TypeAppWeb
                                   ? "AppWeb"
                                   : (project.TypeDBBigData
                                       ? "DBBigData"
                                       : (project.TypeSysSec
                                           ? "SysSec"
                                           : (project.TypeSE ? "SE" : "Transparent")))))))) + ".png";
            Topic2.ImageUrl = "pictures/projectTyp" + (project.TypeHW && project.TypeDesignUX
                               ? "HW"
                               : (project.TypeCGIP && (project.TypeDesignUX || project.TypeHW)
                                   ? "CGIP"
                                   : (project.TypeMlAlg && (project.TypeDesignUX || project.TypeHW || project.TypeCGIP)
                                       ? "MlAlg"
                                       : (project.TypeAppWeb &&
                                          (project.TypeDesignUX || project.TypeHW || project.TypeCGIP || project.TypeMlAlg)
                                           ? "AppWeb"
                                           : (project.TypeDBBigData &&
                                              (project.TypeDesignUX || project.TypeHW || project.TypeCGIP || project.TypeMlAlg ||
                                               project.TypeAppWeb)
                                               ? "DBBigData"
                                               : (project.TypeSysSec &&
                                                  (project.TypeDesignUX || project.TypeHW || project.TypeCGIP || project.TypeMlAlg ||
                                                   project.TypeAppWeb || project.TypeDBBigData)
                                                   ? "SysSec"
                                                   : (project.TypeSE && (project.TypeDesignUX || project.TypeHW || project.TypeCGIP ||
                                                                   project.TypeMlAlg || project.TypeAppWeb ||
                                                                   project.TypeDBBigData || project.TypeSysSec)
                                                       ? "SE"
                                                       : "Transparent"))))))) + ".png";

            chkNDA.Checked = project.UnderNDA;

            //Set the Students
            Student1Name.Text = !string.IsNullOrEmpty(project.LogStudent1Name)
                ? "<a href=\"mailto:" + project.LogStudent1Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent1Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Student2Name.Text = !string.IsNullOrEmpty(project.LogStudent2Name)
                ? "<a href=\"mailto:" + project.LogStudent2Mail + "\">" +
                  Server.HtmlEncode(project.LogStudent2Name).Replace(" ", "&nbsp;") + "</a>"
                : "";

            //Set the Advisor
            Advisor1Name.Text = project.Advisor1 != null
                ? "<a href=\"mailto:" + project.Advisor1?.Mail + "\">" +
                  Server.HtmlEncode(project.Advisor1.Name).Replace(" ", "&nbsp;") + "</a>"
                : "?";
            Advisor2Name.Text = project.Advisor2 != null
                ? "<a href=\"mailto:" + project.Advisor2.Mail + "\">" +
                  Server.HtmlEncode(project.Advisor2.Name).Replace(" ", "&nbsp;") + "</a>"
                : "";


            //Set the Expert
            if (project.LogProjectType != null)
                if (!project.LogProjectType.P5 && project.LogProjectType.P6)
                    ExpertName.Text = !string.IsNullOrEmpty(project.LogExpertID.ToString())
                        ? "<a href=\"mailto:" + project.Expert.Mail + "\">" +
                          Server.HtmlEncode(project.Expert.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "Noch nicht entschieden";
                else
                    ExpertName.Text = "Noch nicht entschieden";
            else
                ExpertName.Text = "Noch nicht entschieden";

            drpExpert.DataSource = db.Experts.Where(i => i.Active).OrderBy(a => a.Name).Select(x => 
                new {
                    Id = x.id,
                    Mail = x.Mail,
                    DropDownString = string.Format("{0} | {1}", x.Name,x.Knowhow).Replace(" ", HttpUtility.HtmlDecode("&nbsp;"))
                }
            );

            drpExpert.DataValueField = "Id";
            drpExpert.DataTextField = "DropDownString";
            drpExpert.DataBind();
            drpExpert.Items.Insert(0, new ListItem("-", "ImpossibleValue"));
            drpExpert.SelectedValue = project.Expert?.id.ToString() ?? "ImpossibleValue";


            ExpertMail.Text = !string.IsNullOrEmpty(project.LogExpertID.ToString())
            ? "<a href=\"mailto:" + project.Expert.Mail + "\">Mail</a>"
            : "";

            //Set the Projecttype
            if (project.LogProjectTypeID == null)
            {
                lblProjectType.Text = "?";
                type = ProjectTypes.NotDefined;
            }
            else if (project.LogProjectType.P5 && !project.LogProjectType.P6)
            {
                lblProjectType.Text = "IP5" + (project.LogProjectDuration==2 ? " (Lang)":"");
                type = ProjectTypes.IP5;
            }
            else if (!project.LogProjectType.P5 && project.LogProjectType.P6)
            {
                lblProjectType.Text = "IP6" + (project.LogProjectDuration == 2 ? " (Lang)" : "");
                type = ProjectTypes.IP6;
            }
            else
                throw new Exception();

            //Sets the DeliveryInfos
            SetProjectDeliveryLabels();

            //Set the Exhibition date? of the Bachelorthesis
            ProjectExhibition.Text = project.ExhibitionBachelorThesis(db);

            //Set the LogLanguage
            if (project.LogLanguageEnglish != null && project.LogLanguageGerman != null)
                if (project.LogLanguageEnglish.Value && !project.LogLanguageGerman.Value)
                    drpLogLanguage.SelectedValue = "1";
                else
                    drpLogLanguage.SelectedValue = "2";
            else
                drpLogLanguage.SelectedValue = "0";

            drpLogLanguage.Items[0].Text = project.UserCanEditAfterStart() ? "(Bitte Auswählen)" : "Noch nicht entschieden";

            //web summary checked?
            cbxWebSummaryChecked.Checked = project.WebSummaryChecked;

            lblWebsummaryLink.Text = !string.IsNullOrEmpty(project.Semester.Name)
                ? project.LogProjectTypeID != null
                    ? "<a target=\"_blank\" href=\"https://web.fhnw.ch/technik/projekte/i/" + (project.LogProjectTypeID == 1
                        ? "ip5" + project.Semester.Name.Substring(0,2)
                        : "bachelor" + project.Semester.Name.Substring(0, 2))
                    + "\">Link</a>"
                    : ""
                : "";

            //fill the Billingstatus dropdown with Data
            drpBillingstatus.DataSource = db.BillingStatus.OrderBy(i => i.DisplayName);
            drpBillingstatus.DataBind();
            drpBillingstatus.Items.Insert(0,
                new ListItem(project.UserCanEditAfterStart() ? "(Bitte Auswählen)" : "Noch nicht eingetragen",
                    "ValueWhichNeverWillBeGivenByTheDB"));
            drpBillingstatus.SelectedValue = project?.BillingStatusID?.ToString() ??
                                             "ValueWhichNeverWillBeGivenByTheDB";

            //set the Labels to the Grades
            lblGradeStudent1.Text = $"Note von {project.LogStudent1Name ?? "Student/in 1"}:";
            lblGradeStudent2.Text = $"Note von {project.LogStudent2Name ?? "Student/in 2"}:";

            //Set the Grades
            nbrGradeStudent1.Text = project.LogGradeStudent1 == null
                ? ""
                : project?.LogGradeStudent1.Value.ToString("N1", CultureInfo.InvariantCulture);
            nbrGradeStudent2.Text = project.LogGradeStudent2 == null
                ? ""
                : project?.LogGradeStudent2.Value.ToString("N1", CultureInfo.InvariantCulture);

            //Set the data from the addressform
            radioClientType.SelectedIndex = project.ClientType;
            txtClientCompany.Text = project.ClientCompany;
            drpClientTitle.SelectedValue = project.ClientAddressTitle == "Herr" ? "1" : "2";
            txtClientName.Text = project.ClientPerson;
            txtClientDepartment.Text = project.ClientAddressDepartment;
            txtClientStreet.Text = project.ClientAddressStreet;
            txtClientPLZ.Text = project.ClientAddressPostcode;
            txtClientCity.Text = project.ClientAddressCity;
            txtClientReference.Text = project.ClientReferenceNumber;
            txtClientEmail.Text = project.ClientMail;

            UpdateClientInfoFormVisibility();

            //disable for unauthorized Users
            ProjectTitle.Enabled = project.UserCanEditAfterStart() && project.CanEditTitle();

            drpLogLanguage.Enabled =
                nbrGradeStudent1.Enabled =
                    nbrGradeStudent2.Enabled =
                        BtnSaveBetween.Enabled =
                            BtnSaveChanges.Enabled =
                                drpBillingstatus.Enabled =
                                    chkNDA.Enabled = 
                                    cbxWebSummaryChecked.Enabled = radioClientType.Enabled = txtClientCompany.Enabled = drpClientTitle.Enabled = txtClientName.Enabled = txtClientDepartment.Enabled = txtClientStreet.Enabled = txtClientPLZ.Enabled = txtClientCity.Enabled = txtClientReference.Enabled = txtClientEmail.Enabled = project.UserCanEditAfterStart();

            DivExpert.Visible = project.Expert != null && !ShibUser.CanVisitAdminPage();
            DivExpertAdmin.Visible = ShibUser.CanVisitAdminPage() && project.LogStudent1Mail != null && project.LogProjectTypeID == 2;


            divBachelor.Visible = project.LogProjectType?.P6 ?? false;

            divGradeStudent1.Visible = !string.IsNullOrEmpty(project.LogStudent1Name);
            divGradeStudent2.Visible = !string.IsNullOrEmpty(project.LogStudent2Name);
            divGradeWarning.Visible = !string.IsNullOrEmpty(project.LogStudent1Name) || !string.IsNullOrEmpty(project.LogStudent2Name);

            divFileUpload.Visible = ShibUser.GetEmail() == project.Advisor1?.Mail ||
                                    ShibUser.GetEmail() == project.Advisor2?.Mail || ShibUser.CanEditAllProjects();

            SetGradeFieldStatus();
            updateGradeFields.Update();
        }

        public void DrpExpert_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExpertMail.Text = drpExpert.SelectedIndex != 0
                ? "<a href=\"mailto:" + db.Experts.FirstOrDefault(ex => ex.id == int.Parse(drpExpert.SelectedValue))?.Mail + "\">Mail</a>"
                : "";
        }

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


        private void SetProjectDeliveryLabels()
        {
            ProjectEndPresentation.Text = "?";

            if (type == ProjectTypes.IP5) //IP5 1/" Semester
            {
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
                ProjectEndPresentation.Text =
                    "Die Studierenden sollen die Schlusspräsentation (Termin, Ort, Auftraggeber) selbständig organisieren.";
            }
            else if (project.LogProjectDuration == 1 && type == ProjectTypes.IP6) //IP6 1 Semester
            {
                lblProjectEndPresentation.Text = "Verteidigung:";
                ProjectEndPresentation.Text = project.Semester.DefenseIP6Start + " - " + project.Semester.DefenseIP6End;
            }
            else if (project.LogProjectDuration == 2 && type == ProjectTypes.IP6) //IP6 2 Semester
            {
                lblProjectEndPresentation.Text = "Verteidigung:";
                ProjectEndPresentation.Text = project.Semester.DefenseIP6BStart + " - " + project.Semester.DefenseIP6BEnd;
            }
            else
            {
                lblProjectEndPresentation.Text = "Schlusspräsentation:";
            }

            var deliveryDate = project.GetDeliveryDate();
            ProjectDelivery.Text = deliveryDate?.ToString("dd.MM.yyyy") ?? "?";

            if (deliveryDate.HasValue)
            {
                if (project.CanEditTitle())
                {
                    if (project.LogProjectType?.P5 == true)
                        ChangeTitleDate.Text = "";
                    else
                        ChangeTitleDate.Text = $"Titeländerung noch bis {(deliveryDate.Value - Global.AllowTitleChangesBeforeSubmission).AddDays(-2).ToString("dd.MM.yyyy")} möglich!";
                }
                else
                    ChangeTitleDate.Text = $"Titeländerung war nur bis {(deliveryDate.Value - Global.AllowTitleChangesBeforeSubmission).AddDays(-2).ToString("dd.MM.yyyy")} möglich!";
            }

            ProjectEndPresentation.Text = (project.LogDefenceDate?.ToString() ?? "") +
                                        (project.LogDefenceRoom != null ? ", Raum: " + project?.LogDefenceRoom : "");
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

        protected void BtnSaveChanges_OnClick(object sender, EventArgs e)
        {
            SaveChanges("Projectlist");
        }

        protected void BtnCancel_OnClick(object sender, EventArgs e)
        {
            Response.Redirect("Projectlist");
        }

        
        protected void DrpBillingstatusChanged(object sender, EventArgs e)
        {
            SetGradeFieldStatus();
            updateGradeFields.Update();

            if (drpBillingstatus.SelectedValue == "ValueWhichNeverWillBeGivenByTheDB")
                return;

            if (db.BillingStatus.Single(bs => bs.Id == int.Parse(drpBillingstatus.SelectedValue)).ShowAddressOnInfoPage && radioClientType.SelectedIndex == (int)ClientType.Internal)
            {
                radioClientType.SelectedIndex = (int)ClientType.Company;
                UpdateClientInfoFormVisibility();
            }
        }

        protected void drpLogLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetGradeFieldStatus();
            updateGradeFields.Update();
        }

        protected void cbxWebSummaryChecked_CheckedChanged(object sender, EventArgs e)
        {
            SetGradeFieldStatus();
            updateGradeFields.Update();
        }

        private bool CheckGradeFieldStatus()
        {
            return cbxWebSummaryChecked.Checked
                && drpBillingstatus.SelectedIndex != 0
                && drpLogLanguage.SelectedIndex != 0;
        }

        private void SetGradeFieldStatus()
        {
            if (project.UserCanEditAfterStart())
            {
                nbrGradeStudent1.Enabled = nbrGradeStudent2.Enabled = CheckGradeFieldStatus();
                divGradeHint.Visible = !CheckGradeFieldStatus() && divGradeStudent1.Visible && divGradeStudent2.Visible;
            }
        }


        protected void BtnSaveBetween_OnClick(object sender, EventArgs e)
        {
            SaveChanges("ProjectInfoPage?id=" + project.Id);
        }

        private void SaveChanges(string redirectTo)
        {
            string validationMessage = null;
            if (project.UserCanEditAfterStart())
            {
                project.Name = ProjectTitle.Text.FixupParagraph();

                project.Expert = drpExpert.SelectedIndex == 0
                    ? null
                    : db.Experts.First(ex => ex.id == int.Parse(drpExpert.SelectedValue));

                project.WebSummaryChecked = cbxWebSummaryChecked.Checked;

                switch (drpLogLanguage.SelectedValue)
                {
                    case "1":
                        project.LogLanguageEnglish = true;
                        project.LogLanguageGerman = false;
                        break;
                    case "2":
                        project.LogLanguageEnglish = false;
                        project.LogLanguageGerman = true;
                        break;
                    default:
                        project.LogLanguageGerman = null;
                        project.LogLanguageEnglish = null;
                        break;
                }

                project.BillingStatusID = drpBillingstatus.SelectedIndex == 0
                    ? (int?)null
                    : int.Parse(drpBillingstatus.SelectedValue);

                if (nbrGradeStudent1.Text != "")
                {
                    if (!CheckGradeFieldStatus() && project.LogGradeStudent1 == null)
                    {
                        validationMessage = "Bitte füllen Sie die Felder Websummary, Durchführungssprache und Verrechnungsstatus aus, um die Noten einzutragen.";
                    }
                    else
                    {
                        var old = project.LogGradeStudent1;
                        project.LogGradeStudent1 = float.Parse(nbrGradeStudent1.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                        if (old != project.LogGradeStudent1)
                            project.GradeSentToAdmin = false;
                    }
                }

                if (nbrGradeStudent2.Text != "")
                {
                    if (!CheckGradeFieldStatus() && project.LogGradeStudent2 == null)
                    {
                        validationMessage = "Bitte füllen Sie die Felder Websummary, Durchführungssprache und Verrechnungsstatus aus, um die Noten einzutragen.";
                    }
                    else
                    {
                        var old = project.LogGradeStudent2;
                        project.LogGradeStudent2 = float.Parse(nbrGradeStudent2.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                        if (old != project.LogGradeStudent2)
                            project.GradeSentToAdmin = false;
                    }
                }

                project.UnderNDA = chkNDA.Checked;

                //this sould always be under the project.BillingstatusId statement
                if (radioClientType.SelectedValue != "Intern" && (txtClientCompany.Text + txtClientName.Text == "" || txtClientStreet.Text == "" || txtClientPLZ.Text == "" || txtClientCity.Text == "" || txtClientEmail.Text == ""))
                {
                    validationMessage = "Bitte füllen Sie alle Pflichtfelder aus.";
                }
                else
                {
                    project.ClientType = radioClientType.SelectedIndex;
                    project.ClientCompany = txtClientCompany.Text;
                    project.ClientAddressTitle = drpClientTitle.SelectedValue == "1" ? "Herr" : "Frau";
                    project.ClientPerson = txtClientName.Text;
                    project.ClientMail = txtClientEmail.Text;
                    project.ClientAddressDepartment = txtClientDepartment.Text == "" ? null : txtClientDepartment.Text;
                    project.ClientAddressStreet = txtClientStreet.Text == "" ? null : txtClientStreet.Text;
                    project.ClientAddressPostcode = txtClientPLZ.Text == "" ? null : txtClientPLZ.Text;
                    project.ClientAddressCity = txtClientCity.Text == "" ? null : txtClientCity.Text;
                    project.ClientReferenceNumber = txtClientReference.Text == "" ? null : txtClientReference.Text;
                }

                if (project.BillingStatus?.ShowAddressOnInfoPage == true && project.ClientType==(int)ClientType.Internal)
                {
                    validationMessage = "Dieser Verrechnungsstatus ist nur bei externen Auftraggebern verfügbar.";
                }

                if (validationMessage == null)
                {
                    project.Name = ProjectTitle.Text.FixupParagraph();
                    project.ModificationDate = DateTime.Now;
                    project.LastEditedBy = ShibUser.GetEmail();
                    db.SubmitChanges();
                    Response.Redirect(redirectTo);
                }
                else
                {
                    ReturnAlert(validationMessage);
                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        protected void RadioClientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateClientInfoFormVisibility();
        }

        private void UpdateClientInfoFormVisibility()
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

        protected void OnUploadComplete(object sender, AjaxFileUploadEventArgs e)
        {
            using(var s = e.GetStreamContents())
                if (db.Attachements.Any(a => a.ProjectId.ToString() == Request.QueryString["id"] && a.FileName == e.FileName && !a.Deleted))
                {
                    SaveFileInDb(db.Attachements.Single(a => a.ProjectId.ToString() == Request.QueryString["id"] && a.FileName == e.FileName && !a.Deleted), s);
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
                ProjectId = int.Parse(Request.QueryString["id"]),
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
            var project =
                db.Projects.Single(item => item.Id == ((ProjectSingleAttachment)e.Row.DataItem).BaseVersionId);

            if (!(ShibUser.GetEmail() == project.Advisor1?.Mail || ShibUser.GetEmail() == project.Advisor2?.Mail ||
                  !ShibUser.CanEditAllProjects()))
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


            gridProjectAttachs.DataSource = db.Attachements.Where(item => item.ProjectId == project.Id && !item.Deleted)
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

        private enum ProjectTypes
        {
            IP5,
            IP6,
            NotDefined
        }

        protected void DownloadFiles_OnClick(object sender, EventArgs e)
        {
            var attachments = db.Attachements.Where(item => item.ProjectId == project.Id && !item.Deleted).ToList();

            if (!ShibUser.IsAuthenticated(db))
            {
                Response.Redirect("error/AccessDenied.aspx");
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
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + project.Name + ".zip" + "\"");
                Response.AddHeader("Content-Length", totalSize.ToString());
                Response.ContentType = "text/plain";
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
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