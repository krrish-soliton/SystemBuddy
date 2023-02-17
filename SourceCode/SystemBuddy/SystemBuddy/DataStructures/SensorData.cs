using LibreHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SystemBuddy
{
    public class SensorData : INotifyPropertyChanged
    {
        public SensorData() => Converter = ConvertUnit;

        private float current;
        private Func<UnitType, float, float, string, string> converter;

        public float Current
        {
            get => current;
            set
            {
                current = value;
                OnPropertyChanged("CurrentValue");
                Min = Math.Min(value, Min);
                OnPropertyChanged("MinValue");
                Max = Math.Max(value, Max);
                OnPropertyChanged("MaxValue");
            }
        }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public float Total { get; set; }

        public float Min { get; private set; }

        public float Max { get; private set; }

        public string Suffix { get; set; }

        public string CurrentValue { get => ToString(); }

        public string MinValue { get { return converter?.Invoke(UnitType, Min, Total, Suffix); } }

        public string MaxValue { get { return converter?.Invoke(UnitType, Max, Total, Suffix); } }

        public Func<UnitType, float, float, string, string> Converter { get => converter; set => converter = value ?? ConvertUnit; }

        public bool IsDrillDown { get; set; }

        public UnitType UnitType { get; set; }


        private ISensor sensor;

        public ISensor Sensor
        {
            get => sensor;
            set
            {
                sensor = value;
                Update();
            }
        }

        public void Update() => Current = Sensor != null ? Sensor.Value.Value : 0;

        public override string ToString() => converter?.Invoke(UnitType, current, Total, Suffix);

        public static implicit operator float(SensorData value) => value.Current;
        private const int roundValue = 1;
        private static string ConvertUnit(UnitType type, float value, float total = 0, string suffix = "")
        {
            if (type == UnitType.TemperatureC) return Math.Round(value, roundValue) + " °C";
            else if (type == UnitType.TemperatureK) return Math.Round(value, roundValue) + " °K";
            else if (type == UnitType.Data) return SizeSuffix(Convert.ToInt64(value));
            else if (type == UnitType.DataPerSecond) return SizeSuffix(Convert.ToInt64(value))+"/s";
            else if (type == UnitType.Percentile)
            {
                if (total > 0)
                    return Math.Round(value / total * 100, roundValue) + "%";
                else return Math.Round(value, 2) + "%";
            }
            else if (type == UnitType.Custom) return Math.Round(value, roundValue) + suffix;
            return Math.Round(value, roundValue).ToString();
        }

        static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        static readonly long[] Sizes =
        {
            1024, //bytes
            Convert.ToInt64(Math.Pow(1024, 2)), //KB
            Convert.ToInt64(Math.Pow(1024, 3)), //MB
            Convert.ToInt64(Math.Pow(1024, 4)), //GB
            Convert.ToInt64(Math.Pow(1024, 5)), //TB
        };

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static string SizeSuffix(long value)
        {
            int current = Sizes.Length - 1;
            for (int i = 0; i < Sizes.Length; i++)
            {
                if (value < Sizes[i])
                {
                    current = i;
                    break;
                }
            }
            if (current == 0) return value + " B";
            return Math.Round((float)value / Sizes[current - 1], roundValue) + " " + SizeSuffixes[current];
        }
    }
}
