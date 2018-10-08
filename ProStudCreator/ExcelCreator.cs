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
        private static readonly string[] HEADERS =
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

        private static readonly string[] MARKETING_HEADERS =
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

        private static readonly string[] Billing_HEADER =
        {
            "Semester",
            "Projekt-ID",
            "projekttittel*",
            "Studierende*",
            "Betreuer*",
            "Projekt x*",
            "Institut",
            "Vertiefung",
            "vertrag",
            "Experte (P6)",
            "Auftraggeber*",
            "Verrechnung*",
            "",
            "",
        };


        // References
        // - http://poi.apache.org/spreadsheet/quick-guide.html#NewWorkbook

        public static void GenerateProjectList(Stream outStream, IEnumerable<Project> _projects)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(SHEET_NAME);

            // Header
            worksheet.CreateRow(0);
            for (var i = 0; i < HEADERS.Length; i++)
                worksheet.GetRow(0).CreateCell(i).SetCellValue(HEADERS[i]);

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                InsertProjectAsExcelRow(projects[i], row);
            }

            for (var i = 0; i < HEADERS.Length; i++)
                worksheet.AutoSizeColumn(i);

            // Save
            workbook.Write(outStream);
        }

        private static void InsertProjectAsExcelRow(Project p, IRow row)
        {
            var db = new ProStudentCreatorDBDataContext();
            p.Semester = p.Semester == null ? Semester.NextSemester(db) : p.Semester = p.Semester;

            var abbreviation = /*Semester.CurrentSemester.ToString() +*/
                p.Semester + "_" + p.Department.DepartmentName + p.ProjectNr.ToString("D2");
            var dispName = abbreviation + "_" + p.Name;

            row.CreateCell(0).SetCellValue(abbreviation);
            row.CreateCell(1).SetCellValue(p.Name);
            row.CreateCell(2).SetCellValue(dispName);
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
            for (var i = 0; i < MARKETING_HEADERS.Length; i++)
            {
                var cell = worksheet.GetRow(0).CreateCell(i);
                cell.CellStyle = HeaderStyle;
                cell.SetCellValue(MARKETING_HEADERS[i]);
            }

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(1 + i);
                ProjectToExcelMarketingRow(projects[i], row, db, DateStyle);
            }

            for (var i = 0; i < HEADERS.Length; i++)
                worksheet.AutoSizeColumn(i);

            // Save
            workbook.Write(outStream);
        }


        private static void ProjectToExcelMarketingRow(Project p, IRow row, ProStudentCreatorDBDataContext db,
            ICellStyle DateStyle)
        {
            var abbreviation = /*Semester.CurrentSemester.ToString() +*/ p.Semester + "_" +
                                                                         p.Department.DepartmentName +
                                                                         p.ProjectNr.ToString("D2");
            var clientDepartment = string.IsNullOrEmpty(p.ClientAddressDepartment) ||
                                   string.IsNullOrEmpty(p.ClientCompany)
                ? ""
                : p.ClientCompany + " Abt:" + p.ClientAddressDepartment;

            var i = 0;
            row.CreateCell(i++).SetCellValue(abbreviation);
            row.CreateCell(i++).SetCellValue(p.Department.DepartmentName);
            row.CreateCell(i++).SetCellValue(p.Name);
            var cell1 = row.CreateCell(i++);
            cell1.CellStyle = DateStyle;
            cell1.SetCellValue(GetStartDate(p, db));
            var cell2 = row.CreateCell(i++);
            cell2.CellStyle = DateStyle;
            if (p.GetDeliveryDate().HasValue)
                cell2.SetCellValue(p.GetDeliveryDate().Value);
            row.CreateCell(i++).SetCellValue(p.ExhibitionBachelorThesis(db));
            row.CreateCell(i++).SetCellValue(p.LogStudent1Name ?? "");
            row.CreateCell(i++).SetCellValue(p.LogStudent1Mail ?? "");
            var cell3 = row.CreateCell(i++);
            if (GetStudentGrade(p.LogGradeStudent1) == -1)
            {
                cell3.SetCellType(CellType.String);
                cell3.SetCellValue("");
            }
            else
            {
                cell3.SetCellValue(GetStudentGrade(p.LogGradeStudent1));
            }
            row.CreateCell(i++).SetCellValue(p.LogStudent2Name ?? "");
            row.CreateCell(i++).SetCellValue(p.LogStudent2Mail ?? "");
            var cell4 = row.CreateCell(i++);
            if (GetStudentGrade(p.LogGradeStudent2) == -1)
            {
                cell4.SetCellType(CellType.String);
                cell4.SetCellValue("");
            }
            else
            {
                cell4.SetCellValue(GetStudentGrade(p.LogGradeStudent2));
            }
            row.CreateCell(i++).SetCellValue(string.IsNullOrEmpty(p.Reservation1Mail) ? "Nein" : "Ja");
            row.CreateCell(i++).SetCellValue(p.Advisor1?.Name ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor1?.Mail ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor2?.Name ?? "");
            row.CreateCell(i++).SetCellValue(p.Advisor2?.Mail ?? "");
            row.CreateCell(i++).SetCellValue(GetAbbreviationProject(p));
            row.CreateCell(i++).SetCellValue(p.LogProjectType?.ExportValue ?? "-");
            row.CreateCell(i++).SetCellValue(GetProjectDuration(p));
            row.CreateCell(i++).SetCellValue(GetLanguage(p));
            row.CreateCell(i++).SetCellValue(p.Expert?.Mail ?? "");
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

        private static string GetAbbreviationProject(Project p)
        {
            if (p.PreviousProject == null)
                return "";
            return p.PreviousProject.Semester + "_" + p.PreviousProject.Department.DepartmentName +
                   p.PreviousProject.ProjectNr.ToString("D2");
        }

        private static DateTime GetStartDate(Project p, ProStudentCreatorDBDataContext db)
        {
            return p.Semester?.StartDate ?? Semester.NextSemester(db).StartDate.Date;
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
        public static void GenerateBillingList(Stream outStream, IEnumerable<Project> _projects,
             ProStudentCreatorDBDataContext db, string semesterName)
        {
            var workbook = new XSSFWorkbook();
            var worksheet = workbook.CreateSheet(Billing_SHEET_NAME);

            var HeaderStyle = workbook.CreateCellStyle();
            HeaderStyle.FillForegroundColor = HSSFColor.PaleBlue.Index;
            HeaderStyle.FillPattern = FillPattern.SolidForeground;

            var DateStyle = workbook.CreateCellStyle();
            DateStyle.DataFormat = workbook.CreateDataFormat().GetFormat("dd.MM.yyyy");


            // Header

            worksheet.CreateRow(0);
            for (var i = 0; i < Billing_HEADER.Length; i++)
            {
                var cell = worksheet.GetRow(0).CreateCell(i);
                cell.CellStyle = HeaderStyle;
                cell.SetCellValue(Billing_HEADER[i]);
                if (i < 11)
                {                                     
                    NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(0, 2, i, i);
                    cell.CellStyle.WrapText = true;
                    worksheet.AddMergedRegion(cra);
                }               
            }

            // Project entries
            var projects = _projects.ToArray();

            for (var i = 0; i < projects.Length; i++)
            {
                var row = worksheet.CreateRow(3 + i);
                ProjectToExcelBillingRow(projects[i], row, db, DateStyle, worksheet, workbook);
            }

            for (var i = 0; i < Billing_HEADER.Length; i++)
                worksheet.AutoSizeColumn(i, true);

            var CellStyleGreen = workbook.CreateCellStyle();
            CellStyleGreen.FillForegroundColor = HSSFColor.BrightGreen.Index;
            CellStyleGreen.FillPattern = FillPattern.SolidForeground;

            var CellStyleRed = workbook.CreateCellStyle();
            CellStyleRed.FillForegroundColor = HSSFColor.Red.Index;
            CellStyleRed.FillPattern = FillPattern.SolidForeground;

            //j = 11 because until the 11 column the Headers look the same 
            //thats why it has to start filling in with the 11th column 
            var j = 11;
            var SecondHeaders = worksheet.CreateRow(1);
            var SecondHeadersCells = worksheet.GetRow(1).CreateCell(j);

            //Second Line
            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j++);
            SecondHeadersCells.CellStyle = CellStyleGreen;
            SecondHeadersCells.SetCellValue("ja");

            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j++);
            SecondHeadersCells.CellStyle = CellStyleGreen;
            SecondHeadersCells.SetCellValue("               ");

            SecondHeadersCells = worksheet.GetRow(1).CreateCell(j++);
            SecondHeadersCells.CellStyle = CellStyleRed;
            SecondHeadersCells.SetCellValue("Nein");


            //Third line 
            //j = 11 because until the 11 column the Headers look the same 
            //thats why it has to start filling in with the 11th column 
            SecondHeaders = worksheet.CreateRow(2);
            j = 11;

            SecondHeadersCells = worksheet.GetRow(2).CreateCell(j++);
            SecondHeadersCells.CellStyle = CellStyleGreen;
            SecondHeadersCells.SetCellValue("Kontaktperson");

            SecondHeadersCells = worksheet.GetRow(2).CreateCell(j++);
            SecondHeadersCells.CellStyle = CellStyleGreen;
            SecondHeadersCells.SetCellValue("Rechnungsadresse");

            SecondHeadersCells = worksheet.GetRow(2).CreateCell(j++);
            SecondHeadersCells.CellStyle = CellStyleRed;
            SecondHeadersCells.SetCellValue("Verrechenbar");
      
            // Save

            workbook.Write(outStream);
        }

        private static void ProjectToExcelBillingRow(Project p, IRow row, ProStudentCreatorDBDataContext db,
            ICellStyle DateStyle, ISheet worksheet, IWorkbook workbook)
        {
            var abbreviation = /*Semester.CurrentSemester.ToString() +*/ p.Semester + "_" +
            p.Department.DepartmentName +
            p.ProjectNr.ToString("D2"); var i = 0;

            var students = p.LogStudent1Name + " / " + p.LogStudent2Name;

            var adress = p.ClientAddressStreet +
                               p.ClientAddressPostcode + p.ClientAddressCity;

            var ColeredRows = row;

            var ColeredRowCells = ColeredRows.CreateCell(i);

            var CellStyleGreen = workbook.CreateCellStyle();
            CellStyleGreen.FillForegroundColor = HSSFColor.Green.Index;
            CellStyleGreen.FillPattern = FillPattern.SolidForeground;

            var CellStyleRed = workbook.CreateCellStyle();
            CellStyleRed.FillForegroundColor = HSSFColor.Red.Index;
            CellStyleRed.FillPattern = FillPattern.SolidForeground;


            row.CreateCell(i++).SetCellValue(p.Semester.Name);
            row.CreateCell(i++).SetCellValue(abbreviation);
            row.CreateCell(i++).SetCellValue(p.Name);
            row.CreateCell(i++).SetCellValue(students);
            row.CreateCell(i++).SetCellValue(p.Advisor1?.Name ?? "");
            row.CreateCell(i++).SetCellValue(p.POneType.ExportValue);
            row.CreateCell(i++).SetCellValue(p.Department.DepartmentName);
            row.CreateCell(i++).SetCellValue("");
            row.CreateCell(i++).SetCellValue("");
            row.CreateCell(i++).SetCellValue(p.Advisor1?.Mail ?? "");

            // Generates the special collerd lines needed
           var k = (p.BillingStatus.Billable) ? (CellStyleGreen):(CellStyleRed);
            
            ColeredRowCells = row.CreateCell(i++);
            ColeredRowCells.CellStyle = k;
            ColeredRowCells.SetCellValue(p.ClientCompany);

            ColeredRowCells = row.CreateCell(i++);
            ColeredRowCells.CellStyle = k;
            ColeredRowCells.SetCellValue(p.ClientPerson);

            ColeredRowCells = row.CreateCell(i++);
            ColeredRowCells.CellStyle = k;
            ColeredRowCells.SetCellValue(adress);

            ColeredRowCells = row.CreateCell(i++);
            ColeredRowCells.CellStyle = k;
            ColeredRowCells.SetCellValue(p.BillingStatus?.DisplayName ?? "");
        }
    }       
}
