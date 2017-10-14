using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace whshScheduleLookup.Model
{
    public class PropertyValuePair : INotifyPropertyChanged
    {
        public string PropertyName { get; set; }
        public string PropertyDisplayName { get; set; }
        public string PropertyValue { get; set; }
        public string PropertyValueName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
