using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public partial class ProjectListControl : System.Web.UI.UserControl
    {
        private IQueryable<ProjectRowElement> Projects { get; set; }
        public ProStudentCreatorDBDataContext db { get; set; }

        public GridView Grid { get; private set; }

        public bool ShowModificationDate { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Grid = ProjectGrid;
        }

        public void SetProjects(IQueryable<Project> projects)
        {
            Projects = projects.Select(p => GetProjectRowElement(p));
            Grid.DataSource = Projects;
            Grid.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Grid.Columns[10].Visible = ShowModificationDate;
            Grid.Columns[Grid.Columns.Count - 5].Visible = ShowModificationDate;
        }

        private ProjectRowElement GetProjectRowElement(Project p)
        {
            return new ProjectRowElement
            {
                id = p.Id,
                advisorName = string.Concat(new[]
                {
                    p.Advisor1 != null
                        ? "<a href=\"mailto:" + p.Advisor1.Mail + "\">" +
                            Server.HtmlEncode(p.Advisor1.Name).Replace(" ", "&nbsp;") + "</a>"
                        : "?",
                    p.Advisor2 != null
                        ? "<br /><a href=\"mailto:" + p.Advisor2.Mail + "\">" +
                            Server.HtmlEncode(p.Advisor2.Name).Replace(" ", "&nbsp;") + "</a>"
                        : ""
                }),
                projectName = p.Name,
                modDate = p.ModificationDate,
                projectTopic1 = p.GetProjectTopics(db).ElementAtOrDefault(0),
                projectTopic2 = p.GetProjectTopics(db).ElementAtOrDefault(1),

                p5 = p.LogProjectType?.P5 ?? (p.POneType.P5 || (p.PTwoType?.P5 ?? false)),
                p6 = p.LogProjectType?.P6 ?? (p.POneType.P6 || (p.PTwoType?.P6 ?? false)),
                lng = p.LogProjectDuration == (byte)2,

                SubmitToCS = p.LogStudyCourse is null ? p.SubmitToStudyCourseCS : p.LogStudyCourse == 1,
                SubmitToDS = p.LogStudyCourse is null ? p.SubmitToStudyCourseDS : p.LogStudyCourse == 2,
                
                ProjectNr = p.GetProjectLabel()
            };
        }

        protected void ProjectGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var project = db.Projects.Single(item => item.Id == ((ProjectRowElement)e.Row.DataItem).id);

                if (!project.UserCanView())
                {
                    var x = e.Row.Cells[e.Row.Cells.Count - 3].Controls;
                    e.Row.Cells[e.Row.Cells.Count - 2].Controls.OfType<DataBoundLiteralControl>().First().Visible = false; //edit
                }

                e.Row.Cells[e.Row.Cells.Count - 1].Controls.OfType<LinkButton>().First().Visible = project.UserCanDelete(); //delete

                Color col = ColorTranslator.FromHtml(project.StateColor);
                foreach (TableCell cell in e.Row.Cells)
                    cell.BackColor = col;
            }
        }

        protected void ProjectRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
            {
                List<ProjectRowElement> sortedProjects = Projects.ToList();

                switch (e.CommandArgument)
                {
                    case "Advisor":
                        Grid.DataSource = sortedProjects.OrderBy(p => p.advisorName.Contains("?")).ThenBy(p => p.advisorName);
                        break;
                    case "ProjectNr":
                        Grid.DataSource = sortedProjects.OrderBy(p => p.ProjectNr);
                        break;
                    case "projectName":
                        Grid.DataSource = sortedProjects.OrderBy(p => p.projectName);
                        break;
                    case "P5":
                        Grid.DataSource = sortedProjects.OrderByDescending(p => p.p5).ThenBy(p => p.lng);
                        break;
                    case "P6":
                        Grid.DataSource = sortedProjects.OrderByDescending(p => p.p6);
                        break;
                    case "Long":
                        Grid.DataSource = sortedProjects.OrderByDescending(p => p.lng);
                        break;
                    case "modDate":
                        Grid.DataSource = sortedProjects.OrderByDescending(p => p.modDate);
                        break;
                }

                Grid.DataBind();
            }
            else
            {
                var id = Convert.ToInt32(e.CommandArgument);
                switch (e.CommandName)
                {
                    case "deleteProject":
                        var project = db.Projects.Single(i => i.Id == id);
                        project.Delete(db);
                        db.SubmitChanges();
                        Response.Redirect(Request.RawUrl);
                        break;
                    default:
                        throw new Exception("Unknown command " + e.CommandName);
                }
            }
        }
        protected void ProjectGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            Grid.DataBind();
        }
    }

    public class ProjectRowElement
    {
        public int id { get; set; }
        public string ProjectNr { get; set; }
        public string advisorName { get; set; }
        public string projectName { get; set; }
        public Topic projectTopic1 { get; set; }
        public Topic projectTopic2 { get; set; }
        public bool p5 { get; set; }
        public bool p6 { get; set; }
        public bool lng { get; set; }
        public bool SubmitToCS { get; set; }
        public bool SubmitToDS { get; set; }
        public DateTime modDate { get; set; }
        public string modDateString { get => modDate.ToString("dd.MM.yyyy"); }
    }
}