using System.Collections.ObjectModel;
using SystemBuddy;

namespace SystemBuddyUI.ViewModel
{
    public class HardwareViewModel
    {
        public ObservableCollection<SensorViewModel> Sensors { get; } = new ObservableCollection<SensorViewModel>();

        HardwareHandler HardwareHandler = new HardwareHandler();

        public HardwareViewModel()
        {
            HardwareHandler.Initialize();
            foreach (var sensor in HardwareHandler.Sensors)
            {
                Sensors.Add(new SensorViewModel(sensor));
            }
        }
    }
}
