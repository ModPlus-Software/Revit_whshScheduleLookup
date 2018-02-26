using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using whshScheduleLookup.Model;
using whshScheduleLookup.ViewModels;

namespace whshScheduleLookup.Views
{
    public partial class ResultsWindow
    {
        private const string LangItem = "whshScheduleLookup";

        public ResultsWindow()
        {
            InitializeComponent();
            Title = ModPlusAPI.Language.GetItem(LangItem, "h2");
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;
            if (!(dataGrid.SelectedItem is ViewScheduleSearchResult result)) return;
            var resultDetailsViewModel = new ResultDetailsViewModel(result);
            var resultDetailsWindow = new ResultDetailsWindow(resultDetailsViewModel);

            resultDetailsWindow.Show();
        }

        private void ResultsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = SizeToContent.Manual;
        }
    }
}
