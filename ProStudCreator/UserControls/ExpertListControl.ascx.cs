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
    public partial class ExpertListControl : System.Web.UI.UserControl
    {

        public ProStudentCreatorDBDataContext db { get; set; }
        public GridView Grid { get; private set; }
        private IQueryable<ExpertRowElement> Experts { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            Grid = ExpertGrid;
        }

        public void SetExperts(IQueryable<Expert> experts)
        {
            Experts = experts.Select(ex => GetExpertRowElement(ex));
            Grid.DataSource = Experts;
            Grid.DataBind();
        }

        private ExpertRowElement GetExpertRowElement(Expert e)
        {
            return new ExpertRowElement
            {
                Id = e.id,
                Name = e.Name,
                Mail = e.Mail,
                Unternehmen = e.Unternehmen,
                Knowhow = e.Knowhow,
                AutomaticPayout = e.AutomaticPayout,
                IsActive = e.Active
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ExpertGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var expert = db.Experts.Single(item => item.id == ((ExpertRowElement)e.Row.DataItem).Id);

                Color col = ColorTranslator.FromHtml("#CEECF5");
                foreach (TableCell cell in e.Row.Cells)
                    cell.BackColor = col;
            }
        }

        protected void ExpertRowClick(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Sort")
            {
                List<ExpertRowElement> sortedExperts = Experts.ToList();

                switch (e.CommandArgument)
                {
                    case "id":
                        Grid.DataSource = sortedExperts.OrderBy(ex => ex.Id);
                        break;
                    case "name":
                        Grid.DataSource = sortedExperts.OrderBy(ex => ex.Name);
                        break;
                    case "mail":
                        Grid.DataSource = sortedExperts.OrderBy(ex => ex.Mail);
                        break;
                    case "unternehmen":
                        Grid.DataSource = sortedExperts.OrderBy(ex => ex.Unternehmen);
                        break;
                    case "knowhow":
                        Grid.DataSource = sortedExperts.OrderBy(ex => ex.Knowhow);
                        break;
                    case "automaticpayout":
                        Grid.DataSource = sortedExperts.OrderByDescending(ex => ex.AutomaticPayout).ThenBy(ex => ex.Name);
                        break;
                    case "isactive":
                        Grid.DataSource = sortedExperts.OrderByDescending(ex => ex.IsActive).ThenBy(ex => ex.Name);
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
        protected void ExpertGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            Grid.DataBind();
        }
    }

    public class ExpertRowElement
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string Unternehmen { get; set; }
        public string Knowhow { get; set; }
        public bool AutomaticPayout { get; set; }
        public bool IsActive { get; set; }
    }
}