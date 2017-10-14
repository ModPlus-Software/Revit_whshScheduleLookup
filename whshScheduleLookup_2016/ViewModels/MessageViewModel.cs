using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace whshScheduleLookup.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private string _message;
        private string _title;

        public string OkButtonName { get; set; } = "ОК";
        public string NoButtonName { get; set; } = "Нет";
        public string CancelButtonName { get; set; } = "Отмена";
        public bool OkButtonVisibility { get; set; } = true;
        public bool NoButtonVisibility { get; set; }
        public bool CancelButtonVisibility { get; set; } 
        public bool? Result { get; set; }

        public MessageViewModel()
        {
             
        }

        public MessageViewModel(string message)
        {
            Message = message;
        }

        public MessageViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
