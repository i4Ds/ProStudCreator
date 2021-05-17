using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ProStudCreator
{
    public partial class Termine : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected (Semester[], int) SemestersToDisplay()
        {
            var currSem = Semester.CurrentSemester(db);
            var changeDateString = Semester.LastSemester(currSem, db).DefenseIP6End;

            //change date
            var changeDate = DateTime.ParseExact(changeDateString, "dd.MM.yyyy", CultureInfo.InvariantCulture)
                + TimeSpan.FromDays(2 * 7);

            if (DateTime.Now >= changeDate)
            {
                return (new[]
                {
                    Semester.CurrentSemester(db), Semester.NextSemester(db), Semester.AfterNextSemester(db), Semester.NextSemester(Semester.AfterNextSemester(db), db)
                }, 0);
            }
            else
            {
                return (new[]
                {
                    Semester.LastSemester(db), Semester.CurrentSemester(db), Semester.NextSemester(db), Semester.AfterNextSemester(db)
                }, 1);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var semestersToDisplay = SemestersToDisplay().Item1;

            var dt = new DataTable();
            foreach (var header in new[]
            {
                "Semester",
                " ",
                "Projekteinreichung (extern)",
                "Projekteinreichung (intern)",
                "Info Veranstaltung",
                "Anmeldung ProApp",
                "Projektzuteilung",
                "Abgabe IP5",
                "Notenabgabe IP5",
                "Abgabe IP5 (Lang)",
                "Abgabe IP6<br/>Verteidigung",
                "Notenabgabe IP5 (Lang) / IP6",
                "Ausstellung Bachelorthesen"
            })
                dt.Columns.Add(header);

            foreach (var semester in semestersToDisplay)
            {
                var semName = semester?.Name ?? "?";
                var semFromTo = $"{semester?.StartDate.ToShortDateString() ?? "?"} bis {semester?.EndDate.ToShortDateString() ?? "?"}";
                var semProjSubExtern = semester?.ProjectSubmissionUntil.AddDays(-7 * 6).ToShortDateString() ?? "?";
                var semProjSubIntern = semester?.ProjectSubmissionUntil.ToShortDateString() ?? "?";
                var semInfoEvent = semester?.InfoEvent?.ToString("g", CultureInfo.CurrentCulture) ?? "?";
                var semProAppApp = semester?.ProAppApplication ?? "?";
                var semProjAllo = semester?.ProjectAllocation ?? "?";
                var semSubIP5N = semester?.SubmissionIP5FullPartTime ?? "?";
                var semGradeIP5N = semester?.GradeIP5Deadline.ToShortDateString() ?? "?";
                var semSubIP5L = semester?.SubmissionIP5Accompanying ?? "?";
                var semSubIP6 = $"{semester?.SubmissionIP6Normal ?? "?"}<br/>{(semester?.DefenseIP6Start == null ? "" : $"{semester?.DefenseIP6Start ?? "?"} bis {semester?.DefenseIP6End ?? "?"}")}";
                var semGradeIP6 = semester?.GradeIP6Deadline.ToShortDateString() ?? "?";
                var semExhib = semester?.ExhibitionBachelorThesis ?? "?";

                dt.Rows.Add(
                    semName,
                    semFromTo,
                    semProjSubExtern,
                    semProjSubIntern,
                    semInfoEvent,
                    semProAppApp,
                    semProjAllo,
                    semSubIP5N,
                    semGradeIP5N,
                    semSubIP5L,
                    semSubIP6,
                    semGradeIP6,
                    semExhib
                );
            }



            var flipHeaders = new string[5];
            flipHeaders[0] = "Semester";
            var semesterHeaders = semestersToDisplay.Select(s => s?.Name ?? "?").ToArray();
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
            var currSemIndex = SemestersToDisplay().Item2 + 1; // +1 for row titles

            for (var i = 0; i < AllEvents.Rows.Count; i++)
            {
                for (var j = 0; j < AllEvents.Rows[i].Cells.Count; j++)
                    AllEvents.Rows[i].Cells[j].Text = HttpUtility.HtmlDecode(AllEvents.Rows[i].Cells[j].Text);


                if (i % 2 == 0)
                {
                    for (var j = 0; j < AllEvents.Rows[i].Cells.Count; j++)
                        if (j == currSemIndex)
                            AllEvents.Rows[i].Cells[j].BackColor = Color.FromArgb(198, 244, 203);
                }
                else
                {
                    for (var j = 0; j < AllEvents.Rows[i].Cells.Count; j++)
                        if (j == currSemIndex)
                            AllEvents.Rows[i].Cells[j].BackColor = Color.FromArgb(218, 236, 220);
                }
            }
        }
    }
}