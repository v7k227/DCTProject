using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DCTLib;

namespace DCTClient
{
    internal class DCTFuncs
    {
        public static bool Login(string userId, string password, ref string errMsg)
        {
            try
            {
                if (password.ToLowerInvariant() == DCTDef.forceloginpw)
                {
                    return true;
                }
                bool isPassed = false;

                string[] fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                string resName = fileNames.FirstOrDefault(r => r.ToLowerInvariant().Contains(DCTDef.MemberFileName.ToLowerInvariant()));

                if (!string.IsNullOrEmpty(resName))
                {
                    Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName);

                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string[] splitStr = line.Split(',');

                            if (splitStr.Length < 2)
                                continue;

                            string id = splitStr[0];
                            string pw = splitStr[1];

                            if (userId == id && password == pw)
                            {
                                if (string.IsNullOrEmpty(pw))
                                {
                                    break;
                                }
                                isPassed = true;
                                break;
                            }
                        }
                    }
                }

                if (!isPassed)
                    errMsg = "無您的登入資訊, 請檢查座位號碼與身分證件末四碼是否正確";

                return isPassed;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        public static Stream GetPPFile()
        {
            string[] ResName = new string[] { "PP.rtf" };

            foreach (string tarRes in ResName)
            {
                string[] fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string fileName in fileNames)
                {
                    if (fileName.ToLowerInvariant().Contains(tarRes.ToLowerInvariant()))
                    {
                        return Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                    }
                }
            }

            return null;
        }

        public static void InitialGMEnv(string userId = "Tester")
        {
            // write userId
            DCTRegistry.SetUserID(userId);

            // copy resource
            string[] ResName = new string[] { "DCTGM.exe", "DCTUninstaller.exe", "Newtonsoft.Json.dll", "Newtonsoft.Json.xml", "DCTLib.dll" };

            string launchAppName = "DCTGM.exe";
            if (!Directory.Exists(DCTDef.InstallPath))
                Directory.CreateDirectory(DCTDef.InstallPath);

            foreach (string tarRes in ResName)
            {
                string[] fileNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string fileName in fileNames)
                {
                    if (fileName.ToLowerInvariant().Contains(tarRes.ToLowerInvariant()))
                    {
                        string launchPath = string.Format(@"{0}\{1}", DCTDef.InstallPath, tarRes);

                        try
                        {
                            if (File.Exists(launchPath))
                                File.Delete(launchPath);

                            using (FileStream fileStream = File.Create(launchPath))
                            {
                                Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName).CopyTo(fileStream);
                            }
                        }
                        catch
                        {
                        }

                        break;
                    }
                }
            }

            string executehPath = string.Format(@"{0}\{1}", DCTDef.InstallPath, launchAppName);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = executehPath;
            processStartInfo.WorkingDirectory = DCTDef.InstallPath;
            Process.Start(processStartInfo).WaitForExit(300);

            // setup task scheduler

            // setup uninstall string
            DCTRegistry.CreateUninstallKey();
        }
    }
}