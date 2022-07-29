using System.Collections.Generic;
using System.Diagnostics;

namespace DCTGM
{
    internal class GameInfoCollector
    {
        public static bool CheckSteam()
        {
            return GetProcessByName("Steam.exe");
        }

        public static bool CheckOrigin()
        {
            return GetProcessByName("Origin.exe");
        }

        public static bool CheckBlizzard()
        {
            return GetProcessByName("Battle.net.exe");
        }

        public static bool CheckUbisoft()
        {
            return GetProcessByName("upc.exe");
        }

        public static bool CheckGarena()
        {
            return GetProcessByName("Garena.exe");
        }

        private static bool GetProcessByName(List<string> names)
        {
            bool isFind = false;

            foreach (string name in names)
            {
                if (GetProcessByName(name))
                {
                    isFind = true;
                    break;
                }
            }

            return isFind;
        }

        private static bool GetProcessByName(string name)
        {
            bool isFind = false;

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileName.ToLowerInvariant().Contains(name.ToLowerInvariant()))
                    {
                        isFind = true;
                        break;
                    }
                }
                catch
                {
                }
            }

            return isFind;
        }
    }
}