﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public partial class PDF : System.Web.UI.Page
    {
        private ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            var id = int.Parse(Request.QueryString["id"]);
            var forceDl = bool.Parse(Request.QueryString["dl"]);

            var idPDF = db.Projects.Single((Project i) => i.Id == id);

            if(!(ShibUser.IsAuthenticated() || idPDF.State==ProjectState.Published))
                throw new HttpException(403, "Nicht berechtigt");

            byte[] bytesInStream;
            using (var output = new MemoryStream())
            {
                using (var document = PdfCreator.CreateDocument())
                {
                    PdfCreator pdfCreator = new PdfCreator();
                    pdfCreator.AppendToPDF(document, output, Enumerable.Repeat(idPDF, 1));
                }
                bytesInStream = output.ToArray();
            }
            Response.Clear();

            if(forceDl)
            {
                Response.ContentType = "application/force-download";
                Response.AddHeader("content-disposition", "attachment; filename=" + idPDF.Department.DepartmentName + idPDF.ProjectNr.ToString("00") + ".pdf");
            }
            else
            {
                Response.ContentType = "application/pdf";
            }
            Response.BinaryWrite(bytesInStream);
            Response.End();
        }
    }
}