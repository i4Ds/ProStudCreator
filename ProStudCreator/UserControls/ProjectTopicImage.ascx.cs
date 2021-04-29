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
    public partial class ProjectTopicImage : System.Web.UI.UserControl
    {
        public int Margin { get; set; } = 0;
        public string Name { get; set; }

        private string SelectedBackColor { get; set; }
        private string SelectedFontColor { get => "#000000"; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var res = ProjectTopic.getTopicValues(Name);
            SelectedBackColor = res.color;

            DivFirstTopic.InnerText = res.firstTopic;
            if (string.IsNullOrWhiteSpace(res.secondTopic))
            {
                DivSecondTopic.Visible = false;
            }
            else
            {
                DivSecondTopic.InnerText = res.secondTopic;
            }

            if (Name == "Transparent")
            {
                DivWrapper.Style["opacity"] = "0";
            }
            else
            {
                DivWrapper.Style["opacity"] = "1";
                DivWrapper.Style["background-color"] = SelectedBackColor;
                DivFirstTopic.Style["color"] = SelectedFontColor;
                DivSecondTopic.Style["color"] = SelectedFontColor;
            }

            DivWrapper.Style["margin"] = $"{Margin}px";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}