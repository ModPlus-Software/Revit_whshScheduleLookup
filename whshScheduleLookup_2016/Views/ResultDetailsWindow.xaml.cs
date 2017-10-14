using System.Windows;
using System.Windows.Input;
using ModPlusAPI.Windows.Helpers;
using whshScheduleLookup.ViewModels;

namespace whshScheduleLookup.Views
{
    public partial class ResultDetailsWindow
    {
        public ResultDetailsWindow(ResultDetailsViewModel vm)
        {
            InitializeComponent();
            this.OnWindowStartUp();
            DataContext = vm;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
