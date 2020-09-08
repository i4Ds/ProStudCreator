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
    public partial class UserEditPage : Page
    {
        public readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        private int? id;
        private UserDepartmentMap pageUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Button color
            saveUser.BackColor = ColorTranslator.FromHtml("#A9F5A9");
            cancelUser.BackColor = ColorTranslator.FromHtml("#F5A9A9");

            // Retrieve the user from DB
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"]);
                pageUser = db.UserDepartmentMap.Single(u => (int?)u.Id == id);

                if (!(ShibUser.IsDepartmentManager(pageUser.Department) || ShibUser.IsWebAdmin()))
                {
                    Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.AbsolutePath}");
                    Response.End();
                }
            }

            if (!IsPostBack)
            {
                if (id.HasValue)
                {
                    //existing project
                    Page.Title = "User bearbeiten";
                    SiteTitle.Text = "User bearbeiten";

                    UpdateUIFromUserObject(false);
                }
                else
                {
                    //new project
                    Page.Title = "Neuer User";
                    SiteTitle.Text = "Neuen User anlegen";

                    UpdateUIFromUserObject(true);
                }
            }
        }

        #region Form

        private void UpdateUIFromUserObject(bool isNewUser)
        {
            // Name
            UserName.Text = pageUser?.Name ?? "";

            // Mail
            UserMail.Text = pageUser?.Mail ?? "";

            // Department
            FillDropDepartment(isNewUser);
            dropDepartment.Enabled = ShibUser.IsWebAdmin();

            // CanBeAdvisor1
            UserCanBeAdvisor1.Checked = pageUser?.CanBeAdvisor1 ?? false;

            // IsActive
            UserIsActive.Checked = pageUser?.IsActive ?? true;

            // CanExportExcel
            UserCanExportExcel.Checked = pageUser?.CanExportExcel ?? false;

            // CanPublishProject
            UserCanPublishProjects.Checked = pageUser?.CanPublishProject ?? false;

            // CanVisitAdminPage
            UserCanVisitAdminPage.Checked = pageUser?.CanVisitAdminPage ?? false;

            // CanSeeAllProjectsInProgress
            UserCanSeeAllProjectsInProgress.Checked = pageUser?.CanSeeAllProjectsInProgress ?? false;

            // CanEditAllProjects
            UserCanEditAllProjects.Checked = pageUser?.CanEditAllProjects ?? false;

            // CanSubmitAllProjects
            UserCanSubmitAllProjects.Checked = pageUser?.CanSubmitAllProjects ?? false;

            // CanReserveProjects
            UserCanReserveProjects.Checked = pageUser?.CanReserveProjects ?? false;

            // Button visibility
            saveUser.Visible = ShibUser.IsDepartmentManager() || ShibUser.IsWebAdmin();
            cancelUser.Visible = true;
            deleteUser.Visible = ShibUser.IsWebAdmin();
        }

        private void FillDropDepartment(bool isNewUser)
        {
            dropDepartment.DataSource = db.Departments;
            dropDepartment.DataBind();
            if (isNewUser)
            {
                dropDepartment.SelectedValue = ShibUser.GetDepartment()?.Id.ToString() ?? 0.ToString();
            }
            else
            {
                dropDepartment.SelectedValue = pageUser.Department.Id.ToString();
            }
        }

        #endregion

        #region Save

        private void UpdateUserFromFormData(UserDepartmentMap user)
        {
            // Name
            user.Name = UserName.Text.FixupParagraph();

            // Mail
            user.Mail = UserMail.Text.FixupParagraph();

            // Department
            int newDepartmentId = int.Parse(dropDepartment.SelectedValue);
            user.Department = db.Departments.Single(d => d.Id == newDepartmentId);

            // CanBeAdvisor1
            user.CanBeAdvisor1 = UserCanBeAdvisor1.Checked;

            // IsActive
            user.IsActive = UserIsActive.Checked;

            // CanExportExcel
            user.CanExportExcel = UserCanExportExcel.Checked;

            // CanPublishProject
            user.CanPublishProject = UserCanPublishProjects.Checked;

            // CanVisitAdminPage
            user.CanVisitAdminPage = UserCanVisitAdminPage.Checked;

            // CanSeeAllProjectsInProgress
            user.CanSeeAllProjectsInProgress = UserCanSeeAllProjectsInProgress.Checked;

            // CanEditAllProjects
            user.CanEditAllProjects = UserCanEditAllProjects.Checked;

            // CanSubmitAllProjects
            user.CanSubmitAllProjects = UserCanSubmitAllProjects.Checked;

            // CanReserveProjects
            user.CanReserveProjects = UserCanReserveProjects.Checked;
        }

        private void SaveUser()
        {
            if (pageUser == null) // New user
            {
                pageUser = CreateNewUser(db);
            }

            UpdateUserFromFormData(pageUser);
            db.SubmitChanges();
        }

        private void DeleteUser()
        {
            if (pageUser == null) // New user
            {
                return;
            }

            db.UserDepartmentMap.DeleteOnSubmit(pageUser);
            db.SubmitChanges();
        }

        public static UserDepartmentMap CreateNewUser(ProStudentCreatorDBDataContext db)
        {
            UserDepartmentMap user = new UserDepartmentMap()
            {
                Name = "",
                Mail = "",
                Department = ShibUser.GetDepartment() ?? db.Departments.Single(d => d.i4DS),
                CanBeAdvisor1 = false,
                CanExportExcel = false,
                CanPublishProject = false,
                CanVisitAdminPage = false,
                CanSeeAllProjectsInProgress = false,
                CanEditAllProjects = false,
                CanSubmitAllProjects = false,
                CanSeeCreationDetails = false,
                CanReserveProjects = false,
                IsDepartmentManager = false,
                IsActive = true,
            };

            db.UserDepartmentMap.InsertOnSubmit(user);
            return user;
        }

        #endregion

        #region Click handlers: Buttons

        protected void SaveCloseUserButton(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveUser();
                Response.Redirect("AdminPage.aspx");
            }
        }

        protected void CancelUserButton(object sender, EventArgs e)
        {
            Response.Redirect("AdminPage.aspx");
        }

        protected void DeleteUserButton(object sender, EventArgs e)
        {
            DeleteUser();
            Response.Redirect("AdminPage.aspx");
        }

        #endregion

    }
}
