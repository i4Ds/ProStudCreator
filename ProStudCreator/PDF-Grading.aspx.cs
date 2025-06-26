using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using NPOI.SS.UserModel;
using ProStudCreator.Ext;
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
        public static readonly string pathSegoeUI = HttpContext.Current.Request.MapPath("~/pictures/Arial-Unicode.ttf");
        public static readonly string pathSegoeUIBold = HttpContext.Current.Request.MapPath("~/pictures/Arial-Unicode-Bold.ttf");
        public static readonly Font fontHeading = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 10);
        public static readonly Font fontBold = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 8);
        public static readonly Font fontBoldItalic = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 8, Font.ITALIC);
        public static readonly Font fontBoldRed = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 8, Font.NORMAL, BaseColor.RED);
        public static readonly Font fontBoldBlue = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 8, Font.NORMAL, BaseColor.BLUE);
        public static readonly Font fontRegular = FontFactory.GetFont(pathSegoeUI, BaseFont.IDENTITY_H, true, 8);
        public static readonly Font fontSmall = FontFactory.GetFont(pathSegoeUI, BaseFont.IDENTITY_H, true, 6);
        public static readonly Font fontSmallBold = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 6);
        public static readonly Font fontSmallBoldItalic = FontFactory.GetFont(pathSegoeUIBold, BaseFont.IDENTITY_H, true, 6, Font.ITALIC);
        public static readonly Font fontSmallItalic = FontFactory.GetFont(pathSegoeUI, BaseFont.IDENTITY_H, true, 6, Font.ITALIC);
        public static readonly Font fontRegularRed = FontFactory.GetFont(pathSegoeUI, BaseFont.IDENTITY_H, true, 8, Font.NORMAL, BaseColor.RED);
        public static readonly Font fontRegularBlue = FontFactory.GetFont(pathSegoeUI, BaseFont.IDENTITY_H, true, 8, Font.NORMAL, BaseColor.BLUE);
        private readonly HyphenationAuto hyph = new HyphenationAuto("de", "none", 2, 2);
        private Translator translator = new Translator();

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

            translator.DetectLanguage(project);

            var cb = writer.DirectContent;

            cb.SetColorFill(BaseColor.BLACK);

            InsertOverviewPage(project, studentIdx, document, grading);
            InsertGradingTable(project, studentIdx, document, grading);
        }

        private void InsertOverviewPage(Project project, int studentIdx, Document document, GradingV1 grading)
        {
            document.Add(new Paragraph("Beurteilungsbogen für die Bachelor-Thesis", fontHeading)
            {
                SpacingAfter = 10f
            });

            {
                var t = new PdfPTable(2) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 20, 80 });

                t.AddCell(new Paragraph("Student:", fontBold));
                t.AddCell(new Paragraph(studentIdx == 1 ? project.GetStudent1FullName() : project.GetStudent2FullName(), fontBold));

                t.AddCell(new Paragraph("Projekttitel:", fontBold));
                t.AddCell(new Paragraph(project.Name, fontBold));
                t.Rows[1].GetCells()[1].FixedHeight = 30f;

                t.AddCell(new Paragraph("Betreuender Dozent:", fontBold));
                t.AddCell(new Paragraph(
                    project.Advisor2 != null
                    ? project.Advisor1.Name + ", " + project.Advisor2.Name
                    : project.Advisor1.Name,
                    fontBold));

                t.AddCell(new Paragraph("Experte:", fontBold));
                t.AddCell(new Paragraph(project.Expert?.Name ?? "?", fontBold));

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

                t.AddCell(new Paragraph("Note, übertragen vom Bewertungsbogen", fontBold));
                t.AddCell(new Paragraph(grading.ComputeFinalGrade().ToString("0.0", CultureInfo.InvariantCulture), fontBold));

                t.Rows[0].GetCells()[1].HorizontalAlignment = PdfPCell.ALIGN_RIGHT;

                document.Add(t);
            }

            {
                var t = new PdfPTable(1) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 100 });

                t.AddCell(new Paragraph("Klassifizierung der Arbeit (Zutreffendes ankreuzen):", fontBold));
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

        private void InsertGradingTable(Project project, int studentIdx, Document document, GradingV1 grading)
        {
            document.Add(new Paragraph("Bewertungsbogen: Projektarbeit 5 und Bachelorthesis (Projektarbeit 6)", fontHeading)
            {
                SpacingAfter = 10f
            });

            {
                var p = new Paragraph
                {
                    new Chunk("Bemerkungen: ", fontBold),
                    new Chunk("Dieser Bewertungsbogen wird von der betreunden Person ausgefüllt. Bei zwei betreuenden Personen wird er von beiden unabhängig ausgefüllt und danach abgeglichen. Wo möglich und sinnvoll wird ein Kommentar zu jeder Bewertung verfasst. Die Studierenden erhalten in jedem Fall die Würdigung in Papierform. Falls erwünscht, wird auch der Bewertungsbogen in PDF-From abgegeben. Nach der Projektarbeit 5 muss dieser Bewertungsbogen zwingen mit den Studierenden besprochen und auf mögliches Verbesserungspotential für die kommende Projektarbeit 6 hingewiesen werden. Nach Abschluss der Projektarbeit 6 wird der Bewertungsbogen auf Wunsch der Studierenden mit diesen besprochen.", fontRegular)
                };
                p.SpacingAfter = 10f;
                p.SetLeading(0, 0.8f);
                document.Add(p);
            }

            {
                var p = new Paragraph
                {
                    new Chunk("Grundsatz: ", fontBoldRed),
                    new Chunk("Die Note 5.0 ist zu erteilen, wenn für das jeweilige Kriterium die Leistung in vollem Umfang die Anforderungen an einen in der Industrie tätigen Ingenieur erfüllt.", fontRegularRed)
                };
                p.SpacingAfter = 10f;
                p.SetLeading(0, 0.8f);
                document.Add(p);
            }

            {
                var t = new PdfPTable(5) { SpacingAfter = 15f, WidthPercentage = 100f };
                t.SetWidths(new float[] { 5, 25, 10, 10, 50 });

                t.AddCell(new Paragraph("", fontBold));
                t.AddCell(new Paragraph("Name:", fontBold));
                t.AddCell(new Paragraph("Gewich-\ntung", fontBoldBlue));
                t.AddCell(new Paragraph("Note", fontBoldBlue));
                t.AddCell(new Paragraph("Beschreibung", fontBold));
                foreach (var c in t.Rows.Last().GetCells())
                    c.VerticalAlignment = Element.ALIGN_MIDDLE;
                t.Rows.Last().GetCells()[2].HorizontalAlignment = Element.ALIGN_CENTER;
                t.Rows.Last().GetCells()[3].HorizontalAlignment = Element.ALIGN_CENTER;

                {
                    t.AddCell(new Paragraph("1", fontBold));

                    var header = new PdfPCell(new Paragraph("ORGANISATION, PLANUNG, METHODIK", fontBold));
                    header.BackgroundColor = new BaseColor(0xD8, 0xE4, 0xBC);
                    header.Colspan = 4;
                    header.HorizontalAlignment = Element.ALIGN_CENTER;
                    t.AddCell(header);
                    t.Rows.Last().GetCells()[0].BackgroundColor = new BaseColor(0xD8,0xE4,0xBC);

                    t.AddCell(new Paragraph("1.1", fontRegular));
                    t.AddCell(new Paragraph()
                    {
                        new Chunk("Lösungskonzept/Strategie\n", fontBoldItalic),
                        new Chunk("Gewichtung aufgrund der Komplexität des Projektes festlegen.", fontSmallItalic),
                    });
                    t.AddCell(new Paragraph(grading.AStrategyWeight.ToString(CultureInfo.InvariantCulture), fontRegularBlue));
                    t.AddCell(new Paragraph(grading.AStrategy.ToString("0.0", CultureInfo.InvariantCulture), fontRegularBlue));
                    t.AddCell(new Paragraph(GradingV1.AStrategySchema, fontSmall));

                    t.AddCell(new Paragraph("1.2", fontRegular));
                    t.AddCell(new Paragraph("Projektvereinbarung: Inhalt", fontBoldItalic));
                    t.AddCell(new Paragraph(grading.AProjectSummaryContentsWeight.ToString(CultureInfo.InvariantCulture), fontRegularBlue));
                    t.AddCell(new Paragraph(grading.AProjectSummaryContents.ToString("0.0", CultureInfo.InvariantCulture), fontRegularBlue));
                    t.AddCell(new Paragraph(GradingV1.AProjectSummaryContentsSchema, fontSmall));

                    t.AddCell(new Paragraph("1.3", fontRegular));
                    t.AddCell(new Paragraph("Projektvereinbarung: Planung des Projektes", fontBoldItalic));
                    t.AddCell(new Paragraph(grading.AProjectSummaryPlanningWeight.ToString(CultureInfo.InvariantCulture), fontRegularBlue));
                    t.AddCell(new Paragraph(grading.AProjectSummaryPlanning.ToString("0.0", CultureInfo.InvariantCulture), fontRegularBlue));
                    t.AddCell(new Paragraph(GradingV1.AProjectSummaryPlanningSchema, fontSmall));
                }

                foreach (var r in t.Rows.Skip(1))
                {
                    var cells = r.GetCells();
                    cells[0].VerticalAlignment = Element.ALIGN_MIDDLE;
                    cells[0].HorizontalAlignment = Element.ALIGN_CENTER;
                    if (cells[2] == null)
                    {
                        //section overview
                    }
                    else
                    {
                        //regular row
                        cells[2].BackgroundColor = new BaseColor(0xd9, 0xd9, 0xd9);
                        cells[3].BackgroundColor = new BaseColor(0xd9, 0xd9, 0xd9);
                        cells[2].HorizontalAlignment = Element.ALIGN_CENTER;
                        cells[3].HorizontalAlignment = Element.ALIGN_CENTER;
                    }
                }

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

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                ///////////////////////////////////////////////////////
                // HEADER 
                //////////////////////////////////////////////////////

                var cellHeight = document.TopMargin;
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
                c.PaddingBottom = 15;
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
            }
        }
    }
}