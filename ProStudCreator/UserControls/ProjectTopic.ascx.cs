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
        private Color SelectedBackColor { get; set; }
        private Color UnselectedBackColor { get => ColorTranslator.FromHtml("#E8E6E6"); }
        private string SelectedFontColor { get => "#000000"; }
        private string UnselectedFontColor { get => "#C8C6C6"; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                Selected = (bool)ViewState[ViewStateKey];
            }
            else
            {
                var res = getTopicValues(Name);
                SelectedBackColor = ColorTranslator.FromHtml(res.color);

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
            DivWrapper.BackColor = SelectedBackColor;
            DivFirstTopic.Style.Add("color", SelectedFontColor);
            DivSecondTopic.Style.Add("color", SelectedFontColor);
        }

        private void Unselect()
        {
            DivWrapper.BackColor = UnselectedBackColor;
            DivFirstTopic.Style.Add("color", UnselectedFontColor);
            DivSecondTopic.Style.Add("color", UnselectedFontColor);
        }

        protected (string firstTopic, string secondTopic, string color) getTopicValues(string name)
        {
            switch(name)
            {
                case "AppWeb":
                    return ("App", "Web", "#BCB5B5");
                default:
                    throw new Exception("No topic with this name");
            }
        }
    }
}