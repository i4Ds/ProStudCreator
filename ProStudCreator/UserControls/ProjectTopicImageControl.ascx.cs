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
    public partial class ProjectTopicImageControl : System.Web.UI.UserControl
    {
        public int Margin { get; set; } = 0;
        public Topic Topic { get; set; }

        private string SelectedBackColor { get; set; }
        private string SelectedFontColor { get => "#000000"; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Topic is null)
            {
                SelectedBackColor = "#FFFFFF";

                DivFirstTopic.InnerText = "";
                DivSecondTopic.Visible = false;
                DivWrapper.Style["opacity"] = "0";
            }
            else
            {
                SelectedBackColor = $"#{Topic.Color}";

                DivFirstTopic.InnerText = Topic.FirstText;
                if (string.IsNullOrWhiteSpace(Topic.SecondText))
                {
                    DivSecondTopic.Visible = false;
                }
                else
                {
                    DivSecondTopic.InnerText = Topic.SecondText;
                }

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