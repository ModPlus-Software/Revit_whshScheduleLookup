using System.Windows.Input;
using whshScheduleLookup.ViewModels;

namespace whshScheduleLookup.Views
{
    public partial class ResultDetailsWindow
    {
        private const string LangItem = "whshScheduleLookup";

        public ResultDetailsWindow(ResultDetailsViewModel vm)
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h4");
            DataContext = vm;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
