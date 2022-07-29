using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DCTGM
{
    internal class DataCollection
    {
        public static bool SendHardwareInfo(string id, JObject jResult)
        {
            return SendToServer(string.Format("api/hardware/{0}", id), jResult);
        }

        public static bool SendGameInfo(string id, JObject jResult)
        {
            return SendToServer(string.Format("api/games/{0}", id), jResult);
        }

        private static bool SendToServer(string api, JObject jResult)
        {
            try
            {
                string sResult = JsonConvert.SerializeObject(jResult, Formatting.Indented);

                DebugLog.DebugInfo(sResult);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(api);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(sResult);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    DebugLog.DebugInfo(result);
                    if (result.ToLowerInvariant().Contains("ok"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }
    }
}