namespace whshScheduleLookup.ViewModels
{
    using ModPlusAPI;
    using ModPlusAPI.Mvvm;

    public class MessageViewModel : VmBase
    {
        private const string LangItem = "whshScheduleLookup";

        private string _message;
        private string _title;

        public string OkButtonName { get; set; } = Language.GetItem(LangItem, "ok");

        public string NoButtonName { get; set; } = Language.GetItem(LangItem, "no");

        public string CancelButtonName { get; set; } = Language.GetItem(LangItem, "cancel");

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
    }
}
