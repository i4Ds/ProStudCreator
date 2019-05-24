using System;
using System.Net.Mail;
using System.Text;
using System.Web.UI;

namespace ProStudCreator.error
{
    public partial class AccessDenied : Page
    {
        protected string errorMsg;

        protected void Page_Load(object sender, EventArgs e)
        {
            errorMsg += "Login:\t" + (ShibUser.GetEmail() ?? "(Nicht verfübar)") + "\n";
            errorMsg += "Abteilung: " + (ShibUser.GetDepartment(new ProStudentCreatorDBDataContext())?.DepartmentName ?? "(Unbekannt)") + "\n";
            errorMsg += "Called URL: " + (Request.QueryString["url"] != null ? Request.QueryString["url"].ToString() : "(Unbekannt)") + "\n";
            SendMail(errorMsg);
        }

        private void SendMail(string errorMsg)
        {
            var mail = new MailMessage { From = new MailAddress("noreply@fhnw.ch") };
            mail.To.Add(new MailAddress(Global.WebAdmin));
            mail.Subject = "ProStud AccessDenied";
            mail.IsBodyHtml = true;

            var mailMessage = new StringBuilder();
            mailMessage.Append("<div style=\"font-family: Arial\">");
            mailMessage.Append(errorMsg.Replace("\n", "<br>"));
            mail.Body = mailMessage.ToString();
            TaskHandler.SendMail(mail);
        }
    }
}