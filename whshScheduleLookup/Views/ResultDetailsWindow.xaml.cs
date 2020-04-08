namespace whshScheduleLookup.Views
{
    using ViewModels;

    public partial class ResultDetailsWindow
    {
        private const string LangItem = "whshScheduleLookup";

        public ResultDetailsWindow(ResultDetailsViewModel vm)
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h4");
            DataContext = vm;
        }
    }
}
