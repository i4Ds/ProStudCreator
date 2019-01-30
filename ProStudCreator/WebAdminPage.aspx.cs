using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{

    public partial class WebAdminPage : Page
    {
        private readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Init(object sender, EventArgs e)
        {
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShibUser.GetEmail() != Global.WebAdmin)
            {
                Response.Redirect("error/AccessDenied.aspx");
                Response.End();
            }

            if (!Page.IsPostBack)
            {
            }

            Session["LastPage"] = "webadminpage";
        }
    }
}


