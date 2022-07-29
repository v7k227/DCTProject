using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace DCTGM
{
    internal class HardwareInfoCollector
    {
        #region OS

        public static string OSInfo(string itemName)
        {
            try
            {
                using (ManagementClass c = new ManagementClass("Win32_OperatingSystem"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        return o[itemName].ToString();
                    }
                }
            }
            catch { }

            return string.Empty;
        }

        #endregion OS

        #region Process

        public static CPU ProcessorInfo()
        {
            CPU cpu = new CPU();

            try
            {
                using (ManagementClass c = new ManagementClass("Win32_Processor"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        cpu.Manufacturer = o["Manufacturer"].ToString();
                        cpu.Name = o["Name"].ToString();
                    }
                }
            }
            catch { }

            return cpu;
        }

        #endregion Process

        #region LogicalDisk

        //"Description"
        //"Size"
        //"FreeSpace"
        //"FileSystem"
        //"Compressed"
        public static LogicalDisk[] LogicalDiskInfo()
        {
            List<LogicalDisk> logicalDisks = new List<LogicalDisk>();

            try
            {
                List<string> blackList = new List<string>() { "card reader", "card  reader", "cardreader", "usb" };

                using (ManagementClass c = new ManagementClass("Win32_DiskDrive"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        LogicalDisk logicalDisk = new LogicalDisk();

                        string DeviceId = o["DeviceId"].ToString();
                        logicalDisk.Model = o["Model"].ToString();
                        logicalDisk.Brand = logicalDisk.Model.Split(' ')[0];
                        logicalDisk.DType = HasNoSeekPenalty(DeviceId) ? DriveType.SSD : DriveType.HDD;

                        bool isBlack = false;

                        foreach (string blackStr in blackList)
                        {
                            if (logicalDisk.Model.ToLowerInvariant().Contains(blackStr))
                            {
                                isBlack = true;
                                break;
                            }
                        }

                        if (!isBlack)
                            logicalDisks.Add(logicalDisk);
                    }

                    foreach (LogicalDisk ld in logicalDisks)
                    {
                        string model = ld.Model.ToLowerInvariant();
                        string[] modelSplit = ld.Model.Split(new char[] { ' ', '_', '-' });
                        if (modelSplit.Length >= 2)
                        {
                            if (modelSplit[0].ToLowerInvariant().StartsWith("st"))
                                ld.Brand = "Seagate";
                            else if (modelSplit[0].ToLowerInvariant().StartsWith("wd"))
                                ld.Brand = "WD";
                            else
                                ld.Brand = modelSplit[0];
                        }
                        else
                        {
                            bool isSeagate = model.StartsWith("st");
                            bool isHitachIBM = (model.StartsWith("ic") || model.StartsWith("hd") || model.StartsWith("hds"));
                            bool isWD = model.StartsWith("wd");
                            bool isSamsung = model.StartsWith("sp");
                            bool isMicron = model.StartsWith("micron");

                            if (isSeagate)
                                ld.Brand = "Seagate";
                            else if (isHitachIBM)
                                ld.Brand = "HitachI";
                            else if (isWD)
                                ld.Brand = "WD";
                            else if (isSamsung)
                                ld.Brand = "Samsung";
                            else
                                ld.Brand = "Unknown";
                        }
                    }
                }
            }
            catch
            {
            }

            return logicalDisks.ToArray();
        }

        private const uint FILE_SHARE_READ = 1;
        private const uint FILE_SHARE_WRITE = 2;
        private const uint OPEN_EXISTING = 3;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern SafeFileHandle CreateFileW(
            [MarshalAs(UnmanagedType.LPWStr)]
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        private const uint FILE_DEVICE_MASS_STORAGE = 0x2d;
        private const uint IOCTL_STORAGE_BASE = FILE_DEVICE_MASS_STORAGE;
        private const uint METHOD_BUFFERED = 0;
        private const uint FILE_ANY_ACCESS = 0;

        public static uint CTL_CODE(uint DeviceType, uint Function,
            uint Method, uint Access)
        {
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        private const uint StorageDeviceSeekPenaltyProperty = 7;
        private const uint PropertyStandardQuery = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct STORAGE_PROPERTY_QUERY
        {
            public uint PropertyId;
            public uint QueryType;

            [MarshalAs(UnmanagedType.ByValArray)]
            public byte[] AdditionalParameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEVICE_SEEK_PENALTY_DESCRIPTOR
        {
            public uint Version;
            public uint Size;

            [MarshalAs(UnmanagedType.U1)]
            public bool IncursSeekPenalty;
        }

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            ref STORAGE_PROPERTY_QUERY lpInBuffer,
            uint nInBufferSize,
            ref DEVICE_SEEK_PENALTY_DESCRIPTOR lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle(SafeFileHandle hDevice);

        private static bool HasNoSeekPenalty(string sDrive)
        {
            SafeFileHandle hDrive = CreateFileW(
                sDrive,
                0,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero);

            if (hDrive == null || hDrive.IsInvalid)
            {
                return false;
            }

            uint IOCTL_STORAGE_QUERY_PROPERTY = CTL_CODE(IOCTL_STORAGE_BASE, 0x500,
                METHOD_BUFFERED, FILE_ANY_ACCESS);

            STORAGE_PROPERTY_QUERY query_seek_penalty = new STORAGE_PROPERTY_QUERY()
            {
                PropertyId = StorageDeviceSeekPenaltyProperty,
                QueryType = PropertyStandardQuery
            };

            DEVICE_SEEK_PENALTY_DESCRIPTOR query_seek_penalty_desc =
                new DEVICE_SEEK_PENALTY_DESCRIPTOR();

            uint returned_query_seek_penalty_size = 0;

            bool query_seek_penalty_result = DeviceIoControl(
                hDrive,
                IOCTL_STORAGE_QUERY_PROPERTY,
                ref query_seek_penalty,
                (uint)Marshal.SizeOf(query_seek_penalty),
                ref query_seek_penalty_desc,
                (uint)Marshal.SizeOf(query_seek_penalty_desc),
                ref returned_query_seek_penalty_size,
                IntPtr.Zero);

            CloseHandle(hDrive);

            if (query_seek_penalty_result == false)
            {
                return false;
            }
            else
            {
                if (query_seek_penalty_desc.IncursSeekPenalty == false)
                {
                    return true;
                }
                else
                {
                    return false;
                };
            };
        }

        //

        #endregion LogicalDisk

        #region Motherboard

        public static string MotherboardInfo()
        {
            try
            {
                using (ManagementClass c = new ManagementClass("Win32_BaseBoard"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        return o["Manufacturer"].ToString();
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        public static ChassisTypes ChassisTypesInfo()
        {
            try
            {
                using (ManagementClass c = new ManagementClass("Win32_SystemEnclosure"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        UInt16[] ct = o["ChassisTypes"] as UInt16[];
                        ChassisTypes chassisTypes = (ChassisTypes)Enum.Parse(typeof(ChassisTypes), ct[0].ToString());
                        return chassisTypes;
                    }
                }
            }
            catch
            {
            }
            return ChassisTypes.Unknown;
            //Select * from Win32_SystemEnclosure
            //ChassisTypes

            //Case 1
            //    Wscript.Echo "Other"
            //Case 2
            //    Wscript.Echo "Unknown"
            //Case 3
            //    Wscript.Echo "Desktop"
            //Case 4
            //    Wscript.Echo "Low Profile Desktop"
            //Case 5
            //    Wscript.Echo "Pizza Box"
            //Case 6
            //    Wscript.Echo "Mini Tower"
            //Case 7
            //    Wscript.Echo "Tower"
            //Case 8
            //    Wscript.Echo "Portable"
            //Case 9
            //    Wscript.Echo "Laptop"
            //Case 10
            //    Wscript.Echo "Notebook"
            //Case 11
            //    Wscript.Echo "Handheld"
            //Case 12
            //    Wscript.Echo "Docking Station"
            //Case 13
            //    Wscript.Echo "All-in-One"
            //Case 14
            //    Wscript.Echo "Sub-Notebook"
            //Case 15
            //    Wscript.Echo "Space Saving"
            //Case 16
            //    Wscript.Echo "Lunch Box"
            //Case 17
            //    Wscript.Echo "Main System Chassis"
            //Case 18
            //    Wscript.Echo "Expansion Chassis"
            //Case 19
            //    Wscript.Echo "Sub-Chassis"
            //Case 20
            //    Wscript.Echo "Bus Expansion Chassis"
            //Case 21
            //    Wscript.Echo "Peripheral Chassis"
            //Case 22
            //    Wscript.Echo "Storage Chassis"
            //Case 23
            //    Wscript.Echo "Rack Mount Chassis"
            //Case 24
            //    Wscript.Echo "Sealed-Case PC"
            //Case Else
            //    Wscript.Echo "Unknown"
            //End Select
        }

        #endregion Motherboard

        #region GraphicCard

        public static GraphicCard[] GraphicCardInfo()
        {
            List<string> keyAMD = new List<string>(new string[]
            {
                "amd ", "ati ", "radeon"
            });
            List<string> keyNvidia = new List<string>(new string[]
            {
                "nvidia", "geforce", "gtx", "gts", "gt"
            });

            List<GraphicCard> graphicCards = new List<GraphicCard>();

            try
            {
                using (ManagementClass c = new ManagementClass("Win32_VideoController"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        GraphicCard graphicCard = new GraphicCard();
                        graphicCard.Name = o["Name"].ToString();
                        graphicCard.graphicCardBrand = GraphicCardBrand.UNKNOWN;

                        graphicCards.Add(graphicCard);
                    }

                    foreach (var graphicCard in graphicCards)
                    {
                        bool isFindAMD = keyAMD.AsEnumerable().Any((r) => graphicCard.Name.ToLowerInvariant().Contains(r.ToLowerInvariant()));
                        bool isFindNvidia = keyNvidia.AsEnumerable().Any((r) => graphicCard.Name.ToLowerInvariant().Contains(r.ToLowerInvariant()));
                        bool isIntel = graphicCard.Name.ToLowerInvariant().Contains("intel");

                        if (isFindAMD)
                            graphicCard.graphicCardBrand = GraphicCardBrand.AMD;
                        else if (isFindNvidia)
                            graphicCard.graphicCardBrand = GraphicCardBrand.NVIDIA;
                        else if (isIntel)
                            graphicCard.graphicCardBrand = GraphicCardBrand.INTEL;
                    }
                }
            }
            catch
            {
            }

            //c = new ManagementClass("Win32_PnPEntity");

            //foreach (ManagementObject o in c.GetInstances())
            //{
            //    if (o["Name"] != null)
            //    {
            //        string name = o["Name"].ToString().ToLowerInvariant();

            //        bool isFindAMD = keyAMD.AsEnumerable().Any((r) => name.Contains(r.ToLowerInvariant()));
            //        bool isFindNvidia = keyNvidia.AsEnumerable().Any((r) => name.Contains(r.ToLowerInvariant()));

            //        if (isFindAMD || isFindNvidia)
            //        {
            //            GraphicCard graphicCard = new GraphicCard();
            //            graphicCard.Name = o["Name"].ToString();
            //            graphicCards.Add(graphicCard);
            //        }
            //    }
            //}

            //graphicCards = graphicCards.Distinct().ToList();

            return graphicCards.ToArray();
        }

        #endregion GraphicCard

        #region Memory Size

        public static Memory[] MemoryInfo(out int memoryDevices)
        {
            List<Memory> memoryCollection = new List<Memory>();
            memoryDevices = 0;

            try
            {
                using (ManagementClass c = new ManagementClass("win32_PhysicalMemory"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        //int DeviceLocator = 0;
                        long Capacity = 0;

                        //int.TryParse(o["DeviceLocator"].ToString(), out DeviceLocator);
                        long.TryParse(o["Capacity"].ToString(), out Capacity);

                        Memory memory = new Memory(o["DeviceLocator"].ToString(), (int)Math.Ceiling(Capacity / 1024.0 / 1024.0 / 1024));
                        memoryCollection.Add(memory);
                    }
                }
            }
            catch
            {
            }

            try
            {
                using (ManagementClass c = new ManagementClass("win32_PhysicalMemoryArray"))
                {
                    foreach (ManagementObject o in c.GetInstances())
                    {
                        int.TryParse(o["MemoryDevices"].ToString(), out memoryDevices);
                    }
                }
            }
            catch
            {
            }

            return memoryCollection.ToArray();
        }

        #endregion Memory Size
    }
}