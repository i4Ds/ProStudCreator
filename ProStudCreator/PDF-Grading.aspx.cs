using iTextSharp.text;
using iTextSharp.text.pdf;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace ProStudCreator
{
    public partial class PDFGrading : Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var forceDl = bool.Parse(Request.QueryString["dl"]);
            var student = int.Parse(Request.QueryString["student"]);
            var proj = db.Projects.Single(i => i.Id == id);

            var studentName = student == 1 ? proj.GetStudent1FullName() : proj.GetStudent2FullName();

            if (!(ShibUser.IsAuthenticated(db) || proj.IsAtLeastPublished()))
            {
                Response.Redirect($"error/AccessDenied.aspx?url={HttpContext.Current.Request.Url.PathAndQuery}");
                Response.End();
                return;
            }

            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                var margin = Utilities.MillimetersToPoints(20f);
                var document = new Document(PageSize.A4, margin, margin, margin, margin);

                try
                {
                    var pdfCreator = new PdfGradingV1Creator();
                    pdfCreator.AppendToPDF(output, proj, student, document);
                    document.Dispose();
                }
                catch
                {
                    try
                    {
                        document.Dispose();
                    }
                    catch
                    {
                    }
                    throw;
                }

                bytesInStream = output.ToArray();
            }
            Response.Clear();

            if (forceDl)
            {
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", $"attachment; filename=\"{proj.GetFilename()}-{studentName}.pdf\"");
            }
            else
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"inline; filename=\"{proj.GetFilename()}-{studentName}.pdf\"");
            }
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }
    }

    public class PdfGradingV1Creator
    {
        public const float LINE_HEIGHT = 1.1f;
        public const float SPACING_BEFORE_TITLE = 16f;

        public static readonly string pathSegoeUI = HttpContext.Current.Request.MapPath("~/pictures/Arial-Unicode.ttf");
        public static readonly string pathSegoeUIBold = HttpContext.Current.Request.MapPath("~/pictures/Arial-Unicode-Bold.ttf");
        public static readonly Font fontTitle = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 10);
        public static readonly Font fontHeading = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 8);
        public static readonly Font fontRegular = FontFactory.GetFont(pathSegoeUI, BaseFont.IDENTITY_H, true, 8);
        public Font fontsmall = FontFactory.GetFont(FontFactory.HELVETICA, 6);
        private readonly HyphenationAuto hyph = new HyphenationAuto("de", "none", 2, 2);
        public float SPACING_AFTER_TITLE = 2f;
        public float SPACING_BEFORE_IMAGE = 16f;
        private Translator translator = new Translator();
        private string strLang;

        public void AppendToPDF(Stream output, Project project, int studentIdx, Document document)
        {
            var grading = studentIdx == 1 ? project.Student1GradingV1 : project.Student2GradingV1;
            var writer = PdfWriter.GetInstance(document, output);

            // the image we're using for the page header      
            var imageHeader = Image.GetInstance(HttpContext.Current.Request.MapPath("~/pictures/Logo.png"));

            // instantiate the custom PdfPageEventHelper
            var ef = new PdfHeaderFooterGenerator
            {
                ImageHeader = imageHeader
            };

            // and add it to the PdfWriter
            writer.PageEvent = ef;
            document.Open();

            ef.CurrentProject = project;

            translator.DetectLanguage(project);

            var cb = writer.DirectContent;

            cb.SetColorFill(BaseColor.BLACK);

            {
                var title = new Paragraph("Beurteilungsbogen für die Bachelor-Thesis", fontTitle).Hyphenate(hyph);
                title.SpacingBefore = 16f;
                title.SpacingAfter = 16f;
                title.SetLeading(0.0f, LINE_HEIGHT);
                document.Add(title);
            }

            {
                var t = new PdfPTable(2) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 20, 80 });

                t.AddCell(new Paragraph("Student:", fontHeading));
                t.AddCell(new Paragraph(studentIdx == 1 ? project.GetStudent1FullName() : project.GetStudent2FullName(), fontHeading));

                t.AddCell(new Paragraph("Projekttitel:", fontHeading));
                t.AddCell(new Paragraph(project.Name, fontHeading));
                t.Rows[1].GetCells()[1].FixedHeight = 30f;

                t.AddCell(new Paragraph("Betreuender Dozent:", fontHeading));
                t.AddCell(new Paragraph(
                    project.Advisor2 != null
                    ? project.Advisor1.Name + ", " + project.Advisor2.Name
                    : project.Advisor1.Name,
                    fontHeading));

                t.AddCell(new Paragraph("Experte:", fontHeading));
                t.AddCell(new Paragraph(project.Expert?.Name ?? "?", fontHeading));

                foreach (var r in t.Rows)
                    r.GetCells()[1].HorizontalAlignment = PdfPCell.ALIGN_CENTER;

                document.Add(t);
            }

            {
                var t = new PdfPTable(1) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 100 });

                t.AddCell(new Paragraph(grading.CriticalAcclaim, fontRegular));
                t.Rows[0].GetCells()[0].FixedHeight = 400f;

                document.Add(t);
            }

            {
                var t = new PdfPTable(2) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 90, 10 });

                t.AddCell(new Paragraph("Note, übertragen vom Bewertungsbogen", fontHeading));
                t.AddCell(new Paragraph(grading.ComputeFinalGrade().ToString("0.0", CultureInfo.InvariantCulture), fontHeading));

                t.Rows[0].GetCells()[1].HorizontalAlignment = PdfPCell.ALIGN_RIGHT;

                document.Add(t);
            }

            {
                var t = new PdfPTable(1) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 100 });

                t.AddCell(new Paragraph("Klassifizierung der Arbeit (Zutreffendes ankreuzen):", fontHeading));
                t.AddCell(new Paragraph((project.UnderNDA ? "☐" : "☑") + " Grundsätzlich zur Veröffentlichung geeignet (nach Absprache mit dem Auftraggeber)", fontRegular));
                t.AddCell(new Paragraph((project.UnderNDA ? "☑" : "☐") + " Aus Gründen der Vertraulichkeit nicht zur Veröffentlichung und Einsichtnahme geeignet", fontRegular));

                document.Add(t);
            }


            {
                var it = new PdfPTable(2) { WidthPercentage = 100f };
                it.DefaultCell.Border = Rectangle.NO_BORDER;
                it.SetWidths(new float[] { 20, 80 });
                it.AddCell(new Paragraph("Windisch,", fontRegular));
                it.AddCell(new Paragraph(DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), fontRegular));
                it.AddCell(new Paragraph("Betreuender Dozent:", fontRegular));
                it.AddCell(new Paragraph("", fontRegular));
                it.AddCell(new Paragraph("Experte:", fontRegular));
                it.AddCell(new Paragraph("", fontRegular));
                foreach (var r in it.Rows)
                    r.GetCells()[1].FixedHeight = 40f;

                var t = new PdfPTable(1) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 100 });
                t.AddCell(it);

                document.Add(t);
            }

            document.NewPage();
        }

        private class PdfHeaderFooterGenerator : PdfPageEventHelper
        {
            /*
             * We use a __single__ Image instance that's assigned __once__;
             * the image bytes added **ONCE** to the PDF file. If you create 
             * separate Image instances in OnEndPage()/OnEndPage(), for example,
             * you'll end up with a much bigger file size.
             */
            public Image ImageHeader { get; set; }

            public Project CurrentProject { get; set; }

            private Translator translator = new Translator(); //Sprache spielt keine Rolle, da die verwendete Methode diese selbst herausfindet.

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                ///////////////////////////////////////////////////////
                // HEADER 
                //////////////////////////////////////////////////////

                // cell height 
                var cellHeight = document.TopMargin;
                // PDF document size      
                var page = document.PageSize;

                // create two column table
                var head = new PdfPTable(1);
                head.TotalWidth = page.Width + 2;
                head.DefaultCell.Border = Rectangle.NO_BORDER;

                // add image; PdfPCell() overload sizes image to fit cell
                var c = new PdfPCell(ImageHeader, true);
                c.HorizontalAlignment = Element.ALIGN_MIDDLE;
                c.FixedHeight = cellHeight - 15;
                c.PaddingLeft = 35;
                c.PaddingTop = 5;
                c.PaddingBottom = 5;
                c.Border = Rectangle.NO_BORDER;
                head.AddCell(c);

                // since the table header is implemented using a PdfPTable, we call
                // WriteSelectedRows(), which requires absolute positions!
                head.WriteSelectedRows(
                    0, -1, // first/last row; -1 flags all write all rows
                    -1, // left offset
                        // ** bottom** yPos of the table
                    page.Height - cellHeight + head.TotalHeight,
                    writer.DirectContent
                );

                ///////////////////////////////////////////////////////
                // FOOTER 
                //////////////////////////////////////////////////////

                //translator.DetectLanguage(CurrentProject);

                //var cell = new PdfPCell(new Phrase(translator.GetHeadingFooter(CurrentProject, db), new Font(Font.FontFamily.HELVETICA, 8)));
                //cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                //cell.FixedHeight = document.TopMargin - 15;
                //cell.PaddingLeft = 58;
                //cell.PaddingTop = 8;
                //cell.PaddingBottom = 0;
                //cell.Border = Rectangle.NO_BORDER;

                //var foot = new PdfPTable(1);
                //foot.TotalWidth = document.PageSize.Width + 2;
                //foot.DefaultCell.Border = Rectangle.NO_BORDER;
                //foot.AddCell(cell);
                //foot.WriteSelectedRows(0, -1, -1, 40, writer.DirectContent);
            }
        }
    }
}