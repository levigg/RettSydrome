using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.Gaze.Core;
using System.Management;
using Microsoft.Win32;

namespace RettSydrome
{
    class CheckSerialNumber
    {
        Uri uri;
        EyeTracker tracker;
        public List<string> serialNumbers;
        public List<string> RexSNs;
        public List<string> IS4SNs;
        //public List<string> limitSNs;
        private string deviceSN;
        //private bool isNetLimitTime = false;
        string regDir = "HKEY_CURRENT_USER\\SOFTWARE\\EyePlayer";

        public int CheckTimeLimit(DateTime DT)//0=SN; 1=Time; 2=pass;
        {
            Global.expirationDate = DT;
            bool SNisExist = false;

            //IS4
            IS4SNs = new List<string>();
            //IS4SNs.Add("IS404-100106431951");
            //IS4SNs.Add("IS404-100106331951");

            serialNumbers = new List<string>();
            //EYEXC
            //serialNumbers.Add("EYEXC-030114622213");
            //EYEX2
            //serialNumbers.Add("EYEX2-030135823232");
            //SENTRY
            //serialNumbers.Add("6904130020021501308");
            //PCEYEGO
            serialNumbers.Add("PCEGO-030155931422");
            //serialNumbers.Add("PCEGO-010176119754");
            serialNumbers.Add("PCEGO-030155832312");

            if (Global.isIS4Device)
            {
                foreach (string str in IS4SNs)
                {
                    if (str == deviceSN) SNisExist = true;
                }
            }
            else
            {
                foreach (string str in serialNumbers)
                {
                    if (str == tracker.GetDeviceInfo().SerialNumber) SNisExist = true;
                }
            }

            if (SNisExist)
            {
                if (uri != null)
                {
                    uri = null;
                    tracker.StopTracking();
                    tracker.Disconnect();
                    tracker.Dispose();
                }
                TimeZoneInfo timeZoneInfo = new TimeZoneInfo();
                for (int i = 0; i < 5; i++)
                {
                    if (timeZoneInfo.OnRefresh())
                    {
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                if ((Global.expirationDate - timeZoneInfo.oUTC).Days < (Global.expirationDate - DateTime.Now).Days)
                    Global.expirationDays = (Global.expirationDate - timeZoneInfo.oUTC).Days;
                else
                    Global.expirationDays = (Global.expirationDate - DateTime.Now).Days;

                //expiration date
                RegistryExpirationDay();

                if (Global.expirationDate.Year >= DateTime.Now.Year && DateTime.Now.Year >= 2016)
                {
                    if (Global.expirationDate.Year == DateTime.Now.Year)
                    {
                        if (Global.expirationDate.DayOfYear < DateTime.Now.DayOfYear) return 1;
                    }
                }
                else
                {
                    return 1;
                }
                if ((int)Registry.GetValue(regDir, "ExpirationDays", -1) < 0) return 1;
            }
            else
            {
                return 0;
            }
            return 2;
        }
        private void RegistryExpirationDay()
        {
            if (Registry.GetValue(regDir, "ExpirationDays", "Not exist") == "Not exist")
            {
                Registry.CurrentUser.CreateSubKey("EyePlayer\\ExpirationDays");
                Registry.SetValue(regDir, "ExpirationDays", Global.expirationDays.ToString(), RegistryValueKind.DWord);
            }
            else
            {
                if ((int)Registry.GetValue(regDir, "ExpirationDays", -1) > Global.expirationDays)
                {
                    Registry.SetValue(regDir, "ExpirationDays", Global.expirationDays.ToString(), RegistryValueKind.DWord);
                }
            }
        }

        public bool CheckDefaultSN()
        {
            //NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //List<string> macList = new List<string>();
            //foreach (var nic in nics)
            //{
            //    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            //    {
            //        macList.Add(nic.GetPhysicalAddress().ToString());
            //    }
            //}
            //if (Registry.GetValue(regDir, "Ethernet", "Not exist") == "Not exist")     Registry.CurrentUser.CreateSubKey("EyePlayer\\Ethernet");
            //Registry.SetValue(regDir, "Ethernet", macList[0], RegistryValueKind.String);
            try
            {
                if (Registry.GetValue(regDir, "device_SN", "Not exist") == "Not exist")
                {
                    Registry.CurrentUser.CreateSubKey("EyePlayer\\Device_SN");
                    Registry.SetValue(regDir, "Device_SN", deviceSN, RegistryValueKind.String);
                }
                else
                {
                    if (Registry.GetValue(regDir, "Device_SN", "Not exist") != deviceSN)
                    {
                        Console.WriteLine("device deferenet!");
                        Registry.SetValue(regDir, "Device_SN", deviceSN, RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //IS4
            IS4SNs = new List<string>();
            IS4SNs.Add("IS404-100106431951");//玠佑
            IS4SNs.Add("IS404-100106331951");//demo
            IS4SNs.Add("IS404-100106042701");
            IS4SNs.Add("IS404-100106140761");//demo
            IS4SNs.Add("IS404-100106044541");//demo
            IS4SNs.Add("IS404-100106441781");
            IS4SNs.Add("IS404-100107002943");
            IS4SNs.Add("IS404-100107003943");
            IS4SNs.Add("IS404-100107004903");
            IS4SNs.Add("IS404-100107006963");
            IS4SNs.Add("IS404-100107006993");
            IS4SNs.Add("IS404-100107005953");
            IS4SNs.Add("IS404-100107005963");
            IS4SNs.Add("IS404-100107005973");
            IS4SNs.Add("IS404-100107104943");
            IS4SNs.Add("IS404-100107104973");
            IS4SNs.Add("IS404-100107007963");
            IS4SNs.Add("IS404-100107008923");
            IS4SNs.Add("IS404-100107009973");
            IS4SNs.Add("IS404-100107009983");
            IS4SNs.Add("IS404-100107009993");
            IS4SNs.Add("IS404-100107105963");
            IS4SNs.Add("IS404-100107106923");
            IS4SNs.Add("IS404-100107200973");
            IS4SNs.Add("IS404-100107302943");
            IS4SNs.Add("IS404-100107307963");
            IS4SNs.Add("IS404-100106343033");

              //REX
            RexSNs = new List<string>();
            RexSNs.Add("tet-usb://rexdl-030104737754/");
            RexSNs.Add("tet-usb://rexdl-030104738724/");
            RexSNs.Add("tet-usb://rexdl-030104737784/");
            RexSNs.Add("tet-usb://rexdl-030104736794/");

            serialNumbers = new List<string>();
            //EYEXC
            serialNumbers.Add("EYEXC-030114622213");//demo
            //EYEX2
            serialNumbers.Add("EYEX2-030135823232");
            serialNumbers.Add("EYEX2-030145125552");
            serialNumbers.Add("EYEX2-030145125542");
            serialNumbers.Add("EYEX2-030135523271");
            serialNumbers.Add("EYEX2-030135522271");
            serialNumbers.Add("EYEX2-030115207385");//大陸
            serialNumbers.Add("EYEX2-030115208375");//大陸
            serialNumbers.Add("EYEX2-030176701583");//2016.8
            serialNumbers.Add("EYEX2-030186919085");//2016.8
            serialNumbers.Add("EYEX2-030196813035");//2016.8
            serialNumbers.Add("EYEX2-030196813005");//2016.8
            serialNumbers.Add("EYEX2-030176701593");//2016.8
            serialNumbers.Add("EYEX2-030135525241");//蔡介立
            serialNumbers.Add("EYEX2-030135522211");
            serialNumbers.Add("EYEX2-031136332165");//新竹特教
            serialNumbers.Add("EYEX2-031136331175");//澳門
            serialNumbers.Add("EYEX2-031146048352");
            serialNumbers.Add("EYEX2-031146046362");
            serialNumbers.Add("EYEX2-031146046382"); 

            serialNumbers.Add("EYEX2-031126616523");
            serialNumbers.Add("EYEX2-031126417523");
            serialNumbers.Add("EYEX2-031126514503");
            serialNumbers.Add("EYEX2-031106016075");
            serialNumbers.Add("EYEX2-031106018055");
            serialNumbers.Add("EYEX2-031136639251");
            serialNumbers.Add("EYEX2-031136731231");
            serialNumbers.Add("EYEX2-031116713532");
            serialNumbers.Add("EYEX2-031126411583");
            serialNumbers.Add("EYEX2-031116617532");
            serialNumbers.Add("EYEX2-031126419503");
            serialNumbers.Add("EYEX2-031126419523");
            serialNumbers.Add("EYEX2-031126513583");
            serialNumbers.Add("EYEX2-031116617542");
            serialNumbers.Add("EYEX2-031126411553");
            serialNumbers.Add("EYEX2-031126916504");
            serialNumbers.Add("EYEX2-031136732201");
            serialNumbers.Add("EYEX2-031126514533");
            serialNumbers.Add("EYEX2-031126414553");
            serialNumbers.Add("EYEX2-031126419553");

            //
            serialNumbers.Add("6904130020021501308");//demo
            serialNumbers.Add("6904130020021501140");
            serialNumbers.Add("6904130020021501717");
            serialNumbers.Add("6904130020031501903");//晁禾醫療
            serialNumbers.Add("6904130020021501140");//晁禾醫療
            serialNumbers.Add("6904130020021501717");//板橋個案
            serialNumbers.Add("6904130020101504486");
            serialNumbers.Add("6904130020101504573");
            serialNumbers.Add("6904130020101504496");
            serialNumbers.Add("6904130020101504463");//立維
            serialNumbers.Add("6904130020101503809");

            serialNumbers.Add("6904130020141505064");
            serialNumbers.Add("6904130020141505846");
            serialNumbers.Add("6904130020101504148");
            serialNumbers.Add("6904130020101504113");
            serialNumbers.Add("6904130020101504261");
            serialNumbers.Add("6904130020101504478");
            serialNumbers.Add("6904130020101504079");
            serialNumbers.Add("6904130020101504257");
            serialNumbers.Add("6904130020101504480");
            serialNumbers.Add("6904130020101504504");
            serialNumbers.Add("6904130020021501308");


            //PCEyeGo
            serialNumbers.Add("PCEGO-010176119754");
            serialNumbers.Add("PCEGO-030165248671");
            serialNumbers.Add("PCEGO-030155931422");

            //PCEXP
            serialNumbers.Add("PCEEX-030105528222");
            serialNumbers.Add("PCEEX-030106910962");

            //PCEyeMini
            serialNumbers.Add("PCE1M-03011662582");
            //if (!Initialize()) return false;

            if (Global.isIS4Device)
            {
                foreach (string str in IS4SNs)
                {
                    if (str == deviceSN) return true;
                }
            }
            else
            {
                foreach (string str in RexSNs)
                {
                    if (uri.AbsoluteUri == str)
                    {
                        if (uri != null)
                        {
                            uri = null;
                            tracker.StopTracking();
                            tracker.Disconnect();
                            tracker.Dispose();
                        }
                        return true;
                    }
                }
                foreach (string str in serialNumbers)
                {
                    if (str == tracker.GetDeviceInfo().SerialNumber)
                    {
                        if (uri != null)
                        {
                            uri = null;
                            tracker.StopTracking();
                            tracker.Disconnect();
                            tracker.Dispose();
                        }
                        return true;
                    }
                }

            }
            return false;
        }

        public bool Initialize()
        {
            try
            {
                System.Management.ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\cimv2");
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PnPEntity");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();
                foreach (ManagementObject Obj in queryCollection)
                {
                    if (Obj["Name"] != null && Obj["Name"].ToString().Contains("EyeChip"))
                    {
                        deviceSN = Obj["DeviceID"].ToString().Split('\\')[Obj["DeviceID"].ToString().Split('\\').Length - 1].ToString();
                        if (deviceSN.Split('-').Length > 1)
                        {
                            if (deviceSN.Split('-')[0] == "IS404" || deviceSN.Split('-')[0] == "PCE1M")
                            {
                                Global.isIS4Device = true;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get EyeChip Error: " + ex.ToString());
            }
            try
            {
                uri = new EyeTrackerCoreLibrary().GetConnectedEyeTracker();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetConnectedEyeTracker ERROR:" + ex.ToString());
            }
            if (uri == null)
            {
                return false;
            }
            tracker = new EyeTracker(uri);
            var thread = new System.Threading.Thread(() =>
            {
                try
                {
                    tracker.RunEventLoop();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EyeTracker RunEventLoop ERROR:" + ex.ToString());
                }
            }
            );
            thread.Start();
            tracker.Connect();
            return true;
        }
    }
}
