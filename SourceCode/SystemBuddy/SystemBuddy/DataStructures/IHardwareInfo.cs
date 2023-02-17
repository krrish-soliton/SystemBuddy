using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;

namespace SystemBuddy
{
    public interface IHardwareInfo
    {
        string ID { get; }

        string Name { get; }

        IHardware Hardware { get; }

        List<SensorData> Sensors { get; }

        void Update(IHardware hardware);
    }
}
