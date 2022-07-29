using Newtonsoft.Json.Linq;

namespace DCTGM
{
    public enum GraphicCardBrand
    {
        UNKNOWN, NVIDIA, INTEL, AMD
    }

    public enum ChassisTypes
    {
        Other = 1,
        Unknown,
        Desktop,
        Low_Profile_Desktop,
        Pizza_Box,
        Mini_Tower,
        Tower,
        Portable,
        Laptop,
        Notebook,
        Handheld,
        Docking_Station,
        All_in_One,
        Sub_Notebook,
        Space_Saving,
        Lunch_Box,
        Main_System_Chassis,
        Expansion_Chassis,
        Sub_Chassis,
        Bus_Expansion_Chassis,
        Peripheral_Chassis,
        Storage_Chassis,
        Rack_Mount_Chassis,
        Sealed_Case_PC
    }

    public class GraphicCard
    {
        public string Name;
        public GraphicCardBrand graphicCardBrand;
    }

    public enum DriveType
    {
        SSD, HDD
    }

    public class LogicalDisk
    {
        public DriveType DType;
        public string Brand;
        public string Model;
    }

    public class CPU
    {
        public string Name;
        public string Manufacturer;
    }

    public class Memory
    {
        public Memory(string deviceLocator, int capacity)
        {
            DeviceLocator = deviceLocator;
            Capacity = capacity;
        }

        public string DeviceLocator;
        public int Capacity;
    }

    internal class HardwareInfo
    {
        private GraphicCard[] graphicCard;
        private LogicalDisk[] logicalDisk;
        private CPU cpu;
        private string Motherboard;
        private ChassisTypes chassisTypes;
        private Memory[] memory;
        private int memoryDevices;

        public HardwareInfo()
        {
            logicalDisk = HardwareInfoCollector.LogicalDiskInfo();
            cpu = HardwareInfoCollector.ProcessorInfo();
            Motherboard = HardwareInfoCollector.MotherboardInfo();
            chassisTypes = HardwareInfoCollector.ChassisTypesInfo();
            graphicCard = HardwareInfoCollector.GraphicCardInfo();
            memory = HardwareInfoCollector.MemoryInfo(out memoryDevices);
        }

        public JObject GetInformation()
        {
            JProperty jCPU = new JProperty("cpu",
                        new JObject(
                            new JProperty("name", cpu.Name),
                            new JProperty("manufacturer", cpu.Manufacturer)
                        )
                    );

            JArray jaGraphics = new JArray();
            for (int i = 0; i < graphicCard.Length; i++)
            {
                jaGraphics.Add(
                    new JObject(
                        new JProperty("name", graphicCard[i].Name),
                        new JProperty("brand", graphicCard[i].graphicCardBrand.ToString())
                        )
                    );
            }
            JProperty jGraphics = new JProperty("graphCards", jaGraphics);

            JArray jaDisks = new JArray();
            for (int i = 0; i < logicalDisk.Length; i++)
            {
                jaDisks.Add(
                    new JObject(
                        new JProperty("type", logicalDisk[i].DType),
                        new JProperty("brand", logicalDisk[i].Brand),
                        new JProperty("model", logicalDisk[i].Model)
                        )
                    );
            }
            JProperty jDisks = new JProperty("disks", jaDisks);

            JArray jMemories = new JArray();
            for (int i = 0; i < memory.Length; i++)
            {
                jMemories.Add(
                    new JObject(
                        new JProperty("deviceLocator", memory[i].DeviceLocator),
                        new JProperty("capacity", memory[i].Capacity)
                        )
                    );
            }
            JProperty jMemory = new JProperty("memory", jMemories);

            JProperty jMotherBoard = new JProperty("motherBoard", new JObject(
                        new JProperty("Motherboard", Motherboard),
                        new JProperty("chassisTypes", chassisTypes.ToString()),
                        new JProperty("memoryDevices", memoryDevices)
                        ));

            JObject jObject = new JObject();

            jObject.Add(jCPU);
            jObject.Add(jGraphics);
            jObject.Add(jDisks);
            jObject.Add(jMemory);
            jObject.Add(jMotherBoard);

            return jObject;
        }
    }
}