using iTextSharp.text.io;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator.UserControls
{
    public partial class ProjectTopic : System.Web.UI.UserControl
    {
        public string Name { get; set; }
        public bool Selected
        {
            get
            {
                return internalSelected;
            }
            set
            {
                ViewState[ViewStateKey] = internalSelected = value;
            }
        }

        private bool internalSelected = false;
        private string ViewStateKey { get => $"ProjectTopic_{Name}"; }
        private string SelectedBackColor { get; set; }
        private string UnselectedBackColor { get => "#E8E6E6"; }
        private string SelectedFontColor { get => "#000000"; }
        private string UnselectedFontColor { get => "#C8C6C6"; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var res = getTopicValues(Name);
            SelectedBackColor = res.color;

            if (IsPostBack)
            {
                Selected = (bool)ViewState[ViewStateKey];
            }
            else
            {
                DivWrapper.ToolTip = res.toolTip;
                DivFirstTopic.InnerText = res.firstTopic;
                if (string.IsNullOrWhiteSpace(res.secondTopic))
                {
                    DivSecondTopic.Visible = false;
                }
                else
                {
                    DivSecondTopic.InnerText = res.secondTopic;
                }
                Selected = Selected;
                UpdateColors();
            }
        }

        protected void ButToggle_OnClick(object sender, EventArgs e)
        {
            Selected = !Selected;
            UpdateColors();
        }

        private void UpdateColors()
        {
            if (Selected)
            {
                Select();
            }
            else
            {
                Unselect();
            }
            TopicUpdatePanel.Update();
        }

        private void Select()
        {
            DivWrapper.Style["background-color"] = SelectedBackColor;
            DivFirstTopic.Style["color"] = SelectedFontColor;
            DivSecondTopic.Style["color"] = SelectedFontColor;
        }

        private void Unselect()
        {
            DivWrapper.Style["background-color"] = UnselectedBackColor;
            DivFirstTopic.Style["color"] = UnselectedFontColor;
            DivSecondTopic.Style["color"] = UnselectedFontColor;
        }

        public static (string firstTopic, string secondTopic, string toolTip, string color) getTopicValues(string name)
        {
            switch(name)
            {
                case "AppWeb":
                    return ("App", "Web", "Mobile Apps, Webentwicklung, ...", "#BCB5B5");
                case "DesignUX":
                    return ("Design", "UX", "Design, Usability, User Interfaces, ...", "#FFB643");
                case "HW":
                    return ("HW", null, "Hardwarenah, IoT, Embedded, Low-level, ...", "#6EF083");
                case "CGIP":
                    return ("CG", "IP", "Computergrafik, 3D, Bildverarbeitung, ...", "#3A56F7");
                case "MLAlg":
                    return ("ML", "Alg", "Mathematik, Algorithmen, Machine Learning, Data Mining, ...", "#EFED5C");
                case "DBBigData":
                    return ("DB", "Big Data", "Datenbanken, Big Data, Data Spaces, ...", "#B895E3");
                case "SysSec":
                    return ("Sys", "Sec", "ITSM, Networks, Security, ...", "#E16060");
                case "SERE":
                    return ("SE", "RE", "Software Engineering, Testing, Tooling, Architectures, Requirements Engineering, ...", "#36AE6E");
                case "DataScience":
                    return ("Data", "Science", "Data Science", "#38B8F7");
                case "Transparent":
                    return ("", "", "", "#000000");
                default:
                    throw new Exception("No topic with this name");
            }
        }
    }
}