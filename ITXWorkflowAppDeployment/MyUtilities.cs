using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Microsoft.SharePoint;

namespace ITXWorkflowAppDeployment
{
    public class MyUtilities
    {
        public static string getSiteURL(SPContext Context)
        {
            if (Context != null)
                return Context.Site.Url;

            return "http://epm2007demo/pwa04"; // for developement
        }

        public static SPList getListUID(SPContext Context)
        {
            if (Context != null)
                return SPContext.Current.List;

            using (var Site = new SPSite(getSiteURL(Context)))
            {
                return Site.RootWeb.Lists["Shared Documents"];
            }
        }

        private const string ErrorLogname = "ITXWfDeployment";

        public static void ErrorLog(string LogStr, EventLogEntryType Type)
        {
            try
            {
                System.Security.Principal.WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
                var El = new EventLog();
                if (EventLog.SourceExists(ErrorLogname) == false)
                    EventLog.CreateEventSource(ErrorLogname, ErrorLogname);
                El.Source = ErrorLogname;
                El.WriteEntry(LogStr, Type);
                El.Close();
                wic.Undo();
            }
            catch (Exception Ex87)
            {
                WriteTextLog(Ex87.Message + "\r" + LogStr);
            }
        }

        public static void WriteTextLog(string LogStr)
        {
            try
            {
                System.Security.Principal.WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
                var Writer = new StreamWriter(@"c:\" + ErrorLogname + ".txt", true);
                Writer.WriteLine(LogStr);
                Writer.Close();
                Writer.Dispose();
            }
            catch
            {
                return;
            }
        }
    }
}