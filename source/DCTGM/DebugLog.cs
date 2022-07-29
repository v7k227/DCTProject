using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DCTLib;

namespace DCTGM
{
    internal class DebugLog
    {
        public static bool isDebugMode = false;

        public static void DebugInfo(string msg)
        {
            if (isDebugMode)
            {
                using (StreamWriter streamWriter = new StreamWriter(DCTDef.debugLogPath, true))
                {
                    streamWriter.WriteLine(msg);
                    streamWriter.Flush();
                }
            }
        }
    }
}