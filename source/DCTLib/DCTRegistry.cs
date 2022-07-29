using Microsoft.Win32;
using System.Linq;

namespace DCTLib
{
    public class DCTRegistry
    {
        private static RegistryKey GetUninstallKey()
        {
            try
            {
                return Registry.LocalMachine
                    .OpenSubKey("SOFTWARE", true)
                    .OpenSubKey("Microsoft", true)
                    .OpenSubKey("Windows", true)
                    .OpenSubKey("CurrentVersion", true)
                    .OpenSubKey("Uninstall", true);
            }
            catch
            {
                return null;
            }
        }

        public static void InitialReg()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey(DCTDef.RegApp);
            if (keyApp.ValueCount == 0)
            {
                keyApp.SetValue(DCTDef.ValUserId, "");
            }
        }

        public static bool IsLoggedIn()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey(DCTDef.RegApp);

            return !string.IsNullOrEmpty(keyApp.GetValue(DCTDef.ValUserId, "").ToString());
        }

        public static void SetUserID(string userId = "Tester")
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey(DCTDef.RegApp);
            keyApp.SetValue(DCTDef.ValUserId, userId);
        }

        public static bool ExistOldVersion()
        {
            RegistryKey keyUninstall = GetUninstallKey();

            return DCTDef.lstOldInstallGUID.Any(r => keyUninstall.OpenSubKey(r) != null);
        }

        public static void CreateUninstallKey()
        {
            RegistryKey keyUninstall = GetUninstallKey();

            keyUninstall = keyUninstall.CreateSubKey(DCTDef.RegInstallGUID);

            keyUninstall.SetValue("DisplayIcon", DCTDef.ValDisplayIcon);
            keyUninstall.SetValue("DisplayName", DCTDef.ValDisplayName);
            keyUninstall.SetValue("DisplayVersion", DCTDef.ValDisplayVersion);
            keyUninstall.SetValue("Publisher", DCTDef.ValPublisher);
            keyUninstall.SetValue("UninstallString", DCTDef.ValUninstallString);
            keyUninstall.SetValue("UserId", DCTDef.ValUserId);
        }

        public static void DeleteSystemKey()
        {
            try
            {
                RegistryKey SWKey = Registry.CurrentUser
                    .OpenSubKey("SOFTWARE", true);

                SWKey.DeleteSubKey(DCTDef.AppName);
            }
            catch
            {
            }

            try
            {
                RegistryKey keyUninstall = GetUninstallKey();

                keyUninstall.DeleteSubKey(DCTDef.RegInstallGUID);
            }
            catch
            {
            }
        }

        public static bool IsHardwareCollectionDone()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey(DCTDef.RegApp);

            return !string.IsNullOrEmpty(keyApp.GetValue(DCTDef.ValHardwareDone, "").ToString());
        }

        public static void SetHardwareCollectionDone()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey(DCTDef.RegApp);

            keyApp.SetValue(DCTDef.ValHardwareDone, "1");
        }

        public static string GetSeatno()
        {
            RegistryKey keyApp = Registry.CurrentUser.CreateSubKey(DCTDef.RegApp);

            return keyApp.GetValue(DCTDef.ValUserId, "").ToString();
        }
    }
}