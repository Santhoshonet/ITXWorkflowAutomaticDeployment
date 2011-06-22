using System;
using System.IO;
using Ionic.Zip;
using Microsoft.SharePoint;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var Site = new SPSite("http://epm2007demo/pwa04"))
            {
                SPList List = Site.RootWeb.Lists["Shared Documents"];
                foreach (SPListItem item in List.Items)
                {
                    if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                    {
                        byte[] bytes = item.Folder.Files[0].OpenBinary();
                        var writer = new FileStream("c:\\text.docx", FileMode.OpenOrCreate);
                        writer.Write(bytes, 0, bytes.Length);
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            return;
            using (var zip = new ZipFile(@"C:\Documents and Settings\Administrator\Desktop\ITXWorkflowAppDeployment\Batch_Installer.zip"))
            {
                zip.ExtractAll(Environment.ExpandEnvironmentVariables("%temp%"));
            }

            return;
            foreach (var directory in Directory.GetDirectories(@"C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN"))
            {
                string[] dirs = directory.Split('\\');
                string curdir = dirs[dirs.Length - 1];
            }

            return;
        }
    }
}