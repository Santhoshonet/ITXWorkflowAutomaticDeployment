using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace ITXWorkflowAppDeploymentForms._layouts.ITSWorkflowAppDeployment
{
    public partial class Association : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool TerminateFlag = false;
            string SiteUrl = MyUtilities.getSiteURL(SPContext.Current);
            try
            {
                SPList List = MyUtilities.getListUID(SPContext.Current);
                List.ParentWeb.AllowUnsafeUpdates = true;

                bool folderFound = false;
                foreach (SPListItem fold in List.Folders)
                {
                    if (fold.DisplayName.ToLower().Trim() == "layouts")
                    {
                        folderFound = true;
                        break;
                    }
                }
                SPListItem folder;
                if (!folderFound)
                {
                    folder = List.Items.Add("", SPFileSystemObjectType.Folder, "layouts");
                    folder.Update();
                }

                folderFound = false;
                foreach (SPListItem fold in List.Folders)
                {
                    if (fold.DisplayName.ToLower().Trim() == "features")
                    {
                        folderFound = true;
                        break;
                    }
                }
                if (!folderFound)
                {
                    folder = List.Items.Add("", SPFileSystemObjectType.Folder, "features");
                    folder.Update();
                }

                folderFound = false;
                foreach (SPListItem fold in List.Folders)
                {
                    if (fold.DisplayName.ToLower().Trim() == "bin")
                    {
                        folderFound = true;
                        break;
                    }
                }
                if (!folderFound)
                {
                    folder = List.Items.Add("", SPFileSystemObjectType.Folder, "bin");
                    folder.Update();
                }

                folderFound = false;
                foreach (SPListItem fold in List.Folders)
                {
                    if (fold.DisplayName.ToLower().Trim() == "gac")
                    {
                        folderFound = true;
                        break;
                    }
                }
                if (!folderFound)
                {
                    folder = List.Items.Add("", SPFileSystemObjectType.Folder, "gac");
                    folder.Update();
                }

                folderFound = false;
                foreach (SPListItem fold in List.Folders)
                {
                    if (fold.DisplayName.ToLower().Trim() == "executables")
                    {
                        folderFound = true;
                        break;
                    }
                }
                if (!folderFound)
                {
                    folder = List.Items.Add("", SPFileSystemObjectType.Folder, "executables");
                    folder.Update();
                }

                List.Update();

                // redirecting to list
                TerminateFlag = true;
                Response.Redirect(SPUtility.GetFullUrl(List.ParentWeb.Site, List.DefaultViewUrl));
            }
            catch (Exception)
            {
                if (!TerminateFlag)
                    Response.Redirect(SiteUrl);
            }
        }
    }
}