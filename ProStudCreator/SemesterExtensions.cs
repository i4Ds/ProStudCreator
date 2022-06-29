using System;
using System.Globalization;
using System.Linq;
using NPOI.Util;

namespace ProStudCreator
{
    public partial class Semester
    {
        public static Semester CurrentSemester(ProStudentCreatorDBDataContext db)
        {
            return ActiveSemester(DateTime.Now, db);
        }

        public static Semester NextSemester(ProStudentCreatorDBDataContext db)
        {
            var safeDayBeforeNextSemester = CurrentSemester(db).DayBeforeNextSemester ?? default(DateTime);
            var safeDate = safeDayBeforeNextSemester.AddDays(7);
            return ActiveSemester(safeDate, db);
        }

        public static Semester AfterNextSemester(ProStudentCreatorDBDataContext db)
        {
            var nextSemester = NextSemester(db);
            var safeDayBeforeNextSemester = nextSemester.DayBeforeNextSemester ?? default(DateTime);
            var safeDate = safeDayBeforeNextSemester.AddDays(7);
            return ActiveSemester(safeDate, db);
        }

        public static Semester LastSemester(ProStudentCreatorDBDataContext db)
        {
            var currentSemester = ActiveSemester(DateTime.Now, db);
            var safeDate = currentSemester.StartDate.AddDays(-7);
            return ActiveSemester(safeDate, db);
        }

        //code by M. Yousuf & M. Svenson (https://stackoverflow.com/a/9064954)
        public static DateTime StartOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
                weekNum--;

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        public static Semester NextSemester(Semester semester, ProStudentCreatorDBDataContext db)
        {
            return ActiveSemester(semester.DayBeforeNextSemester.Value.AddDays(7), db);
        }

        public static Semester AfterNextSemester(Semester semester, ProStudentCreatorDBDataContext db)
        {
            return NextSemester(NextSemester(semester, db), db);
        }

        public static Semester LastSemester(Semester semester, ProStudentCreatorDBDataContext db)
        {
            return ActiveSemester(semester.StartDate.AddDays(-7), db);
        }

        public static Semester ActiveSemester(DateTime date, ProStudentCreatorDBDataContext db)
        {
            return db.Semester.SingleOrDefault(s => date >= s.StartDate && date < s.DayBeforeNextSemester);
        }

        public bool IsSpringSemester()
        {
            return Name.Contains("FS");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
