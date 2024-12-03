using System.ComponentModel;

namespace Dynamic_Lighting_Key_Indicator
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _deviceStatusMessage;
        public string DeviceStatusMessage
        {
            get { return _deviceStatusMessage; }
            set
            {
                if (_deviceStatusMessage != value)
                {
                    _deviceStatusMessage = value;
                    OnPropertyChanged(nameof(DeviceStatusMessage));
                }
            }
        }

        private string _deviceWatcherStatusMessage;
        public string DeviceWatcherStatusMessage
        {
            get { return _deviceWatcherStatusMessage; }
            set
            {
                if (_deviceWatcherStatusMessage != value)
                {
                    _deviceWatcherStatusMessage = value;
                    OnPropertyChanged(nameof(DeviceWatcherStatusMessage));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
