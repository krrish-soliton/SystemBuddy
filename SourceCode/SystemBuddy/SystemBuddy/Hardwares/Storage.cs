using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using System.Collections.Generic;
using System.Linq;

namespace SystemBuddy
{
    public class Storage : IHardwareInfo
    {
        public Storage()
        {
            Sensors.AddRange(new[] { Temperature, ReadRate, WriteRate, TotalActivity });
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public List<Partition> Partitions { get; } = new List<Partition>();

        public float TotalSize { get; set; }

        public int TotalAllocated { get; set; }

        public int TotalUnAllocated { get; set; }

        public int TotalUsed { get; set; }

        public int Health { get; set; }

        public IHardware Hardware { get; private set; }

        public SensorData Temperature { get; } = new SensorData() { Name = "Data Temp", UnitType = UnitType.TemperatureC };

        public SensorData ReadRate { get; } = new SensorData() { Name = "Read Rate", UnitType = UnitType.DataPerSecond };

        public SensorData WriteRate { get; } = new SensorData() { Name = "Write Rate", UnitType = UnitType.DataPerSecond };

        public SensorData TotalActivity { get; } = new SensorData() { Name = "Total Activity", UnitType = UnitType.Percentile };

        public List<SensorData> Sensors { get; } = new List<SensorData>();

        public void UpdateSensors()
        {
            foreach (var sensor in Sensors) sensor.Update();
        }

        public void Update(IHardware hardware)
        {
            if (hardware is AbstractStorage storage)
            {
                Name = storage.Name;
                Hardware = storage;
                ReadRate.ParentID = "Storage - " + hardware.Name;
                WriteRate.ParentID = "Storage - " + hardware.Name;
                TotalActivity.ParentID = "Storage - " + hardware.Name;
                Temperature.ParentID = "Storage - " + hardware.Name;
                ReadRate.Sensor = hardware.Sensors.FirstOrDefault(i => i.Name == "Read Rate");
                WriteRate.Sensor = hardware.Sensors.FirstOrDefault(i => i.Name == "Write Rate");
                TotalActivity.Sensor = hardware.Sensors.FirstOrDefault(i => i.Name == "Total Activity");
                Temperature.Sensor = hardware.Sensors.FirstOrDefault(i => i.SensorType == SensorType.Temperature);
            }
        }
    }
}
