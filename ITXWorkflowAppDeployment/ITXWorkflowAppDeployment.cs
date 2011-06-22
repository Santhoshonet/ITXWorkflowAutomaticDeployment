using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Workflow.Activities;
using Ionic.Zip;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace ITXWorkflowAppDeployment
{
    public sealed partial class ITXWorkflowAppDeployment : SequentialWorkflowActivity
    {
        public ITXWorkflowAppDeployment()
        {
            InitializeComponent();
        }

        private void Deployment_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                // File saving into temp folder
                string TempFolderPath = Path.GetTempPath() + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour +
                                        DateTime.Now.Minute + DateTime.Now.Second;

                AddEventEntry("Started", "Installation started ...", "");

                string gacfolder = TempFolderPath + @"\gac";
                string featuresfolder = TempFolderPath + @"\features";
                string layoutsfolder = TempFolderPath + @"\layouts";
                string binfolder = TempFolderPath + @"\bin";
                string executablefolder = TempFolderPath + @"\executables";

                AddEventEntry("Started", "Creating Tmp folder in the path '" + TempFolderPath + "'", "");

                if (!Directory.Exists(TempFolderPath))
                    Directory.CreateDirectory(TempFolderPath);

                AddEventEntry("Done", "Created Tmp folder in the path '" + TempFolderPath + "'", "");

                #region Version1

                /*
             // SAving the deployment files from document library individual folders
            try
            {
                SPList List = onWorkflowActivated1_WorkflowProperties1.List;
                foreach (SPListItem item in List.Folders)
                {
                    if (item.FileSystemObjectType == SPFileSystemObjectType.Folder)
                    {
                        if (item.DisplayName.ToLower().Trim() == "layouts")
                            SaveFile(layoutsfolder, item);
                        else if (item.DisplayName.ToLower().Trim() == "features")
                            SaveFile(featuresfolder, item);
                        else if (item.DisplayName.ToLower().Trim() == "bin")
                            SaveFile(binfolder, item);
                        else if (item.DisplayName.ToLower().Trim() == "gac")
                            SaveFile(gacfolder, item);
                        else if (item.DisplayName.ToLower().Trim() == "executables")
                            SaveFile(executablefolder, item);
                    }
                }
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(ex.StackTrace))
                    MyUtilities.ErrorLog("Error at creating file in temp folder due to " + ex.Message, EventLogEntryType.Error);
                else
                    MyUtilities.ErrorLog("Error at creating file in temp folder due to " + ex.StackTrace, EventLogEntryType.Error);
            } */

                #endregion Version1

                // unzip the uploaded file into temp folder and extracting here
                try
                {
                    SPListItem Item = onWorkflowActivated1_WorkflowProperties1.Item;
                    if (Item.FileSystemObjectType == SPFileSystemObjectType.File)
                    {
                        if (Path.GetExtension(Item.File.Name).ToLower() == ".zip" || Path.GetExtension(Item.File.Name).ToLower() == "zip")
                        {
                            if (File.Exists(TempFolderPath + "\\" + Item.File.Name))
                            {
                                AddEventEntry("Started",
                                              "The current workflow item file have been already found at '" +
                                              TempFolderPath + @"\" + Item.File.Name + "', trying to delete the existing file.", "");
                                File.Delete(TempFolderPath + @"\" + Item.File.Name);
                                AddEventEntry("Done",
                                              "The current workflow item file have been deleted, the path is '" +
                                              TempFolderPath + @"\" + Item.File.Name + "'.", "");
                            }

                            AddEventEntry("Started",
                                          "The current workflow item file have been started to save at '" +
                                          TempFolderPath + @"\" + Item.File.Name + "'", "");

                            byte[] bytes = Item.File.OpenBinary();
                            var writer = new FileStream(TempFolderPath + @"\" + Item.File.Name, FileMode.OpenOrCreate);
                            writer.Write(bytes, 0, bytes.Length);
                            writer.Flush();
                            writer.Close();

                            AddEventEntry("Done",
                                          "The current workflow item file have been saved at '" +
                                          TempFolderPath + @"\" + Item.File.Name + "'", "");

                            // file extaracting here
                            using (var Zip = new ZipFile(TempFolderPath + @"\" + Item.File.Name))
                            {
                                AddEventEntry("Started",
                                              "The current workflow item file [zip] have been started to extract at '" +
                                              TempFolderPath, "");
                                Zip.ExtractAll(TempFolderPath, ExtractExistingFileAction.OverwriteSilently);
                                AddEventEntry("Done",
                                              "The current workflow item file [zip] have been extracted at '" +
                                              TempFolderPath, "");
                            }
                        }
                        else
                            AddEventEntry("Failure Notice.", "The current workflow item file is not a zippped file.", "");
                    }
                    else
                        AddEventEntry("Failure Notice.", "The current workflow item is not a file object.", "");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                        MyUtilities.ErrorLog("Error at read and unzip the uploaded file due to " + ex.Message,
                                             EventLogEntryType.Error);
                    else
                        MyUtilities.ErrorLog("Error at read and unzip the uploaded file due to " + ex.StackTrace,
                                             EventLogEntryType.Error);
                    AddEventEntry("Error", "Error! " + ex.Message, "");
                }

                AddEventEntry("Started", "Creating the missed out folders to avoid unneccesary errors.", "");
                // creating missed out folders to avoid unneccesary errors below
                if (!Directory.Exists(layoutsfolder))
                    Directory.CreateDirectory(layoutsfolder);
                if (!Directory.Exists(featuresfolder))
                    Directory.CreateDirectory(featuresfolder);
                if (!Directory.Exists(binfolder))
                    Directory.CreateDirectory(binfolder);
                if (!Directory.Exists(executablefolder))
                    Directory.CreateDirectory(executablefolder);
                if (!Directory.Exists(gacfolder))
                    Directory.CreateDirectory(gacfolder);

                AddEventEntry("Done", "Created the missed out folders to avoid unneccesary errors.", "");

                AddEventEntry("Started", "Configuring sharepoint standard folder paths.", "");
                //Preparing batch script
                string batfilepath = TempFolderPath + @"\install.bat";
                string TwelvehiveFolderPath = SPUtility.GetGenericSetupPath(string.Empty);
                string LayoutsFolderPath = SPUtility.GetGenericSetupPath(string.Empty) + @"\Template\layouts";
                string FeaturesFolderPath = SPUtility.GetGenericSetupPath(string.Empty) + @"\Template\Features";

                AddEventEntry("Done", "Configured the sharepoint standard folder paths successfully.", "");

                try
                {
                    if (File.Exists(batfilepath))
                    {
                        AddEventEntry("Started", "Trying to delete the existing script file at " + batfilepath, "");
                        File.Delete(batfilepath);
                        AddEventEntry("Done", "Deleted the existing script file at " + batfilepath, "");
                    }

                    AddEventEntry("Started", "Started to build batch scripting in the file.", "");
                    //Preparing bat file to install
                    using (var writer = new StreamWriter(batfilepath))
                    {
                        //writer.WriteLine("@ echo off");

                        writer.WriteLine(@"xcopy """ + TempFolderPath + @"\features\*"" """ + FeaturesFolderPath + @""" /s /c /q /h /r /y");
                        writer.WriteLine(@"xcopy """ + TempFolderPath + @"\layouts\*"" """ + LayoutsFolderPath + @""" /s /c /q /h /r /y");

                        AddEventEntry("Started", "Started to get virtual paths to install binary files.", "");
                        var ExtendedUrlList = new List<string>();
                        try
                        {
                            //To Get Port from Siteurl
                            var uri = new Uri(onWorkflowActivated1_WorkflowProperties1.SiteUrl);
                            int Port = uri.Port;
                            if (Port != 80)
                            {
                                string Wss80Path = ITXProjectsLibrary.Deployment.GetWssVirtualDirectoryPath("80");
                                if (Wss80Path != string.Empty)
                                {
                                    ExtendedUrlList.Add(Wss80Path);
                                }
                            }
                            foreach (int zoneindex in Enum.GetValues(typeof(SPUrlZone)))
                            {
                                string ExtendedUrl =
                                    ITXProjectsLibrary.Deployment.GetVirtualDirectoryPath((SPUrlZone)zoneindex,
                                                                                          onWorkflowActivated1_WorkflowProperties1
                                                                                              .Site);
                                bool Found = false;
                                foreach (string s in ExtendedUrlList)
                                {
                                    if (s.ToLower().Trim() == ExtendedUrl.ToLower().Trim())
                                    {
                                        Found = true;
                                        break;
                                    }
                                }
                                if (!Found)
                                {
                                    ExtendedUrlList.Add(ExtendedUrl);
                                }
                            }
                            AddEventEntry("Done", "Completed to get virtual paths.", "");
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            AddEventEntry("Started", "Started writing scripts into file.", "");
                            foreach (string Urls in ExtendedUrlList)
                            {
                                if (!string.IsNullOrEmpty(Urls))
                                    writer.WriteLine(@"xcopy bin\* """ + Urls + @"\bin" + @""" /s /c /q /h /r /y");
                            }
                            AddEventEntry("Done", "Completed writing scripts into file.", "");
                        }

                        AddEventEntry("Started", "Started writing scripts for installing Global Assembly Cache files[Dll's].", "");
                        // installing in GAC
                        string gacutilpath = Environment.ExpandEnvironmentVariables("%windir%") +
                                             @"\Microsoft.NET\Framework\v1.1.4322\gacutil.exe";
                        foreach (var filepath in Directory.GetFiles(gacfolder))
                        {
                            writer.WriteLine(@"""" + gacutilpath + @""" /i """ + filepath + @"""");
                        }
                        AddEventEntry("Done", "Completed writing scripts for installing Global Assembly Cache files[Dll's].", "");

                        AddEventEntry("Started", "Started writing scripts for installing sharepoint features using stsadm.", "");
                        // Installing the Features
                        foreach (string featuresdirpath in Directory.GetDirectories(featuresfolder))
                        {
                            if (File.Exists(featuresdirpath + @"\feature.xml"))
                            {
                                string[] dirs = featuresdirpath.Split('\\');
                                string curdir = dirs[dirs.Length - 1];
                                writer.WriteLine(@"""" + TwelvehiveFolderPath +
                                                 @"\BIN\STSADM.EXE"" -o installfeature -filename """ + curdir +
                                                 @"\feature.xml"" -force");
                                writer.WriteLine(@"""" + TwelvehiveFolderPath +
                                                 @"\BIN\STSADM.EXE"" -o activatefeature -filename """ + curdir +
                                                 @"\feature.xml"" -url """ + onWorkflowActivated1_WorkflowProperties1.SiteUrl + @""" -force");
                            }
                        }
                        AddEventEntry("Done", "Completed writing scripts for installing sharepoint features.", "");

                        AddEventEntry("Started", "Started to close the script file's write session.", "");
                        writer.Flush();
                        writer.Close();
                        AddEventEntry("Done", "Completed the file closing process.", "");
                    }

                    AddEventEntry("Done", "Completed building the batch scripting.", "");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                        MyUtilities.ErrorLog("Error at preparing script due to " + ex.Message, EventLogEntryType.Error);
                    else
                        MyUtilities.ErrorLog("Error at preparing script due to " + ex.StackTrace,
                                             EventLogEntryType.Error);
                    AddEventEntry("Error", "Error! " + ex.Message, "");
                }

                // Executing the Script
                try
                {
                    AddEventEntry("Started", "Started to execute the script file 1.", "");
                    string OutPut = ExecuteProcess(batfilepath, ProcessWindowStyle.Hidden, true);
                    try
                    {
                        onWorkflowActivated1_WorkflowProperties1.Web.AllowUnsafeUpdates = true;
                        SPListItem TaskItem = onWorkflowActivated1_WorkflowProperties1.TaskList.Items.Add();
                        TaskItem["Title"] = "Output";
                        TaskItem["Description"] = OutPut;
                        TaskItem.Update();
                    }
                    catch (Exception)
                    {
                    }
                    AddEventEntry("Done", "Completed the execution of script from file-1.", "");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                        MyUtilities.ErrorLog("Error at executing script due to " + ex.Message, EventLogEntryType.Error);
                    else
                        MyUtilities.ErrorLog("Error at executing script due to " + ex.StackTrace,
                                             EventLogEntryType.Error);
                    AddEventEntry("Error", "Error! " + ex.Message, "");
                }

                // now we need to run the files in Executables folder.
                try
                {
                    AddEventEntry("Started", "Started to prepare script file to run executable files.", "");
                    batfilepath = TempFolderPath + @"\install1.bat";
                    //Preparing bat file to install
                    if (File.Exists(batfilepath))
                    {
                        AddEventEntry("Started", "Trying to delete the existing script file at " + batfilepath, "");
                        File.Delete(batfilepath);
                        AddEventEntry("Done", "Deleted the existing script file at " + batfilepath, "");
                    }
                    using (var writer = new StreamWriter(batfilepath))
                    {
                        //writer.WriteLine("@ echo off");
                        foreach (string filepath in Directory.GetFiles(executablefolder))
                        {
                            writer.WriteLine(@"""" + filepath + @""" " + onWorkflowActivated1_WorkflowProperties1.SiteUrl);
                        }
                        writer.Flush();
                        writer.Close();
                    }
                    AddEventEntry("Done", "Completed the process of preparing script file to run executable files.", "");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                        MyUtilities.ErrorLog("Error at runnning executable files due to " + ex.Message,
                                             EventLogEntryType.Error);
                    else
                        MyUtilities.ErrorLog("Error at runnning executable files due to " + ex.StackTrace,
                                             EventLogEntryType.Error);
                    AddEventEntry("Error", "Error! " + ex.Message, "");
                }

                // Executing the Second Script
                try
                {
                    AddEventEntry("Started", "Started to execute the script file 2.", "");
                    string OutPut = ExecuteProcess(batfilepath, ProcessWindowStyle.Hidden, true);
                    try
                    {
                        onWorkflowActivated1_WorkflowProperties1.Web.AllowUnsafeUpdates = true;
                        SPListItem TaskItem = onWorkflowActivated1_WorkflowProperties1.TaskList.Items.Add();
                        TaskItem["Title"] = "Output_executables";
                        TaskItem["Description"] = OutPut;
                        TaskItem.Update();
                    }
                    catch (Exception)
                    {
                    }
                    AddEventEntry("Done", "Completed the execution process of script from file-2.", "");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                        MyUtilities.ErrorLog("Error at executing second script due to " + ex.Message,
                                             EventLogEntryType.Error);
                    else
                        MyUtilities.ErrorLog("Error at executing second script due to " + ex.StackTrace,
                                             EventLogEntryType.Error);
                    AddEventEntry("Error", "Error! " + ex.Message, "");
                }

                // finally deleting the temp folder
                try
                {
                    AddEventEntry("Started", "Started to delete the whole temp folder.", "");
                    Directory.Delete(TempFolderPath, true);
                    AddEventEntry("Done", "Succesfully deleted the whole temp folder.", "");
                }
                catch (Exception ex)
                {
                    if (string.IsNullOrEmpty(ex.StackTrace))
                        MyUtilities.ErrorLog("Error at deleting temp folder due to " + ex.Message,
                                             EventLogEntryType.Error);
                    else
                        MyUtilities.ErrorLog("Error at deleting temp folder due to " + ex.StackTrace,
                                             EventLogEntryType.Error);
                    AddEventEntry("Error", "Error! " + ex.Message, "");
                }

                AddEventEntry("Done", "Installation process completed successfully...", "");
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(ex.StackTrace))
                    MyUtilities.ErrorLog("Error at workflow installation due to " + ex.Message,
                                         EventLogEntryType.Error);
                else
                    MyUtilities.ErrorLog("Error at workflow installation due to " + ex.StackTrace,
                                         EventLogEntryType.Error);
                AddEventEntry("Error", "Error! " + ex.Message, "");
            }
        }

        public static void SaveFile(string path, SPListItem item)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (SPFile file in item.Folder.Files)
            {
                byte[] bytes = file.OpenBinary();
                var writer = new StreamWriter(path + "\\" + file.Name);
                writer.Write(bytes);
                writer.Flush();
                writer.Close();
            }

            foreach (SPListItem sub_item in item.ListItems)
            {
                if (sub_item.FileSystemObjectType == SPFileSystemObjectType.Folder)
                    SaveFile(path + "\\" + sub_item.DisplayName, sub_item);
            }
        }

        public SPWorkflowActivationProperties onWorkflowActivated1_WorkflowProperties1 = new SPWorkflowActivationProperties();

        public void AddEventEntry(string outcome, string description, string Otherdata)
        {
            SPWorkflow.CreateHistoryEvent(onWorkflowActivated1_WorkflowProperties1.Web, onWorkflowActivated1_WorkflowProperties1.WorkflowId, (int)SPWorkflowHistoryEventType.WorkflowComment, onWorkflowActivated1_WorkflowProperties1.OriginatorUser, new TimeSpan(1), outcome, description, Otherdata);
        }

        public static string ExecuteProcess(String FilePath, ProcessWindowStyle Style, bool WaitForExit)
        {
            string Output = string.Empty;
            try
            {
                var startInfo = new ProcessStartInfo(FilePath) { UseShellExecute = false, WindowStyle = Style };
                startInfo.RedirectStandardOutput = true;
                var batchExecute = new Process { StartInfo = startInfo };
                batchExecute.Start();
                //batchExecute.BeginOutputReadLine();
                Output = batchExecute.StandardOutput.ReadToEnd();
                if (WaitForExit)
                    batchExecute.WaitForExit();
            }
            catch (Exception)
            {
            }
            return Output;
        }
    }
}