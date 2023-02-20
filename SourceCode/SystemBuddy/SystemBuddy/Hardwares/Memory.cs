using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SystemBuddy
{
    public class Memory : IHardwareInfo
    {
        public Memory() { }

        public IHardware Hardware { get; private set; }

        public SensorData Size { get; } = new SensorData() { Name = "Memory", ParentID = "Memory", UnitType = UnitType.Custom, Suffix = " GB" };

        public string ID { get; private set; }

        public string Name { get; private set; }

        public List<SensorData> Sensors { get; } = new List<SensorData>();

        public void UpdateSensors()
        {
            foreach (var sensor in Sensors) sensor.Update();
        }

        public void Update(IHardware hardware)
        {
            if (hardware.HardwareType == HardwareType.Memory)
            {
                Hardware = hardware;
                var sensor = hardware.Sensors.FirstOrDefault(i => i.Name == "Memory Used");
                if (sensor == null) return;
                var totalSensor = hardware.Sensors.FirstOrDefault(i => i.Name == "Total Memory");
                if (totalSensor == null)
                {
                    var ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
                    Size.Total = ramCounter.NextValue() / 1024;
                    ramCounter.Dispose();
                }
                else Size.Total = totalSensor.Value.Value;
                Size.Sensor = sensor;
                Sensors.Add(Size);
            }
        }
    }
}
