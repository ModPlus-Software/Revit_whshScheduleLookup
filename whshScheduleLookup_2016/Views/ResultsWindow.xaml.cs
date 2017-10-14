using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModPlusAPI.Windows.Helpers;
using whshScheduleLookup.Model;
using whshScheduleLookup.ViewModels;

namespace whshScheduleLookup.Views
{
    public partial class ResultsWindow
    {
        public ResultsWindow()
        {
            InitializeComponent();
            this.OnWindowStartUp();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid dataGrid)) return;
            if (!(dataGrid.SelectedItem is ViewScheduleSearchResult result)) return;
            var resultDetailsViewModel = new ResultDetailsViewModel(result);
            var resultDetailsWindow = new ResultDetailsWindow(resultDetailsViewModel);
            //resultDetailsWindow.Topmost = true;
            //this.Topmost = false;
            resultDetailsWindow.Show();
        }

        private void ResultsWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            SizeToContent = SizeToContent.Manual;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
