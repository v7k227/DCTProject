using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCTLib;
using Newtonsoft.Json;
using System.Timers;
using System.Net;
using System.IO;

namespace DCTGM
{
    internal class Program
    {
        private static string seatNo;
        private static double interval = 5 * 60 * 1000; // 預設每5分鐘蒐集一次資訊

        private static void Main(string[] args)
        {
            SetupDebugEnvironment();

            seatNo = DCTRegistry.GetSeatno();
            DebugLog.DebugInfo(seatNo);

            // 避免軟體一起動就開始做事, 延遲3秒啟動主要服務
            Timer delay = new Timer(3000);
            delay.Elapsed += Delay_Elapsed;
            delay.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(60 * 60 * 1000);
            }
        }

        private static void Delay_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer delay = sender as Timer;
            delay.Stop();

            // 啟動蒐集資訊的服務
            Timer t = new Timer(1 * 1000);
            t.Elapsed += T_Elapsed_DataCollection;
            t.Start();
        }

        private static void T_Elapsed_DataCollection(object sender, ElapsedEventArgs e)
        {
            (sender as Timer).Interval = interval;

            if (!DCTRegistry.IsHardwareCollectionDone())
            {
                HardwareInformationCollection();
            }

            SoftwareInformationCollection();
        }

        private static void SetupDebugEnvironment()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                DebugLog.isDebugMode = (Environment.GetCommandLineArgs()[1].ToLowerInvariant() == "-d");
            }

            interval = (DebugLog.isDebugMode) ? 0.25 * 60 * 1000 : 5 * 60 * 1000;
        }

        private static void SoftwareInformationCollection()
        {
            GameInfo gameInfo = new GameInfo();

            if (DataCollection.SendGameInfo(seatNo, gameInfo.GetInformation()))
            {
                // do nothing
            }
        }

        private static void HardwareInformationCollection()
        {
            try
            {
                HardwareInfo HI = new HardwareInfo();

                if (DataCollection.SendHardwareInfo(seatNo, HI.GetInformation()))
                {
                    // 註記硬體資訊蒐集成功, 避免重複蒐集資料
                    DCTRegistry.SetHardwareCollectionDone();
                }
            }
            catch
            {
            }
        }
    }
}