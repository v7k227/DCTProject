using System;
using System.Collections.Generic;

namespace DCTLib
{
    public class DCTDef
    {
        /// <summary>
        /// For Remove previous version
        /// Example: When the App is use for DCT 2019, the set should include DCT 2018, DCT 2017...
        /// </summary>
        public static readonly List<string> lstOldInstallGUID = new List<string>()
        {
            "{498E851D-7EAD-4C2A-A957-467C065E6916}", // DCT 2018
            "{DCT-7FE1-47A2-95B6-E73AEE36B1CD}" // DCT 2019
        };

        /// <summary>
        /// force login password for engineer debug.
        /// </summary>
        public static readonly string forceloginpw = "qwas";

        /// <summary>
        /// the app name is use for system
        /// 1. install path
        /// 2. task schedule
        /// </summary>
        public static readonly string AppName = "DCT";

        public static readonly string RegApp = string.Format(@"Software\{0}", DCTDef.AppName);

        public static readonly string InstallPath = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);

        public static readonly string MemberFileName = "ui.csv";

        public static readonly string debugLogPath = string.Format(@"{0}\{1}.txt", InstallPath, "log");

        /// <summary>
        /// For Install the application
        /// 1. uninstall value in registry key
        /// 2. software value in registry key
        /// </summary>
        public static readonly string RegInstallGUID = "{DCT-D86C-4AED-A8B3-46DEE45FCCEF}";

        public static readonly string ValUserId = "UserId";
        public static readonly string ValDisplayIcon = string.Format(@"{0}\{1}.exe", InstallPath, "DCTGM");
        public static readonly string ValDisplayName = string.Format("{0} 認證程式", AppName);
        public static readonly string ValDisplayVersion = "DCT";
        public static readonly string ValPublisher = "DCT";
        public static readonly string ValUninstallString = string.Format("\"{0}\\{1}.exe\" \"{2}\" \"{3}\"", InstallPath, "DCTUninstaller", RegInstallGUID, ValDisplayIcon);

        public static readonly string ValHardwareDone = "HardwareDone";
    }
}