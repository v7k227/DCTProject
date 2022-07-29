using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DCTLib;
using System.Linq;

namespace DCTUninstaller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string UninstallGUID = args[0];
            string ExeName = Path.GetFileName(args[1]);
            string processName = Path.GetFileNameWithoutExtension(args[1]);
            string ExePath = Directory.GetParent(args[1]).FullName;

            try
            {
                Process[] processes = Process.GetProcesses();

                processes.FirstOrDefault(p => p.ProcessName.Contains(processName))?.Kill();

                Thread.Sleep(500);
                Directory.Delete(ExePath, true);
            }
            catch
            {
                //Console.WriteLine(eee.Message);
            }

            DCTTaskScheduler.DeleteTask();
            DCTRegistry.DeleteSystemKey();
            Console.WriteLine(string.Format("{0} 已經成功從您的裝置內移除, 視窗將於 3 秒後關閉", DCTDef.AppName));
            for (int t = 3; t > 0; t--)
            {
                Thread.Sleep(1000); //睡一秒
            }
        }
    }
}