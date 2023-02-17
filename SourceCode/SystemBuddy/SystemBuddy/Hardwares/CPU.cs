using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.CPU;
using System.Collections.Generic;
using System.Linq;

namespace SystemBuddy
{
    public class CPU : IHardwareInfo
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public SensorData PackageTemperature { get; } = new SensorData { Name = "CPU temp", UnitType = UnitType.TemperatureC };

        public Dictionary<string, SensorData> CoreTemperatures { get; } = new Dictionary<string, SensorData>();

        public SensorData Load { get; } = new SensorData { Name = "CPU Load", UnitType = UnitType.Percentile };

        public Dictionary<string, SensorData> CoreLoads { get; } = new Dictionary<string, SensorData>();

        public IHardware Hardware { get; private set; }

        public int Cores { get; private set; }

        public int Threads { get; private set; }

        public List<SensorData> Sensors { get; } = new List<SensorData>();

        public void UpdateSensors()
        {
            foreach (var sensor in Sensors)
            {
                sensor.Update();
            }
        }

        public void Update(IHardware hardware)
        {
            if (!(hardware is GenericCpu cpu)) return;
            Cores = cpu.CpuId.Length;
            Threads = cpu.CpuId[0].Length;
            Hardware = hardware;
            Load.Sensor = Hardware.Sensors.FirstOrDefault(i => i.Name == "CPU Total");
            PackageTemperature.Sensor = Hardware.Sensors.FirstOrDefault(i => i.Name == "CPU Package");
            for (int i = 0; i < Cores; i++)
            {
                ISensor sensor = Hardware.Sensors.FirstOrDefault(s => s.Name == "CPU Core #" + (i + 1));
                string sensorID = sensor.Identifier.ToString();
                if (!CoreTemperatures.ContainsKey(sensorID)) CoreTemperatures.Add(sensorID, new SensorData { Name = $"Core {i} Temp", UnitType = UnitType.TemperatureC, Identifier = sensor.Identifier.ToString() });
                CoreTemperatures[sensor.Identifier.ToString()].Sensor = sensor;
                for (int j = 0; j < Threads; j++)
                {
                    sensor = Hardware.Sensors.FirstOrDefault(s => s.Name == "CPU Core #" + (i + 1) + " Thread #" + (j + 1));
                    sensorID = sensor.Identifier.ToString();
                    if (!CoreLoads.ContainsKey(sensorID)) CoreLoads.Add(sensorID, new SensorData { Name = $"Core {i} Thread {j}", UnitType = UnitType.Percentile, Identifier = sensor.Identifier.ToString() });
                    CoreLoads[sensorID].Sensor = sensor;
                }
            }
            Sensors.AddRange(CoreLoads.Values);
            Sensors.Add(Load);
            Sensors.AddRange(CoreTemperatures.Values);
            Sensors.Add(PackageTemperature);
        }
    }
}
