using System.ComponentModel;
using SystemBuddy;

namespace SystemBuddyUI.ViewModel
{
    public class SensorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SensorViewModel(SensorData sensor)
        {
            SensorData = sensor;
            SensorData.PropertyChanged += (sender, property) => PropertyChanged?.Invoke(this, property);
        }

        public string MinValue { get { return SensorData.MinValue; } }

        public string MaxValue { get { return SensorData.MaxValue; } }

        public string CurrentValue { get { return SensorData.CurrentValue; } }

        public string Name { get { return SensorData.Name; } }

        internal SensorData SensorData { get; set; }

    }
}
