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
    public partial class ExpertEditPage : Page
    {
        public readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private Expert pageExpert;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Button color
            saveExpert.BackColor = ColorTranslator.FromHtml("#A9F5A9");
            cancelExpert.BackColor = ColorTranslator.FromHtml("#F5A9A9");

            // Retrieve the expert from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                pageExpert = db.Experts.Single(ex => (int?)ex.id == id);

                if (!(ShibUser.IsDepartmentManager() || ShibUser.IsWebAdmin()))
                {
                    Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.PathAndQuery}");
                    Response.End();
                }
            }

            if (!IsPostBack)
            {
                if (id.HasValue)
                {
                    //existing project
                    Page.Title = "Expert bearbeiten";
                    SiteTitle.Text = "Expert bearbeiten";

                    UpdateUIFromExpertObject();
                }
                else
                {
                    //new project
                    Page.Title = "Neuer Expert";
                    SiteTitle.Text = "Neuen Expert anlegen";

                    UpdateUIFromExpertObject();
                }
            }
        }

        #region Form

        private void UpdateUIFromExpertObject()
        {
            // Name
            ExpertName.Text = pageExpert?.Name ?? "";

            // Mail
            ExpertMail.Text = pageExpert?.Mail ?? "";

            // Unternehmen
            ExpertUnternehmen.Text = pageExpert?.Unternehmen ?? "";

            // Knowhow
            ExpertKnowhow.Text = pageExpert?.Knowhow ?? "";

            // AutomaticPayout
            ExpertAutomaticPayout.Checked = pageExpert?.AutomaticPayout ?? true;

            // IsActive
            ExpertIsActive.Checked = pageExpert?.Active ?? true;

            // Button visibility
            saveExpert.Visible = ShibUser.IsDepartmentManager() || ShibUser.IsWebAdmin();
            cancelExpert.Visible = true;
            deleteExpert.Visible = ShibUser.IsWebAdmin();
        }

        #endregion

        #region Save

        private void UpdateExpertFromFormData(Expert expert)
        {
            // Name
            expert.Name = ExpertName.Text.FixupParagraph();

            // Mail
            expert.Mail = ExpertMail.Text.FixupParagraph();

            // Unternehmen
            expert.Unternehmen = ExpertUnternehmen.Text.FixupParagraph();

            // Knowhow
            expert.Knowhow = ExpertKnowhow.Text.FixupParagraph();

            // CanBeAdvisor1
            expert.AutomaticPayout = ExpertAutomaticPayout.Checked;

            // IsActive
            expert.Active = ExpertIsActive.Checked;
        }

        private void SaveExpert()
        {
            if (pageExpert == null) // New expert
            {
                pageExpert = CreateNewExpert(db);
            }

            UpdateExpertFromFormData(pageExpert);
            db.SubmitChanges();
        }

        private void DeleteExpert()
        {
            if (pageExpert == null) // New expert
            {
                return;
            }

            db.Experts.DeleteOnSubmit(pageExpert);
            db.SubmitChanges();
        }

        public static Expert CreateNewExpert(ProStudentCreatorDBDataContext db)
        {
            Expert expert = new Expert()
            {
                Name = "",
                Mail = "",
                Unternehmen = "",
                Knowhow = "",
                AutomaticPayout = true,
                Active = true
            };

            db.Experts.InsertOnSubmit(expert);
            return expert;
        }

        #endregion

        #region Click handlers: Buttons

        protected void SaveCloseExpertButton(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveExpert();
                Response.Redirect("AdminPage.aspx");
            }
        }

        protected void CancelExpertButton(object sender, EventArgs e)
        {
            Response.Redirect("AdminPage.aspx");
        }

        protected void DeleteExpertButton(object sender, EventArgs e)
        {
            DeleteExpert();
            Response.Redirect("AdminPage.aspx");
        }

        #endregion

    }
}
