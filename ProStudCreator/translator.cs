using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using i4Ds.LanguageToolkit;

namespace ProStudCreator
{
    public class Translator
    {
        public Language? language { get;set; }

        public void DetectLanguage(Project p)
        {
            var allText = p.InitialPosition + " " + p.Objective + " " + p.ProblemStatement + " " + p.ProblemStatement + " " + p.Remarks;
            if(allText != "")
            {
                language = LanguageDetector.DetectLanguage(allText);
                if (language != Language.English && language != Language.German)
                {
                    language = Language.German;
                    if (p.LanguageEnglish && !p.LanguageGerman)
                    {
                        language = Language.English;
                    }
                    else if (p.LanguageGerman && !p.LanguageEnglish)
                    {
                        language = Language.German;
                    }
                }
                                
            }

        } 
        public Translator()
        {

        }
        public Translator(Language language)
        {
            this.language = language;
        }

        public string GetAnd()
        {
            if (language == Language.English)
                return "and";
            return "und";
        }

        public string GetOr()
        {
            if (language == Language.English)
                return "or";
            return "oder";
        }

        public string GetHeadingInitialPosition()
        {
            if(language == Language.English)
                return "Initial position";
            return "Ausgangslage";
        }
        public string GetHeadingObjective()
        {
            if (language == Language.English)
                return "Objective";
            return "Ziel der Arbeit";
        }
        public string GetHeadingProblemStatement()
        {
            if(language == Language.English)
                return "Problem statement";
            return "Problemstellung";
        }

        public string GetHeadingAnnotation()
        {
            if (language == Language.English)
                return "Note";
            return "Bemerkung";
        }
        public string GetHeadingTechnology()
        {
            if (language == Language.English)
                return "Technologies/Technical emphasis/References";
            return "Technologien/Fachliche Schwerpunkte/Referenzen";
        }
        public  string GetHeadingOneSemester()
        {
            if(language == Language.English)
                return "This project must be realised in a single semester.\n";
            return "Dieses Projekt muss in einem einzigen Semester durchgeführt werden.\n";
        }

        public string GetReservedString(string Reservation1Name, string Reservation2Name)
        {
            var strReservations = "";
            if(language == Language.English)
            {
                strReservations = "This Project is reserved for " + Reservation1Name;
                if (Reservation2Name != "") strReservations += " and " + Reservation2Name;
                return strReservations;
            }

            strReservations = "Dieses Projekt ist für " + Reservation1Name;
            if (Reservation2Name != "") strReservations += " und " + Reservation2Name;
            strReservations += " reserviert.\n";

            return strReservations;
        }
        public string GetHeadingAdvisor()
        {
            if(language == Language.English)
                return "Advisor";
            return "Betreuer";
        }
        public string GetHeadingPriority()
        {
            if(language == Language.English)
                return "Priority";
            return "Priorität";
        }
        public string GetHeadingClient()
        {
            if(language == Language.English)
                return "Client";
            return "Auftragsgeber";
        }
        public string GetHeadingWorkScope()
        {
            if (language == Language.English)
                return "Work scope";
            return "Arbeitsumfang";
        }
        public string GetHeadingTeamSize()
        {
            if (language == Language.English)
                return "Team size";
            return "Teamgrösse";
        }
        public string GetHeadingLangugages()
        {
            if (language == Language.English)
                return "Languages";
            return "Sprachen";
        }
        public string GetLanguageGerman()
        {
            if (language == Language.English)
                return "German";
            return "Deutsch";
        }
        public string GetLanguageEnglish()
        {
            if (language == Language.English)
                return "English";
            return "Englisch";
        }
        public string GetStudyCourseTitle()
        {
            if (language == Language.English)
                return "Study course";
            return "Studiengang";
        }
        public string GetStudyCourseCS()
        {
            if (language == Language.English)
                return "Computer Science";
            return "Informatik";
        }
        public string GetStudyCourseDS()
        {
            return "Data Science";
        }

        public string GetHeadingFooter(Project CurrentProject, ProStudentCreatorDBDataContext db)
        {
            var foot = "";
            if (language == Language.English)
            {
                foot += " Computer Science/" + CurrentProject.Department.DepartmentName + "/Student projects " +
                    CurrentProject?.Semester?.Name ?? Semester.NextSemester(db).Name;
                return foot;
            }
            foot += "Studiengang Informatik/" + CurrentProject.Department.DepartmentName + "/Studierendenprojekte " +
                    CurrentProject?.Semester?.Name ?? Semester.NextSemester(db).Name;
            return foot;
        }



    }
}