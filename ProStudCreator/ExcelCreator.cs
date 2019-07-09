using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ProStudCreator
{
    public class ExcelCreator
    {
        private static readonly string SHEET_NAME = "Projects";
        private static readonly string MARKETING_SHEET_NAME = "_IP56_Informatikprojekte";
        private static readonly string Billing_SHEET_NAME = "_Verrechnungs_Excel";
        private static readonly string MARKOM_SHEET_NAME = "_IP6_Ausstellung";

        private static readonly string[] ProjectListHeader =
        {
            "Abbreviation",
            "Name",
            "Display Name",
            "1P_Type",
            "2P_Type",
            "1P_Teamsize",
            "2P_Teamsize",
            "Betreuer",
            "Betreuer2",
            "Fixe Zuteilung",
            "Fixe Zuteilung 2",
            "ID",
            "Continuation",
            "German",
            "English",
            "TypeAppWeb",
            "TypeCGIP",
            "TypeDBBigData",
            "TypeDesignUX",
            "TypeHW",
            "TypeMlAlg",
            "TypeSE",
            "TypeSysSec"
        };

        private static readonly string[] AdminHeader =
        {
            "Projektnummer",
            "Institut",
            "Projekttitel",
            "Projektstart",
            "Projektabgabe",
            "Ausstellung Bachelorthesis",
            "Student/in 1",
            "Student/in 1 E-Mail",
            "Note Student/in 1",
            "Student/in 2",
            "Student/in 2 E-Mail",
            "Note Student/in 2",
            "Wurde reserviert",
            "Hauptbetreuende/r",
            "Hauptbetreuende/r E-Mail",
            "Nebenbetreuende/r",
            "Nebenbetreuende/r E-Mail",
            "Weiterführung von",
            "Projekttyp",
            "Anzahl Semester",
            "Durchführungssprache",
            "Experte",
            "Experte Bezahlt",
            "Automatische Auszahlung",
            "Verteidigung-Datum",
            "Verteidigung-Raum",
            "Verrechungsstatus",
            "Kunden-Unternehmen",
            "Kunden-Anrede",
            "Kunden-Name",
            "Kunden-E-Mail Adresse",
            "Kunden-Abteilung",
            "Kunden-Strasse und Nummer",
            "Kunden-PLZ",
            "Kunden-Ort",
            "Kunden-Referenznummer",
            "Kunden-Adresse",
            "Interne DB-ID"
        };

        private static readonly string[] BillingHeader =
        {
            "Semester",
            "Projekt-nummer",
            "Projekttittel",
            "Studierende",
            "Betreuer",
            "Projekt x",
            "Institut",
            "Vertiefungs-richtung",
            "Vertraulich",
            "Experte (P6)",
            "Auftraggeber",
            "Verrechnung",
            "",
            "",
            "Mitglied AIHK",
            "bezahlt"
        };

        private static readonly string[] MarKomHeader =
        {
            "Platz",
            "Projekttitel",
            "Stud 1 Name",
            "Stud 2 Name",
            "Firma Name",
            "Firma Ort"
        };

        // Reference http://poi.apache.org/spreadsheet/quick-guide.html#NewWorkbook
        public static void GenerateProjectList(Stream outStream, IEnumerable<Project> _projects)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (var i = 0; i < ProjectListHeader.Length; i++)
                worksheet.GetRow(0).CreateCell(i).SetCellValue(ProjectListHeader[i]);

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
                InsertProjectAsExcelRow(projects[i], worksheet.CreateRow(1 + i));

            for (var i = 0; i < ProjectListHeader.Length; i++)
                worksheet.AutoSizeColumn(i);

            worksheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, ProjectListHeader.Length - 1));

            // Save
            workbook.Write(outStream);
        }

        private static void InsertProjectAsExcelRow(Project p, IRow row)
        {
            var db = new ProStudentCreatorDBDataContext();
            p.Semester = p.Semester == null ? Semester.NextSemester(db) : p.Semester = p.Semester;

            row.CreateCell(0).SetCellValue(p.GetFullNr());
            row.CreateCell(1).SetCellValue(p.Name);
            row.CreateCell(2).SetCellValue(p.GetFullTitle());
            row.CreateCell(3).SetCellValue(p.POneType.ExportValue);
            row.CreateCell(4).SetCellValue(p.PTwoType != null ? p.PTwoType.ExportValue : p.POneType.ExportValue);
            row.CreateCell(5).SetCellValue(p.POneTeamSize.ExportValue);
            row.CreateCell(6).SetCellValue(p.PTwoTeamSize != null ? p.PTwoTeamSize.ExportValue : p.POneTeamSize.ExportValue);
            row.CreateCell(7).SetCellValue(p.Advisor1?.Mail ?? "");
            row.CreateCell(8).SetCellValue(p.Advisor2?.Mail ?? "");
            row.CreateCell(9).SetCellValue(p.Reservation1Mail);
            row.CreateCell(10).SetCellValue(p.Reservation2Mail);
            row.CreateCell(11).SetCellValue(p.Id);

            row.CreateCell(12).SetCellValue(p.PreviousProject != null ? 1 : 0);
            row.CreateCell(13).SetCellValue(p.LanguageGerman ? 1 : 0);
            row.CreateCell(14).SetCellValue(p.LanguageEnglish ? 1 : 0);
            row.CreateCell(15).SetCellValue(p.TypeAppWeb);
            row.CreateCell(16).SetCellValue(p.TypeCGIP);
            row.CreateCell(17).SetCellValue(p.TypeDBBigData);
            row.CreateCell(18).SetCellValue(p.TypeDesignUX);
            row.CreateCell(19).SetCellValue(p.TypeHW);
            row.CreateCell(20).SetCellValue(p.TypeMlAlg);
            row.CreateCell(21).SetCellValue(p.TypeSE);
            row.CreateCell(22).SetCellValue(p.TypeSysSec);
        }


        public static void GenerateMarketingList(Stream outStream, IEnumerable<Project> _projects,
            ProStudentCreatorDBDataContext db, string semesterName)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(semesterName + MARKETING_SHEET_NAME);

            var HeaderStyle = workbook.CreateCellStyle();
            HeaderStyle.BorderBottom = BorderStyle.Thick;
            HeaderStyle.FillForegroundColor = HSSFColor.PaleBlue.Index;
            HeaderStyle.FillPattern = FillPattern.SolidForeground;

            var DateStyle = workbook.CreateCellStyle();
            DateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd.MM.yyyy");

            // Header
            worksheet.CreateRow(0);
            for (var i = 0; i < AdminHeader.Length; i++)
            {
                var cell = worksheet.GetRow(0).CreateCell(i);
                cell.CellStyle = HeaderStyle;
                cell.SetCellValue(AdminHeader[i]);
            }

            // Project entries
            var projects = _projects.ToArray();
            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                ProjectToExcelMarketingRow(projects[i], row, db, DateStyle);
            }

            for (var i = 0; i < ProjectListHeader.Length; i++)
                worksheet.AutoSizeColumn(i);

            worksheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, AdminHeader.Length - 1));

            // Save
            workbook.Write(outStream);
        }

        private static void ProjectToExcelMarketingRow(Project p, IRow row, ProStudentCreatorDBDataContext db,
            ICellStyle DateStyle)
        {
            var sName1 = p.LogStudent1Name ?? "";
            var sMail1 = p.LogStudent1Mail ?? "";
            var sGrad1 = p.LogGradeStudent1;

            string sName2 = null;
            string sMail2 = null;
            float? sGrad2 = null;

            if (!string.IsNullOrWhiteSpace(p.LogStudent2Mail))
            {
                if (string.Compare(sMail1.Split('@')[0].Split('.')[1], p.LogStudent2Mail.Split('@')[0].Split('.')[1]) == 1)
                {
                    sName2 = sName1;
                    sMail2 = sMail1;
                    sGrad2 = sGrad1;

                    sName1 = p.LogStudent2Name ?? "";
                    sMail1 = p.LogStudent2Mail ?? "";
                    sGrad1 = p.LogGradeStudent2;
                }
                else
                {
                    sName2 = p.LogStudent2Name ?? "";
                    sMail2 = p.LogStudent2Mail ?? "";
                    sGrad2 = p.LogGradeStudent2;
                }
            }


            var clientDepartment = string.IsNullOrEmpty(p.ClientAddressDepartment) ||
                                   string.IsNullOrEmpty(p.ClientCompany)
                ? "" : p.ClientCompany + " Abt:" + p.ClientAddressDepartment;

            var i = 0;
            row.CreateCell(i++).SetCellValue(p.GetFullNr());
            row.CreateCell(i++).SetCellValue(p.Department.DepartmentName);
            row.CreateCell(i++).SetCellValue(p.Name);
            var cell1 = row.CreateCell(i++);
            cell1.CellStyle = DateStyle;
            cell1.SetCellValue(p.Semester?.StartDate ?? Semester.NextSemester(db).StartDate.Date);
            var cell2 = row.CreateCell(i++);
            cell2.CellStyle = DateStyle;
            if (p.GetDeliveryDate().HasValue)
                cell2.SetCellValue(p.GetDeliveryDate().Value);
            row.CreateCell(i++).SetCellValue(p.ExhibitionBachelorThesis(db));
            row.CreateCell(i++).SetCellValue(sName1);
            row.CreateCell(i++).SetCellValue(sMail1);
            var cell3 = row.CreateCell(i++);
            if (GetStudentGrade(sGrad1) == -1)
            {
                cell3.SetCellType(CellType.String);
                cell3.SetCellValue("");
            }
            else
            {
                cell3.SetCellValue(GetStudentGrade(sGrad1));
            }
            row.CreateCell(i++).SetCellValue(sName2);
            row.CreateCell(i++).SetCellValue(sMail2);
            var cell4 = row.CreateCell(i++);
            if (GetStudentGrade(sGrad2) == -1)
            {
                cell4.SetCellType(CellType.String);
                cell4.SetCellValue("");
            }
            else
            {
                cell4.SetCellValue(GetStudentGrade(sGrad2));
            }
            row.CreateCell(i++).SetCellValue(string.IsNullOrEmpty(p.Reservation1Mail) ? "Nein" : "Ja");
            row.CreateCell(i++).SetCellValue(p.Advisor1?.Name ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor1?.Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor2?.Name ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor2?.Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.GetFullNr());
            row.CreateCell(i++).SetCellValue(p.LogProjectType?.ExportValue ?? "-");
            row.CreateCell(i++).SetCellValue(GetProjectDuration(p));
            row.CreateCell(i++).SetCellValue(GetLanguage(p));
            row.CreateCell(i++).SetCellValue(p.Expert?.Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.LogExpertPaid.ToString() ?? "");
            row.CreateCell(i++).SetCellValue(p.Expert?.AutomaticPayout.ToString() ?? "");
            row.CreateCell(i++).SetCellValue(p.LogDefenceDate?.ToString() ?? "-");
            row.CreateCell(i++).SetCellValue(p.LogDefenceRoom ?? "-");
            row.CreateCell(i++).SetCellValue(p.BillingStatus?.DisplayName ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientCompany ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressTitle ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientPerson ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientMail ?? "");
            row.CreateCell(i++).SetCellValue(clientDepartment);
            row.CreateCell(i++).SetCellValue(p.ClientAddressStreet ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressPostcode ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientAddressCity ?? "");
            row.CreateCell(i++).SetCellValue(p.ClientReferenceNumber ?? "");
            row.CreateCell(i++).SetCellValue(GetClientAddress(p));
            row.CreateCell(i++).SetCellValue(p.Id);            
        }
         
        private static string GetLanguage(Project p)
        {
            if ((p.LogLanguageGerman ?? false) && !(p.LogLanguageEnglish ?? false))
                return "Deutsch";

            if (!(p.LogLanguageGerman ?? false) && (p.LogLanguageEnglish ?? false))
                return "Englisch";

            return "";
        }

        private static string GetProjectDuration(Project p)
        {
            if (p.LogProjectDuration == null)
                return "";
            else if (p.LogProjectDuration == 1)
                return "Normal";
            else if (p.LogProjectDuration == 2)
                return "Lang";
            else
                throw new Exception($"Unexpected LogProjectDuration: {p.LogProjectDuration}");
        }

        private static double GetStudentGrade(float? grade)
        {
            if (grade == null)
                return -1;
            return Math.Round((double)grade, 1);
        }

        private static string GetClientAddress(Project p)
        {
            var address = new StringBuilder();
            address.AppendLine(p.ClientCompany ?? "");
            if (!string.IsNullOrEmpty(p.ClientAddressDepartment))
                address.AppendLine("Abt:" + p.ClientAddressDepartment);
            address.Append(p.ClientAddressTitle ?? "");
            address.Append(" ");
            address.AppendLine(p.ClientPerson ?? "");
            address.AppendLine(p.ClientAddressStreet ?? "");
            address.Append(p.ClientAddressPostcode ?? "");
            address.Append(" ");
            address.AppendLine(p.ClientAddressCity ?? "");

            return address.ToString();
        }

        public static void GenerateBillingList(Stream outStream, IEnumerable<Project> _projects, ProStudentCreatorDBDataContext db, string semesterName)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(Billing_SHEET_NAME);
            
            var cellStyleGreen = workbook.CreateCellStyle();
            cellStyleGreen.FillForegroundColor = HSSFColor.LightGreen.Index;
            cellStyleGreen.BorderBottom = BorderStyle.Thin;
            cellStyleGreen.BorderTop = BorderStyle.Thin;
            cellStyleGreen.BorderRight = BorderStyle.Thin;
            cellStyleGreen.BorderLeft = BorderStyle.Thin;
            cellStyleGreen.FillPattern = FillPattern.SolidForeground;

            var cellStyleGreenThick = workbook.CreateCellStyle();
            cellStyleGreenThick.FillForegroundColor = HSSFColor.LightGreen.Index;
            cellStyleGreenThick.BorderBottom = BorderStyle.Thin;
            cellStyleGreenThick.BorderTop = BorderStyle.Thick;
            cellStyleGreenThick.BorderRight = BorderStyle.Thin;
            cellStyleGreenThick.BorderLeft = BorderStyle.Thin;
            cellStyleGreenThick.FillPattern = FillPattern.SolidForeground;

            var cellStyleRed = workbook.CreateCellStyle();
            cellStyleRed.FillForegroundColor = HSSFColor.Rose.Index;
            cellStyleRed.BorderBottom = BorderStyle.Thin;
            cellStyleRed.BorderTop = BorderStyle.Thin;
            cellStyleRed.BorderLeft = BorderStyle.Thin;
            cellStyleRed.BorderRight = BorderStyle.Thin;
            cellStyleRed.FillPattern = FillPattern.SolidForeground;

            var cellStyleRedThick = workbook.CreateCellStyle();
            cellStyleRedThick.FillForegroundColor = HSSFColor.Rose.Index;
            cellStyleRedThick.BorderBottom = BorderStyle.Thin;
            cellStyleRedThick.BorderTop = BorderStyle.Thick;
            cellStyleRedThick.BorderLeft = BorderStyle.Thin;
            cellStyleRedThick.BorderRight = BorderStyle.Thin;
            cellStyleRedThick.FillPattern = FillPattern.SolidForeground;

            var cellStyleBorder = workbook.CreateCellStyle();
            cellStyleBorder.BorderBottom = BorderStyle.Thin;
            cellStyleBorder.BorderTop = BorderStyle.Thin;
            cellStyleBorder.BorderLeft = BorderStyle.Thin;
            cellStyleBorder.BorderRight = BorderStyle.Thin;

            var cellStyleBorderThickTop = workbook.CreateCellStyle();
            cellStyleBorderThickTop.BorderBottom = BorderStyle.Thin;
            cellStyleBorderThickTop.BorderTop = BorderStyle.Thick;
            cellStyleBorderThickTop.BorderLeft = BorderStyle.Thin;
            cellStyleBorderThickTop.BorderRight = BorderStyle.Thin;

            var cellStyleHeader = workbook.CreateCellStyle();
            cellStyleHeader.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            cellStyleHeader.FillPattern = FillPattern.SolidForeground;

            var cellStyleHeaderYellow = workbook.CreateCellStyle();
            cellStyleHeaderYellow.FillForegroundColor = HSSFColor.Yellow.Index;
            cellStyleHeaderYellow.FillPattern = FillPattern.SolidForeground;

            var DateStyle = workbook.CreateCellStyle();
            DateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd.MM.yyyy");

            //add header
            worksheet.CreateRow(0);
            for (var i = 0; i < BillingHeader.Length; i++)
            {
                var cell = worksheet.GetRow(0).CreateCell(i);
                cell.CellStyle = i < 14 ? cellStyleHeader : cellStyleHeaderYellow;
                cell.SetCellValue(BillingHeader[i]);
                if (i < 11 || i > 13)
                {
                    cell.CellStyle.WrapText = true;
                    worksheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, i, i));
                }
            }

            // Project entries
            var projects = _projects.ToArray();
            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(3 + i);
                var p = projects[i];

                row.CreateCell(0).SetCellValue(p.Semester.Name);
                row.CreateCell(1).SetCellValue(p.GetFullNr());
                row.CreateCell(2).SetCellValue(p.Name);
                if(p.LogStudent2Name!=null)
                    row.CreateCell(3).SetCellValue(p.LogStudent1Name + " / " + p.LogStudent2Name);
                else
                    row.CreateCell(3).SetCellValue(p.LogStudent1Name);
                row.CreateCell(4).SetCellValue(p.Advisor1?.Name ?? "");
                row.CreateCell(5).SetCellValue(p.LogProjectType?.ExportValue ?? "");
                row.CreateCell(6).SetCellValue(p.Department.DepartmentName);
                row.CreateCell(7).SetCellValue("");
                row.CreateCell(8).SetCellValue("");
                row.CreateCell(9).SetCellValue(p.Expert?.Mail ?? "");
                row.CreateCell(10).SetCellValue(p.ClientCompany);
                row.CreateCell(11).SetCellValue(p.ClientPerson);

                if(!string.IsNullOrEmpty(p.ClientAddressStreet) && (!string.IsNullOrEmpty(p.ClientAddressPostcode) || !string.IsNullOrEmpty(p.ClientAddressCity)))
                    row.CreateCell(12).SetCellValue($"{p.ClientAddressStreet}, {p.ClientAddressPostcode} {p.ClientAddressCity}");
                else
                    row.CreateCell(12).SetCellValue($"{p.ClientAddressStreet}{p.ClientAddressPostcode} {p.ClientAddressCity}");

                row.CreateCell(13).SetCellValue(p.BillingStatus?.DisplayName ?? "");
                row.CreateCell(14);
                row.CreateCell(15);


                //add border to the first few columns
                for (var cellcount = 0; cellcount < 11; cellcount++)
                    row.GetCell(cellcount).CellStyle = (row.RowNum == 3) ? cellStyleBorderThickTop : cellStyleBorder;
                for (var cellcount = 14; cellcount < 16; cellcount++)
                    row.GetCell(cellcount).CellStyle = (row.RowNum == 3) ? cellStyleBorderThickTop : cellStyleBorder;

                ICellStyle cellStyle;
                if (p.BillingStatus != null)
                {
                    //color the later columns, according to 
                    cellStyle = p.BillingStatus.Billable ? cellStyleGreen : cellStyleRed;
                    if (row.RowNum == 3) //add thick border for row 3, after the header
                        cellStyle = p.BillingStatus.Billable ? cellStyleGreenThick : cellStyleRedThick;
                }
                else
                    cellStyle = (row.RowNum == 3) ? cellStyleBorderThickTop : cellStyleBorder;

                for (var cellcount = 11; cellcount < 14; cellcount++)
                    row.GetCell(cellcount).CellStyle = cellStyle;
            }

            //j = 11 because until the 11 column the Headers look the same 
            //thats why it has to start filling in with the 11th column 
            var j = 11;
            var SecondHeaders = worksheet.CreateRow(1);
            var SecondHeadersCells = worksheet.GetRow(1).CreateCell(j);

            //Second Line
            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j++);
            SecondHeadersCells.CellStyle = cellStyleGreen;
            SecondHeadersCells.SetCellValue("ja");

            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j++);
            SecondHeadersCells.CellStyle = cellStyleGreen;
            SecondHeadersCells.SetCellValue("               ");

            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j++);
            SecondHeadersCells.CellStyle = cellStyleRed;
            SecondHeadersCells.SetCellValue("Nein");

            SecondHeaders = worksheet.CreateRow(2);
            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j);

            //Third line 
            //j = 11 because until the 11 column the Headers look the same 
            //thats why it has to start filling in with the 11th column 
            j = 11;
            SecondHeadersCells = worksheet.GetRow(2).CreateCell(j++);
            SecondHeadersCells.CellStyle = cellStyleGreen;
            SecondHeadersCells.SetCellValue("Kontaktperson");

            SecondHeadersCells = worksheet.GetRow(2).CreateCell(j++);
            SecondHeadersCells.CellStyle = cellStyleGreen;
            SecondHeadersCells.SetCellValue("Rechnungsadresse");

            SecondHeadersCells = worksheet.GetRow(2).CreateCell(j++);
            SecondHeadersCells.CellStyle = cellStyleRed;
            SecondHeadersCells.SetCellValue("Verrechenbar");

            for (var i = 0; i < BillingHeader.Length; i++)
                worksheet.AutoSizeColumn(i, true);

            workbook.Write(outStream);
        }

        public static void GenerateMarKomExcel(Stream outStream, IEnumerable<Project> _projects, ProStudentCreatorDBDataContext db, string semesterName)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(semesterName + MARKOM_SHEET_NAME);

            var HeaderStyle = workbook.CreateCellStyle();
            HeaderStyle.BorderBottom = BorderStyle.Thick;
            HeaderStyle.FillForegroundColor = HSSFColor.PaleBlue.Index;
            HeaderStyle.FillPattern = FillPattern.SolidForeground;

            var DateStyle = workbook.CreateCellStyle();
            DateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd.MM.yyyy");

            // Header
            worksheet.CreateRow(0);
            for (var i = 0; i < MarKomHeader.Length; i++)
            {
                var cell = worksheet.GetRow(0).CreateCell(i);
                cell.CellStyle = HeaderStyle;
                cell.SetCellValue(MarKomHeader[i]);
            }

            // Project entries
            var projects = _projects.ToArray();
            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                ProjectToMarKomRow(projects[i], row);
            }

            for (var i = 0; i < MarKomHeader.Length; i++)
                worksheet.AutoSizeColumn(i);

            worksheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, MarKomHeader.Length - 1));

            workbook.Write(outStream);
        }

        private static void ProjectToMarKomRow(Project p, IRow row)
        {
            var i = 0;
            row.CreateCell(i++);
            row.CreateCell(i++).SetCellValue(p.Name);
            row.CreateCell(i++).SetCellValue(p.LogStudent1Name);
            row.CreateCell(i++).SetCellValue(p.LogStudent2Name);
            row.CreateCell(i++).SetCellValue(p.ClientCompany);
            row.CreateCell(i++).SetCellValue(p.ClientAddressCity);
        }
    }
}
