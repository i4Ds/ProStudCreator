using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Optimization;
using System.Web.Routing;
using iTextSharp.text.io;
using NPOI.Util;

namespace ProStudCreator
{
    public class Global : HttpApplication
    {
        private const string DummyCacheItemKey = "DummyCacheItem";
        private const string DummyPage = "CheckAllTasks";
        public static readonly TimeSpan AllowTitleChangesBeforeSubmission = TimeSpan.FromDays(11*7);
        public static readonly TimeSpan ExpectFinalPresentationAfterSubmissionForIP5 = TimeSpan.FromDays(3*7);
        public static readonly TimeSpan GradingDuration = TimeSpan.FromDays(3);
        public static readonly string WebAdmin = "simon.felix@fhnw.ch";
        public static readonly string GradeAdmin = "admin.technik@fhnw.ch";
        public static readonly string PayExpertAdmin = "hannelore.gerber@fhnw.ch";
        public static readonly string InvoiceCustomersAdmin = "simon.felix@fhnw.ch"; // "hannelore.gerber@fhnw.ch";
        public static readonly string MarKomAdmin = "jadwiga.gabrys@fhnw.ch";

        private void Application_Start(object sender, EventArgs e)
        {
            // Code, der beim Anwendungsstart ausgeführt wird
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StreamUtil.AddToResourceSearch(Assembly.Load("itext-hyph-xml"));

            Application.Add("dummyRequest", "");

            TaskHandler.CheckAllTasks();

            //Solves problem which  occures if you installed a different language on your Pc 
            //with out it the Encoding may not work on every Pc.
            ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.Default.CodePage;


            

        }




        private void RegisterChacheEntry()
        {
            if (null != HttpContext.Current.Cache[DummyCacheItemKey])
                return;

            HttpContext.Current.Cache.Add(DummyCacheItemKey, "DummyTest", null,
                DateTime.MaxValue, TimeSpan.FromHours(3),
                CacheItemPriority.Normal,
                new CacheItemRemovedCallback(CacheItemRemovedCallback));

        }


        public void CacheItemRemovedCallback(string key, object value, CacheItemRemovedReason reason)
        {
            HitPage();

            TaskHandler.CheckAllTasks();
        }

        private void HitPage()
        {
            using (var client = new WebClient())
                try
                {
#if DEBUG
                    client.DownloadData(Application.Get("dummyRequest").ToString());
#else

                    client.DownloadData(ConfigurationManager.AppSettings["localhost_remote"] + DummyPage);
#endif
                }
                catch(Exception e)
                {
                    throw new RuntimeException($"Request of {ConfigurationManager.AppSettings["localhost_remote"] + DummyPage} failed", e);
                }
        }



        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
#if DEBUG
            if (Application.Get("dummyRequest").ToString() == "")
            {
                Application.Set("dummyRequest", "http://" + Request.ServerVariables["HTTP_HOST"] + "/" + DummyPage);
                RegisterChacheEntry();
            }
#else

            if (Application.Get("dummyRequest").ToString() == "")
            {
                Application.Set("dummyRequest", ConfigurationManager.AppSettings["localhost_remote"] + DummyPage);
                RegisterChacheEntry();
            }


#endif

            if (HttpContext.Current.Request.Url.ToString() == Application.Get("dummyRequest").ToString())
            {
                //Add the item in cache and when succesful, do the work.
                RegisterChacheEntry();
            }
        }
    }
}