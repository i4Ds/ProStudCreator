using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProStudCreator
{
    public partial class SiteMaster : MasterPage
    {
        private const string AntiXsrfTokenKey = "__AntiXsrfToken";
        private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
        private string _antiXsrfTokenValue;

        public bool inDebugMode;

        protected void Page_Init(object sender, EventArgs e)
        {
            // Der Code unten schützt vor XSRF-Angriffen.
            var requestCookie = Request.Cookies[AntiXsrfTokenKey];
            Guid requestCookieGuidValue;
            if (requestCookie != null && Guid.TryParse(requestCookie.Value, out requestCookieGuidValue))
            {
                // Das Anti-XSRF-Token aus dem Cookie verwenden
                _antiXsrfTokenValue = requestCookie.Value;
                Page.ViewStateUserKey = _antiXsrfTokenValue;
            }
            else
            {
                // Neues Anti-XSRF-Token generieren und im Cookie speichern
                _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
                Page.ViewStateUserKey = _antiXsrfTokenValue;

                var responseCookie = new HttpCookie(AntiXsrfTokenKey)
                {
                    HttpOnly = true,
                    Value = _antiXsrfTokenValue
                };
                if (FormsAuthentication.RequireSSL && Request.IsSecureConnection)
                    responseCookie.Secure = true;
                Response.Cookies.Set(responseCookie);
            }

            Page.PreLoad += Master_Page_PreLoad;
        }

        protected void Master_Page_PreLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Anti-XSRF-Token festlegen
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                ViewState[AntiXsrfUserNameKey] = Context.User.Identity.Name ?? string.Empty;
            }
            else
            {
                // Anti-XSRF-Token überprüfen
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue
                    || (string)ViewState[AntiXsrfUserNameKey] != (Context.User.Identity.Name ?? string.Empty))
                    throw new InvalidOperationException("Fehler bei der Überprüfung des Anti-XSRF-Tokens.");
            }

            if (!ShibUser.IsAuthenticated(new ProStudentCreatorDBDataContext()))
            {
                //throw new HttpException(403, "Nicht berechtigt");
                Response.Redirect("error/AccessDenied.aspx?url=" + HttpContext.Current.Request.Url.AbsoluteUri);
                Response.End();
                return;
            }


            NavAdmin.Visible = ShibUser.CanVisitAdminPage();
            NavWebAdmin.Visible = ShibUser.GetEmail() == Global.WebAdmin;
        }

        public readonly ProStudentCreatorDBDataContext db = new ProStudentCreatorDBDataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
#if DEBUG
            inDebugMode = true;
#endif
            //register new Users
            /*if (!db.UserDepartmentMap.Select(i => i.Mail).Contains(ShibUser.GetEmail()))
            {
                db.UserDepartmentMap.InsertOnSubmit(new UserDepartmentMap(){DepartmentId = ShibUser.GetDepartment(db).Id, Mail = ShibUser.GetEmail(), Name = ShibUser.GetFullName()});
            }*/
        }

        private void Page_Error(object sender, EventArgs e)
        {
        }

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            Context.GetOwinContext().Authentication.SignOut();
            Response.Redirect("/Account/Login.aspx");
        }
    }
}