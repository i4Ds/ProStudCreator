using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ProStudCreator.Ext
{
    public partial class Termine : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();
        //protected GridView AllEvents;

        protected void Page_Load(object sender, EventArgs e)
        {
            var semestersToDisplay = new[]
            {
                Semester.LastSemester(db), Semester.CurrentSemester(db), Semester.NextSemester(db),
                Semester.AfterNextSemester(db)
            };


            var dt = new DataTable();
            foreach (var header in new[]
            {
                "Semester",
                " ",
                "Projekteinreichung",
                "Info Veranstaltung",
                "Anmeldung ProApp",
                "Projektzuteilung",
                "Abgabe IP5",
                "Abgabe IP5 (BB/lang)",
                "Abgabe IP6<br/>Verteidigung",
                "Ausstellung Bachelorthesen"
            })
                dt.Columns.Add(header);

            foreach (var semester in semestersToDisplay)
                dt.Rows.Add(semester.Name,
                    $"{semester.StartDate.ToShortDateString()} bis {semester.EndDate.ToShortDateString()}",
                    semester.ProjectSubmissionUntil.AddDays(-7 * 6).ToShortDateString(),
                    semester.InfoEvent?.ToString("g", CultureInfo.CurrentCulture) ?? "?",
                    semester.ProAppApplication,
                    semester.ProjectAllocation,
                    semester.SubmissionIP5FullPartTime, 
                    semester.SubmissionIP5Accompanying,
                    semester.SubmissionIP6Normal + "<br/>" + (semester.DefenseIP6Start == null
                        ? ""
                        : $"{semester.DefenseIP6Start} bis {semester.DefenseIP6End}"),
                    /*semester.SubmissionIP6Variant2 + "<br/>" + (semester.DefenseIP6BStart == null
                        ? ""
                        : $"{semester.DefenseIP6BStart} bis {semester.DefenseIP6BEnd}"),*/
                    semester.ExhibitionBachelorThesis);


            var flipHeaders = new string[5];
            flipHeaders[0] = "Semester";
            var semesterHeaders = semestersToDisplay.Select(s => s.Name).ToArray();
            for (var i = 1; i < 5; i++)
                flipHeaders[i] = semesterHeaders[i - 1];


            var newTable = FlipDataTable(dt, flipHeaders);
            newTable.Rows[0].Delete();
            AllEvents.DataSource = newTable;
            AllEvents.DataBind();
        }

        public static DataTable FlipDataTable(DataTable dt, string[] semesters)
        {
            var table = new DataTable();
            //Get all the rows and change into columns
            for (var i = 0; i <= dt.Rows.Count; i++)
                table.Columns.Add(semesters[i]);


            //get all the columns and make it as rows
            for (var j = 0; j < dt.Columns.Count; j++)
            {
                var dr = table.NewRow();
                dr[0] = dt.Columns[j].ToString();
                for (var k = 1; k <= dt.Rows.Count; k++)
                    dr[k] = dt.Rows[k - 1][j];
                table.Rows.Add(dr);
            }
            return table;
        }

        protected void AllEvents_DataBinding(object sender, EventArgs e)
        {
            for (var i = 0; i < AllEvents.Rows.Count; i++)
            {
                for (var j = 0; j < AllEvents.Rows[i].Cells.Count; j++)
                    AllEvents.Rows[i].Cells[j].Text = HttpUtility.HtmlDecode(AllEvents.Rows[i].Cells[j].Text);


                if (i % 2 == 0)
                {
                    for (var j = 0; j < AllEvents.Rows[i].Cells.Count; j++)
                        if (j == 2)
                            AllEvents.Rows[i].Cells[j].BackColor = Color.FromArgb(198, 244, 203);
                }
                else
                {
                    for (var j = 0; j < AllEvents.Rows[i].Cells.Count; j++)
                        if (j == 2)
                            AllEvents.Rows[i].Cells[j].BackColor = Color.FromArgb(218, 236, 220);
                }
            }
        }
    }
}