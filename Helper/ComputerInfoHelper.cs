//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Management;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;

//namespace Wemew.Program.Assets.utility
//{
//    public class CountAllHelper
//    {
//        static public void Send(string text)
//        {
//            //string url = $"{Consts.Apis.SendLocalInfo}?barid={Program.LoginResProtocol.Id}&text={text}";

//            //var request = WebRequest.Create(url);

//            //request.BeginGetResponse(null, null);
//        }

//        static public string ComputerInfoResult()
//        {
//            return $"Cpu:{GetCPUName().Replace(" ", "")}_" +
//            $"Mem:{GetPhysicalMemory().Replace(" ","")}_" +
//            $"Sys:{GetSystemType().Replace(" ", "")}";
//        }

//        public static string GetCPUVersion()
//        {
//            string st = "";
//            ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * from Win32_Processor");
//            foreach (ManagementObject mo in mos.Get())
//            {
//                st = mo["Version"].ToString();
//            }
//            return st;
//        }

//        static public string GetCPUName()
//        {
//            string st = "";
//            ManagementObjectSearcher driveID = new ManagementObjectSearcher("Select * from Win32_Processor");
//            foreach (ManagementObject mo in driveID.Get())
//            {
//                st = mo["Name"].ToString();
//            }
//            return st;
//        }

//        static public string GetPhysicalMemory()
//        {
//            string st = "";
//            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
//            ManagementObjectCollection moc = mc.GetInstances();
//            foreach (ManagementObject mo in moc)
//            {
//                long count = long.Parse(mo["TotalPhysicalMemory"].ToString());
//                st = (count / 1024 / 1024 / 1024) + " GB";
//            }
//            return st;
//        }

//        static public string GetSystemType()
//        {
//            string st = "";
//            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
//            ManagementObjectCollection moc = mc.GetInstances();
//            foreach (ManagementObject mo in moc)
//            {
//                st = mo["Caption"].ToString();
//            }
//            return st;
//        }
//    }

//}
