using System.Diagnostics;
using System.IO;
using System.Text;

namespace DCTLib
{
    public class DCTTaskScheduler
    {
        public static void CreateTask()
        {
            if (File.Exists("DCT.xml"))
                File.Delete("DCT.xml");
            string xmlTaskScheduler = DCTLib.DCTTaskScheduler.GetTaskScheduler(DCTDef.InstallPath + "\\DCTGM.exe");
            File.WriteAllText(DCTDef.InstallPath + "\\DCT.xml", xmlTaskScheduler);
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "Schtasks";
            myProcess.StartInfo.Arguments = "/create /TN \"" + DCTDef.AppName + "\" /XML \"" + DCTDef.InstallPath + "\\DCT.xml" + "\"";
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            myProcess.WaitForExit();
        }

        public static void DeleteTask()
        {
            // DELETE TASK SCHEDULER
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "Schtasks";
            myProcess.StartInfo.Arguments = string.Format("/Delete /TN \"{0}\" /F", DCTDef.AppName);
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.Start();
            myProcess.WaitForExit();
        }

        public static string GetTaskScheduler(string LauncherFullName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version = \"1.0\" encoding = \"UTF-16\" ?>");

            sb.AppendLine("<Task version = \"1.2\" xmlns = \"http://schemas.microsoft.com/windows/2004/02/mit/task\" >");

            sb.AppendLine("<RegistrationInfo >");

            //sb.AppendLine("<Date > 2018-09-10T15:10:04.6442419 </Date >");

            //sb.AppendLine("<Author > ITNB000054\\Sean Wu</Author >");

            sb.AppendLine(string.Format("<Description > {0} utility </Description >", DCTDef.AppName));

            sb.AppendLine(string.Format("<URI >\\{0} </URI >", DCTDef.AppName));

            sb.AppendLine("</RegistrationInfo >");

            sb.AppendLine("<Triggers >");

            sb.AppendLine("<LogonTrigger >");

            sb.AppendLine("<Enabled > true </Enabled >");

            sb.AppendLine("</LogonTrigger >");

            sb.AppendLine("<BootTrigger >");

            sb.AppendLine("<Enabled > true </Enabled >");

            sb.AppendLine("</BootTrigger >");

            sb.AppendLine("</Triggers >");

            sb.AppendLine("<Principals >");

            sb.AppendLine("<Principal id = \"Author\" >");

            //sb.AppendLine("<UserId > S-1-5-21-3869865221-2769262067-862642286-1001 </UserId >");

            sb.AppendLine("<LogonType > InteractiveToken </LogonType >");

            sb.AppendLine("<RunLevel > HighestAvailable </RunLevel >");

            sb.AppendLine("</Principal >");

            sb.AppendLine("</Principals >");

            sb.AppendLine("<Settings >");

            sb.AppendLine("<MultipleInstancesPolicy > IgnoreNew </MultipleInstancesPolicy >");

            sb.AppendLine("<DisallowStartIfOnBatteries > true </DisallowStartIfOnBatteries >");

            sb.AppendLine("<StopIfGoingOnBatteries > true </StopIfGoingOnBatteries >");

            sb.AppendLine("<AllowHardTerminate > true </AllowHardTerminate >");

            sb.AppendLine("<StartWhenAvailable > true </StartWhenAvailable >");

            sb.AppendLine("<RunOnlyIfNetworkAvailable > false </RunOnlyIfNetworkAvailable >");

            sb.AppendLine("<IdleSettings >");

            sb.AppendLine("<StopOnIdleEnd > true </StopOnIdleEnd >");

            sb.AppendLine("<RestartOnIdle > false </RestartOnIdle >");

            sb.AppendLine("</IdleSettings >");

            sb.AppendLine("<AllowStartOnDemand > true </AllowStartOnDemand >");

            sb.AppendLine("<Enabled > true </Enabled >");

            sb.AppendLine("<Hidden > true </Hidden >");

            sb.AppendLine("<RunOnlyIfIdle > false </RunOnlyIfIdle >");

            sb.AppendLine("<WakeToRun > false </WakeToRun >");

            sb.AppendLine("<ExecutionTimeLimit > PT72H </ExecutionTimeLimit >");

            sb.AppendLine("<Priority > 7 </Priority >");

            sb.AppendLine("</Settings >");

            sb.AppendLine("<Actions Context = \"Author\" >");

            sb.AppendLine("<Exec >");

            sb.AppendLine(string.Format("<Command > \"{0}\" </Command >", LauncherFullName));

            sb.AppendLine("</Exec >");

            sb.AppendLine("</Actions >");

            sb.AppendLine("</Task >");

            return sb.ToString();
        }
    }
}