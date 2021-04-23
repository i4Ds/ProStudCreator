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

        protected (string firstTopic, string secondTopic, string color) getTopicValues(string name)
        {
            switch(name)
            {
                case "AppWeb":
                    return ("App", "Web", "#BCB5B5");
                case "DesignUX":
                    return ("Design", "UX", "#FFB643");
                case "HW":
                    return ("HW", null, "#6EF083");
                case "CGIP":
                    return ("CG", "IP", "#4D83FF");
                case "MLAlg":
                    return ("ML", "Alg", "#EFED5C");
                case "DBBigData":
                    return ("DB", "Big Data", "#B895E3");
                case "SysSec":
                    return ("Sys", "Sec", "#E16060");
                case "SERE":
                    return ("SE", "RE", "#36AE6E");
                default:
                    throw new Exception("No topic with this name");
            }
        }
    }
}