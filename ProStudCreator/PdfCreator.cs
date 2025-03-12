using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ProStudCreator.UserControls;

namespace ProStudCreator
{
    public class PdfCreator
    {
        public const float LINE_HEIGHT = 1.1f;
        public const float SPACING_BEFORE_TITLE = 16f;

        private static readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        public Font fontHeading = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        public Font fontRegular = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        public Font fontsmall = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        private readonly HyphenationAuto hyph = new HyphenationAuto("de", "none", 2, 2);
        public float SPACING_AFTER_TITLE = 2f;
        public float SPACING_BEFORE_IMAGE = 16f;
        private Translator translator = new Translator();
        private string strLang;

        public void AppendToPDF(Document document, Stream output, IEnumerable<Project> projects)
        {
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

            foreach (var project in projects)
            {
                ef.CurrentProject = project;
                WritePDF(project, document, writer);
                writer.Flush();
                output.Flush();
            }
        }

        private void AppendToPDF(Stream output, IEnumerable<Project> projects, Layout layout, Document document)
        {
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

            foreach (var project in projects)
            {
                ef.CurrentProject = project;
                WritePDF(project, document, layout, writer);
            }
        }

        private void WritePDF(Project currentProject, Document document, PdfWriter writer)
        {
            foreach (Layout l in Enum.GetValues(typeof(Layout)))
                if (CalcNumberOfPages(currentProject, l) == 1)
                {
                    WritePDF(currentProject, document, l, writer);
                    return;
                }

            WritePDF(currentProject, document, Layout.SmallPictureRight, writer);
        }

        private void WritePDF(Project currentProject, Document document, Layout layout, PdfWriter writer)
        {
            var fontRegularLink = new Font(fontRegular);
            fontRegularLink.Color = BaseColor.BLUE;
            fontRegularLink.SetStyle("underline");
            translator.DetectLanguage(currentProject);
            
            //
            // Header contents
            //

            var proj = currentProject;

            // var topic1 = GetCurrentProjectTypeOne(proj);
            // var topic2 = GetCurrentProjectTypeTwo(proj);

            var topics = proj.GetProjectTopics(db);
            var topic1 = topics.ElementAtOrDefault(0);
            var topic2 = topics.ElementAtOrDefault(1);

            var cb = writer.DirectContent;

            if (topic1 != null)
            {
                Font topicFont = new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL, BaseColor.BLACK);

                // (var topic1name1, var topic1name2, _, var topic1color) = ProjectTopic.getTopicValues(topic1);
                var topic1name1 = topic1.FirstText;
                var topic1name2 = topic1.SecondText;
                var topic1color = $"#{topic1.Color}";

                var rectW = 50;
                var rectH = 32;

                var rectlly = PageSize.A4.Height - document.TopMargin + 10;
                var rect1llx = 388;

                cb.SetColorFill(new BaseColor(System.Drawing.ColorTranslator.FromHtml(topic1color)));
                cb.Rectangle(rect1llx, rectlly, rectW, rectH);
                cb.Fill();

                if (topic1name2 != null)
                {
                    ColumnText ct = new ColumnText(cb);
                    var phr1 = new Phrase(topic1name1, topicFont);
                    ct.SetSimpleColumn(phr1, rect1llx, rectlly + rectH / 2, rect1llx + rectW, rectlly + rectH, rectH / 4 + 4, Element.ALIGN_CENTER);
                    ct.Go();
                    ct = new ColumnText(cb);
                    var phr2 = new Phrase(topic1name2, topicFont);
                    ct.SetSimpleColumn(phr2, rect1llx, rectlly, rect1llx + rectW, rectlly + rectH / 2, rectH / 4 + 4, Element.ALIGN_CENTER);
                    ct.Go();
                }
                else
                {
                    ColumnText ct = new ColumnText(cb);
                    var phr1 = new Phrase(topic1name1, topicFont);
                    ct.SetSimpleColumn(phr1, rect1llx, rectlly, rect1llx + rectW, rectlly + rectH, rectH / 2 + 4, Element.ALIGN_CENTER);
                    ct.Go();
                }

                if (topic2 != null)
                {
                    var rect2llx = 443;
                    // (var topic2name1, var topic2name2, _, var topic2color) = ProjectTopic.getTopicValues(topic2);
                    var topic2name1 = topic2.FirstText;
                    var topic2name2 = topic2.SecondText;
                    var topic2color = $"#{topic2.Color}";

                    cb.SetColorFill(new BaseColor(System.Drawing.ColorTranslator.FromHtml(topic2color)));
                    cb.Rectangle(rect2llx, rectlly, rectW, rectH);
                    cb.Fill();

                    if (topic2name2 != null)
                    {
                        ColumnText ct = new ColumnText(cb);
                        var phr1 = new Phrase(topic2name1, topicFont);
                        ct.SetSimpleColumn(phr1, rect2llx, rectlly + rectH / 2, rect2llx + rectW, rectlly + rectH, rectH / 4 + 4, Element.ALIGN_CENTER);
                        ct.Go();
                        ct = new ColumnText(cb);
                        var phr2 = new Phrase(topic2name2, topicFont);
                        ct.SetSimpleColumn(phr2, rect2llx, rectlly, rect2llx + rectW, rectlly + rectH / 2, rectH / 4 + 4, Element.ALIGN_CENTER);
                        ct.Go();
                    }
                    else
                    {
                        ColumnText ct = new ColumnText(cb);
                        var phr1 = new Phrase(topic2name1, topicFont);
                        ct.SetSimpleColumn(phr1, rect2llx, rectlly, rect2llx + rectW, rectlly + rectH, rectH / 2 + 4, Element.ALIGN_CENTER);
                        ct.Go();
                    }
                }
            }

            cb.SetColorFill(BaseColor.BLACK);

            var title = new Paragraph(
                proj.GetProjectLabel() + ": " + proj.Name,
                FontFactory.GetFont("Arial", 16, Font.BOLD)).Hyphenate(hyph);
            title.SpacingBefore = 8f;
            title.SpacingAfter = 16f;
            title.SetLeading(0.0f, LINE_HEIGHT);
            document.Add(title);

            var projectTable = new PdfPTable(5) { SpacingAfter = 6f };
            projectTable.DefaultCell.Border = Rectangle.NO_BORDER;
            projectTable.HorizontalAlignment = Element.ALIGN_RIGHT;
            projectTable.WidthPercentage = 100f;
            projectTable.SetWidths(new float[] { 25, 50, 25, 25, 25 });

            //  Row 1
            projectTable.AddCell(new Paragraph(translator.GetHeadingAdvisor() + ":", fontHeading));
            if (proj.Advisor1 != null)
                projectTable.AddCell(new Anchor(proj.Advisor1.Name, fontRegularLink)
                {
                    Reference = "mailto:" + proj.Advisor1.Mail
                });
            else
                projectTable.AddCell(new Paragraph("?", fontRegular));

            projectTable.AddCell("");
            projectTable.AddCell(new Paragraph(translator.GetHeadingPriority()+" 1", fontHeading));
            projectTable.AddCell(new Paragraph(translator.GetHeadingPriority()+" 2", fontHeading));

            // Row 2
            if (proj.Advisor2 != null)
            {
                projectTable.AddCell("");
                projectTable.AddCell(new Anchor(proj.Advisor2.Name, fontRegularLink)
                {
                    Reference = "mailto:" + proj.Advisor2.Mail
                });
            }
            else if (proj.ClientCompany != "")
            {
                projectTable.AddCell(new Paragraph(translator.GetHeadingClient()+":", fontHeading));
                projectTable.AddCell(new Paragraph(proj.ClientCompany, fontRegular));
            }
            else
            {
                projectTable.AddCell("");
                projectTable.AddCell("");
            }

            projectTable.AddCell(new Paragraph(translator.GetHeadingWorkScope()+":", fontHeading));
            projectTable.AddCell(new Paragraph(proj.POneType.Description, fontRegular));
            projectTable.AddCell(new Paragraph(proj.PTwoType == null ? "---" : proj.PTwoType.Description, fontRegular));

            // Row 3
            if (proj.ClientCompany != "" && proj.Advisor2 != null)
            {
                projectTable.AddCell(new Paragraph(translator.GetHeadingClient()+":", fontHeading));
                projectTable.AddCell(new Paragraph(proj.ClientCompany, fontRegular));
            }
            else
            {
                projectTable.AddCell("");
                projectTable.AddCell("");
            }
            // Invoice Type
            projectTable.AddCell(new Paragraph("Rechnungsart:", fontHeading));
            projectTable.AddCell(new Paragraph(proj.InvoiceType ?? "Paper", fontRegular)); // Default to "Paper"
            projectTable.AddCell("");

            // Invoice Email (only if Invoice Type is "Email")
            if (proj.InvoiceType == "Email")
            {
                projectTable.AddCell(new Paragraph("Rechnungs-E-Mail:", fontHeading));
                projectTable.AddCell(new Paragraph(proj.InvoiceContact ?? "-", fontRegular));
            }
            else
            {
                projectTable.AddCell("");
                projectTable.AddCell("");
            }


            projectTable.AddCell(new Paragraph(translator.GetHeadingTeamSize()+":", fontHeading));
            projectTable.AddCell(new Paragraph(proj.POneTeamSize.Description, fontRegular));
            projectTable.AddCell(new Paragraph(proj.PTwoTeamSize == null ? "---" : proj.PTwoTeamSize.Description,
                fontRegular));

            // Row 4
            strLang = $"{translator.GetLanguageGerman()} {translator.GetOr()} {translator.GetLanguageEnglish()}";
            if(currentProject.LanguageEnglish && !currentProject.LanguageGerman)
            {
                strLang = translator.GetLanguageEnglish();
            }
            else if(currentProject.LanguageGerman && !currentProject.LanguageEnglish)
            {
                strLang = translator.GetLanguageGerman();
            }
            projectTable.AddCell(new Paragraph(translator.GetHeadingLangugages()+":", fontHeading));
            projectTable.AddCell(new Paragraph(strLang, fontRegular));
            projectTable.AddCell("");
            projectTable.AddCell("");
            projectTable.AddCell("");

            // Row 5
            var strStudyCourse = "";
            if (currentProject.SubmitToStudyCourseCS)
            {
                if (currentProject.SubmitToStudyCourseDS)
                    strStudyCourse = $"{translator.GetStudyCourseCS()} {translator.GetAnd()} {translator.GetStudyCourseDS()}";
                else
                    strStudyCourse = translator.GetStudyCourseCS();
            }
            else if (currentProject.SubmitToStudyCourseDS)
                strStudyCourse = translator.GetStudyCourseDS();
            projectTable.AddCell(new Paragraph(translator.GetStudyCourseTitle() + ":", fontHeading));
            projectTable.AddCell(new Paragraph(strStudyCourse, fontRegular));
            projectTable.AddCell("");
            projectTable.AddCell("");
            projectTable.AddCell("");

            // End header
            document.Add(projectTable);

            //
            // Body
            //

            //pdf with image
            if (proj.Picture != null)
            {
                try
                {
                    var img = Image.GetInstance(proj.Picture.ToArray());

                    //checks which layout should be used.
                    switch (layout)
                    {
                        case Layout.BigPictureInTheMiddle:
                            PictureInTheMiddle(proj, img, document, ImageSize.Big);
                            break;
                        case Layout.BigPictureRight:
                            PictureRightLayout(proj, img, document, ImageSize.Big, writer);
                            break;
                        case Layout.MediumPictureInTheMiddle:
                            PictureInTheMiddle(proj, img, document, ImageSize.Medium);
                            break;
                        case Layout.MediumPictureRight:
                            PictureRightLayout(proj, img, document, ImageSize.Medium, writer);
                            break;
                        case Layout.SmallPictureInTheMiddle:
                            PictureInTheMiddle(proj, img, document, ImageSize.Small);
                            break;
                        case Layout.SmallPictureRight:
                            PictureRightLayout(proj, img, document, ImageSize.Small, writer);
                            break;
                    }
                }
                catch (IOException)
                {
                    //could not parse the image...

                    //pdf without image
                    AddParagraph(proj.InitialPosition, document, translator.GetHeadingInitialPosition(), proj.InitialPosition);
                    AddParagraph(proj.Objective, document, translator.GetHeadingObjective(), proj.Objective);
                    AddParagraph(proj.ProblemStatement, document, translator.GetHeadingProblemStatement(), proj.ProblemStatement);
                    AddParagraph(proj.References, document, translator.GetHeadingTechnology(), proj.References);
                }
            }
            else
            {
                //pdf without image
                AddParagraph(proj.InitialPosition, document, translator.GetHeadingInitialPosition(), proj.InitialPosition);
                AddParagraph(proj.Objective, document, translator.GetHeadingObjective(), proj.Objective);
                AddParagraph(proj.ProblemStatement, document, translator.GetHeadingProblemStatement(), proj.ProblemStatement);
                AddParagraph(proj.References, document, translator.GetHeadingTechnology(), proj.References);
            }


            //
            // Footer
            //
            var strReservations = "";
            var strRemarks = "";
            //var strOneSem = "";
            if (proj.Remarks != "")
                strRemarks += proj.Remarks;

            if (proj.Reservation1Name != "")
            {
                strReservations += translator.GetReservedString(proj.Reservation1Name, proj.Reservation2Name);
            }

            /*if (proj.DurationOneSemester)
            {
                strOneSem = translator.GetHeadingOneSemester();
            }*/

            if (strReservations.Length > 0 || strRemarks.Length > 0 /* || strOneSem.Length > 0*/)
            {
                document.Add(new Paragraph(translator.GetHeadingAnnotation(), fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });

                if (strRemarks.Length > 0)
                    foreach (var text in strRemarks.ToLinkedParagraph(fontRegular, hyph))
                    {
                        text.SpacingAfter = 0f;
                        text.SetLeading(0.0f, LINE_HEIGHT);
                        text.Alignment = Element.ALIGN_JUSTIFIED;
                        document.Add(text);
                    }
                /*if (strOneSem.Length > 0)
                {
                    var oneSem = new Paragraph(strOneSem, fontRegular);
                    oneSem.SpacingAfter = 1f;
                    oneSem.SetLeading(0.0f, LINE_HEIGHT);
                    document.Add(oneSem);
                }*/
                if (strReservations.Length > 0)
                {
                    var fontRegularRed = new Font(fontRegular);
                    fontRegularRed.Color = BaseColor.RED;

                    var text = new Paragraph(strReservations, fontRegularRed);
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    document.Add(text);
                }
            }

            document.NewPage();
        }

        public int CalcNumberOfPages(Project PDF)
        {
            var minimumNumberOfPages = int.MaxValue;
            foreach (Layout l in Enum.GetValues(typeof(Layout)))
                minimumNumberOfPages = Math.Min(minimumNumberOfPages, CalcNumberOfPages(PDF, l));
            return minimumNumberOfPages;
        }

        private int CalcNumberOfPages(Project PDF, Layout layout)
        {
            using (var output = new MemoryStream())
            {
                using (var document = CreateDocument())
                {
                    AppendToPDF(output, Enumerable.Repeat(PDF, 1), layout, document);
                }
                using (var pdfReader = new PdfReader(output.ToArray()))
                {
                    return pdfReader.NumberOfPages;
                }
            }
        }

        private void PictureRightLayout(Project proj, Image img, Document document, ImageSize imgsize, PdfWriter writer)
        {
            // http://stackoverflow.com/questions/9272777/auto-scaling-of-images
            // image.ScaleAbsoluteWidth(160f);

            switch (imgsize)
            {
                case ImageSize.Big:
                    document.Add(DescribedImage(writer, img, proj.ImgDescription, 300, 300));
                    break;
                case ImageSize.Medium:
                    document.Add(DescribedImage(writer, img, proj.ImgDescription, 250, 250));
                    break;
                case ImageSize.Small:
                    document.Add(DescribedImage(writer, img, proj.ImgDescription, 200, 200));
                    break;
            }

            AddParagraph(proj.InitialPosition, document, translator.GetHeadingInitialPosition(), proj.InitialPosition);
            AddParagraph(proj.Objective, document, translator.GetHeadingObjective(), proj.Objective);
            AddParagraph(proj.ProblemStatement, document, translator.GetHeadingProblemStatement(), proj.ProblemStatement);
            AddParagraph(proj.References, document, translator.GetHeadingTechnology(), proj.References);
        }

        public Image DescribedImage(PdfWriter writer, Image img, string description, float heighttoscale,
            float widthtoscale)
        {
            img.ScaleToFit(heighttoscale, widthtoscale);
            var width = img.ScaledWidth;
            var height = img.ScaledHeight;

            //height used for one line of imgdescription
            var descriptionheight = 12f;

            //set up template
            var cb = writer.DirectContent;
            var template = cb.CreateTemplate(width + 10f, height + descriptionheight * 5);

            if (!string.IsNullOrEmpty(description))
            {
                var linkedPara = new Paragraph();
                linkedPara.AddRange(description.ToLinkedParagraph(fontsmall, hyph));

                //set up ct for description
                var ct = new ColumnText(template);
                var imgDescription = new Phrase(linkedPara);
                ct.SetSimpleColumn(imgDescription, 10f, 0, template.Width, descriptionheight * 5, 10,
                    Element.ALIGN_JUSTIFIED);
                ct.Go(true);

                //Get the lines used to write the comment
                var lineswriten = ct.LinesWritten;

                template.Height = height + lineswriten * descriptionheight;

                //add the comment to the template
                ct.SetSimpleColumn(imgDescription, 10f, 0, template.Width, descriptionheight * lineswriten, 10,
                    Element.ALIGN_JUSTIFIED);
                ct.Go(false);
            }
            else
            {
                template.Height = height + 5f;
            }
            //add img to template
            template.AddImage(img, width, 0, 0, height, template.Width - width, template.Height - height);
            var i = Image.GetInstance(template);

            i.Alignment = Element.ALIGN_RIGHT | Image.TEXTWRAP;
            return i;
        }


        private void AddParagraph(string test, Document document, string title, string content)
        {
            if (test != "")
            {
                document.Add(new Paragraph(title, fontHeading)
                {
                    SpacingBefore = SPACING_BEFORE_TITLE,
                    SpacingAfter = SPACING_AFTER_TITLE
                });


                foreach (var text in content.ToLinkedParagraph(fontRegular, hyph))
                {
                    text.SpacingAfter = 1f;
                    text.SetLeading(0.0f, LINE_HEIGHT);
                    text.Alignment = Element.ALIGN_JUSTIFIED;
                    document.Add(text);
                }
            }
        }

        private void PictureInTheMiddle(Project proj, Image img, Document document, ImageSize imgsize)
        {
            switch (imgsize)
            {
                case ImageSize.Big:
                    img.ScaleToFit(480f, 480f);
                    break;
                case ImageSize.Medium:
                    img.ScaleToFit(380f, 380f);
                    break;
                case ImageSize.Small:
                    img.ScaleToFit(290f, 290f);
                    break;
            }

            img.Alignment = Element.ALIGN_MIDDLE;
            img.SpacingBefore = SPACING_BEFORE_IMAGE;

            img.SpacingBefore = SPACING_BEFORE_TITLE;
            document.Add(img);

            if (proj.ImgDescription != "")
            {
                fontsmall.SetColor(100, 100, 100);

                var p = new Paragraph(proj.ImgDescription, fontsmall);
                p.PaddingTop = -1f;
                p.Alignment = Element.ALIGN_CENTER;

                document.Add(p);
            }
            AddParagraph(proj.InitialPosition, document, translator.GetHeadingInitialPosition(), proj.InitialPosition);
            AddParagraph(proj.Objective, document, translator.GetHeadingObjective(), proj.Objective);
            AddParagraph(proj.ProblemStatement, document, translator.GetHeadingProblemStatement(), proj.ProblemStatement);
            AddParagraph(proj.References, document, translator.GetHeadingTechnology(), proj.References);
        }

        public static Document CreateDocument()
        {
            var margin = Utilities.MillimetersToPoints(20f);
            return new Document(PageSize.A4, margin, margin, margin, margin);
        }

        private enum Layout
        {
            BigPictureInTheMiddle,
            MediumPictureInTheMiddle,
            BigPictureRight,
            SmallPictureInTheMiddle,
            MediumPictureRight,
            SmallPictureRight
        }

        private enum ImageSize
        {
            Big,
            Medium,
            Small
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

                translator.DetectLanguage(CurrentProject);

                // create two column table
                var foot = new PdfPTable(1);
                foot.TotalWidth = document.PageSize.Width + 2;
                foot.DefaultCell.Border = Rectangle.NO_BORDER;

                // add image; PdfPCell() overload sizes image to fit cell
                var cell = new PdfPCell(new Phrase(translator.GetHeadingFooter(CurrentProject,db),new Font(Font.FontFamily.HELVETICA, 8)));
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = document.TopMargin - 15;
                cell.PaddingLeft = 58;
                cell.PaddingTop = 8;
                cell.PaddingBottom = 0;
                cell.Border = Rectangle.NO_BORDER;
                foot.AddCell(cell);
                foot.WriteSelectedRows(0, -1, -1, 40, writer.DirectContent);
            }
        }
    }
}
