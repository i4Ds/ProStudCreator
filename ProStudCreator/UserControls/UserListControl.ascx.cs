using AjaxControlToolkit;
using NPOI.SS.Formula.Atp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public partial class UserListControl : System.Web.UI.UserControl
    {

        public ProStudentCreatorDBDataContext db { get; set; }
        public GridView Grid { get; private set; }
        private IQueryable<UserRowElement> Users { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Grid = UserGrid;
        }

        public void SetUsers(IQueryable<UserDepartmentMap> users)
        {
            Users = users.Select(u => GetUserRowElement(u));
            Grid.DataSource = Users;
            Grid.DataBind();
        }

        private UserRowElement GetUserRowElement(UserDepartmentMap u)
        {
            return new UserRowElement
            {
                Id = u.Id,
                Name = u.Name,
                Mail = u.Mail,
                Department = u.Department,
                CanBeAdvisor1 = u.CanBeAdvisor1,
                IsActive = u.IsActive,
                CanExportExcel = u.CanExportExcel,
                CanPublishProject = u.CanPublishProject,
                CanVisitAdminPage = u.CanVisitAdminPage,
                CanSeeAllProjectsInProgress = u.CanSeeAllProjectsInProgress,
                CanEditAllProjects = u.CanEditAllProjects,
                CanSubmitAllProjects = u.CanSubmitAllProjects,
                CanReserveProjects = u.CanReserveProjects
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UserGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var user = db.UserDepartmentMap.Single(item => item.Id == ((UserRowElement)e.Row.DataItem).Id);

                Color col = ColorTranslator.FromHtml("#CEECF5");
                foreach (TableCell cell in e.Row.Cells)
                    cell.BackColor = col;
            }
        }

        protected void UserRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
            {
                List<UserRowElement> sortedUsers = Users.ToList();

                switch (e.CommandArgument)
                {
                    case "id":
                        Grid.DataSource = sortedUsers.OrderBy(u => u.Id);
                        break;
                    case "name":
                        Grid.DataSource = sortedUsers.OrderBy(u => u.Name);
                        break;
                    case "mail":
                        Grid.DataSource = sortedUsers.OrderBy(u => u.Mail);
                        break;
                    case "department":
                        Grid.DataSource = sortedUsers.OrderBy(u => u.Department.Id).ThenBy(u => u.Name);
                        break;
                    case "canbeadvisor1":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanBeAdvisor1).ThenBy(u => u.Name);
                        break;
                    case "canexportexcel":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanExportExcel).ThenBy(u => u.Name);
                        break;
                    case "canpublishproject":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanPublishProject).ThenBy(u => u.Name);
                        break;
                    case "canvisitadminpage":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanVisitAdminPage).ThenBy(u => u.Name);
                        break;
                    case "canseeallprojectsinprogress":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanSeeAllProjectsInProgress).ThenBy(u => u.Name);
                        break;
                    case "caneditallprojects":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanEditAllProjects).ThenBy(u => u.Name);
                        break;
                    case "cansubmitallprojects":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanSubmitAllProjects).ThenBy(u => u.Name);
                        break;
                    case "canreserveprojects":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.CanReserveProjects).ThenBy(u => u.Name);
                        break;
                    case "isactive":
                        Grid.DataSource = sortedUsers.OrderByDescending(u => u.IsActive).ThenBy(u => u.Name);
                        break;
                }

                Grid.DataBind();
            }
            else
            {
                var id = Convert.ToInt32(e.CommandArgument);
                switch (e.CommandName)
                {
                    default:
                        throw new Exception("Unknown command " + e.CommandName);
                }
            }
        }
        protected void UserGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            Grid.DataBind();
        }
    }

    public class UserRowElement
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public Department Department { get; set; }
        public string DepartmentName { get => Department.DepartmentName; }
        public bool CanBeAdvisor1 { get; set; }
        public bool IsActive { get; set; }
        public bool CanExportExcel { get; set; }
        public bool CanPublishProject { get; set; }
        public bool CanVisitAdminPage { get; set; }
        public bool CanSeeAllProjectsInProgress { get; set; }
        public bool CanEditAllProjects { get; set; }
        public bool CanSubmitAllProjects { get; set; }
        public bool CanReserveProjects { get; set; }
    }
}