using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace SystemBuddy
{
    public class HardwareHandler : IDisposable
    {
        public HardwareHandler()
        {
            timer.Elapsed += OnTimerElapsed;
        }

        public event EventHandler ValueUpdated;

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) => Monitor();

        Computer computer;

        public void Dispose() => computer.Close();

        private PersistentSettings _settings;
        UpdateVisitor updateVisitor;
        public void Initialize()
        {
            _settings = new PersistentSettings();
            _settings.Load(Path.ChangeExtension("Settings", ".config"));
            computer = new Computer(_settings);
            updateVisitor = new UpdateVisitor();
            computer.Open();
            computer.IsCpuEnabled = true;
            computer.IsGpuEnabled = true;
            computer.IsMemoryEnabled = true;
            computer.IsMotherboardEnabled = true;
            computer.IsControllerEnabled = true;
            computer.IsNetworkEnabled = true;
            computer.IsStorageEnabled = true;
            computer.IsBatteryEnabled = true;
            computer.IsPsuEnabled = true;
            Monitor();
            timer.Start();
        }

        public CPU CPUInfo { get; private set; } = new CPU();

        public Memory MemInfo { get; private set; } = new Memory();

        public Dictionary<string, Storage> Storages { get; private set; } = new Dictionary<string, Storage>();


        public Timer timer = new Timer() { Interval = 1000 };

        public string Info { get; private set; }

        public List<SensorData> Sensors { get; private set; } = new List<SensorData>();

        private bool isVisited;

        public void Monitor()
        {
            computer.Accept(updateVisitor);
            if (!isVisited)
            {
                Sensors.Clear();
                CPUInfo.Update(computer.Hardware.FirstOrDefault(i => i.HardwareType == HardwareType.Cpu));
                Sensors.AddRange(CPUInfo.Sensors);
                MemInfo.Update(computer.Hardware.FirstOrDefault(i => i.HardwareType == HardwareType.Memory));
                Sensors.AddRange(MemInfo.Sensors);
                foreach (var hardware in computer.Hardware.Where(i => i.HardwareType == HardwareType.Storage))
                {
                    if (!Storages.ContainsKey(hardware.Identifier.ToString()))
                    {
                        Storages.Add(hardware.Identifier.ToString(), new Storage() { ID = hardware.Identifier.ToString() });
                    }
                    Storages[hardware.Identifier.ToString()].Update(hardware);
                    Sensors.AddRange(Storages[hardware.Identifier.ToString()].Sensors);
                }
                isVisited = true;
            }
            foreach (var sensor in Sensors) sensor.Update();
            PrintData();
            ValueUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void PrintData()
        {
            StringBuilder data = new StringBuilder();
            foreach (IHardware hardware in computer.Hardware)
            {
                data.AppendLine($"Hardware: {hardware.Name}");
                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    data.AppendLine($"\tSubhardware: {subhardware.Name}");
                    data.AppendLine();
                    foreach (ISensor sensor in subhardware.Sensors)
                        data.AppendLine($"\t\tSensor: {sensor.Name}.{sensor.SensorType}, value: {sensor.Value}");
                }
                foreach (ISensor sensor in hardware.Sensors)
                    data.AppendLine($"\t\tSensor: {sensor.Name}.{sensor.SensorType}, value: {sensor.Value}");
            }
            Info = data.ToString();
        }
    }
}
